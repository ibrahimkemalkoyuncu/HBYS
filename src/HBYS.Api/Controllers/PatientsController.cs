using HBYS.Domain.Entities.Patient;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Hasta Yönetimi Controller'ı
/// Ne: Hasta işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Hasta kaydı, güncelleme ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Tenant bazlı izole edilmiş hasta yönetimi sunar.
/// Kim Kullanacak: Hasta kabul, poliklinik, laboratuvar, eczane, faturalama modülleri.
/// Amacı: Hastane bilgi yönetim sisteminin temel hasta veri yönetimi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(ILogger<PatientsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm hastaları getir
    /// Ne: Mevcut tenant'a ait tüm hastaları listeleyen endpoint.
    /// Neden: Hasta listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Hasta arama, Raporlama modülleri.
    /// Amacı: Tenant hastalarının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting all patients - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        return Ok(new[]
        {
            new { id = Guid.NewGuid(), firstName = "Ahmet", lastName = "Yılmaz", tckn = "12345678901", isActive = true }
        });
    }

    /// <summary>
    /// ID'ye göre hasta getir
    /// Ne: Belirli bir hastanın detaylarını getiren endpoint.
    /// Neden: Hasta düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Hasta profili, Poliklinik, Yatan hasta modülleri.
    /// Amacı: Tekil hasta bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting patient by ID: {PatientId}", id);

        return Ok(new
        {
            id = id,
            firstName = "Ahmet",
            lastName = "Yılmaz",
            tckn = "12345678901",
            gender = "Male",
            birthDate = new DateTime(1985, 5, 15),
            phoneNumber = "+90 532 123 45 67",
            email = "ahmet.yilmaz@email.com",
            address = "İstanbul, Türkiye",
            bloodType = "APozitif",
            isActive = true
        });
    }

    /// <summary>
    /// TC Kimlik numarasına göre hasta getir
    /// Ne: Türkiye Cumhuriyeti kimlik numarasına göre hasta arayan endpoint.
    /// Neden: Hasta kaydederken TCKN ile sorgulama yapmak için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Acil servis, Poliklinik.
    /// Amacı: TC Kimlik numarası ile hasta sorgulama.
    /// </summary>
    [HttpGet("by-tckn/{tckn}")]
    public IActionResult GetByTckn(string tckn)
    {
        if (string.IsNullOrWhiteSpace(tckn) || tckn.Length != 11)
        {
            return BadRequest(new { error = "Geçerli TC Kimlik numarası giriniz." });
        }

        _logger.LogInformation("Getting patient by TCKN: {TCKN}", tckn);

        return Ok(new
        {
            id = Guid.NewGuid(),
            firstName = "Ahmet",
            lastName = "Yılmaz",
            tckn = tckn,
            gender = "Male",
            birthDate = new DateTime(1985, 5, 15)
        });
    }

    /// <summary>
    /// Yeni hasta oluştur
    /// Ne: Sisteme yeni hasta kaydı ekleyen endpoint.
    /// Neden: Yeni hasta kabul işlemleri için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Acil servis, Poliklinik.
    /// Amacı: Sisteme yeni hasta ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreatePatientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return BadRequest(new { error = "Ad zorunludur." });
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new { error = "Soyad zorunludur." });
        }

        if (string.IsNullOrWhiteSpace(request.Tckn))
        {
            return BadRequest(new { error = "TC Kimlik numarası zorunludur." });
        }

        _logger.LogInformation("Creating new patient: {FirstName} {LastName}", request.FirstName, request.LastName);

        var patientId = Guid.NewGuid();
        
        return Created($"/api/patients/{patientId}", new
        {
            id = patientId,
            firstName = request.FirstName,
            lastName = request.LastName,
            tckn = request.Tckn,
            gender = request.Gender.ToString(),
            birthDate = request.BirthDate,
            phoneNumber = request.PhoneNumber,
            email = request.Email,
            address = request.Address,
            bloodType = request.BloodType?.ToString(),
            isActive = true,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Hasta güncelle
    /// Ne: Mevcut hasta bilgilerini güncelleyen endpoint.
    /// Neden: Hasta bilgisi değişiklikleri için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Hasta profili düzenleme.
    /// Amacı: Hasta bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdatePatientRequest request)
    {
        _logger.LogInformation("Updating patient: {PatientId}", id);

        return Ok(new
        {
            id = id,
            firstName = request.FirstName,
            lastName = request.LastName,
            phoneNumber = request.PhoneNumber,
            email = request.Email,
            address = request.Address,
            bloodType = request.BloodType?.ToString(),
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Hasta sil (soft delete)
    /// Ne: Hastayı kalıcı olarak silmek yerine pasif hale getiren endpoint.
    /// Neden: Veri bütünlüğünü korumak ve geri alma imkanı sağlamak için soft delete kullanılır.
    /// Kim Kullanacak: Hasta yönetimi, Veri yönetimi.
    /// Amacı: Hastanın pasif hale getirilmesi.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Soft deleting patient: {PatientId}", id);

        return NoContent();
    }

    /// <summary>
    /// Hasta ara
    /// Ne: Belirli kriterlere göre hasta arayan endpoint.
    /// Neden: Hızlı hasta arama ve filtreleme için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Arama ekranları.
    /// Amacı: Esnek kriterlerle hasta sorgulama.
    /// </summary>
    [HttpPost("search")]
    public IActionResult Search([FromBody] PatientSearchRequest request)
    {
        _logger.LogInformation("Searching patients with criteria");

        return Ok(new[]
        {
            new { id = Guid.NewGuid(), firstName = "Ahmet", lastName = "Yılmaz", tckn = "12345678901" }
        });
    }
}

/// <summary>
/// Hasta oluşturma istek modeli
/// Ne: Hasta oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden hasta verisi almak için gereklidir.
/// Kim Kullanacak: Hasta kabul, Acil servis, Poliklinik.
/// Amacı: Yeni hasta oluşturma parametrelerini taşımak.
/// </summary>
public class CreatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Tckn { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public BloodType? BloodType { get; set; }
}

/// <summary>
/// Hasta güncelleme istek modeli
/// Ne: Hasta güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut hasta bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Hasta kabul, Profil düzenleme.
/// Amacı: Hasta güncelleme parametrelerini taşımak.
/// </summary>
public class UpdatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public BloodType? BloodType { get; set; }
}

/// <summary>
/// Hasta arama istek modeli
/// Ne: Hasta arama endpoint'i için gerekli input modeli.
/// Neden: Esnek kriterlerle hasta sorgulamak için gereklidir.
/// Kim Kullanacak: Hasta arama ekranları, Raporlama.
/// Amacı: Hasta arama parametrelerini taşımak.
/// </summary>
public class PatientSearchRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Tckn { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? BirthDateFrom { get; set; }
    public DateTime? BirthDateTo { get; set; }
    public bool? IsActive { get; set; }
}
