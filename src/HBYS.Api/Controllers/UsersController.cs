using HBYS.Application.Services.Tenant;
using HBYS.Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Users Controller
/// Ne: Kullanıcı yönetimi için API endpoint'leri.
/// Kim Kullanacak: Frontend uygulaması, Admin paneli.
/// Amacı: Kullanıcı CRUD işlemleri.
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
    /// Tüm kullanıcıları getir (mevcut tenant için)
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

        // Demo: Boş liste döndür (gerçek implementasyonda DB'den çekilecek)
        return Ok(new List<object>());
    }

    /// <summary>
    /// ID'ye göre kullanıcı getir
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

        // Demo response
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

        // Demo: Kullanıcı oluştur (gerçek implementasyonda DB'ye kaydedilecek)
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
/// Create user request modeli
/// </summary>
public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? EmployeeId { get; set; }
}

/// <summary>
/// Update user request modeli
/// </summary>
public class UpdateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
