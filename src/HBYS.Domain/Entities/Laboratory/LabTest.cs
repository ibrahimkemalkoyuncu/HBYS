using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Laboratory;

/// <summary>
/// Laboratuvar Testi Entity Sınıfı
/// Ne: Laboratuvar testlerinin tanımlarını temsil eden varlık sınıfıdır.
/// Neden: Test kataloğu yönetimi için gereklidir.
/// Özelliği: Test kategorileri, referans aralıkları, birimler.
/// Kim Kullanacak: Laboratuvar, Yönetim.
/// Amacı: Test kataloğunun merkezi yönetimi.
/// </summary>
public class LabTest : BaseEntity
{
    /// <summary>
    /// Test kodu
    /// Ne: Testin benzersiz kodu.
/// Neden: Test tanımlama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test tekil tanımlama.
/// </summary>
    public string TestCode { get; set; } = string.Empty;

    /// <summary>
    /// Test adı
    /// Ne: Testin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test adı.
/// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Test adı (İngilizce)
    /// Ne: Testin İngilizce adı.
/// Neden: Uluslararası raporlama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: İngilizce ad.
/// </summary>
    public string? TestNameEn { get; set; }

    /// <summary>
    /// Kategori
    /// Ne: Testin kategorisi.
/// Neden: Kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Kategori belirleme.
/// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Alt kategori
    /// Ne: Testin alt kategorisi.
/// Neden: Alt kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Alt kategori belirleme.
/// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// Birim
    /// Ne: Sonuç birimi.
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Birim belirleme.
/// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Referans alt değer
    /// Ne: Referans aralığı alt değeri.
/// Neden: Referans aralığı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Referans belirleme.
/// </summary>
    public decimal? ReferenceLow { get; set; }

    /// <summary>
    /// Referans üst değer
    /// Ne: Referans aralığı üst değeri.
/// Neden: Referans aralığı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Referans belirleme.
/// </summary>
    public decimal? ReferenceHigh { get; set; }

    /// <summary>
    /// Referans metin
    /// Ne: Referans aralığı metni.
/// Neden: Metinsel referans için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Referans metni.
/// </summary>
    public string? ReferenceText { get; set; }

    /// <summary>
    /// Ondalık basamak
    /// Ne: Sonuç ondalık basamağı.
/// Neden: Formatlama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Format belirleme.
/// </summary>
    public int DecimalPlaces { get; set; } = 2;

    /// <summary>
    /// Metod
    /// Ne: Test metodu.
/// Neden: Metod takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Metod belirleme.
/// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Cihaz
    /// Ne: Kullanılan cihaz.
/// Neden: Cihaz takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Cihaz belirleme.
/// </summary>
    public string? Device { get; set; }

    /// <summary>
    /// Fiyat
    /// Ne: Testin fiyatı.
/// Neden: Faturalama için gereklidir.
/// Kim Kullanacak: Laboratuvar, Muhasebe.
/// Amacı: Fiyat belirleme.
/// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// SGK fiyatı
    /// Ne: SGK'li fiyat.
/// Neden: SGK faturalama için gereklidir.
/// Kim Kullanacak: Laboratuvar, Muhasebe.
/// Amacı: SGK fiyatı.
/// </summary>
    public decimal? SgkPrice { get; set; }

    /// <summary>
    /// Hazırlık
    /// Ne: Hasta hazırlık bilgisi.
/// Neden: Hasta bilgilendirme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Hazırlık bilgisi.
/// </summary>
    public string? Preparation { get; set; }

    /// <summary>
    /// Aktif mi?
    /// Ne: Testin aktif olup olmadığı.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Aktif durumu.
/// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Dış laboratuvara gönderilecek mi?
    /// Ne: Dış laboratuvar kullanımı.
/// Neden: Dış laboratuvar takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Dış laboratuvar durumu.
/// </summary>
    public bool IsExternal { get; set; }

    /// <summary>
    /// Dış laboratuvar
    /// Ne: Dış laboratuvar adı.
/// Neden: Dış laboratuvar takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Dış laboratuvar bilgisi.
/// </summary>
    public string? ExternalLab { get; set; }
}

/// <summary>
/// Laboratuvar Test Sonucu Entity Sınıfı
/// Ne: Laboratuvar test sonuçlarını temsil eden varlık sınıfıdır.
/// Neden: Test sonuçlarının kaydedilmesi için gereklidir.
/// Özelliği: Sonuç girişi, limit kontrolü, delta check.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test sonuçlarının merkezi yönetimi.
/// </summary>
public class LabResult : BaseEntity
{
    /// <summary>
    /// Laboratuvar talep ID
    /// Ne: İlişkili laboratuvar talebi.
/// Neden: Talep ilişkisi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Talep ilişkisi.
/// </summary>
    public Guid LabRequestId { get; set; }

    /// <summary>
    /// Test ID
    /// Ne: Yapılan test.
/// Neden: Test takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test takibi.
/// </summary>
    public Guid LabTestId { get; set; }

    /// <summary>
    /// Test kodu
    /// Ne: Testin kodu.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test kodu.
/// </summary>
    public string TestCode { get; set; } = string.Empty;

    /// <summary>
    /// Test adı
    /// Ne: Testin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Test adı.
/// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Sonuç değeri
    /// Ne: Test sonucu.
/// Neden: Sonuç takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç belirleme.
/// </summary>
    public string? ResultValue { get; set; }

    /// <summary>
    /// Sonuç sayısal değeri
    /// Ne: Sonucun sayısal hali.
/// Neden: Hesaplama için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sayısal sonuç.
/// </summary>
    public decimal? ResultNumeric { get; set; }

    /// <summary>
    /// Birim
    /// Ne: Sonuç birimi.
/// Neden: Birim takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Birim belirleme.
/// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Referans aralığı
    /// Ne: Referans aralığı metni.
/// Neden: Karşılaştırma için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Referans bilgisi.
/// </summary>
    public string? ReferenceRange { get; set; }

    /// <summary>
    /// Normal mi?
    /// Ne: Sonuç normal aralıkta mı?
    /// Neden: Durum belirleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Normal/patalojik durumu.
/// </summary>
    public bool IsNormal { get; set; } = true;

    /// <summary>
    /// Durum (Normal/Yüksek/Düşük)
    /// Ne: Sonuç durumu.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Durum belirleme.
/// </summary>
    public ResultFlag Flag { get; set; } = ResultFlag.Normal;

    /// <summary>
    /// Sonuç giriş tarihi
    /// Ne: Sonuç girildiği tarih.
/// Neden: Sonuç zamanı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Sonuç zamanı.
/// </summary>
    public DateTime ResultDate { get; set; }

    /// <summary>
    /// Sonuç giren
    /// Ne: Sonucu giren laborant.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Laborant takibi.
/// </summary>
    public Guid EnteredById { get; set; }

    /// <summary>
    /// Sonuç giren adı
    /// Ne: Sonucu giren laborantın adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Laborant bilgisi.
/// </summary>
    public string EnteredByName { get; set; } = string.Empty;

    /// <summary>
    /// Onaylandı mı?
    /// Ne: Sonuç onaylandı mı?
    /// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Onay durumu.
/// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Onay tarihi
    /// Ne: Sonuç onaylandığı tarih.
/// Neden: Onay zamanı için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Onay zamanı.
/// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// Onaylayan
    /// Ne: Sonucu onaylayan laborant/doktor.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Onaylayan takibi.
/// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Yorum
    /// Ne: Sonuç yorumu.
/// Neden: Yorum ekleme için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Yorum saklama.
/// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Sonuç bayrağı enum
/// Ne: Sonuç durumlarını tanımlayan enum.
/// Neden: Durumların tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Laboratuvar.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum ResultFlag
{
    /// <summary>
    /// Normal
    /// </summary>
    Normal = 0,
    /// <summary>
    /// Düşük
    /// </summary>
    Low = 1,
    /// <summary>
    /// Yüksek
    /// </summary>
    High = 2,
    /// <summary>
    /// Kritik düşük
    /// </summary>
    CriticallyLow = 3,
    /// <summary>
    /// Kritik yüksek
    /// </summary>
    CriticallyHigh = 4
}
