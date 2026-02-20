# HBYS Eğitim Dokümanı

**Tarih:** 2026-02-18  
**Versiyon:** 1.0.0  
**Yazar:** Kemal (Software Architect) - Afney Software House

---

## 📚 Giriş

Bu doküman, HBYS (Hastane Bilgi Yönetim Sistemi) projesinin mimarisini, yapısını ve geliştirme süreçlerini junior yazılımcılar için detaylı olarak açıklamaktadır.

---

## 🏗️ Projenin Amacı ve Vizyonu

HBYS, Türkiye'deki hastaneler için kapsamlı bir bilgi yönetim sistemi olarak tasarlanmıştır. Sistem şu deployment modellerini destekler:

- **SaaS:** Bulut tabanlı, çoklu tenant desteği
- **On-Prem:** Şirket içi kurulum
- **Grup Hastane:** Birden fazla hastane için merkezi yönetim

---

## 🧱 Mimari Yapı

### 1. Modular Monolith

Proje, **Modular Monolith** mimarisi üzerine inşa edilmiştir. Bu ne anlama geliyor?

**Neden Monolith?**
- Başlangıçta microservices'in karmaşıklığına gerek yok
- Tüm modüller tek process içinde çalışır
- Deployment ve debugging kolaylığı
- Gelecekte ihtiyaç halinde modüller ayrılabilir (Extraction-Ready)

**Modül Yapısı:**
```
src/
├── HBYS.Api           # Web katmanı
├── HBYS.Application   # İş mantığı
├── HBYS.Domain       # Domain entities
├── HBYS.Infrastructure # Cross-cutting
└── HBYS.Persistence  # Veri erişimi
```

### 2. Domain-Driven Design (DDD)

DDD, karmaşık business logic'i modellemek için kullanılan bir yaklaşımdır.

**Temel Kavramlar:**

- **Entity:** Benzersiz kimliği olan nesneler (örn: Patient, Tenant)
- **Value Object:** Kimliksiz, değiştirilemez nesneler (örn: Address, PhoneNumber)
- **Aggregate:** İlişkili entity grupları
- **Domain Service:** Entity'lerde yer almayan iş mantığı
- **Repository:** Veri erişim soyutlaması

### 3. Vertical Slice Architecture

Her özellik (feature), kendi katmanlarını (Controller, Service, Repository) içerir. Bu ne anlama geliyor?

Geleneksel katmanlı mimari:
```
Controllers → Services → Repositories → Database
```

Vertical Slice:
```
Patient/
  ├── CreatePatientEndpoint
  ├── GetPatientEndpoint
  └── UpdatePatientEndpoint
```

**Avantajları:**
- Feature bazlı geliştirme
- Daha az cross-cutting concerns
- Her feature bağımsız olarak test edilebilir

---

## 📦 Teknoloji Stack

### .NET 10
En güncel .NET sürümü. LTS (Long Term Support) özellikleri ile production için güvenilir.

### Entity Framework Core
ORM olarak EF Core kullanıyoruz. Neden?

- LINQ ile type-safe sorgular
- Migration desteği
- Code-first yaklaşımı
- SQL Server entegrasyonu

### Dapper
Read-heavy senaryolar için Dapper kullanılır. Neden?

- EF Core'dan daha hızlı
- Raw SQL desteği
- Mikro-optimizasyon gerektiğinde

### MediatR
CQRS (Command Query Responsibility Segregation) pattern için MediatR kullanılır.

**Örnek:**
```csharp
// Command (Yazma işlemi)
public record CreateTenantCommand(string Name, TenantType Type) : IRequest<Tenant>;

// Handler
public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, Tenant>
{
    public async Task<Tenant> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        // Business logic here
    }
}
```

### Serilog
Structured logging için Serilog kullanılır.

**Örnek:**
```csharp
Log.Information("Tenant created: {TenantName}, {TenantType}", name, type);
```

---

## 🔐 Tenant Isolation (Çoklu Kiracılık)

HBYS, çoklu tenant mimarisi destekler. Her tenant verisi izole edilmiştir.

### Tenant Türleri:
```csharp
public enum TenantType
{
    SaaS,        # Bulut tabanlı
    OnPrem,      # Şirket içi
    GroupHospital # Grup hastane
}
```

### Implementasyon:
Tenant bilgisi HTTP Header üzerinden alınır:
```csharp
X-Tenant-ID: {tenant-id}
```

---

## 📁 Proje Yapısı Detayları

### HBYS.Domain
Domain entity'leri burada yer alır.

**Örnek Entity:**
```csharp
public class Tenant : BaseEntity
{
    public string Name { get; set; }
    public TenantType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### HBYS.Persistence
Veritabanı işlemleri burada yapılır.

**DbContext:**
```csharp
public class HbysDbContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    // ...
}
```

### HBYS.Application
İş mantığı ve business logic burada yer alır.

**Multi-tenant Service:**
```csharp
public class TenantContextAccessor : ITenantContextAccessor
{
    public string GetCurrentTenantId()
    {
        // HTTP Context'ten tenant ID alınır
    }
}
```

### HBYS.Api
Web API katmanı. Minimal API kullanılır.

**Controller Örneği:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "healthy" });
}
```

---

## 🔨 Geliştirme Süreci

### 1. Yeni Entity Ekleme

1. **Domain katmanında entity oluştur:**
```csharp
// src/HBYS.Domain/Entities/Patient.cs
public class Patient : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}
```

2. **Persistence katmanında DbContext'e ekle:**
```csharp
public DbSet<Patient> Patients { get; set; }
```

3. **Migration oluştur:**
```bash
dotnet ef migrations add AddPatient
```

4. **Repository interface oluştur:**
```csharp
public interface IPatientRepository : IRepository<Patient>
{
}
```

### 2. Yeni Endpoint Ekleme

1. **Command/Query oluştur (MediatR ile):**
```csharp
public record GetPatientQuery(Guid Id) : IRequest<Patient>;
```

2. **Handler oluştur:**
```csharp
public class GetPatientHandler : IRequestHandler<GetPatientQuery, Patient>
{
    public async Task<Patient> Handle(GetPatientQuery request, CancellationToken ct)
    {
        // Implementation
    }
}
```

3. **Controller'a endpoint ekle:**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var patient = await mediator.Send(new GetPatientQuery(id));
    return Ok(patient);
}
```

### 3. Logging

Her işlem için Serilog ile loglama yapılmalı:

```csharp
Log.Information("Patient created: {PatientId}, {PatientName}", 
    patient.Id, patient.FullName);
```

---

## 🧪 Test

### Unit Test
Business logic için unit testler yazılmalıdır.

```csharp
[Fact]
public void Patient_Age_Calculated_Correctly()
{
    var patient = new Patient 
    { 
        BirthDate = DateTime.Now.AddYears(-30) 
    };
    
    Assert.Equal(30, patient.Age);
}
```

### Integration Test
API endpoint'leri için integration testler.

---

## 📋 Commit Mesajları

Commit mesajları Türkçe veya İngilizce olabilir. Örnek format:

```
[FEATURE] Patient entity eklendi
[BUGFIX] Tenant ID hatası düzeltildi
[REFACTOR] Repository pattern güncellendi
[DOCS] README güncellendi
```

---

## 🔗 Faydalı Kaynaklar

- [Microsoft EF Core Docs](https://docs.microsoft.com/ef/core/)
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [Serilog Docs](https://serilog.net/)
- [DDD Fundamentals](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

---

## 📞 Yardım

Sorularınız için:
- Proje lead: Kemal (Software Architect)
- Email: [email@domain.com]

---

## 📜 Changelog

### 2026-02-18
- Proje başlangıcı
- FAZ 0 altyapısı (Tenant, User, License)
- Swagger entegrasyonu
- Serilog logging
- SQL Server entegrasyonu

---

*Bu doküman sürekli güncellenmektedir.*
