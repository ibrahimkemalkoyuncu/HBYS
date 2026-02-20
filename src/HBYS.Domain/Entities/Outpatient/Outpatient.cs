using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Outpatient;

/// <summary>
/// Poliklinik Muayene Entity Sınıfı
/// Ne: HBYS sisteminin poliklinik muayene yönetimi için temel varlık sınıfıdır.
/// Neden: Hasta muayenelerinin kaydı, takibi ve tıbbi dokümantasyon için gereklidir.
/// Özelliği: Anamnez, muayene bulguları, tanı ve tedavi planı yönetimi.
/// Kim Kullanacak: Poliklinik, Doktor paneli, Eczane, Laboratuvar, Radyoloji.
/// Amacı: Hasta muayene sürecinin merkezi yönetimi.
/// </summary>
public class Outpatient : BaseEntity
{
    /// <summary>
    /// Muayene numarası
    /// Ne: Otomatik oluşturulan benzersiz muayene numarası.
/// Neden: Muayene takibi için gereklidir.
/// Kim Kullanacak: Tüm muayene işlemleri.
/// Amacı: Muayene tekil tanımlama.
/// </summary>
    public string ExaminationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hasta ID
    /// Ne: Muayene edilen hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile muayene arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Poliklinik, Eczane.
/// Amacı: Hasta muayene ilişkisi.
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
    /// Randevu ID
    /// Ne: Bu muayenenin ait olduğu randevu.
/// Neden: Randevu-muayene ilişkisi için gereklidir.
/// Kim Kullanacak: Poliklinik, Randevu.
/// Amacı: Randevu takibi.
/// </summary>
    public Guid? AppointmentId { get; set; }

    /// <summary>
    /// Doktor ID
    /// Ne: Muayene eden doktor.
/// Neden: Doktor-muayene ilişkisi için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Doktor takibi.
/// </summary>
    public Guid DoctorId { get; set; }

    /// <summary>
    /// Doktor adı (denormalize)
    /// Ne: Doktorun adı soyadı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Doktor bilgisi.
/// </summary>
    public string DoctorName { get; set; } = string.Empty;

    /// <summary>
    /// Bölüm ID
    /// Ne: Muayenenin yapıldığı bölüm.
/// Neden: Bölüm-muayene ilişkisi için gereklidir.
/// Kim Kullanacak: Poliklinik, Raporlama.
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
    /// Muayene tarihi ve saati
    /// Ne: Muayenenin yapıldığı tarih ve saat.
/// Neden: Muayene zamanı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Muayene zamanı takibi.
/// </summary>
    public DateTime ExaminationDateTime { get; set; }

    /// <summary>
    /// Muayene durumu
    /// Ne: Muayenenin mevcut durumu.
/// Neden: İş akışı yönetimi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Muayene durumu takibi.
/// </summary>
    public ExaminationStatus Status { get; set; }

    /// <summary>
    /// Şikayet (Anamnez)
    /// Ne: Hastanın şikayeti ve öyküsü.
/// Neden: Tıbbi dokümantasyon için gereklidir.
/// Kim Kullanacak: Doktor paneli, Eczane.
/// Amacı: Hasta öyküsü.
/// </summary>
    public string? Complaint { get; set; }

    /// <summary>
    /// Özgeçmiş
    /// Ne: Hastanın tıbbi özgeçmişi.
/// Neden: Tıbbi geçmiş için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Tıbbi geçmiş bilgisi.
/// </summary>
    public string? MedicalHistory { get; set; }

    /// <summary>
    /// Soygeçmiş
    /// Ne: Hastanın aile tıbbi geçmişi.
/// Neden: Kalıtsal riskler için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Aile geçmişi bilgisi.
/// </summary>
    public string? FamilyHistory { get; set; }

    /// <summary>
    /// Fizik muayene bulguları
    /// Ne: Fizik muayene sırasında tespit edilen bulgular.
/// Neden: Tıbbi dokümantasyon için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Muayene bulguları.
/// </summary>
    public string? PhysicalExamination { get; set; }

    /// <summary>
    /// Şikayet süresi
    /// Ne: Şikayetin ne kadar süredir devam ettiği.
/// Neden: Tanı için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Süre bilgisi.
/// </summary>
    public string? DurationOfComplaint { get; set; }

    /// <summary>
    /// Vital bulgular - Tansiyon
    /// Ne: Kan basıncı değeri.
/// Neden: Temel vital bulgu için gereklidir.
/// Kim Kullanacak: Doktor paneli, Hemşire.
/// Amacı: Tansiyon takibi.
/// </summary>
    public string? BloodPressure { get; set; }

    /// <summary>
    /// Vital bulgular - Nabız
    /// Ne: Nabız değeri.
/// Neden: Temel vital bulgu için gereklidir.
/// Kim Kullanacak: Doktor paneli, Hemşire.
/// Amacı: Nabız takibi.
/// </summary>
    public int? Pulse { get; set; }

    /// <summary>
    /// Vital bulgular - Ateş
    /// Ne: Vücut ısısı.
/// Neden: Temel vital bulgu için gereklidir.
/// Kim Kullanacak: Doktor paneli, Hemşire.
/// Amacı: Ateş takibi.
/// </summary>
    public decimal? Temperature { get; set; }

    /// <summary>
    /// Vital bulgular - Solunum sayısı
    /// Ne: Solunum sayısı.
/// Neden: Temel vital bulgu için gereklidir.
/// Kim Kullanacak: Doktor paneli, Hemşire.
/// Amacı: Solunum takibi.
/// </summary>
    public int? RespiratoryRate { get; set; }

    /// <summary>
    /// Oksijen saturasyonu
    /// Ne: SpO2 değeri.
/// Neden: Solunum değerlendirmesi için gereklidir.
/// Kim Kullanacak: Doktor paneli, Acil servis.
/// Amacı: Oksijen durumu takibi.
/// </summary>
    public decimal? OxygenSaturation { get; set; }

    /// <summary>
    /// Tanılar
    /// Ne: Konulan tanılar (ICD kodları).
    /// Neden: Tıbbi dokümantasyon ve raporlama için gereklidir.
/// Kim Kullanacak: Doktor paneli, Faturalama, Raporlama.
/// Amacı: Tanı bilgisi.
/// </summary>
    public string? Diagnoses { get; set; }

    /// <summary>
    /// Ana tanı
    /// Ne: Birincil tanı.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Doktor paneli, Faturalama.
/// Amacı: Birincil tanı belirleme.
/// </summary>
    public string? MainDiagnosis { get; set; }

    /// <summary>
    /// Tedavi planı
    /// Ne: Önerilen tedavi planı.
/// Neden: Tedavi takibi için gereklidir.
/// Kim Kullanacak: Doktor paneli, Eczane.
/// Amacı: Tedavi planı.
/// </summary>
    public string? TreatmentPlan { get; set; }

    /// <summary>
    /// İlaçlar
    /// Ne: Reçete edilen ilaçlar.
/// Neden: Eczane işlemleri için gereklidir.
/// Kim Kullanacak: Doktor paneli, Eczane.
/// Amacı: İlaç reçeteleme.
/// </summary>
    public string? Medications { get; set; }

    /// <summary>
    /// Tetkikler
    /// Ne: İstenen tetkikler (laboratuvar, radyoloji).
    /// Neden: Tetkik işlemleri için gereklidir.
/// Kim Kullanacak: Doktor paneli, Laboratuvar, Radyoloji.
/// Amacı: Tetkik isteme.
/// </summary>
    public string? LabTests { get; set; }

    /// <summary>
    /// İstem numarası
    /// Ne: Tetkik istem numarası.
/// Neden: Tetkik takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar, Radyoloji.
/// Amacı: İstem takibi.
/// </summary>
    public string? RequestNumber { get; set; }

    /// <summary>
    /// Kontrol tarihi
    /// Ne: Önerilen kontrol tarihi.
/// Neden: Takip için gereklidir.
/// Kim Kullanacak: Doktor paneli, Randevu.
/// Amacı: Kontrol planlaması.
/// </summary>
    public DateTime? FollowUpDate { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Ek notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Doktor paneli.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Muayene tamamlandı mı?
    /// Ne: Muayenenin tamamlanıp tamamlanmadığı.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Poliklinik.
/// Amacı: Tamamlanma durumu.
/// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Tamamlanma tarihi
    /// Ne: Muayenenin tamamlandığı zaman.
/// Neden: Takip için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Tamamlanma zamanı.
/// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Muayene durumu enum
/// Ne: Muayene durumlarını tanımlayan enum.
/// Neden: Muayene iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Muayene durumlarının standartlaştırılması.
/// </summary>
public enum ExaminationStatus
{
    /// <summary>
    /// Başladı - Muayene başladı
    /// </summary>
    Started = 1,
    /// <summary>
    /// Devam ediyor - Muayene sürüyor
    /// </summary>
    InProgress = 2,
    /// <summary>
    /// Tamamlandı - Muayene bitti
    /// </summary>
    Completed = 3,
    /// <summary>
    /// İptal edildi - Muayene iptal oldu
    /// </summary>
    Cancelled = 4,
    /// <summary>
    /// Beklemede - Sonuç bekleniyor
    /// </summary>
    WaitingForResults = 5
}
