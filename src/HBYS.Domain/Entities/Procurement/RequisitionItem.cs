using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Procurement;

/// <summary>
/// Satın Alma Talebi Kalem Entity Sınıfı
/// Ne: Satın alma talebinin detay kalemlerini temsil eden varlık sınıfıdır.
/// Neden: Talep içindeki ürünlerin yönetimi için gereklidir.
/// Özelliği: Miktar, birim, tahmini fiyat yönetimi.
/// Kim Kullanacak: Departmanlar, Satın alma.
/// Amacı: Talep kalemlerinin merkezi yönetimi.
/// </summary>
public class RequisitionItem : BaseEntity
{
    /// <summary>
    /// Talep ID
    /// Ne: İlişkili olduğu talep.
/// Neden: Talep kalemi ilişkisi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep ilişkisi.
/// </summary>
    public Guid RequisitionId { get; set; }

    /// <summary>
    /// Ürün/Stok ID
    /// Ne: Talep edilen ürün/stok.
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
    /// Talep edilen miktar
    /// Ne: Talep edilen miktar.
/// Neden: Miktar takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Miktar belirleme.
/// </summary>
    public decimal RequestedQuantity { get; set; }

    /// <summary>
    /// Onaylanan miktar
    /// Ne: Onaylanan miktar.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Onaylanan miktar.
/// </summary>
    public decimal? ApprovedQuantity { get; set; }

    /// <summary>
    /// Birim
    /// Ne: Miktar birimi (adet, koli, kg).
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Birim belirleme.
/// </summary>
    public string Unit { get; set; } = "ADET";

    /// <summary>
    /// Tahmini birim fiyat
    /// Ne: Ürünün tahmini birim fiyatı.
/// Neden: Bütçe tahmini için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Tahmini fiyat.
/// </summary>
    public decimal? EstimatedUnitPrice { get; set; }

    /// <summary>
    /// Tahmini toplam
    /// Ne: Kalemin tahmini toplam tutarı.
/// Neden: Tutar hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Tahmini toplam.
/// </summary>
    public decimal? EstimatedTotal { get; set; }

    /// <summary>
    /// Mevcut stok
    /// Ne: Depodaki mevcut stok miktarı.
/// Neden: Stok kontrolü için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Mevcut stok takibi.
/// </summary>
    public decimal? CurrentStock { get; set; }

    /// <summary>
    /// Kalem durumu
    /// Ne: Kalemin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Durum takibi.
/// </summary>
    public RequisitionItemStatus Status { get; set; }

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
/// Talep kalem durumu enum
/// Ne: Talep kalem durumlarını tanımlayan enum.
/// Neden: Kalem iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kalem durumlarının standartlaştırılması.
/// </summary>
public enum RequisitionItemStatus
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
    /// Reddedildi
    /// </summary>
    Rejected = 3,
    /// <summary>
    /// Sipariş edildi
    /// </summary>
    Ordered = 4,
    /// <summary>
    /// Kısmi teslim edildi
    /// </summary>
    PartiallyDelivered = 5,
    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 6,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 7
}
