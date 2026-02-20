using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Procurement;

/// <summary>
/// Satın Alma Talebi Entity Sınıfı
/// Ne: Departmanlardan gelen satın alma taleplerini temsil eden varlık sınıfıdır.
/// Neden: Satın alma ihtiyaçlarının merkezi yönetimi için gereklidir.
/// Özelliği: Talep yönetimi, onay akışı, öncelik.
/// Kim Kullanacak: Tüm departmanlar, Satın alma, Yönetim.
/// Amacı: Satın alma taleplerinin merkezi yönetimi.
/// </summary>
public class Requisition : BaseEntity
{
    /// <summary>
    /// Talep numarası
    /// Ne: Otomatik oluşturulan benzersiz talep numarası.
/// Neden: Talep takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Talep tekil tanımlama.
/// </summary>
    public string RequisitionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Talep tarihi
    /// Ne: Talebin oluşturulduğu tarih.
/// Neden: Talep zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Talep zamanı takibi.
/// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// İstenen tarih
    /// Ne: Ürünlerin ihtiyaç duyulduğu tarih.
/// Neden: Planlama için gereklidir.
/// Kim Kullanacak: Satın alma, Departmanlar.
/// Amacı: İhtiyaç tarihi.
/// </summary>
    public DateTime RequiredDate { get; set; }

    /// <summary>
    /// Departman ID
    /// Ne: Talebi oluşturan departman.
/// Neden: Departman takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Departman takibi.
/// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Departman adı
    /// Ne: Talebi oluşturan departmanın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Departman bilgisi.
/// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Talep eden ID
    /// Ne: Talebi oluşturan kullanıcı.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Denetim.
/// Amacı: Talep eden takibi.
/// </summary>
    public Guid RequestedById { get; set; }

    /// <summary>
    /// Talep eden adı
    /// Ne: Talebi oluşturan kullanıcının adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep eden bilgisi.
/// </summary>
    public string RequestedByName { get; set; } = string.Empty;

    /// <summary>
    /// Talep tipi
    /// Ne: Talebin türü.
/// Neden: Talep türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep tipi yönetimi.
/// </summary>
    public RequisitionType Type { get; set; }

    /// <summary>
    /// Talep durumu
    /// Ne: Talebin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Talep durumu takibi.
/// </summary>
    public RequisitionStatus Status { get; set; }

    /// <summary>
    /// Öncelik
    /// Ne: Talebin öncelik seviyesi.
/// Neden: Önceliklendirme için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Öncelik belirleme.
/// </summary>
    public Priority Priority { get; set; }

    /// <summary>
    /// Toplam tahmini tutar
    /// Ne: Talebin toplam tahmini tutarı.
/// Neden: Bütçe planlaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Tutar tahmini.
/// </summary>
    public decimal EstimatedTotalAmount { get; set; }

    /// <summary>
    /// Onaylayan ID
    /// Ne: Talebi onaylayan kullanıcı.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Onaylayan takibi.
/// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Onay tarihi
    /// Ne: Talebin onaylandığı tarih.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Onay zamanı.
/// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Reddetme sebebi
    /// Ne: Talebin reddedilme sebebi.
/// Neden: Red takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Red bilgisi.
/// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Talep ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Talebin iptal edilip edilmediği.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İptal durumu.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Sipariş ID
    /// Ne: Oluşturulan sipariş.
/// Neden: Sipariş ilişkisi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Sipariş takibi.
/// </summary>
    public Guid? PurchaseOrderId { get; set; }
}

/// <summary>
/// Talep tipi enum
/// Ne: Talep tiplerini tanımlayan enum.
/// Neden: Talep türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep tiplerinin standartlaştırılması.
/// </summary>
public enum RequisitionType
{
    /// <summary>
    /// Stok talebi
    /// </summary>
    StockRequest = 1,
    /// <summary>
    /// Acil talep
    /// </summary>
    UrgentRequest = 2,
    /// <summary>
    /// Demirbaş talebi
    /// </summary>
    EquipmentRequest = 3,
    /// <summary>
    /// Hizmet talebi
    /// </summary>
    ServiceRequest = 4
}

/// <summary>
/// Talep durumu enum
/// Ne: Talep durumlarını tanımlayan enum.
/// Neden: Talep iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Talep durumlarının standartlaştırılması.
/// </summary>
public enum RequisitionStatus
{
    /// <summary>
    /// Taslak
    /// </summary>
    Draft = 1,
    /// <summary>
    /// Gönderildi
    /// </summary>
    Submitted = 2,
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
    /// Satın alma sürecinde
    /// </summary>
    InProcurement = 6,
    /// <summary>
    /// Sipariş edildi
    /// </summary>
    Ordered = 7,
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
/// Öncelik enum
/// Ne: Öncelik seviyelerini tanımlayan enum.
/// Neden: Önceliklerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Önceliklerin standartlaştırılması.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Düşük
    /// </summary>
    Low = 1,
    /// <summary>
    /// Normal
    /// </summary>
    Normal = 2,
    /// <summary>
    /// Yüksek
    /// </summary>
    High = 3,
    /// <summary>
    /// Acil
    /// </summary>
    Critical = 4
}
