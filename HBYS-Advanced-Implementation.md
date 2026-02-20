# HBYS Advanced Implementation Details
## DTOs, EF Core Configuration, Unit of Work, Health Checks, API Versioning, and Distributed Tracing

---

# PART 1: DATA TRANSFER OBJECTS (DTOs)

## 1.1 Patient DTOs

### PatientDto.cs
```csharp
/// <summary>
/// Hasta veri transfer nesnesi.
/// Ne: Hasta verilerinin API üzerinden transferi için kullanılan DTO sınıfıdır.
/// Neden: Entity'lerin doğrudan expose edilmesini önlemek için gereklidir.
/// Özelliği: Id, PatientNumber, FirstName, LastName, FullName, Gender, DateOfBirth, Age, BloodType, Status, 
///           PhotoUrl, ContactInfos, Insurances, Allergies, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: PatientController, CreatePatientCommandHandler, GetPatientByIdQueryHandler.
/// Amaç: Hasta verilerinin transferi.
/// </summary>
public class PatientDto
{
    public Guid Id { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age => CalculateAge();
    public string? BloodType { get; set; }
    public PatientStatus Status { get; set; }
    public string? PhotoUrl { get; set; }
    public List<PatientContactDto> ContactInfos { get; set; } = new();
    public List<PatientInsuranceDto> Insurances { get; set; } = new();
    public List<PatientAllergyDto> Allergies { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public static PatientDto FromEntity(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            PatientNumber = patient.PatientNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Gender = patient.Gender,
            DateOfBirth = patient.DateOfBirth,
            BloodType = patient.BloodType,
            Status = patient.Status,
            PhotoUrl = patient.PhotoUrl,
            ContactInfos = patient.ContactInfos.Select(PatientContactDto.FromEntity).ToList(),
            Insurances = patient.Insurances.Select(PatientInsuranceDto.FromEntity).ToList(),
            Allergies = patient.Allergies.Select(PatientAllergyDto.FromEntity).ToList(),
            CreatedAt = patient.CreatedAt
        };
    }

    private int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }
}

/// <summary>
/// Hasta detay veri transfer nesnesi.
/// Ne: Hasta detaylı bilgilerinin transferi için kullanılan DTO sınıfıdır.
/// Neden: Detaylı hasta bilgileri için gereklidir.
/// Özelliği: PatientDto özelliklerine ek olarak ChronicDiseases, Medications, MedicalHistories içerir.
/// Kim Kullanacak: GetPatientByIdQueryHandler.
/// Amaç: Detaylı hasta bilgisi transferi.
/// </summary>
public class PatientDetailDto : PatientDto
{
    public List<PatientChronicDiseaseDto> ChronicDiseases { get; set; } = new();
    public List<PatientMedicationDto> Medications { get; set; } = new();
    public List<PatientMedicalHistoryDto> MedicalHistories { get; set; } = new();
    public PatientEmergencyContactDto? EmergencyContact { get; set; }

    public new static PatientDetailDto FromEntity(Patient patient, bool includeMedicalHistory = false, bool includeAppointments = false)
    {
        var dto = new PatientDetailDto
        {
            Id = patient.Id,
            PatientNumber = patient.PatientNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Gender = patient.Gender,
            DateOfBirth = patient.DateOfBirth,
            BloodType = patient.BloodType,
            Status = patient.Status,
            PhotoUrl = patient.PhotoUrl,
            ContactInfos = patient.ContactInfos.Select(PatientContactDto.FromEntity).ToList(),
            Insurances = patient.Insurances.Select(PatientInsuranceDto.FromEntity).ToList(),
            Allergies = patient.Allergies.Select(PatientAllergyDto.FromEntity).ToList(),
            ChronicDiseases = patient.ChronicDiseases.Select(PatientChronicDiseaseDto.FromEntity).ToList(),
            Medications = patient.Medications.Select(PatientMedicationDto.FromEntity).ToList(),
            EmergencyContact = patient.Guardians
                .Where(g => g.GuardianType == GuardianType.EmergencyContact)
                .Select(PatientEmergencyContactDto.FromEntity)
                .FirstOrDefault(),
            CreatedAt = patient.CreatedAt
        };

        if (includeMedicalHistory)
        {
            dto.MedicalHistories = patient.MedicalHistories
                .OrderByDescending(h => h.EventDate)
                .Select(PatientMedicalHistoryDto.FromEntity)
                .ToList();
        }

        return dto;
    }
}

/// <summary>
/// Hasta liste veri transfer nesnesi.
/// Ne: Hasta listesi için kullanılan DTO sınıfıdır.
/// Neden: Liste görüntüleme için gereklidir.
/// Özelliği: Id, PatientNumber, FullName, Age, Gender, Status, PrimaryPhone, CreatedAt içerir.
/// Kim Kullanacak: SearchPatientsQueryHandler.
/// Amaç: Hasta listesi transferi.
/// </summary>
public class PatientListDto
{
    public Guid Id { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public PatientStatus Status { get; set; }
    public string? PrimaryPhone { get; set; }
    public DateTime CreatedAt { get; set; }

    public static PatientListDto FromEntity(Patient patient)
    {
        return new PatientListDto
        {
            Id = patient.Id,
            PatientNumber = patient.PatientNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Age = patient.CalculateAge(),
            Gender = patient.Gender,
            Status = patient.Status,
            PrimaryPhone = patient.ContactInfos
                .FirstOrDefault(c => c.ContactType == ContactType.Phone && c.IsPrimary)?.Value,
            CreatedAt = patient.CreatedAt
        };
    }
}
```

## 1.2 Appointment DTOs

### AppointmentDto.cs
```csharp
/// <summary>
/// Randevu veri transfer nesnesi.
/// Ne: Randevu verilerinin transferi için kullanılan DTO sınıfıdır.
/// Neden: Randevu bilgilerinin API üzerinden transferi için gereklidir.
/// Özelliği: Id, AppointmentNumber, PatientId, PatientName, PhysicianId, PhysicianName, DepartmentId, DepartmentName,
///           AppointmentDate, StartTime, EndTime, Duration, AppointmentType, Status, Reason, Notes, CreatedAt içerir.
/// Kim Kullanacak: AppointmentController, CreateAppointmentCommandHandler.
/// Amaç: Randevu verilerinin transferi.
/// </summary>
public class AppointmentDto
{
    public Guid Id { get; set; }
    public string AppointmentNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid PhysicianId { get; set; }
    public string PhysicianName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration => (int)(EndTime - StartTime).TotalMinutes;
    public AppointmentType AppointmentType { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StatusText => Status switch
    {
        AppointmentStatus.Scheduled => "Planlandı",
        AppointmentStatus.Confirmed => "Onaylandı",
        AppointmentStatus.Completed => "Tamamlandı",
        AppointmentStatus.Cancelled => "İptal Edildi",
        AppointmentStatus.NoShow => "Gelmedi",
        _ => "Bilinmiyor"
    };

    public static AppointmentDto FromEntity(Appointment appointment, Patient? patient = null, Employee? physician = null, Department? department = null)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            AppointmentNumber = appointment.AppointmentNumber,
            PatientId = appointment.PatientId,
            PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : string.Empty,
            PhysicianId = appointment.PhysicianId,
            PhysicianName = physician != null ? $"{physician.FirstName} {physician.LastName}" : string.Empty,
            DepartmentId = appointment.DepartmentId,
            DepartmentName = department?.Name ?? string.Empty,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            AppointmentType = appointment.AppointmentType,
            Status = appointment.Status,
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            ConfirmedAt = appointment.ConfirmedAt,
            CancelledAt = appointment.CancelledAt,
            CreatedAt = appointment.CreatedAt
        };
    }
}

/// <summary>
/// Zaman dilimi veri transfer nesnesi.
/// Ne: Müsait randevu zaman dilimlerinin transferi için kullanılan DTO sınıfıdır.
/// Neden: Müsaitlik sorgulama için gereklidir.
/// Özelliği: StartTime, EndTime, IsAvailable özelliklerine sahiptir.
/// Kim Kullanacak: GetAvailableSlotsQueryHandler.
/// Amaç: Zaman dilimi transferi.
/// </summary>
public class TimeSlotDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string StartTimeFormatted => DateTime.Today.Add(StartTime).ToString("HH:mm");
    public string EndTimeFormatted => DateTime.Today.Add(EndTime).ToString("HH:mm");
}
```

## 1.3 Invoice DTOs

### InvoiceDto.cs
```csharp
/// <summary>
/// Fatura veri transfer nesnesi.
/// Ne: Fatura verilerinin transferi için kullanılan DTO sınıfıdır.
/// Neden: Fatura bilgilerinin API üzerinden transferi için gereklidir.
/// Özelliği: Id, InvoiceNumber, PatientId, PatientName, InvoiceType, InvoiceDate, DueDate, SubTotal, TaxAmount,
///           DiscountAmount, TotalAmount, Currency, Status, PaymentStatus, Balance, InvoiceItems, CreatedAt içerir.
/// Kim Kullanacak: InvoiceController, CreateInvoiceCommandHandler.
/// Amaç: Fatura verilerinin transferi.
/// </summary>
public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public InvoiceStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal Balance { get; set; }
    public decimal PaidAmount { get; set; }
    public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public string StatusText => Status switch
    {
        InvoiceStatus.Draft => "Taslak",
        InvoiceStatus.Issued => "Düzenlendi",
        InvoiceStatus.Cancelled => "İptal Edildi",
        _ => "Bilinmiyor"
    };

    public string PaymentStatusText => PaymentStatus switch
    {
        PaymentStatus.Unpaid => "Ödenmedi",
        PaymentStatus.PartialPaid => "Kısmi Ödendi",
        PaymentStatus.Paid => "Ödendi",
        PaymentStatus.Overdue => "Vadesi Geçti",
        _ => "Bilinmiyor"
    };

    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.Today && PaymentStatus != PaymentStatus.Paid;

    public static InvoiceDto FromEntity(Invoice invoice, Patient? patient = null)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PatientId = invoice.PatientId,
            PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : string.Empty,
            InvoiceType = invoice.InvoiceType,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            Currency = invoice.Currency,
            Status = invoice.Status,
            PaymentStatus = invoice.PaymentStatus,
            Balance = invoice.Balance,
            PaidAmount = invoice.PaidAmount,
            InvoiceItems = invoice.InvoiceItems.Select(InvoiceItemDto.FromEntity).ToList(),
            CreatedAt = invoice.CreatedAt
        };
    }
}

/// <summary>
/// Fatura kalemi veri transfer nesnesi.
/// Ne: Fatura kalemi verilerinin transferi için kullanılan DTO sınıfıdır.
/// Neden: Fatura kalemlerinin API üzerinden transferi için gereklidir.
/// Özelliği: Id, ServiceType, ServiceCode, ServiceName, Quantity, UnitPrice, TotalPrice, DiscountRate, 
///           IsCoveredByInsurance, PatientPayRate, Status, PaidAmount içerir.
/// Kim Kullanacak: InvoiceDto, CreateInvoiceCommandHandler.
/// Amaç: Fatura kalemi verilerinin transferi.
/// </summary>
public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public ServiceType ServiceType { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public bool IsCoveredByInsurance { get; set; }
    public decimal PatientPayRate { get; set; }
    public InvoiceItemStatus Status { get; set; }
    public decimal PaidAmount { get; set; }

    public decimal InsuranceCoverage => IsCoveredByInsurance ? TotalPrice * (1 - PatientPayRate / 100) : 0;
    public decimal PatientPayment => TotalPrice - InsuranceCoverage;

    public static InvoiceItemDto FromEntity(InvoiceItem item)
    {
        return new InvoiceItemDto
        {
            Id = item.Id,
            ServiceType = item.ServiceType,
            ServiceCode = item.ServiceCode,
            ServiceName = item.ServiceName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            DiscountRate = item.DiscountRate,
            IsCoveredByInsurance = item.IsCoveredByInsurance,
            PatientPayRate = item.PatientPayRate,
            Status = item.Status,
            PaidAmount = item.PaidAmount
        };
    }
}
```

---

# PART 2: EF CORE CONFIGURATION

## 2.1 Patient Module Configuration

### PatientConfiguration.cs
```csharp
/// <summary>
/// Patient entity configuration sınıfı.
/// Ne: Patient entity'sinin EF Core configuration sınıfıdır.
/// Neden: Veritabanı şemasının doğru yapılandırılması için gereklidir.
/// Özelliği: Tablo adı, index'ler, foreign key'ler, constraint'ler yapılandırılır.
/// Kim Kullanacak: DbContext configuration.
/// Amaç: Patient entity mapping.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Table name
        builder.ToTable("Patients", "Patient");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.PatientNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.TurkishId)
            .IsRequired()
            .HasMaxLength(256); // Encrypted

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Gender)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.BloodType)
            .HasMaxLength(5);

        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        // TenantId - required for multi-tenancy
        builder.Property(p => p.TenantId)
            .IsRequired();

        // Unique indexes
        builder.HasIndex(p => new { p.TenantId, p.PatientNumber })
            .IsUnique()
            .HasDatabaseName("UK_Patients_Tenant_PatientNumber");

        builder.HasIndex(p => new { p.TenantId, p.TurkishId })
            .IsUnique()
            .HasDatabaseName("UK_Patients_Tenant_TurkishId");

        // Indexes
        builder.HasIndex(p => p.TenantId)
            .HasDatabaseName("IX_Patients_TenantId");

        builder.HasIndex(p => new { p.TenantId, p.LastName, p.FirstName })
            .HasDatabaseName("IX_Patients_Tenant_Name");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Patients_Status");

        // Soft delete filter
        builder.HasQueryFilter(p => p.Status != PatientStatus.Deceased || p.Status == PatientStatus.Active);

        // Concurrency
        builder.Property(p => p.UpdatedAt)
            .IsConcurrencyToken();
    }
}

/// <summary>
/// PatientContact entity configuration sınıfı.
/// Ne: PatientContact entity'sinin EF Core configuration sınıfıdır.
/// Neden: İletişim bilgileri için doğru şema yapılandırması için gereklidir.
/// Kim Kullanacak: DbContext configuration.
/// </summary>
public class PatientContactConfiguration : IEntityTypeConfiguration<PatientContact>
{
    public void Configure(EntityTypeBuilder<PatientContact> builder)
    {
        builder.ToTable("PatientContacts", "Patient");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContactType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.IsPrimary)
            .HasDefaultValue(false);

        builder.Property(c => c.IsVerified)
            .HasDefaultValue(false);

        builder.Property(c => c.TenantId)
            .IsRequired();

        // Relationships
        builder.HasOne(c => c.Patient)
            .WithMany(p => p.ContactInfos)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.PatientId)
            .HasDatabaseName("IX_PatientContacts_PatientId");

        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_PatientContacts_TenantId");
    }
}
```

## 2.2 Appointment Module Configuration

### AppointmentConfiguration.cs
```csharp
/// <summary>
/// Appointment entity configuration sınıfı.
/// Ne: Appointment entity'sinin EF Core configuration sınıfıdır.
/// Neden: Randevu verileri için doğru şema yapılandırması için gereklidir.
/// Kim Kullanacak: DbContext configuration.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments", "Appointment");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AppointmentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.AppointmentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.AppointmentDate)
            .IsRequired();

        builder.Property(a => a.StartTime)
            .IsRequired()
            .HasConversion(
                v => v.Ticks,
                v => TimeSpan.FromTicks(v));

        builder.Property(a => a.EndTime)
            .IsRequired()
            .HasConversion(
                v => v.Ticks,
                v => TimeSpan.FromTicks(v));

        builder.Property(a => a.Reason)
            .HasMaxLength(500);

        builder.Property(a => a.CancelledReason)
            .HasMaxLength(500);

        builder.Property(a => a.TenantId)
            .IsRequired();

        // Unique index
        builder.HasIndex(a => new { a.TenantId, a.AppointmentNumber })
            .IsUnique()
            .HasDatabaseName("UK_Appointments_Tenant_AppointmentNumber");

        // Indexes
        builder.HasIndex(a => a.PatientId)
            .HasDatabaseName("IX_Appointments_PatientId");

        builder.HasIndex(a => a.PhysicianId)
            .HasDatabaseName("IX_Appointments_PhysicianId");

        builder.HasIndex(a => a.AppointmentDate)
            .HasDatabaseName("IX_Appointments_AppointmentDate");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Appointments_Status");

        // Composite index for physician schedule
        builder.HasIndex(a => new { a.PhysicianId, a.AppointmentDate, a.Status })
            .HasDatabaseName("IX_Appointments_Physician_Date_Status");

        // Relationships
        builder.HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Physician)
            .WithMany()
            .HasForeignKey(a => a.PhysicianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Department)
            .WithMany()
            .HasForeignKey(a => a.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

# PART 3: UNIT OF WORK

## 3.1 Unit of Work Implementation

### IUnitOfWork.cs
```csharp
/// <summary>
/// Unit of Work arayüzü.
/// Ne: Database transaction yönetimi için kullanılan Unit of Work arayüzüdür.
/// Neden: Birden fazla repository'nin tek transaction içinde yönetilmesi için gereklidir.
/// Özelliği: SaveChanges, SaveChangesAsync, BeginTransaction, CommitTransaction, RollbackTransaction metotlarını içerir.
/// Kim Kullanacak: Command handlers, Services.
/// Amaç: Transaction yönetimi.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    IInvoiceRepository Invoices { get; }
    ILicenseRepository Licenses { get; }
    ITenantRepository Tenants { get; }
    IUserRepository Users { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### UnitOfWork.cs
```csharp
/// <summary>
/// Unit of Work implementasyonu.
/// Ne: EF Core kullanarak database transaction yönetimi sağlayan sınıftır.
/// Neden: Birden fazla repository'nin tek transaction içinde yönetilmesi için gereklidir.
/// Özelliği: DbContext management, transaction management, repository access.
/// Kim Kullanacak: Command handlers, Services.
/// Amaç: Transaction yönetimi.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly HbysDbContext _context;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private IDbContextTransaction? _transaction;

    // Repositories
    private IPatientRepository? _patientRepository;
    private IAppointmentRepository? _appointmentRepository;
    private IInvoiceRepository? _invoiceRepository;
    private ILicenseRepository? _licenseRepository;
    private ITenantRepository? _tenantRepository;
    private IUserRepository? _userRepository;

    public UnitOfWork(
        HbysDbContext context,
        ITenantContextAccessor tenantContextAccessor)
    {
        _context = context;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public IPatientRepository Patients => 
        _patientRepository ??= new PatientRepository(_context, _tenantContextAccessor);

    public IAppointmentRepository Appointments => 
        _appointmentRepository ??= new AppointmentRepository(_context, _tenantContextAccessor);

    public IInvoiceRepository Invoices => 
        _invoiceRepository ??= new InvoiceRepository(_context, _tenantContextAccessor);

    public ILicenseRepository Licenses => 
        _licenseRepository ??= new LicenseRepository(_context, _tenantContextAccessor);

    public ITenantRepository Tenants => 
        _tenantRepository ??= new TenantRepository(_context);

    public IUserRepository Users => 
        _userRepository ??= new UserRepository(_context, _tenantContextAccessor);

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");

        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");

        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

---

# PART 4: HEALTH CHECKS

## 4.1 Health Check Implementation

### HbysHealthCheck.cs
```csharp
/// <summary>
/// HBYS health check sınıfı.
/// Ne: Uygulama sağlık durumunu kontrol eden sınıftır.
/// Neden: Kubernetes liveness ve readiness probe'ları için gereklidir.
/// Özelliği: Database, Redis, external services durumlarını kontrol eder.
/// Kim Kullanacak: Health check endpoint.
/// Amaç: Uygulama sağlık durumu kontrolü.
/// </summary>
public class HbysHealthCheck : IHealthCheck
{
    private readonly HbysDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILicenseService _licenseService;
    private readonly ILogger<HbysHealthCheck> _logger;

    public HbysHealthCheck(
        HbysDbContext context,
        IConnectionMultiplexer redis,
        ILicenseService licenseService,
        ILogger<HbysHealthCheck> logger)
    {
        _context = context;
        _redis = redis;
        _licenseService = licenseService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var checks = new Dictionary<string, object>();

        try
        {
            // Database check
            var dbHealthy = await CheckDatabaseAsync(cancellationToken);
            checks["Database"] = dbHealthy ? "Healthy" : "Unhealthy";

            // Redis check
            var redisHealthy = await CheckRedisAsync(cancellationToken);
            checks["Redis"] = redisHealthy ? "Healthy" : "Unhealthy";

            // License check
            var licenseHealthy = await CheckLicenseAsync(cancellationToken);
            checks["License"] = licenseHealthy ? "Active" : "Expired";

            var isHealthy = dbHealthy && redisHealthy && licenseHealthy;

            return isHealthy
                ? HealthCheckResult.Healthy("All checks passed", checks)
                : HealthCheckResult.Degraded("Some checks failed", checks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return HealthCheckResult.Unhealthy("Health check failed", ex, checks);
        }
    }

    private async Task<bool> CheckDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckRedisAsync(CancellationToken cancellationToken)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.PingAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckLicenseAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _licenseService.IsLicenseActiveAsync();
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Custom health check extensions.
/// Ne: Health check registration için extension metodları.
/// Kim Kullanacak: Program.cs
/// </summary>
public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddHbysHealthChecks(this IHealthChecksBuilder builder)
    {
        builder.AddCheck<HbysHealthCheck>("hbys_health_check");

        builder.AddDbContextCheck<HbysDbContext>("database");
        
        builder.AddRedis(_ => _.GetService<IConnectionMultiplexer>()?.Configuration ?? "localhost:6379");

        return builder;
    }
}
```

---

# PART 5: API VERSIONING

## 5.1 API Versioning Configuration

### ApiVersioningConfiguration.cs
```csharp
/// <summary>
/// API versioning configuration sınıfı.
/// Ne: API versiyonlama yapılandırması için kullanılan sınıftır.
/// Neden: Backward compatibility için gereklidir.
/// Özelliği: URL path, header, media type versiyonlama destekler.
/// Kim Kullanacak: Program.cs
/// </summary>
public static class ApiVersioningConfiguration
{
    public static IServiceCollection AddHbysApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // Default version
            options.DefaultApiVersion = new ApiVersion(1, 0);
            
            // Report supported versions
            options.ReportApiVersions = true;
            
            // Version detection strategies
            options.VersionByRouteEnabled = true;  // /api/v1/patients
            options.VersionByHeaderVersion = "X-Api-Version";
            options.VersionByQueryStringVersion = "api-version";
            
            // Version neutral - serve latest version
            options.AssumeDefaultVersionWhenUnspecified = true;
            
            // Error response
            options.ErrorResponses = new VersioningErrorResponseProvider();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}

/// <summary>
/// Versioning error response provider.
/// Ne: Versiyonlama hataları için custom response üretir.
/// Kim Kullanacak: API Versioning
/// </summary>
public class VersioningErrorResponseProvider : IErrorResponseProvider
{
    public ProblemDetails ProduceErrorResponse(ApiVersioningContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid API Version",
            Detail = $"The requested API version '{context.RawRequestedApiVersion}' is not supported. " +
                     $"Supported versions: {string.Join(", ", context.SupportedApiVersions.Select(v => $"v{v.MajorVersion}.{v.MinorVersion}"))}",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        problemDetails.Extensions["supportedVersions"] = context.SupportedApiVersions
            .Select(v => $"v{v.MajorVersion}.{v.MinorVersion}")
            .ToList();

        problemDetails.Extensions["currentVersion"] = $"v{context.DefaultApiVersion.MajorVersion}.{context.DefaultApiVersion.MinorVersion}";

        return problemDetails;
    }
}
```

---

# PART 6: DISTRIBUTED TRACING

## 6.1 Tracing Configuration

### TracingExtensions.cs
```csharp
/// <summary>
/// Distributed tracing configuration sınıfı.
/// Ne: OpenTelemetry kullanarak distributed tracing yapılandırması için kullanılan sınıftır.
/// Neden: Microservices arası request takibi için gereklidir.
/// Özelliği: Activity source, exporter, sampler configuration.
/// Kim Kullanacak: Program.cs
/// </summary>
public static class TracingExtensions
{
    public static IServiceCollection AddHbysTracing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService("HBYS.Api")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production",
                    ["host.name"] = Environment.MachineName
                }))
            .WithTracing(tracing => tracing
                .AddSource("HBYS")
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRedisInstrumentation()
                .AddSource("System.Net.Http")
                .AddJaegerExporter(options =>
                {
                    options.AgentHost = configuration["Jaeger:AgentHost"] ?? "localhost";
                    options.AgentPort = int.Parse(configuration["Jaeger:AgentPort"] ?? "6831");
                })
                // Alternative: Zipkin
                // .AddZipkinExporter(options =>
                // {
                //     options.Endpoint = new Uri(configuration["Zipkin:Endpoint"] ?? "http://localhost:9411/api/v2/spans");
                // })
            );

        // Add activity source
        services.AddSingleton<ActivitySource>(new ActivitySource("HBYS"));

        return services;
    }
}

/// <summary>
/// Activity middleware.
/// Ne: Request'ler için correlation ID oluşturan middleware'dir.
/// Neden: Distributed tracing için gereklidir.
/// Kim Kullanacak: Pipeline
/// </summary>
public class ActivityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ActivitySource _activitySource;
    private readonly ILogger<ActivityMiddleware> _logger;

    public ActivityMiddleware(
        RequestDelegate next,
        ActivitySource activitySource,
        ILogger<ActivityMiddleware> logger)
    {
        _next = next;
        _activitySource = activitySource;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var activityName = $"{context.Request.Method} {context.Request.Path}";
        
        using var activity = _activitySource.StartActivity(activityName);

        // Add common tags
        activity?.AddTag("http.method", context.Request.Method);
        activity?.AddTag("http.url", context.Request.Path);
        activity?.AddTag("http.host", context.Request.Host);

        // Add correlation ID
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
        
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        activity?.AddTag("correlation.id", correlationId);

        // Add tenant ID if available
        var tenantId = context.User.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            activity?.AddTag("tenant.id", tenantId);
        }

        try
        {
            await _next(context);
            
            activity?.AddTag("http.status_code", context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            activity?.AddTag("error", true);
            activity?.AddTag("error.message", ex.Message);
            _logger.LogError(ex, "Request failed: {Path}", context.Request.Path);
            throw;
        }
    }
}
```

---

# PART 7: PAGINATION

## 7.1 Pagination Implementation

### PaginatedResult.cs
```csharp
/// <summary>
/// Sayfalanmış sonuç sınıfı.
/// Ne: Sayfalanmış veri sonuçları için kullanılan sınıftır.
/// Neden: Large dataset'lerin sayfalanması için gereklidir.
/// Özelliği: Items, TotalCount, PageNumber, PageSize, TotalPages, HasNextPage, HasPreviousPage özelliklerine sahiptir.
/// Kim Kullanacak: Query handlers, API controllers.
/// Amaç: Sayfalanmış veri transferi.
/// </summary>
public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
    public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    public PaginatedResult(
        IReadOnlyList<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PaginatedResult<T> Create(
        IReadOnlyList<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        return new PaginatedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}

/// <summary>
/// Pagination request sınıfı.
/// Ne: Sayfalama parametreleri için kullanılan sınıftır.
/// Neden: Query parameter'ları için gereklidir.
/// Kim Kullanacak: Query handlers, API controllers.
/// </summary>
public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }

    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    public void Normalize()
    {
        if (PageNumber < 1)
            PageNumber = 1;

        if (PageSize < 1)
            PageSize = DefaultPageSize;

        if (PageSize > MaxPageSize)
            PageSize = MaxPageSize;
    }
}
```

---

Bu dokümantasyon, HBYS sisteminin ileri düzey implementasyon detaylarını içermektedir. DTO'lar, EF Core configuration'lar, Unit of Work pattern, health checks, API versioning, distributed tracing ve pagination implementasyonları dahildir.