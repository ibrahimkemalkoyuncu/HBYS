using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Accounting;

/// <summary>
/// Muhasebe Hareketi Entity Sınıfı
/// Ne: Hastane muhasebe hareketlerini temsil eden varlık sınıfıdır.
/// Neden: Muhasebe işlemlerinin merkezi yönetimi için gereklidir.
/// Özelliği: Gelir/gider, fatura ödemeleri, kasatransaction yönetimi.
/// Kim Kullanacak: Muhasebe, Yönetim, Finans.
/// Amacı: Muhasebe süreçlerinin merkezi yönetimi.
/// </summary>
public class AccountingTransaction : BaseEntity
{
    /// <summary>
    /// Hareket numarası
    /// Ne: Otomatik oluşturulan benzersiz hareket numarası.
/// Neden: Hareket takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hareket tekil tanımlama.
/// </summary>
    public string TransactionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hareket tarihi
    /// Ne: Hareketin yapıldığı tarih.
/// Neden: Hareket zamanı için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hareket zamanı takibi.
/// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Hareket tipi
    /// Ne: Gelir veya gider.
/// Neden: Hareket tipi bazlı işlem için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hareket tipi yönetimi.
/// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Alt tip
    /// Ne: Hareketin alt kategorisi.
/// Neden: Alt tip bazlı işlem için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Alt tip belirleme.
/// </summary>
    public string SubType { get; set; } = string.Empty;

    /// <summary>
    /// Miktar
    /// Ne: Hareketin tutarı.
/// Neden: Tutar takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Tutar belirleme.
/// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// KDV tutarı
    /// Ne: KDV tutarı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: KDV hesaplama.
/// </summary>
    public decimal VatAmount { get; set; }

    /// <summary>
    /// Toplam tutar
    /// Ne: Toplam tutar (miktar + KDV).
/// Neden: Toplam hesaplaması için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Toplam hesaplama.
/// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Para birimi
    /// Ne: İşlem para birimi.
/// Neden: Para birimi takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Para birimi belirleme.
/// </summary>
    public string Currency { get; set; } = "TRY";

    /// <summary>
    /// Kur
    /// Ne: Döviz kuru.
/// Neden: Kur hesaplaması için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kur belirleme.
/// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Kaynak tipi
    /// Ne: Hareketin kaynağı (Fatura, Tahsilat, Ödeme).
/// Neden: Kaynak takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kaynak belirleme.
/// </summary>
    public string? SourceType { get; set; }

    /// <summary>
    /// Kaynak ID
    /// Ne: Hareketin kaynağındaki ID.
/// Neden: Kaynak ilişkisi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kaynak takibi.
/// </summary>
    public Guid? SourceId { get; set; }

    /// <summary>
    /// Hasta ID
    /// Ne: İlişkili hasta (varsa).
/// Neden: Hasta takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hasta ilişkisi.
/// </summary>
    public Guid? PatientId { get; set; }

    /// <summary>
    /// Hasta adı
    /// Ne: Hastanın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hasta bilgisi.
/// </summary>
    public string? PatientName { get; set; }

    /// <summary>
    /// Tedarikçi ID
    /// Ne: İlişkili tedarikçi (varsa).
/// Neden: Tedarikçi takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Tedarikçi ilişkisi.
/// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Tedarikçi adı
    /// Ne: Tedarikçinin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Tedarikçi bilgisi.
/// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// Hesap kodu
    /// Ne: Muhasebe hesap kodu.
/// Neden: Hesap takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hesap belirleme.
/// </summary>
    public string AccountCode { get; set; } = string.Empty;

    /// <summary>
    /// Hesap adı
    /// Ne: Muhasebe hesabının adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Hesap bilgisi.
/// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Açıklama
    /// Ne: Hareket açıklaması.
/// Neden: Bilgi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Açıklama saklama.
/// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ödeme tipi
    /// Ne: Ödeme şekli.
/// Neden: Ödeme takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Ödeme tipi belirleme.
/// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Ödeme durumu
    /// Ne: Ödeme durumu.
/// Neden: Ödeme takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Ödeme durumu.
/// </summary>
    public PaymentTransactionStatus PaymentStatus { get; set; }

    /// <summary>
    /// Tahsilat/Ödeme tarihi
    /// Ne: Nakit veya banka işlem tarihi.
/// Neden: Kasa takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kasa zamanı.
/// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// Banka
    /// Ne: Banka adı.
/// Neden: Banka takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Banka bilgisi.
/// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// IBAN
    /// Ne: IBAN numarası.
/// Neden: IBAN takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: IBAN bilgisi.
/// </summary>
    public string? Iban { get; set; }

    /// <summary>
    /// Çek/Senet numarası
    /// Ne: Çek veya senet numarası.
/// Neden: Çek/Senet takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Çek/Senet bilgisi.
/// </summary>
    public string? CheckNumber { get; set; }

    /// <summary>
    /// Vade tarihi
    /// Ne: Ödeme vade tarihi.
/// Neden: Vade takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Vade bilgisi.
/// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Kayıtlı/kapanış tarihi
    /// Ne: Muhasebe kayıt veya kapanış tarihi.
/// Neden: Kayıt takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kayıt zamanı.
/// </summary>
    public DateTime? PostedDate { get; set; }

    /// <summary>
    /// Kayıtlı/kapanış yapan
    /// Ne: Muhasebe kaydı yapan kullanıcı.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Kullanıcı takibi.
/// </summary>
    public Guid? PostedById { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Hareket iptal edilmiş mi?
    /// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: İptal durumu.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// İptal sebebi
    /// Ne: İptal edilme sebebi.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: İptal bilgisi.
/// </summary>
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Hareket tipi enum
/// Ne: Hareket tiplerini tanımlayan enum.
/// Neden: Tiplerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Tiplerin standartlaştırılması.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Gelir
    /// </summary>
    Income = 1,
    /// <summary>
    /// Gider
    /// </summary>
    Expense = 2
}

/// <summary>
/// Ödeme yöntemi enum
/// Ne: Ödeme yöntemlerini tanımlayan enum.
/// Neden: Yöntemlerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Yöntemlerin standartlaştırılması.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Nakit
    /// </summary>
    Cash = 1,
    /// <summary>
    /// Kredi kartı
    /// </summary>
    CreditCard = 2,
    /// <summary>
    /// Banka havalesi
    /// </summary>
    BankTransfer = 3,
    /// <summary>
    /// Çek
    /// </summary>
    Check = 4,
    /// <summary>
    /// Senet
    /// </summary>
    PromissoryNote = 5
}

/// <summary>
/// Ödeme durumu enum
/// Ne: Ödeme durumlarını tanımlayan enum.
/// Neden: Durumların tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Muhasebe.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum PaymentTransactionStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Ödendi
    /// </summary>
    Paid = 2,
    /// <summary>
    /// Kısmi ödendi
    /// </summary>
    PartiallyPaid = 3,
    /// <summary>
    /// Vadesi geçti
    /// </summary>
    Overdue = 4,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 5
}
