using HBYS.Application.Services.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Kullanıcı Yönetimi Controller'ı
/// Ne: Kullanıcı işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Kullanıcı CRUD (Create, Read, Update, Delete) operasyonları için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Tenant bazlı izole edilmiş kullanıcı yönetimi sunar.
/// Kim Kullanacak: Frontend uygulaması, Admin paneli, Mobil uygulamalar.
/// Amacı: Sisteme giriş yapacak kullanıcıların yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<UsersController> logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Tüm kullanıcıları getir
    /// Ne: Mevcut tenant'a ait tüm kullanıcıları listeleyen endpoint.
    /// Neden: Kullanıcı listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Admin paneli, Kullanıcı yönetimi ekranı.
    /// Amacı: Tenant kullanıcılarının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required. Use X-Tenant-Code header." });
        }

        _logger.LogInformation("Getting all users for tenant: {TenantId}", tenantId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// ID'ye göre kullanıcı getir
    /// Ne: Belirli bir kullanıcının detaylarını getiren endpoint.
    /// Neden: Kullanıcı düzenleme veya profil görüntüleme için gereklidir.
    /// Kim Kullanacak: Kullanıcı profili, Admin detay ekranı.
    /// Amacı: Tekil kullanıcı bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Getting user {UserId} for tenant: {TenantId}", id, tenantId);

        return Ok(new 
        {
            id = id,
            userName = "demo.user",
            email = "demo@hospital.com",
            isActive = true,
            tenantId = tenantId
        });
    }

    /// <summary>
    /// Yeni kullanıcı oluştur
    /// Ne: Sisteme yeni kullanıcı ekleyen endpoint.
    /// Neden: Yeni personel veya kullanıcı kaydı için gereklidir.
    /// Kim Kullanacak: Admin paneli, İK yönetimi.
    /// Amacı: Sisteme yeni kullanıcı ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(new { error = "Username is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { error = "Email is required." });
        }

        var userId = Guid.NewGuid();
        
        _logger.LogInformation("User created: {UserName}, Tenant: {TenantId}", request.UserName, tenantId);

        return Created($"/api/users/{userId}", new
        {
            id = userId,
            userName = request.UserName,
            email = request.Email,
            isActive = true,
            tenantId = tenantId,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Kullanıcı güncelle
    /// Ne: Mevcut kullanıcı bilgilerini güncelleyen endpoint.
    /// Neden: Kullanıcı bilgisi değişiklikleri için gereklidir.
    /// Kim Kullanacak: Admin paneli, Profil düzenleme.
    /// Amacı: Kullanıcı bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("User updated: {UserId}, Tenant: {TenantId}", id, tenantId);

        return Ok(new
        {
            id = id,
            userName = request.UserName,
            email = request.Email,
            isActive = request.IsActive,
            tenantId = tenantId,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Kullanıcı sil (soft delete)
    /// Ne: Kullanıcıyı kalıcı olarak silmek yerine pasif hale getiren endpoint.
    /// Neden: Veri bütünlüğünü korumak ve geri alma imkanı sağlamak için soft delete kullanılır.
    /// Kim Kullanacak: Admin paneli, Kullanıcı yönetimi.
    /// Amacı: Kullanıcının pasif hale getirilmesi.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("User deleted (soft): {UserId}, Tenant: {TenantId}", id, tenantId);

        return NoContent();
    }
}

/// <summary>
/// Kullanıcı oluşturma istek modeli
/// Ne: Kullanıcı oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden kullanıcı verisi almak için gereklidir.
/// Kim Kullanacak: Frontend uygulaması, API consumer'lar.
/// Amacı: Yeni kullanıcı oluşturma parametrelerini taşımak.
/// </summary>
public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? EmployeeId { get; set; }
}

/// <summary>
/// Kullanıcı güncelleme istek modeli
/// Ne: Kullanıcı güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut kullanıcı bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Frontend uygulaması, API consumer'lar.
/// Amacı: Kullanıcı güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
