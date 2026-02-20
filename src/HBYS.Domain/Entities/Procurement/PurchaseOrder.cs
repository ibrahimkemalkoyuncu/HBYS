using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Procurement;

/// <summary>
/// Satın Alma Siparişi Entity Sınıfı
/// Ne: HBYS sisteminin satın alma yönetimi için temel varlık sınıfıdır.
/// Neden: Tedarikçi siparişleri, satın alma talepleri ve tedarik zinciri yönetimi için gereklidir.
/// Özelliği: Sipariş durumu, tedarikçi yönetimi, maliyet takibi.
/// Kim Kullanacak: Satın alma, Deposu, Yönetim, Muhasebe.
/// Amacı: Hastane satın alma sürecinin merkezi yönetimi.
/// </summary>
public class PurchaseOrder : BaseEntity
{
    /// <summary>
    /// Sipariş numarası
    /// Ne: Otomatik oluşturulan benzersiz sipariş numarası.
/// Neden: Sipariş takibi için gereklidir.
/// Kim Kullanacak: Tüm satın alma işlemleri.
/// Amacı: Sipariş tekil tanımlama.
/// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Sipariş tarihi
    /// Ne: Siparişin verildiği tarih.
/// Neden: Sipariş zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Sipariş zamanı takibi.
/// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Tedarikçi ID
    /// Ne: Siparişin verildiği tedarikçi.
/// Neden: Tedarikçi takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi takibi.
/// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Tedarikçi adı (denormalize)
    /// Ne: Tedarikçinin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Tedarikçi bilgisi.
/// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// Tedarikçi iletişim (denormalize)
    /// Ne: Tedarikçinin iletişim bilgileri.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İletişim bilgisi.
/// </summary>
    public string? SupplierContact { get; set; }

    /// <summary>
    /// Sipariş tipi
    /// Ne: Siparişin türü.
/// Neden: Sipariş türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş tipi yönetimi.
/// </summary>
    public OrderType Type { get; set; }

    /// <summary>
    /// Sipariş durumu
    /// Ne: Siparişin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Sipariş durumu takibi.
/// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Teslim tarihi
    /// Ne: Siparişin teslim edileceği tarih.
/// Neden: Planlama için gereklidir.
/// Kim Kullanacak: Satın alma, Deposu.
/// Amacı: Teslim zamanı takibi.
/// </summary>
    public DateTime? DeliveryDate { get; set; }

    /// <summary>
    /// Teslim adresi
    /// Ne: Siparişin teslim edileceği adres.
/// Neden: Teslimat için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Teslimat adresi.
/// </summary>
    public string? DeliveryAddress { get; set; }

    /// <summary>
    /// Toplam tutar
    /// Ne: Siparişin toplam tutarı.
/// Neden: Maliyet hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Tutar hesaplama.
/// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// İndirim
    /// Ne: Uygulanan indirim.
/// Neden: İndirim hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İndirim hesaplama.
/// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// KDV
    /// Ne: KDV tutarı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Vergi hesaplama.
/// </summary>
    public decimal Vat { get; set; }

    /// <summary>
    /// Net tutar
    /// Ne: Ödenecek net tutar.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Net tutar hesaplama.
/// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Ödeme tipi
    /// Ne: Ödeme şekli.
/// Neden: Ödeme planlaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Ödeme tipi belirleme.
/// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Ödeme durumu
    /// Ne: Ödeme durumu.
/// Neden: Ödeme takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Ödeme durumu takibi.
/// </summary>
    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Sipariş veren ID
    /// Ne: Siparişi veren kullanıcı.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Denetim.
/// Amacı: Sipariş veren takibi.
/// </summary>
    public Guid OrderedById { get; set; }

    /// <summary>
    /// Sipariş veren adı
    /// Ne: Siparişi veren kullanıcının adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş veren bilgisi.
/// </summary>
    public string OrderedByName { get; set; } = string.Empty;

    /// <summary>
    /// Onaylayan ID
    /// Ne: Siparişi onaylayan kullanıcı.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Onaylayan takibi.
/// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Onay tarihi
    /// Ne: Siparişin onaylandığı tarih.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Onay zamanı.
/// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Sipariş ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Siparişin iptal edilip edilmediği.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İptal durumu.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// İptal sebebi
    /// Ne: Siparişin iptal edilme sebebi.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İptal bilgisi.
/// </summary>
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Sipariş tipi enum
/// Ne: Sipariş tiplerini tanımlayan enum.
/// Neden: Sipariş türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş tiplerinin standartlaştırılması.
/// </summary>
public enum OrderType
{
    /// <summary>
    /// Stok siparişi
    /// </summary>
    StockOrder = 1,
    /// <summary>
    /// Acil sipariş
    /// </summary>
    UrgentOrder = 2,
    /// <summary>
    /// Sözleşme siparişi
    /// </summary>
    ContractOrder = 3,
    /// <summary>
    /// Teklif siparişi
    /// </summary>
    QuotationOrder = 4
}

/// <summary>
/// Sipariş durumu enum
/// Ne: Sipariş durumlarını tanımlayan enum.
/// Neden: Sipariş iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş durumlarının standartlaştırılması.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Taslak
    /// </summary>
    Draft = 1,
    /// <summary>
    /// Gönderildi
    /// </summary>
    Sent = 2,
    /// <summary>
    /// Onay bekliyor
    /// </summary>
    PendingApproval = 3,
    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 4,
    /// <summary>
    /// Reddedildi
    /// </summary>
    Rejected = 5,
    /// <summary>
    /// Tedarikçi onayı bekliyor
    /// </summary>
    SupplierConfirmation = 6,
    /// <summary>
    /// Kısmi teslim edildi
    /// </summary>
    PartiallyDelivered = 7,
    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 8,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 9
}

/// <summary>
/// Ödeme tipi enum
/// Ne: Ödeme tiplerini tanımlayan enum.
/// Neden: Ödeme şekillerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Ödeme tiplerinin standartlaştırılması.
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Peşin
    /// </summary>
    Cash = 1,
    /// <summary>
    /// Vadeli
    /// </summary>
    Credit = 2,
    /// <summary>
    /// Taksitli
    /// </summary>
    Installment = 3,
    /// <summary>
    /// Banka havalesi
    /// </summary>
    BankTransfer = 4
}

/// <summary>
/// Ödeme durumu enum
/// Ne: Ödeme durumlarını tanımlayan enum.
/// Neden: Ödeme durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Ödeme durumlarının standartlaştırılması.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Ödenmedi
    /// </summary>
    Unpaid = 1,
    /// <summary>
    /// Kısmi ödendi
    /// </summary>
    PartiallyPaid = 2,
    /// <summary>
    /// Ödendi
    /// </summary>
    Paid = 3,
    /// <summary>
    /// Vadesi geçti
    /// </summary>
    Overdue = 4
}
