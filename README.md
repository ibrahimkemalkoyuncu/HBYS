# HBYS - Hastane Bilgi Yönetim Sistemi

**Versiyon:** 1.0.0  
**Tarih:** 2026-02-18  
**Geliştirici:** Afney Software House - Kemal (Software Architect)

---

## 📋 Proje Hakkında

HBYS (Hastane Bilgi Yönetim Sistemi), Türkiye için SaaS + On-Prem + Grup Hastane destekli kapsamlı bir hastane bilgi yönetim sistemidir. Modern .NET 10 teknolojileri ve mimari best practices kullanılarak geliştirilmektedir.

## 🏗️ Mimari Özellikler

### Teknoloji Stack
- **.NET 10** (LTS)
- **Modular Monolith** (Extraction-Ready)
- **DDD** (Domain-Driven Design)
- **Vertical Slice Architecture**
- **MediatR** (CQRS Pattern)
- **Minimal API**
- **EF Core** (Primary ORM)
- **Dapper** (Read-heavy senaryolar)
- **Angular 21** (Frontend - Planlanan)
- **SQL Server**
- **Redis Cache** (Tenant-aware)
- **Serilog** (Structured Logging)

### Mimari Prensipler
- Tenant isolation (Zorunlu)
- License feature flag (Zorunlu)
- Immutable audit log
- Açıklama dili: Türkçe
- Kod dili: İngilizce

---

## 📦 Kullanılan Paketler

### HBYS.Api
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Seq" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.9.0" />
```

### HBYS.Persistence
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
```

### HBYS.Application
```xml
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="FluentValidation" Version="11.11.0" />
```

---

## 🚀 Başlangıç

### Gereksinimler
- .NET 10 SDK
- SQL Server (LocalDB veya tam sürüm)
- Visual Studio 2022+ veya VS Code

### Kurulum

1. Repoyu klonlayın:
```bash
git clone https://github.com/ibrahimkemalkoyuncu/HBYS.git
cd HBYS
```

2. Projeleri restore edin:
```bash
dotnet restore
```

3. Uygulamayı çalıştırın:
```bash
dotnet run --project src/HBYS.Api
```

### API Endpoints
- **Swagger UI:** http://localhost:5292/swagger
- **Health Check:** http://localhost:5292/api/tenants/health

---

## 📁 Proje Yapısı

```
HBYS/
├── src/
│   ├── HBYS.Api/           # Web API Layer
│   │   ├── Controllers/    # API Controllers
│   │   ├── Program.cs      # Uygulama başlangıcı
│   │   └── Properties/     # Launch settings
│   ├── HBYS.Application/  # Application Services
│   │   ├── Services/       # Business logic
│   │   └── Validators/     # FluentValidation
│   ├── HBYS.Domain/       # Domain Entities
│   │   ├── Entities/       # Entity classes
│   │   └── Interfaces/    # Repository interfaces
│   ├── HBYS.Infrastructure/ # Cross-cutting concerns
│   │   └── Services/       # Infrastructure services
│   └── HBYS.Persistence/   # Data access
│       └── Context/        # EF Core DbContext
├── docs/                   # Documentation
└── HBYS.sln               # Solution file
```

---

## 📊 Sprint Modeli

### FAZ 0 (Sprint 1-4): Temel Altyapı
- Identity & Authentication
- Tenant Management
- License Management
- Audit Logging
- Configuration

### FAZ 1 (Sprint 6-10): Hasta Yönetimi
- Patient Management
- Appointment
- Outpatient
- Billing

### FAZ 2 (Sprint 12-16): Klinik Operasyonlar
- Inpatient
- Emergency
- Pharmacy
- Inventory
- Procurement

### FAZ 3 (Sprint 18-20): Tanı Hizmetleri
- Laboratory
- Radiology

### FAZ 4 (Sprint 22-24): Finans & Raporlama
- Accounting
- Reporting

### FAZ 5 (Sprint 26-30): İnsan Kaynakları
- HR
- Quality
- Document
- Notification

### FAZ 6 (Sprint 31-36): İleri Seviye
- Integration
- Monitoring
- Data Warehouse
- API Gateway
- Multi-Hospital Orchestration

---

## 🔐 Güvenlik

- JWT Authentication (Planlanan)
- Role-based Access Control (RBAC)
- Tenant-based Data Isolation
- Immutable Audit Logs

---

## 📝 Lisans

MIT License

---

## 👨‍💻 Geliştirici

**Kemal** - Software Architect  
Afney Software House

---

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın
