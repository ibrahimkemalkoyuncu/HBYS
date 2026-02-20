using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Appointment;

/// <summary>
/// Randevu Entity Sınıfı
/// Ne: HBYS sisteminin randevu yönetimi için temel varlık sınıfıdır.
/// Neden: Hasta randevularının takibi, yönetilmesi ve raporlanması için gereklidir.
/// Özelliği: Doktor bazlı, bölüm bazlı ve hasta bazlı randevu yönetimi.
/// Kim Kullanacak: Hasta kabul, Poliklinik, Randevu yönetimi, SMS/ bildirim sistemleri.
/// Amacı: Hastane randevu sisteminin merkezi yönetimi.
/// </summary>
public class Appointment : BaseEntity
{
    /// <summary>
    /// Randevu numarası
    /// Ne: Otomatik oluşturulan benzersiz randevu numarası.
/// Neden: Randevu takibi için gereklidir.
/// Kim Kullanacak: Tüm randevu işlemleri.
/// Amacı: Randevu tekil tanımlama.
/// </summary>
    public string AppointmentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hasta ID
    /// Ne: Randevu alan hastanın tekil tanımlayıcısı.
/// Neden: Hasta ile randevu arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Hasta kabul, Poliklinik.
/// Amacı: Hasta randevu ilişkisi.
/// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Hasta adı (denormalize)
    /// Ne: Hastanın adı (görüntüleme için).
    /// Neden: Sorgu performansı için gereklidir.
/// Kim Kullanacak: Randevu listesi, Raporlama.
/// Amacı: Hızlı görüntüleme.
/// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Hasta TC Kimlik (denormalize)
    /// Ne: Hastanın TC Kimlik numarası.
/// Neden: Kimlik doğrulama için gereklidir.
/// Kim Kullanacak: Hasta kabul, Kimlik kontrolü.
/// Amacı: Hasta kimlik bilgisi.
/// </summary>
    public string PatientTckn { get; set; } = string.Empty;

    /// <summary>
    /// Doktor ID
    /// Ne: Randevu alan doktorun tekil tanımlayıcısı.
/// Neden: Doktor ile randevu arasında ilişki kurmak için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Doktor randevu ilişkisi.
/// </summary>
    public Guid DoctorId { get; set; }

    /// <summary>
    /// Doktor adı (denormalize)
    /// Ne: Doktorun adı soyadı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Randevu listesi, Hasta bilgilendirme.
/// Amacı: Doktor bilgisi görüntüleme.
/// </summary>
    public string DoctorName { get; set; } = string.Empty;

    /// <summary>
    /// Bölüm ID
    /// Ne: Randevunun alındığı bölüm.
/// Neden: Bölüm bazlı randevu takibi için gereklidir.
/// Kim Kullanacak: Poliklinik, Raporlama.
/// Amacı: Bölüm randevu ilişkisi.
/// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Bölüm adı (denormalize)
    /// Ne: Bölümün adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Randevu listesi.
/// Amacı: Bölüm bilgisi görüntüleme.
/// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Randevu tarihi ve saati
    /// Ne: Randevunun gerçekleşeceği tarih ve saat.
/// Neden: Randevu planlaması için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Randevu zamanı belirleme.
/// </summary>
    public DateTime AppointmentDateTime { get; set; }

    /// <summary>
    /// Randevu bitiş saati
    /// Ne: Randevunun tahmini bitiş saati.
/// Neden: Salon/doktor zaman çizelgesi için gereklidir.
/// Kim Kullanacak: Poliklinik, Oda yönetimi.
/// Amacı: Randevu süresi takibi.
/// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Randevu durumu
    /// Ne: Randevunun mevcut durumu.
/// Neden: Randevu iş akışı yönetimi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Randevu durumu takibi.
/// </summary>
    public AppointmentStatus Status { get; set; }

    /// <summary>
    /// Randevu tipi
    /// Ne: Randevunun türü (muayene, kontrol, işlem).
    /// Neden: Randevu türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Poliklinik, Faturalama.
/// Amacı: Randevu tipi yönetimi.
/// </summary>
    public AppointmentType Type { get; set; }

    /// <summary>
    /// Şikayet/yakınma
    /// Ne: Hastanın şikayeti.
/// Neden: Anamnez için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Hasta şikayet bilgisi.
/// </summary>
    public string? Complaint { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Randevu ile ilgili ek notlar.
/// Neden: Özel durumlar için gereklidir.
/// Kim Kullanacak: Poliklinik, Doktor paneli.
/// Amacı: Ek bilgi saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// Ne: Randevunun sisteme kaydedildiği zaman.
/// Neden: İzleme ve raporlama için gereklidir.
/// Kim Kullanacak: Raporlama, Denetim.
/// Amacı: Randevu zamanı takibi.
/// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Onaylandı mı?
    /// Ne: Randevunun onaylanıp onaylanmadığı.
/// Neden: Kesinleşmiş randevular için gereklidir.
/// Kim Kullanacak: Hasta bilgilendirme.
/// Amacı: Randevu onay takibi.
/// </summary>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Onay tarihi
    /// Ne: Randevunun onaylandığı zaman.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Onay zamanı.
/// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// İptal edildi mi?
    /// Ne: Randevunun iptal edilip edilmediği.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: İptal durumu takibi.
/// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// İptal sebebi
    /// Ne: Randevunun iptal edilme sebebi.
/// Neden: İptal raporları için gereklidir.
/// Kim Kullanacak: Raporlama, Yönetim.
/// Amacı: İptal bilgisi.
/// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// İptal tarihi
    /// Ne: Randevunun iptal edildiği zaman.
/// Neden: İptal takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: İptal zamanı.
/// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Geldi mi?
    /// Ne: Hastanın randevuya gelip gelmediği.
/// Neden: Devamsızlık takibi için gereklidir.
/// Kim Kullanacak: Poliklinik, Raporlama.
/// Amacı: Gelme durumu takibi.
/// </summary>
    public bool DidCome { get; set; }

    /// <summary>
    /// Geliş saati
    /// Ne: Hastanın hastaneye geldiği zaman.
/// Neden: Gecikme takibi için gereklidir.
/// Kim Kullanacak: Poliklinik.
/// Amacı: Geliş zamanı.
/// </summary>
    public DateTime? ArrivalTime { get; set; }

    /// <summary>
    /// Sıra numarası
    /// Ne: Gün içindeki randevu sırası.
/// Neden: Sıra yönetimi için gereklidir.
/// Kim Kullanacak: Poliklinik, Hasta bilgilendirme.
/// Amacı: Sıra takibi.
/// </summary>
    public int? QueueNumber { get; set; }
}

/// <summary>
/// Randevu durumu enum
/// Ne: Randevu durumlarını tanımlayan enum.
/// Neden: Randevu iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Tüm randevu işlemleri.
/// Amacı: Randevu durumlarının standartlaştırılması.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>
    /// Beklemede - Randevu oluşturuldu, henüz onaylanmadı
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Onaylandı - Randevu kesinleşti
    /// </summary>
    Confirmed = 2,
    /// <summary>
    /// Tamamlandı - Randevu gerçekleşti
    /// </summary>
    Completed = 3,
    /// <summary>
    /// İptal edildi - Randevu iptal oldu
    /// </summary>
    Cancelled = 4,
    /// <summary>
    /// Gelmedi - Hasta randevuya gelmedi
    /// </summary>
    NoShow = 5,
    /// <summary>
    /// Beklemede - Hasta beklemede
    /// </summary>
    Waiting = 6,
    /// <summary>
    /// İşlemde - Muayene/tedavi sürüyor
    /// </summary>
    InProgress = 7
}

/// <summary>
/// Randevu tipi enum
/// Ne: Randevu tiplerini tanımlayan enum.
/// Neden: Randevu türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Poliklinik, Faturalama.
/// Amacı: Randevu tiplerinin standartlaştırılması.
/// </summary>
public enum AppointmentType
{
    /// <summary>
    /// İlk muayene
    /// </summary>
    FirstExam = 1,
    /// <summary>
    /// Kontrol muayenesi
    /// </summary>
    CheckUp = 2,
    /// <summary>
    /// Tedavi işlemi
    /// </summary>
    Treatment = 3,
    /// <summary>
    /// Tetkik/ laboratuvar
    /// </summary>
    LabTest = 4,
    /// <summary>
    /// Görüntüleme/radyoloji
    /// </summary>
    Radiology = 5,
    /// <summary>
    /// Aşı
    /// </summary>
    Vaccine = 6,
    /// <summary>
    /// Check-up paketi
    /// </summary>
    CheckupPackage = 7,
    /// <summary>
    /// Diğer
    /// </summary>
    Other = 99
}
