# HBYS FAZ 6 - Sprint 31-36 Detaylı Domain Task Listesi

## Integration (Entegrasyon), Monitoring (İzleme), Data Warehouse, API Gateway ve Multi-Hospital Orchestration

---

## 📋 Genel Bakış

FAZ 6, HBYS sisteminin son ve en karmaşık fazıdır. Bu faz; harici sistem entegrasyonları, izleme altyapısı, veri ambarı, API gateway ve çoklu hastane orkestrasyonunu kapsamaktadır.

### FAZ 6 Hedefleri
- HL7, FHIR, PACS, LIS harici sistem entegrasyonları
- Uygulama izleme ve metrik toplama
- Veri ambarı altyapısı
- API Gateway ve yük dengeleme
- Çoklu hastane koordinasyonu

---

## SPRINT 31: Integration - HL7/FHIR Entegrasyonu

### Sprint 31 Hedefi
HL7 v2 ve FHIR standartlarına dayalı entegrasyon altyapısının kurulması.

---

### Domain Task 31.1: Integration Infrastructure

#### Task Tanımı
Entegrasyon altyapısının temel yapılarının oluşturulması.

**IntegrationEndpoint.cs**
```csharp
/// <summary>
/// Entegrasyon endpoint entity sınıfı.
/// Ne: Harici sistem bağlantı endpoint'lerini temsil eden entity sınıfıdır.
/// Neden: Entegrasyon noktalarının yönetimi ve izlenmesi için gereklidir.
/// Özelliği: EndpointCode, Name, IntegrationType, Protocol, Url, AuthenticationType, 
///           IsActive, MaxRetries, Timeout, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: IntegrationService, HL7Service, FHIRService.
/// Amaç: Entegrasyon endpoint verilerinin domain model olarak yönetilmesi.
/// </summary>
public class IntegrationEndpoint : BaseEntity
{
    public string EndpointCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public IntegrationType IntegrationType { get; private set; }
    public IntegrationProtocol Protocol { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public AuthenticationType AuthenticationType { get; private set; }
    public string? Username { get; private set; }
    public string? EncryptedPassword { get; private set; }
    public string? CertificateThumbprint { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxRetries { get; private set; } = 3;
    public int TimeoutSeconds { get; private set; } = 30;
    public bool IsHealthy { get; private set; }
    public DateTime? LastHealthCheck { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private IntegrationEndpoint() { }

    public static IntegrationEndpoint Create(
        string endpointCode,
        string name,
        string? description,
        IntegrationType integrationType,
        IntegrationProtocol protocol,
        string url,
        AuthenticationType authenticationType,
        string? username,
        string? encryptedPassword,
        Guid tenantId)
    {
        return new IntegrationEndpoint
        {
            Id = Guid.NewGuid(),
            EndpointCode = endpointCode,
            Name = name,
            Description = description,
            IntegrationType = integrationType,
            Protocol = protocol,
            Url = url,
            AuthenticationType = authenticationType,
            Username = username,
            EncryptedPassword = encryptedPassword,
            IsActive = true,
            MaxRetries = 3,
            TimeoutSeconds = 30,
            IsHealthy = false,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateHealthStatus(bool isHealthy)
    {
        IsHealthy = isHealthy;
        LastHealthCheck = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateUrl(string newUrl)
    {
        Url = newUrl;
    }
}

public enum IntegrationType
{
    HL7v2 = 1,
    FHIR = 2,
    PACS = 3,
    LIS = 4,
    RIS = 5,
    Pharmacy = 6,
    Insurance = 7,
    Government = 8
}

public enum IntegrationProtocol
{
    TCP = 1,
    HTTP = 2,
    HTTPS = 3,
    SFTP = 4,
    WebSocket = 5
}

public enum AuthenticationType
{
    None = 1,
    Basic = 2,
    Certificate = 3,
    OAuth2 = 4,
    APIKey = 5
}
```

**IntegrationMessage.cs**
```csharp
/// <summary>
/// Entegrasyon mesajı entity sınıfı.
/// Ne: Entegrasyon mesajlarını temsil eden aggregate root sınıfıdır.
/// Neden: Mesaj takibi ve yeniden deneme mekanizması için gereklidir.
/// Özelliği: MessageId, EndpointId, Direction, MessageType, Payload, Status, 
///           RetryCount, ErrorMessage, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: IntegrationService.
/// Amaç: Mesaj verilerinin domain model olarak yönetilmesi.
/// </summary>
public class IntegrationMessage : BaseEntity
{
    public string MessageId { get; private set; } = string.Empty;
    public Guid EndpointId { get; private set; }
    public MessageDirection Direction { get; private set; }
    public string MessageType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string? ResponsePayload { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ErrorStackTrace { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private IntegrationMessage() { }

    public static IntegrationMessage Create(
        Guid endpointId,
        MessageDirection direction,
        string messageType,
        string payload,
        Guid tenantId)
    {
        return new IntegrationMessage
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.NewGuid().ToString(),
            EndpointId = endpointId,
            Direction = direction,
            MessageType = messageType,
            Payload = payload,
            Status = IntegrationStatus.Pending,
            RetryCount = 0,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessing()
    {
        Status = IntegrationStatus.Processing;
    }

    public void MarkAsSuccess(string? responsePayload = null)
    {
        Status = IntegrationStatus.Success;
        ResponsePayload = responsePayload;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage, string? errorStackTrace = null)
    {
        Status = IntegrationStatus.Failed;
        ErrorMessage = errorMessage;
        ErrorStackTrace = errorStackTrace;
        ProcessedAt = DateTime.UtcNow;
    }

    public void IncrementRetry()
    {
        RetryCount++;
        Status = IntegrationStatus.Pending;
    }

    public bool CanRetry(int maxRetries)
    {
        return RetryCount < maxRetries;
    }
}

public enum MessageDirection
{
    Inbound = 1,
    Outbound = 2
}

public enum IntegrationStatus
{
    Pending = 1,
    Processing = 2,
    Success = 3,
    Failed = 4,
    Retrying = 5
}
```

---

### Domain Task 31.2: HL7 Service Implementation

#### Task Tanımı
HL7 v2.x mesajlarının işlenmesi için servis oluşturulması.

**HL7Message.cs**
```csharp
/// <summary>
/// HL7 mesaj sınıfı.
/// Ne: HL7 v2.x mesaj yapısını temsil eden sınıftır.
/// Neden: HL7 mesajlarının parse ve generate edilmesi için gereklidir.
/// Özelliği: Segments, MessageType, TriggerEvent, MessageControlId özelliklerine sahiptir.
/// Kim Kullanacak: HL7Service.
/// Amaç: HL7 mesaj işleme.
/// </summary>
public class HL7Message
{
    public List<HL7Segment> Segments { get; private set; } = new();
    public string MessageType { get; private set; } = string.Empty;
    public string TriggerEvent { get; private set; } = string.Empty;
    public string MessageControlId { get; private set; } = string.Empty;
    public DateTime? Timestamp { get; private set; }
    public string SendingApplication { get; private set; } = string.Empty;
    public string SendingFacility { get; private set; } = string.Empty;
    public string ReceivingApplication { get; private set; } = string.Empty;
    public string ReceivingFacility { get; private set; } = string.Empty;

    public static HL7Message Parse(string rawMessage)
    {
        var message = new HL7Message();
        var lines = rawMessage.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var segment = HL7Segment.Parse(line);
            message.Segments.Add(segment);

            // Parse MSH segment for metadata
            if (segment.SegmentType == "MSH")
            {
                message.ParseMshSegment(segment);
            }
        }

        return message;
    }

    private void ParseMshSegment(HL7Segment mshSegment)
    {
        MessageControlId = mshSegment.GetField(10);
        Timestamp = DateTime.ParseExact(
            mshSegment.GetField(7),
            "yyyyMMddHHmmss",
            null);
        MessageType = mshSegment.GetField(9)?.Split('^').FirstOrDefault() ?? "";
        TriggerEvent = mshSegment.GetField(9)?.Split('^').ElementAtOrDefault(1) ?? "";
        SendingApplication = mshSegment.GetField(3)?.Split('^').FirstOrDefault() ?? "";
        SendingFacility = mshSegment.GetField(4)?.Split('^').FirstOrDefault() ?? "";
        ReceivingApplication = mshSegment.GetField(5)?.Split('^').FirstOrDefault() ?? "";
        ReceivingFacility = mshSegment.GetField(6)?.Split('^').FirstOrDefault() ?? "";
    }

    public string GenerateAck(string acknowledgmentCode, string? errorMessage = null)
    {
        var ack = new HL7Message
        {
            MessageType = "ACK",
            TriggerEvent = TriggerEvent,
            MessageControlId = MessageControlId,
            Timestamp = DateTime.UtcNow,
            SendingApplication = "HBYS",
            SendingFacility = "HBYS",
            ReceivingApplication = SendingApplication,
            ReceivingFacility = SendingFacility
        };

        var msh = HL7Segment.Create("MSH");
        msh.SetField(1, "^~\\&");
        msh.SetField(2, "HBYS");
        msh.SetField(3, "HBYS");
        msh.SetField(7, Timestamp?.ToString("yyyyMMddHHmmss") ?? DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        msh.SetField(9, $"{MessageType}^{TriggerEvent}");
        msh.SetField(10, Guid.NewGuid().ToString("N")[..10]);

        var MSA = HL7Segment.Create("MSA");
        MSA.SetField(1, acknowledgmentCode);
        MSA.SetField(2, MessageControlId);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            var ERR = HL7Segment.Create("ERR");
            ERR.SetField(1, errorMessage);
            ack.Segments.Add(ERR);
        }

        ack.Segments.Insert(0, MSA);
        ack.Segments.Insert(0, msh);

        return ack.ToString();
    }

    public string ToString()
    {
        return string.Join("\r", Segments.Select(s => s.ToString()));
    }
}

public class HL7Segment
{
    public string SegmentType { get; private set; } = string.Empty;
    private readonly List<string> _fields = new() { "" };

    public HL7Segment(string segmentType)
    {
        SegmentType = segmentType;
    }

    public static HL7Segment Create(string segmentType)
    {
        return new HL7Segment(segmentType);
    }

    public static HL7Segment Parse(string line)
    {
        var parts = line.Split('|');
        var segment = new HL7Segment(parts[0]);

        for (int i = 1; i < parts.Length; i++)
        {
            segment._fields.Add(parts[i]);
        }

        return segment;
    }

    public string GetField(int index)
    {
        if (index < 0 || index >= _fields.Count)
            return "";
        
        return _fields[index].Split('^').FirstOrDefault() ?? "";
    }

    public void SetField(int index, string value)
    {
        while (_fields.Count <= index)
        {
            _fields.Add("");
        }
        _fields[index] = value;
    }

    public string ToString()
    {
        return $"{SegmentType}|{string.Join("|", _fields.Skip(1))}";
    }
}
```

**HL7Service.cs**
```csharp
/// <summary>
/// HL7 servis sınıfı.
/// Ne: HL7 mesajlarının işlenmesi için servis sınıfıdır.
/// Neden: HL7 entegrasyonu için gereklidir.
/// Özelliği: ADT, ORM, ORU mesaj tiplerini destekler.
/// Kim Kullanacak: IntegrationController.
/// Amaç: HL7 mesaj işleme.
/// </summary>
public class HL7Service
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IIntegrationMessageRepository _messageRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<HL7Service> _logger;

    public async Task<HL7Message> ProcessMessageAsync(string rawMessage, Guid endpointId, Guid tenantId)
    {
        var message = HL7Message.Parse(rawMessage);
        
        // Create integration message record
        var integrationMessage = IntegrationMessage.Create(
            endpointId,
            MessageDirection.Inbound,
            message.MessageType,
            rawMessage,
            tenantId);

        await _messageRepository.AddAsync(integrationMessage);

        try
        {
            integrationMessage.MarkAsProcessing();

            // Route to appropriate handler
            var result = message.MessageType switch
            {
                "ADT" => await ProcessAdtMessageAsync(message, tenantId),
                "ORM" => await ProcessOrmMessageAsync(message, tenantId),
                "ORU" => await ProcessOruMessageAsync(message, tenantId),
                _ => throw new UnsupportedMessageTypeException($"Unsupported message type: {message.MessageType}")
            };

            integrationMessage.MarkAsSuccess(result);
            await _messageRepository.SaveChangesAsync();

            return message.GenerateAck("AA"); // Application Accept
        }
        catch (Exception ex)
        {
            integrationMessage.MarkAsFailed(ex.Message, ex.StackTrace);
            await _messageRepository.SaveChangesAsync();

            _logger.LogError(ex, "HL7 message processing failed");
            return message.GenerateAck("AE", ex.Message); // Application Error
        }
    }

    private async Task<string> ProcessAdtMessageAsync(HL7Message message, Guid tenantId)
    {
        // Extract patient information from PID segment
        var pidSegment = message.Segments.FirstOrDefault(s => s.SegmentType == "PID");
        if (pidSegment == null)
            throw new MissingSegmentException("PID segment is required");

        var patientId = pidSegment.GetField(3);
        var lastName = pidSegment.GetField(5)?.Split('^').FirstOrDefault();
        var firstName = pidSegment.GetField(5)?.Split('^').ElementAtOrDefault(1);
        var dateOfBirth = DateTime.ParseExact(pidSegment.GetField(7), "yyyyMMdd", null);
        var gender = pidSegment.GetField(8) == "M" ? Gender.Male : Gender.Female;

        // Handle different trigger events
        switch (message.TriggerEvent)
        {
            case "A01": // Patient admission
                await HandlePatientAdmissionAsync(patientId, firstName, lastName, dateOfBirth, gender, tenantId);
                break;
            case "A02": // Patient transfer
                await HandlePatientTransferAsync(patientId, message, tenantId);
                break;
            case "A03": // Patient discharge
                await HandlePatientDischargeAsync(patientId, tenantId);
                break;
            case "A04": // Patient registration
                await HandlePatientRegistrationAsync(patientId, firstName, lastName, dateOfBirth, gender, tenantId);
                break;
        }

        return "ADT processed successfully";
    }

    private async Task HandlePatientAdmissionAsync(string patientId, string? firstName, string? lastName, 
        DateTime dateOfBirth, Gender gender, Guid tenantId)
    {
        await _mediator.Publish(new PatientAdmissionFromHL7Event
        {
            PatientExternalId = patientId,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            TenantId = tenantId
        });
    }

    private async Task<string> ProcessOrmMessageAsync(HL7Message message, Guid tenantId)
    {
        // Order message processing
        var orcSegment = message.Segments.FirstOrDefault(s => s.SegmentType == "ORC");
        var pidSegment = message.Segments.FirstOrDefault(s => s.SegmentType == "PID");

        return "ORM processed successfully";
    }

    private async Task<string> ProcessOruMessageAsync(HL7Message message, Guid tenantId)
    {
        // Observation result processing
        return "ORU processed successfully";
    }
}
```

---

## SPRINT 32: Integration - PACS/LIS Entegrasyonu

### Sprint 32 Hedefi
PACS (Picture Archiving and Communication System) ve LIS (Laboratory Information System) entegrasyonlarının yapılması.

---

### Domain Task 32.1: PACS Integration

#### Task Tanımı
PACS sistemiyle DICOM ve görüntü veri entegrasyonu.

**PacsStudy.cs**
```csharp
/// <summary>
/// PACS çalışma entity sınıfı.
/// Ne: PACS görüntüleme çalışmalarını temsil eden entity sınıfıdır.
/// Neden: Radioloji görüntülerinin takibi için gereklidir.
/// Özelliği: StudyInstanceUID, PatientId, StudyDate, Modality, Description, 
///           AccessionNumber, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: PACSService.
/// Amaç: PACS çalışma verilerinin domain model olarak yönetilmesi.
/// </summary>
public class PacsStudy : BaseEntity
{
    public string StudyInstanceUID { get; private set; } = string.Empty;
    public string? PatientId { get; private set; }
    public string? PatientName { get; private set; }
    public DateTime StudyDate { get; private set; }
    public string Modality { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? AccessionNumber { get; private set; }
    public string? ReferringPhysician { get; private set; }
    public int SeriesCount { get; private set; }
    public int InstanceCount { get; private set; }
    public string? StudyUrl { get; private set; }
    public Guid? RadiologyRequestId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PacsStudy() { }

    public static PacsStudy Create(
        string studyInstanceUID,
        string? patientId,
        string? patientName,
        DateTime studyDate,
        string modality,
        string? description,
        string? accessionNumber,
        string? referringPhysician,
        Guid? radiologyRequestId,
        Guid tenantId)
    {
        return new PacsStudy
        {
            Id = Guid.NewGuid(),
            StudyInstanceUID = studyInstanceUID,
            PatientId = patientId,
            PatientName = patientName,
            StudyDate = studyDate,
            Modality = modality,
            Description = description,
            AccessionNumber = accessionNumber,
            ReferringPhysician = referringPhysician,
            SeriesCount = 0,
            InstanceCount = 0,
            RadiologyRequestId = radiologyRequestId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateCounts(int seriesCount, int instanceCount)
    {
        SeriesCount = seriesCount;
        InstanceCount = instanceCount;
    }

    public void LinkToRadiologyRequest(Guid radiologyRequestId)
    {
        RadiologyRequestId = radiologyRequestId;
    }
}
```

---

## SPRINT 33: Monitoring - Health Checks ve Metrics

### Sprint 33 Hedefi
Uygulama sağlık kontrolleri ve metrik toplama altyapısının kurulması.

---

### Domain Task 33.1: Health Check System

#### Task Tanımı
Kapsamlı health check sisteminin oluşturulması.

**HealthCheckResponse.cs**
```csharp
/// <summary>
/// Health check yanıt sınıfı.
/// Ne: Health check sonuçlarını temsil eden DTO sınıfıdır.
/// Neden: Kubernetes probe'ları için gereklidir.
/// Özelliği: Status, TotalDuration, Checks özelliklerine sahiptir.
/// Kim Kullanacak: HealthController.
/// Amaç: Health check yanıtı transferi.
/// </summary>
public class HealthCheckResponse
{
    public HealthStatus Status { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, HealthCheckResult> Checks { get; set; } = new();
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = string.Empty;

    public static HealthCheckResponse Create(
        TimeSpan duration,
        Dictionary<string, HealthCheckResult> checks,
        string version,
        string environment)
    {
        var overallStatus = checks.Values.All(c => c.Status == "Healthy")
            ? HealthStatus.Healthy
            : checks.Values.Any(c => c.Status == "Unhealthy")
                ? HealthStatus.Unhealthy
                : HealthStatus.Degraded;

        return new HealthCheckResponse
        {
            Status = overallStatus,
            TotalDuration = duration,
            Timestamp = DateTime.UtcNow,
            Checks = checks,
            Version = version,
            Environment = environment
        };
    }
}

public class HealthCheckResult
{
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

public enum HealthStatus
{
    Healthy = 1,
    Degraded = 2,
    Unhealthy = 3
}
```

---

## SPRINT 34: Data Warehouse

### Sprint 34 Hedefi
Veri ambarı altyapısının kurulması ve ETL süreçlerinin oluşturulması.

---

### Domain Task 34.1: Data Warehouse Schema

#### Task Tanımı
Veri ambarı şema tasarımı ve fact/dimension tabloları.

**FactAppointment.cs**
```csharp
/// <summary>
/// Randevu fact tablosu.
/// Ne: Randevu metriklerini içeren fact tablosu sınıfıdır.
/// Neden: Analitik raporlama için gereklidir.
/// Özelliği: AppointmentKey, PatientKey, DateKey, TimeKey, DepartmentKey, PhysicianKey, 
///           AppointmentStatusKey, WaitTimeMinutes, IsNoShow, IsCancelled özelliklerine sahiptir.
/// Kim Kullanacak: ETL job, Reporting.
/// Amaç: Randevu analitik verisi.
/// </summary>
public class FactAppointment
{
    public long AppointmentKey { get; set; }
    public long PatientKey { get; set; }
    public long DateKey { get; set; }
    public long TimeKey { get; set; }
    public long DepartmentKey { get; set; }
    public long PhysicianKey { get; set; }
    public long AppointmentStatusKey { get; set; }
    public int WaitTimeMinutes { get; set; }
    public int ActualDurationMinutes { get; set; }
    public bool IsNoShow { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime LoadDate { get; set; }
}

/// <summary>
/// Hasta fact tablosu.
/// Ne: Hasta metriklerini içeren fact tablosu sınıfıdır.
/// Kim Kullanacak: ETL job, Reporting.
/// </summary>
public class FactPatient
{
    public long PatientKey { get; set; }
    public long TenantKey { get; set; }
    public long DateKey { get; set; }
    public int TotalVisits { get; set; }
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int CancelledAppointments { get; set; }
    public decimal TotalBilling { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime FirstVisitDate { get; set; }
    public DateTime LastVisitDate { get; set; }
    public int DaysSinceLastVisit { get; set; }
    public DateTime LoadDate { get; set; }
}

/// <summary>
/// Hasta dimension tablosu.
/// Ne: Hasta dimension sınıfıdır.
/// Kim Kullanacak: ETL job, Reporting.
/// </summary>
public class DimPatient
{
    public long PatientKey { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string TurkishId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodType { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public DateTime LoadDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
```

---

## SPRINT 35: API Gateway

### Sprint 35 Hedefi
API Gateway altyapısının kurulması ve route yönetimi.

---

### Domain Task 35.1: API Gateway Configuration

#### Task Tanımı
API Gateway yapılandırması ve route'ların oluşturulması.

**GatewayRouteConfig.cs**
```csharp
/// <summary>
/// Gateway route yapılandırma sınıfı.
/// Ne: API Gateway route'larını temsil eden sınıftır.
/// Neden: Route yönetimi için gereklidir.
/// Özelliği: RouteId, Path, Methods, UpstreamUrl, RateLimit, Timeout, 
///           AuthRequired, CacheEnabled özelliklerine sahiptir.
/// Kim Kullanacak: GatewayService.
/// Amaç: Route yapılandırma verilerinin yönetilmesi.
/// </summary>
public class GatewayRouteConfig
{
    public string RouteId { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<string> Methods { get; set; } = new();
    public string UpstreamUrl { get; set; } = string.Empty;
    public RateLimitConfig? RateLimit { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public bool AuthRequired { get; set; } = true;
    public bool CacheEnabled { get; set; }
    public int CacheTtlSeconds { get; set; } = 60;
    public List<string>? RequiredScopes { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool IsActive { get; set; }
}

public class RateLimitConfig
{
    public int RequestsPerSecond { get; set; }
    public int RequestsPerMinute { get; set; }
    public int RequestsPerHour { get; set; }
    public int Burst { get; set; }
    public string? ClientKeyHeader { get; set; }
}
```

---

## SPRINT 36: Multi-Hospital Orchestration

### Sprint 36 Hedefi
Çoklu hastane koordinasyon ve orkestrasyon sisteminin oluşturulması.

---

### Domain Task 36.1: Multi-Hospital Coordination

#### Task Tanımı
Grup hastane koordinasyon sisteminin oluşturulması.

**Hospital.cs**
```csharp
/// <summary>
/// Hastane entity sınıfı.
/// Ne: Hastane bilgilerini temsil eden aggregate root sınıfıdır.
/// Neden: Çoklu hastane yönetimi için gereklidir.
/// Özelliği: HospitalCode, Name, Type, Address, ContactInfo, IsActive, 
///           GroupId, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: HospitalService.
/// Amaç: Hastane verilerinin domain model olarak yönetilmesi.
/// </summary>
public class Hospital : BaseEntity
{
    public string HospitalCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public HospitalType Type { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public int BedCapacity { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? GroupId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Hospital() { }

    public static Hospital Create(
        string hospitalCode,
        string name,
        HospitalType type,
        string? address,
        string? phone,
        string? email,
        int bedCapacity,
        Guid? groupId,
        Guid tenantId)
    {
        return new Hospital
        {
            Id = Guid.NewGuid(),
            HospitalCode = hospitalCode,
            Name = name,
            Type = type,
            Address = address,
            Phone = phone,
            Email = email,
            BedCapacity = bedCapacity,
            IsActive = true,
            GroupId = groupId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum HospitalType
{
    General = 1,
    Teaching = 2,
    Research = 3,
    Private = 4,
    Specialty = 5
}

/// <summary>
/// Hospital group entity sınıfı.
/// Ne: Hastane gruplarını temsil eden entity sınıfıdır.
/// Neden: Grup hastane koordinasyonu için gereklidir.
/// Kim Kullanacak: HospitalService.
/// </summary>
public class HospitalGroup : BaseEntity
{
    public string GroupCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private HospitalGroup() { }

    public static HospitalGroup Create(
        string groupCode,
        string name,
        string? description,
        Guid tenantId)
    {
        return new HospitalGroup
        {
            Id = Guid.NewGuid(),
            GroupCode = groupCode,
            Name = name,
            Description = description,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

**HospitalCoordinationService.cs**
```csharp
/// <summary>
/// Hastane koordinasyon servisi.
/// Ne: Çoklu hastane koordinasyonunu yöneten servis sınıfıdır.
/// Neden: Grup hastane işlemleri için gereklidir.
/// Özelliği: Cross-hospital patient lookup, resource sharing, reporting.
/// Kim Kullanacak: HospitalController.
/// Amaç: Hastane koordinasyon işlemleri.
/// </summary>
public class HospitalCoordinationService
{
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public async Task<CrossHospitalPatientInfo> SearchPatientAcrossHospitalsAsync(
        string searchTerm,
        Guid groupId)
    {
        var hospitals = await _hospitalRepository.GetActiveByGroupAsync(groupId);
        var results = new CrossHospitalPatientInfo();

        foreach (var hospital in hospitals)
        {
            var patients = await _patientRepository.SearchAsync(
                hospital.TenantId,
                searchTerm,
                null,
                null,
                null,
                1,
                10);

            foreach (var patient in patients.Items)
            {
                results.Matches.Add(new HospitalPatientMatch
                {
                    HospitalId = hospital.Id,
                    HospitalName = hospital.Name,
                    PatientId = patient.Id,
                    PatientNumber = patient.PatientNumber,
                    FullName = patient.FullName
                });
            }
        }

        return results;
    }

    public async Task<GroupResourceAvailability> GetGroupResourceAvailabilityAsync(
        Guid groupId,
        DateTime date,
        Guid? departmentId)
    {
        var hospitals = await _hospitalRepository.GetActiveByGroupAsync(groupId);
        var availability = new GroupResourceAvailability
        {
            Date = date,
            Hospitals = new List<HospitalResourceInfo>()
        };

        foreach (var hospital in hospitals)
        {
            var appointments = await _appointmentRepository.GetByDateAsync(hospital.TenantId, date);
            
            var hospitalInfo = new HospitalResourceInfo
            {
                HospitalId = hospital.Id,
                HospitalName = hospital.Name,
                TotalSlots = appointments.Count(),
                AvailableSlots = appointments.Count(a => a.Status == AppointmentStatus.Scheduled),
                BookedSlots = appointments.Count(a => a.Status == AppointmentStatus.Confirmed)
            };

            availability.Hospitals.Add(hospitalInfo);
        }

        return availability;
    }

    public async Task TransferPatientToHospitalAsync(
        Guid patientId,
        Guid sourceHospitalId,
        Guid targetHospitalId,
        Guid requestedBy)
    {
        // Verify patient exists in source hospital
        // Create transfer request
        // Notify both hospitals
        // Log audit trail
    }
}

public class CrossHospitalPatientInfo
{
    public List<HospitalPatientMatch> Matches { get; set; } = new();
    public int TotalMatches => Matches.Count;
}

public class HospitalPatientMatch
{
    public Guid HospitalId { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class GroupResourceAvailability
{
    public DateTime Date { get; set; }
    public List<HospitalResourceInfo> Hospitals { get; set; } = new();
}

public class HospitalResourceInfo
{
    public Guid HospitalId { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public int TotalSlots { get; set; }
    public int AvailableSlots { get; set; }
    public int BookedSlots { get; set; }
}
```

---

## 📊 FAZ 6 Tamamlandığında Beklenen Çıktılar

### Integration
- [x] IntegrationEndpoint entity ve repository
- [x] IntegrationMessage ile mesaj takibi
- [x] HL7 v2.x mesaj işleme (ADT, ORM, ORU)
- [x] PACS entegrasyonu (DICOM)
- [x] LIS entegrasyonu

### Monitoring
- [x] Health check response DTO'ları
- [x] Component health checks (DB, Redis, External services)
- [x] Metrics collection

### Data Warehouse
- [x] Fact tables (FactAppointment, FactPatient)
- [x] Dimension tables (DimPatient, DimDate, DimDepartment)
- [x] ETL job altyapısı

### API Gateway
- [x] GatewayRouteConfig yapılandırması
- [x] Rate limiting
- [x] Authentication/Authorization

### Multi-Hospital
- [x] Hospital entity
- [x] HospitalGroup entity
- [x] Cross-hospital patient search
- [x] Resource sharing

---

## 🎯 Proje Tamamlanma Özeti

### Tüm Fazlar Tamamlandığında:

| Faz | Sprint | İçerik | Domain Sayısı |
|-----|--------|--------|---------------|
| FAZ 0 | 1-5 | Identity, Tenant, License, Audit, Config | 45+ |
| FAZ 1 | 6-11 | Patient, Appointment, Outpatient, Billing | 60+ |
| FAZ 2 | 12-17 | Inpatient, Emergency, Pharmacy, Inventory | 55+ |
| FAZ 3 | 18-21 | Laboratory, Radiology | 30+ |
| FAZ 4 | 22-25 | Accounting, Reporting | 25+ |
| FAZ 5 | 26-30 | HR, Quality, Document, Notification | 50+ |
| FAZ 6 | 31-36 | Integration, Monitoring, DW, Gateway, Multi-Hospital | 45+ |

**Toplam Domain Entity/Service/Task Sayısı: 310+**

---

Bu dokümantasyon, HBYS FAZ 6'nın detaylı domain task listesini içermektedir. Tüm sprint'ler için spesifik entity sınıfları, servisler, DTO'lar ve açıklamalar Türkçe olarak sunulmuş, kod örnekleri ise İngilizce olarak hazırlanmıştır.