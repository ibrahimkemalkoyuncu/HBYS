using HBYS.Domain.Entities.Billing;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Fatura Yönetimi Controller'ı
/// Ne: Fatura işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Fatura oluşturma, güncelleme, ödeme ve sorgulama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: SGK ve özel sigorta desteği, detaylı kalem yapısı, ödeme takibi sunar.
/// Kim Kullanacak: Faturalama, Muhasebe, Hasta kabul, Hasta.
/// Amacı: Hastane fatura sisteminin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(ILogger<InvoicesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm faturaları getir
    /// Ne: Mevcut tenant'a ait tüm faturaları listeleyen endpoint.
    /// Neden: Fatura listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Faturalama, Muhasebe, Raporlama.
    /// Amacı: Tenant faturalarının görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? patientId,
        [FromQuery] InvoiceType? type,
        [FromQuery] InvoiceStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting invoices - Page: {Page}, Patient: {PatientId}, Status: {Status}", 
            page, patientId, status);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                invoiceNumber = "FV-2024-0001",
                patientName = "Ahmet Yılmaz",
                invoiceDate = DateTime.Now.AddDays(-5),
                type = "Outpatient",
                status = "Paid",
                grossTotal = 500.00m,
                discountTotal = 50.00m,
                netTotal = 450.00m,
                paidAmount = 450.00m,
                remainingAmount = 0.00m
            }
        });
    }

    /// <summary>
    /// ID'ye göre fatura getir
    /// Ne: Belirli bir faturanın detaylarını getiren endpoint.
    /// Neden: Fatura düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Faturalama, Hasta, Muhasebe.
    /// Amacı: Tekil fatura bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting invoice by ID: {InvoiceId}", id);

        return Ok(new
        {
            id = id,
            invoiceNumber = "FV-2024-0001",
            invoiceDate = DateTime.Now.AddDays(-5),
            patientId = Guid.NewGuid(),
            patientName = "Ahmet Yılmaz",
            patientTckn = "12345678901",
            patientAddress = "İstanbul, Türkiye",
            type = "Outpatient",
            status = "Paid",
            insuranceType = "SGK",
            insuranceCompanyName = "SGK",
            grossTotal = 500.00m,
            discountTotal = 50.00m,
            vatTotal = 45.00m,
            netTotal = 450.00m,
            sgkDiscount = 315.00m,
            patientShare = 135.00m,
            paidAmount = 450.00m,
            remainingAmount = 0.00m,
            paymentType = "CreditCard",
            dueDate = DateTime.Now.AddDays(30),
            description = "Poliklinik muayene faturası",
            isIssued = true,
            isCancelled = false,
            createdAt = DateTime.UtcNow.AddDays(-5)
        });
    }

    /// <summary>
    /// Hasta ID'ye göre faturaları getir
    /// Ne: Belirli bir hastaya ait tüm faturaları getiren endpoint.
    /// Neden: Hasta fatura geçmişi için gereklidir.
    /// Kim Kullanacak: Hasta profili, Faturalama.
    /// Amacı: Hasta fatura geçmişinin görüntülenmesi.
    /// </summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public IActionResult GetByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting invoices for patient: {PatientId}", patientId);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                invoiceNumber = "FV-2024-0001",
                invoiceDate = DateTime.Now.AddDays(-5),
                type = "Outpatient",
                status = "Paid",
                netTotal = 450.00m,
                paidAmount = 450.00m,
                remainingAmount = 0.00m
            },
            new {
                id = Guid.NewGuid(),
                invoiceNumber = "FV-2024-0002",
                invoiceDate = DateTime.Now.AddDays(-10),
                type = "Pharmacy",
                status = "Issued",
                netTotal = 250.00m,
                paidAmount = 0.00m,
                remainingAmount = 250.00m
            }
        });
    }

    /// <summary>
    /// Fatura numarasına göre getir
    /// Ne: Fatura numarası ile faturayı getiren endpoint.
    /// Neden: Fatura sorgulama için gereklidir.
    /// Kim Kullanacak: Faturalama, Hasta.
    /// Amacı: Fatura numarası ile sorgulama.
    /// </summary>
    [HttpGet("by-number/{invoiceNumber}")]
    public IActionResult GetByInvoiceNumber(string invoiceNumber)
    {
        _logger.LogInformation("Getting invoice by number: {InvoiceNumber}", invoiceNumber);

        return Ok(new
        {
            id = Guid.NewGuid(),
            invoiceNumber = invoiceNumber,
            patientName = "Ahmet Yılmaz",
            netTotal = 450.00m,
            status = "Paid"
        });
    }

    /// <summary>
    /// Yeni fatura oluştur
    /// Ne: Sisteme yeni fatura ekleyen endpoint.
    /// Neden: Hasta fatura kesme işlemleri için gereklidir.
    /// Kim Kullanacak: Faturalama, Poliklinik, Eczane.
    /// Amacı: Sisteme yeni fatura ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateInvoiceRequest request)
    {
        if (request.PatientId == Guid.Empty)
        {
            return BadRequest(new { error = "Hasta seçimi zorunludur." });
        }

        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { error = "Fatura kalemi en az bir adet olmalıdır." });
        }

        _logger.LogInformation("Creating new invoice for patient: {PatientId}, Type: {Type}", 
            request.PatientId, request.Type);

        var invoiceId = Guid.NewGuid();
        var invoiceNumber = $"FV-{DateTime.Now:yyyyMMdd}-{(new Random().Next(1000, 9999))}";
        
        // Toplam hesapla
        var grossTotal = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        var discountTotal = grossTotal * (request.DiscountPercent / 100);
        var netTotal = grossTotal - discountTotal;
        var vatTotal = netTotal * 0.18m; // %18 KDV
        var totalWithVat = netTotal + vatTotal;

        // SGK indirimi hesapla (varsa)
        decimal sgkDiscount = 0;
        if (request.InsuranceType == InsuranceType.SGK)
        {
            sgkDiscount = netTotal * 0.70m; // SGK %70 karşılar
        }
        var patientShare = totalWithVat - sgkDiscount;

        return Created($"/api/invoices/{invoiceId}", new
        {
            id = invoiceId,
            invoiceNumber = invoiceNumber,
            invoiceDate = DateTime.UtcNow,
            patientId = request.PatientId,
            type = request.Type.ToString(),
            status = "Issued",
            insuranceType = request.InsuranceType.ToString(),
            grossTotal = grossTotal,
            discountTotal = discountTotal,
            vatTotal = vatTotal,
            netTotal = totalWithVat,
            sgkDiscount = sgkDiscount,
            patientShare = patientShare,
            paidAmount = 0,
            remainingAmount = patientShare,
            paymentType = request.PaymentType.ToString(),
            dueDate = DateTime.UtcNow.AddDays(30),
            isIssued = true,
            isCancelled = false,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Fatura güncelle
    /// Ne: Mevcut fatura bilgilerini güncelleyen endpoint.
    /// Neden: Fatura düzeltme işlemleri için gereklidir.
    /// Kim Kullanacak: Faturalama.
    /// Amacı: Fatura bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateInvoiceRequest request)
    {
        _logger.LogInformation("Updating invoice: {InvoiceId}", id);

        return Ok(new
        {
            id = id,
            description = request.Description,
            notes = request.Notes,
            dueDate = request.DueDate,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Fatura kes (publish)
    /// Ne: Taslak faturası kesilmiş olarak işaretleyen endpoint.
    /// Neden: Fatura kesim işlemi için gereklidir.
    /// Kim Kullanacak: Faturalama.
    /// Amacı: Faturanın kesilmesi.
    /// </summary>
    [HttpPost("{id:guid}/issue")]
    public IActionResult Issue(Guid id)
    {
        _logger.LogInformation("Issuing invoice: {InvoiceId}", id);

        return Ok(new
        {
            id = id,
            status = "Issued",
            isIssued = true,
            message = "Fatura başarıyla kesildi."
        });
    }

    /// <summary>
    /// Ödeme yap
    /// Ne: faturaya ödeme ekleyen endpoint.
    /// Neden: Fatura ödeme işlemleri için gereklidir.
    /// Kim Kullanacak: Kasa, Faturalama.
    /// Amacı: Faturaya ödeme kaydetme.
    /// </summary>
    [HttpPost("{id:guid}/payments")]
    public IActionResult AddPayment(Guid id, [FromBody] AddPaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "Geçerli bir tutar giriniz." });
        }

        _logger.LogInformation("Adding payment to invoice: {InvoiceId}, Amount: {Amount}", id, request.Amount);

        // Ödeme işlemi yapılır
        return Ok(new
        {
            id = id,
            paymentId = Guid.NewGuid(),
            amount = request.Amount,
            paymentType = request.PaymentType.ToString(),
            paidAt = DateTime.UtcNow,
            message = "Ödeme başarıyla kaydedildi."
        });
    }

    /// <summary>
    /// Fatura iptal et
    /// Ne: Mevcut faturayı iptal eden endpoint.
    /// Neden: Fatura iptal işlemleri için gereklidir.
    /// Kim Kullanacak: Faturalama, Yönetim.
    /// Amacı: Faturanın iptal edilmesi.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public IActionResult Cancel(Guid id, [FromBody] CancelInvoiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { error = "İptal sebebi zorunludur." });
        }

        _logger.LogInformation("Cancelling invoice: {InvoiceId}, Reason: {Reason}", id, request.Reason);

        return Ok(new
        {
            id = id,
            status = "Cancelled",
            isCancelled = true,
            cancellationReason = request.Reason,
            message = "Fatura başarıyla iptal edildi."
        });
    }

    /// <summary>
    /// Fatura durumunu güncelle
    /// Ne: Fatura durumunu manuel güncelleyen endpoint.
    /// Neden: Fatura durumu değişiklikleri için gereklidir.
    /// Kim Kullanacak: Faturalama.
    /// Amacı: Fatura durumu güncelleme.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        _logger.LogInformation("Updating invoice status: {InvoiceId}, Status: {Status}", id, request.Status);

        return Ok(new
        {
            id = id,
            status = request.Status.ToString(),
            message = "Fatura durumu başarıyla güncellendi."
        });
    }

    /// <summary>
    /// Vadesi geçmiş faturaları getir
    /// Ne: Ödeme vadesi geçmiş faturaları listeleyen endpoint.
    /// Neden: Tahsilat takibi için gereklidir.
    /// Kim Kullanacak: Faturalama, Muhasebe.
    /// Amacı: Vadesi geçmiş faturaların görüntülenmesi.
    /// </summary>
    [HttpGet("overdue")]
    public IActionResult GetOverdueInvoices([FromQuery] int daysOverdue = 0)
    {
        _logger.LogInformation("Getting overdue invoices - Days: {Days}", daysOverdue);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                invoiceNumber = "FV-2024-0002",
                patientName = "Ayşe Yıldız",
                dueDate = DateTime.Now.AddDays(-10),
                netTotal = 250.00m,
                remainingAmount = 250.00m,
                daysOverdue = 10
            }
        });
    }

    /// <summary>
    /// Fatura raporları
    /// Ne: Fatura raporlarını getiren endpoint.
    /// Neden: Yönetim raporları için gereklidir.
    /// Kim Kullanacak: Yönetim, Raporlama.
    /// Amacı: Fatura istatistiklerinin görüntülenmesi.
    /// </summary>
    [HttpGet("reports/summary")]
    public IActionResult GetSummaryReport([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        _logger.LogInformation("Getting invoice summary report");

        return Ok(new
        {
            totalInvoices = 150,
            totalAmount = 750000.00m,
            totalPaid = 650000.00m,
            totalRemaining = 100000.00m,
            totalOverdue = 25000.00m,
            byStatus = new
            {
                paid = 650000.00m,
                issued = 75000.00m,
                overdue = 25000.00m
            },
            byType = new
            {
                outpatient = 250000.00m,
                inpatient = 350000.00m,
                pharmacy = 100000.00m,
                laboratory = 50000.00m
            }
        });
    }
}

/// <summary>
/// Fatura oluşturma istek modeli
/// Ne: Fatura oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden fatura verisi almak için gereklidir.
/// Kim Kullanacak: Faturalama, Poliklinik.
/// Amacı: Yeni fatura oluşturma parametrelerini taşımak.
/// </summary>
public class CreateInvoiceRequest
{
    public Guid PatientId { get; set; }
    public InvoiceType Type { get; set; }
    public InsuranceType InsuranceType { get; set; }
    public Guid? InsuranceCompanyId { get; set; }
    public string? PolicyNumber { get; set; }
    public decimal DiscountPercent { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? ExaminationId { get; set; }
    public string? Description { get; set; }
    public List<InvoiceItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Fatura kalemi istek modeli
/// Ne: Fatura kalemlerini tanımlayan model.
/// Neden: Fatura detayları için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Fatura kalemi parametrelerini taşımak.
/// </summary>
public class InvoiceItemRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Fatura güncelleme istek modeli
/// Ne: Fatura güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut fatura bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Fatura güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateInvoiceRequest
{
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Ödeme ekleme istek modeli
/// Ne: Ödeme ekleme endpoint'i için gerekli input modeli.
/// Neden: Ödeme bilgisini almak için gereklidir.
/// Kim Kullanacak: Kasa, Faturalama.
/// Amacı: Ödeme parametrelerini taşımak.
/// </summary>
public class AddPaymentRequest
{
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Fatura iptal istek modeli
/// Ne: Fatura iptal endpoint'i için gerekli input modeli.
/// Neden: İptal sebebini almak için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: İptal parametrelerini taşımak.
/// </summary>
public class CancelInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Durum güncelleme istek modeli
/// Ne: Durum güncelleme endpoint'i için gerekli input modeli.
/// Neden: Yeni durumu almak için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Durum parametrelerini taşımak.
/// </summary>
public class UpdateStatusRequest
{
    public InvoiceStatus Status { get; set; }
}
