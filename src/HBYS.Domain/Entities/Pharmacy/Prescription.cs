using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Pharmacy;

/// <summary>
/// Reçete Entity Sınıfı
/// Ne: HBYS sisteminin eczane ve ilaç yönetimi için temel varlık sınıfıdır.
/// Neden: Reçete işlemleri, ilaç satış, stok takibi ve ecza dolabı yönetimi için gereklidir.
/// Özelliği: Reçete oluşturma, ilaç veriliş takibi, stok yönetimi.
/// Kim Kullanacak: Eczane, Doktor, Hemşire, Poliklinik, Yatan hasta.
/// Amacı: Hastane eczane sürecinin merkezi yönetimi.
/// </summary>
public class Prescription : BaseEntity
{
    /// <summary>
    /// Reçete numarası
    /// Ne: Otomatik oluşturulan benzersiz reçete numarası.
/// Neden: Reçete takibi için gereklidir.
/// Kim Kullanacak: Tüm eczane işlemleri.
/// Amacı: Reçete tekil tanımlama.
/// </summary>
    public string PrescriptionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Reçete tarihi
    /// Ne: Reçetenin kesildiği tarih.
/// Neden: Reçete zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Reçete zamanı takibi.
/// </summary>
    public DateTime PrescriptionDate { get; set; }

    /// <summary>
    /// Hasta ID
    /// Ne: Reçetenin yazıldığı hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile reçete arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hasta reçete ilişkisi.
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
    /// Reçete tipi
    /// Ne: Reçetenin türü.
/// Neden: Reçete türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Eczane, Doktor.
/// Amacı: Reçete tipi yönetimi.
/// </summary>
    public PrescriptionType Type { get; set; }

    /// <summary>
    /// Reçete kaynağı
    /// Ne: Reçetenin hangi modülden geldiği.
/// Neden: Kaynak takibi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Kaynak belirleme.
/// </summary>
    public PrescriptionSource Source { get; set; }

    /// <summary>
    /// İlgili muayene ID
    /// Ne: Reçetenin yazıldığı muayene.
/// Neden: Muayene-reçete ilişkisi için gereklidir.
/// Kim Kullanacak: Poliklinik, Eczane.
/// Amacı: Muayene takibi.
/// </summary>
    public Guid? ExaminationId { get; set; }

    /// <summary>
    /// İlgili yatan hasta ID
    /// Ne: Reçetenin yazıldığı yatan hasta.
/// Neden: Yatan hasta-reçete ilişkisi için gereklidir.
/// Kim Kullanacak: Yatan hasta, Eczane.
/// Amacı: Yatan hasta takibi.
/// </summary>
    public Guid? InpatientId { get; set; }

    /// <summary>
    /// Doktor ID
    /// Ne: Reçeteyi yazan doktor.
/// Neden: Doktor takibi için gereklidir.
/// Kim Kullanacak: Eczane, Denetim.
/// Amacı: Doktor takibi.
/// </summary>
    public Guid DoctorId { get; set; }

    /// <summary>
    /// Doktor adı (denormalize)
    /// Ne: Reçeteyi yazan doktorun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Doktor bilgisi.
/// </summary>
    public string DoctorName { get; set; } = string.Empty;

    /// <summary>
    /// Bölüm ID
    /// Ne: Reçetenin yazıldığı bölüm.
/// Neden: Bölüm takibi için gereklidir.
/// Kim Kullanacak: Eczane, Raporlama.
/// Amacı: Bölüm takibi.
/// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Bölüm adı (denormalize)
    /// Ne: Bölümün adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Bölüm bilgisi.
/// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Reçete durumu
    /// Ne: Reçetenin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Reçete durumu takibi.
/// </summary>
    public PrescriptionStatus Status { get; set; }

    /// <summary>
    /// Toplam tutar
    /// Ne: Reçetenin toplam tutarı.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Eczane, Faturalama.
/// Amacı: Tutar hesaplama.
/// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// İndirim
    /// Ne: Uygulanan indirim.
/// Neden: İndirim hesaplaması için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: İndirim hesaplama.
/// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Net tutar
    /// Ne: Ödenecek net tutar.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Eczane, Faturalama.
/// Amacı: Net tutar hesaplama.
/// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Sigorta karşılaması
    /// Ne: Sigorta tarafından karşılanan tutar.
/// Neden: Sigorta hesaplaması için gereklidir.
/// Kim Kullanacak: Eczane, SGK.
/// Amacı: Sigorta tutarı.
/// </summary>
    public decimal InsuranceCoverage { get; set; }

    /// <summary>
    /// Hasta payı
    /// Ne: Hastanın ödeyeceği tutar.
/// Neden: Hasta ödemesi için gereklidir.
/// Kim Kullanacak: Eczane, Hasta.
/// Amacı: Hasta payı hesaplama.
/// </summary>
    public decimal PatientShare { get; set; }

    /// <summary>
    /// Veriliş tarihi
    /// Ne: İlaçların verildiği tarih.
/// Neden: Veriliş takibi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Veriliş zamanı.
/// </summary>
    public DateTime? DispensedDate { get; set; }

    /// <summary>
    /// Eczacı ID
    /// Ne: İlaçları veren eczacı.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Eczane, Denetim.
/// Amacı: Eczacı takibi.
/// </summary>
    public Guid? PharmacistId { get; set; }

    /// <summary>
    /// Eczacı adı
    /// Ne: İlaçları veren eczacının adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Eczacı bilgisi.
/// </summary>
    public string? PharmacistName { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Reçete ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Reçetenin iptal edilip edilmediği.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: İptal durumu.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// İptal sebebi
    /// Ne: Reçetenin iptal edilme sebebi.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: İptal bilgisi.
/// </summary>
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Reçete tipi enum
/// Ne: Reçete tiplerini tanımlayan enum.
/// Neden: Reçete türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Reçete tiplerinin standartlaştırılması.
/// </summary>
public enum PrescriptionType
{
    /// <summary>
    /// Normal reçete
    /// </summary>
    Normal = 1,
    /// <summary>
    /// Kırmızı reçete (kontrollü ilaçlar)
    /// </summary>
    Red = 2,
    /// <summary>
    /// Mor reçete (psikotrop ilaçlar)
    /// </summary>
    Purple = 3,
    /// <summary>
    /// Turuncu reçete
    /// </summary>
    Orange = 4,
    /// <summary>
    /// Yeşil reçete
    /// </summary>
    Green = 5,
    /// <summary>
    /// Hasta çıkış reçetesi
    /// </summary>
    Discharge = 6,
    /// <summary>
    /// Yatan hasta reçetesi
    /// </summary>
    Inpatient = 7
}

/// <summary>
/// Reçete kaynağı enum
/// Ne: Reçete kaynaklarını tanımlayan enum.
/// Neden: Kaynak türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Kaynak türlerinin standartlaştırılması.
/// </summary>
public enum PrescriptionSource
{
    /// <summary>
    /// Poliklinik
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
    /// Hekim çağrısı
    /// </summary>
    DoctorCall = 4,
    /// <summary>
    /// Taburculuk
    /// </summary>
    Discharge = 5
}

/// <summary>
/// Reçete durumu enum
/// Ne: Reçete durumlarını tanımlayan enum.
/// Neden: Reçete iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: Reçete durumlarının standartlaştırılması.
/// </summary>
public enum PrescriptionStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Hazırlanıyor
    /// </summary>
    Preparing = 2,
    /// <summary>
    /// Hazır
    /// </summary>
    Ready = 3,
    /// <summary>
    /// Verildi
    /// </summary>
    Dispensed = 4,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 5,
    /// <summary>
    /// İade edildi
    /// </summary>
    Returned = 6
}
