# HBYS Module Implementation Details
## Specific Entity Classes, Repositories, Event Handlers, Background Jobs, and Deployment Configurations

---

# PART 1: PATIENT MODULE IMPLEMENTATION

## 1.1 Entity Classes with Full Implementation

### Patient.cs
```csharp
/// <summary>
/// Hasta entity sınıfı.
/// Ne: Hastaneye başvuran tüm bireyleri temsil eden aggregate root sınıfıdır.
/// Neden: Hasta bilgilerinin merkezi yönetimi ve tüm hasta işlemlerinin izlenmesi için gereklidir.
/// Özelliği: PatientNumber (unique), TurkishId (encrypted), FirstName, LastName, Gender, DateOfBirth, 
///           BloodType, ContactInfos, Insurances, EmergencyContact, Allergies, ChronicDiseases, Medications,
///           Status, TenantId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt özelliklerine sahiptir.
/// Kim Kullanacak: PatientRepository, CreatePatientCommandHandler, GetPatientByIdQueryHandler, 
///                tüm hasta işlemi yapan modüller.
/// Amaç: Hasta verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Patient : BaseEntity, IAuditTEntity
{
    public string PatientNumber { get; private set; } = string.Empty;
    public string TurkishId { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string? BloodType { get; private set; }
    public string? PhotoUrl { get; private set; }
    public PatientStatus Status { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation Properties
    private readonly List<PatientContact> _contactInfos = new();
    public IReadOnlyCollection<PatientContact> ContactInfos => _contactInfos.AsReadOnly();

    private readonly List<PatientInsurance> _insurances = new();
    public IReadOnlyCollection<PatientInsurance> Insurances => _insurances.AsReadOnly();

    private readonly List<PatientGuardian> _guardians = new();
    public IReadOnlyCollection<PatientGuardian> Guardians => _guardians.AsReadOnly();

    private readonly List<PatientAllergy> _allergies = new();
    public IReadOnlyCollection<PatientAllergy> Allergies => _allergies.AsReadOnly();

    private readonly List<PatientChronicDisease> _chronicDiseases = new();
    public IReadOnlyCollection<PatientChronicDisease> ChronicDiseases => _chronicDiseases.AsReadOnly();

    private readonly List<PatientMedication> _medications = new();
    public IReadOnlyCollection<PatientMedication> Medications => _medications.AsReadOnly();

    private readonly List<PatientMedicalHistory> _medicalHistories = new();
    public IReadOnlyCollection<PatientMedicalHistory> MedicalHistories => _medicalHistories.AsReadOnly();

    // Private constructor for EF Core
    private Patient() { }

    // Factory method
    public static Patient Create(
        string patientNumber,
        string turkishId,
        string firstName,
        string lastName,
        Gender gender,
        DateTime dateOfBirth,
        string? bloodType,
        Guid tenantId,
        Guid createdBy)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            PatientNumber = patientNumber,
            TurkishId = turkishId,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            BloodType = bloodType,
            Status = PatientStatus.Active,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        return patient;
    }

    // Domain methods
    public void UpdateBasicInfo(string firstName, string lastName, Gender gender, DateTime dateOfBirth, string? bloodType)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        BloodType = bloodType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddContactInfo(PatientContact contact)
    {
        if (_contactInfos.Any(c => c.IsPrimary))
            contact.IsPrimary = false;
        
        _contactInfos.Add(contact);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddInsurance(PatientInsurance insurance)
    {
        // Deactivate existing insurances
        foreach (var existing in _insurances.Where(i => i.IsActive))
        {
            existing.Deactivate();
        }
        
        _insurances.Add(insurance);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAllergy(PatientAllergy allergy)
    {
        _allergies.Add(allergy);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddChronicDisease(PatientChronicDisease disease)
    {
        _chronicDiseases.Add(disease);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMedication(PatientMedication medication)
    {
        _medications.Add(medication);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = PatientStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }

    public string FullName => $"{FirstName} {LastName}";
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum PatientStatus
{
    Active = 1,
    Inactive = 2,
    Deceased = 3
}
```

### PatientContact.cs
```csharp
/// <summary>
/// Hasta iletişim bilgisi entity sınıfı.
/// Ne: Hasta iletişim bilgilerini (telefon, e-posta, adres) temsil eden entity sınıfıdır.
/// Neden: Birden fazla iletişim bilgisi desteklemek ve iletişim bilgilerini izole etmek için gereklidir.
/// Özelliği: PatientId, ContactType, Value, IsPrimary, IsVerified, TenantId, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: PatientRepository, Patient aggregate.
/// Amaç: İletişim bilgilerinin domain model olarak yönetilmesi.
/// </summary>
public class PatientContact : BaseEntity
{
    public Guid PatientId { get; private set; }
    public ContactType ContactType { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public bool IsVerified { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation Property
    public Patient Patient { get; private set; } = null!;

    private PatientContact() { }

    public static PatientContact Create(
        Guid patientId,
        ContactType contactType,
        string value,
        bool isPrimary,
        Guid tenantId)
    {
        return new PatientContact
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            ContactType = contactType,
            Value = value,
            IsPrimary = isPrimary,
            IsVerified = false,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Verify()
    {
        IsVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue;
        IsVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum ContactType
{
    Phone = 1,
    Email = 2,
    Address = 3,
    EmergencyPhone = 4
}
```

### PatientAllergy.cs
```csharp
/// <summary>
/// Hasta alerji entity sınıfı.
/// Ne: Hasta alerji bilgilerini temsil eden entity sınıfıdır.
/// Neden: Güvenli tedavi için alerji bilgilerinin kritik öneme sahip olması nedeniyle gereklidir.
/// Özelliği: PatientId, AllergyType, Allergen, Severity, Reaction, OnsetDate, IsActive, TenantId, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: PatientRepository, Patient aggregate, PharmacyModule.
/// Amaç: Alerji bilgilerinin domain model olarak yönetilmesi.
/// </summary>
public class PatientAllergy : BaseEntity
{
    public Guid PatientId { get; private set; }
    public AllergyType AllergyType { get; private set; }
    public string Allergen { get; private set; } = string.Empty;
    public AllergySeverity Severity { get; private set; }
    public string? Reaction { get; private set; }
    public DateTime? OnsetDate { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private PatientAllergy() { }

    public static PatientAllergy Create(
        Guid patientId,
        AllergyType allergyType,
        string allergen,
        AllergySeverity severity,
        string? reaction,
        DateTime? onsetDate,
        Guid tenantId,
        Guid createdBy)
    {
        return new PatientAllergy
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            AllergyType = allergyType,
            Allergen = allergen,
            Severity = severity,
            Reaction = reaction,
            OnsetDate = onsetDate,
            IsActive = true,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string allergen, AllergySeverity severity, string? reaction)
    {
        Allergen = allergen;
        Severity = severity;
        Reaction = reaction;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum AllergyType
{
    Food = 1,
    Drug = 2,
    Environmental = 3,
    Other = 4
}

public enum AllergySeverity
{
    Mild = 1,
    Moderate = 2,
    Severe = 3,
    LifeThreatening = 4
}
```

## 1.2 Repository Implementation

### IPatientRepository.cs
```csharp
/// <summary>
/// Hasta repository arayüzü.
/// Ne: Hasta verilerine erişim için kullanılan repository arayüzüdür.
/// Neden: Veri erişim katmanının soyutlanması için gereklidir.
/// Özelliği: CRUD ve sorgulama metotlarını içerir.
/// Kim Kullanacak: PatientService, CreatePatientCommandHandler, GetPatientByIdQueryHandler.
/// Amaç: Hasta veri erişiminin yönetilmesi.
/// </summary>
public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Patient?> GetByTurkishIdAsync(string encryptedTurkishId, Guid tenantId);
    Task<Patient?> GetByPatientNumberAsync(string patientNumber, Guid tenantId);
    Task<IEnumerable<Patient>> GetByIdsAsync(IEnumerable<Guid> ids, Guid tenantId);
    Task<PaginatedResult<Patient>> SearchAsync(
        Guid tenantId,
        string? searchTerm,
        PatientStatus? status,
        DateTime? dateOfBirthFrom,
        DateTime? dateOfBirthTo,
        int pageNumber,
        int pageSize);
    Task<string> GeneratePatientNumberAsync(Guid tenantId);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### PatientRepository.cs
```csharp
/// <summary>
/// Hasta repository implementasyonu.
/// Ne: EF Core kullanarak hasta verilerine erişim sağlayan repository sınıfıdır.
/// Neden: Veritabanı işlemlerinin merkezi yönetimi için gereklidir.
/// Özelliği: IPatientRepository arayüzünü implement eder. Tenant filtering otomatik uygulanır.
/// Kim Kullanacak: PatientService, Command/Query Handlers.
/// Amaç: Hasta veri erişim implementasyonu.
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly HbysDbContext _context;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public PatientRepository(
        HbysDbContext context,
        ITenantContextAccessor tenantContextAccessor)
    {
        _context = context;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<Patient?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Patients
            .AsNoTracking()
            .Include(p => p.ContactInfos)
            .Include(p => p.Insurances.Where(i => i.IsActive))
            .Include(p => p.Guardians)
            .Include(p => p.Allergies.Where(a => a.IsActive))
            .Include(p => p.ChronicDiseases.Where(c => c.IsActive))
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }

    public async Task<Patient?> GetByTurkishIdAsync(string encryptedTurkishId, Guid tenantId)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.TurkishId == encryptedTurkishId && p.TenantId == tenantId);
    }

    public async Task<Patient?> GetByPatientNumberAsync(string patientNumber, Guid tenantId)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientNumber == patientNumber && p.TenantId == tenantId);
    }

    public async Task<IEnumerable<Patient>> GetByIdsAsync(IEnumerable<Guid> ids, Guid tenantId)
    {
        var idList = ids.ToList();
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && idList.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<PaginatedResult<Patient>> SearchAsync(
        Guid tenantId,
        string? searchTerm,
        PatientStatus? status,
        DateTime? dateOfBirthFrom,
        DateTime? dateOfBirthTo,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Patients
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p => 
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                p.PatientNumber.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (dateOfBirthFrom.HasValue)
        {
            query = query.Where(p => p.DateOfBirth >= dateOfBirthFrom.Value);
        }

        if (dateOfBirthTo.HasValue)
        {
            query = query.Where(p => p.DateOfBirth <= dateOfBirthTo.Value);
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Patient>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<string> GeneratePatientNumberAsync(Guid tenantId)
    {
        var year = DateTime.Now.Year;
        var prefix = $"P{year}";
        
        var lastPatient = await _context.Patients
            .Where(p => p.TenantId == tenantId && p.PatientNumber.StartsWith(prefix))
            .OrderByDescending(p => p.PatientNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastPatient != null)
        {
            var lastNumber = lastPatient.PatientNumber.Replace(prefix, "");
            if (int.TryParse(lastNumber, out var lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D5}";
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);
        
        if (patient != null)
        {
            _context.Patients.Remove(patient);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

## 1.3 Event Handlers

### PatientCreatedEventHandler.cs
```csharp
/// <summary>
/// Hasta oluşturuldu event handler.
/// Ne: PatientCreatedEvent'i işleyen ve yan etkileri gerçekleştiren event handler sınıfıdır.
/// Neden: Hasta oluşturulduktan sonra yapılması gereken işlemleri (bildirim, log vb.) tetiklemek için gereklidir.
/// Özelliği: INotificationHandler<PatientCreatedEvent> implement eder.
/// Kim Kullanacak: MediatR event publishing.
/// Amaç: Event'in işlenmesi.
/// </summary>
public class PatientCreatedEventHandler : INotificationHandler<PatientCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;
    private readonly ILogger<PatientCreatedEventHandler> _logger;

    public PatientCreatedEventHandler(
        INotificationService notificationService,
        IAuditService auditService,
        ILogger<PatientCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(PatientCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "PatientCreatedEvent received for PatientId: {PatientId}, Tenant: {TenantId}",
            notification.PatientId,
            notification.TenantId);

        try
        {
            // Log audit
            await _auditService.LogActionAsync(
                notification.TenantId,
                notification.CreatedBy,
                "PatientCreated",
                "Patient",
                notification.PatientId.ToString(),
                $"Patient created: {notification.FirstName} {notification.LastName}",
                cancellationToken);

            // TODO: Send welcome notification if needed
            // await _notificationService.SendPatientWelcomeAsync(notification.PatientId, notification.TenantId);

            _logger.LogInformation(
                "PatientCreatedEvent processed successfully for PatientId: {PatientId}",
                notification.PatientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing PatientCreatedEvent for PatientId: {PatientId}",
                notification.PatientId);
            // Don't throw - event processing should be idempotent
        }
    }
}
```

### PatientAllergyAddedEventHandler.cs
```csharp
/// <summary>
/// Alerji eklendi event handler.
/// Ne: Alerji eklendiğinde tetiklenen event'i işleyen handler sınıfıdır.
/// Neden: Kritik alerji bilgilerinin hemen yayılması için gereklidir.
/// Özelliği: Severe alerjiler için acil bildirim gönderir.
/// Kim Kullanacak: MediatR.
/// Amaç: Alerji ekleme duyurusu.
/// </summary>
public class PatientAllergyAddedEventHandler : INotificationHandler<AllergyAddedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IPharmacyService _pharmacyService;
    private readonly ILogger<PatientAllergyAddedEventHandler> _logger;

    public PatientAllergyAddedEventHandler(
        INotificationService notificationService,
        IPharmacyService pharmacyService,
        ILogger<PatientAllergyAddedEventHandler> logger)
    {
        _notificationService = notificationService;
        _pharmacyService = pharmacyService;
        _logger = logger;
    }

    public async Task Handle(AllergyAddedEvent notification, CancellationToken cancellationToken)
    {
        // Update pharmacy system with allergy information
        await _pharmacyService.UpdatePatientAllergyStatusAsync(
            notification.PatientId,
            notification.Allergen,
            notification.Severity,
            cancellationToken);

        // If severe allergy, send critical alert
        if (notification.Severity >= AllergySeverity.Severe)
        {
            await _notificationService.SendCriticalAlertAsync(
                notification.TenantId,
                notification.PatientId,
                $"Severe allergy detected: {notification.Allergen}",
                NotificationPriority.Critical,
                cancellationToken);
        }

        _logger.LogInformation(
            "Allergy added event processed for PatientId: {PatientId}, Allergen: {Allergen}",
            notification.PatientId,
            notification.Allergen);
    }
}
```

---

# PART 2: APPOINTMENT MODULE IMPLEMENTATION

## 2.1 Entity Classes

### Appointment.cs
```csharp
/// <summary>
/// Randevu entity sınıfı.
/// Ne: Hasta-hekim randevularını temsil eden aggregate root sınıfıdır.
/// Neden: Hastane operasyonlarının temelini oluşturur ve kaynak planlaması için gereklidir.
/// Özelliği: AppointmentNumber, PatientId, PhysicianId, DepartmentId, AppointmentDate, StartTime, EndTime,
///           AppointmentType, Status, Reason, Notes, Confirmation, Cancellation, TenantId, CreatedBy, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: AppointmentRepository, AppointmentService, SchedulingService.
/// Amaç: Randevu verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Appointment : BaseEntity, IAuditEntity
{
    public string AppointmentNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid PhysicianId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public DateTime AppointmentDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public AppointmentType AppointmentType { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public Guid? ConfirmedBy { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public Guid? CancelledBy { get; private set; }
    public string? CancelledReason { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? NoShowAt { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation Properties
    public Patient Patient { get; private set; } = null!;
    public Employee Physician { get; private set; } = null!;
    public Department Department { get; private set; } = null!;

    private Appointment() { }

    public static Appointment Create(
        string appointmentNumber,
        Guid patientId,
        Guid physicianId,
        Guid departmentId,
        DateTime appointmentDate,
        TimeSpan startTime,
        TimeSpan endTime,
        AppointmentType appointmentType,
        string? reason,
        Guid tenantId,
        Guid createdBy)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            AppointmentNumber = appointmentNumber,
            PatientId = patientId,
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            AppointmentDate = appointmentDate,
            StartTime = startTime,
            EndTime = endTime,
            AppointmentType = appointmentType,
            Status = AppointmentStatus.Scheduled,
            Reason = reason,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Confirm(Guid confirmedBy)
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = AppointmentStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        ConfirmedBy = confirmedBy;
    }

    public void Cancel(Guid cancelledBy, string reason)
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel completed or already cancelled appointments");

        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        CancelledReason = reason;
    }

    public void Reschedule(DateTime newDate, TimeSpan newStartTime, TimeSpan newEndTime)
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot reschedule completed or cancelled appointments");

        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only confirmed or scheduled appointments can be completed");

        Status = AppointmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Cannot mark as no-show");

        Status = AppointmentStatus.NoShow;
        NoShowAt = DateTime.UtcNow;
    }

    public TimeSpan Duration => EndTime - StartTime;
    public bool IsOverdue => AppointmentDate.Date < DateTime.Today && Status == AppointmentStatus.Scheduled;
}

public enum AppointmentType
{
    Checkup = 1,
    FollowUp = 2,
    Procedure = 3,
    Consultation = 4,
    Emergency = 5
}

public enum AppointmentStatus
{
    Scheduled = 1,
    Confirmed = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5,
    Rescheduled = 6
}
```

### AppointmentSlot.cs
```csharp
/// <summary>
/// Randevu slotu entity sınıfı.
/// Ne: Hekimlerin müsait zaman dilimlerini temsil eden entity sınıfıdır.
/// Neden: Randevu planlaması için müsaitlik bilgisi gereklidir.
/// Özelliği: PhysicianId, DepartmentId, DayOfWeek, StartTime, EndTime, SlotDuration, IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: AppointmentRepository, SchedulingService.
/// Amaç: Müsaitlik verilerinin domain model olarak yönetilmesi.
/// </summary>
public class AppointmentSlot : BaseEntity
{
    public Guid PhysicianId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int SlotDuration { get; private set; } // minutes
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AppointmentSlot() { }

    public static AppointmentSlot Create(
        Guid physicianId,
        Guid departmentId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        Guid tenantId,
        Guid createdBy)
    {
        if (slotDuration < 5 || slotDuration > 120)
            throw new ArgumentException("Slot duration must be between 5 and 120 minutes");

        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        return new AppointmentSlot
        {
            Id = Guid.NewGuid(),
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            SlotDuration = slotDuration,
            IsActive = true,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public IEnumerable<TimeSpan> GetAvailableTimes()
    {
        var times = new List<TimeSpan>();
        var current = StartTime;

        while (current.Add(TimeSpan.FromMinutes(SlotDuration)) <= EndTime)
        {
            times.Add(current);
            current = current.Add(TimeSpan.FromMinutes(SlotDuration));
        }

        return times;
    }

    public int GetTotalSlots()
    {
        var duration = EndTime - StartTime;
        return (int)(duration.TotalMinutes / SlotDuration);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(TimeSpan startTime, TimeSpan endTime, int slotDuration)
    {
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
    }
}
```

## 2.2 Scheduling Service

### ISchedulingService.cs
```csharp
/// <summary>
/// Randevu planlama servisi arayüzü.
/// Ne: Randevu planlama ve müsaitlik kontrolü yapan servis arayüzüdür.
/// Neden: Randevu oluştururken çakışma kontrolü yapmak için gereklidir.
/// Özelliği: GetAvailableSlots, CheckAvailability, CreateAppointment metotlarını içerir.
/// Kim Kullanacak: CreateAppointmentCommandHandler.
/// Amaç: Randevu planlama işlemlerinin yönetilmesi.
/// </summary>
public interface ISchedulingService
{
    Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(
        Guid physicianId,
        Guid departmentId,
        DateTime date,
        Guid tenantId);

    Task<bool> CheckAvailabilityAsync(
        Guid physicianId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeAppointmentId,
        Guid tenantId);

    Task<Appointment> CreateAppointmentAsync(
        Guid patientId,
        Guid physicianId,
        Guid departmentId,
        DateTime appointmentDate,
        TimeSpan startTime,
        AppointmentType appointmentType,
        string? reason,
        Guid createdBy,
        Guid tenantId);
}
```

### SchedulingService.cs
```csharp
/// <summary>
/// Randevu planlama servisi implementasyonu.
/// Ne: Randevu planlama ve müsaitlik kontrolü yapan servis sınıfıdır.
/// Neden: Randevu oluştururken çakışma kontrolü yapmak için gereklidir.
/// Özelliği: Slot bazlı kontrol, exception yönetimi, otomatik numara üretimi.
/// Kim Kullanacak: CreateAppointmentCommandHandler.
/// Amaç: Randevu planlama işlemlerinin yönetilmesi.
/// </summary>
public class SchedulingService : ISchedulingService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _slotRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<SchedulingService> _logger;

    public SchedulingService(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository slotRepository,
        IPatientRepository patientRepository,
        IEmployeeRepository employeeRepository,
        IMediator mediator,
        ILogger<SchedulingService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _slotRepository = slotRepository;
        _patientRepository = patientRepository;
        _employeeRepository = employeeRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(
        Guid physicianId,
        Guid departmentId,
        DateTime date,
        Guid tenantId)
    {
        // Get physician's slots for the day
        var slots = await _slotRepository.GetByPhysicianAndDayAsync(
            physicianId, date.DayOfWeek, tenantId);

        if (!slots.Any())
            return Enumerable.Empty<TimeSlotDto>();

        // Get existing appointments for the date
        var existingAppointments = await _appointmentRepository.GetByPhysicianAndDateAsync(
            physicianId, date, tenantId);

        var bookedTimes = existingAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.NoShow)
            .Select(a => a.StartTime)
            .ToHashSet();

        var result = new List<TimeSlotDto>();

        foreach (var slot in slots)
        {
            var availableTimes = slot.GetAvailableTimes()
                .Where(t => !bookedTimes.Contains(t));

            foreach (var time in availableTimes)
            {
                result.Add(new TimeSlotDto
                {
                    StartTime = time,
                    EndTime = time.Add(TimeSpan.FromMinutes(slot.SlotDuration)),
                    IsAvailable = true
                });
            }
        }

        return result.OrderBy(t => t.StartTime);
    }

    public async Task<bool> CheckAvailabilityAsync(
        Guid physicianId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeAppointmentId,
        Guid tenantId)
    {
        var existingAppointments = await _appointmentRepository.GetByPhysicianAndDateAsync(
            physicianId, date, tenantId);

        var overlapping = existingAppointments
            .Where(a => 
                a.Status != AppointmentStatus.Cancelled && 
                a.Status != AppointmentStatus.NoShow &&
                a.Id != excludeAppointmentId)
            .Any(a => 
                (startTime >= a.StartTime && startTime < a.EndTime) ||
                (endTime > a.StartTime && endTime <= a.EndTime) ||
                (startTime <= a.StartTime && endTime >= a.EndTime));

        return !overlapping;
    }

    public async Task<Appointment> CreateAppointmentAsync(
        Guid patientId,
        Guid physicianId,
        Guid departmentId,
        DateTime appointmentDate,
        TimeSpan startTime,
        AppointmentType appointmentType,
        string? reason,
        Guid createdBy,
        Guid tenantId)
    {
        // Validate patient exists
        var patient = await _patientRepository.GetByIdAsync(patientId, tenantId);
        if (patient == null)
            throw new PatientNotFoundException(patientId);

        // Validate physician exists
        var physician = await _employeeRepository.GetByIdAsync(physicianId);
        if (physician == null)
            throw new EmployeeNotFoundException(physicianId);

        // Calculate end time based on appointment type
        var duration = GetAppointmentDuration(appointmentType);
        var endTime = startTime.Add(TimeSpan.FromMinutes(duration));

        // Check availability
        var isAvailable = await CheckAvailabilityAsync(
            physicianId, appointmentDate, startTime, endTime, null, tenantId);

        if (!isAvailable)
            throw new AppointmentSlotNotAvailableException(
                $"Slot is not available for physician {physicianId} at {appointmentDate} {startTime}");

        // Generate appointment number
        var appointmentNumber = await _appointmentRepository.GenerateAppointmentNumberAsync(
            tenantId, appointmentDate);

        // Create appointment
        var appointment = Appointment.Create(
            appointmentNumber,
            patientId,
            physicianId,
            departmentId,
            appointmentDate,
            startTime,
            endTime,
            appointmentType,
            reason,
            tenantId,
            createdBy);

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Appointment created: {AppointmentNumber} for Patient: {PatientId} with Physician: {PhysicianId}",
            appointmentNumber, patientId, physicianId);

        // Publish event
        await _mediator.Publish(new AppointmentCreatedEvent
        {
            AppointmentId = appointment.Id,
            PatientId = patientId,
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            AppointmentDate = appointmentDate,
            AppointmentTime = startTime,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt
        });

        return appointment;
    }

    private static int GetAppointmentDuration(AppointmentType type)
    {
        return type switch
        {
            AppointmentType.Checkup => 30,
            AppointmentType.FollowUp => 15,
            AppointmentType.Procedure => 60,
            AppointmentType.Consultation => 45,
            AppointmentType.Emergency => 30,
            _ => 30
        };
    }
}
```

---

# PART 3: BILLING MODULE IMPLEMENTATION

## 3.1 Invoice Aggregate

### Invoice.cs
```csharp
/// <summary>
/// Fatura aggregate root sınıfı.
/// Ne: Hastane faturalarını temsil eden aggregate root sınıfıdır.
/// Neden: Mali işlemlerin temelini oluşturur ve fatura kalemleri ile birlikte yönetilir.
/// Özelliği: InvoiceNumber, PatientId, TenantId, InvoiceType, InvoiceDate, DueDate, SubTotal, TaxAmount,
///           DiscountAmount, TotalAmount, Currency, Status, PaymentStatus, CreatedBy, CreatedAt, InvoiceItems özelliklerine sahiptir.
/// Kim Kullanacak: InvoiceRepository, InvoiceService, CreateInvoiceCommandHandler.
/// Amaç: Fatura verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Invoice : BaseEntity, IAuditEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid TenantId { get; private set; }
    public InvoiceType InvoiceType { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public InvoiceStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public Guid? CancelledBy { get; private set; }
    public string? CancellationReason { get; private set; }

    // Navigation Properties
    private readonly List<InvoiceItem> _invoiceItems = new();
    public IReadOnlyCollection<InvoiceItem> InvoiceItems => _invoiceItems.AsReadOnly();

    public Patient Patient { get; private set; } = null!;

    private Invoice() { }

    public static Invoice Create(
        string invoiceNumber,
        Guid patientId,
        InvoiceType invoiceType,
        DateTime invoiceDate,
        DateTime? dueDate,
        Guid tenantId,
        Guid createdBy)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            PatientId = patientId,
            TenantId = tenantId,
            InvoiceType = invoiceType,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            SubTotal = 0,
            TaxAmount = 0,
            DiscountAmount = 0,
            TotalAmount = 0,
            Currency = "TRY",
            Status = InvoiceStatus.Draft,
            PaymentStatus = PaymentStatus.Unpaid,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(InvoiceItem item)
    {
        _invoiceItems.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _invoiceItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _invoiceItems.Remove(item);
            RecalculateTotals();
        }
    }

    public void ApplyDiscount(decimal discountAmount, DiscountType discountType)
    {
        if (discountType == DiscountType.Percentage)
        {
            DiscountAmount = SubTotal * (discountAmount / 100);
        }
        else
        {
            DiscountAmount = discountAmount;
        }

        if (DiscountAmount > SubTotal)
            DiscountAmount = SubTotal;

        RecalculateTotals();
    }

    public void Issue()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be issued");

        if (!_invoiceItems.Any())
            throw new InvalidOperationException("Invoice must have at least one item");

        Status = InvoiceStatus.Issued;
        IssuedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid(decimal paidAmount)
    {
        PaymentStatus = paidAmount >= TotalAmount 
            ? PaymentStatus.Paid 
            : PaymentStatus.PartialPaid;

        if (PaymentStatus == PaymentStatus.Paid)
            PaidAt = DateTime.UtcNow;
    }

    public void Cancel(string reason, Guid cancelledBy)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Invoice is already cancelled");

        if (PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Cannot cancel a paid invoice");

        Status = InvoiceStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        CancellationReason = reason;
    }

    private void RecalculateTotals()
    {
        SubTotal = _invoiceItems.Sum(i => i.TotalPrice);
        TaxAmount = SubTotal * 0.20m; // %20 KDV
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
    }

    public decimal Balance => TotalAmount - PaidAmount;
    public decimal PaidAmount => _invoiceItems.Sum(i => i.PaidAmount);
    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.Today && PaymentStatus != PaymentStatus.Paid;
}

public enum InvoiceType
{
    Patient = 1,
    Insurance = 2,
    Corporate = 3
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    Cancelled = 3
}

public enum PaymentStatus
{
    Unpaid = 1,
    PartialPaid = 2,
    Paid = 3,
    Overdue = 4
}

public enum DiscountType
{
    Fixed = 1,
    Percentage = 2
}
```

### InvoiceItem.cs
```csharp
/// <summary>
/// Fatura kalemi entity sınıfı.
/// Ne: Her fatura kalemini temsil eden entity sınıfıdır.
/// Neden: Fatura-işlem ilişkisi için gereklidir.
/// Özelliği: InvoiceId, ServiceType, ServiceId, ServiceCode, ServiceName, Quantity, UnitPrice, 
///           TotalPrice, DiscountRate, IsCoveredByInsurance, PatientPayRate, Status özelliklerine sahiptir.
/// Kim Kullanacak: Invoice aggregate.
/// Amaç: Fatura kalem verilerinin domain model olarak yönetilmesi.
/// </summary>
public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public ServiceType ServiceType { get; private set; }
    public Guid? ServiceId { get; private set; }
    public string ServiceCode { get; private set; } = string.Empty;
    public string ServiceName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public bool IsCoveredByInsurance { get; private set; }
    public decimal PatientPayRate { get; private set; }
    public InvoiceItemStatus Status { get; private set; }
    public decimal PaidAmount { get; private set; }

    private InvoiceItem() { }

    public static InvoiceItem Create(
        Guid invoiceId,
        ServiceType serviceType,
        Guid? serviceId,
        string serviceCode,
        string serviceName,
        int quantity,
        decimal unitPrice,
        bool isCoveredByInsurance,
        decimal patientPayRate)
    {
        var item = new InvoiceItem
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            ServiceType = serviceType,
            ServiceId = serviceId,
            ServiceCode = serviceCode,
            ServiceName = serviceName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice,
            DiscountRate = 0,
            IsCoveredByInsurance = isCoveredByInsurance,
            PatientPayRate = patientPayRate,
            Status = InvoiceItemStatus.Pending,
            PaidAmount = 0
        };

        return item;
    }

    public void ApplyDiscount(decimal discountRate)
    {
        DiscountRate = discountRate;
        TotalPrice = Quantity * UnitPrice * (1 - discountRate / 100);
    }

    public void RecordPayment(decimal amount)
    {
        PaidAmount += amount;
        if (PaidAmount >= TotalPrice)
            Status = InvoiceItemStatus.Paid;
        else if (PaidAmount > 0)
            Status = InvoiceItemStatus.PartialPaid;
    }
}

public enum ServiceType
{
    Examination = 1,
    Procedure = 2,
    Test = 3,
    Medication = 4,
    Material = 5,
    Room = 6,
    Other = 7
}

public enum InvoiceItemStatus
{
    Pending = 1,
    PartialPaid = 2,
    Paid = 3,
    Cancelled = 4
}
```

---

# PART 4: BACKGROUND JOBS IMPLEMENTATION

## 4.1 Appointment Reminder Job

### AppointmentReminderJob.cs
```csharp
/// <summary>
/// Randevu hatırlatma background job.
/// Ne: Yaklaşan randevular için hasta bildirimi gönderen scheduled job sınıfıdır.
/// Neden: Randevu hatırlatmalarının otomatik yapılması için gereklidir.
/// Özelliği: Quartz.NET kullanılarak implement edilmiştir. Her saat çalışır.
/// Kim Kullanacak: Quartz scheduler.
/// Amaç: Randevu hatırlatma işlemlerinin otomatik yapılması.
/// </summary>
[DisallowConcurrentExecution]
public class AppointmentReminderJob : IJob
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AppointmentReminderJob> _logger;

    public AppointmentReminderJob(
        IAppointmentRepository appointmentRepository,
        INotificationService notificationService,
        ILogger<AppointmentReminderJob> logger)
    {
        _appointmentRepository = appointmentRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Appointment reminder job started");

        try
        {
            var now = DateTime.UtcNow;
            var reminderTimes = new[] 
            { 
                now.AddHours(24),  // 24 hours before
                now.AddHours(2),   // 2 hours before
                now.AddMinutes(30) // 30 minutes before
            };

            foreach (var reminderTime in reminderTimes)
            {
                var appointments = await _appointmentRepository.GetAppointmentsForReminderAsync(
                    reminderTime,
                    context.CancellationToken);

                foreach (var appointment in appointments)
                {
                    try
                    {
                        await _notificationService.SendAppointmentReminderAsync(
                            appointment.PatientId,
                            appointment.TenantId,
                            appointment.AppointmentNumber,
                            appointment.AppointmentDate,
                            appointment.StartTime,
                            appointment.PhysicianId,
                            context.CancellationToken);

                        _logger.LogInformation(
                            "Reminder sent for appointment {AppointmentNumber} to patient {PatientId}",
                            appointment.AppointmentNumber,
                            appointment.PatientId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to send reminder for appointment {AppointmentNumber}",
                            appointment.AppointmentNumber);
                    }
                }
            }

            _logger.LogInformation("Appointment reminder job completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in appointment reminder job");
            throw;
        }
    }
}
```

## 4.2 License Expiration Job

### LicenseExpirationJob.cs
```csharp
/// <summary>
/// Lisans son kullanma kontrol job.
/// Ne: Yakında expire olacak lisanslar için bildirim gönderen scheduled job sınıfıdır.
/// Neden: Lisans süre bitiminden önce uyarı göndermek için gereklidir.
/// Özelliği: Günde bir çalışır. 30, 7, 1 gün önceden uyarı gönderir.
/// Kim Kullanacak: Quartz scheduler.
/// Amaç: Lisans süre yönetiminin otomatik yapılması.
/// </summary>
[DisallowConcurrentExecution]
public class LicenseExpirationJob : IJob
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly INotificationService _notificationService;
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<LicenseExpirationJob> _logger;

    public LicenseExpirationJob(
        ILicenseRepository licenseRepository,
        INotificationService notificationService,
        ITenantRepository tenantRepository,
        ILogger<LicenseExpirationJob> logger)
    {
        _licenseRepository = licenseRepository;
        _notificationService = notificationService;
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("License expiration check job started");

        try
        {
            var warningDays = new[] { 30, 7, 1 };

            foreach (var days in warningDays)
            {
                var expiringLicenses = await _licenseRepository.GetExpiringLicensesAsync(
                    days,
                    context.CancellationToken);

                foreach (var license in expiringLicenses)
                {
                    var tenant = await _tenantRepository.GetByIdAsync(license.TenantId);
                    if (tenant == null) continue;

                    await _notificationService.SendLicenseExpirationWarningAsync(
                        license.TenantId,
                        tenant.TenantCode,
                        license.ModuleName,
                        license.ExpiryDate!.Value,
                        days,
                        context.CancellationToken);

                    _logger.LogInformation(
                        "License expiration warning sent for tenant {TenantCode}, module {Module}, days remaining {Days}",
                        tenant.TenantCode,
                        license.ModuleName,
                        days);
                }
            }

            // Handle expired licenses
            var expiredLicenses = await _licenseRepository.GetExpiredLicensesAsync(
                context.CancellationToken);

            foreach (var license in expiredLicenses)
            {
                await _licenseRepository.DeactivateLicenseAsync(license.Id);
                
                _logger.LogInformation(
                    "License expired and deactivated: {LicenseId} for tenant {TenantId}",
                    license.Id,
                    license.TenantId);
            }

            _logger.LogInformation("License expiration check job completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in license expiration check job");
            throw;
        }
    }
}
```

---

# PART 5: KUBERNETES DEPLOYMENT

## 5.1 Deployment Manifest

### deployment.yaml
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: hbys-api
  namespace: hbys
  labels:
    app: hbys-api
    version: v1.0.0
spec:
  replicas: 3
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  selector:
    matchLabels:
      app: hbys-api
  template:
    metadata:
      labels:
        app: hbys-api
        version: v1.0.0
    spec:
      affinity:
        podAntiAffinity:
          preferredDuringSchedulingIgnoredDuringExecution:
          - weight: 100
            podAffinityTerm:
              labelSelector:
                matchLabels:
                  app: hbys-api
              topologyKey: kubernetes.io/hostname
      containers:
      - name: hbys-api
        image: hbys.azurecr.io/api:v1.0.0
        imagePullPolicy: Always
        ports:
        - containerPort: 5000
          name: http
          protocol: TCP
        - containerPort: 5001
          name: grpc
          protocol: TCP
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: hbys-secrets
              key: connection-string
        - name: Redis__ConnectionString
          valueFrom:
            secretKeyRef:
              name: hbys-secrets
              key: redis-connection
        - name: Jwt__SecretKey
          valueFrom:
            secretKeyRef:
              name: hbys-secrets
              key: jwt-secret
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        readinessProbe:
          httpGet:
            path: /ready
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3
      - name: log-shipper
        image: fluent/fluent-bit:2.1.0
        volumeMounts:
        - name: log-volume
          mountPath: /var/log
        - name: fluent-bit-config
          mountPath: /fluent-bit/etc
      volumes:
      - name: log-volume
        emptyDir: {}
      - name: fluent-bit-config
        configMap:
          name: hbys-fluent-bit-config
---
apiVersion: v1
kind: Service
metadata:
  name: hbys-api
  namespace: hbys
spec:
  type: ClusterIP
  ports:
  - port: 80
    targetPort: 5000
    protocol: TCP
    name: http
  selector:
    app: hbys-api
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: hbys-api-hpa
  namespace: hbys
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: hbys-api
  minReplicas: 3
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Percent
        value: 10
        periodSeconds: 60
    scaleUp:
      stabilizationWindowSeconds: 0
      policies:
      - type: Percent
        value: 100
        periodSeconds: 15
      - type: Pods
        value: 4
        periodSeconds: 15
      selectPolicy: Max
```

## 5.2 Ingress Configuration

### ingress.yaml
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: hbys-ingress
  namespace: hbys
  annotations:
    kubernetes.io/ingress.class: nginx
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/proxy-body-size: "50m"
    nginx.ingress.kubernetes.io/proxy-read-timeout: "300"
    nginx.ingress.kubernetes.io/proxy-send-timeout: "300"
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/rate-limit: "100"
    nginx.ingress.kubernetes.io/rate-limit-window: "1s"
spec:
  tls:
  - hosts:
    - api.hbys.com.tr
    - www.hbys.com.tr
    secretName: hbys-tls
  rules:
  - host: api.hbys.com.tr
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: hbys-api
            port:
              number: 80
  - host: www.hbys.com.tr
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: hbys-web
            port:
              number: 80
```

---

# PART 6: CI/CD PIPELINE

## 6.1 Azure DevOps Pipeline

### azure-pipelines.yml
```yaml
trigger:
  branches:
    include:
    - main
    - develop
    - release/*

pr:
  branches:
    include:
    - main
    - develop

variables:
  dockerRegistryServiceConnection: 'HBYS-Acr'
  imageRepository: 'hbys'
  dockerfilePath: '$(Build.SourcesDirectory)/src/HBYS.Api/Dockerfile'
  tag: '$(Build.BuildNumber)'
  majorVersion: '1'
  minorVersion: '0'

stages:
- stage: Build
  displayName: Build and Test
  jobs:
  - job: Build
    displayName: Build Application
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: UseDotNet@2
      displayName: 'Use .NET 10'
      inputs:
        packageType: 'sdk'
        version: '10.0.x'

    - script: |
        dotnet restore HBYS.sln
        dotnet build HBYS.sln --configuration Release --no-restore
      displayName: 'Restore and Build'

    - task: DotNetCoreCLI@2
      displayName: 'Run Unit Tests'
      inputs:
        command: 'test'
        projects: 'tests/**/*.csproj'
        arguments: '--configuration Release --collect:"XPlat Code Coverage" --results-directory:$(Agent.TempDirectory)/TestResults'

    - task: PublishCodeCoverageResults@1
      displayName: 'Publish Code Coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/TestResults/**/coverage.cobertura.xml'

    - task: Docker@2
      displayName: 'Build Docker Image'
      inputs:
        containerRegistry: $(dockerRegistryServiceConnection)
        repository: $(imageRepository)
        command: 'build'
        Dockerfile: $(dockerfilePath)
        tags: |
          $(tag)
          latest

    - task: Docker@2
      displayName: 'Push Docker Image'
      inputs:
        containerRegistry: $(dockerRegistryServiceConnection)
        repository: $(imageRepository)
        command: 'push'
        tags: |
          $(tag)
          latest

- stage: Test
  displayName: Integration Tests
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: IntegrationTests
    displayName: Run Integration Tests
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: DockerCompose@0
      displayName: 'Start Test Infrastructure'
      inputs:
        containerRegistry: $(dockerRegistryServiceConnection)
        dockerComposeFile: 'docker-compose.test.yml'
        action: 'Build services'
        additionalImageTags: '$(tag)'

    - task: DockerCompose@0
      displayName: 'Run Integration Tests'
      dockerComposeFile: 'docker-compose.test.yml'
      action: 'Run a specific container image'
      containerImage: 'hbys-integration-tests:$(tag)'

    - task: DockerCompose@0
      displayName: 'Stop Test Infrastructure'
      dockerComposeFile: 'docker-compose.test.yml'
      action: 'Down services'

- stage: Deploy_Dev
  displayName: Deploy to Development
  dependsOn: Test
  condition: succeeded()
  variables:
    environment: 'dev'
  jobs:
  - deployment: DeployToDev
    environment: 'HBYS-Dev'
    pool:
      vmImage: 'ubuntu-latest'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: Kubernetes@1
            displayName: 'Deploy to Dev Cluster'
            inputs:
              connectionType: 'Kubernetes Service Connection'
              kubernetesServiceConnection: 'HBYS-Dev-K8s'
              namespace: 'hbys-dev'
              manifests: |
                $(Pipeline.Workspace)/manifests/dev/*
              imagePullSecrets: 'hbys-acr-secret'
              containers: |
                container: hbys-api:$(tag)

- stage: Deploy_Staging
  displayName: Deploy to Staging
  dependsOn: Deploy_Dev
  condition: succeeded('Deploy_Dev')
  variables:
    environment: 'staging'
  jobs:
  - deployment: DeployToStaging
    environment: 'HBYS-Staging'
    pool:
      vmImage: 'ubuntu-latest'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: Kubernetes@1
            displayName: 'Deploy to Staging Cluster'
            inputs:
              connectionType: 'Kubernetes Service Connection'
              kubernetesServiceConnection: 'HBYS-Staging-K8s'
              namespace: 'hbys-staging'
              manifests: |
                $(Pipeline.Workspace)/manifests/staging/*
              imagePullSecrets: 'hbys-acr-secret'

- stage: Deploy_Production
  displayName: Deploy to Production
  dependsOn: Deploy_Staging
  condition: succeeded('Deploy_Staging')
  variables:
    environment: 'prod'
  jobs:
  - deployment: DeployToProduction
    environment: 'HBYS-Production'
    pool:
      vmImage: 'ubuntu-latest'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: Kubernetes@1
            displayName: 'Deploy to Production Cluster'
            inputs:
              connectionType: 'Kubernetes Service Connection'
              kubernetesServiceConnection: 'HBYS-Production-K8s'
              namespace: 'hbys-prod'
              manifests: |
                $(Pipeline.Workspace)/manifests/prod/*
              imagePullSecrets: 'hbys-acr-secret'
          - task: AzureAppServiceManage@0
            displayName: 'Swap Staging to Production'
            inputs:
              azureSubscription: 'HBYS-Production'
              Action: 'Swap Slots'
              WebAppName: 'hbys-api'
              SwapSource: 'staging'
```

Bu dokümantasyon, HBYS sisteminin spesifik implementasyon detaylarını içermektedir. Entity class'ları, repository implementasyonları, event handler'lar, scheduling service, billing aggregate, background jobs, Kubernetes deployment ve CI/CD pipeline örnekleri dahildir.