# HBYS - Hastane Bilgi Yönetim Sistemi
## Enterprise SaaS + On-Prem + Grup Hastane Mimarisi

**Versiyon:** 1.0.0  
**Tarih:** 2026-02-18  
**Yazar:** Kemal (Afney Software House - Yazılım Mimarı)  
**Durum:** Mimari Tasarım Dokümantasyonu  

---

# 1. MİMARİ GENEL BAKIŞ

## 1.1 Temel Prensipler

### 1.1.1 Mimari Yaklaşım
Bu HBYS sistemi, **Modular Monolith** (Modüler Monolit) mimarisi üzerine inşa edilmektedir. Bu yaklaşım, başlangıçta microservices'e gerek kalmadan domain-driven design prensiplerine uygun, bağımsız olarak geliştirilebilen modüller oluşturmamızı sağlar. Her modül kendi veritabanı şemasına, API'sine ve iş mantığına sahip olacaktır. Zamanla, belirli modüller (örneğin Laboratory, Radiology gibi yüksek yüklü sistemler) extraction ölçümlerine göre bağımsız microservice'lere taşınabilir. Bu yaklaşım, gereksiz erken soyutlama maliyetinden kaçınırken, sistem evrimine hazır bir altyapı sunar.

### 1.1.2 Evrimsel Sprint Modeli
Proje, klasik waterfall veya büyük bang yaklaşımı yerine **Evrimsel Sprint Modeli** ile geliştirilecektir. Her sprint sonunda çalışan, entegre bir sistem elde edilecektir. Bu sayede, her faz sonunda stakeholder'lara somut değer sunulabilecektir. Sprint'ler arası geri bildirim döngüleri, mimari kararların esnekliğini sağlayacaktır. Özellikle FAZ 0'ın sağlam temeller atması, sonraki fazların başarısı için kritik öneme sahiptir.

## 1.2 Teknoloji Stack

### 1.2.1 Backend Teknolojileri
Sistem backend'inde **.NET 10** (LTS versiyonu kullanılacaktır) tercih edilmiştir. Bu seçim, Microsoft'un uzun vadeli destek garantisi, güçlü ekosistem ve Türkiye'deki .NET geliştirici pazarının genişliği göz önünde bulundurularak yapılmıştır. **Minimal API** yaklaşımı ile hafif, performanslı endpoint'ler oluşturulacaktır. **MediatR** pattern'i, CQRS implementasyonu için kullanılacak ve komut sorgu sorumluluğunun ayrılmasını sağlayacaktır. Veri erişim katmanında **Entity Framework Core** (primary) ve **Dapper** (read-heavy senaryolar için) birlikte kullanılacaktir. EF Core migration ve schema yönetimi için, Dapper ise raporlama ve dashboard sorguları için optimize edilecektir.

### 1.2.2 Frontend Teknolojileri
Frontend tarafında **Angular 21** kullanılacaktır. Angular'ın strong typing, modular yapı ve enterprise-ready olması, uzun vadeli bakım kolaylığı sağlayacaktır. Lazy loading ve signal-based reactivity, kullanıcı deneyimini iyileştirecektir. Tüm component'ler standalone olacak ve modüler yapı desteklenecektir.

### 1.2.3 Veritabanı ve Cache
**SQL Server** primary veritabanı olarak kullanılacaktır. Multi-tenant mimari ile her tenant'ın mantıksal izolasyonu sağlanacaktır (schema-based veya database-based). **Redis Cache** tenant-aware olacak şekilde yapılandırılacak ve session, distributed cache, ve rate limiting için kullanılacaktır.

### 1.2.4 Altyapı
Logging için **Serilog** structured logging formatında uygulanacaktır. Tüm log'lar JSON formatında tutulacak ve aranabilir olacaktır. Audit log'ları immutable olarak tasarlanacak ve WORM (Write Once Read Many) prensibiyle saklanacaktır.

## 1.3 Deployment Modelleri

### 1.3.1 SaaS (Multi-Tenant)
Büyük ölçekli hastaneler ve zincir hastaneler için SaaS modeli sunulacaktır. Multi-tenant architecture ile kaynak optimizasyonu sağlanacaktır. Tenant başına izolasyon zorunlu olacak ve veri sızıntısı riski minimize edilecektir.

### 1.3.2 On-Premise
Küçük ve orta ölçekli hastaneler için on-premise kurulum seçeneği sunulacaktır. Docker/Kubernetes container desteği ile kolay deployment sağlanacaktır. Lisans modeli, makine başına veya kullanıcı başına olabilecektir.

### 1.3.3 Grup Hastane
Birden fazla hastaneyi yöneten holding yapıları için merkezi yönetim konsolu sunulacaktır. Cross-tenant raporlama, tedarik zinciri yönetimi ve konsolide billing özellikleri desteklenecektir.

---

# 2. MİMARİ YASALAR

## 2.1 Zorunlu Teknoloji Seçimleri

Bu bölümde belirtilen teknolojiler, mimari team anlaşması ile kesinleştirilmiştir ve proje boyunca değiştirilemez:

- **Runtime:** .NET 10 (LTS)
- **API Style:** Minimal API
- **Mediator:** MediatR
- **ORM:** EF Core (Primary) + Dapper (Read-heavy)
- **Frontend:** Angular 21
- **Database:** SQL Server
- **Cache:** Redis (tenant-aware)
- **Logging:** Serilog
- **Audit:** Immutable audit log

## 2.2 Mimari Kısıtlamalar

### 2.2.1 Tenant İzolasyonu Zorunluluğu
Her tenant'ın verisi kesinlikle izole edilmelidir. Cross-tenant veri erişimi sadece yetkili roller tarafından, audit log'lanarak yapılabilmelidir. Tenant ID her tabloarda zorunlu foreign key olarak bulunacaktır. Query filter'ları otomatik olarak tenant ID uygulayacaktır.

### 2.2.2 Lisans Feature Flag Zorunluluğu
Tüm özellikler feature flag sistemi ile yönetilecektir. Lisans anahtarı tenant'a özel olacak ve module-based licensing uygulanacaktır. Deneme süresi, paket değişikliği ve grace period desteği sağlanacaktır.

### 2.2.3 Dil Kuralları
Açıklama dili Türkçe olacaktır. Tüm class, method, property açıklamaları Türkçe yazılacaktır. Kod dili İngilizce olacaktır. Domain terminology'si için Türkçe karşılıklar kullanılacaktır (örn: Hasta, Randevu, Fatura).

---

# 3. SPRINT EVRİM MODELİ DETAYLARI

## 3.1 Faz Yapısı

### FAZ 0: Temel Altyapı (Sprint 1-4 + Stabilizasyon)
Bu faz, tüm sistemin temelini oluşturmaktadır. Başarısızlık durumunda diğer fazlar başlayamayacağı için bu faza özel önem verilmelidir. Identity management, tenant yönetimi, lisanslama, audit ve konfigürasyon sistemleri bu fazda geliştirilecektir. Sprint 5'te stabilizasyon yapılacak ve sistem deploy edilebilir hale getirilecektir.

### FAZ 1: Ayaktan Hasta Hizmetleri (Sprint 6-10 + Stabilizasyon)
Bu faz, hastanelerin günlük operasyonlarını destekleyen temel hasta hizmetlerini kapsamaktadır. Hasta kayıt, randevu yönetimi, poliklinik muayenesi ve faturalama modülleri bu fazda geliştirilecektir.

### FAZ 2: Yatan Hasta ve Acil Hizmetler (Sprint 12-16 + Stabilizasyon)
Bu faz, yatan hasta yönetimi, acil servis operasyonları, eczane ve stok yönetimi modüllerini kapsamaktadır. İlaç takibi ve sarf malzeme yönetimi kritik öneme sahiptir.

### FAZ 3: Tıbbi Görüntüleme ve Laboratuvar (Sprint 18-20 + Stabilizasyon)
Bu faz, tıbbi laboratuvar sonuçları ve radyoloji görüntüleme sistemlerinin entegrasyonunu kapsamaktadır. LIS (Laboratory Information System) ve PACS (Picture Archiving and Communication System) entegrasyonları bu fazda ele alınacaktır.

### FAZ 4: Muhasebe ve Raporlama (Sprint 22-24 + Stabilizasyon)
Bu faz, mali işlemler, muhasebe entegrasyonu ve kapsamlı raporlama altyapısını kapsamaktadır.

### FAZ 5: İnsan Kaynakları ve Kalite (Sprint 26-30)
Bu faz, personel yönetimi, kalite belgelendirme, doküman yönetimi ve bildirim sistemlerini kapsamaktadır.

### FAZ 6: Entegrasyon ve İzleme (Sprint 31-36)
Bu faz, harici sistem entegrasyonları, API gateway, monitoring, data warehouse ve multi-hospital orchestration konularını kapsamaktadır.

---

# 4. MODÜLER YAPILANDIRMA

## 4.1 Solution Yapısı

```
HBYS.sln
├── src/
│   ├── HBYS.Modules/
│   │   ├── HBYS.Modules.Identity/
│   │   ├── HBYS.Modules.Tenant/
│   │   ├── HBYS.Modules.License/
│   │   ├── HBYS.Modules.Audit/
│   │   ├── HBYS.Modules.Configuration/
│   │   ├── HBYS.Modules.Patient/
│   │   ├── HBYS.Modules.Appointment/
│   │   ├── HBYS.Modules.Outpatient/
│   │   ├── HBYS.Modules.Billing/
│   │   ├── HBYS.Modules.Inpatient/
│   │   ├── HBYS.Modules.Emergency/
│   │   ├── HBYS.Modules.Pharmacy/
│   │   ├── HBYS.Modules.Inventory/
│   │   ├── HBYS.Modules.Procurement/
│   │   ├── HBYS.Modules.Laboratory/
│   │   ├── HBYS.Modules.Radiology/
│   │   ├── HBYS.Modules.Accounting/
│   │   ├── HBYS.Modules.Reporting/
│   │   ├── HBYS.Modules.HR/
│   │   ├── HBYS.Modules.Quality/
│   │   ├── HBYS.Modules.Document/
│   │   ├── HBYS.Modules.Notification/
│   │   ├── HBYS.Modules.Integration/
│   │   ├── HBYS.Modules.Monitoring/
│   │   ├── HBYS.Modules.DataWarehouse/
│   │   └── HBYS.Modules.APIGateway/
│   ├── HBYS.Shared/
│   │   ├── HBYS.SharedKernel/
│   │   ├── HBYS.SharedKernel.Common/
│   │   ├── HBYS.SharedKernel.Domain/
│   │   ├── HBYS.SharedKernel.Events/
│   │   └── HBYS.SharedKernel.Exceptions/
│   ├── HBYS.Infrastructure/
│   │   ├── HBYS.Infrastructure.Persistence/
│   │   ├── HBYS.Infrastructure.Cache/
│   │   ├── HBYS.Infrastructure.Logging/
│   │   ├── HBYS.Infrastructure.Auth/
│   │   └── HBYS.Infrastructure.MessageBus/
│   ├── HBYS.Api/
│   │   └── (Minimal API Entry Point)
│   └── HBYS.BackgroundJobs/
├── tests/
│   └── (Her modül için test projeleri)
└── docs/
    └── (Mimari dokümantasyon)
```

## 4.2 Modül Yapısı (Vertical Slice)

Her modül, vertical slice mimarisine uygun olarak aşağıdaki yapıda olacaktır:

```
HBYS.Modules.{ModuleName}/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Aggregates/
│   ├── Events/
│   └── Interfaces/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Validators/
│   ├── DTOs/
│   └── Interfaces/
├── Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   └── Services/
├── Api/
│   ├── Endpoints/
│   ├── Controllers/
│   └── Filters/
└── Tests/
    ├── Unit/
    └── Integration/
```

---

# 5. MİMARİ KRİTİK KARARLAR

## 5.1 Multi-Tenant Yaklaşımı

### 5.1.1 Tenant İzolasyon Stratejisi
Sistemde **Schema-Based Multi-Tenancy** yaklaşımı benimsenmiştir. Her tenant, paylaşımlı bir veritabanında ayrı bir schema'ya sahip olacaktır. Bu yaklaşım, maliyet etkinliği sağlarken, izolasyon seviyesi gereksinimlerini karşılamaktadır. On-Premise senaryolarında tenant başına ayrı veritabanı kullanılabilecektir.

### 5.1.2 Tenant Context
Tenant context, her istek için middleware tarafından çözümlenecektir. Header, subdomain veya JWT token üzerinden tenant bilgisi alınacaktır. Tenant context, tüm repository ve service katmanlarına otomatik olarak enjekte edilecektir.

## 5.2 CQRS Implementasyonu

### 5.2.1 Command Side
Tüm write operasyonları MediatR Command olarak implemente edilecektir. Her command, ayrı bir handler tarafından işlenecektir. Validation, business rule check ve domain events dispatching bu katmanda yapılacaktır. Command sonucu olarak, başarılı ise ID veya aggregate root, başarısız ise hata mesajları dönecektir.

### 5.2.2 Query Side
Read operasyonları için ayrı query handler'lar kullanılacaktır. Raporlama ve dashboard senaryoları için Dapper kullanılacaktır. Query'ler, read model'lere göre şekillendirilecek ve gereksiz join'lerden kaçınılacaktır.

## 5.3 Event Sourcing Temelleri

### 5.3.1 Domain Events
Domain events, aggregate'ler tarafından tetiklenen olaylardır. Bu events, modüller arası koordinasyon için kullanılacaktır. Event'ler, immutable ve serializable olacaktır. Event handler'lar, outbox pattern kullanılarak eventually consistent olacaktır.

### 5.3.2 Integration Events
Harici sistemlerle entegrasyon için integration events kullanılacaktır. RabbitMQ veya Azure Service Bus üzerinden publish/subscribe modeli uygulanacaktır. Event versioning stratejisi belirlenecektir.

---

# 6. GÜVENLİK MİMARİSİ

## 6.1 Authentication & Authorization

### 6.1.1 JWT Tabanlı Kimlik Doğrulama
Sistem, JWT (JSON Web Token) tabanlı kimlik doğrulama kullanacaktır. Access token ve refresh token mekanizması uygulanacaktır. Token'lar, Redis'te blacklist olarak tutulacak ve logout durumunda geçersiz kılınacaktır.

### 6.1.2 Role-Based Access Control (RBAC)
Rol bazlı erişim kontrolü uygulanacaktır. Her modül için ayrı yetki tanımları yapılacaktır. Permission-based yetkilendirme ile fine-grained kontrol sağlanacaktır.

## 6.2 Data Security

### 6.2.1 Şifreleme
Tüm sensitive veriler (TC Kimlik No, telefon, adres) AES-256 ile şifrelenecektir. Database-level encryption (TDE) aktif olacaktır.

### 6.2.2 Audit
Tüm veri erişim ve değişiklikleri audit log'a kaydedilecektir. Immutable log'lar, WORM uyumlu depolamada saklanacaktır.

---

# 7. PERFORMANS VE ÖLÇEKLENDİRME

## 7.1 Caching Stratejisi

### 7.1.1 Katmanlı Cache
Application-level cache olarak Redis kullanılacaktır. Tenant-aware cache key yapısı uygulanacaktır. Cache invalidation için event-driven yaklaşım benimsenecektir.

### 7.1.2 Query Cache
Sık kullanılan query'ler için output caching uygulanacaktır. Cache süreleri, data freshness gereksinimlerine göre belirlenecektir.

## 7.2 Connection Management

### 7.2.1 Database Connection Pooling
EF Core connection pooling aktif olacaktır. Dapper için manual connection management uygulanacaktır. Connection timeout ve retry policy'leri belirlenecektir.

---

# 8. DEPLOYMENT VE OPERASYON

## 8.1 Container Desteği

### 8.1.1 Docker
Tüm servisler, Docker container olarak deploy edilebilecektir. Multi-stage build ile optimize image'lar oluşturulacaktır.

### 8.1.2 Kubernetes
Kubernetes için Helm chart'ları hazırlanacaktır. HPA (Horizontal Pod Autoscaler) konfigürasyonu yapılacaktır.

## 8.2 CI/CD

### 8.2.1 Pipeline
GitHub Actions veya Azure DevOps kullanılacaktır. Her commit için unit test'ler çalıştırılacaktır. Integration test'ler, pre-production ortamında çalıştırılacaktır.

---

# 9. MİMARİ METRİKLERİ

## 9.1 Ölçülebilir Hedefler

### 9.1.1 Teknik Metrikler
- Code coverage: Minimum %80
- Cyclomatic complexity: Maksimum 15
- Response time P95: Maksimum 200ms
- Uptime: %99.9

### 9.1.2 Extraction Kriterleri
- Modül CPU usage > %30 (sustained)
- Modül memory usage > 2GB
- Modül request throughput > 1000 req/s
- Modül independent deployment gereksinimi

---

# 10. TEKNİK BORÇ KONTROLÜ

## 10.1 Takip Mekanizması

### 10.1.1 Definition of Done
Her sprint için "Definition of Done" kriterleri belirlenecektir. Code review, test coverage, documentation gözden geçirme bu kriterlere dahil olacaktır.

### 10.1.2 Teknik Borç Log
Her sprint sonunda teknik borç listesi güncellenecektir. Borç önceliklendirmesi, business value ve risk bazlı yapılacaktır.

---

# 11. EKLER

## 11.1 Sözlük

| Terim | Açıklama |
|-------|----------|
| Tenant | HBYS kullanan hastane/kurum |
| Module | İşlevsel birim (Patient, Billing vb.) |
| Aggregate | Domain aggregate root |
| Command | Write operasyonu |
| Query | Read operasyonu |
| Event | Domain event |

## 11.2 Referanslar

- Domain-Driven Design (Eric Evans)
- Implementing Domain-Driven Design (Vaughn Vernon)
- Vertical Slice Architecture (Jimmy Bogard)
- Microsoft .NET Documentation
- Angular Documentation
