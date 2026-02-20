using HBYS.Domain.Entities.Radiology;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Radyoloji Controller Sınıfı
/// Ne: Radyoloji modülünün HTTP API endpoint'lerini yöneten controller sınıfıdır.
/// Neden: Radyoloji işlemlerinin RESTful API üzerinden erişilebilir olması için gereklidir.
/// Özelliği: RadiologyRequest, RadiologyTest yönetimi.
/// Kim Kullanacak: Radyoloji, Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Radyoloji süreçlerinin API üzerinden yönetimi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RadiologyController : ControllerBase
{
    private readonly ILogger<RadiologyController> _logger;

    public RadiologyController(ILogger<RadiologyController> logger)
    {
        _logger = logger;
    }

    // Radiology Requests

    /// <summary>
    /// Tüm radyoloji taleplerini getirir
    /// Ne: Sistemdeki tüm radyoloji taleplerini listeler.
/// Neden: Talep listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji, Yönetim.
/// Amacı: Talep listesi sunma.
/// </summary>
    [HttpGet("requests")]
    public IActionResult GetRequests(
        [FromQuery] Guid? patientId,
        [FromQuery] RadiologyRequestStatus? status,
        [FromQuery] RadiologyPriority? priority,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        _logger.LogInformation("Getting radiology requests");
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre talep getirir
    /// Ne: Belirtilen ID'ye sahip radyoloji talebini getirir.
/// Neden: Talep detay görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Talep detay sunma.
/// </summary>
    [HttpGet("requests/{id}")]
    public IActionResult GetRequestById(Guid id)
    {
        _logger.LogInformation("Getting radiology request {Id}", id);
        return Ok(new { Id = id, RequestNumber = "RAD-2026-0001", Status = RadiologyRequestStatus.Pending });
    }

    /// <summary>
    /// Yeni radyoloji talebi oluşturur
    /// Ne: Sisteme yeni bir radyoloji talebi ekler.
/// Neden: Talep oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Talep oluşturma.
/// </summary>
    [HttpPost("requests")]
    public IActionResult CreateRequest([FromBody] CreateRadiologyRequestRequest request)
    {
        _logger.LogInformation("Creating radiology request");
        return Created($"/api/radiology/requests/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Randevu planlama
    /// Ne: Talep için randevu planlar.
/// Neden: Randevu planlama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Randevu planlama.
/// </summary>
    [HttpPost("requests/{id}/schedule")]
    public IActionResult ScheduleRequest(Guid id, [FromBody] ScheduleRadiologyRequestRequest request)
    {
        _logger.LogInformation("Scheduling radiology request {Id}", id);
        return Ok(new { Status = "Scheduled" });
    }

    /// <summary>
    /// Çekim tamamlama
    /// Ne: Radyoloji çekimini tamamlar.
/// Neden: Çekim tamamlama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Çekim tamamlama.
/// </summary>
    [HttpPost("requests/{id}/complete")]
    public IActionResult CompleteStudy(Guid id, [FromBody] CompleteStudyRequest request)
    {
        _logger.LogInformation("Completing study for request {Id}", id);
        return Ok(new { Status = "Study Completed" });
    }

    /// <summary>
    /// Rapor yazma
    /// Ne: Radyoloji raporunu yazar.
/// Neden: Rapor yazma için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor yazma.
/// </summary>
    [HttpPost("requests/{id}/report")]
    public IActionResult WriteReport(Guid id, [FromBody] WriteReportRequest request)
    {
        _logger.LogInformation("Writing report for request {Id}", id);
        return Ok(new { Status = "Report Written" });
    }

    /// <summary>
    /// Rapor onaylama
    /// Ne: Radyoloji raporunu onaylar.
/// Neden: Onay işlemi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor onaylama.
/// </summary>
    [HttpPost("requests/{id}/approve")]
    public IActionResult ApproveReport(Guid id)
    {
        _logger.LogInformation("Approving report for request {Id}", id);
        return Ok(new { Status = "Approved" });
    }

    /// <summary>
    /// Kritik bulgu bildirimi
    /// Ne: Kritik bulguyu bildirir.
/// Neden: Acil bildirim için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kritik bulgu bildirimi.
/// </summary>
    [HttpPost("requests/{id}/critical-finding")]
    public IActionResult ReportCriticalFinding(Guid id, [FromBody] ReportCriticalFindingRequest request)
    {
        _logger.LogInformation("Reporting critical finding for request {Id}", id);
        return Ok(new { Status = "Critical Finding Reported" });
    }

    /// <summary>
    /// Talep iptal
    /// Ne: Radyoloji talebini iptal eder.
/// Neden: İptal işlemi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Talep iptal etme.
/// </summary>
    [HttpPost("requests/{id}/cancel")]
    public IActionResult CancelRequest(Guid id, [FromBody] CancelRadiologyRequestRequest request)
    {
        _logger.LogInformation("Cancelling request {Id}", id);
        return Ok(new { Status = "Cancelled" });
    }

    // Radiology Tests (Catalog)

    /// <summary>
    /// Tüm tetkikleri getirir
    /// Ne: Radyoloji tetkik katalogını listeler.
/// Neden: Tetkik listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik listesi sunma.
/// </summary>
    [HttpGet("tests")]
    public IActionResult GetTests([FromQuery] string? category, [FromQuery] string? modality, [FromQuery] bool? isActive)
    {
        _logger.LogInformation("Getting radiology tests");
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre tetkik getirir
    /// Ne: Belirtilen ID'ye sahip tetkiği getirir.
/// Neden: Tetkik detay görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik detay sunma.
/// </summary>
    [HttpGet("tests/{id}")]
    public IActionResult GetTestById(Guid id)
    {
        _logger.LogInformation("Getting radiology test {Id}", id);
        return Ok(new { Id = id, TestCode = "RAD-001", TestName = "Akciğer Grafisi" });
    }

    /// <summary>
    /// Yeni tetkik oluşturur
    /// Ne: Sisteme yeni bir tetkik ekler.
/// Neden: Tetkik oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Radyoloji, Yönetim.
/// Amacı: Tetkik oluşturma.
/// </summary>
    [HttpPost("tests")]
    public IActionResult CreateTest([FromBody] CreateRadiologyTestRequest request)
    {
        _logger.LogInformation("Creating radiology test");
        return Created($"/api/radiology/tests/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Tetkik günceller
    /// Ne: Mevcut bir tetkiği günceller.
/// Neden: Tetkik bilgisi güncelleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik güncelleme.
/// </summary>
    [HttpPut("tests/{id}")]
    public IActionResult UpdateTest(Guid id, [FromBody] UpdateRadiologyTestRequest request)
    {
        _logger.LogInformation("Updating radiology test {Id}", id);
        return Ok(new { Status = "Updated" });
    }

    // Patient History

    /// <summary>
    /// Hasta görüntüleme geçmişini getirir
    /// Ne: Belirli bir hastanın geçmiş radyoloji görüntülerini getirir.
/// Neden: Hasta geçmişi görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji, Poliklinik.
/// Amacı: Hasta geçmişi sunma.
/// </summary>
    [HttpGet("patients/{patientId}/history")]
    public IActionResult GetPatientHistory(Guid patientId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        _logger.LogInformation("Getting patient {PatientId} radiology history", patientId);
        return Ok(Array.Empty<object>());
    }
}

// Request DTOs

/// <summary>
/// Radyoloji talebi oluşturma isteği
/// Ne: Talep oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden talep oluşturma için gereklidir.
/// Kim Kullanacak: Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Talep verisi taşıma.
/// </summary>
public class CreateRadiologyRequestRequest
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? PatientIdentityNumber { get; set; }
    public DateTime? PatientBirthDate { get; set; }
    public string? Gender { get; set; }
    public string RequestingUnit { get; set; } = string.Empty;
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public Guid RadiologyTestId { get; set; }
    public string RadiologyTestName { get; set; } = string.Empty;
    public string? ClinicalInformation { get; set; }
    public string? PreviousStudy { get; set; }
    public RadiologyPriority Priority { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Randevu planlama isteği
/// Ne: Randevu planlamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden randevu planlama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Randevu verisi taşıma.
/// </summary>
public class ScheduleRadiologyRequestRequest
{
    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public string? Device { get; set; }
    public Guid? ScheduledById { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Çekim tamamlama isteği
/// Ne: Çekimi tamamlamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden çekim tamamlama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Çekim verisi taşıma.
/// </summary>
public class CompleteStudyRequest
{
    public DateTime StudyDate { get; set; }
    public Guid PerformedById { get; set; }
    public string? AccessionNumber { get; set; }
    public string? StudyInstanceUid { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Rapor yazma isteği
/// Ne: Rapor yazmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden rapor yazma için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor verisi taşıma.
/// </summary>
public class WriteReportRequest
{
    public Guid RadiologistId { get; set; }
    public string RadiologistName { get; set; } = string.Empty;
    public string Report { get; set; } = string.Empty;
    public bool HasCriticalFinding { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Kritik bulgu bildirimi isteği
/// Ne: Kritik bulgu bildirmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden kritik bulgu bildirme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kritik bulgu verisi taşıma.
/// </summary>
public class ReportCriticalFindingRequest
{
    public string CriticalFinding { get; set; } = string.Empty;
    public Guid ReportedById { get; set; }
    public DateTime ReportedDate { get; set; }
    public bool PatientNotified { get; set; }
    public string? NotifiedBy { get; set; }
    public DateTime? NotificationDate { get; set; }
}

/// <summary>
/// Talep iptal isteği
/// Ne: Talep iptal etmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden iptal etme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: İptal verisi taşıma.
/// </summary>
public class CancelRadiologyRequestRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Tetkik oluşturma isteği
/// Ne: Tetkik oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden tetkik oluşturma için gereklidir.
/// Kim Kullanacak: Radyoloji, Yönetim.
/// Amacı: Tetkik verisi taşıma.
/// </summary>
public class CreateRadiologyTestRequest
{
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? TestNameEn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Modality { get; set; } = string.Empty;
    public string? SopClassUid { get; set; }
    public int DurationMinutes { get; set; }
    public string? Preparation { get; set; }
    public bool RequiresContrast { get; set; }
    public string? ContrastPreparation { get; set; }
    public string? FastingInfo { get; set; }
    public decimal Price { get; set; }
    public decimal? SgkPrice { get; set; }
    public string? DeviceRequirement { get; set; }
    public bool RequiresRadiologist { get; set; } = true;
    public bool IsExternal { get; set; }
    public string? ExternalCenter { get; set; }
}

/// <summary>
/// Tetkik güncelleme isteği
/// Ne: Tetkik güncellemek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden tetkik güncelleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik verisi güncelleme.
/// </summary>
public class UpdateRadiologyTestRequest
{
    public string? TestName { get; set; }
    public string? TestNameEn { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Modality { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Preparation { get; set; }
    public bool? RequiresContrast { get; set; }
    public string? ContrastPreparation { get; set; }
    public string? FastingInfo { get; set; }
    public decimal? Price { get; set; }
    public decimal? SgkPrice { get; set; }
    public string? DeviceRequirement { get; set; }
    public bool? RequiresRadiologist { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsExternal { get; set; }
    public string? ExternalCenter { get; set; }
}
