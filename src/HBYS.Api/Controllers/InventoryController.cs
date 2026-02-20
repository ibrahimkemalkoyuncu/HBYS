using HBYS.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Stok/Envanter Yönetimi Controller'ı
/// Ne: Stok işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Stok giriş/çıkış, sorgulama ve raporlama işlemleri için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Kritik stok uyarısı, minimum stok takibi, lot/batch yönetimi sunar.
/// Kim Kullanacak: Deposu, Eczane, Satın alma, Yönetim.
/// Amacı: Stok sürecinin API üzerinden yönetilmesi.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(ILogger<InventoryController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tüm stok kalemlerini getir
    /// Ne: Mevcut tenant'a ait tüm stok kalemlerini listeleyen endpoint.
    /// Neden: Stok listeleme ekranı için gereklidir.
    /// Kim Kullanacak: Deposu, Raporlama.
    /// Amacı: Tenant stok kalemlerinin görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] InventoryType? type,
        [FromQuery] bool? isActive,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting inventory items - Page: {Page}, Type: {Type}", page, type);

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                code = "STK-001",
                name = "Parasetamol 500 mg",
                type = "Medication",
                unit = "Kutu",
                currentQuantity = 150,
                minimumQuantity = 50,
                criticalQuantity = 20,
                unitPrice = 25.00m,
                barcode = "8691234567890"
            }
        });
    }

    /// <summary>
    /// ID'ye göre stok kalemi getir
    /// Ne: Belirli bir stok kaleminin detaylarını getiren endpoint.
    /// Neden: Stok düzenleme veya detay görüntüleme için gereklidir.
    /// Kim Kullanacak: Deposu, Eczane.
    /// Amacı: Tekil stok kalemi bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting inventory item by ID: {InventoryId}", id);

        return Ok(new
        {
            id = id,
            code = "STK-001",
            name = "Parasetamol 500 mg",
            type = "Medication",
            barcode = "8691234567890",
            unit = "Kutu",
            categoryId = Guid.NewGuid(),
            categoryName = "Ağrı Kesiciler",
            currentQuantity = 150,
            minimumQuantity = 50,
            maximumQuantity = 500,
            criticalQuantity = 20,
            unitPrice = 25.00m,
            vatRate = 18,
            trackExpiryDate = true,
            isActive = true,
            warehouseId = Guid.NewGuid(),
            warehouseName = "Ana Depo",
            shelfLocation = "Raf A-1",
            supplierId = Guid.NewGuid(),
            supplierName = "İlaç A.Ş.",
            description = "Ağrı kesici"
        });
    }

    /// <summary>
    /// Barkod ile stok kalemi getir
    /// Ne: Barkod numarası ile stok kalemi getiren endpoint.
    /// Neden: Barkod okuma işlemleri için gereklidir.
    /// Kim Kullanacak: Deposu, Eczane.
    /// Amacı: Barkod ile stok sorgulama.
    /// </summary>
    [HttpGet("by-barcode/{barcode}")]
    public IActionResult GetByBarcode(string barcode)
    {
        _logger.LogInformation("Getting inventory item by barcode: {Barcode}", barcode);

        return Ok(new
        {
            id = Guid.NewGuid(),
            code = "STK-001",
            name = "Parasetamol 500 mg",
            currentQuantity = 150,
            unitPrice = 25.00m
        });
    }

    /// <summary>
    /// Kritik stok kalemlerini getir
    /// Ne: Minimum stok seviyesinin altındaki kalemleri listeleyen endpoint.
    /// Neden: Kritik stok uyarısı için gereklidir.
    /// Kim Kullanacak: Deposu, Satın alma, Yönetim.
    /// Amacı: Kritik stokların görüntülenmesi.
    /// </summary>
    [HttpGet("critical")]
    public IActionResult GetCriticalItems()
    {
        _logger.LogInformation("Getting critical inventory items");

        return Ok(new[]
        {
            new {
                id = Guid.NewGuid(),
                code = "STK-002",
                name = "Eldiven (L)",
                currentQuantity = 15,
                minimumQuantity = 50,
                criticalQuantity = 20,
                unit = "Kutu",
                unitPrice = 45.00m
            }
        });
    }

    /// <summary>
    /// Yeni stok kalemi oluştur
    /// Ne: Sisteme yeni stok kalemi ekleyen endpoint.
    /// Neden: Stok kalemi ekleme işlemleri için gereklidir.
    /// Kim Kullanacak: Deposu.
    /// Amacı: Sisteme yeni stok kalemi ekleme.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateInventoryItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { error = "Stok kodu zorunludur." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Stok adı zorunludur." });
        }

        _logger.LogInformation("Creating new inventory item: {Code} - {Name}", request.Code, request.Name);

        var itemId = Guid.NewGuid();

        return Created($"/api/inventory/{itemId}", new
        {
            id = itemId,
            code = request.Code,
            name = request.Name,
            type = request.Type.ToString(),
            barcode = request.Barcode,
            unit = request.Unit,
            categoryId = request.CategoryId,
            minimumQuantity = request.MinimumQuantity,
            maximumQuantity = request.MaximumQuantity,
            criticalQuantity = request.CriticalQuantity,
            unitPrice = request.UnitPrice,
            vatRate = request.VatRate,
            trackExpiryDate = request.TrackExpiryDate,
            isActive = true,
            currentQuantity = 0,
            createdAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Stok kalemi güncelle
    /// Ne: Mevcut stok kalemi bilgilerini güncelleyen endpoint.
    /// Neden: Stok bilgisi değişiklikleri için gereklidir.
    /// Kim Kullanacak: Deposu.
    /// Amacı: Stok kalemi bilgilerinin güncellenmesi.
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateInventoryItemRequest request)
    {
        _logger.LogInformation("Updating inventory item: {InventoryId}", id);

        return Ok(new
        {
            id = id,
            name = request.Name,
            minimumQuantity = request.MinimumQuantity,
            maximumQuantity = request.MaximumQuantity,
            criticalQuantity = request.CriticalQuantity,
            unitPrice = request.UnitPrice,
            warehouseId = request.WarehouseId,
            shelfLocation = request.ShelfLocation,
            supplierId = request.SupplierId,
            description = request.Description,
            updatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Stok girişi yap
    /// Ne: Stoka yeni miktar ekleyen endpoint.
    /// Neden: Stok giriş işlemleri için gereklidir.
    /// Kim Kullanacak: Deposu.
    /// Amacı: Stok girişi kaydetme.
    /// </summary>
    [HttpPost("{id:guid}/stock-in")]
    public IActionResult StockIn(Guid id, [FromBody] StockInRequest request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new { error = "Geçerli bir miktar giriniz." });
        }

        _logger.LogInformation("Stock in: {InventoryId}, Quantity: {Quantity}", id, request.Quantity);

        return Ok(new
        {
            id = id,
            quantity = request.Quantity,
            previousQuantity = 150,
            newQuantity = 150 + request.Quantity,
            transactionType = "StockIn",
            notes = request.Notes,
            transactionDate = DateTime.UtcNow,
            message = "Stok girişi başarıyla yapıldı."
        });
    }

    /// <summary>
    /// Stok çıkışı yap
    /// Ne: Stoktan miktar düşen endpoint.
    /// Neden: Stok çıkış işlemleri için gereklidir.
    /// Kim Kullanacak: Deposu, Eczane.
    /// Amacı: Stok çıkışı kaydetme.
    /// </summary>
    [HttpPost("{id:guid}/stock-out")]
    public IActionResult StockOut(Guid id, [FromBody] StockOutRequest request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new { error = "Geçerli bir miktar giriniz." });
        }

        _logger.LogInformation("Stock out: {InventoryId}, Quantity: {Quantity}", id, request.Quantity);

        return Ok(new
        {
            id = id,
            quantity = request.Quantity,
            previousQuantity = 150,
            newQuantity = 150 - request.Quantity,
            transactionType = "StockOut",
            reason = request.Reason,
            notes = request.Notes,
            transactionDate = DateTime.UtcNow,
            message = "Stok çıkışı başarıyla yapıldı."
        });
    }

    /// <summary>
    /// Stok raporları
    /// Ne: Stok raporlarını getiren endpoint.
    /// Neden: Yönetim raporları için gereklidir.
    /// Kim Kullanacak: Yönetim, Raporlama.
    /// Amacı: Stok istatistiklerinin görüntülenmesi.
    /// </summary>
    [HttpGet("reports")]
    public IActionResult GetReports()
    {
        _logger.LogInformation("Getting inventory reports");

        return Ok(new
        {
            totalItems = 500,
            totalValue = 2500000.00m,
            criticalItems = 15,
            expiredItems = 3,
            outOfStock = 5,
            byType = new
            {
                medication = 150,
                medicalSupply = 200,
                labSupply = 80,
                surgerySupply = 50,
                other = 20
            }
        });
    }

    /// <summary>
    /// Stok hareket geçmişi
    /// Ne: Stok kaleminin hareket geçmişini getiren endpoint.
    /// Neden: İzleme için gereklidir.
    /// Kim Kullanacak: Deposu, Denetim.
    /// Amacı: Stok hareketlerinin görüntülenmesi.
    /// </summary>
    [HttpGet("{id:guid}/history")]
    public IActionResult GetHistory(Guid id, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        _logger.LogInformation("Getting inventory history: {InventoryId}", id);

        return Ok(new object[]
        {
            new {
                id = Guid.NewGuid(),
                transactionType = "StockIn",
                quantity = 100,
                previousQuantity = 50,
                newQuantity = 150,
                notes = "Sipariş teslimatı",
                transactionDate = DateTime.Now.AddDays(-5),
                performedBy = "Kullanıcı 1"
            },
            new {
                id = Guid.NewGuid(),
                transactionType = "StockOut",
                quantity = 10,
                previousQuantity = 160,
                newQuantity = 150,
                reason = "Eczane çıkışı",
                notes = "Reçete: RC-2024-001",
                transactionDate = DateTime.Now.AddDays(-2),
                performedBy = "Kullanıcı 2"
            }
        });
    }
}

/// <summary>
/// Stok kalemi oluşturma istek modeli
/// Ne: Stok kalemi oluşturma endpoint'i için gerekli input modeli.
/// Neden: API üzerinden stok kalemi verisi almak için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Yeni stok kalemi oluşturma parametrelerini taşımak.
/// </summary>
public class CreateInventoryItemRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public InventoryType Type { get; set; }
    public string? Barcode { get; set; }
    public string Unit { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal MaximumQuantity { get; set; }
    public decimal CriticalQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public bool TrackExpiryDate { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? ShelfLocation { get; set; }
    public Guid? SupplierId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Stok kalemi güncelleme istek modeli
/// Ne: Stok kalemi güncelleme endpoint'i için gerekli input modeli.
/// Neden: Mevcut stok kalemi bilgilerini güncellemek için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Stok kalemi güncelleme parametrelerini taşımak.
/// </summary>
public class UpdateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal MinimumQuantity { get; set; }
    public decimal MaximumQuantity { get; set; }
    public decimal CriticalQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? ShelfLocation { get; set; }
    public Guid? SupplierId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Stok girişi istek modeli
/// Ne: Stok girişi endpoint'i için gerekli input modeli.
/// Neden: Giriş bilgilerini almak için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Stok girişi parametrelerini taşımak.
/// </summary>
public class StockInRequest
{
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// Stok çıkışı istek modeli
/// Ne: Stok çıkışı endpoint'i için gerekli input modeli.
/// Neden: Çıkış bilgilerini almak için gereklidir.
/// Kim Kullanacak: Deposu, Eczane.
/// Amacı: Stok çıkışı parametrelerini taşımak.
/// </summary>
public class StockOutRequest
{
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
