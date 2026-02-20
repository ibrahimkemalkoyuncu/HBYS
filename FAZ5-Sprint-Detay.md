# HBYS FAZ 5 - Sprint 26-30 Detaylı Domain Task Listesi

## HR (İnsan Kaynakları), Quality (Kalite), Document (Doküman), Notification (Bildirim) Modülleri

---

## 📋 Genel Bakış

FAZ 5, HBYS sisteminin insan kaynakları, kalite yönetimi, doküman yönetimi ve bildirim sistemlerini kapsamaktadır. Bu faz, hastane operasyonlarının destekleyici sistemlerini içermektedir.

### FAZ 5 Hedefleri
- Personel yönetimi ve bordrolama altyapısı
- Kalite指标 (KPI) takibi ve raporlaması
- Kurumsal doküman yönetimi
- Çok kanallı bildirim sistemi

---

## SPRINT 26: HR - Personel Yönetimi Altyapısı

### Sprint 26 Hedefi
İnsan kaynakları modülünün temel altyapısı, personel entity'leri, temel CRUD işlemleri ve organizasyon şeması yapısının kurulması.

---

### Domain Task 26.1: Employee Entity ve Organization Structure

#### Task Tanımı
Personel entity'sinin oluşturulması ve organizasyon yapısının tasarlanması.

#### Entity Sınıfları

**Employee.cs**
```csharp
/// <summary>
/// Personel entity sınıfı.
/// Ne: Hastanede çalışan tüm personeli temsil eden aggregate root sınıfıdır.
/// Neden: Personel bilgilerinin merkezi yönetimi ve tüm personel işlemlerinin izlenmesi için gereklidir.
/// Özelliği: EmployeeNumber (unique), TurkishId (encrypted), FirstName, LastName, Gender, DateOfBirth, 
///           EmployeeType, DepartmentId, PositionId, EmploymentStatus, StartDate, EndDate, Salary, 
///           BankAccount (encrypted), SocialSecurityNumber (encrypted), TenantId, CreatedBy, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: EmployeeRepository, CreateEmployeeCommandHandler, GetEmployeeByIdQueryHandler,
///                PayrollService, ScheduleService.
/// Amaç: Personel verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Employee : BaseEntity, IAuditTEntity
{
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string TurkishId { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string? PhotoUrl { get; private set; }
    public EmployeeType EmployeeType { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid? PositionId { get; private set; }
    public Guid? ManagerId { get; private set; }
    public EmploymentStatus EmploymentStatus { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public decimal? Salary { get; private set; }
    public string? BankAccount { get; private set; }
    public string? SocialSecurityNumber { get; private set; }
    public string? DiplomaNumber { get; private set; }
    public string? MedicalLicenseNumber { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation Properties
    public Department Department { get; private set; } = null!;
    public Employee? Manager { get; private set; }
    public Position? Position { get; private set; }
    
    private readonly List<Employee> _subordinates = new();
    public IReadOnlyCollection<Employee> Subordinates => _subordinates.AsReadOnly();

    private readonly List<EmployeeDocument> _documents = new();
    public IReadOnlyCollection<EmployeeDocument> Documents => _documents.AsReadOnly();

    private readonly List<EmployeeShift> _shifts = new();
    public IReadOnlyCollection<EmployeeShift> Shifts => _shifts.AsReadOnly();

    private readonly List<EmployeeLeave> _leaves = new();
    public IReadOnlyCollection<EmployeeLeave> Leaves => _leaves.AsReadOnly();

    private Employee() { }

    public static Employee Create(
        string employeeNumber,
        string turkishId,
        string firstName,
        string lastName,
        Gender gender,
        DateTime dateOfBirth,
        EmployeeType employeeType,
        Guid departmentId,
        Guid? positionId,
        Guid? managerId,
        DateTime startDate,
        decimal? salary,
        Guid tenantId,
        Guid createdBy)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = employeeNumber,
            TurkishId = turkishId,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            EmployeeType = employeeType,
            DepartmentId = departmentId,
            PositionId = positionId,
            ManagerId = managerId,
            EmploymentStatus = EmploymentStatus.Active,
            StartDate = startDate,
            Salary = salary,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        return employee;
    }

    public void UpdateBasicInfo(
        string firstName,
        string lastName,
        Gender gender,
        DateTime dateOfBirth,
        Guid departmentId,
        Guid? positionId)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        DepartmentId = departmentId;
        PositionId = positionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmploymentInfo(
        Guid? managerId,
        decimal? salary,
        string? bankAccount)
    {
        ManagerId = managerId;
        Salary = salary;
        BankAccount = bankAccount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Terminate(DateTime endDate)
    {
        EmploymentStatus = EmploymentStatus.Terminated;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        EmploymentStatus = EmploymentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        EmploymentStatus = EmploymentStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
    
    public int CalculateTenure()
    {
        var end = EndDate ?? DateTime.Today;
        return (int)((end - StartDate).TotalDays / 365);
    }
}

public enum EmployeeType
{
    Physician = 1,
    Nurse = 2,
    Technician = 3,
    Administrative = 4,
    Support = 5,
    Management = 6
}

public enum EmploymentStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Terminated = 4,
    OnLeave = 5
}
```

**Department.cs**
```csharp
/// <summary>
/// Departman entity sınıfı.
/// Ne: Hastane departmanlarını temsil eden entity sınıfıdır.
/// Neden: Organizasyon yapısı ve departman hiyerarşisi için gereklidir.
/// Özelliği: DepartmentCode, Name, Description, ParentDepartmentId, DepartmentType, Floor, Building, 
///           IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: Employee aggregate, AppointmentService, ScheduleService.
/// Amaç: Departman verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Department : BaseEntity
{
    public string DepartmentCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentDepartmentId { get; private set; }
    public DepartmentType DepartmentType { get; private set; }
    public string? Floor { get; private set; }
    public string? Building { get; private set; }
    public int? Capacity { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation Properties
    public Department? ParentDepartment { get; private set; }
    
    private readonly List<Department> _subDepartments = new();
    public IReadOnlyCollection<Department> SubDepartments => _subDepartments.AsReadOnly();

    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Department() { }

    public static Department Create(
        string departmentCode,
        string name,
        string? description,
        Guid? parentDepartmentId,
        DepartmentType departmentType,
        string? floor,
        string? building,
        int? capacity,
        Guid tenantId)
    {
        return new Department
        {
            Id = Guid.NewGuid(),
            DepartmentCode = departmentCode,
            Name = name,
            Description = description,
            ParentDepartmentId = parentDepartmentId,
            DepartmentType = departmentType,
            Floor = floor,
            Building = building,
            Capacity = capacity,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(string name, string? description, string? floor, string? building, int? capacity)
    {
        Name = name;
        Description = description;
        Floor = floor;
        Building = building;
        Capacity = capacity;
    }
}

public enum DepartmentType
{
    Medical = 1,
    Surgical = 2,
    Diagnostic = 3,
    Emergency = 4,
    Administrative = 5,
    Support = 6,
    Management = 7
}
```

**Position.cs**
```csharp
/// <summary>
/// Pozisyon entity sınıfı.
/// Ne: İş pozisyonlarını temsil eden entity sınıfıdır.
/// Neden: Personel atamaları ve yetki yönetimi için gereklidir.
/// Özelliği: PositionCode, Name, Description, DepartmentId, SalaryScale, IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: Employee aggregate.
/// Amaç: Pozisyon verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Position : BaseEntity
{
    public string PositionCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid DepartmentId { get; private set; }
    public int SalaryScale { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Position() { }

    public static Position Create(
        string positionCode,
        string name,
        string? description,
        Guid departmentId,
        int salaryScale,
        Guid tenantId)
    {
        return new Position
        {
            Id = Guid.NewGuid(),
            PositionCode = positionCode,
            Name = name,
            Description = description,
            DepartmentId = departmentId,
            SalaryScale = salaryScale,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

---

### Domain Task 26.2: Employee Repository ve Servisleri

#### Task Tanımı
Personel veri erişim katmanı ve temel servislerin oluşturulması.

**IEmployeeRepository.cs**
```csharp
/// <summary>
/// Personel repository arayüzü.
/// Ne: Personel verilerine erişim için kullanılan repository arayüzüdür.
/// Neden: Veri erişim katmanının soyutlanması için gereklidir.
/// Özelliği: CRUD ve sorgulama metotlarını içerir.
/// Kim Kullanacak: EmployeeService, CreateEmployeeCommandHandler.
/// Amaç: Personel veri erişiminin yönetilmesi.
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Employee?> GetByTurkishIdAsync(string encryptedTurkishId, Guid tenantId);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber, Guid tenantId);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(Guid departmentId, Guid tenantId);
    Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<Guid> ids, Guid tenantId);
    Task<PaginatedResult<Employee>> SearchAsync(
        Guid tenantId,
        string? searchTerm,
        EmployeeType? employeeType,
        EmploymentStatus? status,
        Guid? departmentId,
        int pageNumber,
        int pageSize);
    Task<string> GenerateEmployeeNumberAsync(Guid tenantId);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**EmployeeService.cs**
```csharp
/// <summary>
/// Personel servisi.
/// Ne: Personel işlemlerini yöneten servis sınıfıdır.
/// Neden: Personel ile ilgili iş kurallarının uygulanması için gereklidir.
/// Özelliği: CRUD, istihdam yönetimi, organizasyon sorgulama metotlarını içerir.
/// Kim Kullanacak: EmployeeController, Command/Query Handlers.
/// Amaç: Personel işlemlerinin yönetilmesi.
/// </summary>
public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IMediator mediator,
        ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request, Guid tenantId, Guid createdBy)
    {
        // Validate department exists
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, tenantId);
        if (department == null)
            throw new DepartmentNotFoundException(request.DepartmentId);

        // Check for duplicate TurkishId
        var existingEmployee = await _employeeRepository.GetByTurkishIdAsync(request.TurkishId, tenantId);
        if (existingEmployee != null)
            throw new DuplicateTurkishIdException(request.TurkishId);

        // Generate employee number
        var employeeNumber = await _employeeRepository.GenerateEmployeeNumberAsync(tenantId);

        // Create employee
        var employee = Employee.Create(
            employeeNumber,
            request.TurkishId,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth,
            request.EmployeeType,
            request.DepartmentId,
            request.PositionId,
            request.ManagerId,
            request.StartDate,
            request.Salary,
            tenantId,
            createdBy);

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        _logger.LogInformation("Employee created: {EmployeeNumber} - {FullName}", 
            employeeNumber, employee.FullName);

        // Publish event
        await _mediator.Publish(new EmployeeCreatedEvent
        {
            EmployeeId = employee.Id,
            EmployeeNumber = employeeNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EmployeeType = employee.EmployeeType,
            DepartmentId = employee.DepartmentId,
            TenantId = tenantId,
            CreatedAt = employee.CreatedAt
        });

        return employee;
    }

    public async Task<Employee> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, Guid tenantId, Guid updatedBy)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, tenantId);
        if (employee == null)
            throw new EmployeeNotFoundException(employeeId);

        employee.UpdateBasicInfo(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth,
            request.DepartmentId,
            request.PositionId);

        if (request.Salary.HasValue || request.BankAccount != null)
        {
            employee.UpdateEmploymentInfo(
                request.ManagerId,
                request.Salary,
                request.BankAccount);
        }

        await _employeeRepository.UpdateAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        _logger.LogInformation("Employee updated: {EmployeeNumber}", employee.EmployeeNumber);

        return employee;
    }

    public async Task<IEnumerable<Employee>> GetOrganizationChartAsync(Guid tenantId, Guid? departmentId = null)
    {
        var employees = await _employeeRepository.SearchAsync(
            tenantId,
            null,
            null,
            EmploymentStatus.Active,
            departmentId,
            1,
            1000);

        return employees.Items.Where(e => e.ManagerId == null);
    }

    public async Task<IEnumerable<Employee>> GetSubordinatesAsync(Guid managerId, Guid tenantId)
    {
        var manager = await _employeeRepository.GetByIdAsync(managerId, tenantId);
        if (manager == null)
            throw new EmployeeNotFoundException(managerId);

        var allEmployees = await _employeeRepository.SearchAsync(
            tenantId,
            null,
            null,
            EmploymentStatus.Active,
            null,
            1,
            10000);

        return allEmployees.Items.Where(e => e.ManagerId == managerId);
    }
}
```

---

### Domain Task 26.3: Employee Event Tasarımı

#### Task Tanımı
Personel domain event'lerinin tasarlanması ve event handler'ların oluşturulması.

**Domain Events**
```csharp
/// <summary>
/// Personel oluşturuldu event.
/// Ne: Personel oluşturulduğunda tetiklenen event sınıfıdır.
/// Neden: Personel oluşturulduktan sonra yapılması gereken işlemleri tetiklemek için gereklidir.
/// Özelliği: EmployeeId, EmployeeNumber, FirstName, LastName, EmployeeType, DepartmentId, TenantId, CreatedAt içerir.
/// Kim Kullanacak: MediatR, EmployeeCreatedEventHandler.
/// Amaç: Event'in işlenmesi.
/// </summary>
public class EmployeeCreatedEvent : INotification
{
    public Guid EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public EmployeeType EmployeeType { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Personel istihdam sonlandırıldı event.
/// Ne: Personel işten çıkarıldığında veya emekli olduğunda tetiklenen event sınıfıdır.
/// Neden: Ertelenen işlemleri tetiklemek için gereklidir.
/// Kim Kullanacak: MediatR.
/// </summary>
public class EmployeeTerminatedEvent : INotification
{
    public Guid EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime TerminationDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
}

/// <summary>
/// Personel departman değişikliği event.
/// Ne: Personel departman değiştirdiğinde tetiklenen event sınıfıdır.
/// Kim Kullanacak: MediatR.
/// </summary>
public class EmployeeDepartmentChangedEvent : INotification
{
    public Guid EmployeeId { get; set; }
    public Guid OldDepartmentId { get; set; }
    public Guid NewDepartmentId { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid TenantId { get; set; }
}

/// <summary>
/// Personel oluşturuldu event handler.
/// Ne: EmployeeCreatedEvent'i işleyen handler sınıfıdır.
/// Neden: Personel oluşturulduktan sonra yapılması gereken işlemleri gerçekleştirmek için gereklidir.
/// Kim Kullanacak: MediatR.
/// </summary>
public class EmployeeCreatedEventHandler : INotificationHandler<EmployeeCreatedEvent>
{
    private readonly IUserService _userService;
    private readonly IEmployeeDocumentService _documentService;
    private readonly IAuditService _auditService;
    private readonly ILogger<EmployeeCreatedEventHandler> _logger;

    public EmployeeCreatedEventHandler(
        IUserService userService,
        IEmployeeDocumentService documentService,
        IAuditService auditService,
        ILogger<EmployeeCreatedEventHandler> logger)
    {
        _userService = userService;
        _documentService = documentService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(EmployeeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("EmployeeCreatedEvent received for EmployeeId: {EmployeeId}", 
            notification.EmployeeId);

        try
        {
            // Create system user for the employee
            await _userService.CreateUserFromEmployeeAsync(
                notification.EmployeeId,
                notification.TenantId,
                cancellationToken);

            // Log audit
            await _auditService.LogActionAsync(
                notification.TenantId,
                Guid.Empty,
                "EmployeeCreated",
                "Employee",
                notification.EmployeeId.ToString(),
                $"Employee created: {notification.FirstName} {notification.LastName}",
                cancellationToken);

            _logger.LogInformation("EmployeeCreatedEvent processed for EmployeeId: {EmployeeId}", 
                notification.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EmployeeCreatedEvent for EmployeeId: {EmployeeId}", 
                notification.EmployeeId);
        }
    }
}
```

---

## SPRINT 27: HR - Vardiya ve İzin Yönetimi

### Sprint 27 Hedefi
Personel vardiya planlaması, izin yönetimi ve devamsızlık takip sistemlerinin oluşturulması.

---

### Domain Task 27.1: Employee Shift Management

#### Task Tanımı
Personel vardiya yönetim sisteminin oluşturulması.

**EmployeeShift.cs**
```csharp
/// <summary>
/// Personel vardiya entity sınıfı.
/// Ne: Personel vardiya bilgilerini temsil eden entity sınıfıdır.
/// Neden: Vardiya planlaması ve takibi için gereklidir.
/// Özelliği: EmployeeId, ShiftType, StartTime, EndTime, Date, IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: ShiftService, ScheduleService.
/// Amaç: Vardiya verilerinin domain model olarak yönetilmesi.
/// </summary>
public class EmployeeShift : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public ShiftType ShiftType { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public DateTime Date { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsOvertime { get; private set; }
    public string? Notes { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private EmployeeShift() { }

    public static EmployeeShift Create(
        Guid employeeId,
        ShiftType shiftType,
        TimeSpan startTime,
        TimeSpan endTime,
        DateTime date,
        bool isOvertime,
        string? notes,
        Guid tenantId)
    {
        return new EmployeeShift
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ShiftType = shiftType,
            StartTime = startTime,
            EndTime = endTime,
            Date = date,
            IsActive = true,
            IsOvertime = isOvertime,
            Notes = notes,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public TimeSpan Duration => EndTime - StartTime;
    
    public bool IsNightShift => StartTime > EndTime || StartTime.Hours >= 22 || StartTime.Hours < 6;
}

public enum ShiftType
{
    Morning = 1,    // 08:00 - 16:00
    Afternoon = 2,  // 16:00 - 24:00
    Night = 3,      // 24:00 - 08:00
    Day = 4,        // 08:00 - 18:00
    Night24 = 5     // 24 saat
}
```

**ShiftService.cs**
```csharp
/// <summary>
/// Vardiya servisi.
/// Ne: Vardiya planlaması ve yönetimi yapan servis sınıfıdır.
/// Neden: Personel vardiya işlemlerinin yönetilmesi için gereklidir.
/// Kim Kullanacak: ShiftController.
/// Amaç: Vardiya işlemlerinin yönetilmesi.
/// </summary>
public class ShiftService
{
    private readonly IEmployeeShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMediator _mediator;

    public async Task<EmployeeShift> CreateShiftAsync(CreateShiftRequest request, Guid tenantId)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, tenantId);
        if (employee == null)
            throw new EmployeeNotFoundException(request.EmployeeId);

        // Check for overlapping shifts
        var existingShifts = await _shiftRepository.GetByEmployeeAndDateAsync(
            request.EmployeeId, request.Date, tenantId);

        foreach (var existing in existingShifts)
        {
            if (IsOverlapping(existing, request.StartTime, request.EndTime))
                throw new ShiftOverlapException("Shift overlaps with existing shift");
        }

        var shift = EmployeeShift.Create(
            request.EmployeeId,
            request.ShiftType,
            request.StartTime,
            request.EndTime,
            request.Date,
            request.IsOvertime,
            request.Notes,
            tenantId);

        await _shiftRepository.AddAsync(shift);
        await _shiftRepository.SaveChangesAsync();

        return shift;
    }

    public async Task<IEnumerable<EmployeeShift>> GetWeeklyScheduleAsync(
        Guid employeeId,
        DateTime weekStartDate,
        Guid tenantId)
    {
        var weekEndDate = weekStartDate.AddDays(7);
        
        return await _shiftRepository.GetByEmployeeAndDateRangeAsync(
            employeeId,
            weekStartDate,
            weekEndDate,
            tenantId);
    }

    public async Task<Dictionary<Guid, List<EmployeeShift>>> GetDepartmentScheduleAsync(
        Guid departmentId,
        DateTime date,
        Guid tenantId)
    {
        var employees = await _employeeRepository.GetByDepartmentAsync(departmentId, tenantId);
        var schedule = new Dictionary<Guid, List<EmployeeShift>>();

        foreach (var employee in employees)
        {
            var shifts = await _shiftRepository.GetByEmployeeAndDateAsync(
                employee.Id, date, tenantId);
            
            schedule[employee.Id] = shifts.ToList();
        }

        return schedule;
    }

    private bool IsOverlapping(EmployeeShift existing, TimeSpan newStart, TimeSpan newEnd)
    {
        return (newStart >= existing.StartTime && newStart < existing.EndTime) ||
               (newEnd > existing.StartTime && newEnd <= existing.EndTime) ||
               (newStart <= existing.StartTime && newEnd >= existing.EndTime);
    }
}
```

---

### Domain Task 27.2: Employee Leave Management

#### Task Tanımı
Personel izin yönetim sisteminin oluşturulması.

**EmployeeLeave.cs**
```csharp
/// <summary>
/// Personel izin entity sınıfı.
/// Ne: Personel izin bilgilerini temsil eden entity sınıfıdır.
/// Neden: İzin takibi ve onay süreçleri için gereklidir.
/// Özelliği: EmployeeId, LeaveType, StartDate, EndDate, TotalDays, Reason, Status, 
///           ApprovedBy, ApprovedAt, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: LeaveService, ApprovalWorkflow.
/// Amaç: İzin verilerinin domain model olarak yönetilmesi.
/// </summary>
public class EmployeeLeave : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public LeaveType LeaveType { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int TotalDays { get; private set; }
    public string? Reason { get; private set; }
    public LeaveStatus Status { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private EmployeeLeave() { }

    public static EmployeeLeave Create(
        Guid employeeId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string? reason,
        Guid tenantId,
        Guid createdBy)
    {
        var totalDays = (int)(endDate - startDate).TotalDays + 1;

        return new EmployeeLeave
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveType = leaveType,
            StartDate = startDate,
            EndDate = endDate,
            TotalDays = totalDays,
            Reason = reason,
            Status = LeaveStatus.Pending,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(Guid approvedBy)
    {
        Status = LeaveStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Reject(Guid rejectedBy, string reason)
    {
        Status = LeaveStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        RejectionReason = reason;
    }

    public void Cancel()
    {
        Status = LeaveStatus.Cancelled;
    }
}

public enum LeaveType
{
    Annual = 1,          // Yıllık izin
    Sick = 2,            // Hastalık izni
    Maternity = 3,        // Doğum izni
    Paternity = 4,        // Babalık izni
    Unpaid = 5,           // Ücretsiz izin
    Excused = 6,          // Mazeret izni
    Compensatory = 7     // Düzeltme izni
}

public enum LeaveStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}
```

**LeaveBalance.cs**
```csharp
/// <summary>
/// İzin bakiyesi entity sınıfı.
/// Ne: Personel yıllık izin bakiyelerini temsil eden entity sınıfıdır.
/// Neden: İzin haklarının takibi için gereklidir.
/// Özelliği: EmployeeId, Year, TotalDays, UsedDays, RemainingDays, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: LeaveService.
/// Amaç: İzin bakiyesi verilerinin domain model olarak yönetilmesi.
/// </summary>
public class LeaveBalance : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public int Year { get; private set; }
    public int TotalDays { get; private set; }
    public int UsedDays { get; private set; }
    public int RemainingDays => TotalDays - UsedDays;
    public Guid TenantId { get; private set; }

    private LeaveBalance() { }

    public static LeaveBalance Create(Guid employeeId, int year, int totalDays, Guid tenantId)
    {
        return new LeaveBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Year = year,
            TotalDays = totalDays,
            UsedDays = 0,
            TenantId = tenantId
        };
    }

    public void UseDays(int days)
    {
        if (days > RemainingDays)
            throw new InsufficientLeaveBalanceException("Insufficient leave balance");
        
        UsedDays += days;
    }

    public void AddDays(int days)
    {
        TotalDays += days;
    }
}
```

---

## SPRINT 28: Quality - Kalite Modülü

### Sprint 28 Hedefi
Kalite yönetim sistemi, KPI takibi ve kalite metriklerinin oluşturulması.

---

### Domain Task 28.1: Quality Indicator ve KPI

#### Task Tanımı
Kalite göstergeleri ve KPI sistem yapısının oluşturulması.

**QualityIndicator.cs**
```csharp
/// <summary>
/// Kalite göstergesi entity sınıfı.
/// Ne: Hastane kalite göstergelerini temsil eden entity sınıfıdır.
/// Neden: Kalite takibi ve raporlaması için gereklidir.
/// Özelliği: IndicatorCode, Name, Description, Category, Unit, TargetValue, MinValue, MaxValue, 
///           Weight, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: QualityService, ReportService.
/// Amaç: Kalite göstergesi verilerinin domain model olarak yönetilmesi.
/// </summary>
public class QualityIndicator : BaseEntity
{
    public string IndicatorCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public QualityCategory Category { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal TargetValue { get; private set; }
    public decimal? MinValue { get; private set; }
    public decimal? MaxValue { get; private set; }
    public decimal Weight { get; private set; }
    public IndicatorType IndicatorType { get; private set; }
    public CalculationMethod CalculationMethod { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private QualityIndicator() { }

    public static QualityIndicator Create(
        string indicatorCode,
        string name,
        string? description,
        QualityCategory category,
        string unit,
        decimal targetValue,
        decimal? minValue,
        decimal? maxValue,
        decimal weight,
        IndicatorType indicatorType,
        CalculationMethod calculationMethod,
        Guid tenantId)
    {
        return new QualityIndicator
        {
            Id = Guid.NewGuid(),
            IndicatorCode = indicatorCode,
            Name = name,
            Description = description,
            Category = category,
            Unit = unit,
            TargetValue = targetValue,
            MinValue = minValue,
            MaxValue = maxValue,
            Weight = weight,
            IndicatorType = indicatorType,
            CalculationMethod = calculationMethod,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public IndicatorStatus GetStatus(decimal actualValue)
    {
        if (IndicatorType == IndicatorType.HigherIsBetter)
        {
            if (actualValue >= TargetValue) return IndicatorStatus.OnTarget;
            if (actualValue >= TargetValue * 0.8m) return IndicatorStatus.Warning;
            return IndicatorStatus.BelowTarget;
        }
        else
        {
            if (actualValue <= TargetValue) return IndicatorStatus.OnTarget;
            if (actualValue <= TargetValue * 1.2m) return IndicatorStatus.Warning;
            return IndicatorStatus.BelowTarget;
        }
    }
}

public enum QualityCategory
{
    PatientSafety = 1,       // Hasta güvenliği
    Clinical = 2,            // Klinik
    Operational = 3,         // Operasyonel
    PatientSatisfaction = 4, // Hasta memnuniyeti
    StaffSatisfaction = 5,   // Personel memnuniyeti
    Financial = 6            // Mali
}

public enum IndicatorType
{
    HigherIsBetter = 1,
    LowerIsBetter = 2
}

public enum CalculationMethod
{
    Sum = 1,
    Average = 2,
    Percentage = 3,
    Ratio = 4
}

public enum IndicatorStatus
{
    OnTarget = 1,
    Warning = 2,
    BelowTarget = 3
}
```

**QualityMeasurement.cs**
```csharp
/// <summary>
/// Kalite ölçümü entity sınıfı.
/// Ne: Kalite göstergesi ölçümlerini temsil eden entity sınıfıdır.
/// Neden: Kalite trend analizi için gereklidir.
/// Özelliği: IndicatorId, PeriodStart, PeriodEnd, ActualValue, CalculatedValue, Status, 
///           RecordedBy, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: QualityService.
/// Amaç: Kalite ölçüm verilerinin domain model olarak yönetilmesi.
/// </summary>
public class QualityMeasurement : BaseEntity
{
    public Guid IndicatorId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public decimal ActualValue { get; private set; }
    public decimal? CalculatedValue { get; private set; }
    public IndicatorStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public Guid RecordedBy { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private QualityMeasurement() { }

    public static QualityMeasurement Create(
        Guid indicatorId,
        DateTime periodStart,
        DateTime periodEnd,
        decimal actualValue,
        decimal? calculatedValue,
        IndicatorStatus status,
        string? notes,
        Guid recordedBy,
        Guid tenantId)
    {
        return new QualityMeasurement
        {
            Id = Guid.NewGuid(),
            IndicatorId = indicatorId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            ActualValue = actualValue,
            CalculatedValue = calculatedValue,
            Status = status,
            Notes = notes,
            RecordedBy = recordedBy,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

---

### Domain Task 28.2: Quality Report ve Dashboard

#### Task Tanımı
Kalite raporları ve dashboard verilerinin oluşturulması.

**QualityDashboardDto.cs**
```csharp
/// <summary>
/// Kalite dashboard DTO.
/// Ne: Kalite dashboard verilerini temsil eden DTO sınıfıdır.
/// Neden: Dashboard görüntüleme için gereklidir.
/// Özelliği: OverallScore, CategoryScores, TrendData, Alerts, Period özelliklerine sahiptir.
/// Kim Kullanacak: QualityController, Dashboard.
/// Amaç: Dashboard verilerinin transferi.
/// </summary>
public class QualityDashboardDto
{
    public decimal OverallScore { get; set; }
    public Dictionary<QualityCategory, CategoryScoreDto> CategoryScores { get; set; } = new();
    public List<TrendPointDto> TrendData { get; set; } = new();
    public List<QualityAlertDto> Alerts { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public string OverallStatus => OverallScore switch
    {
        >= 90 => "Mükemmel",
        >= 75 => "İyi",
        >= 60 => "Orta",
        _ => "Zayıf"
    };
}

public class CategoryScoreDto
{
    public QualityCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int IndicatorCount { get; set; }
    public int OnTargetCount { get; set; }
    public int WarningCount { get; set; }
    public int BelowTargetCount { get; set; }
}

public class TrendPointDto
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
}

public class QualityAlertDto
{
    public Guid IndicatorId { get; set; }
    public string IndicatorName { get; set; } = string.Empty;
    public decimal ActualValue { get; set; }
    public decimal TargetValue { get; set; }
    public IndicatorStatus Status { get; set; }
    public DateTime DetectedAt { get; set; }
}
```

---

## SPRINT 29: Document - Doküman Yönetimi

### Sprint 29 Hedefi
Kurumsal doküman yönetim sistemi, sürüm kontrolü ve erişim yetkilendirmesi.

---

### Domain Task 29.1: Document Management

#### Task Tanımı
Doküman yönetim sisteminin oluşturulması.

**Document.cs**
```csharp
/// <summary>
/// Doküman entity sınıfı.
/// Ne: Kurumsal dokümanları temsil eden aggregate root sınıfıdır.
/// Neden: Doküman merkezi yönetimi ve sürüm kontrolü için gereklidir.
/// Özelliği: DocumentNumber, Title, Description, DocumentType, Category, FileUrl, FileSize, 
///           FileHash, CurrentVersion, Status, TenantId, CreatedBy, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: DocumentRepository, DocumentService.
/// Amaç: Doküman verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Document : BaseEntity, IAuditTEntity
{
    public string DocumentNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public Guid CategoryId { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string FileHash { get; private set; } = string.Empty;
    public string FileExtension { get; private set; } = string.Empty;
    public int CurrentVersion { get; private set; }
    public DocumentStatus Status { get; private set; }
    public DateTime? EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation Properties
    public DocumentCategory Category { get; private set; } = null!;
    
    private readonly List<DocumentVersion> _versions = new();
    public IReadOnlyCollection<DocumentVersion> Versions => _versions.AsReadOnly();

    private readonly List<DocumentAccess> _accesses = new();
    public IReadOnlyCollection<DocumentAccess> Accesses => _accesses.AsReadOnly();

    private Document() { }

    public static Document Create(
        string documentNumber,
        string title,
        string? description,
        DocumentType documentType,
        Guid categoryId,
        string fileUrl,
        long fileSize,
        string fileHash,
        string fileExtension,
        DateTime? effectiveDate,
        DateTime? expiryDate,
        Guid tenantId,
        Guid createdBy)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DocumentNumber = documentNumber,
            Title = title,
            Description = description,
            DocumentType = documentType,
            CategoryId = categoryId,
            FileUrl = fileUrl,
            FileSize = fileSize,
            FileHash = fileHash,
            FileExtension = fileExtension,
            CurrentVersion = 1,
            Status = DocumentStatus.Draft,
            EffectiveDate = effectiveDate,
            ExpiryDate = expiryDate,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        return document;
    }

    public void Publish()
    {
        if (Status == DocumentStatus.Published)
            throw new InvalidOperationException("Document is already published");

        Status = DocumentStatus.Published;
        CurrentVersion = 1;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddVersion(DocumentVersion version)
    {
        _versions.Add(version);
        CurrentVersion = version.Version;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAccess(DocumentAccess access)
    {
        _accesses.Add(access);
    }

    public void Expire()
    {
        Status = DocumentStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.Today;
}

public enum DocumentType
{
    Policy = 1,              // Politika
    Procedure = 2,           // Prosedür
    WorkInstruction = 3,     // İş talimatı
    Form = 4,                // Form
    Record = 5,               // Kayıt
    Report = 6,               // Rapor
    Training = 7,            // Eğitim materyali
    Contract = 8              // Sözleşme
}

public enum DocumentStatus
{
    Draft = 1,
    PendingApproval = 2,
    Published = 3,
    Expired = 4,
    Archived = 5
}
```

**DocumentCategory.cs**
```csharp
/// <summary>
/// Doküman kategorisi entity sınıfı.
/// Ne: Doküman kategorilerini temsil eden entity sınıfıdır.
/// Neden: Doküman organizasyonu için gereklidir.
/// Özelliği: CategoryCode, Name, Description, ParentCategoryId, IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: Document aggregate.
/// Amaç: Kategori verilerinin domain model olarak yönetilmesi.
/// </summary>
public class DocumentCategory : BaseEntity
{
    public string CategoryCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }

    private DocumentCategory() { }

    public static DocumentCategory Create(
        string categoryCode,
        string name,
        string? description,
        Guid? parentCategoryId,
        Guid tenantId)
    {
        return new DocumentCategory
        {
            Id = Guid.NewGuid(),
            CategoryCode = categoryCode,
            Name = name,
            Description = description,
            ParentCategoryId = parentCategoryId,
            IsActive = true,
            TenantId = tenantId
        };
    }
}
```

---

## SPRINT 30: Notification - Bildirim Sistemi

### Sprint 30 Hedefi
Çok kanallı bildirim sistemi, template yönetimi ve bildirim kanal entegrasyonları.

---

### Domain Task 30.1: Notification System

#### Task Tanımı
Bildirim sisteminin oluşturulması ve kanal entegrasyonları.

**Notification.cs**
```csharp
/// <summary>
/// Bildirim entity sınıfı.
/// Ne: Sistem bildirimlerini temsil eden aggregate root sınıfıdır.
/// Neden: Bildirim merkezi yönetimi ve takibi için gereklidir.
/// Özelliği: NotificationNumber, Title, Body, NotificationType, Priority, Channels, Recipients, 
///           Status, ScheduledAt, SentAt, TenantId, CreatedBy, CreatedAt özelliklerine sahiptir.
/// Kim Kullanacak: NotificationRepository, NotificationService.
/// Amaç: Bildirim verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Notification : BaseEntity
{
    public string NotificationNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationType NotificationType { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public int SentCount { get; private set; }
    public int FailedCount { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Notification Channels
    private readonly List<NotificationChannel> _channels = new();
    public IReadOnlyCollection<NotificationChannel> Channels => _channels.AsReadOnly();

    // Recipients
    private readonly List<NotificationRecipient> _recipients = new();
    public IReadOnlyCollection<NotificationRecipient> Recipients => _recipients.AsReadOnly();

    // Delivery Results
    private readonly List<NotificationDelivery> _deliveries = new();
    public IReadOnlyCollection<NotificationDelivery> Deliveries => _deliveries.AsReadOnly();

    private Notification() { }

    public static Notification Create(
        string title,
        string body,
        NotificationType notificationType,
        NotificationPriority priority,
        Guid tenantId,
        Guid createdBy,
        DateTime? scheduledAt = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            NotificationNumber = GenerateNotificationNumber(),
            Title = title,
            Body = body,
            NotificationType = notificationType,
            Priority = priority,
            Status = scheduledAt.HasValue ? NotificationStatus.Scheduled : NotificationStatus.Pending,
            ScheduledAt = scheduledAt,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddChannel(NotificationChannel channel)
    {
        _channels.Add(channel);
    }

    public void AddRecipient(NotificationRecipient recipient)
    {
        _recipients.Add(recipient);
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void RecordDelivery(NotificationDelivery delivery)
    {
        _deliveries.Add(delivery);
        
        if (delivery.Status == DeliveryStatus.Delivered)
            SentCount++;
        else
            FailedCount++;
    }

    private static string GenerateNotificationNumber()
    {
        return $"NOT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

public enum NotificationType
{
    Appointment = 1,
    Invoice = 2,
    LabResult = 3,
    Prescription = 4,
    System = 5,
    Alert = 6,
    Reminder = 7,
    Marketing = 8
}

public enum NotificationPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4
}

public enum NotificationStatus
{
    Pending = 1,
    Scheduled = 2,
    Sending = 3,
    Sent = 4,
    Failed = 5,
    Cancelled = 6
}
```

**NotificationChannel.cs**
```csharp
/// <summary>
/// Bildirim kanalı entity sınıfı.
/// Ne: Bildirim kanallarını temsil eden entity sınıfıdır.
/// Neden: Çoklu kanal desteği için gereklidir.
/// Özelliği: Channel, IsEnabled, Configuration özelliklerine sahiptir.
/// Kim Kullanacak: Notification aggregate.
/// Amaç: Kanal verilerinin domain model olarak yönetilmesi.
/// </summary>
public class NotificationChannel : BaseEntity
{
    public Guid NotificationId { get; private set; }
    public ChannelType Channel { get; private set; }
    public bool IsEnabled { get; private set; }
    public string? Configuration { get; private set; }

    private NotificationChannel() { }

    public static NotificationChannel Create(
        Guid notificationId,
        ChannelType channel,
        string? configuration = null)
    {
        return new NotificationChannel
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            Channel = channel,
            IsEnabled = true,
            Configuration = configuration
        };
    }
}

public enum ChannelType
{
    Email = 1,
    SMS = 2,
    Push = 3,
    InApp = 4,
    WhatsApp = 5
}
```

**NotificationTemplate.cs**
```csharp
/// <summary>
/// Bildirim şablonu entity sınıfı.
/// Ne: Bildirim şablonlarını temsil eden entity sınıfıdır.
/// Neden: Tekrarlayan bildirimler için şablon kullanımı için gereklidir.
/// Özelliği: TemplateCode, Name, Channel, Subject, Body, Variables, IsActive, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: NotificationService.
/// Amaç: Şablon verilerinin domain model olarak yönetilmesi.
/// </summary>
public class NotificationTemplate : BaseEntity
{
    public string TemplateCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ChannelType Channel { get; private set; }
    public string? Subject { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public string Variables { get; private set; } = string.Empty; // JSON
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private NotificationTemplate() { }

    public static NotificationTemplate Create(
        string templateCode,
        string name,
        ChannelType channel,
        string? subject,
        string body,
        string variables,
        Guid tenantId)
    {
        return new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            TemplateCode = templateCode,
            Name = name,
            Channel = channel,
            Subject = subject,
            Body = body,
            Variables = variables,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string RenderBody(Dictionary<string, string> values)
    {
        var result = Body;
        foreach (var (key, value) in values)
        {
            result = result.Replace($"{{{key}}}", value);
        }
        return result;
    }
}
```

---

### Domain Task 30.2: Notification Service

#### Task Tanımı
Bildirim gönderme servisinin oluşturulması.

**INotificationService.cs**
```csharp
/// <summary>
/// Bildirim servisi arayüzü.
/// Ne: Bildirim gönderme işlemlerini yöneten servis arayüzüdür.
/// Neden: Çoklu kanal desteği ve template rendering için gereklidir.
/// Özelliği: SendNotification, SendTemplateNotification, SendBulkNotification metotlarını içerir.
/// Kim Kullanacak: Command handlers, Background jobs.
/// Amaç: Bildirim işlemlerinin yönetilmesi.
/// </summary>
public interface INotificationService
{
    Task<Notification> SendNotificationAsync(
        string title,
        string body,
        NotificationType type,
        NotificationPriority priority,
        List<NotificationRecipientInfo> recipients,
        List<ChannelType> channels,
        Guid tenantId,
        Guid createdBy,
        DateTime? scheduledAt = null);

    Task<Notification> SendTemplateNotificationAsync(
        string templateCode,
        Dictionary<string, string> variables,
        List<NotificationRecipientInfo> recipients,
        List<ChannelType> channels,
        Guid tenantId,
        Guid createdBy,
        DateTime? scheduledAt = null);

    Task<Notification> SendBulkNotificationAsync(
        string title,
        string body,
        NotificationType type,
        List<NotificationRecipientInfo> recipients,
        ChannelType channel,
        Guid tenantId,
        Guid createdBy);
}
```

**NotificationService.cs**
```csharp
/// <summary>
/// Bildirim servisi implementasyonu.
/// Ne: Çok kanallı bildirim gönderme işlemlerini yöneten servis sınıfıdır.
/// Neden: Email, SMS, Push, InApp kanalları için ayrı adapter'lar kullanılır.
/// Özelliği: Template rendering, rate limiting, delivery tracking.
/// Kim Kullanacak: Command handlers, Background jobs.
/// Amaç: Bildirim işlemlerinin yönetilmesi.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IEmailAdapter _emailAdapter;
    private readonly ISmsAdapter _smsAdapter;
    private readonly IPushAdapter _pushAdapter;
    private readonly IInAppAdapter _inAppAdapter;
    private readonly ILogger<NotificationService> _logger;

    public async Task<Notification> SendNotificationAsync(
        string title,
        string body,
        NotificationType type,
        NotificationPriority priority,
        List<NotificationRecipientInfo> recipients,
        List<ChannelType> channels,
        Guid tenantId,
        Guid createdBy,
        DateTime? scheduledAt = null)
    {
        var notification = Notification.Create(
            title,
            body,
            type,
            priority,
            tenantId,
            createdBy,
            scheduledAt);

        // Add channels
        foreach (var channel in channels)
        {
            notification.AddChannel(NotificationChannel.Create(notification.Id, channel));
        }

        // Add recipients
        foreach (var recipient in recipients)
        {
            notification.AddRecipient(NotificationRecipient.Create(
                notification.Id,
                recipient.RecipientId,
                recipient.RecipientType,
                recipient.Contact,
                recipient.Language));
        }

        await _notificationRepository.AddAsync(notification);

        // If not scheduled, send immediately
        if (!scheduledAt.HasValue)
        {
            await SendAsync(notification, channels);
        }

        await _notificationRepository.SaveChangesAsync();

        return notification;
    }

    public async Task<Notification> SendTemplateNotificationAsync(
        string templateCode,
        Dictionary<string, string> variables,
        List<NotificationRecipientInfo> recipients,
        List<ChannelType> channels,
        Guid tenantId,
        Guid createdBy,
        DateTime? scheduledAt = null)
    {
        var template = await _templateRepository.GetByCodeAsync(templateCode, tenantId);
        if (template == null)
            throw new TemplateNotFoundException(templateCode);

        var renderedBody = template.RenderBody(variables);
        var subject = template.Subject;

        if (!string.IsNullOrEmpty(subject))
        {
            var renderedSubject = subject;
            foreach (var (key, value) in variables)
            {
                renderedSubject = renderedSubject.Replace($"{{{key}}}", value);
            }
            subject = renderedSubject;
        }

        return await SendNotificationAsync(
            template.Name,
            renderedBody,
            NotificationType.System,
            NotificationPriority.Normal,
            recipients,
            channels,
            tenantId,
            createdBy,
            scheduledAt);
    }

    private async Task SendAsync(Notification notification, List<ChannelType> channels)
    {
        notification.MarkAsSent();

        foreach (var recipient in notification.Recipients)
        {
            foreach (var channel in channels)
            {
                var delivery = await SendToChannelAsync(notification, recipient, channel);
                notification.RecordDelivery(delivery);
            }
        }
    }

    private async Task<NotificationDelivery> SendToChannelAsync(
        Notification notification,
        NotificationRecipient recipient,
        ChannelType channel)
    {
        try
        {
            var adapter = GetChannelAdapter(channel);
            var result = await adapter.SendAsync(notification.Title, notification.Body, recipient.Contact);

            return NotificationDelivery.Create(
                notification.Id,
                recipient.Id,
                channel,
                result.Success ? DeliveryStatus.Delivered : DeliveryStatus.Failed,
                result.MessageId,
                result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification via {Channel}", channel);
            
            return NotificationDelivery.Create(
                notification.Id,
                recipient.Id,
                channel,
                DeliveryStatus.Failed,
                null,
                ex.Message);
        }
    }

    private INotificationAdapter GetChannelAdapter(ChannelType channel)
    {
        return channel switch
        {
            ChannelType.Email => _emailAdapter,
            ChannelType.SMS => _smsAdapter,
            ChannelType.Push => _pushAdapter,
            ChannelType.InApp => _inAppAdapter,
            _ => throw new NotSupportedException($"Channel {channel} is not supported")
        };
    }
}

/// <summary>
/// Bildirim adapter arayüzü.
/// Ne: Kanal-spesifik gönderim adapter'ları için arayüz.
/// Kim Kullanacak: NotificationService.
/// </summary>
public interface INotificationAdapter
{
    Task<SendResult> SendAsync(string title, string body, string contact);
}

public class SendResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
}
```

---

## 📊 Sprint 30 Tamamlandığında Beklenen Çıktılar

### Employee Management
- [x] Employee entity ve repository
- [x] Department ve Position yapısı
- [x] Organizasyon şeması sorgulamaları
- [x] Employee domain events

### Shift & Leave Management
- [x] EmployeeShift entity ve CRUD
- [x] EmployeeLeave entity ve onay süreci
- [x] LeaveBalance takibi

### Quality Management
- [x] QualityIndicator entity
- [x] QualityMeasurement kayıtları
- [x] Dashboard DTO'ları

### Document Management
- [x] Document entity ve sürüm kontrolü
- [x] DocumentCategory yapısı
- [x] DocumentAccess yetkilendirmesi

### Notification System
- [x] Notification entity
- [x] NotificationTemplate sistemi
- [x] Çok kanallı adapter yapısı (Email, SMS, Push, InApp)

---

## 📋 Sonraki Adımlar

FAZ 5 tamamlandıktan sonra FAZ 6'ya geçiş yapılacaktır:

### FAZ 6 İçerik:
- **Sprint 31-32**: Integration (Entegrasyon) - HL7, FHIR, PACS, LIS entegrasyonları
- **Sprint 33-34**: Monitoring (İzleme) - Health checks, metrics, alerting
- **Sprint 35**: Data Warehouse - Veri ambarı altyapısı
- **Sprint 36**: API Gateway & Multi-Hospital Orchestration