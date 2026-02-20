using HBYS.Domain.Entities.Billing;
using HBYS.Domain.Entities.Inpatient;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Yatan Hasta Yönetimi Controller'ı
/// Ne: Yatan hasta işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Yatış, çıkış, transfer ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Oda/yatak yönetimi, taburculuk süreci, günlük takip sunar.
/// Kim Kullanacak: Servis, Hemşire, Doktor, Eczane, Faturalama.
/// Amacı: Yatan hasta sürecinin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InpatientsController : ControllerBase
{
    private readonly ILogger<InpatientsController> _logger;

    public InpatientsController(ILogger<InpatientsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm yatan hastaları getir
    /// Ne: Mevcut tenant'a ait tüm yatan hastaları listeleyen endpoint.
    /// Neden: Yatan hasta listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Servis, Hemşire, Raporlama.
    /// Amacı: Tenant yatan hastalarının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] Guid? departmentId,
        [FromQuery] InpatientStatus? status,
        [FromQuery] DateTime? admissionDateFrom,
        [FromQuery] DateTime? admissionDateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting inpatients - Page: {Page}, Department: {DepartmentId}, Status: {Status}", 
            page, departmentId, status);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                admissionNumber = "YH-2024-0001",
                patientName = "Ahmet Yılmaz",
                departmentName = "Dahiliye",
                roomNumber = "101",
                bedNumber = "A",
                responsibleDoctorName = "Dr. Mehmet Demir",
                admissionDate = DateTime.Now.AddDays(-3),
                status = "UnderTreatment"
            }
        });
    }

    /// <summary>
    /// ID'ye göre yatan hasta getir
    /// Ne: Belirli bir yatan hastanın detaylarını getiren endpoint.
    /// Neden: Yatan hasta düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Servis, Doktor, Hemşire, Eczane.
    /// Amacı: Tekil yatan hasta bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting inpatient by ID: {InpatientId}", id);

        return Ok(new
        {
            id = id,
            admissionNumber = "YH-2024-0001",
            patientId = Guid.NewGuid(),
            patientName = "Ahmet Yılmaz",
            patientTckn = "12345678901",
            admissionDate = DateTime.Now.AddDays(-3),
            admissionTime = TimeSpan.FromHours(14),
            admissionType = "Scheduled",
            admissionReason = "Apandisit şüphesi",
            preliminaryDiagnosis = "Akut apandisit",
            departmentId = Guid.NewGuid(),
            departmentName = "Dahiliye",
            roomId = Guid.NewGuid(),
            roomNumber = "101",
            bedNumber = "A",
            responsibleDoctorId = Guid.NewGuid(),
            responsibleDoctorName = "Dr. Mehmet Demir",
            status = "UnderTreatment",
            hasAttendant = false,
            insuranceType = "SGK",
            dailyRoomRate = 500.00m,
            totalCharge = 1500.00m,
            specialNotes = "Diyabet hastası"
        });
    }

    /// <summary>
    /// Hasta ID'ye göre yatan hasta getir
    /// Ne: Belirli bir hastanın yatış bilgisini getiren endpoint.
    /// Neden: Hasta yatış geçmişi için gereklidir.
    /// Kim Kullanacak: Hasta profili, Servis.
    /// Amacı: Hasta yatış geçmişinin görüntülenmesi.
    /// </summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public IActionResult GetByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting inpatient for patient: {PatientId}", patientId);

        return Ok(new
        {
            id = Guid.NewGuid(),
            admissionNumber = "YH-2024-0001",
            departmentName = "Dahiliye",
            roomNumber = "101",
            admissionDate = DateTime.Now.AddDays(-3),
            status = "UnderTreatment"
        });
    }

    /// <summary>
    /// Aktif yatan hastaları getir
    /// Ne: Halihazırda hastanede yatan hastaları listeleyen endpoint.
    /// Neden: Servis doluluk takibi için gereklidir.
    /// Kim Kullanacak: Servis, Hemşire, Yönetim.
    /// Amacı: Aktif yatan hastaların görüntülenmesi.
    /// </summary>
    [HttpGet("active")]
    public IActionResult GetActive([FromQuery] Guid? departmentId)
    {
        _logger.LogInformation("Getting active inpatients - Department: {DepartmentId}", departmentId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                admissionNumber = "YH-2024-0001",
                patientName = "Ahmet Yılmaz",
                roomNumber = "101",
                bedNumber = "A",
                responsibleDoctorName = "Dr. Mehmet Demir",
                admissionDate = DateTime.Now.AddDays(-3),
                daysOfStay = 3,
                status = "UnderTreatment"
            }
        });
    }

    /// <summary>
    /// Yeni yatış oluştur
    /// Ne: Sisteme yeni yatan hasta ekleyen endpoint.
    /// Neden: Hasta yatış işlemleri için gereklidir.
    /// Kim Kullanacak: Servis, Acil servis.
    /// Amacı: Sisteme yeni yatış ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateInpatientRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        if (request.RoomId == Guid.Empty)
        {
            return BadRequest(new { error = "Oda seçimi zorunludur." });
        }

        _logger.LogInformation("Creating new inpatient for patient: {PatientId}, Room: {RoomId}", 
            request.PatientId, request.RoomId);

        var inpatientId = Guid.NewGuid();
        var admissionNumber = $"YH-{DateTime.Now:yyyyMMdd}-{(new Random().Next(100, 999))}";

        return Created($"/api/inpatients/{inpatientId}", new
        {
            id = inpatientId,
            admissionNumber = admissionNumber,
            patientId = request.PatientId,
            admissionDate = DateTime.UtcNow,
            admissionTime = DateTime.UtcNow.TimeOfDay,
            admissionType = request.AdmissionType.ToString(),
            admissionReason = request.AdmissionReason,
            preliminaryDiagnosis = request.PreliminaryDiagnosis,
            departmentId = request.DepartmentId,
            roomId = request.RoomId,
            responsibleDoctorId = request.ResponsibleDoctorId,
            status = "Admitted",
            hasAttendant = request.HasAttendant,
            insuranceType = request.InsuranceType.ToString(),
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Yatan hasta güncelle
    /// Ne: Mevcut yatan hasta bilgilerini güncelleyen endpoint.
    /// Neden: Hasta bilgisi değişiklikleri için gereklidir.
    /// Kim Kullanacak: Servis, Hemşire.
    /// Amacı: Yatan hasta bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateInpatientRequest request)
    {
        _logger.LogInformation("Updating inpatient: {InpatientId}", id);

        return Ok(new
        {
            id = id,
            specialNotes = request.SpecialNotes,
            hasAttendant = request.HasAttendant,
            attendantName = request.AttendantName,
            attendantPhone = request.AttendantPhone,
            emergencyContact = request.EmergencyContact,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Oda transferi yap
    /// Ne: Hastayı başka odaya transfer eden endpoint.
    /// Neden: Oda değişiklikleri için gereklidir.
    /// Kim Kullanacak: Servis, Oda yönetimi.
    /// Amacı: Oda transferi.
    /// </summary>
    [HttpPost("{id:guid}/transfer-room")]
    public IActionResult TransferRoom(Guid id, [FromBody] TransferRoomRequest request)
    {
        if (request.NewRoomId == Guid.Empty)
        {
            return BadRequest(new { error = "Hedef oda seçimi zorunludur." });
        }

        _logger.LogInformation("Transferring inpatient: {InpatientId} to room: {NewRoomId}", id, request.NewRoomId);

        return Ok(new
        {
            id = id,
            roomId = request.NewRoomId,
            roomNumber = "105",
            bedNumber = request.BedNumber,
            message = "Oda transferi başarıyla yapıldı.",
            transferredAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Taburculuk işlemi
    /// Ne: Hastayı taburcu eden endpoint.
    /// Neden: Taburculuk işlemleri için gereklidir.
    /// Kim Kullanacak: Doktor, Servis.
    /// Amacı: Taburculuk işlemi.
    /// </summary>
    [HttpPost("{id:guid}/discharge")]
    public IActionResult Discharge(Guid id, [FromBody] InpatientDischargeRequest request)
    {
        _logger.LogInformation("Discharging inpatient: {InpatientId}", id);

        var daysOfStay = (DateTime.UtcNow - DateTime.UtcNow.AddDays(-3)).Days;

        return Ok(new
        {
            id = id,
            dischargeDate = DateTime.UtcNow,
            dischargeTime = DateTime.UtcNow.TimeOfDay,
            dischargeReason = request.DischargeReason,
            finalDiagnosis = request.FinalDiagnosis,
            dischargeSummary = request.DischargeSummary,
            recommendations = request.Recommendations,
            followUpAppointmentDate = request.FollowUpAppointmentDate,
            prescriptions = request.Prescriptions,
            daysOfStay = daysOfStay,
            totalCharge = daysOfStay * 500.00m, // Örnek hesaplama
            status = "Discharged",
            message = "Taburculuk işlemi başarıyla tamamlandı."
        });
    }

    /// <summary>
    /// Hasta durumunu güncelle
    /// Ne: Yatan hastanın durumunu güncelleyen endpoint.
    /// Neden: Durum değişiklikleri (ameliyat, yoğun bakım) için gereklidir.
    /// Kim Kullanacak: Servis, Hemşire.
    /// Amacı: Durum güncelleme.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] InpatientUpdateStatusRequest request)
    {
        _logger.LogInformation("Updating inpatient status: {InpatientId}, Status: {Status}", id, request.Status);

        return Ok(new
        {
            id = id,
            status = request.Status.ToString(),
            message = "Durum başarıyla güncellendi."
        });
    }

    /// <summary>
    /// Servis doluluk oranları
    /// Ne: Bölümlere göre doluluk oranlarını getiren endpoint.
    /// Neden: Yönetim raporları için gereklidir.
    /// Kim Kullanacak: Yönetim, Raporlama.
    /// Amacı: Doluluk oranlarının görüntülenmesi.
    /// </summary>
    [HttpGet("occupancy")]
    public IActionResult GetOccupancyRates()
    {
        _logger.LogInformation("Getting occupancy rates");

        return Ok(new[]
        {
            new {
                departmentName = "Dahiliye",
                totalBeds = 30,
                occupiedBeds = 25,
                occupancyRate = 83.33m
            },
            new {
                departmentName = "Cerrahi",
                totalBeds = 25,
                occupiedBeds = 20,
                occupancyRate = 80.00m
            },
            new {
                departmentName = "Yoğun Bakım",
                totalBeds = 10,
                occupiedBeds = 8,
                occupancyRate = 80.00m
            }
        });
    }
}

/// <summary>
/// Yatan hasta oluşturma istek modeli
/// Ne: Yatan hasta oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden yatan hasta verisi almak için gereklidir.
/// Kim Kullanacak: Servis, Acil servis.
/// Amacı: Yeni yatan hasta oluşturma parametrelerini taşımak.
/// </summary>
public class CreateInpatientRequest
{
    public Guid PatientId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid RoomId { get; set; }
    public Guid ResponsibleDoctorId { get; set; }
    public AdmissionType AdmissionType { get; set; }
    public string? AdmissionReason { get; set; }
    public string? PreliminaryDiagnosis { get; set; }
    public bool HasAttendant { get; set; }
    public InsuranceType InsuranceType { get; set; }
}

/// <summary>
/// Yatan hasta güncelleme istek modeli
/// Ne: Yatan hasta güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut yatan hasta bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Yatan hasta güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateInpatientRequest
{
    public string? SpecialNotes { get; set; }
    public bool HasAttendant { get; set; }
    public string? AttendantName { get; set; }
    public string? AttendantPhone { get; set; }
    public string? EmergencyContact { get; set; }
}

/// <summary>
/// Oda transfer istek modeli
/// Ne: Oda transfer endpoint'i için gerekli input modeli.
/// Neden: Transfer bilgisini almak için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Transfer parametrelerini taşımak.
/// </summary>
public class TransferRoomRequest
{
    public Guid NewRoomId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

/// <summary>
/// Taburculuk istek modeli
/// Ne: Taburculuk endpoint'i için gerekli input modeli.
/// Neden: Taburculuk bilgilerini almak için gereklidir.
/// Kim Kullanacak: Doktor, Servis.
/// Amacı: Taburculuk parametrelerini taşımak.
/// </summary>
public class InpatientDischargeRequest
{
    public string DischargeReason { get; set; } = string.Empty;
    public string? FinalDiagnosis { get; set; }
    public string? DischargeSummary { get; set; }
    public string? Recommendations { get; set; }
    public DateTime? FollowUpAppointmentDate { get; set; }
    public string? Prescriptions { get; set; }
}

/// <summary>
/// Durum güncelleme istek modeli
/// Ne: Durum güncelleme endpoint'i için gerekli input modeli.
/// Neden: Yeni durumu almak için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Durum parametrelerini taşımak.
/// </summary>
public class InpatientUpdateStatusRequest
{
    public InpatientStatus Status { get; set; }
}
