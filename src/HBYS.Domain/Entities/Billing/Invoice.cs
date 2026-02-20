using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Billing;

/// <summary>
/// Fatura Entity Sınıfı
/// Ne: HBYS sisteminin fatura yönetimi için temel varlık sınıfıdır.
/// Neden: Hasta hizmet faturalaması, ödeme takibi ve muhasebe entegrasyonu için gereklidir.
/// Özelliği: SGK ve özel sigorta desteği, detaylı kalem yapısı, ödeme planı.
/// Kim Kullanacak: Faturalama, Muhasebe, Hasta kabul, Eczane, Laboratuvar.
/// Amacı: Hastane fatura sisteminin merkezi yönetimi.
/// </summary>
public class Invoice : BaseEntity
{
    /// <summary>
    /// Fatura numarası
    /// Ne: Otomatik oluşturulan benzersiz fatura numarası.
/// Neden: Fatura takibi için gereklidir.
/// Kim Kullanacak: Tüm fatura işlemleri.
/// Amacı: Fatura tekil tanımlama.
/// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Fatura tarihi
    /// Ne: Faturanın kesildiği tarih.
/// Neden: Fatura tarihi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Fatura zamanı takibi.
/// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// Hasta ID
    /// Ne: Fatura kesilen hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile fatura arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Faturalama, Hasta.
/// Amacı: Hasta fatura ilişkisi.
/// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Hasta adı (denormalize)
    /// Ne: Hastanın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hızlı görüntüleme.
/// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Hasta TC Kimlik (denormalize)
    /// Ne: Hastanın TC Kimlik numarası.
/// Neden: Kimlik doğrulama için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hasta kimlik bilgisi.
/// </summary>
    public string PatientTckn { get; set; } = string.Empty;

    /// <summary>
    /// Hasta adresi (denormalize)
    /// Ne: Hastanın adresi.
/// Neden: Fatura kesimi için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Fatura adresi.
/// </summary>
    public string? PatientAddress { get; set; }

    /// <summary>
    /// Fatura tipi
    /// Ne: Faturanın türü (muayene, tetkik, ilaç, yatan hasta).
    /// Neden: Fatura türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Fatura tipi yönetimi.
/// </summary>
    public InvoiceType Type { get; set; }

    /// <summary>
    /// Fatura durumu
    /// Ne: Faturanın ödeme durumu.
/// Neden: Ödeme takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Fatura durumu takibi.
/// </summary>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// Sigorta türü
    /// Ne: Hastanın sigorta türü.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Faturalama, SGK entegrasyonu.
/// Amacı: Sigorta bilgisi.
/// </summary>
    public InsuranceType InsuranceType { get; set; }

    /// <summary>
    /// Sigorta şirketi ID
    /// Ne: Özel sigorta için şirket.
/// Neden: Özel sigorta faturalaması için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Sigorta şirketi takibi.
/// </summary>
    public Guid? InsuranceCompanyId { get; set; }

    /// <summary>
    /// Sigorta şirketi adı
    /// Ne: Sigorta şirketinin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Sigorta şirketi bilgisi.
/// </summary>
    public string? InsuranceCompanyName { get; set; }

    /// <summary>
    /// Poliçe numarası
    /// Ne: Sigorta polüçe numarası.
/// Neden: Sigorta doğrulama için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Poliçe takibi.
/// </summary>
    public string? PolicyNumber { get; set; }

    /// <summary>
    /// Brüt toplam
    /// Ne: Fatura kalemlerinin toplam brüt tutarı.
/// Neden: Fatura hesaplaması için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Brüt tutar hesaplama.
/// </summary>
    public decimal GrossTotal { get; set; }

    /// <summary>
    /// İndirim toplam
    /// Ne: Toplam indirim tutarı.
/// Neden: İndirim hesaplaması için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: İndirim hesaplama.
/// </summary>
    public decimal DiscountTotal { get; set; }

    /// <summary>
    /// KDV toplam
    /// Ne: Toplam KDV tutarı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Faturalama, Muhasebe.
/// Amacı: KDV hesaplama.
/// </summary>
    public decimal VatTotal { get; set; }

    /// <summary>
    /// Net toplam
    /// Ne: Ödenecek net tutar.
/// Neden: Fatura toplamı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Net tutar hesaplama.
/// </summary>
    public decimal NetTotal { get; set; }

    /// <summary>
    /// SGK indirimi
    /// Ne: SGK tarafından karşılanan tutar.
/// Neden: SGK faturalaması için gereklidir.
/// Kim Kullanacak: Faturalama, SGK.
/// Amacı: SGK tutarı.
/// </summary>
    public decimal SgkDiscount { get; set; }

    /// <summary>
    /// Hasta payı
    /// Ne: Hastanın ödeyeceği tutar.
/// Neden: Hasta ödemesi için gereklidir.
/// Kim Kullanacak: Faturalama, Hasta.
/// Amacı: Hasta payı hesaplama.
/// </summary>
    public decimal PatientShare { get; set; }

    /// <summary>
    /// Ödenen tutar
    /// Ne: Şu ana kadar ödenen toplam tutar.
/// Neden: Ödeme takibi için gereklidir.
/// Kim Kullanacak: Faturalama, Kasa.
/// Amacı: Ödeme takibi.
/// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Kalan tutar
    /// Ne: Ödenmeyen kalan tutar.
/// Neden: Kalan borç takibi için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Kalan tutar takibi.
/// </summary>
    public decimal RemainingAmount { get; set; }

    /// <summary>
    /// Ödeme tipi
    /// Ne: Faturanın ödeme şekli.
/// Neden: Ödeme planlaması için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Ödeme tipi belirleme.
/// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Vade tarihi
    /// Ne: Faturanın ödeneceği son tarih.
/// Neden: Vade takibi için gereklidir.
/// Kim Kullanacak: Faturalama, Muhasebe.
/// Amacı: Vade takibi.
/// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Muayene ID (varsa)
    /// Ne: İlgili poliklinik muayenesi.
/// Neden: Muayene-fatura ilişkisi için gereklidir.
/// Kim Kullanacak: Poliklinik, Faturalama.
/// Amacı: Muayene takibi.
/// </summary>
    public Guid? ExaminationId { get; set; }

    /// <summary>
    /// Yatan Hasta ID (varsa)
    /// Ne: İlgili yatan hasta işlemi.
/// Neden: Yatan hasta-fatura ilişkisi için gereklidir.
/// Kim Kullanacak: Yatan hasta, Faturalama.
/// Amacı: Yatan hasta takibi.
/// </summary>
    public Guid? InpatientId { get; set; }

    /// <summary>
    /// Reçete ID (varsa)
    /// Ne: İlgili eczane reçetesi.
/// Neden: Reçete-fatura ilişkisi için gereklidir.
/// Kim Kullanacak: Eczane, Faturalama.
/// Amacı: Reçete takibi.
/// </summary>
    public Guid? PrescriptionId { get; set; }

    /// <summary>
    /// Açıklama
    /// Ne: Fatura ile ilgili ek açıklama.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Açıklama saklama.
/// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Fatura ile ilgili özel notlar.
/// Neden: Not tutmak için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Fatura kesildi mi?
    /// Ne: Faturanın kesilip kesilmediği.
/// Neden: Fatura durumu için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Fatura kesim durumu.
/// </summary>
    public bool IsIssued { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Faturanın iptal edilip edilmediği.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: İptal durumu.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// İptal sebebi
    /// Ne: Faturanın iptal edilme sebebi.
/// Neden: İptal raporları için gereklidir.
/// Kim Kullanacak: Faturalama, Raporlama.
/// Amacı: İptal bilgisi.
/// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// Ne: Faturanın sisteme kaydedildiği zaman.
/// Neden: İzleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Fatura zamanı takibi.
/// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Fatura tipi enum
/// Ne: Fatura tiplerini tanımlayan enum.
/// Neden: Fatura türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Tüm fatura işlemleri.
/// Amacı: Fatura tiplerinin standartlaştırılması.
/// </summary>
public enum InvoiceType
{
    /// <summary>
    /// Poliklinik muayene
    /// </summary>
    Outpatient = 1,
    /// <summary>
    /// Yatan hasta
    /// </summary>
    Inpatient = 2,
    /// <summary>
    /// Acil servis
    /// </summary>
    Emergency = 3,
    /// <summary>
    /// Eczane ilaç
    /// </summary>
    Pharmacy = 4,
    /// <summary>
    /// Laboratuvar tetkik
    /// </summary>
    Laboratory = 5,
    /// <summary>
    /// Radyoloji tetkik
    /// </summary>
    Radiology = 6,
    /// <summary>
    /// Ameliyat
    /// </summary>
    Surgery = 7,
    /// <summary>
    /// Diğer
    /// </summary>
    Other = 99
}

/// <summary>
/// Fatura durumu enum
/// Ne: Fatura durumlarını tanımlayan enum.
/// Neden: Fatura iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Tüm fatura işlemleri.
/// Amacı: Fatura durumlarının standartlaştırılması.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// Taslak - Fatura henüz kesilmedi
    /// </summary>
    Draft = 1,
    /// <summary>
    /// Kesildi - Fatura kesildi, ödeme bekleniyor
    /// </summary>
    Issued = 2,
    /// <summary>
    /// Kısmi ödenmiş - Kısmi ödeme yapıldı
    /// </summary>
    PartiallyPaid = 3,
    /// <summary>
    /// Ödendi - Tamamen ödendi
    /// </summary>
    Paid = 4,
    /// <summary>
    /// Vadesi geçti - Ödeme vadesi doldu
    /// </summary>
    Overdue = 5,
    /// <summary>
    /// İptal edildi - Fatura iptal oldu
    /// </summary>
    Cancelled = 6,
    /// <summary>
    /// Tahsil edildi - Ödeme tahsil edildi (muhasebe)
    /// </summary>
    Collected = 7
}

/// <summary>
/// Sigorta türü enum
/// Ne: Sigorta türlerini tanımlayan enum.
/// Neden: Sigorta bilgisinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Faturalama, Hasta kabul.
/// Amacı: Sigorta türlerinin standartlaştırılması.
/// </summary>
public enum InsuranceType
{
    /// <summary>
    /// Ücretli (sigortasız)
    /// </summary>
    SelfPay = 0,
    /// <summary>
    /// Sosyal Güvenlik Kurumu
    /// </summary>
    SGK = 1,
    /// <summary>
    /// Özel sağlık sigortası
    /// </summary>
    Private = 2,
    /// <summary>
    /// Tamamlayıcı sağlık sigortası
    /// </summary>
    Supplementary = 3
}

/// <summary>
/// Ödeme tipi enum
/// Ne: Ödeme tiplerini tanımlayan enum.
/// Neden: Ödeme şekillerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Faturalama, Kasa.
/// Amacı: Ödeme tiplerinin standartlaştırılması.
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Peşin
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
    /// Senet
    /// </summary>
    PromissoryNote = 4,
    /// <summary>
    /// Çek
    /// </summary>
    Check = 5,
    /// <summary>
    /// Taksitli
    /// </summary>
    Installment = 6,
    /// <summary>
    /// SGK direkt
    /// </summary>
    SgkDirect = 7
}
