namespace HBYS.Domain.Entities.Identity;

/// <summary>
/// Tenant entity sınıfı.
/// Ne: Multi-tenant mimaride tenant bilgilerini temsil eden aggregate root sınıfıdır.
/// Neden: Tenant izolasyonu ve yönetimi için gereklidir.
/// Özelliği: TenantCode, Name, DisplayName, TenantType, IsActive, ExpiresAt, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: TenantRepository, TenantService, ITenantContextAccessor.
/// Amacı: Tenant verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>
    /// Tenant kodu. Örn: "DEMO", "SAGLIK", "ONUR"
    /// </summary>
    public string TenantCode { get; private set; } = string.Empty;
    
    /// <summary>
    /// Tenant adı
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Görüntüleme adı
    /// </summary>
    public string DisplayName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Tenant tipi: 1=SaaS, 2=OnPrem, 3=Group
    /// </summary>
    public TenantType TenantType { get; private set; }
    
    /// <summary>
    /// Veritabanı adı (schema-per-tenant için)
    /// </summary>
    public string? DatabaseName { get; private set; }
    
    /// <summary>
    /// Şema adı (schema-per-tenant için)
    /// </summary>
    public string? SchemaName { get; private set; }
    
    /// <summary>
    /// Bağlantı string'i (encrypted)
    /// </summary>
    public string? ConnectionString { get; private set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Lisans bitiş tarihi
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }
    
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Tenant() { }

    /// <summary>
    /// Factory method - Yeni tenant oluşturma
    /// </summary>
    public static Tenant Create(
        string tenantCode,
        string name,
        string displayName,
        TenantType tenantType,
        string? databaseName = null,
        string? schemaName = null,
        string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(tenantCode))
            throw new ArgumentException("Tenant code is required", nameof(tenantCode));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        return new Tenant
        {
            Id = Guid.NewGuid(),
            TenantCode = tenantCode.ToUpperInvariant(),
            Name = name,
            DisplayName = displayName,
            TenantType = tenantType,
            DatabaseName = databaseName,
            SchemaName = schemaName,
            ConnectionString = connectionString,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Tenant'ı deaktive et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Tenant'ı aktive et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Lisans süresini uzat
    /// </summary>
    public void ExtendLicense(DateTime expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Tenant bilgilerini güncelle
    /// </summary>
    public void UpdateInfo(string name, string displayName)
    {
        Name = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// Lisans expire olmuş mu?
    /// </summary>
    public bool IsLicenseExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}

/// <summary>
/// Tenant tipi enum
/// </summary>
public enum TenantType
{
    /// <summary>
    /// SaaS - Bulut tabanlı
    /// </summary>
    SaaS = 1,
    
    /// <summary>
    /// On-Premise - Şirket içi
    /// </summary>
    OnPrem = 2,
    
    /// <summary>
    /// Grup Hastane - Çoklu hastane
    /// </summary>
    Group = 3
}
