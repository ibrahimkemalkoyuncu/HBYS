using HBYS.Domain.Entities.Emergency;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Acil Servis Yönetimi Controller'ı
/// Ne: Acil servis işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Acil hasta kabul, triaj, müdahale ve çıkış işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Triaj değerlendirmesi, acil müdahale kaydı, sevk/taburculuk yönetimi sunar.
/// Kim Kullanacak: Acil servis, Doktor, Hemşire, Ameliyathane.
/// Amacı: Acil servis sürecinin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmergenciesController : ControllerBase
{
    private readonly ILogger<EmergenciesController> _logger;

    public EmergenciesController(ILogger<EmergenciesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm acil başvuruları getir
    /// Ne: Mevcut tenant'a ait tüm acil başvuruları listeleyen endpoint.
    /// Neden: Acil servis listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Acil servis, Raporlama.
    /// Amacı: Tenant acil başvurularının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] EmergencyStatus? status,
        [FromQuery] TriageCategory? triageCategory,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting emergencies - Page: {Page}, Status: {Status}", page, status);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                emergencyNumber = "AS-2024-0001",
                patientName = "Ayşe Yıldız",
                arrivalDateTime = DateTime.Now.AddHours(-2),
                triageCategory = "Orange",
                responsibleDoctorName = "Dr. Ali Kaya",
                treatmentRoom = "Müdahale Odası 1",
                status = "UnderIntervention"
            }
        });
    }

    /// <summary>
    /// ID'ye göre acil başvuru getir
    /// Ne: Belirli bir acil başvurunun detaylarını getiren endpoint.
    /// Neden: Acil başvuru düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Acil servis, Doktor, Hemşire.
    /// Amacı: Tekil acil başvuru bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting emergency by ID: {EmergencyId}", id);

        return Ok(new
        {
            id = id,
            emergencyNumber = "AS-2024-0001",
            patientId = Guid.NewGuid(),
            patientName = "Ayşe Yıldız",
            patientTckn = "98765432109",
            arrivalDateTime = DateTime.Now.AddHours(-2),
            arrivalType = "ByAmbulance",
            reasonForVisit = "Göğüs ağrısı",
            triageScore = 2,
            triageCategory = "Orange",
            triageNote = "Acil değerlendirme gerekli",
            treatmentRoom = "Müdahale Odası 1",
            bed = "A-Yatak 1",
            responsibleDoctorId = Guid.NewGuid(),
            responsibleDoctorName = "Dr. Ali Kaya",
            vitalSigns = "TA: 140/90, Nabız: 90, Ateş: 37.2",
            complaint = "Şiddetli göğüs ağrısı, nefes darlığı",
            status = "UnderIntervention",
            totalStayMinutes = 120
        });
    }

    /// <summary>
    /// Aktif acil başvuruları getir
    /// Ne: Halihazırda acil serviste işlem gören hastaları listeleyen endpoint.
    /// Neden: Acil servis doluluk takibi için gereklidir.
    /// Kim Kullanacak: Acil servis, Yönetim.
    /// Amacı: Aktif başvuruların görüntülenmesi.
    /// </summary>
    [HttpGet("active")]
    public IActionResult GetActive()
    {
        _logger.LogInformation("Getting active emergencies");

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                emergencyNumber = "AS-2024-0001",
                patientName = "Ayşe Yıldız",
                arrivalDateTime = DateTime.Now.AddHours(-2),
                triageCategory = "Orange",
                treatmentRoom = "Müdahale Odası 1",
                status = "UnderIntervention",
                waitTimeMinutes = 120
            }
        });
    }

    /// <summary>
    /// Yeni acil başvuru oluştur
    /// Ne: Sisteme yeni acil başvuru ekleyen endpoint.
    /// Neden: Acil servis hasta kabul işlemleri için gereklidir.
    /// Kim Kullanacak: Acil servis, Triaj hemşiresi.
    /// Amacı: Sisteme yeni acil başvuru ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateEmergencyRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        _logger.LogInformation("Creating new emergency for patient: {PatientId}", request.PatientId);

        var emergencyId = Guid.NewGuid();
        var emergencyNumber = $"AS-{DateTime.Now:yyyyMMddHHmmss}";

        return Created($"/api/emergencies/{emergencyId}", new
        {
            id = emergencyId,
            emergencyNumber = emergencyNumber,
            patientId = request.PatientId,
            arrivalDateTime = DateTime.UtcNow,
            arrivalType = request.ArrivalType.ToString(),
            reasonForVisit = request.ReasonForVisit,
            status = "Waiting",
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Triaj yap
    /// Ne: Hastanın triaj değerlendirmesini kaydeden endpoint.
    /// Neden: Triaj işlemleri için gereklidir.
    /// Kim Kullanacak: Triaj hemşiresi.
    /// Amacı: Triaj değerlendirmesi kaydetme.
    /// </summary>
    [HttpPost("{id:guid}/triage")]
    public IActionResult PerformTriage(Guid id, [FromBody] PerformTriageRequest request)
    {
        _logger.LogInformation("Performing triage for emergency: {EmergencyId}, Category: {Category}", 
            id, request.TriageCategory);

        return Ok(new
        {
            id = id,
            triageScore = request.TriageScore,
            triageCategory = request.TriageCategory.ToString(),
            triageNote = request.TriageNote,
            triageNurseId = request.TriageNurseId,
            triageTime = DateTime.UtcNow,
            status = "InExamination",
            message = "Triaj başarıyla tamamlandı."
        });
    }

    /// <summary>
    /// Muayene/gözlem güncelle
    /// Ne: Hastanın muayene ve gözlem bilgilerini güncelleyen endpoint.
    /// Neden: Muayene kaydı için gereklidir.
    /// Kim Kullanacak: Doktor, Hemşire.
    /// Amacı: Muayene bilgilerini güncelleme.
    /// </summary>
    [HttpPut("{id:guid}/examination")]
    public IActionResult UpdateExamination(Guid id, [FromBody] UpdateExaminationRequest request)
    {
        _logger.LogInformation("Updating examination for emergency: {EmergencyId}", id);

        return Ok(new
        {
            id = id,
            vitalSigns = request.VitalSigns,
            complaint = request.Complaint,
            diagnosis = request.Diagnosis,
            treatmentPlan = request.TreatmentPlan,
            procedures = request.Procedures,
            medicationsGiven = request.MedicationsGiven,
            firstExaminationTime = DateTime.UtcNow,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Durum güncelle
    /// Ne: Acil başvuru durumunu güncelleyen endpoint.
    /// Neden: Durum değişiklikleri için gereklidir.
    /// Kim Kullanacak: Acil servis.
    /// Amacı: Durum güncelleme.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        _logger.LogInformation("Updating emergency status: {EmergencyId}, Status: {Status}", id, request.Status);

        return Ok(new
        {
            id = id,
            status = request.Status.ToString(),
            treatmentRoom = request.TreatmentRoom,
            message = "Durum başarıyla güncellendi."
        });
    }

    /// <summary>
    /// Taburculuk/çıkış işlemi
    /// Ne: Hastanın acil servisten çıkışını kaydeden endpoint.
    /// Neden: Çıkış işlemleri için gereklidir.
    /// Kim Kullanacak: Doktor.
    /// Amacı: Çıkış işlemi.
    /// </summary>
    [HttpPost("{id:guid}/discharge")]
    public IActionResult Discharge(Guid id, [FromBody] DischargeRequest request)
    {
        _logger.LogInformation("Discharging emergency: {EmergencyId}, Type: {Type}", id, request.DischargeType);

        var totalMinutes = 180; // Örnek süre

        return Ok(new
        {
            id = id,
            dischargeDateTime = DateTime.UtcNow,
            dischargeType = request.DischargeType.ToString(),
            dischargeNote = request.DischargeNote,
            recommendations = request.Recommendations,
            followUpAppointment = request.FollowUpAppointment,
            referredDepartmentId = request.ReferredDepartmentId,
            referredDepartmentName = request.ReferredDepartmentName,
            totalStayMinutes = totalMinutes,
            status = request.DischargeType == DischargeType.Admitted ? "Admitted" : "Discharged",
            message = "Çıkış işlemi başarıyla tamamlandı."
        });
    }

    /// <summary>
    /// Triaj istatistikleri
    /// Ne: Triaj kategorilerine göre istatistikleri getiren endpoint.
    /// Neden: Raporlama için gereklidir.
    /// Kim Kullanacak: Yönetim, Raporlama.
    /// Amacı: Triaj istatistiklerinin görüntülenmesi.
    /// </summary>
    [HttpGet("statistics")]
    public IActionResult GetStatistics([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        _logger.LogInformation("Getting emergency statistics");

        return Ok(new
        {
            totalCases = 500,
            byCategory = new
            {
                red = 15,
                orange = 85,
                yellow = 200,
                green = 180,
                blue = 20
            },
            averageStayMinutes = 145,
            admissionRate = 35.5m,
            dischargeRate = 64.5m,
            ambulanceArrivalRate = 40.0m
        });
    }
}

/// <summary>
/// Acil başvuru oluşturma istek modeli
/// Ne: Acil başvuru oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden acil başvuru verisi almak için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Yeni acil başvuru oluşturma parametrelerini taşımak.
/// </summary>
public class CreateEmergencyRequest
{
    public Guid PatientId { get; set; }
    public ArrivalType ArrivalType { get; set; }
    public string? ReasonForVisit { get; set; }
}

/// <summary>
/// Triaj istek modeli
/// Ne: Triaj endpoint'i için gerekli input modeli.
/// Neden: Triaj bilgilerini almak için gereklidir.
/// Kim Kullanacak: Triaj hemşiresi.
/// Amacı: Triaj parametrelerini taşımak.
/// </summary>
public class PerformTriageRequest
{
    public int TriageScore { get; set; }
    public TriageCategory TriageCategory { get; set; }
    public string? TriageNote { get; set; }
    public Guid? TriageNurseId { get; set; }
}

/// <summary>
/// Muayene güncelleme istek modeli
/// Ne: Muayene güncelleme endpoint'i için gerekli input modeli.
/// Neden: Muayene bilgilerini almak için gereklidir.
/// Kim Kullanacak: Doktor.
/// Amacı: Muayene parametrelerini taşımak.
/// </summary>
public class UpdateExaminationRequest
{
    public string? VitalSigns { get; set; }
    public string? Complaint { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Procedures { get; set; }
    public string? MedicationsGiven { get; set; }
}

/// <summary>
/// Durum güncelleme istek modeli
/// Ne: Durum güncelleme endpoint'i için gerekli input modeli.
/// Neden: Durum bilgilerini almak için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Durum parametrelerini taşımak.
/// </summary>
public class UpdateStatusRequest
{
    public EmergencyStatus Status { get; set; }
    public string? TreatmentRoom { get; set; }
}

/// <summary>
/// Çıkış istek modeli
/// Ne: Çıkış endpoint'i için gerekli input modeli.
/// Neden: Çıkış bilgilerini almak için gereklidir.
/// Kim Kullanacak: Doktor.
/// Amacı: Çıkış parametrelerini taşımak.
/// </summary>
public class DischargeRequest
{
    public DischargeType DischargeType { get; set; }
    public string? DischargeNote { get; set; }
    public string? Recommendations { get; set; }
    public DateTime? FollowUpAppointment { get; set; }
    public Guid? ReferredDepartmentId { get; set; }
    public string? ReferredDepartmentName { get; set; }
}
