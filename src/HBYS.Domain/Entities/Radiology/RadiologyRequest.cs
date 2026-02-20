using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Radiology;

/// <summary>
/// Radyoloji Talebi Entity Sınıfı
/// Ne: Hastadan alınan radyoloji test taleplerini temsil eden varlık sınıfıdır.
/// Neden: Radyoloji işlemlerinin merkezi yönetimi için gereklidir.
/// Özelliği: Görüntüleme, sonuç takibi, PACS entegrasyonu.
/// Kim Kullanacak: Radyoloji, Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Radyoloji süreçlerinin merkezi yönetimi.
/// </summary>
public class RadiologyRequest : BaseEntity
{
    /// <summary>
    /// Talep numarası
    /// Ne: Otomatik oluşturulan benzersiz talep numarası.
/// Neden: Talep takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Talep tekil tanımlama.
/// </summary>
    public string RequestNumber { get; set; } = string.Empty;

    /// <summary>
    /// Hasta ID
    /// Ne: İlişkili hasta.
/// Neden: Hasta takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Hasta ilişkisi.
/// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Hasta adı (denormalize)
    /// Ne: Hastanın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Hasta bilgisi.
/// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Hasta TC Kimlik (denormalize)
    /// Ne: Hastanın TC Kimlik numarası.
/// Neden: Yasal takip için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Hasta kimlik.
/// </summary>
    public string? PatientIdentityNumber { get; set; }

    /// <summary>
    /// Hasta doğum tarihi
    /// Ne: Hastanın doğum tarihi.
/// Neden: Yaş hesaplama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Yaş bilgisi.
/// </summary>
    public DateTime? PatientBirthDate { get; set; }

    /// <summary>
    /// Cinsiyet
    /// Ne: Hastanın cinsiyeti.
/// Neden: Tanı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Cinsiyet bilgisi.
/// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// İstek yapan birim
    /// Ne: Testi isteyen birim.
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Radyoloji, Birimler.
/// Amacı: İstek birimi takibi.
/// </summary>
    public string RequestingUnit { get; set; } = string.Empty;

    /// <summary>
    /// İstek yapan doktor
    /// Ne: Testi isteyen doktor.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Doktor takibi.
/// </summary>
    public Guid RequestedById { get; set; }

    /// <summary>
    /// İstek yapan doktor adı
    /// Ne: Testi isteyen doktorun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Doktor bilgisi.
/// </summary>
    public string RequestedByName { get; set; } = string.Empty;

    /// <summary>
    /// Talep tarihi
    /// Ne: Talebin oluşturulduğu tarih.
/// Neden: Talep zamanı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Talep zamanı takibi.
/// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// İstenen tetkik
    /// Ne: İstenen radyoloji tetkiği.
/// Neden: Tetkik takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik belirleme.
/// </summary>
    public Guid RadiologyTestId { get; set; }

    /// <summary>
    /// Tetkik adı
    /// Ne: İstenen tetkiğin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik bilgisi.
/// </summary>
    public string RadiologyTestName { get; set; } = string.Empty;

    /// <summary>
    /// Klinik bilgi
    /// Ne: Klinik şikayet ve önbilgi.
/// Neden: Tanı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Klinik bilgi.
/// </summary>
    public string? ClinicalInformation { get; set; }

    /// <summary>
    /// Önceki tetkik
    /// Ne: Önceki tetkik bilgisi.
/// Neden: Karşılaştırma için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Önceki tetkik.
/// </summary>
    public string? PreviousStudy { get; set; }

    /// <summary>
    /// Öncelik
    /// Ne: Talebin öncelik seviyesi.
/// Neden: Önceliklendirme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Öncelik belirleme.
/// </summary>
    public RadiologyPriority Priority { get; set; }

    /// <summary>
    /// Durum
    /// Ne: Talebin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Durum takibi.
/// </summary>
    public RadiologyRequestStatus Status { get; set; }

    /// <summary>
    /// Çekim tarihi
    /// Ne: Radyoloji çekim tarihi.
/// Neden: Çekim zamanı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Çekim zamanı.
/// </summary>
    public DateTime? StudyDate { get; set; }

    /// <summary>
    /// Çekim yapan teknisyen
    /// Ne: Çekimi yapan radyoloji teknisyeni.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Teknisyen takibi.
/// </summary>
    public Guid? PerformedById { get; set; }

    /// <summary>
    /// Radyolog
    /// Ne: Görüntüyü yorumlayan radyolog.
/// Neden: Yorum takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Radyolog takibi.
/// </summary>
    public Guid? RadiologistId { get; set; }

    /// <summary>
    /// Radyolog adı
    /// Ne: Görüntüyü yorumlayan radyologun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Radyolog bilgisi.
/// </summary>
    public string? RadiologistName { get; set; }

    /// <summary>
    /// Accession Number
    /// Ne: PACS sistemindeki benzersiz erişim numarası.
/// Neden: PACS entegrasyonu için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: PACS numarası.
/// </summary>
    public string? AccessionNumber { get; set; }

    /// <summary>
    /// Study Instance UID
    /// Ne: DICOM Study Instance UID.
/// Neden: DICOM entegrasyonu için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: DICOM UID.
/// </summary>
    public string? StudyInstanceUid { get; set; }

    /// <summary>
    /// Rapor tarihi
    /// Ne: Raporun yazıldığı tarih.
/// Neden: Rapor zamanı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor zamanı.
/// </summary>
    public DateTime? ReportDate { get; set; }

    /// <summary>
    /// Rapor
    /// Ne: Radyoloji raporu.
/// Neden: Rapor takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor saklama.
/// </summary>
    public string? Report { get; set; }

    /// <summary>
    /// Rapor onaylandı mı?
    /// Ne: Rapor onaylandı mı?
    /// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Rapor onay durumu.
/// </summary>
    public bool IsReportApproved { get; set; }

    /// <summary>
    /// Kritik bulgu
    /// Ne: Kritik bir bulgu var mı?
    /// Neden: Acil bildirim için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kritik bulgu takibi.
/// </summary>
    public bool HasCriticalFinding { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Talep ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Radyoloji öncelik enum
/// Ne: Öncelik seviyelerini tanımlayan enum.
/// Neden: Önceliklerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Önceliklerin standartlaştırılması.
/// </summary>
public enum RadiologyPriority
{
    /// <summary>
    /// Rutin
    /// </summary>
    Routine = 1,
    /// <summary>
    /// Acil
    /// </summary>
    Urgent = 2,
    /// <summary>
    /// Stat (Hemen)
    /// </summary>
    Stat = 3
}

/// <summary>
/// Radyoloji talep durumu enum
/// Ne: Talep durumlarını tanımlayan enum.
/// Neden: Talep iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum RadiologyRequestStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Randevulu
    /// </summary>
    Scheduled = 2,
    /// <summary>
    /// Çekildi
    /// </summary>
    Performed = 3,
    /// <summary>
    /// Radyolojide
    /// </summary>
    InRadiology = 4,
    /// <summary>
    /// Raporlandı
    /// </summary>
    Reported = 5,
    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 6,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 7
}
