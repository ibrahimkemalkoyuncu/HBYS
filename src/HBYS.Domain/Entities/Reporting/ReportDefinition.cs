using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Reporting;

/// <summary>
/// Rapor Tanımı Entity Sınıfı
/// Ne: Sistemdeki rapor tanımlarını temsil eden varlık sınıfıdır.
/// Neden: Rapor kataloğu yönetimi için gereklidir.
/// Özelliği: Rapor kategorileri, parametreler, şablonlar.
/// Kim Kullanacak: Yönetim, Raporlama, Tüm departmanlar.
/// Amacı: Rapor süreçlerinin merkezi yönetimi.
/// </summary>
public class ReportDefinition : BaseEntity
{
    /// <summary>
    /// Rapor kodu
    /// Ne: Raporun benzersiz kodu.
/// Neden: Rapor tanımlama için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Rapor tekil tanımlama.
/// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// Rapor adı
    /// Ne: Raporun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Rapor adı.
/// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// Rapor açıklaması
    /// Ne: Raporun açıklaması.
/// Neden: Bilgi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Açıklama.
/// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kategori
    /// Ne: Raporun kategorisi.
/// Neden: Kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Kategori belirleme.
/// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Alt kategori
    /// Ne: Raporun alt kategorisi.
/// Neden: Alt kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Alt kategori belirleme.
/// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// Rapor tipi
    /// Ne: Raporun türü (PDF, Excel, Word).
/// Neden: Rapor tipi takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Tip belirleme.
/// </summary>
    public ReportType Type { get; set; }

    /// <summary>
    /// Rapor sınıfı
    /// Ne: Raporu üreten sınıfın adı.
/// Neden: Rapor üretimi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Sınıf belirleme.
/// </summary>
    public string ReportClass { get; set; } = string.Empty;

    /// <summary>
    /// SQL sorgusu
    /// Ne: Rapor verilerini çeken sorgu.
/// Neden: Veri çekme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Sorgu saklama.
/// </summary>
    public string? SqlQuery { get; set; }

    /// <summary>
    /// Parametreler
    /// Ne: Rapor parametreleri (JSON formatında).
/// Neden: Parametre takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Parametre saklama.
/// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// Şablon dosyası
    /// Ne: Rapor şablon dosyası yolu.
/// Neden: Şablon takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Şablon belirleme.
/// </summary>
    public string? TemplatePath { get; set; }

    /// <summary>
    /// Aktif mi?
    /// Ne: Raporun aktif olup olmadığı.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Aktif durumu.
/// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Yetkilendirme gerekli mi?
    /// Ne: Rapor için yetki gerekli mi?
    /// Neden: Yetki takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Yetki durumu.
/// </summary>
    public bool RequiresPermission { get; set; } = true;

    /// <summary>
    /// Sıralama
    /// Ne: Rapor sıralaması.
/// Neden: Sıralama için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Sıralama belirleme.
/// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// Rapor Çıktısı Entity Sınıfı
/// Ne: Oluşturulan rapor çıktılarını temsil eden varlık sınıfıdır.
/// Neden: Rapor geçmişi yönetimi için gereklidir.
/// Özelliği: Rapor oluşturma, zamanlama, dağıtım.
/// Kim Kullanacak: Yönetim, Raporlama.
/// Amacı: Rapor çıktılarının merkezi yönetimi.
/// </summary>
public class ReportOutput : BaseEntity
{
    /// <summary>
    /// Rapor tanımı ID
    /// Ne: İlişkili rapor tanımı.
/// Neden: Rapor ilişkisi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Rapor ilişkisi.
/// </summary>
    public Guid ReportDefinitionId { get; set; }

    /// <summary>
    /// Rapor adı
    /// Ne: Raporun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Rapor adı.
/// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// Çıktı numarası
    /// Ne: Benzersiz çıktı numarası.
/// Neden: Çıktı takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Çıktı tekil tanımlama.
/// </summary>
    public string OutputNumber { get; set; } = string.Empty;

    /// <summary>
    /// Çıktı tipi
    /// Ne: Çıktının türü (PDF, Excel).
/// Neden: Tip takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Tip belirleme.
/// </summary>
    public ReportType OutputType { get; set; }

    /// <summary>
    /// Çıktı dosya yolu
    /// Ne: Çıktı dosyasının yolu.
/// Neden: Dosya takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Dosya belirleme.
/// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Dosya adı
    /// Ne: Çıktı dosyasının adı.
/// Neden: Dosya takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Dosya adı.
/// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Dosya boyutu
    /// Ne: Çıktı dosyasının boyutu.
/// Neden: Boyut takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Boyut belirleme.
/// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Parametreler
    /// Ne: Rapor çalıştırma parametreleri.
/// Neden: Parametre takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Parametre saklama.
/// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// Çalıştırma başlangıç zamanı
    /// Ne: Raporun çalışmaya başladığı zaman.
/// Neden: Zaman takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Başlangıç zamanı.
/// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Çalıştırma bitiş zamanı
    /// Ne: Raporun çalışmayı bitirdiği zaman.
/// Neden: Zaman takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Bitiş zamanı.
/// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Durum
    /// Ne: Rapor çalışma durumu.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Durum belirleme.
/// </summary>
    public ReportOutputStatus Status { get; set; }

    /// <summary>
    /// Hata mesajı
    /// Ne: Çalışma hatası varsa.
/// Neden: Hata takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Hata bilgisi.
/// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Çalıştıran
    /// Ne: Raporu çalıştıran kullanıcı.
/// Neden: Sorumluluk takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Kullanıcı takibi.
/// </summary>
    public Guid RunById { get; set; }

    /// <summary>
    /// Çalıştıran adı
    /// Ne: Raporu çalıştıran kullanıcının adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Kullanıcı bilgisi.
/// </summary>
    public string RunByName { get; set; } = string.Empty;

    /// <summary>
    /// Zamanlandı mı?
    /// Ne: Rapor zamanlandı mı?
    /// Neden: Zamanlama takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Zamanlama durumu.
/// </summary>
    public bool IsScheduled { get; set; }

    /// <summary>
    /// Zamanlama ID
    /// Ne: Zamanlama tanımı.
/// Neden: Zamanlama ilişkisi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Zamanlama takibi.
/// </summary>
    public Guid? ScheduleId { get; set; }

    /// <summary>
    /// Dağıtıldı mı?
    /// Ne: Rapor dağıtıldı mı?
    /// Neden: Dağıtım takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Dağıtım durumu.
/// </summary>
    public bool IsDistributed { get; set; }

    /// <summary>
    /// Dağıtım zamanı
    /// Ne: Raporun dağıtıldığı zaman.
/// Neden: Dağıtım takibi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Dağıtım zamanı.
/// </summary>
    public DateTime? DistributedDate { get; set; }
}

/// <summary>
/// Rapor tipi enum
/// Ne: Rapor tiplerini tanımlayan enum.
/// Neden: Tiplerin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Tiplerin standartlaştırılması.
/// </summary>
public enum ReportType
{
    /// <summary>
    /// PDF
    /// </summary>
    PDF = 1,
    /// <summary>
    /// Excel
    /// </summary>
    Excel = 2,
    /// <summary>
    /// Word
    /// </summary>
    Word = 3,
    /// <summary>
    /// CSV
    /// </summary>
    CSV = 4
}

/// <summary>
/// Rapor çıktı durumu enum
/// Ne: Rapor çıktı durumlarını tanımlayan enum.
/// Neden: Durumların tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Durumların standartlaştırılması.
/// </summary>
public enum ReportOutputStatus
{
    /// <summary>
    /// Beklemede
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Çalışıyor
    /// </summary>
    Running = 2,
    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 3,
    /// <summary>
    /// Başarısız
    /// </summary>
    Failed = 4,
    /// <summary>
    /// İptal edildi
    /// </summary>
    Cancelled = 5
}
