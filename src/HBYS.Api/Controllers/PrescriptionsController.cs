using HBYS.Domain.Entities.Pharmacy;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Eczane/Reçete Yönetimi Controller'ı
/// Ne: Reçete işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Reçete oluşturma, ilaç verme, sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Reçete durumu takibi, ilaç veriliş kaydı, stok kontrolü sunar.
/// Kim Kullanacak: Eczane, Doktor, Hemşire, Hasta.
/// Amacı: Eczane sürecinin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(ILogger<PrescriptionsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm reçeteleri getir
    /// Ne: Mevcut tenant'a ait tüm reçeteleri listeleyen endpoint.
    /// Neden: Reçete listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Eczane, Raporlama.
    /// Amacı: Tenant reçetelerinin görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? patientId,
        [FromQuery] PrescriptionType? type,
        [FromQuery] PrescriptionStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting prescriptions - Page: {Page}, Status: {Status}", page, status);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                prescriptionNumber = "RC-2024-0001",
                prescriptionDate = DateTime.Now.AddDays(-1),
                patientName = "Ahmet Yılmaz",
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                type = "Normal",
                status = "Dispensed",
                totalAmount = 150.00m,
                netAmount = 150.00m
            }
        });
    }

    /// <summary>
    /// ID'ye göre reçete getir
    /// Ne: Belirli bir reçetenin detaylarını getiren endpoint.
    /// Neden: Reçete düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Eczane, Doktor, Hasta.
    /// Amacı: Tekil reçete bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting prescription by ID: {PrescriptionId}", id);

        return Ok(new
        {
            id = id,
            prescriptionNumber = "RC-2024-0001",
            prescriptionDate = DateTime.Now.AddDays(-1),
            patientId = Guid.NewGuid(),
            patientName = "Ahmet Yılmaz",
            patientTckn = "12345678901",
            type = "Normal",
            source = "Outpatient",
            examinationId = Guid.NewGuid(),
            doctorId = Guid.NewGuid(),
            doctorName = "Dr. Mehmet Demir",
            departmentId = Guid.NewGuid(),
            departmentName = "Dahiliye",
            status = "Dispensed",
            totalAmount = 150.00m,
            discount = 0.00m,
            netAmount = 150.00m,
            insuranceCoverage = 105.00m,
            patientShare = 45.00m,
            dispensedDate = DateTime.Now,
            pharmacistId = Guid.NewGuid(),
            pharmacistName = "Ecz. Ali Veli",
            items = new[]
            {
                new { code = "IL001", name = "Parasetamol 500 mg", quantity = 2, unitPrice = 25.00m, totalPrice = 50.00m },
                new { code = "IL002", name = "Amoksisilin 500 mg", quantity = 1, unitPrice = 100.00m, totalPrice = 100.00m }
            }
        });
    }

    /// <summary>
    /// Hasta ID'ye göre reçeteleri getir
    /// Ne: Belirli bir hastaya ait tüm reçeteleri getiren endpoint.
    /// Neden: Hasta reçete geçmişi için gereklidir.
    /// Kim Kullanacak: Hasta profili, Eczane.
    /// Amacı: Hasta reçete geçmişinin görüntülenmesi.
    /// </summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public IActionResult GetByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting prescriptions for patient: {PatientId}", patientId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                prescriptionNumber = "RC-2024-0001",
                prescriptionDate = DateTime.Now.AddDays(-1),
                doctorName = "Dr. Mehmet Demir",
                departmentName = "Dahiliye",
                type = "Normal",
                status = "Dispensed",
                netAmount = 150.00m
            }
        });
    }

    /// <summary>
    /// Bekleyen reçeteleri getir
    /// Ne: Henüz verilmeyen reçeteleri listeleyen endpoint.
    /// Neden: Eczane iş akışı için gereklidir.
    /// Kim Kullanacak: Eczane.
    /// Amacı: Bekleyen reçetelerin görüntülenmesi.
    /// </summary>
    [HttpGet("pending")]
    public IActionResult GetPending()
    {
        _logger.LogInformation("Getting pending prescriptions");

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                prescriptionNumber = "RC-2024-0002",
                prescriptionDate = DateTime.Now,
                patientName = "Ayşe Yıldız",
                doctorName = "Dr. Ali Kaya",
                departmentName = "Kardiyoloji",
                type = "Normal",
                status = "Pending",
                netAmount = 200.00m
            }
        });
    }

    /// <summary>
    /// Yeni reçete oluştur
    /// Ne: Sisteme yeni reçete ekleyen endpoint.
    /// Neden: Reçete yazma işlemleri için gereklidir.
    /// Kim Kullanacak: Doktor, Eczane.
    /// Amacı: Sisteme yeni reçete ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreatePrescriptionRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        if (request.DoctorId == Guid.Empty)
        {
            return BadRequest(new { error = "Doktor seçimi zorunludur." });
        }

        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { error = "En az bir ilaç eklenmelidir." });
        }

        _logger.LogInformation("Creating new prescription for patient: {PatientId}, Doctor: {DoctorId}", 
            request.PatientId, request.DoctorId);

        var prescriptionId = Guid.NewGuid();
        var prescriptionNumber = $"RC-{DateTime.Now:yyyyMMddHHmmss}";

        // Toplam hesapla
        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);

        return Created($"/api/prescriptions/{prescriptionId}", new
        {
            id = prescriptionId,
            prescriptionNumber = prescriptionNumber,
            prescriptionDate = DateTime.UtcNow,
            patientId = request.PatientId,
            type = request.Type.ToString(),
            source = request.Source.ToString(),
            examinationId = request.ExaminationId,
            inpatientId = request.InpatientId,
            doctorId = request.DoctorId,
            departmentId = request.DepartmentId,
            status = "Pending",
            totalAmount = totalAmount,
            discount = 0,
            netAmount = totalAmount,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Reçete durumunu güncelle
    /// Ne: Reçete durumunu güncelleyen endpoint.
    /// Neden: Reçete hazırlama ve verme işlemleri için gereklidir.
    /// Kim Kullanacak: Eczane.
    /// Amacı: Reçete durumu güncelleme.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        _logger.LogInformation("Updating prescription status: {PrescriptionId}, Status: {Status}", id, request.Status);

        return Ok(new
        {
            id = id,
            status = request.Status.ToString(),
            message = "Durum başarıyla güncellendi."
        });
    }

    /// <summary>
    /// İlaç verilişi kaydet
    /// Ne: Reçetedeki ilaçların verildiğini kaydeden endpoint.
    /// Neden: İlaç veriliş kaydı için gereklidir.
    /// Kim Kullanacak: Eczane.
    /// Amacı: İlaç veriliş kaydetme.
    /// </summary>
    [HttpPost("{id:guid}/dispense")]
    public IActionResult Dispense(Guid id, [FromBody] DispenseRequest request)
    {
        _logger.LogInformation("Dispensing prescription: {PrescriptionId}, Pharmacist: {PharmacistId}", 
            id, request.PharmacistId);

        return Ok(new
        {
            id = id,
            status = "Dispensed",
            dispensedDate = DateTime.UtcNow,
            pharmacistId = request.PharmacistId,
            pharmacistName = "Ecz. Test",
            message = "İlaçlar başarıyla verildi."
        });
    }

    /// <summary>
    /// Reçete iptal et
    /// Ne: Reçeteyi iptal eden endpoint.
    /// Neden: Reçete iptal işlemleri için gereklidir.
    /// Kim Kullanacak: Eczane, Doktor.
    /// Amacı: Reçete iptal etme.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public IActionResult Cancel(Guid id, [FromBody] CancelPrescriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { error = "İptal sebebi zorunludur." });
        }

        _logger.LogInformation("Cancelling prescription: {PrescriptionId}, Reason: {Reason}", id, request.Reason);

        return Ok(new
        {
            id = id,
            status = "Cancelled",
            isCancelled = true,
            cancellationReason = request.Reason,
            message = "Reçete başarıyla iptal edildi."
        });
    }

    /// <summary>
    /// Eczane raporları
    /// Ne: Eczane satış raporlarını getiren endpoint.
    /// Neden: Raporlama için gereklidir.
    /// Kim Kullanacak: Yönetim, Eczane.
    /// Amacı: Eczane istatistiklerinin görüntülenmesi.
    /// </summary>
    [HttpGet("reports")]
    public IActionResult GetReports([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        _logger.LogInformation("Getting pharmacy reports");

        return Ok(new
        {
            totalPrescriptions = 250,
            totalAmount = 125000.00m,
            totalPatientShare = 37500.00m,
            totalInsuranceCoverage = 87500.00m,
            byType = new
            {
                normal = 180,
                red = 15,
                purple = 10,
                discharge = 45
            },
            topMedications = new[]
            {
                new { name = "Parasetamol 500 mg", count = 150 },
                new { name = "Amoksisilin 500 mg", count = 80 },
                new { name = "D vitamini", count = 65 }
            }
        });
    }
}

/// <summary>
/// Reçete oluşturma istek modeli
/// Ne: Reçete oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden reçete verisi almak için gereklidir.
/// Kim Kullanacak: Doktor.
/// Amacı: Yeni reçete oluşturma parametrelerini taşımak.
/// </summary>
public class CreatePrescriptionRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    public PrescriptionType Type { get; set; }
    public PrescriptionSource Source { get; set; }
    public Guid? ExaminationId { get; set; }
    public Guid? InpatientId { get; set; }
    public List<PrescriptionItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Reçete kalemi istek modeli
/// Ne: Reçete kalemlerini tanımlayan model.
/// Neden: Reçete ilaçları için gereklidir.
/// Kim Kullanacak: Doktor.
/// Amacı: Reçete kalemi parametrelerini taşımak.
/// </summary>
public class PrescriptionItemRequest
{
    public string MedicationCode { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Usage { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Durum güncelleme istek modeli
/// Ne: Durum güncelleme endpoint'i için gerekli input modeli.
/// Neden: Yeni durumu almak için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Durum parametrelerini taşımak.
/// </summary>
public class PrescriptionUpdateStatusRequest
{
    public PrescriptionStatus Status { get; set; }
}

/// <summary>
/// İlaç veriliş istek modeli
/// Ne: İlaç veriliş endpoint'i için gerekli input modeli.
/// Neden: Eczacı bilgisini almak için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Veriliş parametrelerini taşımak.
/// </summary>
public class DispenseRequest
{
    public Guid PharmacistId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Reçete iptal istek modeli
/// Ne: Reçete iptal endpoint'i için gerekli input modeli.
/// Neden: İptal sebebini almak için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: İptal parametrelerini taşımak.
/// </summary>
public class CancelPrescriptionRequest
{
    public string Reason { get; set; } = string.Empty;
}
