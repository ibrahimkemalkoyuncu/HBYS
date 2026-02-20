# HBYS Advanced Services
## SignalR, GraphQL, Event Sourcing, Saga Pattern, and External Integrations

---

# PART 1: SIGNALR REAL-TIME SERVICES

## 1.1 SignalR Hub Implementation

```csharp
/// <summary>
/// HBYS SignalR hub.
/// Ne: Real-time bildirimler için SignalR hub sınıfıdır.
/// Neden: Hasta randevusu, laboratuvar sonucu gibi anlık bildirimler için gereklidir.
/// </summary>
public class HbysHub : Hub
{
    private readonly IHubContext<HbysHub> _hubContext;
    private readonly IUserConnectionManager _connectionManager;
    private readonly ILogger<HbysHub> _logger;

    public HbysHub(
        IHubContext<HbysHub> hubContext,
        IUserConnectionManager connectionManager,
        ILogger<HbysHub> logger)
    {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    /// <summary>
    /// Connect to hub with tenant and user context.
    /// Ne: Hub'a bağlanma metodu.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var tenantId = httpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        var userId = httpContext?.Request.Headers["X-User-Id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            _connectionManager.AddConnection(userId, Context.ConnectionId);
            
            _logger.LogInformation("User {UserId} connected to tenant {TenantId}", userId, tenantId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Disconnect from hub.
    /// Ne: Hub'dan bağlantı kesme metodu.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            _connectionManager.RemoveConnection(userId);
            _logger.LogInformation("User {UserId} disconnected", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join specific appointment room.
    /// Ne: Randevu odasına katılma metodu.
    /// </summary>
    public async Task JoinAppointmentRoom(string appointmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"appointment_{appointmentId}");
        _logger.LogInformation("User joined appointment room {AppointmentId}", appointmentId);
    }

    /// <summary>
    /// Leave appointment room.
    /// Ne: Randevu odasından çıkma metodu.
    /// </summary>
    public async Task LeaveAppointmentRoom(string appointmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"appointment_{appointmentId}");
    }
}

/// <summary>
/// Notification service with SignalR integration.
/// Ne: SignalR ile bildirim servisi.
/// </summary>
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<HbysHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public async Task SendAppointmentNotificationAsync(Guid tenantId, Guid patientId, AppointmentNotification notification)
    {
        await _hubContext.Clients.Group($"tenant_{tenantId}")
            .SendAsync("AppointmentNotification", notification);
        
        _logger.LogInformation("Appointment notification sent to patient {PatientId}", patientId);
    }

    public async Task SendLabResultNotificationAsync(Guid tenantId, Guid patientId, LabResultNotification notification)
    {
        await _hubContext.Clients.Group($"tenant_{tenantId}")
            .SendAsync("LabResultNotification", notification);
        
        _logger.LogInformation("Lab result notification sent to patient {PatientId}", patientId);
    }

    public async Task SendToUserAsync(Guid userId, string message, object? data = null)
    {
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("ReceiveMessage", message, data);
    }
}
```

---

# PART 2: GRAPHQL IMPLEMENTATION

## 2.1 GraphQL Schema and Resolvers

```csharp
/// <summary>
/// GraphQL query types.
/// Ne: GraphQL sorgu tipleri tanımlaması.
/// </summary>
public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Name("Query");
        
        descriptor.Field(f => f.GetPatient(default!, default!))
            .Argument("id", a => a.Type<NonNullType<UuidType>>())
            .Type<PatientType>()
            .Description("Get patient by ID");
            
        descriptor.Field(f => f.SearchPatients(default!, default!))
            .Argument("searchTerm", a => a.Type<StringType>())
            .Argument("pageNumber", a => a.Type<IntType>().DefaultValue(1))
            .Argument("pageSize", a => a.Type<IntType>().DefaultValue(20))
            .Type<NonNullType<ListType<NonNullType<PatientType>>>>()
            .Description("Search patients");
            
        descriptor.Field(f => f.GetAppointments(default!, default!))
            .Argument("patientId", a => a.Type<UuidType>())
            .Type<NonNullType<ListType<NonNullType<AppointmentType>>>>()
            .Description("Get patient appointments");
    }
}

/// <summary>
/// GraphQL mutation types.
/// Ne: GraphQL mutasyon tipleri tanımlaması.
/// </summary>
public class MutationType : ObjectType<Mutation>
{
    protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor.Name("Mutation");
        
        descriptor.Field(f => f.CreatePatient(default!, default!))
            .Argument("input", a => a.Type<NonNullType<CreatePatientInputType>>())
            .Type<NonNullType<PatientType>>()
            .Description("Create new patient");
            
        descriptor.Field(f => f.CreateAppointment(default!, default!))
            .Argument("input", a => a.Type<NonNullType<CreateAppointmentInputType>>())
            .Type<NonNullType<AppointmentType>>()
            .Description("Create new appointment");
    }
}

/// <summary>
/// Patient GraphQL type.
/// Ne: Hasta GraphQL tipi tanımlaması.
/// </summary>
public class PatientType : ObjectType<Patient>
{
    protected override void Configure(IObjectTypeDescriptor<Patient> descriptor)
    {
        descriptor.Name("Patient");
        
        descriptor.Field(f => f.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(f => f.PatientNumber).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.LastName).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.FullName).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.DateOfBirth).Type<NonNullType<DateTimeType>>();
        descriptor.Field(f => f.Gender).Type<NonNullType<IntType>>();
        
        descriptor.Field("contacts")
            .Type<ListType<ContactType>>()
            .Resolve(ctx => ctx.Source.ContactInfos);
            
        descriptor.Field("allergies")
            .Type<ListType<AllergyType>>()
            .Resolve(ctx => ctx.Source.Allergies);
    }
}
```

---

# PART 3: EVENT SOURCING

## 3.1 Event Store Implementation

```csharp
/// <summary>
/// Event store interface.
/// Ne: Event sourcing için event store arayüzü.
/// </summary>
public interface IEventStore
{
    Task AppendAsync(EventStoreEntry entry);
    Task<List<EventStoreEntry>> GetEventsAsync(string aggregateId, long fromVersion);
    Task<List<EventStoreEntry>> GetEventsByCorrelationAsync(string correlationId);
    Task<List<EventStoreEntry>> GetEventsByTenantAsync(Guid tenantId, DateTime from, DateTime to);
}

/// <summary>
/// Event store entry.
/// Ne: Event store kaydı.
/// </summary>
public class EventStoreEntry
{
    public Guid Id { get; set; }
    public string AggregateId { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int Version { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Aggregate base class for event sourcing.
/// Ne: Event sourcing için aggregate base sınıfı.
/// </summary>
public abstract class EventSourcingAggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }
    
    private readonly List<EventStoreEntry> _uncommittedEvents = new();
    public IReadOnlyList<EventStoreEntry> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    
    protected abstract void Apply(EventStoreEntry eventEntry);
    
    public void LoadFromHistory(IEnumerable<EventStoreEntry> events)
    {
        foreach (var evt in events)
        {
            Apply(evt);
            Version = evt.Version;
        }
    }
    
    protected void RaiseDomainEvent(string eventType, object eventData, Guid tenantId, Guid userId)
    {
        var entry = new EventStoreEntry
        {
            Id = Guid.NewGuid(),
            AggregateId = Id.ToString(),
            AggregateType = GetType().Name,
            Version = Version + 1,
            EventType = eventType,
            EventData = JsonSerializer.Serialize(eventData),
            TenantId = tenantId,
            UserId = userId,
            CorrelationId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        
        _uncommittedEvents.Add(entry);
        Apply(entry);
        Version++;
    }
    
    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }
}
```

---

# PART 4: SAGA PATTERN

## 4.1 Saga Orchestrator

```csharp
/// <summary>
/// Saga state machine for appointment creation.
/// Ne: Randevu oluşturma için saga state machine.
/// </summary>
public class AppointmentSaga : SagaStateMachine<AppointmentSagaState>
{
    public AppointmentSaga(AppointmentSagaState state) : base(state) { }
    
    public Event<AppointmentCreated> AppointmentCreated { get; set; } = null!;
    public Event<PatientVerified> PatientVerified { get; set; } = null!;
    public Event<PhysicianAvailable> PhysicianAvailable { get; set; } = null!;
    public Event<AppointmentBooked> AppointmentBooked { get; set; } = null!;
    public Event<NotificationSent> NotificationSent { get; set; } = null!;
    public Event<AppointmentFailed> AppointmentFailed { get; set; } = null!;
    
    public State<AppointmentSagaState> VerifyingPatient { get; set; } = null!;
    public State<AppointmentSagaState> CheckingAvailability { get; set; } = null!;
    public State<AppointmentSagaState> BookingAppointment { get; set; } = null!;
    public State<AppointmentSagaState> SendingNotification { get; set; } = null!;
    public State<AppointmentSagaState> Completed { get; set; } = null!;
    public State<AppointmentSagaState> Failed { get; set; } = null!;
    
    public AppointmentSaga()
    {
        InstanceState(x => x.CurrentState);
        
        Initially(
            When(AppointmentCreated)
                .TransitionTo(VerifyingPatient)
        );
        
        During(VerifyingPatient,
            When(PatientVerified)
                .TransitionTo(CheckingAvailability),
            When(AppointmentFailed)
                .TransitionTo(Failed)
        );
        
        During(CheckingAvailability,
            When(PhysicianAvailable)
                .TransitionTo(BookingAppointment),
            When(AppointmentFailed)
                .TransitionTo(Failed)
        );
        
        During(BookingAppointment,
            When(AppointmentBooked)
                .TransitionTo(SendingNotification),
            When(AppointmentFailed)
                .TransitionTo(Failed)
        );
        
        During(SendingNotification,
            When(NotificationSent)
                .TransitionTo(Completed)
        );
    }
}

/// <summary>
/// Saga state.
/// Ne: Saga durumu.
/// </summary>
public class AppointmentSagaState : SagaState
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid PhysicianId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public string? FailureReason { get; set; }
    public int CurrentState { get; set; }
}
```

---

# PART 5: PAYMENT GATEWAY INTEGRATION

## 5.1 Payment Service Implementation

```csharp
/// <summary>
/// Payment gateway interface.
/// Ne: Ödeme gateway arayüzü.
/// </summary>
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<RefundResult> ProcessRefundAsync(RefundRequest request);
    Task<PaymentStatus> GetPaymentStatusAsync(string transactionId);
}

/// <summary>
/// Payment request.
/// Ne: Ödeme isteği.
/// </summary>
public class PaymentRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public PaymentMethod Method { get; set; }
    public CardInfo? Card { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Payment result.
/// Ne: Ödeme sonucu.
/// </summary>
public class PaymentResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime? TransactionDate { get; set; }
}

/// <summary>
/// Virtual POS service.
/// Ne: Sanal POS servisi.
/// </summary>
public class VirtualPosService : IPaymentGateway
{
    private readonly IPaymentProvider _provider;
    private readonly ILogger<VirtualPosService> _logger;
    
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        var providerRequest = new ProviderPaymentRequest
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency,
            CardNumber = request.Card?.Number,
            CardHolder = request.Card?.HolderName,
            CardExpiryMonth = request.Card?.ExpiryMonth,
            CardExpiryYear = request.Card?.ExpiryYear,
            CardCvv = request.Card?.Cvv
        };
        
        var response = await _provider.ProcessPaymentAsync(providerRequest);
        
        return new PaymentResult
        {
            Success = response.ResultCode == "00",
            TransactionId = response.TransactionId,
            AuthorizationCode = response.AuthorizationCode,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ResultCode,
            TransactionDate = DateTime.UtcNow
        };
    }
    
    public async Task<RefundResult> ProcessRefundAsync(RefundRequest request)
    {
        // Refund implementation
    }
    
    public async Task<PaymentStatus> GetPaymentStatusAsync(string transactionId)
    {
        // Status check implementation
    }
}
```

---

# PART 6: EMAIL SERVICE

## 6.1 Email Service Implementation

```csharp
/// <summary>
/// Email service interface.
/// Ne: E-posta servisi arayüzü.
/// </summary>
public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailMessage message);
    Task<EmailResult> SendTemplatedEmailAsync(string templateCode, Dictionary<string, string> variables, EmailRecipient to);
    Task<EmailResult> SendBulkEmailAsync(List<EmailRecipient> recipients, EmailMessage message);
}

/// <summary>
/// Email message.
/// Ne: E-posta mesajı.
/// </summary>
public class EmailMessage
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public EmailRecipient From { get; set; } = null!;
    public List<EmailRecipient> To { get; set; } = new();
    public List<EmailRecipient>? Cc { get; set; }
    public List<EmailRecipient>? Bcc { get; set; }
    public List<EmailAttachment>? Attachments { get; set; }
    public bool IsHtml { get; set; } = true;
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
}

/// <summary>
/// SMTP email service.
/// Ne: SMTP e-posta servisi.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ITemplateService _templateService;
    private readonly ILogger<SmtpEmailService> _logger;
    
    public async Task<EmailResult> SendEmailAsync(EmailMessage message)
    {
        try
        {
            using var client = CreateSmtpClient();
            
            var mailMessage = new MailMessage
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml,
                Priority = MapPriority(message.Priority)
            };
            
            mailMessage.From = new MailAddress(message.From.Email, message.From.Name);
            
            foreach (var recipient in message.To)
            {
                mailMessage.To.Add(new MailAddress(recipient.Email, recipient.Name));
            }
            
            if (message.Cc != null)
            {
                foreach (var cc in message.Cc)
                {
                    mailMessage.CC.Add(new MailAddress(cc.Email, cc.Name));
                }
            }
            
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(
                        new MemoryStream(attachment.Content), 
                        attachment.FileName, 
                        attachment.ContentType));
                }
            }
            
            await client.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email sent successfully to {Recipients}", 
                string.Join(", ", message.To.Select(r => r.Email)));
            
            return new EmailResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            return new EmailResult { Success = false, ErrorMessage = ex.Message };
        }
    }
    
    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient
        {
            Host = _settings.Host,
            Port = _settings.Port,
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            Timeout = _settings.Timeout
        };
    }
}
```

---

# PART 7: FILE STORAGE SERVICE

## 7.1 Azure Blob Storage Implementation

```csharp
/// <summary>
/// File storage service interface.
/// Ne: Dosya depolama servisi arayüzü.
/// </summary>
public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(Stream content, string fileName, string container, Guid tenantId);
    Task<Stream> DownloadAsync(string filePath, Guid tenantId);
    Task DeleteAsync(string filePath, Guid tenantId);
    Task<string> GetSignedUrlAsync(string filePath, Guid tenantId, TimeSpan expiration);
    Task<List<FileInfo>> ListFilesAsync(string prefix, Guid tenantId);
}

/// <summary>
/// Azure blob storage service.
/// Ne: Azure Blob depolama servisi.
/// </summary>
public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _container;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<AzureBlobStorageService> _logger;
    
    public async Task<FileUploadResult> UploadAsync(Stream content, string fileName, string container, Guid tenantId)
    {
        var containerClient = GetContainerClient(container);
        
        // Generate unique file name
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var blobPath = $"{tenantId}/{DateTime.UtcNow:yyyy/MM}/{newFileName}";
        
        var blobClient = containerClient.GetBlobClient(blobPath);
        
        // Encrypt if needed
        using var encryptedStream = await EncryptStreamAsync(content);
        
        await blobClient.UploadAsync(encryptedStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(extension)
            },
            Metadata = new Dictionary<string, string>
            {
                ["originalFileName"] = fileName,
                ["tenantId"] = tenantId.ToString(),
                ["uploadedAt"] = DateTime.UtcNow.ToString("o")
            }
        });
        
        _logger.LogInformation("File uploaded: {BlobPath}", blobPath);
        
        return new FileUploadResult
        {
            Success = true,
            FilePath = blobPath,
            FileName = fileName,
            ContentType = GetContentType(extension),
            Size = content.Length
        };
    }
    
    public async Task<Stream> DownloadAsync(string filePath, Guid tenantId)
    {
        var containerClient = GetContainerClient("documents");
        var blobClient = containerClient.GetBlobClient(filePath);
        
        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }
    
    public async Task DeleteAsync(string filePath, Guid tenantId)
    {
        var containerClient = GetContainerClient("documents");
        var blobClient = containerClient.GetBlobClient(filePath);
        
        await blobClient.DeleteIfExistsAsync();
    }
    
    public async Task<string> GetSignedUrlAsync(string filePath, Guid tenantId, TimeSpan expiration)
    {
        var containerClient = GetContainerClient("documents");
        var blobClient = containerClient.GetBlobClient(filePath);
        
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = filePath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiration)
        };
        
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        var sasToken = sasBuilder.ToSasQueryParameters(
            new UserDelegationKey(DateTime.UtcNow, DateTime.UtcNow.AddDays(7)));
        
        return $"{blobClient.Uri}?{sasToken}";
    }
}
```

---

Bu dokümantasyon, HBYS sisteminin ileri düzey servis implementasyonlarını içermektedir. SignalR real-time services, GraphQL, Event sourcing, Saga pattern, Payment gateway, Email service, ve File storage service dahildir.