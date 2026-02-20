using HBYS.Domain.Entities.Outpatient;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Poliklinik Muayene Yönetimi Controller'ı
/// Ne: Poliklinik muayene işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Muayene kaydı, güncelleme ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Anamnez, muayene bulguları, tanı ve tedavi planı yönetimi sunar.
/// Kim Kullanacak: Poliklinik, Doktor paneli, Eczane, Laboratuvar.
/// Amacı: Hasta muayene sürecinin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OutpatientsController : ControllerBase
{
    private readonly ILogger<OutpatientsController> _logger;

    public OutpatientsController(ILogger<OutpatientsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm muayeneleri getir
    /// Ne: Mevcut tenant'a ait tüm poliklinik muayenelerini listeleyen endpoint.
    /// Neden: Muayene listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Poliklinik, Raporlama, Doktor paneli.
    /// Amacı: Tenant muayenelerinin görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? doctorId,
        [FromQuery] Guid? departmentId,
        [FromQuery] ExaminationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting examinations - Page: {Page}, Doctor: {DoctorId}", page, doctorId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                examinationNumber = "MU-2024-0001",
                patientName = "Ahmet Yılmaz",
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                examinationDateTime = DateTime.Now,
                status = "Completed"
            }
        });
    }

    /// <summary>
    /// ID'ye göre muayene getir
    /// Ne: Belirli bir muayenenin detaylarını getiren endpoint.
    /// Neden: Muayene düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Doktor paneli, Eczane, Laboratuvar.
    /// Amacı: Tekil muayene bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting examination by ID: {ExaminationId}", id);

        return Ok(new
        {
            id = id,
            examinationNumber = "MU-2024-0001",
            patientId = Guid.NewGuid(),
            patientName = "Ahmet Yılmaz",
            patientTckn = "12345678901",
            appointmentId = Guid.NewGuid(),
            doctorId = Guid.NewGuid(),
            doctorName = "Dr. Mehmet Demir",
            departmentId = Guid.NewGuid(),
            departmentName = "Dahiliye",
            examinationDateTime = DateTime.Now,
            status = "Completed",
            complaint = "Baş ağrısı",
            medicalHistory = "Diyabet, Hipertansiyon",
            familyHistory = "Baba: Kalp hastalığı",
            physicalExamination = "Genel durum iyi",
            bloodPressure = "120/80",
            pulse = 72,
            temperature = 36.5m,
            diagnoses = "Gerilim tipi baş ağrısı (ICD: G44.2)",
            mainDiagnosis = "Gerilim tipi baş ağrısı",
            treatmentPlan = "Ağrı kesici önerildi",
            medications = "parasetamol 500 mg, günde 3 kez",
            isCompleted = true,
            completedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Hasta ID'ye göre muayeneleri getir
    /// Ne: Belirli bir hastaya ait tüm muayeneleri getiren endpoint.
    /// Neden: Hasta tıbbi geçmişi için gereklidir.
    /// Kim Kullanacak: Doktor paneli, Hasta profili, Raporlama.
    /// Amacı: Hasta muayene geçmişinin görüntülenmesi.
    /// </summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public IActionResult GetByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting examinations for patient: {PatientId}", patientId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                examinationNumber = "MU-2024-0001",
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                examinationDateTime = DateTime.Now.AddDays(-30),
                mainDiagnosis = "Gerilim tipi baş ağrısı",
                status = "Completed"
            }
        });
    }

    /// <summary>
    /// Yeni muayene oluştur
    /// Ne: Sisteme yeni poliklinik muayenesi ekleyen endpoint.
    /// Neden: Hasta muayene kaydı işlemleri için gereklidir.
    /// Kim Kullanacak: Poliklinik, Doktor paneli.
    /// Amacı: Sisteme yeni muayene ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateOutpatientRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        if (request.DoctorId == Guid.Empty)
        {
            return BadRequest(new { error = "Doktor seçimi zorunludur." });
        }

        _logger.LogInformation("Creating new examination for patient: {PatientId}, Doctor: {DoctorId}", 
            request.PatientId, request.DoctorId);

        var examinationId = Guid.NewGuid();
        var examinationNumber = $"MU-{DateTime.Now:yyyy}-{(DateTime.Now.DayOfYear):D4}";

        return Created($"/api/outpatients/{examinationId}", new
        {
            id = examinationId,
            examinationNumber = examinationNumber,
            patientId = request.PatientId,
            doctorId = request.DoctorId,
            departmentId = request.DepartmentId,
            appointmentId = request.AppointmentId,
            examinationDateTime = DateTime.UtcNow,
            status = "Started",
            isCompleted = false,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Muayene güncelle (vital bulgular)
    /// Ne: Mevcut muayenenin vital bulgularını güncelleyen endpoint.
    /// Neden: Hemşire veya doktor vital bulguları girmek için gereklidir.
    /// Kim Kullanacak: Hemşire, Doktor paneli.
    /// Amacı: Vital bulguların güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}/vitals")]
    public IActionResult UpdateVitals(Guid id, [FromBody] UpdateVitalsRequest request)
    {
        _logger.LogInformation("Updating vitals for examination: {ExaminationId}", id);

        return Ok(new
        {
            id = id,
            bloodPressure = request.BloodPressure,
            pulse = request.Pulse,
            temperature = request.Temperature,
            respiratoryRate = request.RespiratoryRate,
            oxygenSaturation = request.OxygenSaturation,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Muayene güncelle (anamnez)
    /// Ne: Mevcut muayenenin anamnez bilgilerini güncelleyen endpoint.
    /// Neden: Hasta öyküsü ve şikayet bilgisi girmek için gereklidir.
    /// Kim Kullanacak: Doktor paneli.
    /// Amacı: Anamnez bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}/anamnesis")]
    public IActionResult UpdateAnamnesis(Guid id, [FromBody] UpdateAnamnesisRequest request)
    {
        _logger.LogInformation("Updating anamnesis for examination: {ExaminationId}", id);

        return Ok(new
        {
            id = id,
            complaint = request.Complaint,
            medicalHistory = request.MedicalHistory,
            familyHistory = request.FamilyHistory,
            durationOfComplaint = request.DurationOfComplaint,
            physicalExamination = request.PhysicalExamination,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Muayene güncelle (tanı ve tedavi)
    /// Ne: Mevcut muayenenin tanı ve tedavi bilgilerini güncelleyen endpoint.
    /// Neden: Tanı ve tedavi planı girmek için gereklidir.
    /// Kim Kullanacak: Doktor paneli, Eczane.
    /// Amacı: Tanı ve tedavi bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}/diagnosis")]
    public IActionResult UpdateDiagnosis(Guid id, [FromBody] UpdateDiagnosisRequest request)
    {
        _logger.LogInformation("Updating diagnosis for examination: {ExaminationId}", id);

        return Ok(new
        {
            id = id,
            diagnoses = request.Diagnoses,
            mainDiagnosis = request.MainDiagnosis,
            treatmentPlan = request.TreatmentPlan,
            medications = request.Medications,
            labTests = request.LabTests,
            followUpDate = request.FollowUpDate,
            notes = request.Notes,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Muayene tamamla
    /// Ne: Mevcut muayeneyi tamamlayan endpoint.
    /// Neden: Muayene sürecini bitirmek için gereklidir.
    /// Kim Kullanacak: Doktor paneli.
    /// Amacı: Muayenenin tamamlanması.
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    public IActionResult Complete(Guid id)
    {
        _logger.LogInformation("Completing examination: {ExaminationId}", id);

        return Ok(new
        {
            id = id,
            status = "Completed",
            isCompleted = true,
            completedAt = DateTime.UtcNow,
            message = "Muayene başarıyla tamamlandı."
        });
    }

    /// <summary>
    /// Muayene iptal et
    /// Ne: Mevcut muayeneyi iptal eden endpoint.
    /// Neden: Muayene iptal işlemleri için gereklidir.
    /// Kim Kullanacak: Poliklinik, Doktor paneli.
    /// Amacı: Muayenenin iptal edilmesi.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public IActionResult Cancel(Guid id, [FromBody] CancelExaminationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { error = "İptal sebebi zorunludur." });
        }

        _logger.LogInformation("Cancelling examination: {ExaminationId}, Reason: {Reason}", id, request.Reason);

        return Ok(new
        {
            id = id,
            status = "Cancelled",
            message = "Muayene başarıyla iptal edildi."
        });
    }

    /// <summary>
    /// Bugünkü muayeneleri getir
    /// Ne: Belirli bir doktor veya bölüm için bugünkü muayeneleri listeleyen endpoint.
    /// Neden: Günlük iş akışı için gereklidir.
    /// Kim Kullanacak: Poliklinik, Doktor paneli.
    /// Amacı: Bugünkü muayenelerin görüntülenmesi.
    /// </summary>
    [HttpGet("today")]
    public IActionResult GetTodayExaminations([FromQuery] Guid? doctorId, [FromQuery] Guid? departmentId)
    {
        _logger.LogInformation("Getting today's examinations - Doctor: {DoctorId}, Department: {DepartmentId}", 
            doctorId, departmentId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                examinationNumber = "MU-2024-0001",
                patientName = "Ahmet Yılmaz",
                examinationDateTime = DateTime.Now,
                status = "InProgress",
                queueNumber = 1
            },
            new {
                id = Guid.NewGuid(),
                examinationNumber = "MU-2024-0002",
                patientName = "Ayşe Yıldız",
                examinationDateTime = DateTime.Now.AddMinutes(30),
                status = "Waiting",
                queueNumber = 2
            }
        });
    }
}

/// <summary>
/// Poliklinik muayene oluşturma istek modeli
/// Ne: Muayene oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden muayene verisi almak için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Yeni muayene oluşturma parametrelerini taşımak.
/// </summary>
public class CreateOutpatientRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? AppointmentId { get; set; }
}

/// <summary>
/// Vital bulgular güncelleme istek modeli
/// Ne: Vital bulgular güncelleme endpoint'i için gerekli input modeli.
/// Neden: Vital bulguları almak için gereklidir.
/// Kim Kullanacak: Hemşire, Doktor paneli.
/// Amacı: Vital bulgular parametrelerini taşımak.
/// </summary>
public class UpdateVitalsRequest
{
    public string? BloodPressure { get; set; }
    public int? Pulse { get; set; }
    public decimal? Temperature { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? OxygenSaturation { get; set; }
}

/// <summary>
/// Anamnez güncelleme istek modeli
/// Ne: Anamnez güncelleme endpoint'i için gerekli input modeli.
/// Neden: Anamnez bilgilerini almak için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Anamnez parametrelerini taşımak.
/// </summary>
public class UpdateAnamnesisRequest
{
    public string? Complaint { get; set; }
    public string? MedicalHistory { get; set; }
    public string? FamilyHistory { get; set; }
    public string? DurationOfComplaint { get; set; }
    public string? PhysicalExamination { get; set; }
}

/// <summary>
/// Tanı ve tedavi güncelleme istek modeli
/// Ne: Tanı ve tedavi güncelleme endpoint'i için gerekli input modeli.
/// Neden: Tanı ve tedavi bilgilerini almak için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Tanı ve tedavi parametrelerini taşımak.
/// </summary>
public class UpdateDiagnosisRequest
{
    public string? Diagnoses { get; set; }
    public string? MainDiagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Medications { get; set; }
    public string? LabTests { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Muayene iptal istek modeli
/// Ne: Muayene iptal endpoint'i için gerekli input modeli.
/// Neden: İptal sebebini almak için gereklidir.
/// Kim Kullanacak: Poliklinik.
/// Amacı: İptal parametrelerini taşımak.
/// </summary>
public class CancelExaminationRequest
{
    public string Reason { get; set; } = string.Empty;
}
