using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Radiology;

/// <summary>
/// Radyoloji Tetkiği Entity Sınıfı
/// Ne: Radyoloji tetkiklerinin tanımlarını temsil eden varlık sınıfıdır.
/// Neden: Radyoloji kataloğu yönetimi için gereklidir.
/// Özelliği: Tetkik kategorileri, PACS modallity, süre, fiyat.
/// Kim Kullanacak: Radyoloji, Yönetim.
/// Amacı: Radyoloji kataloğunun merkezi yönetimi.
/// </summary>
public class RadiologyTest : BaseEntity
{
    /// <summary>
    /// Tetkik kodu
    /// Ne: Tetkiğin benzersiz kodu.
/// Neden: Tetkik tanımlama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik tekil tanımlama.
/// </summary>
    public string TestCode { get; set; } = string.Empty;

    /// <summary>
    /// Tetkik adı
    /// Ne: Tetkiğin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Tetkik adı.
/// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Tetkik adı (İngilizce)
    /// Ne: Tetkiğin İngilizce adı.
/// Neden: Uluslararası standartlar için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: İngilizce ad.
/// </summary>
    public string? TestNameEn { get; set; }

    /// <summary>
    /// Kategori
    /// Ne: Tetkiğin kategorisi (X-RAY, MR, BT, Ultrason).
/// Neden: Kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kategori belirleme.
/// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Alt kategori
    /// Ne: Tetkiğin alt kategorisi.
/// Neden: Alt kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Alt kategori belirleme.
/// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// Modality
    /// Ne: PACS modallity kodu (CR, CT, MR, US, XA).
/// Neden: PACS entegrasyonu için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Modality belirleme.
/// </summary>
    public string Modality { get; set; } = string.Empty;

    /// <summary>
    /// SOP Class UID
    /// Ne: DICOM SOP Class UID.
/// Neden: DICOM entegrasyonu için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: SOP Class belirleme.
/// </summary>
    public string? SopClassUid { get; set; }

    /// <summary>
    /// Süre (dakika)
    /// Ne: Tetkiğin tahmini süresi.
/// Neden: Randevu planlaması için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Süre belirleme.
/// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Hazırlık
    /// Ne: Hasta hazırlık bilgisi.
/// Neden: Hasta bilgilendirme için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Hazırlık bilgisi.
/// </summary>
    public string? Preparation { get; set; }

    /// <summary>
    /// Kontrast gerekli mi?
    /// Ne: Kontrast madde kullanılacak mı?
    /// Neden: Kontrast takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kontrast durumu.
/// </summary>
    public bool RequiresContrast { get; set; }

    /// <summary>
    /// Kontrast hazırlığı
    /// Ne: Kontrast öncesi hazırlık bilgisi.
/// Neden: Kontrast hazırlığı için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Kontrast hazırlığı.
/// </summary>
    public string? ContrastPreparation { get; set; }

    /// <summary>
    /// Aç kalma süresi
    /// Ne: Tetkik öncesi aç kalınacak süre.
/// Neden: Hazırlık için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Aç kalma süresi.
/// </summary>
    public string? FastingInfo { get; set; }

    /// <summary>
    /// Fiyat
    /// Ne: Tetkiğin fiyatı.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Radyoloji, Muhasebe.
/// Amacı: Fiyat belirleme.
/// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// SGK fiyatı
    /// Ne: SGK'li fiyat.
/// Neden: SGK faturalama için gereklidir.
/// Kim Kullanacak: Radyoloji, Muhasebe.
/// Amacı: SGK fiyatı.
/// </summary>
    public decimal? SgkPrice { get; set; }

    /// <summary>
    /// Cihaz gereksinimi
    /// Ne: Hangi cihazda yapılacağı.
/// Neden: Cihaz planlaması için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Cihaz belirleme.
/// </summary>
    public string? DeviceRequirement { get; set; }

    /// <summary>
    /// Radyolog gereksinimi
    /// Ne: Radyolog zorunlu mu?
    /// Neden: Radyolog atama için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Radyolog gereksinimi.
/// </summary>
    public bool RequiresRadiologist { get; set; } = true;

    /// <summary>
    /// Aktif mi?
    /// Ne: Tetkiğin aktif olup olmadığı.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Aktif durumu.
/// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Dış merkeze gönderilecek mi?
    /// Ne: Dış merkez kullanımı.
/// Neden: Dış merkez takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Dış merkez durumu.
/// </summary>
    public bool IsExternal { get; set; }

    /// <summary>
    /// Dış merkez
    /// Ne: Dış merkez adı.
/// Neden: Dış merkez takibi için gereklidir.
/// Kim Kullanacak: Radyoloji.
/// Amacı: Dış merkez bilgisi.
/// </summary>
    public string? ExternalCenter { get; set; }
}
