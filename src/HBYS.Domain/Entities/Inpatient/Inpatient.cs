using HBYS.Domain.Entities;
using HBYS.Domain.Entities.Patient;

namespace HBYS.Domain.Entities.Inpatient;

/// <summary>
/// Yatan Hasta Entity Sınıfı
/// Ne: HBYS sisteminin yatan hasta yönetimi için temel varlık sınıfıdır.
/// Neden: Hasta yatış işlemlerinin takibi, oda/yatak yönetimi ve taburculuk için gereklidir.
/// Özelliği: Yatış/çıkış süreci, oda transfer, hemşire bakım planı, günlük vizit takibi.
/// Kim Kullanacak: Yatan hasta, Servis, Hemşire, Doktor, Eczane, Faturalama.
/// Amacı: Hastane yatan hasta sürecinin merkezi yönetimi.
/// </summary>
public class Inpatient : BaseEntity
{
    /// <summary>
    /// Yatış numarası
    /// Ne: Otomatik oluşturulan benzersiz yatış numarası.
/// Neden: Yatış takibi için gereklidir.
/// Kim Kullanacak: Tüm yatan hasta işlemleri.
/// Amacı: Yatış tekil tanımlama.
/// </summary>
    public string AdmissionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hasta ID
    /// Ne: Yatan hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile yatış arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hasta yatış ilişkisi.
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
    /// Yatış tarihi
    /// Ne: Hastanın yatırıldığı tarih.
/// Neden: Yatış zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Yatış zamanı takibi.
/// </summary>
    public DateTime AdmissionDate { get; set; }

    /// <summary>
    /// Yatış saati
    /// Ne: Hastanın yatırıldığı saat.
/// Neden: Detaylı zaman takibi için gereklidir.
/// Kim Kullanacak: Servis, Hemşire.
/// Amacı: Yatış saati takibi.
/// </summary>
    public TimeSpan AdmissionTime { get; set; }

    /// <summary>
    /// Yatış tipi
    /// Ne: Yatışın türü (planlı, acil).
    /// Neden: Yatış türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Servis, Acil servis.
/// Amacı: Yatış tipi yönetimi.
/// </summary>
    public AdmissionType AdmissionType { get; set; }

    /// <summary>
    /// Yatış sebebi
    /// Ne: Hastanın yatış sebebi.
/// Neden: Tıbbi dokümantasyon için gereklidir.
/// Kim Kullanacak: Doktor, Servis.
/// Amacı: Yatış sebebi bilgisi.
/// </summary>
    public string? AdmissionReason { get; set; }

    /// <summary>
    /// Yatıştan önceki tanı
    /// Ne: Yatış sırasında konulan ön tanı.
/// Neden: Tıbbi takip için gereklidir.
/// Kim Kullanacak: Doktor, Servis.
/// Amacı: Ön tanı bilgisi.
/// </summary>
    public string? PreliminaryDiagnosis { get; set; }

    /// <summary>
    /// Bölüm ID
    /// Ne: Hastanın yattığı bölüm.
/// Neden: Bölüm-yatış ilişkisi için gereklidir.
/// Kim Kullanacak: Servis, Raporlama.
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
    /// Oda ID
    /// Ne: Hastanın yattığı oda.
/// Neden: Oda-yatış ilişkisi için gereklidir.
/// Kim Kullanacak: Servis, Oda yönetimi.
/// Amacı: Oda takibi.
/// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Oda numarası (denormalize)
    /// Ne: Odanın numarası.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Oda bilgisi.
/// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Yatak numarası
    /// Ne: Hastanın yattığı yatak.
/// Neden: Yatak takibi için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Yatak bilgisi.
/// </summary>
    public string BedNumber { get; set; } = string.Empty;

    /// <summary>
    /// Sorumlu doktor ID
    /// Ne: Hastanın sorumlu doktoru.
/// Neden: Doktor-yatış ilişkisi için gereklidir.
/// Kim Kullanacak: Doktor paneli, Servis.
/// Amacı: Doktor takibi.
/// </summary>
    public Guid ResponsibleDoctorId { get; set; }

    /// <summary>
    /// Sorumlu doktor adı (denormalize)
    /// Ne: Sorumlu doktorun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Doktor bilgisi.
/// </summary>
    public string ResponsibleDoctorName { get; set; } = string.Empty;

    /// <summary>
    /// Yatış durumu
    /// Ne: Yatan hastanın mevcut durumu.
/// Neden: İş akışı yönetimi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Yatış durumu takibi.
/// </summary>
    public InpatientStatus Status { get; set; }

    /// <summary>
    /// Taburculuk tarihi
    /// Ne: Hastanın taburcu edh.
/// Nedenildiği tari: Çıkış takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Taburculuk zamanı.
/// </summary>
    public DateTime? DischargeDate { get; set; }

    /// <summary>
    /// Taburculuk saati
    /// Ne: Hastanın taburcu edildiği saat.
/// Neden: Detaylı çıkış takibi için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Taburculuk saati.
/// </summary>
    public TimeSpan? DischargeTime { get; set; }

    /// <summary>
    /// Taburculuk sebebi
    /// Ne: Hastanın taburcu edilme sebebi.
/// Neden: Taburculuk dokümantasyonu için gereklidir.
/// Kim Kullanacak: Doktor, Servis.
/// Amacı: Çıkış sebebi.
/// </summary>
    public string? DischargeReason { get; set; }

    /// <summary>
    /// Taburculuk tanısı
    /// Ne: Taburculukta konulan kesin tanı.
/// Neden: Tıbbi dokümantasyon için gereklidir.
/// Kim Kullanacak: Doktor, Faturalama.
/// Amacı: Kesin tanı.
/// </summary>
    public string? FinalDiagnosis { get; set; }

    /// <summary>
    /// Taburculuk özeti
    /// Ne: Hastanın taburculuk özeti.
/// Neden: Epikriz için gereklidir.
/// Kim Kullanacak: Doktor, Hasta.
/// Amacı: Özet bilgi.
/// </summary>
    public string? DischargeSummary { get; set; }

    /// <summary>
    /// Öneriler
    /// Ne: Taburculuk sonrası öneriler.
/// Neden: Hasta bilgilendirme için gereklidir.
/// Kim Kullanacak: Doktor, Hasta.
/// Amacı: Öneriler bilgisi.
/// </summary>
    public string? Recommendations { get; set; }

    /// <summary>
    /// Kontrol randevusu tarihi
    /// Ne: Önerilen kontrol randevusu.
/// Neden: Takip için gereklidir.
/// Kim Kullanacak: Randevu, Hasta.
/// Amacı: Kontrol planlaması.
/// </summary>
    public DateTime? FollowUpAppointmentDate { get; set; }

    /// <summary>
    /// Reçeteler
    /// Ne: Taburculuk reçeteleri.
/// Neden: Eczane işlemleri için gereklidir.
/// Kim Kullanacak: Eczane, Hasta.
/// Amacı: Reçete bilgisi.
/// </summary>
    public string? Prescriptions { get; set; }

    /// <summary>
    /// Gün sayısı
    /// Ne: Hastanın kaç gün yattığı.
/// Neden: İstatistik için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Gün sayısı hesaplama.
/// </summary>
    public int DaysOfStay { get; set; }

    /// <summary>
    /// Refakatçi var mı?
    /// Ne: Hastanın refakatçisi olup olmadığı.
/// Neden: Oda planlaması için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Refakatçi durumu.
/// </summary>
    public bool HasAttendant { get; set; }

    /// <summary>
    /// Refakatçi adı
    /// Ne: Refakatçinin adı.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Refakatçi bilgisi.
/// </summary>
    public string? AttendantName { get; set; }

    /// <summary>
    /// Refakatçi telefon
    /// Ne: Refakatçinin telefonu.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: İletişim bilgisi.
/// </summary>
    public string? AttendantPhone { get; set; }

    /// <summary>
    /// Özel notlar
    /// Ne: Yatış ile ilgili özel notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Servis, Hemşire.
/// Amacı: Not saklama.
/// </summary>
    public string? SpecialNotes { get; set; }

    /// <summary>
    /// Acil durum bilgisi
    /// Ne: Acil durumda ulaşılacak kişi.
/// Neden: Acil iletişim için gereklidir.
/// Kim Kullanacak: Servis.
/// Amacı: Acil durum bilgisi.
/// </summary>
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Sigorta türü
    /// Ne: Hastanın sigorta türü.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Sigorta bilgisi.
/// </summary>
    public InsuranceType InsuranceType { get; set; }

    /// <summary>
    /// Oda ücreti (günlük)
    /// Ne: Odanın günlük ücreti.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Ücret hesaplama.
/// </summary>
    public decimal DailyRoomRate { get; set; }

    /// <summary>
    /// Toplam ücret
    /// Ne: Yatışın toplam ücreti.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Toplam hesaplama.
/// </summary>
    public decimal TotalCharge { get; set; }
}

/// <summary>
/// Yatış tipi enum
/// Ne: Yatış tiplerini tanımlayan enum.
/// Neden: Yatış türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Servis, Acil servis.
/// Amacı: Yatış tiplerinin standartlaştırılması.
/// </summary>
public enum AdmissionType
{
    /// <summary>
    /// Planlı yatış
    /// </summary>
    Scheduled = 1,
    /// <summary>
    /// Acil yatış
    /// </summary>
    Emergency = 2,
    /// <summary>
    /// Transfer yatış
    /// </summary>
    Transfer = 3,
    /// <summary>
    /// Yoğun bakım
    /// </summary>
    IntensiveCare = 4
}

/// <summary>
/// Yatan hasta durumu enum
/// Ne: Yatan hasta durumlarını tanımlayan enum.
/// Neden: Yatan hasta iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Servis, Hemşire, Doktor.
/// Amacı: Yatan hasta durumlarının standartlaştırılması.
/// </summary>
public enum InpatientStatus
{
    /// <summary>
    /// Yatış yapıldı
    /// </summary>
    Admitted = 1,
    /// <summary>
    /// Tedavi görüyor
    /// </summary>
    UnderTreatment = 2,
    /// <summary>
    /// Ameliyatta
    /// </summary>
    InSurgery = 3,
    /// <summary>
    /// Yoğun bakımda
    /// </summary>
    InIntensiveCare = 4,
    /// <summary>
    /// Taburcu edildi
    /// </summary>
    Discharged = 5,
    /// <summary>
    /// Sevk edildi
    /// </summary>
    Transferred = 6,
    /// <summary>
    /// Exitus (Ölüm)
    /// </summary>
    Deceased = 7,
    /// <summary>
    /// İsteği üzerine çıkış
    /// </summary>
    LeftAgainstMedicalAdvice = 8
}
