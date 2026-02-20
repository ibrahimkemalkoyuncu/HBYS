using HBYS.Domain.Entities.Appointment;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Randevu Yönetimi Controller'ı
/// Ne: Randevu işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Randevu oluşturma, güncelleme, iptal ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Doktor bazlı, bölüm bazlı ve hasta bazlı randevu yönetimi sunar.
/// Kim Kullanacak: Hasta kabul, Poliklinik, Randevu yönetimi paneli, Mobil uygulamalar.
/// Amacı: Hastane randevu sisteminin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(ILogger<AppointmentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm randevuları getir
    /// Ne: Mevcut tenant'a ait tüm randevuları listeleyen endpoint.
    /// Neden: Randevu listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Randevu yönetimi, Raporlama, Poliklinik.
    /// Amacı: Tenant randevularının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? doctorId,
        [FromQuery] Guid? departmentId,
        [FromQuery] AppointmentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting appointments - Page: {Page}, Doctor: {DoctorId}, Status: {Status}", 
            page, doctorId, status);

        return Ok(new[]
        {
            new { 
                id = Guid.NewGuid(), 
                appointmentNumber = "RV-2024-0001",
                patientName = "Ahmet Yılmaz",
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                appointmentDateTime = DateTime.Now.AddDays(1).AddHours(9),
                status = "Confirmed",
                type = "FirstExam"
            }
        });
    }

    /// <summary>
    /// ID'ye göre randevu getir
    /// Ne: Belirli bir randevunun detaylarını getiren endpoint.
    /// Neden: Randevu düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Randevu detay, Poliklinik, Hasta bilgilendirme.
    /// Amacı: Tekil randevu bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting appointment by ID: {AppointmentId}", id);

        return Ok(new
        {
            id = id,
            appointmentNumber = "RV-2024-0001",
            patientId = Guid.NewGuid(),
            patientName = "Ahmet Yılmaz",
            patientTckn = "12345678901",
            doctorId = Guid.NewGuid(),
            doctorName = "Dr. Mehmet Demir",
            departmentId = Guid.NewGuid(),
            departmentName = "Dahiliye",
            appointmentDateTime = DateTime.Now.AddDays(1).AddHours(9),
            endDateTime = DateTime.Now.AddDays(1).AddHours(9).AddMinutes(30),
            status = "Confirmed",
            type = "FirstExam",
            complaint = "Baş ağrısı",
            notes = "Hasta önceden randevu almıştı",
            isConfirmed = true,
            didCome = false,
            createdAt = DateTime.UtcNow.AddDays(-2)
        });
    }

    /// <summary>
    /// Hasta ID'ye göre randevuları getir
    /// Ne: Belirli bir hastaya ait tüm randevuları getiren endpoint.
    /// Neden: Hasta randevu geçmişi için gereklidir.
    /// Kim Kullanacak: Hasta profili, Hasta bilgilendirme, Raporlama.
    /// Amacı: Hasta randevu geçmişinin görüntülenmesi.
    /// </summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public IActionResult GetByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting appointments for patient: {PatientId}", patientId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                appointmentNumber = "RV-2024-0001",
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                appointmentDateTime = DateTime.Now.AddDays(1).AddHours(9),
                status = "Confirmed",
                type = "FirstExam"
            }
        });
    }

    /// <summary>
    /// Doktor ID'ye göre randevuları getir
    /// Ne: Belirli bir doktora ait tüm randevuları getiren endpoint.
    /// Neden: Doktor randevu çizelgesi için gereklidir.
    /// Kim Kullanacak: Doktor paneli, Poliklinik, Randevu yönetimi.
    /// Amacı: Doktor randevu takviminin görüntülenmesi.
    /// </summary>
    [HttpGet("by-doctor/{doctorId:guid}")]
    public IActionResult GetByDoctorId(Guid doctorId, [FromQuery] DateTime? date)
    {
        _logger.LogInformation("Getting appointments for doctor: {DoctorId}, Date: {Date}", doctorId, date);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                appointmentNumber = "RV-2024-0001",
                patientName = "Ahmet Yılmaz",
                appointmentDateTime = date ?? DateTime.Now,
                status = "Confirmed",
                queueNumber = 1
            }
        });
    }

    /// <summary>
    /// Yeni randevu oluştur
    /// Ne: Sisteme yeni randevu ekleyen endpoint.
    /// Neden: Hasta randevu oluşturma işlemleri için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Mobil uygulama, Web sitesi.
    /// Amacı: Sisteme yeni randevu ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateAppointmentRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        if (request.DoctorId == Guid.Empty)
        {
            return BadRequest(new { error = "Doktor seçimi zorunludur." });
        }

        if (request.AppointmentDateTime == default)
        {
            return BadRequest(new { error = "Randevu tarihi ve saati zorunludur." });
        }

        if (request.AppointmentDateTime < DateTime.Now)
        {
            return BadRequest(new { error = "Randevu tarihi geçmiş bir zaman olamaz." });
        }

        _logger.LogInformation("Creating new appointment for patient: {PatientId}, Doctor: {DoctorId}", 
            request.PatientId, request.DoctorId);

        var appointmentId = Guid.NewGuid();
        var appointmentNumber = $"RV-{DateTime.Now:yyyy}-{(DateTime.Now.DayOfYear):D4}";

        return Created($"/api/appointments/{appointmentId}", new
        {
            id = appointmentId,
            appointmentNumber = appointmentNumber,
            patientId = request.PatientId,
            doctorId = request.DoctorId,
            departmentId = request.DepartmentId,
            appointmentDateTime = request.AppointmentDateTime,
            endDateTime = request.AppointmentDateTime.AddMinutes(30),
            status = "Pending",
            type = request.Type.ToString(),
            complaint = request.Complaint,
            notes = request.Notes,
            isConfirmed = false,
            isCancelled = false,
            didCome = false,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Randevu güncelle
    /// Ne: Mevcut randevu bilgilerini güncelleyen endpoint.
    /// Neden: Randevu değişiklikleri için gereklidir.
    /// Kim Kullanacak: Randevu yönetimi, Poliklinik.
    /// Amacı: Randevu bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        _logger.LogInformation("Updating appointment: {AppointmentId}", id);

        return Ok(new
        {
            id = id,
            appointmentDateTime = request.AppointmentDateTime,
            endDateTime = request.AppointmentDateTime.AddMinutes(30),
            type = request.Type.ToString(),
            complaint = request.Complaint,
            notes = request.Notes,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Randevu onayla
    /// Ne: Bekleyen randevuyu onaylayan endpoint.
    /// Neden: Randevu kesinleştirme işlemi için gereklidir.
    /// Kim Kullanacak: Randevu yönetimi, Hasta bilgilendirme.
    /// Amacı: Randevunun onaylanması.
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public IActionResult Confirm(Guid id)
    {
        _logger.LogInformation("Confirming appointment: {AppointmentId}", id);

        return Ok(new
        {
            id = id,
            status = "Confirmed",
            isConfirmed = true,
            confirmedAt = DateTime.UtcNow,
            message = "Randevu başarıyla onaylandı."
        });
    }

    /// <summary>
    /// Randevu iptal et
    /// Ne: Mevcut randevuyu iptal eden endpoint.
    /// Neden: Randevu iptal işlemleri için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Hasta, Randevu yönetimi.
    /// Amacı: Randevunun iptal edilmesi.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public IActionResult Cancel(Guid id, [FromBody] CancelAppointmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { error = "İptal sebebi zorunludur." });
        }

        _logger.LogInformation("Cancelling appointment: {AppointmentId}, Reason: {Reason}", id, request.Reason);

        return Ok(new
        {
            id = id,
            status = "Cancelled",
            isCancelled = true,
            cancellationReason = request.Reason,
            cancelledAt = DateTime.UtcNow,
            message = "Randevu başarıyla iptal edildi."
        });
    }

    /// <summary>
    /// Randevu durumu güncelle (geldi/gelmedi)
    /// Ne: Randevunun gerçekleşme durumunu güncelleyen endpoint.
    /// Neden: Hasta geliş takibi için gereklidir.
    /// Kim Kullanacak: Poliklinik, Hasta kabul.
    /// Amacı: Gelme durumunun işaretlenmesi.
    /// </summary>
    [HttpPost("{id:guid}/arrival")]
    public IActionResult MarkArrival(Guid id, [FromBody] MarkArrivalRequest request)
    {
        _logger.LogInformation("Marking arrival for appointment: {AppointmentId}, DidCome: {DidCome}", id, request.DidCome);

        return Ok(new
        {
            id = id,
            didCome = request.DidCome,
            arrivalTime = request.DidCome ? (DateTime?)DateTime.UtcNow : null,
            status = request.DidCome ? "Waiting" : "NoShow",
            message = request.DidCome ? "Hastanın gelişi işaretlendi." : "Hasta gelmedi olarak işaretlendi."
        });
    }

    /// <summary>
    /// Uygun randevu saatlerini getir
    /// Ne: Belirli bir doktor ve tarih için müsait saatleri listeleyen endpoint.
    /// Neden: Hasta randevu oluştururken uygun saatleri görmek için gereklidir.
    /// Kim Kullanacak: Hasta kabul, Mobil uygulama, Web sitesi.
    /// Amacı: Uygun randevu zamanlarının görüntülenmesi.
    /// </summary>
    [HttpGet("available-slots")]
    public IActionResult GetAvailableSlots(
        [FromQuery] Guid doctorId,
        [FromQuery] DateTime date)
    {
        _logger.LogInformation("Getting available slots for doctor: {DoctorId}, Date: {Date}", doctorId, date);

        // Örnek uygun saatler
        var slots = new[]
        {
            new { time = date.AddHours(9), available = true },
            new { time = date.AddHours(9).AddMinutes(30), available = true },
            new { time = date.AddHours(10), available = false },
            new { time = date.AddHours(10).AddMinutes(30), available = true },
            new { time = date.AddHours(11), available = true },
            new { time = date.AddHours(14), available = true },
            new { time = date.AddHours(14).AddMinutes(30), available = true },
            new { time = date.AddHours(15), available = false },
            new { time = date.AddHours(15).AddMinutes(30), available = true }
        };

        return Ok(slots);
    }
}

/// <summary>
/// Randevu oluşturma istek modeli
/// Ne: Randevu oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden randevu verisi almak için gereklidir.
/// Kim Kullanacak: Hasta kabul, Mobil uygulama.
/// Amacı: Yeni randevu oluşturma parametrelerini taşımak.
/// </summary>
public class CreateAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentType Type { get; set; }
    public string? Complaint { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Randevu güncelleme istek modeli
/// Ne: Randevu güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut randevu bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Randevu yönetimi.
/// Amacı: Randevu güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateAppointmentRequest
{
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentType Type { get; set; }
    public string? Complaint { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Randevu iptal istek modeli
/// Ne: Randevu iptal endpoint'i için gerekli input modeli.
/// Neden: İptal sebebini almak için gereklidir.
/// Kim Kullanacak: Randevu yönetimi.
/// Amacı: İptal parametrelerini taşımak.
/// </summary>
public class CancelAppointmentRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Geliş işaretleme istek modeli
/// Ne: Randevu geliş işaretleme endpoint'i için gerekli input modeli.
/// Neden: Gelme durumunu almak için gereklidir.
/// Kim Kullanacak: Poliklinik, Hasta kabul.
/// Amacı: Geliş parametrelerini taşımak.
/// </summary>
public class MarkArrivalRequest
{
    public bool DidCome { get; set; }
}
