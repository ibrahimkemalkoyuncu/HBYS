# HBYS Extraction-Ready Event Tasarımı, Risk Analizi, Performans Metrikleri ve Teknik Borç Kontrolü

---

# 1. EXTRACTION-READY EVENT TASARIMI

## 1.1 Event-Driven Mimari Temelleri

### 1.1.1 Domain Events
Domain events, modüller arası gevşek bağlı iletişimi sağlayan temel mekanizmalardır. Her event, belirli bir domain olayını temsil eder ve ilgili handler'lar tarafından işlenir. Bu yapı, microservices'e geçişte kritik öneme sahiptir çünkü event'ler, servisler arası iletişimin ana hatlarını belirler. Event tasarımında dikkat edilmesi gereken en önemli nokta, event'lerin immutable olması ve geçmişe dönük değiştirilememesidir.

### 1.1.2 Event Sourcing Yapısı
Event sourcing, her state değişikliğini bir event olarak kaydetme yaklaşımıdır. Bu yaklaşım, sistemin tam geçmişini tutarak debugging, audit ve replay yetenekleri sağlar. HBYS sisteminde event sourcing, özellikle hasta kayıtları, faturalama ve tedavi order'ları gibi kritik süreçlerde uygulanacaktır. Event store olarak SQL Server veya Cosmos DB kullanılabilir.

## 1.2 Core Domain Events

### 1.2.1 Patient Modülü Events

#### PatientCreatedEvent
**Ne:** Hasta oluşturulduğunda tetiklenen core event'tir.  
**Neden:** Diğer modüllerin hasta oluşumundan haberdar olması için gereklidir.  
**Özelliği:** Event version, patientId, tenantId, registrationDate, patientType, initialDepartmentId içerir.  
**Extraction-Ready:** Hasta kaydı, tüm modüllerle paylaşılabilir ve hasta ID'si üzerinden join yapılabilir.  
**Kim Kullanacak:** Appointment, Outpatient, Inpatient, Emergency, Billing, Pharmacy, Laboratory, Radiology modülleri.  
**Amaç:** Hasta oluşumunun sistem genelinde duyurulması.

#### PatientUpdatedEvent
**Ne:** Hasta bilgileri güncellendiğinde tetiklenen core event'tir.  
**Neden:** Güncel hasta bilgilerinin tüm modüllere yayılması için gereklidir.  
**Özelliği:** Event version, patientId, updatedFields (changed properties), oldValue, newValue, updatedBy, updatedAt içerir.  
**Extraction-Ready:** Güncelleme bilgileri, CDC (Change Data Capture) ile izlenebilir.  
**Kim Kullanacak:** Tüm hasta işlemi yapan modüller.  
**Amaç:** Hasta bilgi güncellemesinin yayılması.

### 1.2.2 Appointment Modülü Events

#### AppointmentCreatedEvent
**Ne:** Randevu oluşturulduğunda tetiklenen core event'tir.  
**Neden:** Randevu oluşumunun ilgili modüllere bildirilmesi için gereklidir.  
**Özelliği:** Event version, appointmentId, patientId, physicianId, departmentId, appointmentDate, appointmentTime, appointmentType, status içerir.  
**Extraction-Ready:** Randevu verileri, patient ve department ID'leri üzerinden izlenebilir.  
**Kim Kullanacak:** Patient, Outpatient, Notification modülleri.  
**Amaç:** Randevu oluşumunun duyurulması.

#### AppointmentCompletedEvent
**Ne:** Randevu tamamlandığında tetiklenen core event'tir.  
**Neden:** Tedavi sürecinin bir sonraki aşamaya geçişi için gereklidir.  
**Özelliği:** Event version, appointmentId, patientId, physicianId, encounterId, completedAt, notes içerir.  
**Extraction-Ready:** Tamamlanma bilgisi, billing ve patient history için kullanılabilir.  
**Kim Kullanacak:** Billing, Patient modülleri.  
**Amaç:** Randevu tamamlanma duyurusu.

### 1.2.3 Billing Modülü Events

#### InvoiceCreatedEvent
**Ne:** Fatura oluşturulduğunda tetiklenen core event'tir.  
**Neden:** Fatura bilgisinin muhasebe ve hasta ile paylaşılması için gereklidir.  
**Özelliği:** Event version, invoiceId, patientId, invoiceType, totalAmount, currency, items (service details) içerir.  
**Extraction-Ready:** Fatura verileri, financial reporting ve accounting sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Accounting, Patient modülleri.  
**Amaç:** Fatura oluşumunun duyurulması.

#### PaymentReceivedEvent
**Ne:** Ödeme alındığında tetiklenen core event'tir.  
**Neden:** Ödeme durumunun güncellenmesi için gereklidir.  
**Özelliği:** Event version, paymentId, invoiceId, amount, paymentMethod, transactionId, receivedAt içerir.  
**Extraction-Ready:** Ödeme verileri, revenue tracking için kullanılabilir.  
**Kim Kullanacak:** Accounting modülü.  
**Amaç:** Ödeme duyurusu.

### 1.2.4 Laboratory Modülü Events

#### LabResultReadyEvent
**Ne:** Laboratuvar sonucu hazır olduğunda tetiklenen core event'tir.  
**Neden:** Sonuç bilgisinin hekim ile paylaşılması için gereklidir.  
**Özelliği:** Event version, resultId, orderId, patientId, testName, isAbnormal, resultValue, referenceRange içerir.  
**Extraction-Ready:** Sonuç verileri, clinical decision support sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Ordering modül (hekim bildirimi).  
**Amaç:** Sonuç duyurusu.

#### CriticalLabResultEvent
**Ne:** Kritik laboratuvar sonucu tespit edildiğinde tetiklenen core event'tir.  
**Neden:** Acil müdahale gerektiren durumlar için gereklidir.  
**Özelliği:** Event version, resultId, patientId, testName, criticalValue, normalRange, detectedAt, notifyPhysicians, notifyPatient içerir.  
**Extraction-Ready:** Kritik değerler, code blue ve acil bildirim sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Notification, Emergency modülleri.  
**Amaç:** Kritik sonuç uyarısı.

### 1.2.5 Radiology Modülü Events

#### RadiologyStudyCompletedEvent
**Ne:** Radyoloji çalışması tamamlandığında tetiklenen core event'tir.  
**Neden:** Görüntülerin raporlama için hazır olduğunu bildirir.  
**Özelliği:** Event version, studyId, orderId, patientId, modality, instanceCount, pacsUid içerir.  
**Extraction-Ready:** DICOM verileri, PACS sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Radiology modülü (raporlama).  
**Amaç:** Çalışma tamamlanma duyurusu.

#### RadiologyReportReadyEvent
**Ne:** Radyoloji raporu hazır olduğunda tetiklenen core event'tir.  
**Neden:** Rapor bilgisinin hekim ile paylaşılması için gereklidir.  
**Özelliği:** Event version, reportId, studyId, patientId, radiologistId, findings, impression içerir.  
**Extraction-Ready:** Rapor verileri, EHR sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Ordering modülü.  
**Amaç:** Rapor duyurusu.

## 1.3 Integration Events

### 1.3.1 Cross-Module Integration Events

#### PatientAdmittedEvent
**Ne:** Hasta yatırıldığında tetiklenen integration event'tir.  
**Neden:** Yatış bilgisinin ilgili modüllere yayılması için gereklidir.  
**Özelliği:** Event version, admissionId, patientId, roomId, bedId, departmentId, admittedAt, attendingPhysicianId içerir.  
**Extraction-Ready:** Yatış verileri, bed management ve nursing care sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Nursing, Pharmacy, Laboratory, Billing modülleri.  
**Amaç:** Yatış duyurusu.

#### PatientDischargedEvent
**Ne:** Hasta taburcu edildiğinde tetiklenen integration event'tir.  
**Neden:** Taburculuk bilgisinin ilgili modüllere yayılması için gereklidir.  
**Özelliği:** Event version, dischargeId, admissionId, patientId, dischargeType, dischargeSummary, followUpInstructions, dischargedAt içerir.  
**Extraction-Ready:** Taburculuk verileri, discharge planning ve home care sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Billing, Pharmacy, Follow-up Appointment modülleri.  
**Amaç:** Taburculuk duyurusu.

#### PrescriptionIssuedEvent
**Ne:** Reçete düzenlendiğinde tetiklenen integration event'tir.  
**Neden:** Reçete bilgisinin eczane ile paylaşılması için gereklidir.  
**Özelliği:** Event version, prescriptionId, patientId, physicianId, encounterId, items (medication details), issuedAt içerir.  
**Extraction-Ready:** Reçete verileri, pharmacy dispensing ve drug interaction sistemleri ile entegre edilebilir.  
**Kim Kullanacak:** Pharmacy modülü.  
**Amaç:** Reçete duyurusu.

## 1.4 Event Versioning Strategy

### 1.4.1 Event Schema Evolution
Event'ler, zamanla değişebilir ve bu değişiklikler geriye dönük uyumluluğu bozabilir. Versioning stratejisi olarak, her event'in header kısmında version bilgisi tutulacaktır. Upward compatibility için yeni alanlar optional olarak eklenecek, eski alanlar kaldırılmayacaktır. Breaking changes durumunda yeni version numarası ile yeni event tipi oluşturulacaktır.

### 1.4.2 Backward Compatibility Kuralları
Yeni event version'ı oluştururken, mevcut consumer'ların çalışmaya devam etmesi sağlanmalıdır. Optional alanlar eklemek, mevcut alanları kaldırmamak ve enum değerlerini değiştirmemek temel kurallardır. Consumer'lar, tanımadıkları alanları görmezden gelmelidir.

## 1.5 Event Delivery Guarantees

### 1.5.1 At-Least-One Delivery
Network hataları veya consumer downtime durumlarında, event'lerin en az bir kez iletilmesi garantilenmelidir. Bu amaçla, retry mekanizması ve dead letter queue kullanılacaktır. Idempotent handler'lar yazarak, aynı event'in birden fazla işlenmesi durumunda tutarsızlıklar önlenecektir.

### 1.5.2 Outbox Pattern
Outbox pattern, event'lerin atomic transaction içinde hem application state hem de event table'a yazılmasını sağlar. Bu yaklaşım, event'lerin güvenilir bir şekilde publish edilmesini garantiler. Background job, outbox table'dan event'leri okuyarak message broker'a publish eder ve başarılı publish sonrası işlenmiş olarak işaretler.

---

# 2. RİSK ANALİZİ

## 2.1 Teknoloji Riskleri

### 2.1.1 .NET 10 Runtime Risk
**Risk:** .NET 10'un henüz yayınlanmamış olması veya beklenen özelliklerin değişmesi.  
**Etki:** Yüksek - Mimari kararlar değişebilir.  
**Olasılık:** Orta - Microsoft'un LTS politikaları güvenilirdir.  
**Azaltma:** .NET 9 (mevcut LTS) kullanılabilir veya early adopter stratejisi izlenebilir. Beta sürecinde POC yapılmalıdır.

### 2.1.2 Angular 21 Risk
**Risk:** Angular 21'in mevcut olmaması veya breaking changes içermesi.  
**Etki:** Orta - Frontend geliştirme etkilenebilir.  
**Olasılık:** Düşük - Angular'ın backward compatibility performansı iyidir.  
**Azaltma:** Angular'ın release takvimi takip edilmeli, migration planı hazır olmalıdır.

### 2.1.3 SQL Server Licensing Risk
**Risk:** SQL Server lisans maliyetlerinin bütçeyi aşması.  
**Etki:** Orta - SaaS maliyetleri artabilir.  
**Olasılık:** Orta - Enterprise lisans gerekebilir.  
**Azaltma:** Azure SQL veya PostgreSQL'e geçiş alternatifi değerlendirilebilir.

## 2.2 Mimari Riskler

### 2.2.1 Modular Monolith ile Başlama Riski
**Risk:** Modular monolith'in microservices'e dönüşümünde zorluklar yaşanması.  
**Etki:** Yüksek - Yanlış extraction kararları maliyetli olabilir.  
**Olasılık:** Orta - Deneyimli ekip ile düşük.  
**Azaltma:** Extraction kriterleri net tanımlanmalı, metrikler düzenli toplanmalıdır. İlk extraction için düşük riskli modül (örn: Notification) seçilmelidir.

### 2.2.2 Tenant İzolasyonu Riski
**Risk:** Multi-tenant izolasyonunda veri sızıntısı olması.  
**Etki:** Çok Yüksek - Yasal ve itibar riski.  
**Olasılık:** Düşük - Dikkatli implementasyon ile önlenebilir.  
**Azaltma:** Row-level security, schema isolation ve düzenli security audit yapılmalıdır.

### 2.2.3 Feature Flag Sistemi Riski
**Risk:** Feature flag'in yanlış yapılandırılması veya güvenlik açıkları.  
**Etki:** Yüksek - Yetkisiz erişim veya özellik açığı.  
**Olasılık:** Orta.  
**Azaltma:** Feature flag değerleri güvenli storage'da tutulmalı, audit log yapılmalıdır.

## 2.3 Operasyonel Riskler

### 2.3.1 Data Migration Risk
**Risk:** Mevcut sistemden veri migrasyonunda veri kaybı veya tutarsızlık.  
**Etki:** Yüksek - Hasta güvenliği etkilenebilir.  
**Olasılık:** Orta - Deneyimli ekip ile düşük.  
**Azaltma:** Parallel run, reconciliation süreçleri ve rollback planları hazır olmalıdır.

### 2.3.2 Performance Risk
**Risk:** Sistemin yoğun yük altında yavaşlaması veya çökmesi.  
**Etki:** Yüksek - Hasta bakımı etkilenebilir.  
**Olasılık:** Orta - Load test ve capacity planning ile düşürülebilir.  
**Azaltma:** Performance test erken başlamalı, baseline metrikler belirlenmeli ve alerting kurulmalıdır.

### 2.3.3 Downtime Risk
**Risk:** Sistem kesintisi veyaplanned maintenance.  
**Etki:** Yüksek - Hasta işlemleri durabilir.  
**Olasılık:** Orta.  
**Azaltma:** High availability architecture, blue-green deployment ve graceful degradation stratejileri uygulanmalıdır.

## 2.4 Yasal ve Uyum Riskleri

### 2.4.1 KVKK Uyum Riski
**Risk:** Kişisel verilerin korunması kanununa uyumsuzluk.  
**Etki:** Çok Yüksek - Yasal yaptırımlar.  
**Olasılık:** Düşük - Uyum danışmanlığı alınmalı.  
**Azaltma:** KVKK compliance checklist uygulanmalı, DPO atanmalıdır.

### 2.4.2 Healthcare Data Risk
**Risk:** Tıbbi veri güvenliği ihlalleri.  
**Etki:** Çok Yüksek - Hasta güvenliği ve yasal yaptırımlar.  
**Olasılık:** Düşük - En güçlü güvenlik önlemleri alınmalı.  
**Azaltma:** HIPAA benzeri standartlar uygulanmalı, encryption ve access control katmanları oluşturulmalıdır.

## 2.5 Ekip ve Proje Yönetimi Riskleri

### 2.5.1 Ekip Deneyimi Riski
**Risk:** Ekipte .NET ve Angular deneyiminin yetersiz olması.  
**Etki:** Orta - Geliştirme hızı düşebilir.  
**Olasılık:** Orta.  
**Azaltma:** Eğitim programları, pair programming ve code review süreçleri uygulanmalıdır.

### 2.5.2 Scope Creep Riski
**Risk:** Sprint süresince kapsamın genişlemesi.  
**Etki:** Orta - Gecikmeler ve teknik borç.  
**Olasılık:** Yüksek - Healthcare sistemlerde yaygın.  
**Azaltma:** Strict Definition of Done, grooming toplantıları ve backlog prioritization uygulanmalıdır.

---

# 3. PERFORMANS METRİKLERİ

## 3.1 Sistem Performans Metrikleri

### 3.1.1 Response Time Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm Yöntemi |
|--------|-------|-------------|---------------|
| API Response Time P50 | < 50ms | > 100ms | Application Insights |
| API Response Time P95 | < 150ms | > 300ms | Application Insights |
| API Response Time P99 | < 300ms | > 500ms | Application Insights |
| Page Load Time | < 2s | > 5s | Real User Monitoring |
| Time to First Byte | < 200ms | > 500ms | Load Balancer |
| First Contentful Paint | < 1s | > 3s | Frontend monitoring |

### 3.1.2 Throughput Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm Yöntemi |
|--------|-------|-------------|---------------|
| Requests per Second | 1000 req/s | < 500 req/s | API Gateway |
| Concurrent Users | 500 users | > 800 users | Load Balancer |
| Database Connections | < 80% pool | > 90% pool | SQL Monitor |
| Message Throughput | 500 msg/s | < 200 msg/s | Service Bus |

### 3.1.3 Availability Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm Yöntemi |
|--------|-------|-------------|---------------|
| Uptime | 99.9% | < 99.5% | Health Checks |
| Mean Time Between Failures | > 30 days | < 7 days | Monitoring |
| Mean Time to Recovery | < 30 min | > 60 min | Incident Management |
| Error Rate | < 0.1% | > 1% | Application Insights |

## 3.2 Uygulama Performans Metrikleri

### 3.2.1 Database Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm Yöntemi |
|--------|-------|-------------|---------------|
| Query Execution Time | < 100ms | > 500ms | SQL Profiler |
| Deadlock Count | 0 | > 5/day | SQL Monitor |
| Index Usage | > 95% queries | < 80% queries | Query Store |
| Cache Hit Ratio | > 90% | < 70% | Redis Monitor |

### 3.2.2 Memory ve CPU Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm Yöntemi |
|--------|-------|-------------|---------------|
| Memory Usage | < 70% | > 85% | OS Monitor |
| CPU Usage Average | < 60% | > 80% | OS Monitor |
| CPU Usage Peak | < 80% | > 95% | OS Monitor |
| Garbage Collection Time | < 10% | > 20% | .NET Profiler |

## 3.3 İş Metrikleri

### 3.3.1 Hasta Akış Metrikleri

| Metrik | Tanım | Hedef | Ölçüm Yöntemi |
|--------|-------|-------|---------------|
| Patient Registration Time | Hasta kayıt süresi | < 3 min | Process tracking |
| Appointment Booking Time | Randevu oluşturma süresi | < 30 sec | Process tracking |
| Lab Result Turnaround | Sonuç süresi | < Target time | Lab system |
| Discharge Process Time | Taburculuk süresi | < 30 min | Process tracking |

### 3.3.2 Operasyonel Verimlilik

| Metrik | Tanım | Hedef | Ölçüm Yöntemi |
|--------|-------|-------|---------------|
| Bed Occupancy Rate | Yatak doluluk oranı | %80-90 | Bed management |
| OR Utilization Rate | Ameliyathane kullanım oranı | > 80% | OR scheduling |
| Staff Productivity | Personel verimliliği | Trend increasing | HR system |
| Inventory Turnover | Stok devir hızı | > 12/year | Inventory system |

## 3.4 Extraction Kriter Metrikleri

### 3.4.1 Modül Çıkarma Ölçütleri

| Kriter | Eşik | Öncelik | Açıklama |
|--------|------|---------|----------|
| CPU Usage | > 30% sustained | Yüksek | Modülün ortalama CPU kullanımı |
| Memory Usage | > 2GB | Yüksek | Modülün ortalama memory kullanımı |
| Request Throughput | > 1000 req/s | Orta | Modüle gelen istek sayısı |
| Independent Deployment | Business gereklilik | Yüksek | Modülün bağımsız deploy ihtiyacı |
| Team Ownership | Farklı ekip | Orta | Modülün farklı ekip tarafından yönetilmesi |
| Scaling Requirement | Farklı ölçekleme | Orta | Modülün farklı ölçekleme ihtiyacı |

### 3.4.2 Extraction Readiness Score

Her modül için aşağıdaki faktörlere göre puanlama yapılacaktır:

- Performance Impact Score (0-10)
- Team Independence Score (0-10)
- Business Criticality Score (0-10)
- Technical Debt Score (0-10)
- Integration Complexity Score (0-10)

Toplam puan 35'in üzerinde olan modüller, microservices extraction için aday olarak değerlendirilecektir.

---

# 4. TEKNİK BORÇ KONTROLÜ

## 4.1 Teknik Borç Kategorileri

### 4.1.1 Code-Level Borç

#### Code Duplication
**Tanım:** Aynı veya benzer kodun birden fazla yerde tekrarlanması.  
**Ölçüm:** Copy-paste detector ile tespit.  
**Eşik:** %3'ün üzerinde.  
**Azaltma:** Code review sırasında kontrol, shared library kullanımı.

#### Cyclomatic Complexity
**Ne:** Metod içindeki karar noktası sayısı.  
**Ölçüm:** Static analysis tools.  
**Eşik:** Metod başına 15, class başına 100.  
**Azaltma:** Refactoring, method decomposition.

#### Comment Coverage
**Tanım:** Yorum satırı oranı.  
**Ölçüm:** Code analysis tools.  
**Eşik:** Public API'ler için %80.  
**Azaltma:** Inline documentation zorunluluğu.

### 4.1.2 Architecture-Level Borç

#### Circular Dependencies
**Tanım:** Modüller arası döngüsel bağımlılıklar.  
**Eşik:** Sıfır döngüsel bağımlılık.  
**Azaltma:** Dependency analysis, interface extraction.

#### God Classes/Modules
**Tanım:** Aşırı büyük ve çok sorumluluğa sahip sınıflar.  
**Eşik:** 500 satırın üzerinde sınıflar.  
**Azaltma:** Single Responsibility Principle uygulaması.

### 4.1.3 Test-Level Borç

#### Test Coverage
**Tanım:** Unit test coverage oranı.  
**Eşik:** %80 minimum, kritik modüller için %90.  
**Azaltma:** TDD yaklaşımı, test otomasyonu.

#### Flaky Tests
**Tanım:** Non-deterministic testler.  
**Eşik:** Sıfır flaky test.  
**Azaltma:** Test isolation, proper mocking.

## 4.2 Teknik Borç Takip Mekanizması

### 4.2.1 Definition of Done Kriterleri

Her sprint için aşağıdaki kriterlerin sağlanması zorunludur:

- [ ] Tüm yeni kod için unit test yazılmış
- [ ] Code coverage %80'in üzerinde
- [ ] Static analysis uyarıları çözülmüş
- [ ] Code review tamamlanmış
- [ ] Documentation güncellenmiş
- [ ] Performance regression testi geçmiş

### 4.2.2 Borç İzleme Süreci

1. **Tespit:** Her sprint içinde teknik borç tespit edilir ve "Technical Debt Backlog"a eklenir.
2. **Önceliklendirme:** Business value, risk ve effort bazlı önceliklendirme yapılır.
3. **Tahsis:** Sprint planlamasında %20 kapasite teknik borç için ayrılır.
4. **Takip:** Her sprint sonunda borç azaltma metrikleri ölçülür.

### 4.2.3 Teknik Borç Metrikleri

| Metrik | Hedef | Ölçüm |
|--------|-------|-------|
| Code Coverage | > 80% | Coverage tools |
| Critical Bugs | < 5 open | Bug tracker |
| Security Vulnerabilities | 0 critical | Security scan |
| Technical Debt Ratio | < 10% | Static analysis |
| Legacy Code Percentage | < 20% | Code analysis |

## 4.3 Borç Azaltma Stratejileri

### 4.3.1 Kısa Vadeli (Sprint İçi)
- Quick fixes ve small refactoring'ler
- Test coverage iyileştirmesi
- Code review sırasında tespit edilen küçük borçların giderilmesi

### 4.3.2 Orta Vadeli (1-3 Sprint)
- Large-scale refactoring
- Architectural improvements
- Legacy sistem migration başlangıcı

### 4.3.3 Uzun Vadeli (Proje Sonu)
- Complete rewrite (gerekli ise)
- Architecture review ve redesign
- Technical roadmap oluşturma

## 4.4 Sprint Bazlı Borç Hedefleri

### FAZ 0 Hedefleri
- Unit test coverage: %70
- Code coverage: %75
- Critical vulnerabilities: 0

### FAZ 1 Hedefleri
- Unit test coverage: %75
- Code coverage: %80
- Security vulnerabilities: 0

### FAZ 2 Hedefleri
- Unit test coverage: %80
- Code coverage: %80
- Technical debt ratio: < 10%

### FAZ 3-6 Hedefleri
- Unit test coverage: %85
- Code coverage: %85
- Technical debt ratio: < 5%

---

# 5. EKLER

## 5.1 Event Template Örneği

```json
{
  "eventId": "guid",
  "eventType": "PatientCreatedEvent",
  "eventVersion": "1.0",
  "tenantId": "guid",
  "correlationId": "guid",
  "causationId": "guid",
  "timestamp": "2026-02-18T12:00:00Z",
  "data": {
    "patientId": "guid",
    "patientNumber": "string",
    "firstName": "string",
    "lastName": "string",
    "registrationDate": "datetime"
  },
  "metadata": {
    "source": "PatientModule",
    "userId": "guid",
    "ipAddress": "string"
  }
}
```

## 5.2 Risk Register Template

| Risk ID | Risk Tanımı | Kategori | Etki | Olasılık | Azaltma Stratejisi | Sorumlu | Durum |
|---------|-------------|----------|------|----------|-------------------|---------|-------|
| R001 | .NET 10 gecikmesi | Teknoloji | Yüksek | Orta | .NET 9 fallback | Architect | Active |
| R002 | KVKK uyumsuzluğu | Yasal | Çok Yüksek | Düşük | Compliance audit | Compliance | Active |

## 5.3 Performans Baseline Değerleri

| Metrik | Baseline | Target | Budget |
|--------|----------|--------|--------|
| API Response P95 | 200ms | 150ms | 50ms |
| Uptime | 99.5% | 99.9% | 0.4% |
| Error Rate | 0.5% | 0.1% | 0.4% |
| Coverage | 60% | 80% | 20% |
