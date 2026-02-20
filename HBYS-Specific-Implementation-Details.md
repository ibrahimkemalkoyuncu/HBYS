# HBYS Specific Implementation Details
## Concrete Database Schemas, API Contracts, Validation Rules, and Technical Specifications

---

# PART 1: EXACT DATABASE SCHEMAS

## 1.1 Patient Module SQL Schema

```sql
-- Patients table with exact columns
CREATE TABLE [Patient].[Patients] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PatientNumber] NVARCHAR(20) NOT NULL,
    [TurkishId] NVARCHAR(256) NOT NULL, -- AES-256 encrypted
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Gender] INT NOT NULL, -- 1=Male, 2=Female, 3=Other
    [DateOfBirth] DATETIME2 NOT NULL,
    [BloodType] NVARCHAR(5) NULL,
    [PhotoUrl] NVARCHAR(500) NULL,
    [Status] INT NOT NULL DEFAULT 1, -- 1=Active, 2=Inactive, 3=Deceased
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedAt] DATETIME2 NULL,
    
    CONSTRAINT [PK_Patients] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Patients_Tenant_PatientNumber] UNIQUE ([TenantId], [PatientNumber]),
    CONSTRAINT [UK_Patients_Tenant_TurkishId] UNIQUE ([TenantId], [TurkishId])
);

CREATE NONCLUSTERED INDEX [IX_Patients_TenantId] ON [Patient].[Patients] ([TenantId]);
CREATE NONCLUSTERED INDEX [IX_Patients_LastName_FirstName] ON [Patient].[Patients] ([LastName], [FirstName]);
CREATE NONCLUSTERED INDEX [IX_Patients_Status] ON [Patient].[Patients] ([Status]);

-- Patient Contacts
CREATE TABLE [Patient].[PatientContacts] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [ContactType] INT NOT NULL, -- 1=Phone, 2=Email, 3=Address, 4=EmergencyPhone
    [Value] NVARCHAR(500) NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    
    CONSTRAINT [PK_PatientContacts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientContacts_Patient] FOREIGN KEY ([PatientId]) 
        REFERENCES [Patient].[Patients] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_PatientContacts_PatientId] ON [Patient].[PatientContacts] ([PatientId]);
CREATE NONCLUSTERED INDEX [IX_PatientContacts_TenantId] ON [Patient].[PatientContacts] ([TenantId]);

-- Patient Allergies
CREATE TABLE [Patient].[PatientAllergies] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [AllergyType] INT NOT NULL, -- 1=Food, 2=Drug, 3=Environmental, 4=Other
    [Allergen] NVARCHAR(200) NOT NULL,
    [Severity] INT NOT NULL, -- 1=Mild, 2=Moderate, 3=Severe, 4=LifeThreatening
    [Reaction] NVARCHAR(500) NULL,
    [OnsetDate] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    
    CONSTRAINT [PK_PatientAllergies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientAllergies_Patient] FOREIGN KEY ([PatientId]) 
        REFERENCES [Patient].[Patients] ([Id]) ON DELETE CASCADE
);

-- Patient Insurances
CREATE TABLE [Patient].[PatientInsurances] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [InsuranceType] INT NOT NULL, -- 1=SSK, 2=BagKur, 3=GreenCard, 4=Private
    [InsuranceCompanyId] UNIQUEIDENTIFIER NULL,
    [PolicyNumber] NVARCHAR(50) NULL,
    [GroupNumber] NVARCHAR(50) NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_PatientInsurances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientInsurances_Patient] FOREIGN KEY ([PatientId]) 
        REFERENCES [Patient].[Patients] ([Id]) ON DELETE CASCADE
);
```

## 1.2 Appointment Module SQL Schema

```sql
-- Appointments table
CREATE TABLE [Appointment].[Appointments] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AppointmentNumber] NVARCHAR(20) NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [PhysicianId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    [AppointmentDate] DATE NOT NULL,
    [StartTime] BIGINT NOT NULL, -- TimeSpan.Ticks
    [EndTime] BIGINT NOT NULL,
    [AppointmentType] INT NOT NULL, -- 1=Checkup, 2=FollowUp, 3=Procedure, 4=Consultation, 5=Emergency
    [Status] INT NOT NULL DEFAULT 1, -- 1=Scheduled, 2=Confirmed, 3=Completed, 4=Cancelled, 5=NoShow
    [Reason] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [ConfirmedAt] DATETIME2 NULL,
    [ConfirmedBy] UNIQUEIDENTIFIER NULL,
    [CancelledAt] DATETIME2 NULL,
    [CancelledBy] UNIQUEIDENTIFIER NULL,
    [CancelledReason] NVARCHAR(500) NULL,
    [CompletedAt] DATETIME2 NULL,
    [NoShowAt] DATETIME2 NULL,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Appointments_Tenant_AppointmentNumber] UNIQUE ([TenantId], [AppointmentNumber])
);

CREATE NONCLUSTERED INDEX [IX_Appointments_PatientId] ON [Appointment].[Appointments] ([PatientId]);
CREATE NONCLUSTERED INDEX [IX_Appointments_PhysicianId] ON [Appointment].[Appointments] ([PhysicianId]);
CREATE NONCLUSTERED INDEX [IX_Appointments_AppointmentDate] ON [Appointment].[Appointments] ([AppointmentDate]);
CREATE NONCLUSTERED INDEX [IX_Appointments_Status] ON [Appointment].[Appointments] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Appointments_Physician_Date_Status] 
    ON [Appointment].[Appointments] ([PhysicianId], [AppointmentDate], [Status]);

-- Appointment Slots (Availability)
CREATE TABLE [Appointment].[AppointmentSlots] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PhysicianId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    [DayOfWeek] INT NOT NULL, -- 0=Sunday, 1=Monday, etc.
    [StartTime] BIGINT NOT NULL,
    [EndTime] BIGINT NOT NULL,
    [SlotDuration] INT NOT NULL, -- minutes
    [IsActive] BIT NOT NULL DEFAULT 1,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_AppointmentSlots] PRIMARY KEY ([Id])
);

CREATE NONCLUSTERED INDEX [IX_AppointmentSlots_Physician_DayOfWeek] 
    ON [Appointment].[AppointmentSlots] ([PhysicianId], [DayOfWeek]);
```

## 1.3 Invoice Module SQL Schema

```sql
-- Invoices table
CREATE TABLE [Billing].[Invoices] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [InvoiceNumber] NVARCHAR(20) NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [InvoiceType] INT NOT NULL, -- 1=Patient, 2=Insurance, 3=Corporate
    [InvoiceDate] DATE NOT NULL,
    [DueDate] DATE NULL,
    [SubTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    [Status] INT NOT NULL DEFAULT 1, -- 1=Draft, 2=Issued, 3=Cancelled
    [PaymentStatus] INT NOT NULL DEFAULT 1, -- 1=Unpaid, 2=PartialPaid, 3=Paid, 4=Overdue
    [IssuedAt] DATETIME2 NULL,
    [PaidAt] DATETIME2 NULL,
    [CancelledAt] DATETIME2 NULL,
    [CancelledBy] UNIQUEIDENTIFIER NULL,
    [CancellationReason] NVARCHAR(500) NULL,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Invoices_Tenant_InvoiceNumber] UNIQUE ([TenantId], [InvoiceNumber])
);

CREATE NONCLUSTERED INDEX [IX_Invoices_PatientId] ON [Billing].[Invoices] ([PatientId]);
CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceDate] ON [Billing].[Invoices] ([InvoiceDate]);
CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [Billing].[Invoices] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Invoices_PaymentStatus] ON [Billing].[Invoices] ([PaymentStatus]);

-- Invoice Items
CREATE TABLE [Billing].[InvoiceItems] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceType] INT NOT NULL, -- 1=Examination, 2=Procedure, 3=Test, 4=Medication, 5=Material, 6=Room
    [ServiceId] UNIQUEIDENTIFIER NULL,
    [ServiceCode] NVARCHAR(20) NOT NULL,
    [ServiceName] NVARCHAR(200) NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [TotalPrice] DECIMAL(18,2) NOT NULL,
    [DiscountRate] DECIMAL(5,2) NOT NULL DEFAULT 0,
    [IsCoveredByInsurance] BIT NOT NULL DEFAULT 0,
    [PatientPayRate] DECIMAL(5,2) NOT NULL DEFAULT 100,
    [Status] INT NOT NULL DEFAULT 1, -- 1=Pending, 2=PartialPaid, 3=Paid, 4=Cancelled
    [PaidAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoiceItems_Invoice] FOREIGN KEY ([InvoiceId]) 
        REFERENCES [Billing].[Invoices] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_InvoiceItems_InvoiceId] ON [Billing].[InvoiceItems] ([InvoiceId]);

-- Payments
CREATE TABLE [Billing].[Payments] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
    [PaymentMethod] INT NOT NULL, -- 1=Cash, 2=CreditCard, 3=DebitCard, 4=BankTransfer, 5=Insurance
    [Amount] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    [TransactionId] NVARCHAR(100) NULL,
    [ReferenceNumber] NVARCHAR(50) NULL,
    [Notes] NVARCHAR(500) NULL,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Invoice] FOREIGN KEY ([InvoiceId]) 
        REFERENCES [Billing].[Invoices] ([Id]) ON DELETE CASCADE
);
```

## 1.4 Tenant Isolation SQL Schema

```sql
-- Tenants table
CREATE TABLE [Identity].[Tenants] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantCode] NVARCHAR(20) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [TenantType] INT NOT NULL, -- 1=SaaS, 2=OnPrem, 3=Group
    [DatabaseName] NVARCHAR(100) NULL, -- For schema-per-tenant
    [SchemaName] NVARCHAR(50) NULL, -- For schema-per-tenant
    [ConnectionString] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NULL,
    
    CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_Tenants_TenantCode] UNIQUE ([TenantCode])
);

-- Tenant Configurations
CREATE TABLE [Identity].[TenantConfigurations] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [ConfigKey] NVARCHAR(100) NOT NULL,
    [ConfigValue] NVARCHAR(MAX) NULL,
    [IsEncrypted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    
    CONSTRAINT [PK_TenantConfigurations] PRIMARY KEY ([Id]),
    CONSTRAINT [UK_TenantConfigurations_Tenant_Key] UNIQUE ([TenantId], [ConfigKey]),
    CONSTRAINT [FK_TenantConfigurations_Tenant] FOREIGN KEY ([TenantId]) 
        REFERENCES [Identity].[Tenants] ([Id]) ON DELETE CASCADE
);
```

---

# PART 2: EXACT API CONTRACTS

## 2.1 Patient API Endpoints

### Create Patient
```
POST /api/v1/patients
Content-Type: application/json
Authorization: Bearer {token}

Request:
{
  "turkishId": "encrypted-string",
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "gender": 1,
  "dateOfBirth": "1990-05-15",
  "bloodType": "A+",
  "contactInfos": [
    {
      "contactType": 1,
      "value": "+905551234567",
      "isPrimary": true
    },
    {
      "contactType": 2,
      "value": "ahmet.yilmaz@email.com",
      "isPrimary": false
    }
  ],
  "insurance": {
    "insuranceType": 1,
    "policyNumber": "POL-123456",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31"
  }
}

Response (201 Created):
{
  "id": "guid",
  "patientNumber": "P20240001",
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "fullName": "Ahmet Yılmaz",
  "gender": 1,
  "dateOfBirth": "1990-05-15",
  "age": 34,
  "bloodType": "A+",
  "status": 1,
  "createdAt": "2024-01-15T10:30:00Z",
  "contactInfos": [...],
  "insurances": [...]
}

Validation Rules:
- turkishId: Required, 11 digits (encrypted)
- firstName: Required, 2-100 characters, Turkish letters only
- lastName: Required, 2-100 characters, Turkish letters only
- gender: Required, 1-3
- dateOfBirth: Required, cannot be future date, age must be >= 0 and <= 120
- bloodType: Optional, must be valid blood type (A+, A-, B+, B-, AB+, AB-, O+, O-)
- contactInfos: Optional, max 5 contacts
- contactType 1 (Phone): Must match Turkey phone format
- contactType 2 (Email): Must be valid email format
```

### Get Patient By ID
```
GET /api/v1/patients/{id}
Authorization: Bearer {token}

Response (200 OK):
{
  "id": "guid",
  "patientNumber": "P20240001",
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "fullName": "Ahmet Yılmaz",
  "gender": 1,
  "genderText": "Erkek",
  "dateOfBirth": "1990-05-15",
  "age": 34,
  "bloodType": "A+",
  "photoUrl": "https://storage.hbys.com/patients/12345.jpg",
  "status": 1,
  "statusText": "Aktif",
  "contactInfos": [
    {
      "id": "guid",
      "contactType": 1,
      "contactTypeText": "Telefon",
      "value": "+905551234567",
      "isPrimary": true,
      "isVerified": true
    }
  ],
  "insurances": [
    {
      "id": "guid",
      "insuranceType": 1,
      "insuranceTypeText": "SSK",
      "policyNumber": "POL-123456",
      "isActive": true,
      "startDate": "2024-01-01",
      "endDate": "2024-12-31"
    }
  ],
  "allergies": [
    {
      "id": "guid",
      "allergyType": 2,
      "allergyTypeText": "İlaç",
      "allergen": "Penisilin",
      "severity": 3,
      "severityText": "Şiddetli",
      "isActive": true
    }
  ],
  "chronicDiseases": [...],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}

Response (404 Not Found):
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Patient not found",
  "traceId": "guid"
}
```

### Search Patients
```
GET /api/v1/patients/search?searchTerm=Ahmet&pageNumber=1&pageSize=20&status=1
Authorization: Bearer {token}

Query Parameters:
- searchTerm: Optional, searches by name and patient number
- status: Optional, 1=Active, 2=Inactive, 3=Deceased
- dateOfBirthFrom: Optional, ISO date
- dateOfBirthTo: Optional, ISO date
- pageNumber: Required, default 1, min 1
- pageSize: Required, default 20, min 1, max 100
- sortBy: Optional, "name" | "createdAt" | "dateOfBirth"
- sortOrder: Optional, "asc" | "desc"

Response (200 OK):
{
  "items": [
    {
      "id": "guid",
      "patientNumber": "P20240001",
      "firstName": "Ahmet",
      "lastName": "Yılmaz",
      "fullName": "Ahmet Yılmaz",
      "age": 34,
      "gender": 1,
      "genderText": "Erkek",
      "status": 1,
      "statusText": "Aktif",
      "primaryPhone": "+905551234567",
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 8,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "firstItemIndex": 1,
  "lastItemIndex": 20
}
```

## 2.2 Appointment API Endpoints

### Create Appointment
```
POST /api/v1/appointments
Content-Type: application/json
Authorization: Bearer {token}

Request:
{
  "patientId": "guid",
  "physicianId": "guid",
  "departmentId": "guid",
  "appointmentDate": "2024-02-15",
  "startTime": "09:30",
  "appointmentType": 1,
  "reason": "Kontrol muayenesi"
}

Response (201 Created):
{
  "id": "guid",
  "appointmentNumber": "APT-20240215-00001",
  "patientId": "guid",
  "patientName": "Ahmet Yılmaz",
  "physicianId": "guid",
  "physicianName": "Dr. Mehmet Demir",
  "departmentId": "guid",
  "departmentName": "Dahiliye",
  "appointmentDate": "2024-02-15",
  "startTime": "09:30",
  "endTime": "10:00",
  "duration": 30,
  "appointmentType": 1,
  "appointmentTypeText": "Kontrol",
  "status": 1,
  "statusText": "Planlandı",
  "reason": "Kontrol muayenesi",
  "notes": null,
  "createdAt": "2024-01-15T10:30:00Z"
}

Validation Rules:
- patientId: Required, must exist and be active
- physicianId: Required, must exist and be active
- departmentId: Required, must match physician's department
- appointmentDate: Required, cannot be in past, max 6 months ahead
- startTime: Required, must be in valid time slots
- appointmentType: Required, 1-5
- reason: Optional, max 500 characters
- Check availability before creating
```

### Get Available Slots
```
GET /api/v1/appointments/available-slots?physicianId={guid}&departmentId={guid}&date=2024-02-15
Authorization: Bearer {token}

Response (200 OK):
{
  "physicianId": "guid",
  "physicianName": "Dr. Mehmet Demir",
  "departmentId": "guid",
  "departmentName": "Dahiliye",
  "date": "2024-02-15",
  "dayOfWeek": "Perşembe",
  "slots": [
    {
      "startTime": "09:00",
      "endTime": "09:30",
      "isAvailable": true
    },
    {
      "startTime": "09:30",
      "endTime": "10:00",
      "isAvailable": false,
      "reason": "Booked"
    },
    {
      "startTime": "10:00",
      "endTime": "10:30",
      "isAvailable": true
    }
  ]
}
```

---

# PART 3: EXACT VALIDATION RULES

## 3.1 Patient Validation

```csharp
/// <summary>
/// Patient validation rules.
/// Ne: Hasta validasyon kurallarını temsil eden sınıftır.
/// Neden: Veri bütünlüğü için gereklidir.
/// </summary>
public static class PatientValidationRules
{
    // Turkish ID (TC Kimlik No) validation
    public static bool IsValidTurkishId(string turkishId)
    {
        if (string.IsNullOrWhiteSpace(turkishId) || turkishId.Length != 11)
            return false;

        if (!long.TryParse(turkishId, out _))
            return false;

        // TC Kimlik No algorithm
        int[] digits = turkishId.Select(c => int.Parse(c.ToString())).ToArray();
        
        int sum1 = 0;
        int sum2 = 0;
        
        for (int i = 0; i < 9; i += 2)
            sum1 += digits[i];
        
        for (int i = 1; i < 8; i += 2)
            sum2 += digits[i];

        int digit10 = (sum1 * 7 - sum2) % 10;
        if (digit10 < 0) digit10 += 10;
        
        int digit11 = (digits.Take(10).Sum()) % 10;

        return digits[9] == digit10 && digits[10] == digit11;
    }

    // Name validation - Turkish letters only
    public static bool IsValidTurkishName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 100)
            return false;

        var turkishLetters = "abcçdefghıijklmnoöprsştuüvyzABCÇDEFGHIİJKLMNOÖPRSŞTUÜVYZ ";
        return name.All(c => turkishLetters.Contains(c));
    }

    // Phone validation - Turkey format
    public static bool IsValidTurkishPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Remove spaces and dashes
        phone = phone.Replace(" ", "").Replace("-", "");

        // Must be 10 or 11 digits
        if (phone.Length < 10 || phone.Length > 11)
            return false;

        // Must start with 5 (mobile) or 3, 4 (landline)
        if (!phone.StartsWith("5") && !phone.StartsWith("3") && !phone.StartsWith("4"))
            return false;

        return long.TryParse(phone, out _);
    }

    // Email validation
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Blood type validation
    public static readonly string[] ValidBloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
    
    public static bool IsValidBloodType(string? bloodType)
    {
        if (string.IsNullOrWhiteSpace(bloodType))
            return true; // Optional field

        return ValidBloodTypes.Contains(bloodType.ToUpper());
    }

    // Date of birth validation
    public static (bool IsValid, string? ErrorMessage) ValidateDateOfBirth(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        
        if (dateOfBirth > today)
            return (false, "Date of birth cannot be in the future");

        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        if (age < 0)
            return (false, "Invalid date of birth");

        if (age > 120)
            return (false, "Age cannot exceed 120 years");

        return (true, null);
    }
}
```

---

# PART 4: SECURITY IMPLEMENTATION

## 4.1 Encryption Service

```csharp
/// <summary>
/// Encryption service interface.
/// Ne: Şifreleme işlemleri için arayüz.
/// Neden: Hassas verilerin korunması için gereklidir.
/// </summary>
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string Hash(string plainText);
    bool VerifyHash(string plainText, string hash);
}

/// <summary>
/// AES-256 encryption service.
/// Ne: AES-256 şifreleme implementasyonu.
/// Neden: Hassas verilerin şifrelenmesi için gereklidir.
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    private const int KeySize = 256;
    private const int BlockSize = 128;

    public AesEncryptionService(IConfiguration configuration)
    {
        var key = configuration["Encryption:Key"] 
            ?? throw new ArgumentException("Encryption key not configured");
        var iv = configuration["Encryption:IV"] 
            ?? throw new ArgumentException("Encryption IV not configured");

        _key = Convert.FromBase64String(key);
        _iv = Convert.FromBase64String(iv);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch
        {
            throw new CryptographicException("Failed to decrypt data");
        }
    }

    public string Hash(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var salt = GenerateSalt();
        var combined = plainText + salt;
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        
        return $"{Convert.ToBase64String(hashBytes)}:{salt}";
    }

    public bool VerifyHash(string plainText, string hash)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(hash))
            return false;

        var parts = hash.Split(':');
        if (parts.Length != 2)
            return false;

        var salt = parts[1];
        using var sha256 = SHA256.Create();
        var combined = plainText + salt;
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        
        return parts[0] == Convert.ToBase64String(hashBytes);
    }

    private string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
}
```

---

# PART 5: ERROR HANDLING

## 5.1 Exception Types

```csharp
/// <summary>
/// Patient not found exception.
/// Ne: Hasta bulunamadığında fırlatılan exception.
/// Neden: Hasta bulunamadı hatası için gereklidir.
/// </summary>
public class PatientNotFoundException : NotFoundException
{
    public Guid PatientId { get; }

    public PatientNotFoundException(Guid patientId)
        : base($"Patient with ID {patientId} not found")
    {
        PatientId = patientId;
    }

    public PatientNotFoundException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Appointment slot not available exception.
/// Ne: Randevu slotu müsait olmadığında fırlatılan exception.
/// Neden: Randevu çakışması için gereklidir.
/// </summary>
public class AppointmentSlotNotAvailableException : BusinessException
{
    public DateTime Date { get; }
    public TimeSpan StartTime { get; }
    public Guid PhysicianId { get; }

    public AppointmentSlotNotAvailableException(
        DateTime date,
        TimeSpan startTime,
        Guid physicianId)
        : base($"Slot not available for physician {physicianId} at {date} {startTime}")
    {
        Date = date;
        StartTime = startTime;
        PhysicianId = physicianId;
    }

    public AppointmentSlotNotAvailableException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Base exception types.
/// Ne: Exception hierarchy.
/// </summary>
public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message) { }
}

public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}

public abstract class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    protected ValidationException(string message, Dictionary<string, string[]> errors = null)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
```

---

# PART 6: CACHING STRATEGY

## 6.1 Cache Keys

```csharp
/// <summary>
/// Cache key constants.
/// Ne: Cache key sabitleri.
/// Neden: Tutarlı cache anahtarları için gereklidir.
/// </summary>
public static class CacheKeys
{
    public const string PatientPrefix = "patient:";
    public const string AppointmentPrefix = "appointment:";
    public const string DepartmentPrefix = "department:";
    public const string PhysicianPrefix = "physician:";
    public const string ConfigurationPrefix = "config:";
    
    public static string Patient(Guid tenantId, Guid patientId) 
        => $"{tenantId}:{PatientPrefix}{patientId}";
    
    public static string PatientList(Guid tenantId, string searchTerm, int page) 
        => $"{tenantId}:{PatientPrefix}list:{searchTerm}:{page}";
    
    public static string Appointment(Guid tenantId, Guid appointmentId) 
        => $"{tenantId}:{AppointmentPrefix}{appointmentId}";
    
    public static string AvailableSlots(Guid tenantId, Guid physicianId, DateTime date) 
        => $"{tenantId}:{AppointmentPrefix}slots:{physicianId}:{date:yyyyMMdd}";
    
    public static string Department(Guid tenantId, Guid departmentId) 
        => $"{tenantId}:{DepartmentPrefix}{departmentId}";
    
    public static string Configuration(Guid tenantId, string key) 
        => $"{tenantId}:{ConfigurationPrefix}{key}";
    
    // TTL constants
    public static readonly TimeSpan PatientTtl = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan AppointmentTtl = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan DepartmentTtl = TimeSpan.FromHours(1);
    public static readonly TimeSpan ConfigurationTtl = TimeSpan.FromHours(24);
}
```

---

# PART 7: AUDIT LOGGING

## 7.1 Audit Log Entry

```csharp
/// <summary>
/// Immutable audit log entry.
/// Ne: Değiştirilemez audit log kaydı.
/// Neden: Tam izleme için gereklidir.
/// </summary>
public class AuditLogEntry
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Changes { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }

    public static AuditLogEntry Create(
        Guid tenantId,
        Guid userId,
        string action,
        string entityName,
        string entityId,
        string changes,
        HttpContext? httpContext = null)
    {
        return new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Changes = changes,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "unknown",
            Timestamp = DateTime.UtcNow
        };
    }
}
```

Bu dokümantasyon, HBYS sisteminin spesifik implementasyon detaylarını içermektedir. Exact database schemas, API contracts, validation rules, security implementation, error handling, caching strategy, ve audit logging dahildir.