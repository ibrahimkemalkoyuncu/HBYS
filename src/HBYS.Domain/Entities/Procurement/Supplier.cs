using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Procurement;

/// <summary>
/// Tedarikçi Entity Sınıfı
/// Ne: Hastane tedarikçilerini temsil eden varlık sınıfıdır.
/// Neden: Tedarikçi yönetimi için gereklidir.
/// Özelliği: Tedarikçi bilgileri, kategoriler, performans.
/// Kim Kullanacak: Satın alma, Muhasebe, Yönetim.
/// Amacı: Tedarikçi kayıtlarının merkezi yönetimi.
/// </summary>
public class Supplier : BaseEntity
{
    /// <summary>
    /// Tedarikçi kodu
    /// Ne: Otomatik oluşturulan benzersiz tedarikçi kodu.
/// Neden: Tedarikçi tanımlama için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Tedarikçi tekil tanımlama.
/// </summary>
    public string SupplierCode { get; set; } = string.Empty;

    /// <summary>
    /// Tedarikçi adı
    /// Ne: Tedarikçinin resmi adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Tedarikçi adı.
/// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Vergi kimlik numarası
    /// Ne: Tedarikçinin VKN'si.
/// Neden: Yasal takip için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Vergi kimlik.
/// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Vergi dairesi
    /// Ne: Kayıtlı vergi dairesi.
/// Neden: Yasal takip için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Vergi dairesi.
/// </summary>
    public string? TaxOffice { get; set; }

    /// <summary>
    /// Telefon
    /// Ne: Tedarikçinin telefonu.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Telefon bilgisi.
/// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// E-posta
    /// Ne: Tedarikçinin e-posta adresi.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: E-posta bilgisi.
/// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Adres
    /// Ne: Tedarikçinin adresi.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Adres bilgisi.
/// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// İl
    /// Ne: İl bilgisi.
/// Neden: Adres takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İl bilgisi.
/// </summary>
    public string? City { get; set; }

    /// <summary>
    /// İlçe
    /// Ne: İlçe bilgisi.
/// Neden: Adres takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: İlçe bilgisi.
/// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Web sitesi
    /// Ne: Tedarikçinin web sitesi.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Web sitesi.
/// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Yetkili kişi
    /// Ne: Tedarikçi yetkilisinin adı.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Yetkili bilgisi.
/// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Yetkili telefon
    /// Ne: Yetkili kişinin telefonu.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Yetkili telefon.
/// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Yetkili e-posta
    /// Ne: Yetkili kişinin e-postası.
/// Neden: İletişim için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Yetkili e-posta.
/// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Tedarikçi kategorileri
    /// Ne: Tedarikçinin sağladığı ürün kategorileri.
/// Neden: Kategori bazlı filtreleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kategori belirleme.
/// </summary>
    public string? Categories { get; set; }

    /// <summary>
    /// Aktif mi?
    /// Ne: Tedarikçinin aktif olup olmadığı.
/// Neden: Durum takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Aktif durumu.
/// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Onaylı tedarikçi mi?
    /// Ne: Onaylı tedarikçi olup olmadığı.
/// Neden: Onay takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Onay durumu.
/// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Performans puanı
    /// Ne: Tedarikçinin performans puanı.
/// Neden: Performans değerlendirmesi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Performans takibi.
/// </summary>
    public decimal PerformanceScore { get; set; }

    /// <summary>
    /// Ödeme vadeleri
    /// Ne: Anlaşmalı ödeme vadeleri.
/// Neden: Ödeme planlaması için gereklidir.
/// Kim Kullanacak: Satın alma, Muhasebe.
/// Amacı: Vade bilgisi.
/// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Kargo bilgileri
    /// Ne: Kargo ve teslimat bilgileri.
/// Neden: Teslimat takibi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kargo bilgisi.
/// </summary>
    public string? ShippingInfo { get; set; }

    /// <summary>
    /// Minimum sipariş tutarı
    /// Ne: Minimum sipariş tutarı.
/// Neden: Sipariş kısıtlaması için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Minimum tutar.
/// </summary>
    public decimal? MinimumOrderAmount { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Tedarikçi ile ilgili notlar.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Lisans bitiş tarihi
    /// Ne: Tedarikçi sözleşme/lisans bitiş tarihi.
/// Neden: Sözleşme takibi için gereklidir.
/// Kim Kullanacak: Satın alma, Yönetim.
/// Amacı: Sözleşme takibi.
/// </summary>
    public DateTime? ContractEndDate { get; set; }
}
