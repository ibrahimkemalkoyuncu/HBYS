namespace HBYS.Domain.Entities.Patient;

/// <summary>
/// Hasta entity sınıfı
/// Ne: Hasta bilgilerini temsil eden domain entity sınıfıdır.
/// Neden: Hasta kayıtları ve bilgileri için gereklidir.
/// Özelliği: TC Kimlik No, ad, soyad, doğum tarihi, cinsiyet, adres, telefon vb. özelliklere sahiptir.
/// Kim Kullanacak: PatientService, PatientRepository, AppointmentService.
/// Amacı: Hasta verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Patient : BaseTenantEntity
{
    /// <summary>
    /// TC Kimlik Numarası (Uniqe)
    /// </summary>
    public string TCKN { get; private set; } = string.Empty;
    
    /// <summary>
    /// Ad
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Soyad
    /// </summary>
    public string LastName { get; private set; } = string.Empty;
    
    /// <summary>
    /// Doğum Tarihi
    /// </summary>
    public DateTime BirthDate { get; private set; }
    
    /// <summary>
    /// Cinsiyet (0: Erkek, 1: Kadın, 2: Diğer)
    /// </summary>
    public int Gender { get; private set; }
    
    /// <summary>
    /// E-posta
    /// </summary>
    public string? Email { get; private set; }
    
    /// <summary>
    /// Telefon
    /// </summary>
    public string Phone { get; private set; } = string.Empty;
    
    /// <summary>
    /// Adres
    /// </summary>
    public string? Address { get; private set; }
    
    /// <summary>
    /// Şehir
    /// </summary>
    public string? City { get; private set; }
    
    /// <summary>
    /// İlçe
    /// </summary>
    public string? District { get; private set; }
    
    /// <summary>
    /// Posta Kodu
    /// </summary>
    public string? PostalCode { get; private set; }
    
    /// <summary>
    /// Kan Grubu
    /// </summary>
    public string? BloodType { get; private set; }
    
    /// <summary>
    /// Rh Faktörü (Pozitif/Negatif)
    /// </summary>
    public string? RhFactor { get; private set; }
    
    /// <summary>
    /// Alerjiler (JSON string olarak saklanabilir)
    /// </summary>
    public string? Allergies { get; private set; }
    
    /// <summary>
    /// Kronik Hastalıklar
    /// </summary>
    public string? ChronicDiseases { get; private set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    private Patient() { }

    /// <summary>
    /// Factory method - Yeni hasta oluştur
    /// </summary>
    public static Patient Create(
        string tckn,
        string firstName,
        string lastName,
        DateTime birthDate,
        int gender,
        string phone,
        Guid tenantId,
        string? email = null,
        string? address = null,
        string? city = null)
    {
        if (string.IsNullOrWhiteSpace(tckn))
            throw new ArgumentException("TCKN is required", nameof(tckn));
            
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required", nameof(lastName));

        return new Patient
        {
            TCKN = tckn,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Gender = gender,
            Phone = phone,
            Email = email,
            Address = address,
            City = city,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Tam adı getir
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Yaşı hesapla
    /// </summary>
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - 
        (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

    /// <summary>
    /// Bilgileri güncelle
    /// </summary>
    public void Update(
        string firstName,
        string lastName,
        string phone,
        string? email = null,
        string? address = null,
        string? city = null,
        string? bloodType = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        Address = address;
        City = city;
        BloodType = bloodType;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deaktive et (soft delete)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Aktive et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Hasta yakını bilgileri
/// </summary>
public class PatientContact : BaseTenantEntity
{
    public Guid PatientId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string Relationship { get; private set; } = string.Empty; // Anne, Baba, Eş, vb.
    public bool IsEmergencyContact { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private PatientContact() { }

    public static PatientContact Create(
        Guid patientId,
        string firstName,
        string lastName,
        string phone,
        string relationship,
        bool isEmergencyContact,
        Guid tenantId)
    {
        return new PatientContact
        {
            PatientId = patientId,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Relationship = relationship,
            IsEmergencyContact = isEmergencyContact,
            TenantId = tenantId
        };
    }
}

/// <summary>
/// Hasta sigorta bilgileri
/// </summary>
public class PatientInsurance : BaseTenantEntity
{
    public Guid PatientId { get; private set; }
    public string InsuranceProvider { get; private set; } = string.Empty; // SGK, Özel Sigorta
    public string PolicyNumber { get; private set; } = string.Empty;
    public DateTime? ExpiryDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private PatientInsurance() { }

    public static PatientInsurance Create(
        Guid patientId,
        string insuranceProvider,
        string policyNumber,
        DateTime? expiryDate,
        Guid tenantId)
    {
        return new PatientInsurance
        {
            PatientId = patientId,
            InsuranceProvider = insuranceProvider,
            PolicyNumber = policyNumber,
            ExpiryDate = expiryDate,
            TenantId = tenantId
        };
    }
}
