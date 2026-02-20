using HBYS.Domain.Entities;

namespace HBYS.Domain.Entities.Inventory;

/// <summary>
/// Stok/Envanter Entity Sınıfı
/// Ne: HBYS sisteminin stok ve envanter yönetimi için temel varlık sınıfıdır.
/// Neden: Tıbbi sarf malzemeleri, ilaç stoğu ve envanter takibi için gereklidir.
/// Özelliği: Stok giriş/çıkış, minimum stok uyarısı, lot/batch takibi.
/// Kim Kullanacak: Deposu, Eczane, Satın alma, Yönetim.
/// Amacı: Hastane envanter sürecinin merkezi yönetimi.
/// </summary>
public class InventoryItem : BaseEntity
{
    /// <summary>
    /// Stok kodu
    /// Ne: Stok kaleminin benzersiz kodu.
/// Neden: Stok takibi için gereklidir.
/// Kim Kullanacak: Tüm envanter işlemleri.
/// Amacı: Stok tekil tanımlama.
/// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Stok adı
    /// Ne: Stok kaleminin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Stok adı.
/// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Stok tipi
    /// Ne: Stok kaleminin türü.
/// Neden: Stok türü bazlı işlem için gereklidir.
/// Kim Kullanacak: Deposu, Eczane.
/// Amacı: Stok tipi yönetimi.
/// </summary>
    public InventoryType Type { get; set; }

    /// <summary>
    /// Barkod
    /// Ne: Stok kaleminin barkod numarası.
/// Neden: Barkod okuma için gereklidir.
/// Kim Kullanacak: Deposu, Eczane.
/// Amacı: Barkod takibi.
/// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Birim
    /// Ne: Stok kaleminin birimi (adet, kutu, paket).
    /// Neden: Miktar ölçümü için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Birim belirleme.
/// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Kategori ID
    /// Ne: Stok kaleminin kategorisi.
/// Neden: Kategorizasyon için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Kategori takibi.
/// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Kategori adı
    /// Ne: Kategorinin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Raporlama.
/// Amacı: Kategori bilgisi.
/// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Mevcut miktar
    /// Ne: Stokta bulunan miktar.
/// Neden: Stok takibi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Miktar takibi.
/// </summary>
    public decimal CurrentQuantity { get; set; }

    /// <summary>
    /// Minimum stok seviyesi
    /// Ne: Uyarı verilecek minimum miktar.
/// Neden: Kritik stok uyarısı için gereklidir.
/// Kim Kullanacak: Deposu, Yönetim.
/// Amacı: Minimum stok takibi.
/// </summary>
    public decimal MinimumQuantity { get; set; }

    /// <summary>
    /// Maksimum stok seviyesi
    /// Ne: Maksimum depolama miktarı.
/// Neden: Kapasite planlaması için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Maksimum stok takibi.
/// </summary>
    public decimal MaximumQuantity { get; set; }

    /// <summary>
    /// Kritik stok seviyesi
    /// Ne: Acil sipariş verilecek miktar.
/// Neden: Kritik durum uyarısı için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Kritik stok takibi.
/// </summary>
    public decimal CriticalQuantity { get; set; }

    /// <summary>
    /// Birim fiyat
    /// Ne: Birim fiyatı.
/// Neden: Maliyet hesaplaması için gereklidir.
/// Kim Kullanacak: Faturalama, Satın alma.
/// Amacı: Fiyat takibi.
/// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// KDV oranı
    /// Ne: KDV oranı.
/// Neden: Vergi hesaplaması için gereklidir.
/// Kim Kullanacak: Faturalama.
/// Amacı: Vergi hesaplama.
/// </summary>
    public decimal VatRate { get; set; }

    /// <summary>
    /// Son kullanma tarihi kontrolü
    /// Ne: Son kullanma tarihi takip edilecek mi?
    /// Neden: SKT takibi için gereklidir.
/// Kim Kullanacak: Eczane.
/// Amacı: SKT kontrolü.
/// </summary>
    public bool TrackExpiryDate { get; set; }

    /// <summary>
    /// Aktif mi?
    /// Ne: Stok kalemi aktif mi?
    /// Neden: Pasif kalemleri gizlemek için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Aktif durumu.
/// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Depo ID
    /// Ne: Stokun bulunduğu depo.
/// Neden: Depo takibi için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Depo takibi.
/// </summary>
    public Guid? WarehouseId { get; set; }

    /// <summary>
    /// Depo adı
    /// Ne: Deponun adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Depo bilgisi.
/// </summary>
    public string? WarehouseName { get; set; }

    /// <summary>
    /// Raf konumu
    /// Ne: Depodaki raf konumu.
/// Neden: Yer tespiti için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Raf takibi.
/// </summary>
    public string? ShelfLocation { get; set; }

    /// <summary>
    /// Tedarikçi ID
    /// Ne: Varsayılan tedarikçi.
/// Neden: Sipariş için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi takibi.
/// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Tedarikçi adı
    /// Ne: Tedarikçinin adı.
/// Neden: Görüntüleme için gereklidir.
/// Kim Kullanacak: Satın alma.
/// Amacı: Tedarikçi bilgisi.
/// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// Açıklama
    /// Ne: Stok kalemi açıklaması.
/// Neden: Ek bilgi için gereklidir.
/// Kim Kullanacak: Tüm modüller.
/// Amacı: Açıklama.
/// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Notlar
    /// Ne: Ek notlar.
/// Neden: Not tutmak için gereklidir.
/// Kim Kullanacak: Deposu.
/// Amacı: Not saklama.
/// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Stok tipi enum
/// Ne: Stok tiplerini tanımlayan enum.
/// Neden: Stok türlerinin tip güvenli yönetimi için gereklidir.
/// Kim Kullanacak: Deposu, Eczane.
/// Amacı: Stok tiplerinin standartlaştırılması.
/// </summary>
public enum InventoryType
{
    /// <summary>
    /// İlaç
    /// </summary>
    Medication = 1,
    /// <summary>
    /// Tıbbi sarf malzeme
    /// </summary>
    MedicalSupply = 2,
    /// <summary>
    /// Laboratuvar malzemesi
    /// </summary>
    LabSupply = 3,
    /// <summary>
    /// Ameliyathane malzemesi
    /// </summary>
    SurgerySupply = 4,
    /// <summary>
    /// Ofis malzemesi
    /// </summary>
    OfficeSupply = 5,
    /// <summary>
    /// Demirbaş
    /// </summary>
    FixedAsset = 6,
    /// <summary>
    /// Diğer
    /// </summary>
    Other = 99
}
