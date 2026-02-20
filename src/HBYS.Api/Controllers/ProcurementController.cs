using HBYS.Domain.Entities.Procurement;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Satın Alma Controller Sınıfı
/// Ne: Satın alma modülünün HTTP API endpoint'lerini yöneten controller sınıfıdır.
/// Neden: Satın alma işlemlerinin RESTful API üzerinden erişilebilir olması için gereklidir.
/// Özelliği: Purchase Order, Requisition, Supplier yönetimi.
/// Kim Kullanacak: Satın alma departmanı, Yönetim, Depo.
/// Amacı: Satın alma süreçlerinin API üzerinden yönetimi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProcurementController : ControllerBase
{
    // Satın Alma Siparişleri

    /// <summary>
    /// Tüm siparişleri getirir
    /// Ne: Sistemdeki tüm satın alma siparişlerini listeler.
/// Neden: Sipariş listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Sipariş listesi sunma.
/// </summary>
    [HttpGet("orders")]
    public IActionResult GetOrders()
    {
        // TODO: Implement with EF Core
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre sipariş getirir
    /// Ne: Belirtilen ID'ye sahip siparişi getirir.
/// Neden: Sipariş detay görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş detay sunma.
/// </summary>
    [HttpGet("orders/{id}")]
    public IActionResult GetOrderById(Guid id)
    {
        // TODO: Implement with EF Core
        return Ok(new { Id = id, OrderNumber = "PO-2026-0001", Status = OrderStatus.Draft });
    }

    /// <summary>
    /// Yeni sipariş oluşturur
    /// Ne: Sisteme yeni bir satın alma siparişi ekler.
/// Neden: Sipariş oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş oluşturma.
/// </summary>
    [HttpPost("orders")]
    public IActionResult CreateOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        // TODO: Implement with EF Core
        return Created($"/api/procurement/orders/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Sipariş günceller
    /// Ne: Mevcut bir siparişi günceller.
/// Neden: Sipariş bilgisi güncelleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş güncelleme.
/// </summary>
    [HttpPut("orders/{id}")]
    public IActionResult UpdateOrder(Guid id, [FromBody] UpdatePurchaseOrderRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Updated" });
    }

    /// <summary>
    /// Sipariş durumunu günceller
    /// Ne: Siparişin durumunu değiştirir.
/// Neden: Sipariş iş akışı için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Durum güncelleme.
/// </summary>
    [HttpPatch("orders/{id}/status")]
    public IActionResult UpdateOrderStatus(Guid id, [FromBody] ProcurementUpdateStatusRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Status Updated" });
    }

    /// <summary>
    /// Sipariş onaylar
    /// Ne: Siparişi onaylar ve onay bilgilerini kaydeder.
/// Neden: Onay işlemi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Sipariş onaylama.
/// </summary>
    [HttpPost("orders/{id}/approve")]
    public IActionResult ApproveOrder(Guid id, [FromBody] ApproveOrderRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Approved" });
    }

    /// <summary>
    /// Siparişi iptal eder
    /// Ne: Siparişi iptal eder ve iptal sebebini kaydeder.
/// Neden: İptal işlemi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş iptal etme.
/// </summary>
    [HttpPost("orders/{id}/cancel")]
    public IActionResult CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Cancelled" });
    }

    // Satın Alma Talepleri

    /// <summary>
    /// Tüm talepleri getirir
    /// Ne: Sistemdeki tüm satın alma taleplerini listeler.
/// Neden: Talep listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm departmanlar, Satın alma.
/// Amacı: Talep listesi sunma.
/// </summary>
    [HttpGet("requisitions")]
    public IActionResult GetRequisitions()
    {
        // TODO: Implement with EF Core
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre talep getirir
    /// Ne: Belirtilen ID'ye sahip talebi getirir.
/// Neden: Talep detay görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma, Departmanlar.
/// Amacı: Talep detay sunma.
/// </summary>
    [HttpGet("requisitions/{id}")]
    public IActionResult GetRequisitionById(Guid id)
    {
        // TODO: Implement with EF Core
        return Ok(new { Id = id, RequisitionNumber = "REQ-2026-0001", Status = RequisitionStatus.Draft });
    }

    /// <summary>
    /// Yeni talep oluşturur
    /// Ne: Sisteme yeni bir satın alma talebi ekler.
/// Neden: Talep oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Tüm departmanlar.
/// Amacı: Talep oluşturma.
/// </summary>
    [HttpPost("requisitions")]
    public IActionResult CreateRequisition([FromBody] CreateRequisitionRequest request)
    {
        // TODO: Implement with EF Core
        return Created($"/api/procurement/requisitions/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Talep onaylar
    /// Ne: Talebi onaylar ve onay bilgilerini kaydeder.
/// Neden: Onay işlemi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Talep onaylama.
/// </summary>
    [HttpPost("requisitions/{id}/approve")]
    public IActionResult ApproveRequisition(Guid id, [FromBody] ApproveRequisitionRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Approved" });
    }

    /// <summary>
    /// Talep reddeder
    /// Ne: Talebi reddeder ve red sebebini kaydeder.
/// Neden: Red işlemi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Talep reddetme.
/// </summary>
    [HttpPost("requisitions/{id}/reject")]
    public IActionResult RejectRequisition(Guid id, [FromBody] RejectRequisitionRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Rejected" });
    }

    // Tedarikçiler

    /// <summary>
    /// Tüm tedarikçileri getirir
    /// Ne: Sistemdeki tüm tedarikçileri listeler.
/// Neden: Tedarikçi listesi görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi listesi sunma.
/// </summary>
    [HttpGet("suppliers")]
    public IActionResult GetSuppliers()
    {
        // TODO: Implement with EF Core
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// ID'ye göre tedarikçi getirir
    /// Ne: Belirtilen ID'ye sahip tedarikçiyi getirir.
/// Neden: Tedarikçi detay görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi detay sunma.
/// </summary>
    [HttpGet("suppliers/{id}")]
    public IActionResult GetSupplierById(Guid id)
    {
        // TODO: Implement with EF Core
        return Ok(new { Id = id, Name = "Tedarikçi A.Ş.", IsActive = true });
    }

    /// <summary>
    /// Yeni tedarikçi oluşturur
    /// Ne: Sisteme yeni bir tedarikçi ekler.
/// Neden: Tedarikçi oluşturma işlemi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Tedarikçi oluşturma.
/// </summary>
    [HttpPost("suppliers")]
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        // TODO: Implement with EF Core
        return Created($"/api/procurement/suppliers/{Guid.NewGuid()}", new { Status = "Created" });
    }

    /// <summary>
    /// Tedarikçi günceller
    /// Ne: Mevcut bir tedarikçiyi günceller.
/// Neden: Tedarikçi bilgisi güncelleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi güncelleme.
/// </summary>
    [HttpPut("suppliers/{id}")]
    public IActionResult UpdateSupplier(Guid id, [FromBody] UpdateSupplierRequest request)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Updated" });
    }

    /// <summary>
    /// Tedarikçi durumunu değiştirir
    /// Ne: Tedarikçinin aktif/pasif durumunu değiştirir.
/// Neden: Durum yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Tedarikçi durum değiştirme.
/// </summary>
    [HttpPatch("suppliers/{id}/status")]
    public IActionResult ToggleSupplierStatus(Guid id)
    {
        // TODO: Implement with EF Core
        return Ok(new { Status = "Status Toggled" });
    }
}

// Request DTOs

/// <summary>
/// Yeni sipariş oluşturma isteği
/// Ne: Sipariş oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden sipariş oluşturma için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş verisi taşıma.
/// </summary>
public class CreatePurchaseOrderRequest
{
    public Guid SupplierId { get; set; }
    public OrderType Type { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryAddress { get; set; }
    public PaymentType PaymentType { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Sipariş kalemi oluşturma isteği
/// Ne: Sipariş kalemi oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden kalem ekleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kalem verisi taşıma.
/// </summary>
public class CreatePurchaseOrderItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "ADET";
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal VatRate { get; set; }
}

/// <summary>
/// Sipariş güncelleme isteği
/// Ne: Sipariş güncellemek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden sipariş güncelleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş verisi güncelleme.
/// </summary>
public class UpdatePurchaseOrderRequest
{
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryAddress { get; set; }
    public PaymentType? PaymentType { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Sipariş durum güncelleme isteği
/// Ne: Sipariş durumu güncellemek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden durum güncelleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Durum verisi taşıma.
/// </summary>
public class ProcurementUpdateStatusRequest
{
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Sipariş onay isteği
/// Ne: Sipariş onaylamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden onaylama için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Onay verisi taşıma.
/// </summary>
public class ApproveOrderRequest
{
    public string? Notes { get; set; }
}

/// <summary>
/// Sipariş iptal isteği
/// Ne: Sipariş iptal etmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden iptal etme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İptal verisi taşıma.
/// </summary>
public class CancelOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Talep oluşturma isteği
/// Ne: Talep oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden talep oluşturma için gereklidir.
/// Kim Kullanacak: Tüm departmanlar.
/// Amacı: Talep verisi taşıma.
/// </summary>
public class CreateRequisitionRequest
{
    public Guid DepartmentId { get; set; }
    public RequisitionType Type { get; set; }
    public Priority Priority { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateRequisitionItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Talep kalemi oluşturma isteği
/// Ne: Talep kalemi oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden kalem ekleme için gereklidir.
/// Kim Kullanacak: Tüm departmanlar.
/// Amacı: Kalem verisi taşıma.
/// </summary>
public class CreateRequisitionItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "ADET";
    public decimal? EstimatedUnitPrice { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Talep onay isteği
/// Ne: Talep onaylamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden onaylama için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Onay verisi taşıma.
/// </summary>
public class ApproveRequisitionRequest
{
    public List<ApproveRequisitionItemRequest> ItemApprovals { get; set; } = new();
    public string? Notes { get; set; }
}

/// <summary>
/// Talep kalemi onay isteği
/// Ne: Talep kalemi onaylamak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden kalem onaylama için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kalem onay verisi taşıma.
/// </summary>
public class ApproveRequisitionItemRequest
{
    public Guid ItemId { get; set; }
    public decimal ApprovedQuantity { get; set; }
}

/// <summary>
/// Talep red isteği
/// Ne: Talep reddetmek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden reddetme için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Red verisi taşıma.
/// </summary>
public class RejectRequisitionRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Tedarikçi oluşturma isteği
/// Ne: Tedarikçi oluşturmak için gerekli veri transfer nesnesi.
/// Neden: API üzerinden tedarikçi oluşturma için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Tedarikçi verisi taşıma.
/// </summary>
public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? TaxOffice { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Categories { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Tedarikçi güncelleme isteği
/// Ne: Tedarikçi güncellemek için gerekli veri transfer nesnesi.
/// Neden: API üzerinden tedarikçi güncelleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi verisi güncelleme.
/// </summary>
public class UpdateSupplierRequest
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Categories { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public string? Notes { get; set; }
}
