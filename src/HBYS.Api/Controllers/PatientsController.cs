using HBYS.Application.Services.Tenant;
using HBYS.Domain.Entities.Patient;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Patients Controller
/// Ne: Hasta yönetimi için API endpoint'leri.
/// Kim Kullanacak: Frontend uygulaması, Hasta kabul, Poliklinik.
/// Amacı: Hasta CRUD işlemleri ve hasta bilgi yönetimi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<PatientsController> logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Tüm hastaları getir (sayfalı)
    /// </summary>
    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Getting patients for tenant: {TenantId}, Page: {Page}", tenantId, page);

        // Demo: Boş liste döndür (gerçek implementasyonda DB'den çekilecek)
        return Ok(new
        {
            items = new List<object>(),
            page = page,
            pageSize = pageSize,
            totalCount = 0,
            totalPages = 0
        });
    }

    /// <summary>
    /// TCKN ile hasta ara
    /// </summary>
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? tckn, [FromQuery] string? name)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Searching patients: TCKN={TCKN}, Name={Name}", tckn, name);

        // Demo: Boş liste
        return Ok(new List<object>());
    }

    /// <summary>
    /// ID'ye göre hasta getir
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Getting patient {PatientId} for tenant: {TenantId}", id, tenantId);

        // Demo response
        return Ok(new
        {
            id = id,
            tckn = "12345678901",
            firstName = "Ahmet",
            lastName = "Yılmaz",
            birthDate = new DateTime(1985, 5, 15),
            gender = 0,
            email = "ahmet@example.com",
            phone = "0532 123 4567",
            address = "İstanbul, Türkiye",
            bloodType = "A+",
            isActive = true,
            createdAt = DateTime.UtcNow.AddMonths(-3),
            tenantId = tenantId
        });
    }

    /// <summary>
    /// Yeni hasta oluştur
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreatePatientRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        if (string.IsNullOrWhiteSpace(request.TCKN))
        {
            return BadRequest(new { error = "TCKN is required." });
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return BadRequest(new { error = "FirstName is required." });
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new { error = "LastName is required." });
        }

        // Demo: Hasta oluştur
        var patientId = Guid.NewGuid();
        
        _logger.LogInformation("Patient created: {TCKN}, Tenant: {TenantId}", request.TCKN, tenantId);

        return Created($"/api/patients/{patientId}", new
        {
            id = patientId,
            tckn = request.TCKN,
            firstName = request.FirstName,
            lastName = request.LastName,
            birthDate = request.BirthDate,
            gender = request.Gender,
            phone = request.Phone,
            email = request.Email,
            isActive = true,
            tenantId = tenantId,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Hasta güncelle
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Patient updated: {PatientId}, Tenant: {TenantId}", id, tenantId);

        return Ok(new
        {
            id = id,
            firstName = request.FirstName,
            lastName = request.LastName,
            phone = request.Phone,
            email = request.Email,
            address = request.Address,
            bloodType = request.BloodType,
            isActive = true,
            tenantId = tenantId,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Hasta sil (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        _logger.LogInformation("Patient deleted (soft): {PatientId}, Tenant: {TenantId}", id, tenantId);

        return NoContent();
    }

    /// <summary>
    /// Hasta yakını ekle
    /// </summary>
    [HttpPost("{id:guid}/contacts")]
    public IActionResult AddContact(Guid id, [FromBody] AddContactRequest request)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        
        if (tenantId == null)
        {
            return BadRequest(new { error = "Tenant context required." });
        }

        var contactId = Guid.NewGuid();
        
        _logger.LogInformation("Patient contact added: {PatientId}, Contact: {ContactId}", id, contactId);

        return Created($"/api/patients/{id}/contacts/{contactId}", new
        {
            id = contactId,
            patientId = id,
            firstName = request.FirstName,
            lastName = request.LastName,
            phone = request.Phone,
            relationship = request.Relationship,
            isEmergencyContact = request.IsEmergencyContact,
            tenantId = tenantId,
            createdAt = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Create patient request modeli
/// </summary>
public class CreatePatientRequest
{
    public string TCKN { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; } // 0: Erkek, 1: Kadın, 2: Diğer
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? BloodType { get; set; }
}

/// <summary>
/// Update patient request modeli
/// </summary>
public class UpdatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? BloodType { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Add contact request modeli
/// </summary>
public class AddContactRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Relationship { get; set; } = string.Empty;
    public bool IsEmergencyContact { get; set; }
}
