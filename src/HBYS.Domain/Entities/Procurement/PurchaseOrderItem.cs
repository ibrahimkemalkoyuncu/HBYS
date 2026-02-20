using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Procurement;

/// <summary>
/// Satın Alma Siparişi Kalem Entity Sınıfı
/// Ne: Satın alma siparişinin detay kalemlerini temsil eden varlık sınıfıdır.
/// Neden: Sipariş içindeki ürünlerin yönetimi için gereklidir.
/// Özelliği: Miktar, birim, fiyat yönetimi.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Sipariş kalemlerinin merkezi yönetimi.
/// </summary>
public class PurchaseOrderItem : BaseEntity
{
    /// <summary>
    /// Sipariş ID
    /// Ne: İlişkili olduğu sipariş.
/// Neden: Sipariş kalemi ilişkisi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş ilişkisi.
/// </summary>
    public Guid PurchaseOrderId { get; set; }

    /// <summary>
    /// Ürün/Stok ID
    /// Ne: Sipariş edilen ürün/stok.
/// Neden: Ürün takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Ürün takibi.
/// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Ürün kodu
    /// Ne: Ürünün kodu.
/// Neden: Ürün tanımlama için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Ürün kodu.
/// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// Ürün adı
    /// Ne: Ürünün adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Ürün adı.
/// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Miktar
    /// Ne: Sipariş edilen miktar.
/// Neden: Miktar takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Miktar belirleme.
/// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Birim
    /// Ne: Miktar birimi (adet, koli, kg).
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Birim belirleme.
/// </summary>
    public string Unit { get; set; } = "ADET";

    /// <summary>
    /// Birim fiyat
    /// Ne: Ürünün birim fiyatı.
/// Neden: Fiyat hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Birim fiyat.
/// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// İndirim oranı
    /// Ne: Ürüne uygulanan indirim oranı.
/// Neden: İndirim hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İndirim oranı.
/// </summary>
    public decimal DiscountRate { get; set; }

    /// <summary>
    /// İndirim tutarı
    /// Ne: Ürüne uygulanan indirim tutarı.
/// Neden: İndirim hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İndirim tutarı.
/// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// KDV oranı
    /// Ne: Ürünün KDV oranı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: KDV oranı.
/// </summary>
    public decimal VatRate { get; set; }

    /// <summary>
    /// KDV tutarı
    /// Ne: Ürünün KDV tutarı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: KDV tutarı.
/// </summary>
    public decimal VatAmount { get; set; }

    /// <summary>
    /// Toplam
    /// Ne: Kalemin toplam tutarı.
/// Neden: Tutar hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Toplam hesaplama.
/// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Teslim edilen miktar
    /// Ne: Bugüne kadar teslim edilen miktar.
/// Neden: Teslimat takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Teslimat takibi.
/// </summary>
    public decimal DeliveredQuantity { get; set; }

    /// <summary>
    /// Bekleyen miktar
    /// Ne: Henüz teslim edilmeyen miktar.
/// Neden: Bekleyen miktar takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Bekleyen miktar.
/// </summary>
    public decimal PendingQuantity => Quantity - DeliveredQuantity;

    /// <summary>
    /// Kalem durumu
    /// Ne: Kalemin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Durum takibi.
/// </summary>
    public ItemStatus Status { get; set; }

    /// <summary>
    /// Talep ID
    /// Ne: İlişkili satın alma talebi.
/// Neden: Talep ilişkisi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep takibi.
/// </summary>
    public Guid? RequisitionId { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Kalem ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Kalem durumu enum
/// Ne: Kalem durumlarını tanımlayan enum.
/// Neden: Kalem iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kalem durumlarının standartlaştırılması.
/// </summary>
public enum ItemStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 2,
    /// <summary>
    /// Sipariş edildi
    /// </summary>
    Ordered = 3,
    /// <summary>
    /// Kısmi teslim edildi
    /// </summary>
    PartiallyDelivered = 4,
    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 5,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 6
}
