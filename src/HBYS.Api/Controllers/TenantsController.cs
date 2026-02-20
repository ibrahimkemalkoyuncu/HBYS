using HBYS.Application.Services.Tenant;
using HBYS.Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Tenant controller.
/// Ne: Tenant yönetimi için API endpoint'leri.
/// Kim Kullanacak: Frontend uygulaması.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<TenantsController> logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Mevcut tenant bilgisini getir
    /// </summary>
    [HttpGet("current")]
    public IActionResult GetCurrentTenant()
    {
        var tenantId = _tenantContextAccessor.TenantId;
        var tenantCode = _tenantContextAccessor.TenantCode;

        if (tenantId == null)
        {
            return Ok(new 
            { 
                message = "No tenant context set. Use X-Tenant-Code header to set tenant.",
                example = "X-Tenant-Code: DEMO"
            });
        }

        return Ok(new 
        { 
            tenantId, 
            tenantCode 
        });
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            application = "HBYS API",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Test endpoint - Tenant oluşturma simülasyonu
    /// </summary>
    [HttpPost("test")]
    public IActionResult CreateTestTenant([FromBody] CreateTenantRequest request)
    {
        var tenant = Tenant.Create(
            request.TenantCode,
            request.Name,
            request.DisplayName,
            (TenantType)request.TenantType
        );

        _logger.LogInformation("Test tenant created: {TenantCode}, {Name}", tenant.TenantCode, tenant.Name);

        return CreatedAtAction(nameof(GetCurrentTenant), new { id = tenant.Id }, new
        {
            tenant.Id,
            tenant.TenantCode,
            tenant.Name,
            tenant.DisplayName,
            tenant.TenantType,
            tenant.IsActive,
            tenant.CreatedAt
        });
    }
}

/// <summary>
/// Tenant oluşturma request modeli
/// </summary>
public class CreateTenantRequest
{
    public string TenantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TenantType { get; set; } = 1; // 1=SaaS, 2=OnPrem, 3=Group
}
