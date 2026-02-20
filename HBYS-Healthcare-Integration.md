# HBYS Healthcare Integration Details
## SGK/MEDULA, E-Prescription, Drug Interaction, UDI, and Clinical Decision Support

---

# PART 1: SGK/MEDULA INTEGRATION

## 1.1 MEDULA Service Implementation

```csharp
/// <summary>
/// MEDULA service interface.
/// Ne: SGK MEDULA entegrasyon servisi arayüzü.
/// Neden: Sosyal Güvenlik Kurumu ile ilaç ve tedavi takibi için gereklidir.
/// </summary>
public interface IMedulaService
{
    Task<MedulaResponse> SendPatientRegistrationAsync(MedulaPatient patient);
    Task<MedulaResponse> Send PrescriptionAsync(MedulaPrescription prescription);
    Task<MedulaResponse> UpdatePrescriptionStatusAsync(string protocolNumber, PrescriptionStatus status);
    Task<MedulaResponse> CheckInsuranceEligibilityAsync(string turkishId, string policyNumber);
}

/// <summary>
/// MEDULA patient registration request.
/// Ne: MEDULA hasta kayıt isteği.
/// </summary>
public class MedulaPatient
{
    public string TurkishId { get; set; } = string.Empty;
    public string SgkNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? BirthPlace { get; set; }
    public string? IdentitySerialNumber { get; set; }
    public string InsuranceType { get; set; } = string.Empty;
    public string ProvisionNumber { get; set; } = string.Empty;
    public DateTime ProvisionStartDate { get; set; }
    public DateTime ProvisionEndDate { get; set; }
}

/// <summary>
/// MEDULA prescription request.
/// Ne: MEDULA reçete isteği.
/// </summary>
public class MedulaPrescription
{
    public string PrescriptionNumber { get; set; } = string.Empty;
    public string ProtocolNumber { get; set; } = string.Empty;
    public string TurkishId { get; set; } = string.Empty;
    public string SgkNumber { get; set; } = string.Empty;
    public DateTime PrescriptionDate { get; set; }
    public string PrescriberTckNumber { get; set; } = string.Empty;
    public string PrescriberName { get; set; } = string.Empty;
    public string PrescriberSurname { get; set; } = string.Empty;
    public string PrescriptionType { get; set; } = string.Empty; // Normal, Repeat, Emergency
    public List<MedulaPrescriptionItem> Items { get; set; } = new();
}

/// <summary>
/// MEDULA prescription item.
/// Ne: MEDULA reçete kalemi.
/// </summary>
public class MedulaPrescriptionItem
{
    public string Barcode { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string Usage { get; set; } = string.Empty; // Kullanım şekli
    public int Quantity { get; set; }
    public string Dose { get; set; } = string.Empty; // Doz
    public string Frequency { get; set; } = string.Empty; // Sıklık
    public int Duration { get; set; } = 0; // Süre (gün)
    public string Description { get; set; } = string.Empty;
    public bool IsRefundable { get; set; }
}

/// <summary>
/// MEDULA service implementation.
/// Ne: MEDULA servisi implementasyonu.
/// </summary>
public class MedulaService : IMedulaService
{
    private readonly HttpClient _httpClient;
    private readonly IMedulaSettings _settings;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<MedulaService> _logger;
    
    public async Task<MedulaResponse> SendPatientRegistrationAsync(MedulaPatient patient)
    {
        var request = new MedulaRequest
        {
            ServiceName = "sgk_hasta_kayit",
            Parameters = new Dictionary<string, object>
            {
                ["tc_kimlik_no"] = patient.TurkishId,
                ["sgk_birim"] = patient.SgkNumber,
                ["adi"] = patient.FirstName,
                ["soyadi"] = patient.LastName,
                ["dogum_tarihi"] = patient.DateOfBirth.ToString("yyyyMMdd"),
                ["cinsiyet"] = patient.Gender == Gender.Male ? "E" : "K",
                ["sigortali_turu"] = patient.InsuranceType,
                ["provizyon_numarasi"] = patient.ProvisionNumber,
                ["provizyon_baslama_tarihi"] = patient.ProvisionStartDate.ToString("yyyyMMdd"),
                ["provizyon_bitis_tarihi"] = patient.ProvisionEndDate.ToString("yyyyMMdd")
            }
        };
        
        return await SendRequestAsync(request);
    }
    
    public async Task<MedulaResponse> SendPrescriptionAsync(MedulaPrescription prescription)
    {
        var items = prescription.Items.Select(i => new Dictionary<string, object>
        {
            ["barkod"] = i.Barcode,
            ["ilac_adi"] = i.DrugName,
            ["kullanim_sekli"] = i.Usage,
            ["adet"] = i.Quantity,
            ["doz"] = i.Dose,
            ["tekrar"] = i.Frequency,
            ["sure"] = i.Duration,
            ["aciklama"] = i.Description,
            ["iade_kontrol"] = i.IsRefundable ? "E" : "H"
        }).ToList();
        
        var request = new MedulaRequest
        {
            ServiceName = "sgk_recete_islem",
            Parameters = new Dictionary<string, object>
            {
                ["recete_numarasi"] = prescription.PrescriptionNumber,
                ["protokol_numarasi"] = prescription.ProtocolNumber,
                ["tc_kimlik_no"] = prescription.TurkishId,
                ["sgk_birim"] = prescription.SgkNumber,
                ["recete_tarihi"] = prescription.PrescriptionDate.ToString("yyyyMMdd"),
                ["doktor_tc"] = prescription.PrescriberTckNumber,
                ["doktor_adi"] = prescription.PrescriberName,
                ["doktor_soyadi"] = prescription.PrescriberSurname,
                ["recete_turu"] = prescription.PrescriptionType,
                ["ilaclar"] = items
            }
        };
        
        return await SendRequestAsync(request);
    }
    
    public async Task<MedulaResponse> CheckInsuranceEligibilityAsync(string turkishId, string policyNumber)
    {
        var request = new MedulaRequest
        {
            ServiceName = "sgk_sorgulama",
            Parameters = new Dictionary<string, object>
            {
                ["tc_kimlik_no"] = turkishId,
                ["sgk_birim"] = policyNumber,
                ["sorgulama_tarihi"] = DateTime.Now.ToString("yyyyMMddHHmmss")
            }
        };
        
        return await SendRequestAsync(request);
    }
    
    private async Task<MedulaResponse> SendRequestAsync(MedulaRequest request)
    {
        var endpoint = $"{_settings.BaseUrl}/{request.ServiceName}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Headers.Add("Authorization", $"Bearer {_settings.Token}");
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(request.Parameters),
            Encoding.UTF8,
            "application/json");
        
        var response = await _httpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("MEDULA request failed: {StatusCode}", response.StatusCode);
            return new MedulaResponse
            {
                Success = false,
                ErrorMessage = $"MEDULA service returned: {response.StatusCode}"
            };
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MedulaResponse>(content);
        
        return result ?? new MedulaResponse { Success = false };
    }
}
```

---

# PART 2: E-PRESCRIPTION SYSTEM

## 2.1 E-Prescription Service

```csharp
/// <summary>
/// E-Prescription service.
/// Ne: Elektronik reçete servisi.
/// Neden: E-reçete düzenleme ve takibi için gereklidir.
/// </summary>
public interface IEprescriptionService
{
    Task<EprescriptionResult> CreatePrescriptionAsync(CreatePrescriptionRequest request);
    Task<EprescriptionResult> UpdatePrescriptionAsync(Guid prescriptionId, UpdatePrescriptionRequest request);
    Task<EprescriptionResult> CancelPrescriptionAsync(Guid prescriptionId, string reason);
    Task<List<PrescriptionItem>> GetPrescriptionItemsAsync(Guid prescriptionId);
    Task<EprescriptionResult> CheckDrugInteractionAsync(List<string> drugBarcodes);
}

/// <summary>
/// E-Prescription entity.
/// Ne: E-Reçete entity.
/// </summary>
public class Eprescription : BaseEntity
{
    public string PrescriptionNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid PhysicianId { get; private set; }
    public DateTime PrescriptionDate { get; private set; }
    public PrescriptionType Type { get; private set; }
    public PrescriptionStatus Status { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Notes { get; private set; }
    public string? MedulaProtocolNumber { get; private set; }
    public DateTime? MedulaResponseDate { get; private set; }
    public string? MedulaResponseCode { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<PrescriptionItem> _items = new();
    public IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();
    
    public static Eprescription Create(
        Guid patientId,
        Guid physicianId,
        PrescriptionType type,
        string? diagnosis,
        string? notes,
        Guid tenantId,
        Guid createdBy)
    {
        return new Eprescription
        {
            Id = Guid.NewGuid(),
            PrescriptionNumber = GeneratePrescriptionNumber(),
            PatientId = patientId,
            PhysicianId = physicianId,
            PrescriptionDate = DateTime.UtcNow,
            Type = type,
            Status = PrescriptionStatus.Draft,
            Diagnosis = diagnosis,
            Notes = notes,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void AddItem(PrescriptionItem item)
    {
        _items.Add(item);
    }
    
    public void SubmitToMedula(string protocolNumber)
    {
        Status = PrescriptionStatus.Submitted;
        MedulaProtocolNumber = protocolNumber;
        MedulaResponseDate = DateTime.UtcNow;
    }
    
    public void Cancel(string reason)
    {
        Status = PrescriptionStatus.Cancelled;
        Notes = reason;
    }
    
    private static string GeneratePrescriptionNumber()
    {
        return $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

/// <summary>
/// Prescription item.
/// Ne: Reçete kalemi.
/// </summary>
public class PrescriptionItem : BaseEntity
{
    public Guid PrescriptionId { get; private set; }
    public Guid MedicationId { get; private set; }
    public string Barcode { get; private set; } = string.Empty;
    public string DrugName { get; private set; } = string.Empty;
    public string Usage { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Dose { get; private set; } = string.Empty;
    public string Frequency { get; private set; } = string.Empty;
    public int Duration { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsPaid { get; private set; }
    public decimal PatientPay { get; private set; }
    public decimal SgkPay { get; private set; }
    
    public static PrescriptionItem Create(
        Guid prescriptionId,
        Guid medicationId,
        string barcode,
        string drugName,
        string usage,
        int quantity,
        string dose,
        string frequency,
        int duration,
        string description)
    {
        return new PrescriptionItem
        {
            Id = Guid.NewGuid(),
            PrescriptionId = prescriptionId,
            MedicationId = medicationId,
            Barcode = barcode,
            DrugName = drugName,
            Usage = usage,
            Quantity = quantity,
            Dose = dose,
            Frequency = frequency,
            Duration = duration,
            Description = description
        };
    }
}
```

---

# PART 3: DRUG INTERACTION CHECKING

## 3.1 Drug Interaction Service

```csharp
/// <summary>
/// Drug interaction service.
/// Ne: İlaç etkileşim kontrol servisi.
/// Neden: İlaç güvenliği için gereklidir.
/// </summary>
public interface IDrugInteractionService
{
    Task<DrugInteractionResult> CheckInteractionsAsync(List<string> drugBarcodes);
    Task<DrugInteractionResult> CheckAllergiesAsync(Guid patientId, List<string> drugBarcodes);
    Task<List<DrugWarning>> GetDrugWarningsAsync(string drugBarcode);
}

/// <summary>
/// Drug interaction result.
/// Ne: İlaç etkileşim sonucu.
/// </summary>
public class DrugInteractionResult
{
    public bool HasInteraction { get; set; }
    public List<DrugInteraction> Interactions { get; set; } = new();
    public List<DrugAllergy> Allergies { get; set; } = new();
    public List<DrugWarning> Warnings { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Drug interaction.
/// Ne: İlaç etkileşimi.
/// </summary>
public class DrugInteraction
{
    public string Drug1Barcode { get; set; } = string.Empty;
    public string Drug1Name { get; set; } = string.Empty;
    public string Drug2Barcode { get; set; } = string.Empty;
    public string Drug2Name { get; set; } = string.Empty;
    public InteractionSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}

/// <summary>
/// Drug interaction database (reference data).
/// Ne: İlaç etkileşim veritabanı.
/// </summary>
public class DrugInteractionRule : BaseEntity
{
    public string Drug1Barcode { get; set; } = string.Empty;
    public string Drug2Barcode { get; set; } = string.Empty;
    public InteractionSeverity Severity { get; set; }
    public string Mechanism { get; set; } = string.Empty; // Etkileşim mekanizması
    public string ClinicalEffect { get; set; } = string.Empty; // Klinik etki
    public string Recommendation { get; set; } = string.Empty;
    public string EvidenceLevel { get; set; } = string.Empty; // Kanıt düzeyi (A, B, C, D, X)
}

/// <summary>
/// Interaction severity levels.
/// Ne: Etkileşim şiddet seviyeleri.
/// </summary>
public enum InteractionSeverity
{
    None = 0,
    Minor = 1,       // Hafif - dikkat
    Moderate = 2,    // Orta - izle
    Severe = 3,      // Şiddetli - dikkat
    Critical = 4     // Kritik - kaçın
}
```

---

# PART 4: UNIQUE DEVICE IDENTIFICATION (UDI)

## 4.1 UDI Tracking

```csharp
/// <summary>
/// Medical device entity.
/// Ne: Tıbbi cihaz entity.
/// Neden: Cihaz takibi ve izlenebilirlik için gereklidir.
/// </summary>
public class MedicalDevice : BaseEntity
{
    public string DeviceIdentifier { get; private set; } = string.Empty;
    public string Udi { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Manufacturer { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public DateTime ManufactureDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DeviceStatus Status { get; private set; }
    public Guid? CurrentPatientId { get; private set; }
    public Guid? CurrentLocationId { get; private set; }
    public Guid TenantId { get; private set; }
    
    public static MedicalDevice Create(
        string deviceIdentifier,
        string udi,
        string name,
        string manufacturer,
        string model,
        string serialNumber,
        DateTime manufactureDate,
        DateTime? expiryDate,
        Guid tenantId)
    {
        return new MedicalDevice
        {
            Id = Guid.NewGuid(),
            DeviceIdentifier = deviceIdentifier,
            Udi = udi,
            Name = name,
            Manufacturer = manufacturer,
            Model = model,
            SerialNumber = serialNumber,
            ManufactureDate = manufactureDate,
            ExpiryDate = expiryDate,
            Status = DeviceStatus.Available,
            TenantId = tenantId
        };
    }
    
    public void AssignToPatient(Guid patientId)
    {
        CurrentPatientId = patientId;
        Status = DeviceStatus.InUse;
    }
    
    public void ReturnToInventory()
    {
        CurrentPatientId = null;
        Status = DeviceStatus.Available;
    }
}

/// <summary>
/// Device tracking event.
/// Ne: Cihaz takip olayı.
/// </summary>
public class DeviceTrackingEvent : BaseEntity
{
    public Guid DeviceId { get; set; }
    public TrackingEventType EventType { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Notes { get; set; }
}
```

---

# PART 5: CLINICAL DECISION SUPPORT

## 5.1 CDS Rules Engine

```csharp
/// <summary>
/// Clinical decision support service.
/// Ne: Klinik karar destek servisi.
/// Neden: Hekimlere tedavi kararlarında yardımcı olmak için gereklidir.
/// </summary>
public interface ICdsService
{
    Task<List<CdsAlert>> EvaluatePatientAsync(Guid patientId, CdsContext context);
    Task<List<CdsRecommendation>> GetRecommendationsAsync(Guid patientId);
}

/// <summary>
/// CDS context.
/// Ne: CDS bağlamı.
/// </summary>
public class CdsContext
{
    public Guid PatientId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public List<string> CurrentMedications { get; set; } = new();
    public List<string> Allergies { get; set; } = new();
    public Dictionary<string, object> LabResults { get; set; } = new();
    public Dictionary<string, object> Vitals { get; set; } = new();
}

/// <summary>
/// CDS alert.
/// Ne: CDS uyarısı.
/// </summary>
public class CdsAlert
{
    public string RuleId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CdsSeverity Severity { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
}

/// <summary>
/// CDS rule.
/// Ne: CDS kuralı.
/// </summary>
public class CdsRule : BaseEntity
{
    public string RuleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // JSON format
    public string Action { get; set; } = string.Empty;
    public CdsSeverity Severity { get; set; }
    public bool IsActive { get; set; }
    public Guid TenantId { get; set; }
}

/// <summary>
/// Example CDS rule: Warfarin and NSAID interaction.
/// Ne: Örnek CDS kuralı: Warfarin ve NSAİ etkileşimi.
/// </summary>
public class WarfarinNsaidRule : CdsRule
{
    public static CdsRule Create()
    {
        return new CdsRule
        {
            Id = Guid.NewGuid(),
            RuleCode = "CDS-WARFARIN-NSAID",
            Name = "Warfarin-NSAİ Etkileşimi",
            Description = "Warfarin kullanan hastalarda NSAİ kullanımı kanama riskini artırır",
            Category = "Drug Interaction",
            Condition = @"
            {
                ""medication"": ""WARFARIN"",
                ""new_medication"": ""NSAID"",
                ""condition"": ""contains(currentMedications, 'WARFARIN') AND contains(newMedications, 'NSAID')""
            }",
            Action = @"
            {
                ""alert"": ""Warfarin ile NSAİ birlikte kullanımı kanama riski oluşturabilir"",
                ""recommendation"": ""Alternatif analjezik düşünülmeli veya PPI eklenmeli"",
                ""severity"": ""High""
            }",
            Severity = CdsSeverity.High,
            IsActive = true
        };
    }
}
```

---

# PART 6: LABORATORY INTERFACE (HL7 ORU)

## 6.1 Lab Result Integration

```csharp
/// <summary>
/// Lab result entity.
/// Ne: Laboratuvar sonuç entity.
/// </summary>
public class LabResult : BaseEntity
{
    public Guid RequestId { get; private set; }
    public string ResultNumber { get; private set; } = string.Empty;
    public DateTime ResultDate { get; private set; }
    public LabResultStatus Status { get; private set; }
    public string? ResultSummary { get; private set; }
    public bool IsAbnormal { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<LabResultItem> _items = new();
    public IReadOnlyCollection<LabResultItem> Items => _items.AsReadOnly();
    
    public static LabResult CreateFromHL7(HL7Message message, Guid requestId, Guid tenantId)
    {
        var result = new LabResult
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            ResultNumber = GenerateResultNumber(),
            ResultDate = DateTime.UtcNow,
            Status = LabResultStatus.Preliminary,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
        
        // Parse OBX segments for result items
        var obxSegments = message.Segments.Where(s => s.SegmentType == "OBX");
        
        foreach (var obx in obxSegments)
        {
            var item = LabResultItem.Create(
                result.Id,
                obx.GetField(3)?.Split('^').FirstOrDefault() ?? "", // Test code
                obx.GetField(3)?.Split('^').ElementAtOrDefault(1) ?? "", // Test name
                obx.GetField(5), // Value
                obx.GetField(6), // Unit
                obx.GetField(7), // Reference range
                obx.GetField(8) == "H" || obx.GetField(8) == "L", // Abnormal flag
                obx.GetField(14) // Date
            );
            
            result._items.Add(item);
        }
        
        result.IsAbnormal = result._items.Any(i => i.IsAbnormal);
        result.ResultSummary = result._items.Any(i => i.IsAbnormal) 
            ? "Abnormal results detected" 
            : "All results normal";
        
        return result;
    }
    
    private static string GenerateResultNumber()
    {
        return $"LAB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

/// <summary>
/// Lab result item.
/// Ne: Laboratuvar sonuç kalemi.
/// </summary>
public class LabResultItem : BaseEntity
{
    public Guid ResultId { get; private set; }
    public string TestCode { get; private set; } = string.Empty;
    public string TestName { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public string? ReferenceRange { get; private set; }
    public bool IsAbnormal { get; private set; }
    public string? AbnormalFlag { get; private set; }
    public DateTime? ResultDate { get; private set; }
    
    public static LabResultItem Create(
        Guid resultId,
        string testCode,
        string testName,
        string value,
        string? unit,
        string? referenceRange,
        bool isAbnormal,
        string? resultDate)
    {
        return new LabResultItem
        {
            Id = Guid.NewGuid(),
            ResultId = resultId,
            TestCode = testCode,
            TestName = testName,
            Value = value,
            Unit = unit,
            ReferenceRange = referenceRange,
            IsAbnormal = isAbnormal,
            AbnormalFlag = isAbnormal ? "A" : null,
            ResultDate = DateTime.TryParse(resultDate, out var date) ? date : null
        };
    }
}
```

---

Bu dokümantasyon, HBYS sisteminin sağlık entegrasyon detaylarını içermektedir. SGK/MEDULA entegrasyonu, e-reçete sistemi, ilaç etkileşim kontrolü, UDI takibi, klinik karar destek sistemi (CDS), ve laboratuvar arayüzü (HL7 ORU) dahildir.