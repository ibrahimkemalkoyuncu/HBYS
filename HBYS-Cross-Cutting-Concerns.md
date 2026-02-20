# HBYS Cross-Cutting Concerns
## Logging, Configuration, Feature Management, Localization, Background Jobs, and API Documentation

---

# PART 1: LOGGING INFRASTRUCTURE

## 1.1 Serilog Configuration

```csharp
/// <summary>
/// Serilog configuration builder.
/// Ne: Serilog yapılandırma builder'ı.
/// Neden: Structured logging için gereklidir.
/// </summary>
public static class SerilogConfiguration
{
    public static IHostBuilder UseHbysSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        var logSettings = configuration.GetSection("Serilog").Get<SerilogSettings>();
        
        builder.UseSerilog((context, services, loggerConfig) =>
        {
            loggerConfig
                .MinimumLevel.Is(logSettings?.MinimumLevel ?? LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.WithProperty("Application", "HBYS")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .Enrich.WithHttpRequestInfo()
                .WriteTo.Console(outputTemplate: 
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logSettings?.FilePath ?? "logs/hbys-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: logSettings?.RetainedFileCount ?? 30,
                    fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Debug()
                .WriteTo.Seq(logSettings?.SeqUrl ?? "http://localhost:5341")
                .WriteTo.ApplicationInsights(
                    services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces);
        });

        return builder;
    }
}

/// <summary>
/// Log enrichment for HTTP request.
/// Ne: HTTP request için log enrichment.
/// </summary>
public class HttpRequestInfoEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = GetHttpContext();
        
        if (httpContext != null)
        {
            var request = httpContext.Request;
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", request.Method));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", request.Path));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestQueryString", request.QueryString.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestHost", request.Host.ToString()));
            
            var tenantId = httpContext.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TenantId", tenantId));
            }
            
            var userId = httpContext.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
            }
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", 
                httpContext.TraceIdentifier));
        }
    }
    
    private HttpContext? GetHttpContext()
    {
        var httpContextAccessor = ServiceProviderServiceExtensions.GetService<IHttpContextAccessor>(
            ServiceProviderServiceExtensions.ServiceProvider);
        return httpContextAccessor?.HttpContext;
    }
}
```

---

# PART 2: CONFIGURATION MANAGEMENT

## 2.1 Options Pattern Implementation

```csharp
/// <summary>
/// Application settings.
/// Ne: Uygulama ayarları.
/// </summary>
public class AppSettings
{
    public JwtSettings Jwt { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
    public EmailSettings Email { get; set; } = new();
    public SmsSettings Sms { get; set; } = new();
    public StorageSettings Storage { get; set; } = new();
    public PaymentSettings Payment { get; set; } = new();
    public FeatureFlags Features { get; set; } = new();
}

/// <summary>
/// JWT settings.
/// Ne: JWT ayarları.
/// </summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public string Algorithm { get; set; } = "HS256";
}

/// <summary>
/// Redis settings.
/// Ne: Redis ayarları.
/// </summary>
public class RedisSettings
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public int DatabaseNumber { get; set; } = 0;
    public int DefaultCacheMinutes { get; set; } = 30;
    public bool AbortOnConnectFail { get; set; } = true;
}

/// <summary>
/// Feature flags.
/// Ne: Feature flag'ler.
/// </summary>
public class FeatureFlags
{
    public bool EnableGraphQL { get; set; }
    public bool EnableSignalR { get; set; }
    public bool EnablePaymentGateway { get; set; }
    public bool EnableEmailNotifications { get; set; }
    public bool EnableSmsNotifications { get; set; }
    public bool EnableLabInterface { get; set; }
    public bool EnablePacsInterface { get; set; }
    public bool EnableHIE { get; set; } // Health Information Exchange
}

/// <summary>
/// Options validation.
/// Ne: Options validation.
/// </summary>
public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(options.Jwt.SecretKey) || options.Jwt.SecretKey.Length < 32)
        {
            errors.Add("JWT SecretKey must be at least 32 characters");
        }
        
        if (options.Jwt.AccessTokenExpirationMinutes < 5)
        {
            errors.Add("JWT AccessTokenExpirationMinutes must be at least 5");
        }
        
        if (errors.Any())
        {
            return ValidateOptionsResult.Fail(errors);
        }
        
        return ValidateOptionsResult.Success;
    }
}
```

---

# PART 3: FEATURE MANAGEMENT

## 3.1 Feature Flag Implementation

```csharp
/// <summary>
/// Feature flag service.
/// Ne: Feature flag servisi.
/// </summary>
public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string featureFlag, Guid tenantId);
    Task<Dictionary<string, bool>> GetAllFlagsAsync(Guid tenantId);
    Task SetFlagAsync(string featureFlag, bool enabled, Guid tenantId);
}

/// <summary>
/// Feature flag attributes.
/// Ne: Feature flag attribute'leri.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FeatureFlagAttribute : Attribute
{
    public string FeatureFlag { get; }
    public bool DefaultValue { get; }
    
    public FeatureFlagAttribute(string featureFlag, bool defaultValue = false)
    {
        FeatureFlag = featureFlag;
        DefaultValue = defaultValue;
    }
}

/// <summary>
/// Feature flag middleware.
/// Ne: Feature flag middleware.
/// </summary>
public class FeatureFlagMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFeatureFlagService _featureFlagService;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        
        if (endpoint != null)
        {
            var featureFlagAttr = endpoint.Metadata.GetMetadata<FeatureFlagAttribute>();
            
            if (featureFlagAttr != null)
            {
                var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
                
                if (Guid.TryParse(tenantIdClaim, out var tenantId))
                {
                    var isEnabled = await _featureFlagService.IsEnabledAsync(
                        featureFlagAttr.FeatureFlag, tenantId);
                    
                    if (!isEnabled)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                            title = "Feature Not Available",
                            status = 404,
                            detail = $"The feature '{featureFlagAttr.FeatureFlag}' is not available"
                        });
                        return;
                    }
                }
            }
        }
        
        await _next(context);
    }
}
```

---

# PART 4: LOCALIZATION

## 4.1 Localization Implementation

```csharp
/// <summary>
/// Localization service.
/// Ne: Localization servisi.
/// </summary>
public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] args);
    string GetString(string key, string culture);
}

/// <summary>
/// Turkish localization resources.
/// Ne: Türkçe lokalizasyon kaynakları.
/// </summary>
public static class TurkishStrings
{
    // Common
    public const string Save = "Kaydet";
    public const string Cancel = "İptal";
    public const string Delete = "Sil";
    public const string Edit = "Düzenle";
    public const string Add = "Ekle";
    public const string Search = "Ara";
    public const string List = "Liste";
    public const string Detail = "Detay";
    
    // Patient
    public const string Patient = "Hasta";
    public const string PatientNumber = "Hasta Numarası";
    public const string FirstName = "Adı";
    public const string LastName = "Soyadı";
    public const string DateOfBirth = "Doğum Tarihi";
    public const string Gender = "Cinsiyet";
    public const string Male = "Erkek";
    public const string Female = "Kadın";
    public const string BloodType = "Kan Grubu";
    
    // Status
    public const string Active = "Aktif";
    public const string Inactive = "Pasif";
    public const string Pending = "Beklemede";
    public const string Completed = "Tamamlandı";
    public const string Cancelled = "İptal Edildi";
    
    // Validation
    public const string RequiredField = "Bu alan zorunludur";
    public const string InvalidFormat = "Geçersiz format";
    public const string MinLength = "En az {0} karakter olmalıdır";
    public const string MaxLength = "En fazla {0} karakter olmalıdır";
}
```

---

# PART 5: BACKGROUND JOBS

## 5.1 Hangfire Configuration

```csharp
/// <summary>
/// Background job service.
/// Ne: Background job servisi.
/// </summary>
public static class BackgroundJobConfiguration
{
    public static IServiceCollection AddHbysBackgroundJobs(this IServiceCollection services)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage("ConnectionString"));
            
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
            options.Queues = new[] { "critical", "default", "low" };
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseHbysBackgroundJobs(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthFilter() },
            StatsPollingInterval = 10000
        });
        
        return app;
    }
}

/// <summary>
/// Recurring jobs registration.
/// Ne: Tekrarlayan işlerin kaydedilmesi.
/// </summary>
public static class RecurringJobs
{
    public static void RegisterRecurringJobs()
    {
        // Appointment reminder - every 30 minutes
        RecurringJob.AddOrUpdate<AppointmentReminderJob>(
            "appointment-reminder",
            job => job.ExecuteAsync(default),
            "*/30 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"),
                MisfireHandling = MisfireHandlingMode.Relaxed
            });
            
        // License expiration check - daily at 00:00
        RecurringJob.AddOrUpdate<LicenseExpirationJob>(
            "license-expiration-check",
            job => job.ExecuteAsync(default),
            "0 0 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
            });
            
        // Invoice overdue check - daily at 06:00
        RecurringJob.AddOrUpdate<InvoiceOverdueJob>(
            "invoice-overdue-check",
            job => job.ExecuteAsync(default),
            "0 6 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
            });
            
        // Cache warmup - daily at 05:00
        RecurringJob.AddOrUpdate<CacheWarmupJob>(
            "cache-warmup",
            job => job.ExecuteAsync(default),
            "0 5 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
            });
    }
}
```

---

# PART 6: API DOCUMENTATION

## 6.1 Swagger Configuration

```csharp
/// <summary>
/// Swagger configuration.
/// Ne: Swagger yapılandırması.
/// </summary>
public static class SwaggerConfiguration
{
    public static IServiceCollection AddHbysSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HBYS API",
                Description = "Hastane Bilgi Yönetim Sistemi Web API\n\n" +
                    "### Authentication\n" +
                    "Bearer token authentication kullanılmaktadır.\n\n" +
                    "### Tenant Isolation\n" +
                    "Tüm isteklerde `X-Tenant-Id` header'ı gerekmektedir.",
                Version = "v1.0",
                Contact = new OpenApiContact
                {
                    Name = "HBYS Support",
                    Email = "support@hbys.com.tr"
                },
                License = new OpenApiLicense
                {
                    Name = "HBYS License"
                }
            });
            
            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            
            // Tenant ID header
            c.AddSecurityDefinition("TenantId", new OpenApiSecurityScheme
            {
                Description = "Tenant ID (GUID)",
                Name = "X-Tenant-Id",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            
            // XML Comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
            
            // Custom schema filters
            c.SchemaFilter<RequiredPropertiesSchemaFilter>();
            c.SchemaFilter<EnumSchemaFilter>();
            
            // Operation filters
            c.OperationFilter<AddRequiredHeaderParameter>();
            c.OperationFilter<TenantIdOperationFilter>();
        });
        
        return services;
    }
}

/// <summary>
/// Operation filter for tenant ID.
/// Ne: Tenant ID operation filter.
/// </summary>
public class TenantIdOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Tenant-Id",
            In = ParameterLocation.Header,
            Description = "Tenant ID (required)",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid"
            }
        });
    }
}
```

---

# PART 7: RESPONSE CACHING

## 7.1 Output Caching

```csharp
/// <summary>
/// Output caching attribute.
/// Ne: Output caching attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CacheAttribute : Attribute
{
    public string? Key { get; set; }
    public int DurationSeconds { get; set; } = 60;
    public string[]? VaryByHeaders { get; set; }
    public string[]? VaryByQueryKeys { get; set; }
}

/// <summary>
/// Response cache service.
/// Ne: Response cache servisi.
/// </summary>
public class ResponseCacheService : IResponseCacheService
{
    private readonly IRedisCache _cache;
    private readonly ILogger<ResponseCacheService> _logger;
    
    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan duration)
    {
        if (response == null) return;
        
        var serialized = JsonSerializer.Serialize(response);
        await _cache.SetStringAsync(cacheKey, serialized, duration);
        
        _logger.LogDebug("Cached response for key: {CacheKey}", cacheKey);
    }
    
    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        return await _cache.GetStringAsync(cacheKey);
    }
    
    public async Task InvalidateCacheAsync(string pattern)
    {
        var keys = await _cache.KeysAsync(pattern);
        foreach (var key in keys)
        {
            await _cache.DeleteAsync(key);
        }
        
        _logger.LogInformation("Invalidated cache keys matching: {Pattern}", pattern);
    }
}
```

---

# PART 8: DISTRIBUTED LOCKING

## 8.1 Distributed Lock Implementation

```csharp
/// <summary>
/// Distributed lock service.
/// Ne: Distributed lock servisi.
/// </summary>
public interface IDistributedLock
{
    Task<bool> TryAcquireAsync(string key, TimeSpan expiration);
    Task ReleaseAsync(string key);
}

/// <summary>
/// Redis-based distributed lock.
/// Ne: Redis tabanlı distributed lock.
/// </summary>
public class RedisDistributedLock : IDistributedLock
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisDistributedLock> _logger;
    
    public async Task<bool> TryAcquireAsync(string key, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock:{key}";
        
        var acquired = await db.StringSetAsync(lockKey, 
            Environment.MachineName, 
            expiration, 
            When.NotExists);
        
        if (acquired)
        {
            _logger.LogDebug("Lock acquired: {LockKey}", lockKey);
        }
        
        return acquired;
    }
    
    public async Task ReleaseAsync(string key)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock:{key}";
        
        var value = await db.StringGetAsync(lockKey);
        if (value == Environment.MachineName)
        {
            await db.KeyDeleteAsync(lockKey);
            _logger.LogDebug("Lock released: {LockKey}", lockKey);
        }
    }
}

/// <summary>
/// Lock attribute for methods.
/// Ne: Metodlar için lock attribute.
/// </summary>
public class DistributedLockAttribute : Attribute
{
    public string Key { get; }
    public int TimeoutSeconds { get; set; } = 30;
    
    public DistributedLockAttribute(string key)
    {
        Key = key;
    }
}
```

---

Bu dokümantasyon, HBYS sisteminin cross-cutting concerns implementasyonlarını içermektedir. Serilog logging, configuration management, feature management, localization, background jobs (Hangfire), Swagger/OpenAPI documentation, response caching, ve distributed locking dahildir.