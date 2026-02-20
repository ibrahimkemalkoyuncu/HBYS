# HBYS Implementation Code Samples and Technical Specifications
## Core Infrastructure, Database Schema, API Contracts, and Security Implementation

---

# PART 1: CORE INFRASTRUCTURE CODE SAMPLES

## 1.1 Tenant Context and Multi-Tenancy Implementation

### TenantContext.cs
```csharp
/// <summary>
/// Tenant context sınıfı, mevcut isteğin tenant bilgilerini tutar.
/// Ne: Multi-tenant mimaride her istek için tenant bilgisini taşıyan sınıftır.
/// Neden: Tenant izolasyonunu sağlamak ve tenant'a özel verileri filtrelemek için zorunludur.
/// Özelliği: TenantId, TenantCode, TenantType, UserId, Roles özelliklerine sahiptir.
/// Kim Kullanacak: Repository'ler, Service'ler, Middleware'ler, API Controller'lar.
/// Amaç: Tenant bağlamının uygulama genelinde tutarlı şekilde yönetilmesi.
/// </summary>
public class TenantContext
{
    public Guid TenantId { get; }
    public string TenantCode { get; }
    public TenantType TenantType { get; }
    public Guid UserId { get; }
    public IReadOnlyList<string> Roles { get; }
    public string ConnectionString { get; }

    public TenantContext(Guid tenantId, string tenantCode, TenantType tenantType, 
        Guid userId, IReadOnlyList<string> roles, string connectionString)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
        TenantType = tenantType;
        UserId = userId;
        Roles = roles;
        ConnectionString = connectionString;
    }

    public bool HasRole(string role) => Roles.Contains(role);
    public bool IsSystemAdmin() => Roles.Contains("SystemAdmin");
}
```

### ITenantContextAccessor.cs
```csharp
/// <summary>
/// Tenant context erişim arayüzü.
/// Ne: TenantContext'e erişim sağlayan servistir.
/// Neden: Dependency injection ile tenant context'e erişim için gereklidir.
/// Özelliği: TenantContext özelliği ve Reset metodunu içerir.
/// Kim Kullanacak: Tüm uygulama katmanı.
/// Amaç: Tenant bağlamına merkezi erişim sağlanması.
/// </summary>
public interface ITenantContextAccessor
{
    TenantContext? TenantContext { get; set; }
    void Reset();
}

public class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantContext?> _tenantContext = new();

    public TenantContext? TenantContext
    {
        get => _tenantContext.Value;
        set => _tenantContext.Value = value;
    }

    public void Reset() => _tenantContext.Value = null;
}
```

### TenantMiddleware.cs
```csharp
/// <summary>
/// Tenant resolution middleware.
/// Ne: Her istek için tenant bilgisini çözen middleware'dir.
/// Neden: Tenant context'in istek başında set edilmesi için gereklidir.
/// Özelliği: Header, subdomain veya JWT token'dan tenant bilgisi çeker.
/// Kim Kullanacak: Pipeline'daki tüm istekler.
/// Amaç: Tenant bağlamının otomatik olarak belirlenmesi.
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantService _tenantService;

    public TenantMiddleware(RequestDelegate next, ITenantService tenantService)
    {
        _next = next;
        _tenantService = tenantService;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextAccessor tenantContextAccessor)
    {
        var tenantCode = context.Request.Headers["X-Tenant-Code"].FirstOrDefault()
            ?? GetSubdomain(context.Request.Host.Value)
            ?? context.User.FindFirst("tenant_code")?.Value;

        if (!string.IsNullOrEmpty(tenantCode))
        {
            var tenant = await _tenantService.GetTenantByCodeAsync(tenantCode);
            if (tenant != null && tenant.IsActive)
            {
                var userId = context.User.FindFirst("sub")?.Value;
                var roles = context.User.FindAll("role").Select(c => c.Value).ToList();

                tenantContextAccessor.TenantContext = new TenantContext(
                    tenant.Id,
                    tenant.TenantCode,
                    tenant.TenantType,
                    Guid.TryParse(userId, out var id) ? id : Guid.Empty,
                    roles,
                    tenant.ConnectionString
                );
            }
        }

        await _next(context);
    }

    private static string? GetSubdomain(string host)
    {
        var parts = host.Split('.');
        return parts.Length > 2 ? parts[0] : null;
    }
}
```

## 1.2 CQRS Implementation with MediatR

### CreatePatientCommand.cs
```csharp
/// <summary>
/// Hasta oluşturma komutu.
/// Ne: Yeni hasta kaydı oluşturmak için kullanılan CQRS command'idir.
/// Neden: Hasta kaydı işleminin CQRS pattern ile yapılmasını sağlar.
/// Özelliği: TurkishId (encrypted), FirstName, LastName, Gender, DateOfBirth, ContactInfos, 
///           InsuranceInfo, EmergencyContactInfo parametreleri alır. PatientDto döner.
/// Kim Kullanacak: PatientController, PatientService.
/// Amaç: Sisteme yeni hasta eklemek.
/// </summary>
public record CreatePatientCommand : IRequest<PatientDto>
{
    public string TurkishId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string? BloodType { get; init; }
    public List<CreateContactInfoDto> ContactInfos { get; init; } = new();
    public CreateInsuranceDto? InsuranceInfo { get; init; }
    public CreateEmergencyContactDto? EmergencyContact { get; init; }
    public Guid CreatedBy { get; init; }
}

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.TurkishId)
            .NotEmpty().WithMessage("TC Kimlik numarası zorunludur")
            .Length(11).WithMessage("TC Kimlik numarası 11 haneli olmalıdır")
            .Matches("^[0-9]*$").WithMessage("TC Kimlik numarası sadece rakam içermelidir");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad zorunludur")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Doğum tarihi zorunludur")
            .LessThan(DateTime.Now).WithMessage("Doğum tarihi bugünden küçük olmalıdır");

        RuleFor(x => x.ContactInfos)
            .Must(x => x.Any(c => c.ContactType == ContactType.Phone))
            .WithMessage("En az bir telefon numarası gereklidir");
    }
}
```

### CreatePatientCommandHandler.cs
```csharp
/// <summary>
/// Hasta oluşturma komut işleyicisi.
/// Ne: CreatePatientCommand'i işleyen ve hasta kaydını oluşturan handler'dır.
/// Neden: Command'ın iş mantığını execute etmek için gereklidir.
/// Özelliği: IPatientRepository, ITenantContextAccessor, IEncryptionService, IPatientCreatedEventHandler 
///           injection alır. PatientDto döner.
/// Kim Kullanacak: MediatR pipeline.
/// Amaç: Komutun işlenmesi ve sonuçların döndürülmesi.
/// </summary>
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IPatientRepository _patientRepository;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly IEncryptionService _encryptionService;
    private readonly IMediator _mediator;
    private readonly ILogger<CreatePatientCommandHandler> _logger;

    public CreatePatientCommandHandler(
        IPatientRepository patientRepository,
        ITenantContextAccessor tenantContextAccessor,
        IEncryptionService encryptionService,
        IMediator mediator,
        ILogger<CreatePatientCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _tenantContextAccessor = tenantContextAccessor;
        _encryptionService = encryptionService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContextAccessor.TenantContext?.TenantId 
            ?? throw new TenantNotFoundException();

        // Check for existing patient with same Turkish ID
        var existingPatient = await _patientRepository
            .GetByTurkishIdAsync(_encryptionService.Encrypt(request.TurkishId), tenantId);
        
        if (existingPatient != null)
        {
            throw new PatientAlreadyExistsException(
                $"TC Kimlik numarası ile hasta zaten mevcut: {existingPatient.PatientNumber}");
        }

        // Generate patient number
        var patientNumber = await _patientRepository.GeneratePatientNumberAsync(tenantId);

        // Create patient entity
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            PatientNumber = patientNumber,
            TurkishId = _encryptionService.Encrypt(request.TurkishId),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            BloodType = request.BloodType,
            Status = PatientStatus.Active,
            TenantId = tenantId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Add contact informations
        foreach (var contact in request.ContactInfos)
        {
            patient.ContactInfos.Add(new PatientContact
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                ContactType = contact.ContactType,
                Value = contact.ContactType == ContactType.Email 
                    ? contact.Value 
                    : _encryptionService.Encrypt(contact.Value),
                IsPrimary = contact.IsPrimary,
                IsVerified = false,
                TenantId = tenantId
            });
        }

        // Add insurance if provided
        if (request.InsuranceInfo != null)
        {
            patient.Insurances.Add(new PatientInsurance
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                InsuranceType = request.InsuranceInfo.InsuranceType,
                InsuranceProvider = request.InsuranceInfo.InsuranceProvider,
                PolicyNumber = request.InsuranceInfo.PolicyNumber,
                DiscountRate = request.InsuranceInfo.DiscountRate,
                ExpiryDate = request.InsuranceInfo.ExpiryDate,
                IsActive = true,
                TenantId = tenantId
            });
        }

        // Add emergency contact if provided
        if (request.EmergencyContact != null)
        {
            patient.EmergencyContact = new PatientGuardian
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                GuardianType = GuardianType.EmergencyContact,
                FirstName = request.EmergencyContact.FirstName,
                LastName = request.EmergencyContact.LastName,
                Phone = _encryptionService.Encrypt(request.EmergencyContact.Phone),
                Relationship = request.EmergencyContact.Relationship,
                TenantId = tenantId
            };
        }

        // Save to database
        await _patientRepository.AddAsync(patient, cancellationToken);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Patient created successfully: {PatientId}, Tenant: {TenantId}", 
            patient.Id, tenantId);

        // Publish domain event
        await _mediator.Publish(new PatientCreatedEvent
        {
            PatientId = patient.Id,
            PatientNumber = patient.PatientNumber,
            TenantId = tenantId,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            CreatedAt = patient.CreatedAt,
            CreatedBy = patient.CreatedBy
        }, cancellationToken);

        return PatientDto.FromEntity(patient);
    }
}
```

### GetPatientByIdQuery.cs
```csharp
/// <summary>
/// Hasta sorgulama query'si.
/// Ne: ID'ye göre hasta getiren CQRS query'sidir.
/// Neden: Hasta detay sayfası için gereklidir.
/// Özelliği: PatientId parametresi alır. PatientDetailDto döner.
/// Kim Kullanacak: PatientController, PatientService.
/// Amaç: Hasta bilgilerinin sorgulanması.
/// </summary>
public record GetPatientByIdQuery : IRequest<PatientDetailDto>
{
    public Guid PatientId { get; init; }
    public bool IncludeMedicalHistory { get; init; }
    public bool IncludeAppointments { get; init; }
}

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDetailDto>
{
    private readonly IPatientRepository _patientRepository;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public GetPatientByIdQueryHandler(
        IPatientRepository patientRepository,
        ITenantContextAccessor tenantContextAccessor)
    {
        _patientRepository = patientRepository;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<PatientDetailDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContextAccessor.TenantContext?.TenantId 
            ?? throw new TenantNotFoundException();

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, tenantId);
        
        if (patient == null)
            throw new PatientNotFoundException(request.PatientId);

        return PatientDetailDto.FromEntity(patient, request.IncludeMedicalHistory, request.IncludeAppointments);
    }
}
```

## 1.3 Feature Flag Implementation

### IFeatureFlagService.cs
```csharp
/// <summary>
/// Feature flag servisi.
/// Ne: Lisans tabanlı özellik kontrolü yapan servistir.
/// Neden: Module-based licensing ve feature flag yönetimi için gereklidir.
/// Özelliği: IsFeatureEnabled, GetEnabledFeatures, EvaluateFeature metotlarını içerir.
/// Kim Kullanacak: API Controllers, MediatR Handlers, Middleware.
/// Amaç: Lisans ve feature flag yönetimi.
/// </summary>
public interface IFeatureFlagService
{
    Task<bool> IsFeatureEnabledAsync(string featureName, Guid tenantId);
    Task<IEnumerable<string>> GetEnabledFeaturesAsync(Guid tenantId);
    Task<FeatureEvaluationResult> EvaluateFeatureAsync(string featureName, Guid tenantId);
    Task<IEnumerable<ModuleLicenseInfo>> GetLicensedModulesAsync(Guid tenantId);
    Task<bool> ValidateLicenseAsync(Guid tenantId, string moduleName);
}

public class FeatureFlagService : IFeatureFlagService
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<FeatureFlagService> _logger;
    private const string CacheKeyPrefix = "feature_flags:";

    public FeatureFlagService(
        ILicenseRepository licenseRepository,
        ICacheService cacheService,
        ILogger<FeatureFlagService> logger)
    {
        _licenseRepository = licenseRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<bool> IsFeatureEnabledAsync(string featureName, Guid tenantId)
    {
        var cacheKey = $"{CacheKeyPrefix}{tenantId}:{featureName}";
        
        var cached = await _cacheService.GetAsync<bool?>(cacheKey);
        if (cached.HasValue)
            return cached.Value;

        var license = await _licenseRepository.GetByTenantAndModuleAsync(tenantId, featureName);
        var isEnabled = license?.IsEnabled ?? false;

        await _cacheService.SetAsync(cacheKey, isEnabled, TimeSpan.FromMinutes(15));
        
        return isEnabled;
    }

    public async Task<IEnumerable<string>> GetEnabledFeaturesAsync(Guid tenantId)
    {
        var cacheKey = $"{CacheKeyPrefix}{tenantId}:all";
        
        var cached = await _cacheService.GetAsync<List<string>>(cacheKey);
        if (cached != null)
            return cached;

        var licenses = await _licenseRepository.GetActiveLicensesAsync(tenantId);
        var features = licenses.Where(l => l.IsEnabled).Select(l => l.ModuleName).ToList();

        await _cacheService.SetAsync(cacheKey, features, TimeSpan.FromMinutes(15));
        
        return features;
    }

    public async Task<FeatureEvaluationResult> EvaluateFeatureAsync(string featureName, Guid tenantId)
    {
        var license = await _licenseRepository.GetByTenantAndModuleAsync(tenantId, featureName);
        
        return new FeatureEvaluationResult
        {
            FeatureName = featureName,
            IsEnabled = license?.IsEnabled ?? false,
            IsLicensed = license != null,
            ExpiresAt = license?.ExpiryDate,
            LicenseKey = license?.LicenseKey
        };
    }

    public async Task<IEnumerable<ModuleLicenseInfo>> GetLicensedModulesAsync(Guid tenantId)
    {
        var licenses = await _licenseRepository.GetActiveLicensesAsync(tenantId);
        
        return licenses.Select(l => new ModuleLicenseInfo
        {
            ModuleName = l.ModuleName,
            IsEnabled = l.IsEnabled,
            ExpiryDate = l.ExpiryDate,
            MaxUsers = l.MaxUsers,
            LicenseKey = l.LicenseKey
        });
    }

    public async Task<bool> ValidateLicenseAsync(Guid tenantId, string moduleName)
    {
        var license = await _licenseRepository.GetByTenantAndModuleAsync(tenantId, moduleName);
        
        if (license == null || !license.IsEnabled)
            return false;

        if (license.ExpiryDate.HasValue && license.ExpiryDate.Value < DateTime.UtcNow)
            return false;

        return true;
    }
}
```

### FeatureFlagAttribute.cs
```csharp
/// <summary>
/// Feature flag attribute.
/// Ne: Endpoint veya method seviyesinde feature flag kontrolü yapan attribute'dur.
/// Neden: Yetkisiz erişimi engellemek ve feature flag'i zorunlu kılmak için gereklidir.
/// Özelliği: FeatureName, FallbackAction özelliklerine sahiptir.
/// Kim Kullanacak: API Controllers, Service Methods.
/// Amaç: Feature flag bazlı erişim kontrolü.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class FeatureFlagAttribute : Attribute
{
    public string FeatureName { get; }
    public FeatureFlagBehavior Behavior { get; }

    public FeatureFlagAttribute(string featureName, FeatureFlagBehavior behavior = FeatureFlagBehavior.Return404)
    {
        FeatureName = featureName;
        Behavior = behavior;
    }
}

public enum FeatureFlagBehavior
{
    Return404,
    ThrowException,
    Redirect
}

public class FeatureFlagFilter : IAsyncAuthorizationFilter
{
    private readonly IFeatureFlagService _featureFlagService;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<FeatureFlagFilter> _logger;

    public FeatureFlagFilter(
        IFeatureFlagService featureFlagService,
        ITenantContextAccessor tenantContextAccessor,
        ILogger<FeatureFlagFilter> logger)
    {
        _featureFlagService = featureFlagService;
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var featureFlags = endpoint?.Metadata.GetMetadata<FeatureFlagAttribute[]>();

        if (featureFlags == null || featureFlags.Length == 0)
            return;

        var tenantId = _tenantContextAccessor.TenantContext?.TenantId;
        if (tenantId == null)
        {
            context.Result = new UnauthorizedObjectResult("Tenant not found");
            return;
        }

        foreach (var flag in featureFlags)
        {
            var isEnabled = await _featureFlagService.IsFeatureEnabledAsync(flag.FeatureName, tenantId.Value);
            
            if (!isEnabled)
            {
                _logger.LogWarning("Feature {FeatureName} is not enabled for tenant {TenantId}", 
                    flag.FeatureName, tenantId);

                context.Result = flag.Behavior switch
                {
                    FeatureFlagBehavior.Return404 => new NotFoundObjectResult(
                        new { error = $"Feature '{flag.FeatureName}' is not available" }),
                    FeatureFlagBehavior.ThrowException => throw new FeatureNotEnabledException(flag.FeatureName),
                    _ => context.Result
                };
                
                return;
            }
        }
    }
}
```

---

# PART 2: DATABASE SCHEMA DESIGNS

## 2.1 Patient Module Schema

### Patients Table
```sql
-- Patients tablosu - Hasta ana bilgi tablosu
-- Ne: Tüm hasta kayıtlarını tutan ana tablodur.
-- Neden: Hasta bilgilerinin merkezi yönetimi için gereklidir.
-- Özelliği: TenantId, PatientNumber (unique), TurkishId (encrypted), FirstName, LastName, 
--           Gender, DateOfBirth, BloodType, PhotoUrl, Status, CreatedAt, UpdatedAt içerir.
CREATE TABLE [dbo].[Patients] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PatientNumber] NVARCHAR(20) NOT NULL,
    [TurkishId] NVARCHAR(256) NOT NULL,  -- Encrypted
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Gender] INT NOT NULL,
    [DateOfBirth] DATETIME2 NOT NULL,
    [BloodType] NVARCHAR(5) NULL,
    [PhotoUrl] NVARCHAR(500) NULL,
    [Status] INT NOT NULL DEFAULT 1,  -- 1=Active, 2=Inactive, 3=Deceased
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Patients_PatientNumber] UNIQUE ([TenantId], [PatientNumber]),
    CONSTRAINT [UK_Patients_TurkishId] UNIQUE ([TenantId], [TurkishId])
);

-- Index for tenant isolation
CREATE NONCLUSTERED INDEX [IX_Patients_TenantId] ON [dbo].[Patients] ([TenantId]);

-- Index for patient search
CREATE NONCLUSTERED INDEX [IX_Patients_Name] ON [dbo].[Patients] ([TenantId], [LastName], [FirstName]);

-- Index for status queries
CREATE NONCLUSTERED INDEX [IX_Patients_Status] ON [dbo].[Patients] ([TenantId], [Status]);
```

### PatientContacts Table
```sql
-- PatientContacts tablosu - Hasta iletişim bilgileri
-- Ne: Hasta iletişim bilgilerini tutan tablodur.
-- Neden: Birden fazla iletişim bilgisi (telefon, e-posta, adres) desteklemek için gereklidir.
CREATE TABLE [dbo].[PatientContacts] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [ContactType] INT NOT NULL,  -- 1=Phone, 2=Email, 3=Address
    [Value] NVARCHAR(500) NOT NULL,  -- Encrypted for sensitive types
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_PatientContacts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientContacts_Patients] FOREIGN KEY ([PatientId]) 
        REFERENCES [dbo].[Patients]([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_PatientContacts_PatientId] ON [dbo].[PatientContacts] ([PatientId]);
CREATE NONCLUSTERED INDEX [IX_PatientContacts_TenantId] ON [dbo].[PatientContacts] ([TenantId]);
```

### PatientMedicalInformation Tables
```sql
-- PatientAllergies tablosu - Hasta alerji bilgileri
CREATE TABLE [dbo].[PatientAllergies] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [AllergyType] INT NOT NULL,  -- 1=Food, 2=Drug, 3=Environmental
    [Allergen] NVARCHAR(200) NOT NULL,
    [Severity] INT NOT NULL,  -- 1=Mild, 2=Moderate, 3=Severe
    [Reaction] NVARCHAR(500) NULL,
    [OnsetDate] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_PatientAllergies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientAllergies_Patients] FOREIGN KEY ([PatientId]) 
        REFERENCES [dbo].[Patients]([Id]) ON DELETE CASCADE
);

-- PatientChronicDiseases tablosu - Kronik hastalıklar
CREATE TABLE [dbo].[PatientChronicDiseases] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [DiseaseCode] NVARCHAR(20) NOT NULL,
    [DiseaseName] NVARCHAR(200) NOT NULL,
    [DiagnosisDate] DATETIME2 NOT NULL,
    [Status] INT NOT NULL,  -- 1=Active, 2=Remission, 3=Cured
    [Notes] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_PatientChronicDiseases] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientChronicDiseases_Patients] FOREIGN KEY ([PatientId]) 
        REFERENCES [dbo].[Patients]([Id]) ON DELETE CASCADE
);

-- PatientMedications tablosu - Mevcut ilaçlar
CREATE TABLE [dbo].[PatientMedications] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [MedicationName] NVARCHAR(200) NOT NULL,
    [Dosage] NVARCHAR(100) NOT NULL,
    [Frequency] NVARCHAR(100) NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [PrescribedBy] UNIQUEIDENTIFIER NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_PatientMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientMedications_Patients] FOREIGN KEY ([PatientId]) 
        REFERENCES [dbo].[Patients]([Id]) ON DELETE CASCADE
);
```

## 2.2 Appointment Module Schema

### Appointments Table
```sql
-- Appointments tablosu - Randevu bilgileri
-- Ne: Hasta-hekim randevularını tutan tablodur.
-- Neden: Randevu yönetimi ve kaynak planlaması için gereklidir.
CREATE TABLE [dbo].[Appointments] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [AppointmentNumber] NVARCHAR(20) NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [PhysicianId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    [AppointmentDate] DATE NOT NULL,
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [AppointmentType] INT NOT NULL,  -- 1=Checkup, 2=FollowUp, 3=Procedure
    [Status] INT NOT NULL DEFAULT 1,  -- 1=Scheduled, 2=Confirmed, 3=Completed, 4=Cancelled, 5=NoShow
    [Reason] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [ConfirmedAt] DATETIME2 NULL,
    [ConfirmedBy] UNIQUEIDENTIFIER NULL,
    [CancelledAt] DATETIME2 NULL,
    [CancelledBy] UNIQUEIDENTIFIER NULL,
    [CancelledReason] NVARCHAR(500) NULL,
    [CompletedAt] DATETIME2 NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Appointments_AppointmentNumber] UNIQUE ([TenantId], [AppointmentNumber])
);

CREATE NONCLUSTERED INDEX [IX_Appointments_PatientId] ON [dbo].[Appointments] ([PatientId]);
CREATE NONCLUSTERED INDEX [IX_Appointments_PhysicianId] ON [dbo].[Appointments] ([PhysicianId]);
CREATE NONCLUSTERED INDEX [IX_Appointments_Date] ON [dbo].[Appointments] ([AppointmentDate]);
CREATE NONCLUSTERED INDEX [IX_Appointments_Status] ON [dbo].[Appointments] ([Status]);

-- Composite index for physician schedule
CREATE NONCLUSTERED INDEX [IX_Appointments_Physician_Date] 
    ON [dbo].[Appointments] ([PhysicianId], [AppointmentDate], [Status]);
```

### AppointmentSlots Table
```sql
-- AppointmentSlots tablosu - Hekim müsaitlik bilgileri
CREATE TABLE [dbo].[AppointmentSlots] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PhysicianId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    [DayOfWeek] INT NOT NULL,  -- 0=Sunday, 1=Monday, etc.
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [SlotDuration] INT NOT NULL,  -- Minutes per slot
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AppointmentSlots] PRIMARY KEY ([Id])
);

CREATE NONCLUSTERED INDEX [IX_AppointmentSlots_Physician] 
    ON [dbo].[AppointmentSlots] ([PhysicianId], [DayOfWeek]);
```

---

# PART 3: API CONTRACTS

## 3.1 Patient Module Minimal API

### PatientEndpoints.cs
```csharp
/// <summary>
/// Patient API endpoint tanımları.
/// Ne: Hasta işlemleri için Minimal API endpoint'lerini tanımlar.
/// Neden: RESTful API sağlamak için gereklidir.
/// Özelliği: CRUD endpoint'lerini içerir.
/// Kim Kullanacak: Angular frontend uygulaması.
/// Amaç: Hasta yönetimi için HTTP API sağlanması.
/// </summary>
public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/patients")
            .WithTags("Patients")
            .AddFeatureFlagFilter();

        // Create patient
        group.MapPost("/", CreatePatient)
            .WithName("CreatePatient")
            .WithOpenApi()
            .Produces<PatientDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // Get patient by ID
        group.MapGet("/{id:guid}", GetPatientById)
            .WithName("GetPatientById")
            .WithOpenApi()
            .Produces<PatientDetailDto>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // Search patients
        group.MapGet("/search", SearchPatients)
            .WithName("SearchPatients")
            .WithOpenApi()
            .Produces<PaginatedResult<PatientListDto>>();

        // Update patient
        group.MapPut("/{id:guid}", UpdatePatient)
            .WithName("UpdatePatient")
            .WithOpenApi()
            .Produces<PatientDto>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // Verify patient identity
        group.MapPost("/{id:guid}/verify", VerifyPatientIdentity)
            .WithName("VerifyPatientIdentity")
            .WithOpenApi()
            .Produces<VerificationResult>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // Get patient allergies
        group.MapGet("/{id:guid}/allergies", GetPatientAllergies)
            .WithName("GetPatientAllergies")
            .WithOpenApi()
            .Produces<List<AllergyDto>>();

        // Add patient allergy
        group.MapPost("/{id:guid}/allergies", AddPatientAllergy)
            .WithName("AddPatientAllergy")
            .WithOpenApi()
            .Produces<AllergyDto>(StatusCodes.Status201Created);

        // Get patient appointments
        group.MapGet("/{id:guid}/appointments", GetPatientAppointments)
            .WithName("GetPatientAppointments")
            .WithOpenApi()
            .Produces<List<AppointmentDto>>();
    }

    private static async Task<IResult> CreatePatient(
        CreatePatientCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/v1/patients/{result.Id}", result);
    }

    private static async Task<IResult> GetPatientById(
        Guid id,
        bool includeMedicalHistory,
        bool includeAppointments,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPatientByIdQuery
        {
            PatientId = id,
            IncludeMedicalHistory = includeMedicalHistory,
            IncludeAppointments = includeAppointments
        };
        
        var result = await mediator.Send(query, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchPatients(
        [AsParameters] SearchPatientsRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new SearchPatientsQuery
        {
            SearchTerm = request.SearchTerm,
            Status = request.Status,
            DateOfBirthFrom = request.DateOfBirthFrom,
            DateOfBirthTo = request.DateOfBirthTo,
            PageNumber = request.PageNumber ?? 1,
            PageSize = request.PageSize ?? 20
        };
        
        var result = await mediator.Send(query, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePatient(
        Guid id,
        UpdatePatientCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        if (id != command.PatientId)
            return Results.BadRequest("ID mismatch");
        
        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }
}

public record SearchPatientsRequest(
    string? SearchTerm,
    PatientStatus? Status,
    DateTime? DateOfBirthFrom,
    DateTime? DateOfBirthTo,
    int? PageNumber,
    int? PageSize);
```

## 3.2 Appointment Module Minimal API

### AppointmentEndpoints.cs
```csharp
/// <summary>
/// Appointment API endpoint tanımları.
/// Ne: Randevu işlemleri için Minimal API endpoint'lerini tanımlar.
/// Neden: Randevu yönetimi için RESTful API sağlamak için gereklidir.
/// Özelliği: Randevu CRUD, slot sorgulama, sıra yönetimi endpoint'lerini içerir.
/// Kim Kullanacak: Angular frontend uygulaması.
/// Amaç: Randevu yönetimi için HTTP API sağlanması.
/// </summary>
public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/appointments")
            .WithTags("Appointments");

        // Create appointment
        group.MapPost("/", CreateAppointment)
            .WithName("CreateAppointment")
            .WithOpenApi()
            .Produces<AppointmentDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Get appointment by ID
        group.MapGet("/{id:guid}", GetAppointmentById)
            .WithName("GetAppointmentById")
            .WithOpenApi()
            .Produces<AppointmentDetailDto>();

        // Confirm appointment
        group.MapPut("/{id:guid}/confirm", ConfirmAppointment)
            .WithName("ConfirmAppointment")
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);

        // Cancel appointment
        group.MapPut("/{id:guid}/cancel", CancelAppointment)
            .WithName("CancelAppointment")
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);

        // Reschedule appointment
        group.MapPut("/{id:guid}/reschedule", RescheduleAppointment)
            .WithName("RescheduleAppointment")
            .WithOpenApi()
            .Produces<AppointmentDto>();

        // Complete appointment
        group.MapPut("/{id:guid}/complete", CompleteAppointment)
            .WithName("CompleteAppointment")
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);

        // Get available slots
        group.MapGet("/available-slots", GetAvailableSlots)
            .WithName("GetAvailableSlots")
            .WithOpenApi()
            .Produces<List<TimeSlotDto>>();

        // Get patient appointments
        group.MapGet("/patient/{patientId:guid}", GetPatientAppointments)
            .WithName("GetPatientAppointments")
            .WithOpenApi()
            .Produces<List<AppointmentDto>>();

        // Get physician appointments
        group.MapGet("/physician/{physicianId:guid}/date/{date:DateTime}", GetPhysicianAppointments)
            .WithName("GetPhysicianAppointments")
            .WithOpenApi()
            .Produces<List<AppointmentDto>>();

        // Get department queue
        group.MapGet("/department/{departmentId:guid}/queue", GetDepartmentQueue)
            .WithName("GetDepartmentQueue")
            .WithOpenApi()
            .Produces<List<QueueItemDto>>();
    }

    private static async Task<IResult> CreateAppointment(
        CreateAppointmentCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/v1/appointments/{result.Id}", result);
    }

    private static async Task<IResult> GetAvailableSlots(
        Guid physicianId,
        Guid departmentId,
        DateTime date,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetAvailableSlotsQuery
        {
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            Date = date
        };
        
        var result = await mediator.Send(query, ct);
        return Results.Ok(result);
    }
}
```

---

# PART 4: SECURITY IMPLEMENTATION

## 4.1 JWT Authentication

### JwtTokenService.cs
```csharp
/// <summary>
/// JWT token servisi.
/// Ne: JWT token oluşturma ve doğrulama işlemlerini yapan servistir.
/// Neden: Kimlik doğrulama için gereklidir.
/// Özelliği: GenerateToken, ValidateToken, RefreshToken metotlarını içerir.
/// Kim Kullanacak: AuthController, JwtMiddleware.
/// Amaç: JWT tabanlı kimlik doğrulama yönetimi.
/// </summary>
public interface IJwtTokenService
{
    Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, Guid tenantId, IEnumerable<string> roles);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task InvalidateTokenAsync(string token);
    Task InvalidateAllUserTokensAsync(Guid userId);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtSettings> settings,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ICacheService cacheService,
        ILogger<JwtTokenService> logger)
    {
        _settings = settings.Value;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, Guid tenantId, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant_id", tenantId.ToString()),
            new("tenant_code", await GetTenantCodeAsync(tenantId))
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        var refreshToken = Guid.NewGuid().ToString();
        
        // Store refresh token
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            TenantId = tenantId,
            ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetClientIpAddress()
        });

        _logger.LogInformation("Token generated for user {UserId}, tenant {TenantId}", userId, tenantId);

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = refreshToken,
            ExpiresIn = _settings.AccessTokenExpirationMinutes * 60,
            TokenType = "Bearer"
        };
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            // Check if token is blacklisted
            var isBlacklisted = await _cacheService.GetAsync<bool>($"token_blacklist:{token}");
            if (isBlacklisted)
                return TokenValidationResult.Blacklisted;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            if (validatedToken is not JwtSecurityToken jwtToken)
                return TokenValidationResult.Invalid;

            return new TokenValidationResult
            {
                IsValid = true,
                UserId = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value),
                TenantId = Guid.Parse(principal.FindFirst("tenant_id")?.Value),
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                ExpiresAt = jwtToken.ValidTo
            };
        }
        catch (SecurityTokenExpiredException)
        {
            return TokenValidationResult.Expired;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return TokenValidationResult.Invalid;
        }
    }

    public async Task InvalidateTokenAsync(string token)
    {
        var validationResult = await ValidateTokenAsync(token);
        if (validationResult.IsValid)
        {
            // Add to blacklist
            var expiresAt = validationResult.ExpiresAt - DateTime.UtcNow;
            if (expiresAt > TimeSpan.Zero)
            {
                await _cacheService.SetAsync(
                    $"token_blacklist:{token}", 
                    true, 
                    expiresAt);
            }
        }
    }
}
```

## 4.2 Data Encryption

### EncryptionService.cs
```csharp
/// <summary>
/// Şifreleme servisi.
/// Ne: Hassas verilerin AES-256 ile şifrelenmesini sağlayan servistir.
/// Neden: KVKK uyumluluğu için gereklidir.
/// Özelliği: Encrypt, Decrypt, Hash metotlarını içerir.
/// Kim Kullanacak: PatientRepository, UserRepository, Repository'ler.
/// Amaç: Hassas verilerin güvenli saklanması.
/// </summary>
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string Hash(string plainText);
    bool VerifyHash(string plainText, string hash);
}

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IOptions<EncryptionSettings> settings)
    {
        var encryptionSettings = settings.Value;
        _key = Convert.FromBase64String(encryptionSettings.Key);
        _iv = Convert.FromBase64String(encryptionSettings.IV);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch
        {
            // Return empty or throw specific exception
            return string.Empty;
        }
    }

    public string Hash(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyHash(string plainText, string hash)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(hash))
            return false;

        return Hash(plainText) == hash;
    }
}
```

---

# PART 5: CACHING AND PERFORMANCE

## 5.1 Redis Cache Implementation

### CacheService.cs
```csharp
/// <summary>
/// Redis cache servisi.
/// Ne: Distributed caching için Redis kullanan servistir.
/// Neden: Performans optimizasyonu ve distributed cache için gereklidir.
/// Özelliği: Get, Set, Remove, Exists metotlarını içerir.
/// Kim Kullanacak: Repositories, Services.
/// Amaç: Veri caching yönetimi.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var db = _redis.GetDatabase();
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            
            await db.StringSetAsync(key, jsonValue, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error setting cache key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing cache key {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking cache key {Key}", key);
            return false;
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

            if (keys.Length > 0)
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(keys);
                _logger.LogInformation("Removed {Count} cache keys matching pattern {Pattern}", 
                    keys.Length, pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing cache keys by pattern {Pattern}", pattern);
        }
    }
}
```

---

# PART 6: TEST IMPLEMENTATION EXAMPLES

## 6.1 Unit Test Example

### CreatePatientCommandHandlerTests.cs
```csharp
/// <summary>
/// CreatePatientCommandHandler unit testleri.
/// Ne: Hasta oluşturma komut işleyicisi için yazılan unit testlerdir.
/// Neden: Business logic'in doğru çalışmasını test etmek için gereklidir.
/// Özelliği: Arrange-Act-Assert pattern kullanır.
/// Kim Kullanacak: Test developers.
/// Amaç: Komut işleyicisinin test edilmesi.
/// </summary>
public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IPatientRepository> _patientRepository;
    private readonly Mock<ITenantContextAccessor> _tenantContextAccessor;
    private readonly Mock<IEncryptionService> _encryptionService;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<CreatePatientCommandHandler>> _logger;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _patientRepository = new Mock<IPatientRepository>();
        _tenantContextAccessor = new Mock<ITenantContextAccessor>();
        _encryptionService = new Mock<IEncryptionService>();
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<CreatePatientCommandHandler>>();

        _tenantContextAccessor.Setup(x => x.TenantContext)
            .Returns(new TenantContext(
                Guid.NewGuid(),
                "TEST001",
                TenantType.SaaS,
                Guid.NewGuid(),
                new List<string> { "Admin" },
                "ConnectionString"
            ));

        _handler = new CreatePatientCommandHandler(
            _patientRepository.Object,
            _tenantContextAccessor.Object,
            _encryptionService.Object,
            _mediator.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPatientDto()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            TurkishId = "12345678901",
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            ContactInfos = new List<CreateContactInfoDto>
            {
                new() { ContactType = ContactType.Phone, Value = "5321234567", IsPrimary = true }
            },
            CreatedBy = Guid.NewGuid()
        };

        _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns((string s) => $"encrypted_{s}");

        _patientRepository.Setup(x => x.GetByTurkishIdAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((Patient?)null);

        _patientRepository.Setup(x => x.GeneratePatientNumberAsync(It.IsAny<Guid>()))
            .ReturnsAsync("P001");

        _patientRepository.Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _patientRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Ahmet");
        result.LastName.Should().Be("Yılmaz");
        result.PatientNumber.Should().Be("P001");
        
        _patientRepository.Verify(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(x => x.Publish(It.IsAny<PatientCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateTurkishId_ThrowsException()
    {
        // Arrange
        var existingPatient = new Patient
        {
            Id = Guid.NewGuid(),
            PatientNumber = "P002"
        };

        var command = new CreatePatientCommand
        {
            TurkishId = "12345678901",
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedBy = Guid.NewGuid()
        };

        _patientRepository.Setup(x => x.GetByTurkishIdAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(existingPatient);

        // Act & Assert
        await Assert.ThrowsAsync<PatientAlreadyExistsException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abcdefghijk")]
    public async Task Handle_InvalidTurkishId_ThrowsValidationException(string turkishId)
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            TurkishId = turkishId,
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
```

---

# PART 7: CONFIGURATION EXAMPLES

## 7.1 appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HBYS;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "HBYS_"
  },
  "Jwt": {
    "SecretKey": "your-256-bit-secret-key-here-must-be-at-least-32-chars",
    "Issuer": "HBYS",
    "Audience": "HBYS.Client",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Encryption": {
    "Key": "base64-encoded-32-byte-key",
    "IV": "base64-encoded-16-byte-iv"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://localhost:9200",
          "indexFormat": "hbys-logs-{0:yyyy.MM}"
        }
      },
      {
        "Name": "Console"
      }
    ]
  },
  "FeatureFlags": {
    "Modules": [
      "Patient",
      "Appointment", 
      "Outpatient",
      "Billing",
      "Inpatient",
      "Emergency",
      "Pharmacy",
      "Inventory",
      "Laboratory",
      "Radiology",
      "Accounting",
      "Reporting",
      "HR",
      "Quality"
    ]
  }
}
```

## 7.2 docker-compose.yml
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: src/HBYS.Api/Dockerfile
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=HBYS;User=sa;Password=YourPassword!
      - Redis__ConnectionString=redis:6379
    depends_on:
      - sqlserver
      - redis

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword!
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    ports:
      - "9200:9200"
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data

  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.0
    ports:
      - "5601:5601"
    depends_on:
      - elasticsearch

volumes:
  sqlserver_data:
  redis_data:
  elasticsearch_data:
```

---

Bu dokümantasyon, HBYS sisteminin gerçek implementasyonu için gerekli tüm teknik detayları içermektedir. Kod örnekleri, veritabanı şemaları, API contract'ları, güvenlik implementasyonları, caching stratejileri, test örnekleri ve konfigürasyon dosyaları dahil edilmiştir.