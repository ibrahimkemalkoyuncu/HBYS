using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Laboratory;

/// <summary>
/// Laboratuvar Test Talebi Entity Sınıfı
/// Ne: Hastadan alınan laboratuvar test taleplerini temsil eden varlık sınıfıdır.
/// Neden: Laboratuvar işlemlerinin merkezi yönetimi için gereklidir.
/// Özelliği: Test durumu, sonuç takibi, numune yönetimi.
/// Kim Kullanacak: Laboratuvar, Poliklinik, Yatan Hasta, Acil Servis.
/// Amacı: Laboratuvar süreçlerinin merkezi yönetimi.
/// </summary>
public class LabRequest : BaseEntity
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
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Hasta ilişkisi.
/// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Hasta adı (denormalize)
    /// Ne: Hastanın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Hasta bilgisi.
/// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Hasta TC Kimlik (denormalize)
    /// Ne: Hastanın TC Kimlik numarası.
/// Neden: Yasal takip için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Hasta kimlik.
/// </summary>
    public string? PatientIdentityNumber { get; set; }

    /// <summary>
    /// İstek yapan birim
    /// Ne: Testi isteyen birim (Poliklinik, Yatan Hasta, Acil).
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar, Birimler.
/// Amacı: İstek birimi takibi.
/// </summary>
    public string RequestingUnit { get; set; } = string.Empty;

    /// <summary>
    /// İstek yapan doktor
    /// Ne: Testi isteyen doktor.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Doktor takibi.
/// </summary>
    public Guid RequestedById { get; set; }

    /// <summary>
    /// İstek yapan doktor adı
    /// Ne: Testi isteyen doktorun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Doktor bilgisi.
/// </summary>
    public string RequestedByName { get; set; } = string.Empty;

    /// <summary>
    /// Talep tarihi
    /// Ne: Talebin oluşturulduğu tarih.
/// Neden: Talep zamanı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Talep zamanı takibi.
/// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Öncelik
    /// Ne: Talebin öncelik seviyesi.
/// Neden: Önceliklendirme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Öncelik belirleme.
/// </summary>
    public LabPriority Priority { get; set; }

    /// <summary>
    /// Durum
    /// Ne: Talebin mevcut durumu.
/// Neden: İş akışı için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Durum takibi.
/// </summary>
    public LabRequestStatus Status { get; set; }

    /// <summary>
    /// Numune alındı mı?
    /// Ne: Numune alınıp alınmadığı.
/// Neden: Numune takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Numune durumu.
/// </summary>
    public bool SampleCollected { get; set; }

    /// <summary>
    /// Numune alma tarihi
    /// Ne: Numune alındığı tarih.
/// Neden: Numune zamanı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Numune zamanı.
/// </summary>
    public DateTime? SampleCollectionDate { get; set; }

    /// <summary>
    /// Numune alan kişi
    /// Ne: Numune alan laborant.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Laborant takibi.
/// </summary>
    public Guid? SampleCollectedById { get; set; }

    /// <summary>
    /// Numune türü
    /// Ne: Alınan numune türü (kan, idrar, gaita).
/// Neden: Numune türü takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Numune türü.
/// </summary>
    public string? SampleType { get; set; }

    /// <summary>
    /// Açılış numarası (Barcode)
    /// Ne: Numune için otomatik oluşturulan barkod.
/// Neden: Numune takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Barkod takibi.
/// </summary>
    public string? BarcodeNumber { get; set; }

    /// <summary>
    /// Sonuçlanma tarihi
    /// Ne: Sonuçların raporlandığı tarih.
/// Neden: Sonuç zamanı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç zamanı.
/// </summary>
    public DateTime? ResultDate { get; set; }

    /// <summary>
    /// Sonuçları onaylayan
    /// Ne: Sonuçları onaylayan laborant/doktor.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Onaylayan takibi.
/// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Onay tarihi
    /// Ne: Sonuçların onaylandığı tarih.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Onay zamanı.
/// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Talep ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Laboratuvar öncelik enum
/// Ne: Öncelik seviyelerini tanımlayan enum.
/// Neden: Önceliklerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Önceliklerin standartlaştırılması.
/// </summary>
public enum LabPriority
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
    /// Çok acil
    /// </summary>
    Stat = 3
}

/// <summary>
/// Laboratuvar talep durumu enum
/// Ne: Talep durumlarını tanımlayan enum.
/// Neden: Talep iş akışı durumlarının tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum LabRequestStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Numune alındı
    /// </summary>
    SampleCollected = 2,
    /// <summary>
    /// Laboratuvarda
    /// </summary>
    InLab = 3,
    /// <summary>
    /// Sonuçlandı
    /// </summary>
    Resulted = 4,
    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 5,
    /// <summary>
    /// Raporlandı
    /// </summary>
    Reported = 6,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 7
}
