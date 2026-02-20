using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Emergency;

/// <summary>
/// Acil Servis Entity Sınıfı
/// Ne: HBYS sisteminin acil servis yönetimi için temel varlık sınıfıdır.
/// Neden: Acil hasta kabul, müdahale, stabilizasyon ve yatış kararı için gereklidir.
/// Özelliği: Triaj değerlendirmesi, acil müdahale kaydı, sevk/tabu.
/// Kim Kullanacak: Acil servis, Doktor, Hemşire, Ameliyathane, Yatan hasta.
/// Amacı: Acil servis sürecinin merkezi yönetimi.
/// </summary>
public class Emergency : BaseEntity
{
    /// <summary>
    /// Acil kayıt numarası
    /// Ne: Otomatik oluşturulan benzersiz acil kayıt numarası.
/// Neden: Acil takip için gereklidir.
/// Kim Kullanacak: Tüm acil işlemler.
/// Amacı: Acil kayıt tekil tanımlama.
/// </summary>
    public string EmergencyNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hasta ID
    /// Ne: Acil servise başvuran hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile acil kayıt arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hasta acil kayıt ilişkisi.
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
    /// Başvuru tarihi ve saati
    /// Ne: Hastanın acil servise başvurduğu tarih ve saat.
/// Neden: Başvuru zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Başvuru zamanı takibi.
/// </summary>
    public DateTime ArrivalDateTime { get; set; }

    /// <summary>
    /// Başvuru şekli
    /// Ne: Hastanın acil servise nasıl geldiği.
/// Neden: Başvuru kaydı için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Başvuru şekli.
/// </summary>
    public ArrivalType ArrivalType { get; set; }

    /// <summary>
    /// Başvuru nedeni
    /// Ne: Hastanın başvurma sebebi.
/// Neden: Acil değerlendirme için gereklidir.
/// Kim Kullanacak: Doktor, Hemşire.
/// Amacı: Başvuru nedeni.
/// </summary>
    public string? ReasonForVisit { get; set; }

    /// <summary>
    /// Triaj skoru
    /// Ne: Triaj değerlendirme skoru (1-5).
    /// Neden: Öncelik belirleme için gereklidir.
/// Kim Kullanacak: Hemşire, Doktor.
/// Amacı: Öncelik belirleme.
/// </summary>
    public int TriageScore { get; set; }

    /// <summary>
    /// Triaj kategorisi
    /// Ne: Triaj sonucu kategorisi.
/// Neden: Öncelik kategorisi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Kategori belirleme.
/// </summary>
    public TriageCategory TriageCategory { get; set; }

    /// <summary>
    /// Triaj notu
    /// Ne: Triaj değerlendirme notu.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Hemşire.
/// Amacı: Triaj notu.
/// </summary>
    public string? TriageNote { get; set; }

    /// <summary>
    /// Triaj hemşiresi ID
    /// Ne: Triajı yapan hemşire.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Hemşire, Denetim.
/// Amacı: Triaj sorumlusu.
/// </summary>
    public Guid? TriageNurseId { get; set; }

    /// <summary>
    /// Triaj zamanı
    /// Ne: Triajın yapıldığı zaman.
/// Neden: Triaj süresi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Triaj zamanı.
/// </summary>
    public DateTime? TriageTime { get; set; }

    /// <summary>
    /// İlk muayene zamanı
    /// Ne: İlk doktor muayenesinin yapıldığı zaman.
/// Neden: Muayene bekleme süresi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Muayene zamanı.
/// </summary>
    public DateTime? FirstExaminationTime { get; set; }

    /// <summary>
    /// Müdahale odası
    /// Ne: Hastanın müdahale edildiği oda.
/// Neden: Oda takibi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Oda bilgisi.
/// </summary>
    public string? TreatmentRoom { get; set; }

    /// <summary>
    /// Yatak/Nöbet
    /// Ne: Hastanın yatırıldığı yatak.
/// Neden: Yatak takibi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Yatak bilgisi.
/// </summary>
    public string? Bed { get; set; }

    /// <summary>
    /// Sorumlu doktor ID
    /// Ne: Hastanın sorumlu doktoru.
/// Neden: Doktor takibi için gereklidir.
/// Kim Kullanacak: Doktor, Acil servis.
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
    /// Vital bulgular
    /// Ne: İlk vital bulgular.
/// Neden: Değerlendirme için gereklidir.
/// Kim Kullanacak: Doktor, Hemşire.
/// Amacı: Vital bilgi.
/// </summary>
    public string? VitalSigns { get; set; }

    /// <summary>
    /// Şikayet/Öykü
    /// Ne: Hastanın şikayeti ve öyküsü.
/// Neden: Değerlendirme için gereklidir.
/// Kim Kullanacak: Doktor.
/// Amacı: Hasta öyküsü.
/// </summary>
    public string? Complaint { get; set; }

    /// <summary>
    /// Tanı
    /// Ne: Konulan tanı/tanılar.
/// Neden: Tedavi için gereklidir.
/// Kim Kullanacak: Doktor, Faturalama.
/// Amacı: Tanı bilgisi.
/// </summary>
    public string? Diagnosis { get; set; }

    /// <summary>
    /// Tedavi planı
    /// Ne: Uygulanan tedavi planı.
/// Neden: Tedavi takibi için gereklidir.
/// Kim Kullanacak: Doktor, Hemşire.
/// Amacı: Tedavi bilgisi.
/// </summary>
    public string? TreatmentPlan { get; set; }

    /// <summary>
    /// Yapılan işlemler
    /// Ne: Uygulanan müdahaleler.
/// Neden: İşlem takibi için gereklidir.
/// Kim Kullanacak: Doktor, Hemşire.
/// Amacı: İşlem bilgisi.
/// </summary>
    public string? Procedures { get; set; }

    /// <summary>
    /// Verilen ilaçlar
    /// Ne: Uygulanan ilaçlar.
/// Neden: İlaç takibi için gereklidir.
/// Kim Kullanacak: Hemşire, Eczane.
/// Amacı: İlaç bilgisi.
/// </summary>
    public string? MedicationsGiven { get; set; }

    /// <summary>
    /// Durum
    /// Ne: Acil servisteki mevcut durum.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Durum takibi.
/// </summary>
    public EmergencyStatus Status { get; set; }

    /// <summary>
    /// Çıkış tarihi ve saati
    /// Ne: Hastanın acil servisten çıkış zamanı.
/// Neden: Çıkış takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Çıkış zamanı.
/// </summary>
    public DateTime? DischargeDateTime { get; set; }

    /// <summary>
    /// Çıkış durumu
    /// Ne: Hastanın acil servisten çıkış şekli.
/// Neden: Sonuç takibi için gereklidir.
/// Kim Kullanacak: Acil servis, Raporlama.
/// Amacı: Çıkış durumu.
/// </summary>
    public DischargeType DischargeType { get; set; }

    /// <summary>
    /// Çıkış notu
    /// Ne: Çıkış notu.
/// Neden: Hasta bilgilendirme için gereklidir.
/// Kim Kullanacak: Doktor, Hasta.
/// Amacı: Çıkış notu.
/// </summary>
    public string? DischargeNote { get; set; }

    /// <summary>
    /// Öneriler
    /// Ne: Taburculuk önerileri.
/// Neden: Hasta bilgilendirme için gereklidir.
/// Kim Kullanacak: Doktor, Hasta.
/// Amacı: Öneriler.
/// </summary>
    public string? Recommendations { get; set; }

    /// <summary>
    /// Takip randevusu
    /// Ne: Önerilen takip randevusu.
/// Neden: Takip için gereklidir.
/// Kim Kullanacak: Randevu.
/// Amacı: Randevu planlaması.
/// </summary>
    public DateTime? FollowUpAppointment { get; set; }

    /// <summary>
    /// Sevk edildiği bölüm
    /// Ne: Yatış yapıldıysa bölüm.
/// Neden: Sevk takibi için gereklidir.
/// Kim Kullanacak: Yatan hasta.
/// Amacı: Sevk bilgisi.
/// </summary>
    public Guid? ReferredDepartmentId { get; set; }

    /// <summary>
    /// Sevk edildiği bölüm adı
    /// Ne: Yatış yapıldıysa bölüm adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Sevk bölümü.
/// </summary>
    public string? ReferredDepartmentName { get; set; }

    /// <summary>
    /// Toplam kalış süresi (dakika)
    /// Ne: Acil serviste geçirilen toplam süre.
/// Neden: Performans ölçümü için gereklidir.
/// Kim Kullanacak: Raporlama, Yönetim.
/// Amacı: Süre hesaplama.
/// </summary>
    public int TotalStayMinutes { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Ek notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Başvuru şekli enum
/// Ne: Başvuru şekillerini tanımlayan enum.
/// Neden: Başvuru türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Başvuru şekillerinin standartlaştırılması.
/// </summary>
public enum ArrivalType
{
    /// <summary>
    /// Aile ile geldi
    /// </summary>
    WithFamily = 1,
    /// <summary>
    /// Ambulans ile geldi
    /// </summary>
    ByAmbulance = 2,
    /// <summary>
    /// Polis/Adli ile geldi
    /// </summary>
    ByPolice = 3,
    /// <summary>
    /// Kendi başına geldi
    /// </summary>
    SelfArrival = 4,
    /// <summary>
    /// Diğer hastaneden transfer
    /// </summary>
    TransferFromOtherHospital = 5
}

/// <summary>
/// Triaj kategorisi enum
/// Ne: Triaj kategorilerini tanımlayan enum.
/// Neden: Öncelik kategorilerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Triaj kategorilerinin standartlaştırılması.
/// </summary>
public enum TriageCategory
{
    /// <summary>
    /// Kırmızı - Resüsitasyon (Hayati tehlike)
    /// </summary>
    Red = 1,
    /// <summary>
    /// Turuncu - Acil (Ciddi)
    /// </summary>
    Orange = 2,
    /// <summary>
    /// Sarı - Acil değil ama beklemeli
    /// </summary>
    Yellow = 3,
    /// <summary>
    /// Yeşil - Az acil
    /// </summary>
    Green = 4,
    /// <summary>
    /// Mavi - Acil değil
    /// </summary>
    Blue = 5
}

/// <summary>
/// Acil servis durumu enum
/// Ne: Acil servis durumlarını tanımlayan enum.
/// Neden: İş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum EmergencyStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Waiting = 1,
    /// <summary>
    /// Triaj yapılıyor
    /// </summary>
    InTriage = 2,
    /// <summary>
    /// Muayenede
    /// </summary>
    InExamination = 3,
    /// <summary>
    /// Müdahale ediliyor
    /// </summary>
    UnderIntervention = 4,
    /// <summary>
    /// Gözlemde
    /// </summary>
    UnderObservation = 5,
    /// <summary>
    /// Ameliyathane
    /// </summary>
    InSurgery = 6,
    /// <summary>
    /// Taburcu edildi
    /// </summary>
    Discharged = 7,
    /// <summary>
    /// Yatış yapıldı
    /// </summary>
    Admitted = 8,
    /// <summary>
    /// Exitus
    /// </summary>
    Deceased = 9,
    /// <summary>
    /// Sevk edildi
    /// </summary>
    Referred = 10
}

/// <summary>
/// Çıkış tipi enum
/// Ne: Çıkış tiplerini tanımlayan enum.
/// Neden: Çıkış türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Acil servis.
/// Amacı: Çıkış tiplerinin standartlaştırılması.
/// </summary>
public enum DischargeType
{
    /// <summary>
    /// Taburcu edildi
    /// </summary>
    Discharged = 1,
    /// <summary>
    /// Yatış yapıldı
    /// </summary>
    Admitted = 2,
    /// <summary>
    /// Sevk edildi
    /// </summary>
    Referred = 3,
    /// <summary>
    /// İsteği üzerine çıkış
    /// </summary>
    LeftAgainstMedicalAdvice = 4,
    /// <summary>
    /// Exitus
    /// </summary>
    Deceased = 5,
    /// <summary>
    /// Gözlem altında
    /// </summary>
    UnderObservation = 6
}
