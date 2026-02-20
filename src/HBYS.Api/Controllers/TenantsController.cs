using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Kurum (Tenant) Yönetimi Controller'ı
/// Ne: Çoklu kurum (multi-tenant) işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Kurum kaydı, güncelleme ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: HBYS'nin SaaS ve grup hastane desteği için temel altyapı sağlar.
/// Kim Kullanacak: Sistem yöneticisi, Kurum yönetimi paneli.
/// Amacı: HBYS sisteminin çoklu kurum destekli altyapısı.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ILogger<TenantsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm kurumları getir
    /// Ne: Sistemdeki tüm kurumları (tenant) listeleyen endpoint.
    /// Neden: Super-admin paneli için tüm kurumları listelemek gereklidir.
    /// Kim Kullanacak: Sistem yöneticisi, Super-admin paneli.
    /// Amacı: Tüm kurumların görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Getting all tenants");

        return Ok(new[]
        {
            new { id = Guid.NewGuid(), code = "HBYS", name = "HBYS Hastanesi", isActive = true }
        });
    }

    /// <summary>
    /// ID'ye göre kurum getir
    /// Ne: Belirli bir kurumun detaylarını getiren endpoint.
    /// Neden: Kurum düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Kurum yönetimi, Ayarlar.
    /// Amacı: Tekil kurum bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting tenant by ID: {TenantId}", id);

        return Ok(new
        {
            id = id,
            code = "HBYS",
            name = "HBYS Hastanesi",
            displayName = "HBYS Hastanesi A.Ş.",
            type = "Hospital",
            address = "İstanbul, Türkiye",
            phoneNumber = "+90 212 123 45 67",
            email = "info@hbys.com",
            isActive = true,
            licenseType = "Enterprise",
            maxUsers = 500,
            maxPatients = 50000
        });
    }

    /// <summary>
    /// Kurum kodu ile getir
    /// Ne: Kurum kodu (tenant code) ile kurum bilgilerini getiren endpoint.
    /// Neden: Tenant context belirlemek için gereklidir.
    /// Kim Kullanacak: API Gateway, Kimlik doğrulama.
    /// Amacı: Kurum koduna göre kurum sorgulama.
    /// </summary>
    [HttpGet("by-code/{code}")]
    public IActionResult GetByCode(string code)
    {
        _logger.LogInformation("Getting tenant by code: {TenantCode}", code);

        return Ok(new
        {
            id = Guid.NewGuid(),
            code = code,
            name = $"{code} Hastanesi",
            isActive = true
        });
    }

    /// <summary>
    /// Yeni kurum oluştur
    /// Ne: Sisteme yeni kurum (tenant) ekleyen endpoint.
    /// Neden: Yeni hastane veya kurum ekleme işlemleri için gereklidir.
    /// Kim Kullanacak: Sistem yöneticisi, Kurum oluşturma sihirbazı.
    /// Amacı: Sisteme yeni kurum ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateTenantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { error = "Kurum kodu zorunludur." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Kurum adı zorunludur." });
        }

        _logger.LogInformation("Creating new tenant: {TenantName}", request.Name);

        var tenantId = Guid.NewGuid();
        
        return Created($"/api/tenants/{tenantId}", new
        {
            id = tenantId,
            code = request.Code,
            name = request.Name,
            displayName = request.DisplayName,
            type = request.Type,
            isActive = true,
            licenseType = request.LicenseType,
            maxUsers = request.MaxUsers,
            maxPatients = request.MaxPatients,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Kurum güncelle
    /// Ne: Mevcut kurum bilgilerini güncelleyen endpoint.
    /// Neden: Kurum bilgisi değişiklikleri için gereklidir.
    /// Kim Kullanacak: Kurum yönetimi, Ayarlar.
    /// Amacı: Kurum bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateTenantRequest request)
    {
        _logger.LogInformation("Updating tenant: {TenantId}", id);

        return Ok(new
        {
            id = id,
            name = request.Name,
            displayName = request.DisplayName,
            address = request.Address,
            phoneNumber = request.PhoneNumber,
            email = request.Email,
            isActive = request.IsActive,
            licenseType = request.LicenseType,
            maxUsers = request.MaxUsers,
            maxPatients = request.MaxPatients,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Kurum sil (soft delete)
    /// Ne: Kurumu kalıcı olarak silmek yerine pasif hale getiren endpoint.
    /// Neden: Veri bütünlüğünü korumak ve geri alma imkanı sağlamak için soft delete kullanılır.
    /// Kim Kullanacak: Sistem yöneticisi, Veri yönetimi.
    /// Amacı: Kurumun pasif hale getirilmesi.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Soft deleting tenant: {TenantId}", id);

        return NoContent();
    }

    /// <summary>
    /// Kurum logosu yükle
    /// Ne: Kuruma ait logo görselini yükleyen endpoint.
    /// Neden: Kurum kimlik görselleştirme için gereklidir.
    /// Kim Kullanacak: Kurum ayarları, Raporlama.
    /// Amacı: Kurum logosunun yüklenmesi ve saklanması.
    /// </summary>
    [HttpPost("{id:guid}/logo")]
    public IActionResult UploadLogo(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Dosya seçilmedi." });
        }

        // Dosya boyutu kontrolü (max 2MB)
        if (file.Length > 2 * 1024 * 1024)
        {
            return BadRequest(new { error = "Dosya boyutu 2MB'ı geçemez." });
        }

        // İzin verilen formatlar
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { error = "Sadece JPG, PNG ve GIF formatları desteklenir." });
        }

        _logger.LogInformation("Uploading logo for tenant: {TenantId}", id);

        // Gerçek dosya yükleme işlemi burada yapılacak
        // Şimdilik başarılı döndürüyoruz
        return Ok(new
        {
            message = "Logo başarıyla yüklendi.",
            fileName = $"tenant-{id}{extension}"
        });
    }
}

/// <summary>
/// Kurum türü enum
/// Ne: Kurum türlerini tanımlayan enum.
/// Neden: Hastane, klinik, sağlık grubu gibi farklı kurum türlerini belirlemek için gereklidir.
/// Kim Kullanacak: Kurum yönetimi, Lisanslama.
/// Amacı: Kurum türlerinin tip güvenli olarak yönetilmesi.
/// </summary>
public enum TenantType
{
    Hospital = 1,
    Clinic = 2,
    HealthGroup = 3,
    DiagnosticCenter = 4
}

/// <summary>
/// Lisans türü enum
/// Ne: Lisans türlerini tanımlayan enum.
/// Neden: Farklı lisans seviyelerini (Enterprise, Standard, Basic) belirlemek için gereklidir.
/// Kim Kullanacak: Lisanslama, Özellik yönetimi.
/// Amacı: Lisans türlerinin tip güvenli olarak yönetilmesi.
/// </summary>
public enum LicenseType
{
    Basic = 1,
    Standard = 2,
    Enterprise = 3,
    Trial = 4
}

/// <summary>
/// Kurum oluşturma istek modeli
/// Ne: Kurum oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden kurum verisi almak için gereklidir.
/// Kim Kullanacak: Sistem yöneticisi, Kurum oluşturma sihirbazı.
/// Amacı: Yeni kurum oluşturma parametrelerini taşımak.
/// </summary>
public class CreateTenantRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public TenantType Type { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public LicenseType LicenseType { get; set; }
    public int MaxUsers { get; set; }
    public int MaxPatients { get; set; }
}

/// <summary>
/// Kurum güncelleme istek modeli
/// Ne: Kurum güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut kurum bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Kurum yönetimi, Ayarlar.
/// Amacı: Kurum güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public LicenseType LicenseType { get; set; }
    public int MaxUsers { get; set; }
    public int MaxPatients { get; set; }
}
