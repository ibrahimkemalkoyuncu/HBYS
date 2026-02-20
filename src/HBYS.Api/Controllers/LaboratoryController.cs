using HBYS.Domain.Entities.Laboratory;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Laboratuvar Controller Sınıfı
/// Ne: Laboratuvar modülünün HTTP API endpoint'lerini yöneten controller sınıfıdır.
/// Neden: Laboratuvar işlemlerinin RESTful API üzerinden erişilebilir olması için gereklidir.
/// Özelliği: LabRequest, LabTest, LabResult yönetimi.
/// Kim Kullanacak: Laboratuvar, Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Laboratuvar süreçlerinin API üzerinden yönetimi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LaboratoryController : ControllerBase
{
    private readonly ILogger<LaboratoryController> _logger;

    public LaboratoryController(ILogger<LaboratoryController> logger)
    {
        _logger = logger;
    }

    // Lab Requests

    /// <summary>
    /// Tüm laboratuvar taleplerini getirir
    /// Ne: Sistemdeki tüm laboratuvar taleplerini listeler.
/// Neden: Talep listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar, Yönetim.
/// Amacı: Talep listesi sunma.
/// </summary>
    [HttpGet("requests")]
    public IActionResult GetRequests(
        [FromQuery] Guid? patientId,
        [FromQuery] LabRequestStatus? status,
        [FromQuery] LabPriority? priority,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        _logger.LogInformation("Getting lab requests");
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre talep getirir
    /// Ne: Belirtilen ID'ye sahip laboratuvar talebini getirir.
/// Neden: Talep detay görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Talep detay sunma.
/// </summary>
    [HttpGet("requests/{id}")]
    public IActionResult GetRequestById(Guid id)
    {
        _logger.LogInformation("Getting lab request {Id}", id);
        return Ok(new { Id = id, RequestNumber = "LAB-2026-0001", Status = LabRequestStatus.Pending });
    }

    /// <summary>
    /// Yeni laboratuvar talebi oluşturur
    /// Ne: Sisteme yeni bir laboratuvar talebi ekler.
/// Neden: Talep oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Talep oluşturma.
/// </summary>
    [HttpPost("requests")]
    public IActionResult CreateRequest([FromBody] CreateLabRequestRequest request)
    {
        _logger.LogInformation("Creating lab request");
        return Created($"/api/laboratory/requests/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Numune toplama
    /// Ne: Talep için numune toplama işlemini kaydeder.
/// Neden: Numune toplama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Numune toplama.
/// </summary>
    [HttpPost("requests/{id}/collect-sample")]
    public IActionResult CollectSample(Guid id, [FromBody] CollectSampleRequest request)
    {
        _logger.LogInformation("Collecting sample for request {Id}", id);
        return Ok(new { Status = "Sample Collected" });
    }

    /// <summary>
    /// Sonuç girişi
    /// Ne: Test sonuçlarını girer.
/// Neden: Sonuç girişi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç girişi.
/// </summary>
    [HttpPost("requests/{id}/results")]
    public IActionResult EnterResults(Guid id, [FromBody] EnterResultsRequest request)
    {
        _logger.LogInformation("Entering results for request {Id}", id);
        return Ok(new { Status = "Results Entered" });
    }

    /// <summary>
    /// Sonuç onaylama
    /// Ne: Sonuçları onaylar ve raporlar.
/// Neden: Onay işlemi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç onaylama.
/// </summary>
    [HttpPost("requests/{id}/approve")]
    public IActionResult ApproveResults(Guid id)
    {
        _logger.LogInformation("Approving results for request {Id}", id);
        return Ok(new { Status = "Approved" });
    }

    /// <summary>
    /// Talep iptal
    /// Ne: Laboratuvar talebini iptal eder.
/// Neden: İptal işlemi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Talep iptal etme.
/// </summary>
    [HttpPost("requests/{id}/cancel")]
    public IActionResult CancelRequest(Guid id, [FromBody] CancelLabRequestRequest request)
    {
        _logger.LogInformation("Cancelling request {Id}", id);
        return Ok(new { Status = "Cancelled" });
    }

    // Lab Tests (Catalog)

    /// <summary>
    /// Tüm testleri getirir
    /// Ne: Laboratuvar test katalogını listeler.
/// Neden: Test listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test listesi sunma.
/// </summary>
    [HttpGet("tests")]
    public IActionResult GetTests([FromQuery] string? category, [FromQuery] bool? isActive)
    {
        _logger.LogInformation("Getting lab tests");
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre test getirir
    /// Ne: Belirtilen ID'ye sahip testi getirir.
/// Neden: Test detay görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test detay sunma.
/// </summary>
    [HttpGet("tests/{id}")]
    public IActionResult GetTestById(Guid id)
    {
        _logger.LogInformation("Getting lab test {Id}", id);
        return Ok(new { Id = id, TestCode = "TST-001", TestName = "Hemogram" });
    }

    /// <summary>
    /// Yeni test oluşturur
    /// Ne: Sisteme yeni bir test ekler.
/// Neden: Test oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Laboratuvar, Yönetim.
/// Amacı: Test oluşturma.
/// </summary>
    [HttpPost("tests")]
    public IActionResult CreateTest([FromBody] CreateLabTestRequest request)
    {
        _logger.LogInformation("Creating lab test");
        return Created($"/api/laboratory/tests/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Test günceller
    /// Ne: Mevcut bir testi günceller.
/// Neden: Test bilgisi güncelleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test güncelleme.
/// </summary>
    [HttpPut("tests/{id}")]
    public IActionResult UpdateTest(Guid id, [FromBody] UpdateLabTestRequest request)
    {
        _logger.LogInformation("Updating lab test {Id}", id);
        return Ok(new { Status = "Updated" });
    }

    // Results History

    /// <summary>
    /// Hasta sonuç geçmişini getirir
    /// Ne: Belirli bir hastanın geçmiş laboratuvar sonuçlarını getirir.
/// Neden: Hasta geçmişi görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar, Poliklinik.
/// Amacı: Hasta geçmişi sunma.
/// </summary>
    [HttpGet("patients/{patientId}/history")]
    public IActionResult GetPatientHistory(Guid patientId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        _logger.LogInformation("Getting patient {PatientId} lab history", patientId);
        return Ok(Array.Empty<object>());
    }
}

// Request DTOs

/// <summary>
/// Laboratuvar talebi oluşturma isteği
/// Ne: Talep oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden talep oluşturma için gereklidir.
/// Kim Kullanacak: Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Talep verisi taşıma.
/// </summary>
public class CreateLabRequestRequest
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? PatientIdentityNumber { get; set; }
    public string RequestingUnit { get; set; } = string.Empty;
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public LabPriority Priority { get; set; }
    public string? Notes { get; set; }
    public List<CreateLabTestItemRequest> Tests { get; set; } = new();
}

/// <summary>
/// Test kalemi oluşturma isteği
/// Ne: Talepteki test kalemlerini oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden test ekleme için gereklidir.
/// Kim Kullanacak: Poliklinik, Yatan Hasta.
/// Amacı: Test kalemi verisi taşıma.
/// </summary>
public class CreateLabTestItemRequest
{
    public Guid LabTestId { get; set; }
}

/// <summary>
/// Numune toplama isteği
/// Ne: Numune toplamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden numune toplama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Numune verisi taşıma.
/// </summary>
public class CollectSampleRequest
{
    public string SampleType { get; set; } = string.Empty;
    public string? BarcodeNumber { get; set; }
    public Guid SampleCollectedById { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Sonuç girişi isteği
/// Ne: Test sonuçlarını girmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden sonuç girişi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç verisi taşıma.
/// </summary>
public class EnterResultsRequest
{
    public List<EnterResultItemRequest> Results { get; set; } = new();
}

/// <summary>
/// Sonuç kalemi girişi isteği
/// Ne: Tek bir test sonucunu girmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden sonuç kalemi girişi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç kalemi verisi taşıma.
/// </summary>
public class EnterResultItemRequest
{
    public Guid LabTestId { get; set; }
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? ResultValue { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public bool IsNormal { get; set; } = true;
    public ResultFlag Flag { get; set; } = ResultFlag.Normal;
    public Guid EnteredById { get; set; }
    public string EnteredByName { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

/// <summary>
/// Talep iptal isteği
/// Ne: Talep iptal etmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden iptal etme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: İptal verisi taşıma.
/// </summary>
public class CancelLabRequestRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Test oluşturma isteği
/// Ne: Test oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden test oluşturma için gereklidir.
/// Kim Kullanacak: Laboratuvar, Yönetim.
/// Amacı: Test verisi taşıma.
/// </summary>
public class CreateLabTestRequest
{
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? TestNameEn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string? Unit { get; set; }
    public decimal? ReferenceLow { get; set; }
    public decimal? ReferenceHigh { get; set; }
    public string? ReferenceText { get; set; }
    public int DecimalPlaces { get; set; } = 2;
    public string? Method { get; set; }
    public string? Device { get; set; }
    public decimal Price { get; set; }
    public decimal? SgkPrice { get; set; }
    public string? Preparation { get; set; }
    public bool IsExternal { get; set; }
    public string? ExternalLab { get; set; }
}

/// <summary>
/// Test güncelleme isteği
/// Ne: Test güncellemek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden test güncelleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test verisi güncelleme.
/// </summary>
public class UpdateLabTestRequest
{
    public string? TestName { get; set; }
    public string? TestNameEn { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Unit { get; set; }
    public decimal? ReferenceLow { get; set; }
    public decimal? ReferenceHigh { get; set; }
    public string? ReferenceText { get; set; }
    public int? DecimalPlaces { get; set; }
    public string? Method { get; set; }
    public string? Device { get; set; }
    public decimal? Price { get; set; }
    public decimal? SgkPrice { get; set; }
    public string? Preparation { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsExternal { get; set; }
    public string? ExternalLab { get; set; }
}
