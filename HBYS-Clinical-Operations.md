# HBYS Clinical Operations
## Radiology Workflow, Operation Theatre, Blood Bank, MPI, and Insurance Claims

---

# PART 1: RADIOLOGY WORKFLOW

## 1.1 Radiology Request and Imaging

```csharp
/// <summary>
/// Radiology request entity.
/// Ne: Radyoloji istek entity.
/// </summary>
public class RadiologyRequest : BaseEntity
{
    public string RequestNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid PhysicianId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid ModalityId { get; private set; }
    public string StudyType { get; private set; } = string.Empty;
    public string ClinicalHistory { get; private set; } = string.Empty;
    public string Findings { get; private set; } = string.Empty;
    public string Impression { get; private set; } = string.Empty;
    public RadiologyRequestStatus Status { get; private set; }
    public string? PacsStudyInstanceUid { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime? StudyDate { get; private set; }
    public DateTime? ReportedDate { get; private set; }
    public Guid? ReportedBy { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<RadiologyAttachment> _attachments = new();
    public IReadOnlyCollection<RadiologyAttachment> Attachments => _attachments.AsReadOnly();
    
    public static RadiologyRequest Create(
        Guid patientId,
        Guid physicianId,
        Guid departmentId,
        Guid modalityId,
        string studyType,
        string clinicalHistory,
        Guid tenantId,
        Guid createdBy)
    {
        return new RadiologyRequest
        {
            Id = Guid.NewGuid(),
            RequestNumber = GenerateRequestNumber(),
            PatientId = patientId,
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            ModalityId = modalityId,
            StudyType = studyType,
            ClinicalHistory = clinicalHistory,
            Status = RadiologyRequestStatus.Requested,
            RequestDate = DateTime.UtcNow,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void AssignToModality(Guid modalityId)
    {
        ModalityId = modalityId;
        Status = RadiologyRequestStatus.Scheduled;
    }
    
    public void StartStudy(DateTime studyDate)
    {
        StudyDate = studyDate;
        Status = RadiologyRequestStatus.InProgress;
    }
    
    public void CompleteStudy(string pacsStudyUid)
    {
        PacsStudyInstanceUid = pacsStudyUid;
        Status = RadiologyRequestStatus.Completed;
    }
    
    public void SubmitReport(string findings, string impression, Guid radiologistId)
    {
        Findings = findings;
        Impression = impression;
        ReportedBy = radiologistId;
        ReportedDate = DateTime.UtcNow;
        Status = RadiologyRequestStatus.Reported;
    }
    
    private static string GenerateRequestNumber()
    {
        return $"RAD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}

/// <summary>
/// Modality entity.
/// Ne: Cihaz entity.
/// </summary>
public class Modality : BaseEntity
{
    public string ModalityCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ModalityType Type { get; private set; }
    public string Manufacturer { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string AeTitle { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public int Port { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxDailyStudies { get; private set; }
    public Guid TenantId { get; private set; }
}
```

---

# PART 2: OPERATION THEATRE MANAGEMENT

## 2.1 Surgery Scheduling

```csharp
/// <summary>
/// Surgery request entity.
/// Ne: Cerrahi istek entity.
/// </summary>
public class SurgeryRequest : BaseEntity
{
    public string SurgeryNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid SurgeonId { get; private set; }
    public Guid AnesthetistId { get; private set; }
    public Guid OperatingRoomId { get; private set; }
    public string SurgeryType { get; private set; } = string.Empty;
    public string Diagnosis { get; private set; } = string.Empty;
    public string PlannedProcedure { get; private set; } = string.Empty;
    public SurgeryStatus Status { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public string? PostOpDiagnosis { get; private set; }
    public string? OperativeNotes { get; private set; }
    public AnesthesiaType AnesthesiaType { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<SurgeryTeamMember> _teamMembers = new();
    public IReadOnlyCollection<SurgeryTeamMember> TeamMembers => _teamMembers.AsReadOnly();
    
    private readonly List<SurgerySupply> _supplies = new();
    public IReadOnlyCollection<SurgerySupply> Supplies => _supplies.AsReadOnly();
    
    public void Schedule(DateTime date, TimeSpan duration, Guid operatingRoomId)
    {
        ScheduledDate = date;
        EstimatedDuration = duration;
        OperatingRoomId = operatingRoomId;
        Status = SurgeryStatus.Scheduled;
    }
    
    public void Start()
    {
        ActualStartTime = DateTime.UtcNow;
        Status = SurgeryStatus.InProgress;
    }
    
    public void Complete(string postOpDiagnosis, string operativeNotes)
    {
        ActualEndTime = DateTime.UtcNow;
        PostOpDiagnosis = postOpDiagnosis;
        OperativeNotes = operativeNotes;
        Status = SurgeryStatus.Completed;
    }
    
    public void AddTeamMember(Guid employeeId, string role)
    {
        _teamMembers.Add(SurgeryTeamMember.Create(Id, employeeId, role));
    }
}

/// <summary>
/// Operating room entity.
/// Ne: Ameliyathane entity.
/// </summary>
public class OperatingRoom : BaseEntity
{
    public string RoomNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Floor { get; private set; }
    public OperatingRoomType Type { get; private set; }
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAvailable { get; private set; }
    public Guid TenantId { get; private set; }
    
    private readonly List<OperatingRoomEquipment> _equipment = new();
    public IReadOnlyCollection<OperatingRoomEquipment> Equipment => _equipment.AsReadOnly();
}
```

---

# PART 3: BLOOD BANK MANAGEMENT

## 3.1 Blood Inventory

```csharp
/// <summary>
/// Blood unit entity.
/// Ne: Kan ünitesi entity.
/// </summary>
public class BloodUnit : BaseEntity
{
    public string UnitNumber { get; private set; } = string.Empty;
    public string BagBarcode { get; private set; } = string.Empty;
    public BloodType BloodType { get; private set; }
    public BloodComponent Component { get; private set; }
    public double Volume { get; private set; } // mL
    public DateTime CollectionDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public BloodUnitStatus Status { get; private set; }
    public string DonorName { get; private set; } = string.Empty;
    public string DonorId { get; private set; } = string.Empty;
    public Guid? CrossMatchPatientId { get; private set; }
    public DateTime? TransfusionDate { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsAvailable => Status == BloodUnitStatus.Available && !IsExpired;
    
    public void CrossMatch(Guid patientId)
    {
        CrossMatchPatientId = patientId;
        Status = BloodUnitStatus.CrossMatched;
    }
    
    public void Transfuse()
    {
        Status = BloodUnitStatus.Transfused;
        TransfusionDate = DateTime.UtcNow;
    }
    
    public void Discard(string reason)
    {
        Status = BloodUnitStatus.Discarded;
    }
}

/// <summary>
/// Blood request entity.
/// Ne: Kan istemi entity.
/// </summary>
public class BloodRequest : BaseEntity
{
    public string RequestNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid PhysicianId { get; private set; }
    public BloodType RequiredBloodType { get; private set; }
    public BloodComponent RequiredComponent { get; private set; }
    public int UnitsRequired { get; private set; }
    public int UnitsIssued { get; private set; }
    public string Urgency { get; private set; } // Routine, Urgent, Emergency
    public string ClinicalIndication { get; private set; } = string.Empty;
    public DateTime RequiredDate { get; private set; }
    public BloodRequestStatus Status { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<BloodUnit> _crossMatchedUnits = new();
    public IReadOnlyCollection<BloodUnit> CrossMatchedUnits => _crossMatchedUnits.AsReadOnly();
}
```

---

# PART 4: MASTER PATIENT INDEX (MPI)

## 4.1 Patient Matching

```csharp
/// <summary>
/// Master patient index service.
/// Ne: Ana hasta indeksi servisi.
/// Neden: Çoklu kayıt eşleştirme ve duplicate detection için gereklidir.
/// </summary>
public interface IMpiService
{
    Task<MpiSearchResult> SearchAsync(MpiSearchCriteria criteria);
    Task<MpiMatchResult> FindPotentialDuplicatesAsync(Guid patientId);
    Task MergePatientsAsync(Guid primaryPatientId, Guid secondaryPatientId, Guid performedBy);
}

/// <summary>
/// MPI search criteria.
/// Ne: MPI arama kriterleri.
/// </summary>
public class MpiSearchCriteria
{
    public string? TurkishId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int MatchThreshold { get; set; } = 80;
}

/// <summary>
/// MPI match score.
/// Ne: MPI eşleşme skoru.
/// </summary>
public class MpiMatchScore
{
    public Guid PatientId { get; set; }
    public double MatchScore { get; set; }
    public Dictionary<string, double> FieldScores { get; set; } = new();
    public bool IsMatch => MatchScore >= 80;
    public string MatchReason { get; set; } = string.Empty;
}

/// <summary>
/// Patient demographics for matching.
/// Ne: Eşleştirme için hasta demografik verileri.
/// </summary>
public class PatientDemographics
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string TurkishId { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
}
```

---

# PART 5: INSURANCE CLAIMS PROCESSING

## 5.1 Insurance Claim Workflow

```csharp
/// <summary>
/// Insurance claim entity.
/// Ne: Sigorta claim entity.
/// </summary>
public class InsuranceClaim : BaseEntity
{
    public string ClaimNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid InsurancePolicyId { get; private set; }
    public string InsuranceProvider { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public decimal ApprovedAmount { get; private set; }
    public decimal PatientResponsibility { get; private set; }
    public ClaimStatus Status { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public DateTime ClaimDate { get; private set; }
    public DateTime? SubmissionDate { get; private set; }
    public DateTime? ResponseDate { get; private set; }
    public string? ResponseCode { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ApprovalNumber { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<ClaimItem> _items = new();
    public IReadOnlyCollection<ClaimItem> Items => _items.AsReadOnly();
    
    public void Submit()
    {
        Status = ClaimStatus.Submitted;
        SubmissionDate = DateTime.UtcNow;
    }
    
    public void ProcessClaim(string responseCode, decimal approvedAmount)
    {
        ResponseCode = responseCode;
        ApprovedAmount = approvedAmount;
        PatientResponsibility = TotalAmount - approvedAmount;
        
        Status = responseCode == "00" ? ClaimStatus.Approved : ClaimStatus.Rejected;
        ResponseDate = DateTime.UtcNow;
        
        if (responseCode == "00")
        {
            ApprovalNumber = GenerateApprovalNumber();
        }
    }
    
    public void Reject(string reason)
    {
        Status = ClaimStatus.Rejected;
        RejectionReason = reason;
        ResponseDate = DateTime.UtcNow;
    }
    
    private static string GenerateApprovalNumber()
    {
        return $"APR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

/// <summary>
/// Claim item.
/// Ne: Claim kalemi.
/// </summary>
public class ClaimItem : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public string ServiceCode { get; private set; } = string.Empty;
    public string ServiceName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal ApprovedPrice { get; private set; }
    public string? RejectionReason { get; private set; }
}
```

---

# PART 6: DATA EXPORT/IMPORT

## 6.1 Export Service

```csharp
/// <summary>
/// Data export service.
/// Ne: Veri export servisi.
/// </summary>
public interface IExportService
{
    Task<byte[]> ExportPatientsAsync(PatientExportFilter filter, ExportFormat format);
    Task<byte[]> ExportAppointmentsAsync(AppointmentExportFilter filter, ExportFormat format);
    Task<byte[]> ExportInvoicesAsync(InvoiceExportFilter filter, ExportFormat format);
    Task<ImportResult> ImportPatientsAsync(Stream file, ImportOptions options);
}

/// <summary>
/// Export format types.
/// Ne: Export format tipleri.
/// </summary>
public enum ExportFormat
{
    CSV = 1,
    Excel = 2,
    PDF = 3,
    JSON = 4,
    XML = 5
}

/// <summary>
/// Patient export filter.
/// Ne: Hasta export filtresi.
/// </summary>
public class PatientExportFilter
{
    public string? SearchTerm { get; set; }
    public PatientStatus? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public ExportFormat Format { get; set; }
    public List<string> Columns { get; set; } = new()
    {
        "PatientNumber", "FirstName", "LastName", "DateOfBirth", "Gender", "Phone", "Email"
    };
}

/// <summary>
/// CSV export service.
/// Ne: CSV export servisi.
/// </summary>
public class CsvExportService : IExportService
{
    public async Task<byte[]> ExportPatientsAsync(PatientExportFilter filter, ExportFormat format)
    {
        var patients = await _patientRepository.SearchAsync(
            filter.TenantId,
            filter.SearchTerm,
            filter.Status,
            filter.CreatedFrom,
            filter.CreatedTo,
            1,
            int.MaxValue);
        
        var sb = new StringBuilder();
        
        // Header
        sb.AppendLine(string.Join(",", filter.Columns));
        
        // Data rows
        foreach (var patient in patients.Items)
        {
            var row = filter.Columns.Select(col => GetPatientColumnValue(patient, col));
            sb.AppendLine(string.Join(",", row));
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
    
    private string GetPatientColumnValue(Patient patient, string column)
    {
        return column switch
        {
            "PatientNumber" => EscapeCsvValue(patient.PatientNumber),
            "FirstName" => EscapeCsvValue(patient.FirstName),
            "LastName" => EscapeCsvValue(patient.LastName),
            "DateOfBirth" => patient.DateOfBirth.ToString("yyyy-MM-dd"),
            "Gender" => patient.Gender.ToString(),
            "Phone" => EscapeCsvValue(patient.ContactInfos.FirstOrDefault(c => c.IsPrimary)?.Value ?? ""),
            "Email" => EscapeCsvValue(patient.ContactInfos.FirstOrDefault(c => c.ContactType == ContactType.Email)?.Value ?? ""),
            _ => ""
        };
    }
    
    private string EscapeCsvValue(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
```

---

# PART 7: CONSENT MANAGEMENT

## 7.1 Patient Consent

```csharp
/// <summary>
/// Patient consent entity.
/// Ne: Hasta onam entity.
/// </summary>
public class PatientConsent : BaseEntity
{
    public Guid PatientId { get; private set; }
    public ConsentType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsGranted { get; private set; }
    public DateTime ConsentDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public Guid GrantedBy { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public bool IsValid => IsGranted && (!ExpiryDate.HasValue || ExpiryDate > DateTime.UtcNow);
    
    public void Grant(DateTime expiryDate, Guid grantedBy, string ipAddress, string userAgent)
    {
        IsGranted = true;
        ConsentDate = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        GrantedBy = grantedBy;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
    
    public void Revoke()
    {
        IsGranted = false;
    }
}

/// <summary>
/// Consent types.
/// Ne: Onam tipleri.
/// </summary>
public enum ConsentType
{
    Treatment = 1,          // Tedavi onamı
    Surgery = 2,            // Cerrahi onamı
    BloodTransfusion = 3,  // Kan transfüzyonu onamı
    DataProcessing = 4,    // Veri işleme onamı
    Marketing = 5,         // Pazarlama onamı
    Research = 6,          // Araştırma onamı
    PhotoVideo = 7,        // Fotoğraf/video onamı
    DataSharing = 8        // Veri paylaşımı onamı
}
```

---

Bu dokümantasyon, HBYS sisteminin klinik operasyon detaylarını içermektedir. Radyoloji iş akışı, ameliyathane yönetimi, kan bankası, MPI (Master Patient Index), sigorta claim işleme, veri export/import, ve hasta onam yönetimi dahildir.