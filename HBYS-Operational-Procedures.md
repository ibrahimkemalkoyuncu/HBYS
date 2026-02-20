# HBYS Operational Procedures
## Stored Procedures, Security Headers, Monitoring Thresholds, Backup/Recovery, and Compliance

---

# PART 1: STORED PROCEDURES

## 1.1 Patient Search Stored Procedure

```sql
-- Patient search with pagination and filters
CREATE OR ALTER PROCEDURE [Patient].[sp_SearchPatients]
    @TenantId UNIQUEIDENTIFIER,
    @SearchTerm NVARCHAR(100) = NULL,
    @Status INT = NULL,
    @DateOfBirthFrom DATETIME2 = NULL,
    @DateOfBirthTo DATETIME2 = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @SortBy NVARCHAR(50) = 'CreatedAt',
    @SortOrder NVARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table for results
    CREATE TABLE #Results (
        RowNumber INT,
        Id UNIQUEIDENTIFIER,
        PatientNumber NVARCHAR(20),
        FirstName NVARCHAR(100),
        LastName NVARCHAR(100),
        DateOfBirth DATETIME2,
        Gender INT,
        Status INT,
        PrimaryPhone NVARCHAR(500),
        CreatedAt DATETIME2,
        TotalCount INT
    );

    -- Main query
    INSERT INTO #Results
    SELECT 
        ROW_NUMBER() OVER (ORDER BY 
            CASE WHEN @SortBy = 'name' AND @SortOrder = 'asc' THEN p.LastName END ASC,
            CASE WHEN @SortBy = 'name' AND @SortOrder = 'desc' THEN p.LastName END DESC,
            CASE WHEN @SortBy = 'createdAt' AND @SortOrder = 'asc' THEN p.CreatedAt END ASC,
            CASE WHEN @SortBy = 'createdAt' AND @SortOrder = 'desc' THEN p.CreatedAt END DESC,
            CASE WHEN @SortBy = 'dob' AND @SortOrder = 'asc' THEN p.DateOfBirth END ASC,
            CASE WHEN @SortBy = 'dob' AND @SortOrder = 'desc' THEN p.DateOfBirth END DESC
        ) AS RowNumber,
        p.Id,
        p.PatientNumber,
        p.FirstName,
        p.LastName,
        p.DateOfBirth,
        p.Gender,
        p.Status,
        (SELECT TOP 1 pc.Value FROM [Patient].[PatientContacts] pc 
         WHERE pc.PatientId = p.Id AND pc.ContactType = 1 AND pc.IsPrimary = 1) AS PrimaryPhone,
        p.CreatedAt,
        COUNT(*) OVER() AS TotalCount
    FROM [Patient].[Patients] p
    WHERE p.TenantId = @TenantId
        AND (@SearchTerm IS NULL OR 
             p.FirstName LIKE '%' + @SearchTerm + '%' OR 
             p.LastName LIKE '%' + @SearchTerm + '%' OR 
             p.PatientNumber LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR p.Status = @Status)
        AND (@DateOfBirthFrom IS NULL OR p.DateOfBirth >= @DateOfBirthFrom)
        AND (@DateOfBirthTo IS NULL OR p.DateOfBirth <= @DateOfBirthTo)
    ORDER BY RowNumber;

    -- Return paginated results
    SELECT 
        Id,
        PatientNumber,
        FirstName,
        LastName,
        DateOfBirth,
        Gender,
        Status,
        PrimaryPhone,
        CreatedAt,
        TotalCount
    FROM #Results
    WHERE RowNumber > (@PageNumber - 1) * @PageSize
      AND RowNumber <= @PageNumber * @PageSize;

    -- Return total count
    SELECT TOP 1 TotalCount FROM #Results;
END;
```

## 1.2 Appointment Availability Stored Procedure

```sql
-- Get available appointment slots
CREATE OR ALTER PROCEDURE [Appointment].[sp_GetAvailableSlots]
    @TenantId UNIQUEIDENTIFIER,
    @PhysicianId UNIQUEIDENTIFIER,
    @DepartmentId UNIQUEIDENTIFIER,
    @Date DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DayOfWeek INT = DATEPART(WEEKDAY, @Date);
    
    -- Get physician's regular slots for the day
    SELECT 
        s.StartTime,
        s.EndTime,
        s.SlotDuration,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM [Appointment].[Appointments] a
                WHERE a.PhysicianId = @PhysicianId
                  AND a.AppointmentDate = @Date
                  AND a.Status NOT IN (4, 5) -- Not Cancelled or NoShow
                  AND ((a.StartTime <= s.StartTime AND a.EndTime > s.StartTime)
                    OR (a.StartTime < s.EndTime AND a.EndTime >= s.EndTime)
                    OR (a.StartTime >= s.StartTime AND a.EndTime <= s.EndTime))
            ) THEN 0
            ELSE 1
        END AS IsAvailable
    FROM [Appointment].[AppointmentSlots] s
    WHERE s.PhysicianId = @PhysicianId
      AND s.DepartmentId = @DepartmentId
      AND s.DayOfWeek = @DayOfWeek
      AND s.IsActive = 1
      AND s.TenantId = @TenantId
    ORDER BY s.StartTime;
END;
```

## 1.3 Invoice Generation Stored Procedure

```sql
-- Generate invoice with items
CREATE OR ALTER PROCEDURE [Billing].[sp_GenerateInvoice]
    @TenantId UNIQUEIDENTIFIER,
    @PatientId UNIQUEIDENTIFIER,
    @InvoiceType INT,
    @DueDate DATE = NULL,
    @CreatedBy UNIQUEIDENTIFIER,
    @InvoiceId UNIQUEIDENTIFIER OUTPUT,
    @InvoiceNumber NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Generate invoice number
        DECLARE @Year NVARCHAR(4) = YEAR(GETDATE());
        DECLARE @Sequence INT;
        
        SELECT @Sequence = ISNULL(MAX(CAST(RIGHT(InvoiceNumber, 5) AS INT)), 0) + 1
        FROM [Billing].[Invoices]
        WHERE TenantId = @TenantId 
          AND InvoiceNumber LIKE 'INV-' + @Year + '-%';

        SET @InvoiceNumber = 'INV-' + @Year + '-' + RIGHT('00000' + CAST(@Sequence AS NVARCHAR(5)), 5);

        -- Create invoice header
        SET @InvoiceId = NEWID();

        INSERT INTO [Billing].[Invoices] (
            Id, InvoiceNumber, PatientId, InvoiceType, InvoiceDate, DueDate,
            SubTotal, TaxAmount, DiscountAmount, TotalAmount, Currency,
            Status, PaymentStatus, TenantId, CreatedBy, CreatedAt
        )
        VALUES (
            @InvoiceId, @InvoiceNumber, @PatientId, @InvoiceType, GETDATE(), @DueDate,
            0, 0, 0, 0, 'TRY',
            1, 1, @TenantId, @CreatedBy, GETUTCDATE()
        );

        COMMIT TRANSACTION;

        -- Return the generated values
        SELECT @InvoiceId AS InvoiceId, @InvoiceNumber AS InvoiceNumber;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END;
```

---

# PART 2: SECURITY HEADERS

## 2.1 Security Headers Middleware

```csharp
/// <summary>
/// Security headers middleware.
/// Ne: HTTP response güvenlik header'larını ekleyen middleware.
/// Neden: XSS, clickjacking, ve diğer saldırılara karşı koruma için gereklidir.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. X-Content-Type-Options
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // 2. X-Frame-Options
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // 3. X-XSS-Protection
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // 4. Strict-Transport-Security (HSTS)
        var hstsMaxAge = _configuration["Security:HSTS:MaxAge"] ?? "31536000"; // 1 year
        context.Response.Headers["Strict-Transport-Security"] = $"max-age={hstsMaxAge}; includeSubDomains";

        // 5. Content-Security-Policy
        var csp = BuildContentSecurityPolicy();
        context.Response.Headers["Content-Security-Policy"] = csp;

        // 6. Referrer-Policy
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // 7. Permissions-Policy
        context.Response.Headers["Permissions-Policy"] = 
            "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

        // 8. Remove server header
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        await _next(context);
    }

    private string BuildContentSecurityPolicy()
    {
        var allowedOrigins = _configuration.GetSection("Security:CSP:AllowedOrigins").Get<string[]>() 
            ?? new[] { "https://hbys.com.tr", "https://www.hbys.com.tr" };
        
        var allowedSources = string.Join(" ", allowedOrigins);
        
        return $"default-src 'self'; " +
               $"script-src 'self' 'unsafe-inline' 'unsafe-eval' {allowedSources}; " +
               $"style-src 'self' 'unsafe-inline' {allowedSources}; " +
               $"img-src 'self' data: https: {allowedSources}; " +
               $"font-src 'self' {allowedSources}; " +
               $"connect-src 'self' {allowedSources}; " +
               $"frame-ancestors 'none'; " +
               $"form-action 'self'; " +
               $"base-uri 'self';";
    }
}
```

## 2.2 CORS Configuration

```csharp
/// <summary>
/// CORS configuration.
/// Ne: Cross-Origin Resource Sharing yapılandırması.
/// Neden: API güvenliği için gereklidir.
/// </summary>
public static class CorsConfiguration
{
    public static IServiceCollection AddHbysCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "https://hbys.com.tr", "https://www.hbys.com.tr" };

        services.AddCors(options =>
        {
            options.AddPolicy("HbysCorsPolicy", builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
                    .WithExposedHeaders("X-Correlation-ID", "X-Total-Count", "X-Page-Number");
            });
        });

        return services;
    }
}
```

---

# PART 3: MONITORING THRESHOLDS

## 3.1 Health Check Thresholds

```csharp
/// <summary>
/// Health check threshold constants.
/// Ne: Sağlık kontrolü eşik değerleri.
/// Neden: Monitoring için gereklidir.
/// </summary>
public static class HealthThresholds
{
    // Database thresholds
    public static readonly TimeSpan DatabaseTimeoutThreshold = TimeSpan.FromSeconds(5);
    public static readonly int DatabaseConnectionPoolMax = 100;
    public static readonly int DatabaseConnectionPoolMin = 10;

    // Redis thresholds
    public static readonly TimeSpan RedisTimeoutThreshold = TimeSpan.FromSeconds(2);
    public static readonly int RedisMaxRetries = 3;

    // API response time thresholds
    public static readonly TimeSpan ApiResponseTimeP95 = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan ApiResponseTimeP99 = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan ApiResponseTimeCritical = TimeSpan.FromSeconds(5);

    // CPU/Memory thresholds
    public static readonly double CpuWarningThreshold = 70.0; // percent
    public static readonly double CpuCriticalThreshold = 90.0;
    public static readonly double MemoryWarningThreshold = 75.0;
    public static readonly double MemoryCriticalThreshold = 90.0;

    // Queue thresholds
    public static readonly int QueueDepthWarning = 100;
    public static readonly int QueueDepthCritical = 500;

    // Error rate thresholds
    public static readonly double ErrorRateWarning = 1.0; // percent
    public static readonly double ErrorRateCritical = 5.0;

    // Custom health check descriptions
    public static readonly Dictionary<string, string> HealthCheckDescriptions = new()
    {
        ["database"] = "Veritabanı bağlantısı ve performansı kontrol eder",
        ["redis"] = "Redis önbellek bağlantısı kontrol eder",
        ["license"] = "Lisans durumunu kontrol eder",
        ["disk"] = "Disk kullanımını kontrol eder",
        ["memory"] = "Bellek kullanımını kontrol eder"
    };
}
```

## 3.2 Alert Rules

```yaml
# Prometheus alert rules
groups:
- name: hbys-alerts
  interval: 30s
  rules:
  # API alerts
  - alert: HBYSApiHighResponseTime
    expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket{job="hbys-api"}[5m])) > 0.5
    for: 5m
    labels:
      severity: warning
      component: api
    annotations:
      summary: "API response time is high"
      description: "95th percentile response time is {{ $value }}s for {{ $labels.endpoint }}"

  - alert: HBYSApiHighErrorRate
    expr: rate(http_requests_total{status=~"5..",job="hbys-api"}[5m]) > 0.01
    for: 2m
    labels:
      severity: critical
      component: api
    annotations:
      summary: "API error rate is high"
      description: "Error rate is {{ $value | humanizePercentage }} for {{ $labels.endpoint }}"

  # Database alerts
  - alert: HBYSDatabaseConnectionPoolExhausted
    expr: sqlserver_num_backups / sqlserver_num_available > 0.9
    for: 2m
    labels:
      severity: critical
      component: database
    annotations:
      summary: "Database connection pool is exhausted"
      description: "Connection pool usage is {{ $value | humanizePercentage }}"

  # Application alerts
  - alert: HBYSApplicationDown
    expr: up{job="hbys-api"} == 0
    for: 1m
    labels:
      severity: critical
      component: application
    annotations:
      summary: "Application is down"
      description: "HBYS API is not responding"

  - alert: HBYSLicenseExpiring
    expr: days_until_license_expiry < 30
    for: 1h
    labels:
      severity: warning
      component: license
    annotations:
      summary: "License is expiring"
      description: "License will expire in {{ $value }} days"
```

---

# PART 4: BACKUP AND RECOVERY

## 4.1 Backup Strategy

```sql
-- Full backup schedule (daily at 2:00 AM)
-- Differential backup (every 6 hours)
-- Transaction log backup (every 15 minutes)

-- Full backup stored procedure
CREATE OR ALTER PROCEDURE [Maintenance].[sp_FullBackup]
    @BackupPath NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BackupFileName NVARCHAR(500);
    DECLARE @DatabaseName NVARCHAR(100) = DB_NAME();

    SET @BackupFileName = @BackupPath + '\' + @DatabaseName + '_Full_' + 
        CONVERT(NVARCHAR(10), GETDATE(), 112) + '_' + 
        REPLACE(CONVERT(NVARCHAR(8), GETDATE(), 108), ':', '') + '.bak';

    BACKUP DATABASE @DatabaseName
    TO DISK = @BackupFileName
    WITH COMPRESSION, 
         CHECKSUM, 
         STATS = 10,
         FORMAT;

    -- Verify backup
    RESTORE VERIFYONLY 
    FROM DISK = @BackupFileName;

    RETURN @@ERROR;
END;
```

## 4.2 Recovery Procedures

```sql
-- Point-in-time recovery
CREATE OR ALTER PROCEDURE [Maintenance].[sp_RestoreToPointInTime]
    @BackupPath NVARCHAR(500),
    @PointInTime DATETIME2,
    @RestorePath NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DatabaseName NVARCHAR(100) = DB_NAME();
    DECLARE @LatestFullBackup NVARCHAR(500);
    DECLARE @LatestDiffBackup NVARCHAR(500);
    DECLARE @LogBackups TABLE (BackupFile NVARCHAR(500), Position INT);

    -- Find latest full backup
    SELECT TOP 1 @LatestFullBackup = bs.physical_device_name
    FROM msdb.dbo.backupset bs
    WHERE bs.database_name = @DatabaseName
      AND bs.type = 'D'
      AND bs.backup_start_date <= @PointInTime
    ORDER BY bs.backup_start_date DESC;

    -- Find differential backups after full backup
    SELECT TOP 1 @LatestDiffBackup = bs.physical_device_name
    FROM msdb.dbo.backupset bs
    WHERE bs.database_name = @DatabaseName
      AND bs.type = 'I'
      AND bs.backup_start_date > (SELECT backup_start_date FROM msdb.dbo.backupset WHERE physical_device_name = @LatestFullBackup)
      AND bs.backup_start_date <= @PointInTime
    ORDER BY bs.backup_start_date DESC;

    -- Restore full backup
    RESTORE DATABASE @DatabaseName
    FROM DISK = @LatestFullBackup
    WITH MOVE @DatabaseName TO @RestorePath + '\' + @DatabaseName + '.mdf',
         MOVE @DatabaseName + '_Log' TO @RestorePath + '\' + @DatabaseName + '_Log.ldf',
         NORECOVERY;

    -- Restore differential if exists
    IF @LatestDiffBackup IS NOT NULL
    BEGIN
        RESTORE DATABASE @DatabaseName
        FROM DISK = @LatestDiffBackup
        WITH NORECOVERY;
    END

    -- Restore log backups up to point in time
    INSERT INTO @LogBackups
    SELECT bs.physical_device_name, bs.position
    FROM msdb.dbo.backupset bs
    WHERE bs.database_name = @DatabaseName
      AND bs.type = 'L'
      AND bs.backup_start_date > ISNULL((SELECT backup_start_date FROM msdb.dbo.backupset WHERE physical_device_name = @LatestDiffBackup), 
                                         (SELECT backup_start_date FROM msdb.dbo.backupset WHERE physical_device_name = @LatestFullBackup))
      AND bs.backup_start_date <= @PointInTime
    ORDER BY bs.backup_start_date;

    DECLARE @LogFile NVARCHAR(500);
    DECLARE @Position INT;
    
    DECLARE log_cursor CURSOR FOR 
        SELECT BackupFile, Position FROM @LogBackups;
    
    OPEN log_cursor;
    FETCH NEXT FROM log_cursor INTO @LogFile, @Position;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        RESTORE LOG @DatabaseName
        FROM DISK = @LogFile
        WITH STOPAT = @PointInTime, NORECOVERY;
        
        FETCH NEXT FROM log_cursor INTO @LogFile, @Position;
    END
    
    CLOSE log_cursor;
    DEALLOCATE log_cursor;

    -- Make database accessible
    RESTORE DATABASE @DatabaseName WITH RECOVERY;
END;
```

---

# PART 5: RATE LIMITING

## 5.1 Rate Limiting Rules

```csharp
/// <summary>
/// Rate limiting configuration.
/// Ne: Rate limiting kuralları yapılandırması.
/// Neden: API abuse önleme için gereklidir.
/// </summary>
public static class RateLimitRules
{
    public static readonly Dictionary<string, RateLimitRule> Rules = new()
    {
        // Authentication endpoints - strict limits
        ["auth:login"] = new RateLimitRule
        {
            PermitsPerMinute = 5,
            PermitsPerHour = 20,
            Burst = 3,
            Window = TimeSpan.FromMinutes(1)
        },
        
        ["auth:forgot-password"] = new RateLimitRule
        {
            PermitsPerMinute = 3,
            PermitsPerHour = 10,
            Burst = 1,
            Window = TimeSpan.FromMinutes(1)
        },

        // Patient search - moderate limits
        ["patients:search"] = new RateLimitRule
        {
            PermitsPerMinute = 60,
            PermitsPerHour = 500,
            Burst = 20,
            Window = TimeSpan.FromMinutes(1)
        },

        // Appointment creation - moderate limits
        ["appointments:create"] = new RateLimitRule
        {
            PermitsPerMinute = 30,
            PermitsPerHour = 200,
            Burst = 10,
            Window = TimeSpan.FromMinutes(1)
        },

        // Read operations - generous limits
        ["appointments:get"] = new RateLimitRule
        {
            PermitsPerMinute = 120,
            PermitsPerHour = 1000,
            Burst = 50,
            Window = TimeSpan.FromMinutes(1)
        },

        // Export/print operations - strict limits
        ["reports:export"] = new RateLimitRule
        {
            PermitsPerMinute = 10,
            PermitsPerHour = 50,
            Burst = 5,
            Window = TimeSpan.FromMinutes(1)
        },

        // Default rule
        ["default"] = new RateLimitRule
        {
            PermitsPerMinute = 100,
            PermitsPerHour = 500,
            Burst = 30,
            Window = TimeSpan.FromMinutes(1)
        }
    };
}

public class RateLimitRule
{
    public int PermitsPerMinute { get; set; }
    public int PermitsPerHour { get; set; }
    public int Burst { get; set; }
    public TimeSpan Window { get; set; }
}
```

---

# PART 6: CIRCUIT BREAKER

## 6.1 Circuit Breaker Configuration

```csharp
/// <summary>
/// Circuit breaker configuration.
/// Ne: Circuit breaker yapılandırması.
/// Neden: Cascade failure önleme için gereklidir.
/// </summary>
public static class CircuitBreakerConfig
{
    public static readonly CircuitBreakerPolicyConfig ExternalServices = new()
    {
        FailureThreshold = 5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 3,
        BreakDuration = TimeSpan.FromSeconds(30),
        OnBreak = (exception, duration) => 
        {
            Log.Warning("Circuit breaker opened for {Duration} due to {Exception}", 
                duration, exception.Message);
        },
        OnReset = () => 
        {
            Log.Information("Circuit breaker reset");
        }
    };

    public static readonly CircuitBreakerPolicyConfig Database = new()
    {
        FailureThreshold = 3,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 2,
        BreakDuration = TimeSpan.FromSeconds(10)
    };

    public static readonly CircuitBreakerPolicyConfig Redis = new()
    {
        FailureThreshold = 2,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 2,
        BreakDuration = TimeSpan.FromSeconds(5)
    };
}

public class CircuitBreakerPolicyConfig
{
    public int FailureThreshold { get; set; }
    public TimeSpan SamplingDuration { get; set; }
    public int MinimumThroughput { get; set; }
    public TimeSpan BreakDuration { get; set; }
    public Action<Exception, TimeSpan>? OnBreak { get; set; }
    public Action? OnReset { get; set; }
}
```

---

# PART 7: RETRY POLICY

## 7.1 Retry Configuration

```csharp
/// <summary>
/// Retry policy configuration.
/// Ne: Yeniden deneme politikası yapılandırması.
/// Neden: Geçici hatalar için gereklidir.
/// </summary>
public static class RetryPolicyConfig
{
    // Database retry policy
    public static readonly RetryPolicy DatabaseRetry = new()
    {
        MaxRetryCount = 3,
        Delay = TimeSpan.FromMilliseconds(200),
        MaxDelay = TimeSpan.FromSeconds(2),
        ExponentialBackoff = true,
        Jitter = true,
        RetryableExceptions = new[]
        {
            typeof(SqlException),
            typeof(TimeoutException),
            typeof(IOException)
        }
    };

    // External API retry policy
    public static readonly RetryPolicy ExternalApiRetry = new()
    {
        MaxRetryCount = 5,
        Delay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30),
        ExponentialBackoff = true,
        Jitter = true,
        RetryableExceptions = new[]
        {
            typeof(HttpRequestException),
            typeof(TimeoutException),
            typeof(TaskCanceledException)
        },
        RetryableStatusCodes = new[]
        {
            HttpStatusCode.RequestTimeout,           // 408
            HttpStatusCode.TooManyRequests,          // 429
            HttpStatusCode.InternalServerError,      // 500
            HttpStatusCode.BadGateway,               // 502
            HttpStatusCode.ServiceUnavailable,       // 503
            HttpStatusCode.GatewayTimeout            // 504
        }
    };

    // Redis retry policy
    public static readonly RetryPolicy RedisRetry = new()
    {
        MaxRetryCount = 3,
        Delay = TimeSpan.FromMilliseconds(100),
        MaxDelay = TimeSpan.FromSeconds(1),
        ExponentialBackoff = true,
        Jitter = true,
        RetryableExceptions = new[]
        {
            typeof(RedisConnectionException),
            typeof(RedisTimeoutException)
        }
    };
}

public class RetryPolicy
{
    public int MaxRetryCount { get; set; }
    public TimeSpan Delay { get; set; }
    public TimeSpan MaxDelay { get; set; }
    public bool ExponentialBackoff { get; set; }
    public bool Jitter { get; set; }
    public Type[] RetryableExceptions { get; set; } = Array.Empty<Type>();
    public HttpStatusCode[]? RetryableStatusCodes { get; set; }
}
```

---

# PART 8: KVKK COMPLIANCE

## 8.1 KVKK Data Processing

```csharp
/// <summary>
/// KVKK compliance constants.
/// Ne: KVKK uyumluluk sabitleri.
/// Neden: Türkiye veri koruma kanunu için gereklidir.
/// </summary>
public static class KvkkConstants
{
    // Data Subject Rights
    public static readonly Dictionary<string, string> DataSubjectRights = new()
    {
        ["right_to_information"] = "Aydınlatma hakkı",
        ["right_to_access"] = "Erişim hakkı",
        ["right_to_rectification"] = "Düzeltme hakkı",
        ["right_to_erasure"] = "Silme hakkı",
        ["right_to_restrict_processing"] = "İşlemenin kısıtlanması hakkı",
        ["right_to_data_portability"] = "Veri taşınabilirliği hakkı",
        ["right_to_object"] = "İtiraz hakkı"
    };

    // Data Categories
    public static readonly Dictionary<string, string> DataCategories = new()
    {
        ["personal_data"] = "Kişisel veri",
        ["sensitive_data"] = "Özel nitelikli kişisel veri",
        ["health_data"] = "Sağlık verisi",
        ["biometric_data"] = "Biyometrik veri"
    };

    // Processing Purposes
    public static readonly Dictionary<string, string> ProcessingPurposes = new()
    {
        ["treatment"] = "Tedavi hizmetlerinin sunulması",
        ["payment"] = "Ödeme ve faturalama",
        ["appointment"] = "Randevu yönetimi",
        ["reporting"] = "Yasal raporlama",
        ["research"] = "Araştırma ve eğitim"
    };

    // Retention Periods (in months)
    public static readonly Dictionary<string, int> RetentionPeriods = new()
    {
        ["patient_record"] = 120,  // 10 years
        ["appointment_record"] = 60,  // 5 years
        ["billing_record"] = 120,  // 10 years
        ["audit_log"] = 60,  // 5 years
        ["employee_record"] = 180,  // 15 years after termination
        ["insurance_record"] = 120  // 10 years
    };

    // Consent Types
    public static readonly Dictionary<string, string> ConsentTypes = new()
    {
        ["explicit"] = "Açık rıza",
        ["implicit"] = "Örtülü rıza",
        ["legal_basis"] = "Hukuki dayanak"
    };
}
```

---

Bu dokümantasyon, HBYS sisteminin operasyonel prosedürlerini içermektedir. Stored procedures, security headers, monitoring thresholds, backup/recovery, rate limiting, circuit breaker, retry policy, ve KVKK compliance dahildir.