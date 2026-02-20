namespace HBYS.Domain.Entities.License;

/// <summary>
/// Lisans entity sınıfı.
/// Ne: Tenant lisans bilgilerini temsil eden aggregate root sınıfıdır.
/// Neden: Lisans yönetimi ve feature flag'ler için gereklidir.
/// Özelliği: ModuleName, LicenseType, Status, StartDate, ExpiryDate, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: LicenseRepository, LicenseService, FeatureFlagService.
/// Amacı: Lisans verilerinin domain model olarak yönetilmesi.
/// </summary>
public class License : HBYS.Domain.Entities.BaseTenantEntity
{
    /// <summary>
    /// Modül adı. Örn: "Patient", "Appointment", "Billing"
    /// </summary>
    public string ModuleName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Lisans tipi: 1=Trial, 2=Standard, 3=Professional, 4=Enterprise
    /// </summary>
    public LicenseType LicenseType { get; private set; }
    
    /// <summary>
    /// Lisans durumu: 1=Active, 2=Expired, 3=Cancelled
    /// </summary>
    public LicenseStatus Status { get; private set; }
    
    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime StartDate { get; private set; }
    
    /// <summary>
    /// Bitiş tarihi
    /// </summary>
    public DateTime? ExpiryDate { get; private set; }
    
    /// <summary>
    /// Maksimum kullanıcı sayısı
    /// </summary>
    public int MaxUsers { get; private set; }
    
    /// <summary>
    /// Maksimum hasta sayısı
    /// </summary>
    public int MaxPatients { get; private set; }
    
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private License() { }

    /// <summary>
    /// Factory method - Yeni lisans oluşturma
    /// </summary>
    public static License Create(
        string moduleName,
        LicenseType licenseType,
        DateTime startDate,
        DateTime? expiryDate,
        int maxUsers,
        int maxPatients,
        Guid tenantId)
    {
        return new License
        {
            ModuleName = moduleName,
            LicenseType = licenseType,
            Status = LicenseStatus.Active,
            StartDate = startDate,
            ExpiryDate = expiryDate,
            MaxUsers = maxUsers,
            MaxPatients = maxPatients,
            TenantId = tenantId
        };
    }

    /// <summary>
    /// Lisansı uzat
    /// </summary>
    public void Extend(DateTime newExpiryDate)
    {
        ExpiryDate = newExpiryDate;
        Status = LicenseStatus.Active;
    }

    /// <summary>
    /// Lisansı iptal et
    /// </summary>
    public void Cancel()
    {
        Status = LicenseStatus.Cancelled;
    }

    /// <summary>
    /// Lisansı yenile
    /// </summary>
    public void Renew()
    {
        Status = LicenseStatus.Active;
    }

    /// <summary>
    /// Lisans expire olmuş mu?
    /// </summary>
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive => Status == LicenseStatus.Active && !IsExpired;
}

/// <summary>
/// Lisans tipi enum
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Deneme
    /// </summary>
    Trial = 1,
    
    /// <summary>
    /// Standart
    /// </summary>
    Standard = 2,
    
    /// <summary>
    /// Profesyonel
    /// </summary>
    Professional = 3,
    
    /// <summary>
    /// Kurumsal
    /// </summary>
    Enterprise = 4
}

/// <summary>
/// Lisans durumu enum
/// </summary>
public enum LicenseStatus
{
    /// <summary>
    /// Aktif
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Süresi dolmuş
    /// </summary>
    Expired = 2,
    
    /// <summary>
    /// İptal edilmiş
    /// </summary>
    Cancelled = 3
}

/// <summary>
/// Lisans özellikleri (Feature Flags)
/// </summary>
public class LicenseFeature : HBYS.Domain.Entities.BaseTenantEntity
{
    /// <summary>
    /// Özellik adı
    /// </summary>
    public string FeatureName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// Lisans ID
    /// </summary>
    public Guid LicenseId { get; private set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsEnabled { get; private set; }
    
    /// <summary>
    /// Sınır değeri (örn: Max appointments)
    /// </summary>
    public int? LimitValue { get; private set; }

    private LicenseFeature() { }

    public static LicenseFeature Create(
        Guid licenseId,
        string featureName,
        string? description,
        bool isEnabled,
        int? limitValue,
        Guid tenantId)
    {
        return new LicenseFeature
        {
            LicenseId = licenseId,
            FeatureName = featureName,
            Description = description,
            IsEnabled = isEnabled,
            LimitValue = limitValue,
            TenantId = tenantId
        };
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public void UpdateLimit(int? limitValue)
    {
        LimitValue = limitValue;
    }
}
