using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Patient;

/// <summary>
/// Hasta Entity Sınıfı
/// Ne: HBYS sisteminin temel hasta varlık sınıfıdır.
/// Neden: Hasta verilerinin kalıcı olarak saklanması ve yönetilmesi için gereklidir.
/// Özelliği: TC Kimlik numarası ile tekil hasta takibi, çoklu adres desteği, kan grubu bilgisi.
/// Kim Kullanacak: Hasta kabul, Poliklinik, Yatuvar, Eatan hasta, Laborczane, Faturalama modülleri.
/// Amacı: Hastane bilgi yönetim sisteminin merkezi hasta veri deposu.
/// </summary>
public class Patient : BaseEntity
{
    /// <summary>
    /// Hasta adı
    /// Ne: Hastanın adı.
/// Neden: Hasta kimlik bilgisi olarak gereklidir.
/// Kim Kullanacak: Tüm hasta işlemleri.
/// Amacı: Hasta tanımlama.
/// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Hasta soyadı
    /// Ne: Hastanın soyadı.
/// Neden: Hasta kimlik bilgisi olarak gereklidir.
/// Kim Kullanacak: Tüm hasta işlemleri.
/// Amacı: Hasta tanımlama.
/// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Tam adı (hesaplanan)
    /// Ne: FirstName ve LastName birleştirilmiş tam ad.
/// Neden: Görüntüleme ve raporlama kolaylığı için gereklidir.
/// Kim Kullanacak: Raporlama, Görüntüleme.
/// Amacı: Tam hasta adının tek alanda gösterilmesi.
/// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// TC Kimlik Numarası
    /// Ne: Türkiye Cumhuriyeti Kimlik Numarası.
/// Neden: Türkiye'de hasta tekil tanımlama için zorunludur.
/// Kim Kullanacak: Hasta kabul, SGK entegrasyonu, Raporlama.
/// Amacı: Ulusal kimlik numarası ile hasta takibi.
/// </summary>
    public string Tckn { get; set; } = string.Empty;

    /// <summary>
    /// Cinsiyet
    /// Ne: Hastanın cinsiyeti.
/// Neden: Tıbbi işlemler ve raporlama için gereklidir.
/// Kim Kullanacak: Tıbbi modüller, Raporlama.
/// Amacı: Cinsiyet bazlı işlem takibi.
/// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Doğum tarihi
    /// Ne: Hastanın doğum tarihi.
/// Neden: Yaş hesaplama, tıbbi öykü için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Yaş ve doğum bilgisi takibi.
/// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Telefon numarası
    /// Ne: Hastanın iletişim telefonu.
/// Neden: İletişim ve randevu hatırlatma için gereklidir.
/// Kim Kullanacak: Randevu, İletişim.
/// Amacı: Hasta ile iletişim.
/// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// E-posta adresi
    /// Ne: Hastanın e-posta adresi.
/// Neden: Dijital iletişim için gereklidir.
/// Kim Kullanacak: Bildirim, Raporlama.
/// Amacı: Elektronik iletişim.
/// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Adres
    /// Ne: Hastanın ikamet adresi.
/// Neden: İletişim ve fatura gönderimi için gereklidir.
/// Kim Kullanacak: Faturalama, İletişim.
/// Amacı: Fiziksel adres bilgisi.
/// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Kan grubu
    /// Ne: Hastanın kan grubu.
/// Neden: Acil müdahale ve kan bankası işlemleri için kritiktir.
/// Kim Kullanacak: Acil servis, Yatan hasta, Ameliyathane.
/// Amacı: Kan grubu bilgisi takibi.
/// </summary>
    public BloodType? BloodType { get; set; }

    /// <summary>
    /// Alerjiler
    /// Ne: Hastanın bilinen alerjileri.
/// Neden: İlaç ve tedavi güvenliği için gereklidir.
/// Kim Kullanacak: Eczane, Reçete, Tedavi.
/// Amacı: Alerji uyarı sistemi.
/// </summary>
    public string? Allergies { get; set; }

    /// <summary>
    /// Kronik hastalıklar
    /// Ne: Hastanın kronik hastalıkları.
/// Neden: Tedavi planlaması için gereklidir.
/// Kim Kullanacak: Tedavi, Raporlama.
/// Amacı: Kronik hastalık takibi.
/// </summary>
    public string? ChronicDiseases { get; set; }

    /// <summary>
    /// Baba adı
    /// Ne: Hastanın babasının adı.
/// Neden: Türkiye'de kimlik doğrulama için gereklidir.
/// Kim Kullanacak: Hasta kabul, Kimlik doğrulama.
/// Amacı: Kimlik bilgisi tamamlama.
/// </summary>
    public string? FatherName { get; set; }

    /// <summary>
    /// Anne adı
    /// Ne: Hastanın annesinin adı.
/// Neden: Türkiye'de kimlik doğrulama için gereklidir.
/// Kim Kullanacak: Hasta kabul, Kimlik doğrulama.
/// Amacı: Kimlik bilgisi tamamlama.
/// </summary>
    public string? MotherName { get; set; }

    /// <summary>
    /// Doğum yeri
    /// Ne: Hastanın doğduğu il/ilçe.
/// Neden: Kimlik bilgisi olarak gereklidir.
/// Kim Kullanacak: Hasta kabul, Raporlama.
/// Amacı: Doğum yeri bilgisi.
/// </summary>
    public string? BirthPlace { get; set; }

    /// <summary>
    /// Cilt adresi
    /// Ne: Hastanın ikamet adresi ile aynı veya farklı cilt adresi.
/// Neden: Fatura ve bildirim gönderimi için gereklidir.
/// Kim Kullanacak: Faturalama, İletişim.
/// Amacı: İkinci adres bilgisi.
/// </summary>
    public string? MailingAddress { get; set; }

    /// <summary>
    /// Uyruk
    /// Ne: Hastanın uyruk bilgisi.
/// Neden: Yabancı hasta takibi için gereklidir.
/// Kim Kullanacak: Hasta kabul, Faturalama.
/// Amacı: Uyruk bazlı işlem takibi.
/// </summary>
    public string? Nationality { get; set; }

    /// <summary>
    /// Pasaport numarası
    /// Ne: Yabancı hastalar için pasaport numarası.
/// Neden: Yabancı hasta kimlik doğrulama için gereklidir.
/// Kim Kullanacak: Hasta kabul, Yabancı hasta işlemleri.
/// Amacı: Pasaport bilgisi takibi.
/// </summary>
    public string? PassportNumber { get; set; }

    /// <summary>
    /// Sigorta türü
    /// Ne: Hastanın sağlık sigortası türü.
/// Neden: Faturalama ve SGK işlemleri için gereklidir.
/// Kim Kullanacak: Faturalama, Hasta kabul.
/// Amacı: Sigorta bilgisi takibi.
/// </summary>
    public InsuranceType? InsuranceType { get; set; }

    /// <summary>
    /// Sigorta numarası
    /// Ne: Hastanın sigorta numarası.
/// Neden: SGK ve özel sigorta doğrulama için gereklidir.
/// Kim Kullanacak: Faturalama, SGK entegrasyonu.
/// Amacı: Sigorta numarası takibi.
/// </summary>
    public string? InsuranceNumber { get; set; }

    /// <summary>
    /// Acil durum kişisi
    /// Ne: Acil durumda ulaşılacak kişi.
/// Neden: Acil durum iletişimi için gereklidir.
/// Kim Kullanacak: Acil servis, Yatan hasta.
/// Amacı: Acil durum bilgisi.
/// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Acil durum telefonu
    /// Ne: Acil durum kişisinin telefonu.
/// Neden: Acil durum iletişimi için gereklidir.
/// Kim Kullanacak: Acil servis, Yatan hasta.
/// Amacı: Acil durum iletişim bilgisi.
/// </summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Hasta ile ilgili özel notlar.
/// Neden: Ek bilgi ve hatırlatmalar için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Hasta ile ilgili notlar.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Cinsiyet enum
/// Ne: Cinsiyet türlerini tanımlayan enum.
/// Neden: Hasta cinsiyet bilgisinin tip güvenli olarak yönetilmesi için gereklidir.
/// Kim Kullanacak: Tüm hasta işlemleri.
/// Amacı: Cinsiyet bilgisinin standartlaştırılması.
/// </summary>
public enum Gender
{
    /// <summary>
    /// Erkek
    /// </summary>
    Male = 1,
    /// <summary>
    /// Kadın
    /// </summary>
    Female = 2,
    /// <summary>
    /// Belirtilmemiş
    /// </summary>
    Other = 3
}

/// <summary>
/// Kan grubu enum
/// Ne: Kan grubu türlerini tanımlayan enum.
/// Neden: Kan grubu bilgisinin tip güvenli olarak yönetilmesi için gereklidir.
/// Kim Kullanacak: Acil servis, Kan bankası, Yatan hasta.
/// Amacı: Kan grubu bilgisinin standartlaştırılması.
/// </summary>
public enum BloodType
{
    APozitif = 1,
    ANegatif = 2,
    BPozitif = 3,
    BNegatif = 4,
    ABPozitif = 5,
    ABNegatif = 6,
    OPozitif = 7,
    ONegatif = 8,
    Bilinmiyor = 9
}

/// <summary>
/// Sigorta türü enum
/// Ne: Sigorta türlerini tanımlayan enum.
/// Neden: Sigorta bilgisinin tip güvenli olarak yönetilmesi için gereklidir.
/// Kim Kullanacak: Faturalama, Hasta kabul.
/// Amacı: Sigorta türlerinin standartlaştırılması.
/// </summary>
public enum InsuranceType
{
    /// <summary>
    /// Sosyal Güvenlik Kurumu
    /// </summary>
    SGK = 1,
    /// <summary>
    /// Emekli Sandığı
    /// </summary>
    EmekliSandigi = 2,
    /// <summary>
    /// Bağ-Kur
    /// </summary>
    BagKur = 3,
    /// <summary>
    /// Özel Sağlık Sigortası
    /// </summary>
    OzelSigorta = 4,
    /// <summary>
    /// Tam Ücret (Sigortasız)
    /// </summary>
    Ucretli = 5
}
