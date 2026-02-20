# HBYS Testing and Infrastructure Details
## Unit Tests, Integration Tests, Performance Tests, and IaC

---

# PART 1: UNIT TESTS

## 1.1 Patient Entity Tests

```csharp
/// <summary>
/// Patient entity unit tests.
/// Ne: Patient entity için birim testleri.
/// Neden: Domain logic testi için gereklidir.
/// </summary>
public class PatientTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePatient()
    {
        // Arrange
        var turkishId = "12345678901"; // Valid TC Kimlik No
        var firstName = "Ahmet";
        var lastName = "Yılmaz";
        var gender = Gender.Male;
        var dateOfBirth = new DateTime(1990, 5, 15);
        var tenantId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();

        // Act
        var patient = Patient.Create(
            "P20240001",
            turkishId,
            firstName,
            lastName,
            gender,
            dateOfBirth,
            null,
            tenantId,
            createdBy);

        // Assert
        patient.Id.Should().NotBeEmpty();
        patient.PatientNumber.Should().Be("P20240001");
        patient.FirstName.Should().Be(firstName);
        patient.LastName.Should().Be(lastName);
        patient.Gender.Should().Be(gender);
        patient.DateOfBirth.Should().Be(dateOfBirth);
        patient.Status.Should().Be(PatientStatus.Active);
        patient.TenantId.Should().Be(tenantId);
        patient.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    public void UpdateBasicInfo_ShouldUpdateProperties()
    {
        // Arrange
        var patient = CreateTestPatient();
        var newFirstName = "Mehmet";
        var newLastName = "Demir";

        // Act
        patient.UpdateBasicInfo(newFirstName, newLastName, Gender.Male, 
            patient.DateOfBirth, "A+");

        // Assert
        patient.FirstName.Should().Be(newFirstName);
        patient.LastName.Should().Be(newLastName);
        patient.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddContactInfo_WithPrimary_ShouldSetOtherContactsNotPrimary()
    {
        // Arrange
        var patient = CreateTestPatient();
        var contact1 = PatientContact.Create(
            patient.Id, ContactType.Phone, "+905551234567", true, patient.TenantId);
        var contact2 = PatientContact.Create(
            patient.Id, ContactType.Phone, "+905551234568", true, patient.TenantId);

        // Act
        patient.AddContactInfo(contact1);
        patient.AddContactInfo(contact2);

        // Assert
        contact1.IsPrimary.Should().BeTrue();
        contact2.IsPrimary.Should().BeFalse();
    }

    [Fact]
    public void CalculateAge_ShouldReturnCorrectAge()
    {
        // Arrange
        var dateOfBirth = DateTime.Today.AddYears(-30).AddDays(-100);
        var patient = CreateTestPatient(dateOfBirth: dateOfBirth);

        // Act
        var age = patient.CalculateAge();

        // Assert
        age.Should().Be(29);
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        // Arrange
        var patient = CreateTestPatient();

        // Act
        patient.Deactivate();

        // Assert
        patient.Status.Should().Be(PatientStatus.Inactive);
        patient.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890")]
    public void UpdateBasicInfo_WithInvalidName_ShouldThrow(string invalidName)
    {
        // Arrange
        var patient = CreateTestPatient();

        // Act & Assert
        var action = () => patient.UpdateBasicInfo(
            invalidName, "Yılmaz", Gender.Male, patient.DateOfBirth, null);
        
        action.Should().Throw<ArgumentException>();
    }

    private Patient CreateTestPatient(DateTime? dateOfBirth = null)
    {
        return Patient.Create(
            "P20240001",
            "12345678901",
            "Ahmet",
            "Yılmaz",
            Gender.Male,
            dateOfBirth ?? new DateTime(1990, 5, 15),
            null,
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}
```

## 1.2 Appointment Service Tests

```csharp
/// <summary>
/// Appointment service unit tests.
/// Ne: Appointment service için birim testleri.
/// Neden: Business logic testi için gereklidir.
/// </summary>
public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepository;
    private readonly Mock<IAppointmentSlotRepository> _slotRepository;
    private readonly Mock<IPatientRepository> _patientRepository;
    private readonly Mock<IEmployeeRepository> _employeeRepository;
    private readonly Mock<IMediator> _mediator;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _appointmentRepository = new Mock<IAppointmentRepository>();
        _slotRepository = new Mock<IAppointmentSlotRepository>();
        _patientRepository = new Mock<IPatientRepository>();
        _employeeRepository = new Mock<IEmployeeRepository>();
        _mediator = new Mock<IMediator>();
        
        _service = new AppointmentService(
            _appointmentRepository.Object,
            _slotRepository.Object,
            _patientRepository.Object,
            _employeeRepository.Object,
            _mediator.Object,
            Mock.Of<ILogger<AppointmentService>>());
    }

    [Fact]
    public async Task CreateAppointment_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var physicianId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var request = new CreateAppointmentRequest
        {
            PatientId = patientId,
            PhysicianId = physicianId,
            DepartmentId = departmentId,
            AppointmentDate = DateTime.Today.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            AppointmentType = AppointmentType.Checkup,
            Reason = "Checkup"
        };

        var patient = CreateTestPatient(patientId);
        var physician = CreateTestEmployee(physicianId);
        var department = CreateTestDepartment(departmentId);

        _patientRepository.Setup(x => x.GetByIdAsync(patientId, tenantId))
            .ReturnsAsync(patient);
        _employeeRepository.Setup(x => x.GetByIdAsync(physicianId))
            .ReturnsAsync(physician);
        _appointmentRepository.Setup(x => x.GetByPhysicianAndDateAsync(physicianId, 
            request.AppointmentDate, tenantId))
            .ReturnsAsync(new List<Appointment>());
        _appointmentRepository.Setup(x => x.GenerateAppointmentNumberAsync(tenantId, 
            request.AppointmentDate))
            .ReturnsAsync("APT-20240115-00001");

        // Act
        var result = await _service.CreateAppointmentAsync(request, tenantId, Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
        result.PhysicianId.Should().Be(physicianId);
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        
        _appointmentRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Once);
        _appointmentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAppointment_WithNonExistentPatient_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            PhysicianId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            AppointmentDate = DateTime.Today.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            AppointmentType = AppointmentType.Checkup
        };

        _patientRepository.Setup(x => x.GetByIdAsync(request.PatientId, tenantId))
            .ReturnsAsync((Patient?)null);

        // Act & Assert
        var action = () => _service.CreateAppointmentAsync(request, tenantId, Guid.NewGuid());
        
        await action.Should().ThrowAsync<PatientNotFoundException>();
    }

    [Fact]
    public async Task CreateAppointment_WithOverlappingTime_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var date = DateTime.Today.AddDays(1);
        var existingAppointment = Appointment.Create(
            "APT-20240115-00001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            date,
            new TimeSpan(9, 0, 0),
            new TimeSpan(9, 30, 0),
            AppointmentType.Checkup,
            null,
            tenantId,
            Guid.NewGuid());

        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            PhysicianId = existingAppointment.PhysicianId,
            DepartmentId = Guid.NewGuid(),
            AppointmentDate = date,
            StartTime = new TimeSpan(9, 15, 0), // Overlaps with existing
            AppointmentType = AppointmentType.Checkup
        };

        _patientRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), tenantId))
            .ReturnsAsync(CreateTestPatient());
        _employeeRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateTestEmployee());
        _appointmentRepository.Setup(x => x.GetByPhysicianAndDateAsync(
            It.IsAny<Guid>(), date, tenantId))
            .ReturnsAsync(new List<Appointment> { existingAppointment });

        // Act & Assert
        var action = () => _service.CreateAppointmentAsync(request, tenantId, Guid.NewGuid());
        
        await action.Should().ThrowAsync<AppointmentSlotNotAvailableException>();
    }

    private Patient CreateTestPatient(Guid? id = null)
    {
        return Patient.Create(
            "P20240001",
            "12345678901",
            "Ahmet",
            "Yılmaz",
            Gender.Male,
            new DateTime(1990, 5, 15),
            null,
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    private Employee CreateTestEmployee(Guid? id = null)
    {
        return Employee.Create(
            "E20240001",
            "12345678901",
            "Dr. Mehmet",
            "Demir",
            Gender.Male,
            new DateTime(1980, 1, 1),
            EmployeeType.Physician,
            Guid.NewGuid(),
            null,
            null,
            new DateTime(2020, 1, 1),
            50000,
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    private Department CreateTestDepartment(Guid? id = null)
    {
        return Department.Create(
            "D001",
            "Dahiliye",
            "Internal Medicine Department",
            null,
            DepartmentType.Medical,
            "1",
            "A",
            50,
            Guid.NewGuid());
    }
}
```

---

# PART 2: INTEGRATION TESTS

## 2.1 Patient API Integration Tests

```csharp
/// <summary>
/// Patient API integration tests.
/// Ne: Patient API entegrasyon testleri.
/// Neden: End-to-end test için gereklidir.
/// </summary>
public class PatientApiTests : IClassFixture<HbysWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly HbysWebApplicationFactory _factory;

    public PatientApiTests(HbysWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            turkishId = "12345678901", // Would be encrypted in real scenario
            firstName = "Ahmet",
            lastName = "Yılmaz",
            gender = 1,
            dateOfBirth = "1990-05-15",
            bloodType = "A+",
            contactInfos = new[]
            {
                new { contactType = 1, value = "+905551234567", isPrimary = true }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/patients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PatientDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Ahmet");
        result.PatientNumber.Should().StartWith("P");
    }

    [Fact]
    public async Task CreatePatient_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            turkishId = "invalid",
            firstName = "", // Invalid - empty
            lastName = "Yılmaz",
            gender = 1,
            dateOfBirth = "1990-05-15"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/patients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("validation");
    }

    [Fact]
    public async Task GetPatient_WithValidId_ShouldReturnOk()
    {
        // Arrange - Create a patient first
        var createRequest = new
        {
            turkishId = "12345678902",
            firstName = "Mehmet",
            lastName = "Demir",
            gender = 1,
            dateOfBirth = "1985-03-10"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/patients", createRequest);
        var patient = JsonSerializer.Deserialize<PatientDto>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.GetAsync($"/api/v1/patients/{patient!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PatientDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        result!.FirstName.Should().Be("Mehmet");
    }

    [Fact]
    public async Task SearchPatients_WithSearchTerm_ShouldReturnMatchingPatients()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/patients/search?searchTerm=Ahmet&pageNumber=1&pageSize=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginatedResult<PatientListDto>>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.FullName.Should().Contain("Ahmet"));
    }
}

/// <summary>
/// Test database setup.
/// Ne: Test veritabanı kurulumu.
/// </summary>
public class HbysWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            // Remove real DbContext
            var descriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<HbysDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add test DbContext
            services.AddDbContext<HbysDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Add test services
            services.AddScoped<IEncryptionService, TestEncryptionService>();
        });
    }
}
```

---

# PART 3: PERFORMANCE TESTS

## 3.1 Load Test Scenarios

```yaml
# k6 load test configuration
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '2m', target: 100 },  // Ramp up
    { duration: '5m', target: 100 }, // Steady state
    { duration: '2m', target: 200 }, // Spike
    { duration: '5m', target: 200 }, // Steady state at high load
    { duration: '2m', target: 0 },   // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
    http_req_failed: ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'https://api.hbys.com.tr';
const TOKEN = __ENV.TOKEN;

const headers = {
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${TOKEN}`,
};

// Patient Search Test
export function patientSearch() {
  const searchTerms = ['Ahmet', 'Mehmet', 'Ali', 'Ayşe', 'Fatma'];
  const randomTerm = searchTerms[Math.floor(Math.random() * searchTerms.length)];
  
  const res = http.get(
    `${BASE_URL}/api/v1/patients/search?searchTerm=${randomTerm}&pageNumber=1&pageSize=20`,
    { headers }
  );

  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
    'has results': (r) => {
      const body = JSON.parse(r.body);
      return body.items && body.items.length > 0;
    },
  });

  sleep(1);
}

// Appointment Creation Test
export function createAppointment() {
  const payload = JSON.stringify({
    patientId: '00000000-0000-0000-0000-000000000001',
    physicianId: '00000000-0000-0000-0000-000000000002',
    departmentId: '00000000-0000-0000-0000-000000000003',
    appointmentDate: '2024-03-15',
    startTime: '09:00',
    appointmentType: 1,
    reason: 'Performance test'
  });

  const res = http.post(
    `${BASE_URL}/api/v1/appointments`,
    payload,
    { headers }
  );

  check(res, {
    'status is 201 or 400': (r) => r.status === 201 || r.status === 400,
    'response time < 1000ms': (r) => r.timings.duration < 1000,
  });

  sleep(1);
}

// Patient Retrieval Test
export function getPatient() {
  const patientIds = [
    '00000000-0000-0000-0000-000000000001',
    '00000000-0000-0000-0000-000000000002',
    '00000000-0000-0000-0000-000000000003',
  ];
  const randomId = patientIds[Math.floor(Math.random() * patientIds.length)];

  const res = http.get(
    `${BASE_URL}/api/v1/patients/${randomId}`,
    { headers }
  );

  check(res, {
    'status is 200 or 404': (r) => r.status === 200 || r.status === 404,
    'response time < 300ms': (r) => r.timings.duration < 300,
  });

  sleep(1);
}

export default function () {
  patientSearch();
  getPatient();
  createAppointment();
}
```

---

# PART 4: INFRASTRUCTURE AS CODE

## 4.1 Terraform Configuration

```hcl
# Terraform configuration for HBYS infrastructure
terraform {
  required_version = ">= 1.5.0"
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.75"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "hbys-terraform"
    storage_account_name = "hbysterrafstate"
    container_name       = "tfstate"
    key                  = "hbys-prod.terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id
}

# Variables
variable "subscription_id" {
  description = "Azure subscription ID"
  type        = string
}

variable "tenant_id" {
  description = "Azure tenant ID"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
  default     = "prod"
}

# Resource Group
resource "azurerm_resource_group" "hbys" {
  name     = "rg-hbys-${var.environment}"
  location = "Turkey North"
  
  tags = {
    Environment = var.environment
    Project     = "HBYS"
    ManagedBy   = "Terraform"
  }
}

# Virtual Network
resource "azurerm_virtual_network" "hbys" {
  name                = "vnet-hbys-${var.environment}"
  resource_group_name = azurerm_resource_group.hbys.name
  location            = azurerm_resource_group.hbys.location
  address_space       = ["10.0.0.0/16"]
  
  tags = {
    Environment = var.environment
  }
}

# Subnets
resource "azurerm_subnet" "app" {
  name                 = "snet-app"
  resource_group_name  = azurerm_resource_group.hbys.name
  virtual_network_name = azurerm_virtual_network.hbys.name
  address_prefixes     = ["10.0.1.0/24"]
}

resource "azurerm_subnet" "db" {
  name                 = "snet-db"
  resource_group_name  = azurerm_resource_group.hbys.name
  virtual_network_name = azurerm_virtual_network.hbys.name
  address_prefixes     = ["10.0.2.0/24"]
}

resource "azurerm_subnet" "redis" {
  name                 = "snet-redis"
  resource_group_name  = azurerm_resource_group.hbys.name
  virtual_network_name = azurerm_virtual_network.hbys.name
  address_prefixes     = ["10.0.3.0/24"]
}

# App Service Plan
resource "azurerm_service_plan" "hbys" {
  name                = "asp-hbys-${var.environment}"
  resource_group_name = azurerm_resource_group.hbys.name
  location            = azurerm_resource_group.hbys.location
  os_type             = "Linux"
  sku_name            = "P1v3"
  worker_count        = 3
  
  tags = {
    Environment = var.environment
  }
}

# SQL Server
resource "azurerm_mssql_server" "hbys" {
  name                         = "sql-hbys-${var.environment}"
  resource_group_name          = azurerm_resource_group.hbys.name
  location                     = azurerm_resource_group.hbys.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
  
  tags = {
    Environment = var.environment
  }
}

# SQL Database
resource "azurerm_mssql_database" "hbys" {
  name      = "hbys-${var.environment}"
  server_id = azurerm_mssql_server.hbys.id
  
  sku_name = "S3"
  max_size_gb = 250
  
  tags = {
    Environment = var.environment
  }
}

# Redis Cache
resource "azurerm_redis_cache" "hbys" {
  name                = "redis-hbys-${var.environment}"
  resource_group_name = azurerm_resource_group.hbys.name
  location            = azurerm_resource_group.hbys.location
  capacity            = 2
  family              = "C"
  sku_name            = "Standard"
  
  redis_configuration {
    maxmemory_reserved = 512
    maxmemory_policy  = "allkeys-lru"
  }
  
  tags = {
    Environment = var.environment
  }
}

# Key Vault
resource "azurerm_key_vault" "hbys" {
  name                       = "kv-hbys-${var.environment}"
  resource_group_name        = azurerm_resource_group.hbys.name
  location                   = azurerm_resource_group.hbys.location
  tenant_id                  = var.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 90
  
  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }
  
  tags = {
    Environment = var.environment
  }
}
```

---

# PART 5: DATABASE MIGRATIONS

## 5.1 Entity Framework Migration

```csharp
/// <summary>
/// Initial migration.
/// Ne: İlk veritabanı migrasyonu.
/// </summary>
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Tenants table
        migrationBuilder.CreateTable(
            name: "Tenants",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                TenantType = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "UK_Tenants_TenantCode",
            schema: "Identity",
            table: "Tenants",
            column: "TenantCode",
            unique: true);

        // Users table
        migrationBuilder.CreateTable(
            name: "Users",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                FailedLoginAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalSchema: "Identity",
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Patients table
        migrationBuilder.CreateTable(
            name: "Patients",
            schema: "Patient",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PatientNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                TurkishId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Gender = table.Column<int>(type: "int", nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                BloodType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Patients", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Patients", schema: "Patient");
        migrationBuilder.DropTable(name: "Users", schema: "Identity");
        migrationBuilder.DropTable(name: "Tenants", schema: "Identity");
    }
}
```

---

Bu dokümantasyon, HBYS sisteminin test ve altyapı detaylarını içermektedir. Unit tests, integration tests, performance tests (k6), Terraform IaC, ve database migrations dahildir.