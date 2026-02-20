using HBYS.Application.Services.Tenant;
using HBYS.Domain.Entities.License;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Licenses Controller
/// Ne: Lisans yönetimi için API endpoint'leri.
/// Kim Kullanacak: Admin paneli, Sistem yöneticisi.
/// Amacı: Tenant lisans yönetimi ve feature flag kontrolü.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<LicensesController> logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Mevcut lisans bilgilerini getir
    /// </summary>
    [HttpGet("current")]
    public IActionResult GetCurrentLicense()
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Getting license for tenant: {TenantId}", tenantId);

        // Demo license response
        return Ok(new
        {
            tenantId = tenantId,
            licenseType = "SaaS",
            maxUsers = 100,
            maxStorageGb = 500,
            expiresAt = DateTime.UtcNow.AddYears(1),
            isActive = true,
            features = new[]
            {
                new { name = "PatientManagement", enabled = true },
                new { name = "Appointment", enabled = true },
                new { name = "Billing", enabled = true },
                new { name = "Laboratory", enabled = true },
                new { name = "Radiology", enabled = true },
                new { name = "Pharmacy", enabled = false },
                new { name = "Inpatient", enabled = false }
            }
        });
    }

    /// <summary>
    /// Feature flag kontrolü
    /// </summary>
    [HttpGet("features/{featureName}")]
    public IActionResult CheckFeature(string featureName)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Checking feature {Feature} for tenant: {TenantId}", featureName, tenantId);

        // Demo: Tüm feature'lar aktif
        return Ok(new
        {
            featureName = featureName,
            enabled = true,
            tenantId = tenantId
        });
    }

    /// <summary>
    /// Tüm feature'ları listele
    /// </summary>
    [HttpGet("features")]
    public IActionResult GetAllFeatures()
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        var features = new[]
        {
            new { name = "PatientManagement", displayName = "Hasta Yönetimi", enabled = true },
            new { name = "Appointment", displayName = "Randevu", enabled = true },
            new { name = "Outpatient", displayName = "Poliklinik", enabled = true },
            new { name = "Inpatient", displayName = "Yatan Hasta", enabled = true },
            new { name = "Emergency", displayName = "Acil", enabled = true },
            new { name = "Billing", displayName = "Faturalandırma", enabled = true },
            new { name = "Laboratory", displayName = "Laboratuvar", enabled = true },
            new { name = "Radiology", displayName = "Radyoloji", enabled = true },
            new { name = "Pharmacy", displayName = "Eczane", enabled = true },
            new { name = "Inventory", displayName = "Stok Yönetimi", enabled = true },
            new { name = "Procurement", displayName = "Satın Alma", enabled = true },
            new { name = "HR", displayName = "İnsan Kaynakları", enabled = false },
            new { name = "Accounting", displayName = "Muhasebe", enabled = false },
            new { name = "Reporting", displayName = "Raporlama", enabled = false }
        };

        return Ok(features);
    }

    /// <summary>
    /// Lisans yenileme talebi (demo)
    /// </summary>
    [HttpPost("renew")]
    public IActionResult RenewLicense([FromBody] RenewLicenseRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("License renewal requested for tenant: {TenantId}", tenantId);

        return Ok(new
        {
            message = "License renewal request received.",
            tenantId = tenantId,
            requestedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Lisans usage raporu
    /// </summary>
    [HttpGet("usage")]
    public IActionResult GetUsage()
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        return Ok(new
        {
            tenantId = tenantId,
            currentUsers = 15,
            maxUsers = 100,
            storageUsedGb = 45.5,
            maxStorageGb = 500,
            apiCallsThisMonth = 15234,
            lastUpdated = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Renew license request modeli
/// </summary>
public class RenewLicenseRequest
{
    public string LicenseType { get; set; } = "SaaS";
    public int DurationMonths { get; set; } = 12;
}
