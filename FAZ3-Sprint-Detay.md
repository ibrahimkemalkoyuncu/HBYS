# FAZ 3: Tıbbi Görüntüleme ve Laboratuvar Modülleri
## Sprint 18-20 Detaylı Domain Task Listesi

---

## SPRINT 18: Laboratory Module - LIS Integration

### Sprint Hedefi
Bu sprint, laboratuvar modülünü ve LIS (Laboratory Information System) entegrasyonunu kapsamaktadır. Laboratuvar test yönetimi, numune kabul, sonuç girişi ve dış laboratuvar entegrasyonu işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. LabTest Entity
**Ne:** Laboratuvar testi entity'si, sistemdeki tüm laboratuvar testlerini temsil eder.  
**Neden:** Laboratuvar ana veri yönetimi için zorunludur.  
**Özelliği:** TestCode, TestName, TestCategory (Biochemistry, Hematology, Microbiology, Pathology, Immunology), SampleType, ContainerType, Method, ContainerColor, TurnaroundTime, Unit, ReferenceRangeMin, ReferenceRangeMax, Price, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar, poliklinik, yatan hasta, acil servis.  
**Amaç:** Laboratuvar test ana veri yönetimi.

#### 2. LabOrder Entity
**Ne:** Laboratuvar siparişi entity'si, hekimlerin laboratuvar taleplerini temsil eder.  
**Neden:** Laboratuvar sürecinin başlangıcını oluşturur.  
**Özelliği:** OrderNumber, PatientId, PhysicianId, EncounterId, EncounterType, Priority (Routine, Urgent, Stat), OrderDate, OrderTime, CollectionDate, CollectionTime, CollectedBy, Status (Ordered, Collected, Received, InProgress, Verified, Reported, Cancelled), Diagnosis, ClinicalNotes, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis, laboratuvar.  
**Amaç:** Laboratuvar sipariş yönetimi.

#### 3. LabOrderItem Entity
**Ne:** Laboratuvar sipariş kalemi entity'si, her siparişteki testleri temsil eder.  
**Neden:** Sipariş-test ilişkisi için gereklidir.  
**Özelliği:** OrderId, TestId, Status (Ordered, Collected, InProgress, Resulted, Cancelled), SampleId, ResultValue, ResultUnit, ResultStatus, ResultDate, ResultVerifiedBy, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Sipariş kalem yönetimi.

#### 4. Sample Entity
**Ne:** Numune entity'si, laboratuvar numunelerini temsil eder.  
**Neden:** Numune takibi için gereklidir.  
**Özelliği:** SampleNumber, OrderItemId, PatientId, SampleType, CollectionDate, CollectionTime, ReceivedDate, ReceivedTime, ReceivedBy, StorageLocation, Status (Collected, Received, Rejected, Processed), RejectionReason, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar, numune alma noktaları.  
**Amaç:** Numune yönetimi.

#### 5. LabResult Entity
**Ne:** Laboratuvar sonucu entity'si, test sonuçlarını temsil eder.  
**Neden:** Sonuç kaydı ve takibi için gereklidir.  
**Özelliği:** ResultId, OrderItemId, TestId, Value, Unit, ResultStatus (Normal, Low, High, Critical), IsAbnormal, Flag, ReferenceRange, Method, EquipmentId, ResultDate, ResultTime, ResultedBy, VerifiedBy, VerifiedDate, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar teknisyenleri, hekimler.  
**Amaç:** Laboratuvar sonuç yönetimi.

#### 6. ExternalLab Entity
**Ne:** Dış laboratuvar entity'si, anlaşmalı dış laboratuvarları temsil eder.  
**Neden:** Dış laboratuvar yönetimi için gereklidir.  
**Özelliği:** LabCode, LabName, ContactPerson, Email, Phone, Address, ContractStartDate, ContractEndDate, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Dış laboratuvar yönetimi.

### Domain Events

#### LabOrderCreatedEvent
**Ne:** Laboratuvar siparişi oluşturulduğunda tetiklenen event'tir.  
**Neden:** Sipariş duyurusu için gereklidir.  
**Özelliği:** OrderId, patientId, priority, tests içerir.  
**Kim Kullanacak:** Notification modülü, Lab modülü.  
**Amaç:** Sipariş duyurusu.

#### SampleCollectedEvent
**Ne:** Numune alındığında tetiklenen event'tir.  
**Neden:** Numune alma duyurusu için gereklidir.  
**Özelliği:** SampleId, orderItemId, collectedAt, collectedBy içerir.  
**Kim Kullanacak:** Lab modülü.  
**Amaç:** Numune alma duyurusu.

#### SampleReceivedEvent
**Ne:** Numune laboratuvara ulaştığında tetiklenen event'tir.  
**Neden:** Numune kabul duyurusu için gereklidir.  
**Özelliği:** SampleId, receivedAt, receivedBy içerir.  
**Kim Kullanacak:** Lab modülü.  
**Amaç:** Numune kabul duyurusu.

#### LabResultReadyEvent
**Ne:** Laboratuvar sonucu hazır olduğunda tetiklenen event'tir.  
**Neden:** Sonuç duyurusu için gereklidir.  
**Özelliği:** ResultId, orderId, patientId, isAbnormal içerir.  
**Kim Kullanacak:** Notification modülü, Ordering modülü.  
**Amaç:** Sonuç duyurusu.

#### CriticalLabResultEvent
**Ne:** Kritik laboratuvar sonucu tespit edildiğinde tetiklenen event'tir.  
**Neden:** Kritik değerler için acil bildirim gereklidir.  
**Özelliği:** ResultId, patientId, testName, criticalValue, detectedAt içerir.  
**Kim Kullanacak:** Notification modülü, CodeBlueTeam.  
**Amaç:** Kritik sonuç uyarısı.

### Commands

#### CreateLabOrderCommand
**Ne:** Laboratuvar siparişi oluşturma command'i.  
**Neden:** Laboratuvar test talebi için gereklidir.  
**Özelliği:** PatientId, physicianId, encounterId, encounterType, priority, testIds, diagnosis parametreleri alır.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis hekimleri.  
**Amaç:** Laboratuvar siparişi oluşturma.

#### CollectSampleCommand
**Ne:** Numune alma command'i.  
**Neden:** Numune kaydı için gereklidir.  
**Özelliği:** OrderItemId, collectedBy, collectionTime parametreleri alır.  
**Kim Kullanacak:** Numune alma personeli.  
**Amaç:** Numune alma.

#### ReceiveSampleCommand
**Ne:** Numune kabul command'i.  
**Neden:** Numune kabul kaydı için gereklidir.  
**Özelliği:** SampleId, receivedBy, storageLocation parametreleri alır.  
**Kim Kullanacak:** Laboratuvar personeli.  
**Amaç:** Numune kabul.

#### RejectSampleCommand
**Ne:** Numune reddetme command'i.  
**Neden:** Uygun olmayan numuneyi reddetmek için gereklidir.  
**Özelliği:** SampleId, rejectionReason, rejectedBy parametreleri alır.  
**Kim Kullanacak:** Laboratuvar personeli.  
**Amaç:** Numune reddetme.

#### RecordLabResultCommand
**Ne:** Laboratuvar sonucu kaydetme command'i.  
**Neden:** Sonuç kaydı için gereklidir.  
**Özelliği:** OrderItemId, value, unit, resultStatus, method, resultedBy parametreleri alır.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Sonuç kaydetme.

#### VerifyLabResultCommand
**Ne:** Laboratuvar sonucu doğrulama command'i.  
**Neden:** Sonuç doğrulama için gereklidir.  
**Özelliği:** ResultId, verifiedBy parametreleri alır.  
**Kim Kullanacak:** Laboratuvar uzmanları.  
**Amaç:** Sonuç doğrulama.

#### CancelLabOrderCommand
**Ne:** Laboratuvar siparişi iptal command'i.  
**Neden:** Sipariş iptali için gereklidir.  
**Özelliği:** OrderId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** İlgili hekimler.  
**Amaç:** Sipariş iptal.

### Queries

#### GetLabOrderByIdQuery
**Ne:** Laboratuvar siparişi detay getirme query'si.  
**Neden:** Sipariş detay sorgulama için gereklidir.  
**Özelliği:** OrderId alır, LabOrderDetailDTO döner.  
**Kim Kullanacak:** Laboratuvar, hekimler.  
**Amaç:** Sipariş sorgulama.

#### GetPatientLabOrdersQuery
**Ne:** Hasta laboratuvar siparişlerini getirme query'si.  
**Neden:** Hasta laboratuvar geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<LabOrderSummaryDTO> döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Hasta laboratuvar geçmişi.

#### GetPendingLabOrdersQuery
**Ne:** Bekleyen laboratuvar siparişlerini getirme query'si.  
**Neden:** İş listesi için gereklidir.  
**Özelliği:** Priority, status alır, List<LabOrderDTO> döner.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Bekleyen siparişler.

#### GetLabResultQuery
**Ne:** Laboratuvar sonucu getirme query'si.  
**Neden:** Sonuç sorgulama için gereklidir.  
**Özelliği:** ResultId alır, LabResultDTO döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Sonuç sorgulama.

#### GetAbnormalResultsQuery
**Ne:** Anormal sonuçları getirme query'si.  
**Neden:** Kritik değerler için gereklidir.  
**Özelliği:** DateRange alır, List<AbnormalResultDTO> döner.  
**Kim Kullanacak:** Laboratuvar uzmanları.  
**Amaç:** Anormal sonuçlar.

#### SearchLabTestsQuery
**Ne:** Laboratuvar testi arama query'si.  
**Neden:** Test arama için gereklidir.  
**Özelliği:** SearchTerm alır, List<LabTestDTO> döner.  
**Kim Kullanacak:** Hekimler, laboratuvar.  
**Amaç:** Test arama.

### Application Services

#### ILabOrderService
**Ne:** Laboratuvar sipariş servisi.  
**Neden:** Sipariş operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateOrder, CollectSample, ReceiveSample, CancelOrder metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Laboratuvar sipariş yönetimi.

#### ILabResultService
**Ne:** Laboratuvar sonuç servisi.  
**Neden:** Sonuç operasyonlarının kapsüllenmesi.  
**Özelliği:** RecordResult, VerifyResult, GetResults metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Laboratuvar sonuç yönetimi.

#### ILabTestService
**Ne:** Laboratuvar test servisi.  
**Neden:** Test operasyonlarının kapsüllenmesi.  
**Özelliği:** SearchTests, GetTestDetails metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Laboratuvar test yönetimi.

### API Endpoints

#### POST /api/v1/lab/orders
**Ne:** Laboratuvar siparişi oluşturma endpoint'i.  
**Neden:** Laboratuvar test talebi.  
**Özelliği:** CreateLabOrderCommand alır, LabOrderDTO döner.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis.  
**Amaç:** Sipariş oluşturma.

#### GET /api/v1/lab/orders/{id}
**Ne:** Sipariş detay endpoint'i.  
**Neden:** Sipariş sorgulama.  
**Özelliği:** OrderId alır, LabOrderDetailDTO döner.  
**Kim Kullanacak:** Laboratuvar, hekimler.  
**Amaç:** Sipariş sorgulama.

#### PUT /api/v1/lab/orders/{id}/collect
**Ne:** Numune alma endpoint'i.  
**Neden:** Numune alma.  
**Özelliği:** CollectSampleCommand alır, SampleDTO döner.  
**Kim Kullanacak:** Numune alma personeli.  
**Amaç:** Numune alma.

#### PUT /api/v1/lab/samples/{id}/receive
**Ne:** Numune kabul endpoint'i.  
**Neden:** Numune kabul.  
**Özelliği:** ReceiveSampleCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Laboratuvar personeli.  
**Amaç:** Numune kabul.

#### PUT /api/v1/lab/samples/{id}/reject
**Ne:** Numune reddetme endpoint'i.  
**Neden:** Numune reddetme.  
**Özelliği:** RejectSampleCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Laboratuvar personeli.  
**Amaç:** Numune reddetme.

#### POST /api/v1/lab/results
**Ne:** Sonuç kaydetme endpoint'i.  
**Neden:** Sonuç kaydı.  
**Özelliği:** RecordLabResultCommand alır, LabResultDTO döner.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Sonuç kaydetme.

#### PUT /api/v1/lab/results/{id}/verify
**Ne:** Sonuç doğrulama endpoint'i.  
**Neden:** Sonuç doğrulama.  
**Özelliği:** VerifyLabResultCommand alır, LabResultDTO döner.  
**Kim Kullanacak:** Laboratuvar uzmanları.  
**Amaç:** Sonuç doğrulama.

#### GET /api/v1/lab/orders/pending
**Ne:** Bekleyen siparişler endpoint'i.  
**Neden:** İş listesi.  
**Özelliği:** Priority alır, List<LabOrderDTO> döner.  
**Kim Kullanacak:** Laboratuvar.  
**Amaç:** Bekleyen siparişler.

#### GET /api/v1/lab/patients/{patientId}/orders
**Ne:** Hasta laboratuvar geçmişi endpoint'i.  
**Neden:** Geçmiş sorgulama.  
**Özelliği:** PatientId alır, List<LabOrderSummaryDTO> döner.  
**Kim Kullanacak:** Hekimler.  
**Amaç:** Hasta geçmişi.

#### GET /api/v1/lab/tests/search
**Ne:** Test arama endpoint'i.  
**Neden:** Test arama.  
**Özelliği:** SearchTerm alır, List<LabTestDTO> döner.  
**Kim Kullanacak:** Hekimler.  
**Amaç:** Test arama.

---

## SPRINT 19: Laboratory Module - External Lab & Reports

### Sprint Hedefi
Bu sprint, laboratuvar modülünün dış laboratuvar entegrasyonu ve raporlama kısmını kapsamaktadır. Dış laboratuvar yönetimi, sonuç raporları ve panel yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. LabPanel Entity
**Ne:** Laboratuvar paneli entity'si, birden fazla testin gruplanmasını temsil eder.  
**Neden:** Panel bazlı sipariş için gereklidir.  
**Özelliği:** PanelCode, PanelName, Category, Description, TestIds (comma separated), Price, DiscountRate, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Hekimler, laboratuvar.  
**Amaç:** Panel yönetimi.

#### 2. ExternalLabOrder Entity
**Ne:** Dış laboratuvar siparişi entity'si, dış laboratuvara gönderilen testleri temsil eder.  
**Neden:** Dış laboratuvar sürecinin takibi için gereklidir.  
**Özelliği:** OrderNumber, LabOrderId, ExternalLabId, SentDate, ExpectedResultDate, ReceivedDate, ResultFile, Status (Sent, Received, Processed, Cancelled), Cost, Notes, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Dış laboratuvar sipariş yönetimi.

#### 3. LabReportTemplate Entity
**Ne:** Laboratuvar rapor şablonu entity'si, sonuç rapor formatlarını temsil eder.  
**Neden:** Rapor oluşturma için gereklidir.  
**Özelliği:** TemplateCode, TemplateName, TemplateContent, Category, IsDefault, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Rapor şablon yönetimi.

#### 4. LabQualityControl Entity
**Ne:** Kalite kontrol entity'si, laboratuvar kalite kontrol sonuçlarını temsil eder.  
**Neden:** Kalite takibi için gereklidir.  
**Özelliği:** ControlDate, TestId, ControlSampleId, ExpectedValue, ActualValue, Deviation, Status (Pass, Fail), PerformedBy, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Kalite kontrol yönetimi.

### Domain Events

#### ExternalLabOrderSentEvent
**Ne:** Dış laboratuvara sipariş gönderildiğinde tetiklenen event'tir.  
**Neden:** Gönderim duyurusu için gereklidir.  
**Özelliği:** ExternalOrderId, externalLabId, sentAt içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Gönderim duyurusu.

#### ExternalLabResultReceivedEvent
**Ne:** Dış laboratuvar sonucu alındığında tetiklenen event'tir.  
**Neden:** Sonuç alma duyurusu için gereklidir.  
**Özelliği:** ExternalOrderId, receivedAt, resultFile içerir.  
**Kim Kullanacak:** Lab modülü.  
**Amaç:** Sonuç alma duyurusu.

### Commands

#### CreateLabPanelCommand
**Ne:** Laboratuvar paneli oluşturma command'i.  
**Neden:** Panel kaydı için gereklidir.  
**Özelliği:** PanelCode, panelName, testIds, price parametreleri alır.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Panel oluşturma.

#### SendToExternalLabCommand
**Ne:** Dış laboratuvara gönderme command'i.  
**Neden:** Dış laboratuvar gönderimi için gereklidir.  
**Özelliği:** LabOrderId, externalLabId, expectedDate parametreleri alır.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Dış laboratuvara gönderme.

#### RecordExternalResultCommand
**Ne:** Dış laboratuvar sonucu kaydetme command'i.  
**Neden:** Sonuç kaydı için gereklidir.  
**Özelliği:** ExternalOrderId, resultFile, resultDate parametreleri alır.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Dış sonuç kaydetme.

#### RecordQualityControlCommand
**Ne:** Kalite kontrol kaydetme command'i.  
**Neden:** Kalite kontrol kaydı için gereklidir.  
**Özelliği:** TestId, controlSampleId, expectedValue, actualValue, performedBy parametreleri alır.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Kalite kontrol kaydetme.

### Queries

#### GetLabPanelsQuery
**Ne:** Laboratuvar panellerini getirme query'si.  
**Neden:** Panel listesi için gereklidir.  
**Özelliği:** Category alır, List<LabPanelDTO> döner.  
**Kim Kullanacak:** Hekimler, laboratuvar.  
**Amaç:** Panel listesi.

#### GetExternalLabOrdersQuery
**Ne:** Dış laboratuvar siparişlerini getirme query'si.  
**Neden:** Dış sipariş takibi için gereklidir.  
**Özelliği:** ExternalLabId, status alır, List<ExternalLabOrderDTO> döner.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Dış siparişler.

#### GetQualityControlHistoryQuery
**Ne:** Kalite kontrol geçmişini getirme query'si.  
**Neden:** Kalite takibi için gereklidir.  
**Özelliği:** TestId, dateRange alır, List<QualityControlDTO> döner.  
**Kim Kullanacak:** Laboratuvar yönetimi.  
**Amaç:** Kalite kontrol geçmişi.

#### GenerateLabReportQuery
**Ne:** Laboratuvar raporu oluşturma query'si.  
**Neden:** Rapor oluşturma için gereklidir.  
**Özelliği:** OrderId, templateId alır, ReportDTO döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Rapor oluşturma.

### Application Services

#### IExternalLabService
**Ne:** Dış laboratuvar servisi.  
**Neden:** Dış laboratuvar operasyonlarının kapsüllenmesi.  
**Özelliği:** SendToExternalLab, RecordExternalResult, GetExternalOrders metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Dış laboratuvar yönetimi.

#### ILabReportService
**Ne:** Laboratuvar rapor servisi.  
**Neden:** Rapor operasyonlarının kapsüllenmesi.  
**Özelliği:** GenerateReport, GetReportTemplate, CreatePanel metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Laboratuvar rapor yönetimi.

### API Endpoints

#### POST /api/v1/lab/panels
**Ne:** Panel oluşturma endpoint'i.  
**Neden:** Panel kaydı.  
**Özelliği:** CreateLabPanelCommand alır, LabPanelDTO döner.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Panel oluşturma.

#### GET /api/v1/lab/panels
**Ne:** Panel listesi endpoint'i.  
**Neden:** Panel listesi.  
**Özelliği:** Category alır, List<LabPanelDTO> döner.  
**Kim Kullanacak:** Hekimler.  
**Amaç:** Panel listesi.

#### POST /api/v1/lab/external/send
**Ne:** Dış laboratuvara gönderme endpoint'i.  
**Neden:** Dış gönderim.  
**Özelliği:** SendToExternalLabCommand alır, ExternalLabOrderDTO döner.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Dış laboratuvara gönderme.

#### POST /api/v1/lab/external/results
**Ne:** Dış sonuç kaydetme endpoint'i.  
**Neden:** Dış sonuç kaydı.  
**Özelliği:** RecordExternalResultCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Laboratuvar yöneticileri.  
**Amaç:** Dış sonuç kaydetme.

#### GET /api/v1/lab/reports/{orderId}
**Ne:** Laboratuvar raporu endpoint'i.  
**Neden:** Rapor oluşturma.  
**Özelliği:** OrderId alır, ReportDTO döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Rapor oluşturma.

#### POST /api/v1/lab/quality-control
**Ne:** Kalite kontrol kaydetme endpoint'i.  
**Neden:** Kalite kontrol kaydı.  
**Özelliği:** RecordQualityControlCommand alır, QualityControlDTO döner.  
**Kim Kullanacak:** Laboratuvar teknisyenleri.  
**Amaç:** Kalite kontrol kaydetme.

---

## SPRINT 20: Radiology Module

### Sprint Hedefi
Bu sprint, radyoloji modülünü ve PACS (Picture Archiving and Communication System) entegrasyonunu kapsamaktadır. Radyoloji sipariş yönetimi, görüntüleme ve raporlama işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. RadiologyTest Entity
**Ne:** Radyoloji testi entity'si, sistemdeki tüm radyoloji testlerini temsil eder.  
**Neden:** Radyoloji ana veri yönetimi için zorunludur.  
**Özelliği:** TestCode, TestName, TestCategory (X-Ray, CT, MRI, Ultrasound, Mammography, NuclearMedicine), BodyPart, Modality, PreparationInstructions, TurnaroundTime, Price, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Radyoloji, poliklinik, yatan hasta, acil servis.  
**Amaç:** Radyoloji test ana veri yönetimi.

#### 2. RadiologyOrder Entity
**Ne:** Radyoloji siparişi entity'si, hekimlerin radyoloji taleplerini temsil eder.  
**Neden:** Radyoloji sürecinin başlangıcını oluşturur.  
**Özelliği:** OrderNumber, PatientId, PhysicianId, EncounterId, EncounterType, Priority (Routine, Urgent, Stat, Emergency), OrderDate, ScheduledDate, ScheduledTime, Status (Ordered, Scheduled, InProgress, Completed, Reported, Cancelled), ClinicalDiagnosis, ClinicalNotes, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis, radyoloji.  
**Amaç:** Radyoloji sipariş yönetimi.

#### 3. RadiologyAppointment Entity
**Ne:** Radyoloji randevusu entity'si, radyoloji randevularını temsil eder.  
**Neden:** Radyoloji randevu takibi için gereklidir.  
**Özelliği:** AppointmentId, OrderId, RadiologyDeviceId, AppointmentDate, StartTime, EndTime, Status (Scheduled, CheckedIn, InProgress, Completed, Cancelled), CheckedInAt, CompletedAt, TechnologistId, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Radyoloji randevu yönetimi.

#### 4. RadiologyDevice Entity
**Ne:** Radyoloji cihazı entity'si, görüntüleme cihazlarını temsil eder.  
**Neden:** Cihaz yönetimi için gereklidir.  
**Özelliği:** DeviceCode, DeviceName, DeviceType, Modality, RoomNumber, Status (Available, InMaintenance, OutOfOrder), IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Radyoloji cihaz yönetimi.

#### 5. RadiologyStudy Entity
**Ne:** Radyoloji çalışması entity'si, yapılan görüntüleme çalışmalarını temsil eder.  
**Neden:** Çalışma takibi için gereklidir.  
**Özelliği:** StudyId, OrderId, PatientId, StudyDate, StudyTime, Modality, BodyPart, StudyStatus (InProgress, Completed, Archived), InstanceCount, StorageLocation, PACSStudyInstanceUID, TechnologistId, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Radyoloji çalışma yönetimi.

#### 6. RadiologyReport Entity
**Ne:** Radyoloji raporu entity'si, radyoloji raporlarını temsil eder.  
**Neden:** Rapor takibi için gereklidir.  
**Özelliği:** ReportId, StudyId, PatientId, RadiologistId, ReportDate, ReportTime, Findings, Impression, Comparison, Recommendation, ReportStatus (Preliminary, Final, Addendum, Amended), VerifiedBy, VerifiedDate, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Radyoloji uzmanları, hekimler.  
**Amaç:** Radyoloji rapor yönetimi.

### Domain Events

#### RadiologyOrderCreatedEvent
**Ne:** Radyoloji siparişi oluşturulduğunda tetiklenen event'tir.  
**Neden:** Sipariş duyurusu için gereklidir.  
**Özelliği:** OrderId, patientId, priority, testName içerir.  
**Kim Kullanacak:** Notification modülü, Radiology modülü.  
**Amaç:** Sipariş duyurusu.

#### RadiologyStudyCompletedEvent
**Ne:** Radyoloji çalışması tamamlandığında tetiklenen event'tir.  
**Neden:** Çalışma tamamlanma duyurusu için gereklidir.  
**Özelliği:** StudyId, orderId, completedAt içerir.  
**Kim Kullanacak:** Radiology modülü.  
**Amaç:** Çalışma tamamlanma duyurusu.

#### RadiologyReportReadyEvent
**Ne:** Radyoloji raporu hazır olduğunda tetiklenen event'tir.  
**Neden:** Rapor duyurusu için gereklidir.  
**Özelliği:** ReportId, patientId, radiologistId içerir.  
**Kim Kullanacak:** Notification modülü, Ordering modülü.  
**Amaç:** Rapor duyurusu.

#### CriticalRadiologyFindingEvent
**Ne:** Kritik radyoloji bulgusu tespit edildiğinde tetiklenen event'tir.  
**Neden:** Kritik bulgu için acil bildirim gereklidir.  
**Özelliği:** ReportId, patientId, criticalFindings, detectedAt içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Kritik bulgu uyarısı.

### Commands

#### CreateRadiologyOrderCommand
**Ne:** Radyoloji siparişi oluşturma command'i.  
**Neden:** Radyoloji test talebi için gereklidir.  
**Özelliği:** PatientId, physicianId, encounterId, encounterType, priority, testId, clinicalDiagnosis parametreleri alır.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis hekimleri.  
**Amaç:** Radyoloji siparişi oluşturma.

#### ScheduleRadiologyCommand
**Ne:** Radyoloji randevusu oluşturma command'i.  
**Neden:** Randevu planlaması için gereklidir.  
**Özelliği:** OrderId, radiologyDeviceId, scheduledDate, scheduledTime parametreleri alır.  
**Kim Kullanacak:** Radyoloji personeli.  
**Amaç:** Randevu oluşturma.

#### StartRadiologyStudyCommand
**Ne:** Radyoloji çalışması başlatma command'i.  
**Neden:** Çalışma başlatma için gereklidir.  
**Özelliği:** AppointmentId, technologistId parametreleri alır.  
**Kim Kullanacak:** Radyoloji teknisyenleri.  
**Amaç:** Çalışma başlatma.

#### CompleteRadiologyStudyCommand
**Ne:** Radyoloji çalışması tamamlama command'i.  
**Neden:** Çalışma tamamlama için gereklidir.  
**Özelliği:** StudyId, instanceCount, storageLocation parametreleri alır.  
**Kim Kullanacak:** Radyoloji teknisyenleri.  
**Amaç:** Çalışma tamamlama.

#### RecordRadiologyReportCommand
**Ne:** Radyoloji raporu kaydetme command'i.  
**Neden:** Rapor kaydı için gereklidir.  
**Özelliği:** StudyId, findings, impression, comparison, recommendation parametreleri alır.  
**Kim Kullanacak:** Radyoloji uzmanları.  
**Amaç:** Rapor kaydetme.

#### VerifyRadiologyReportCommand
**Ne:** Radyoloji raporu doğrulama command'i.  
**Neden:** Rapor doğrulama için gereklidir.  
**Özelliği:** ReportId, verifiedBy parametreleri alır.  
**Kim Kullanacak:** Radyoloji uzmanları.  
**Amaç:** Rapor doğrulama.

#### CancelRadiologyOrderCommand
**Ne:** Radyoloji siparişi iptal command'i.  
**Neden:** Sipariş iptali için gereklidir.  
**Özelliği:** OrderId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** İlgili hekimler.  
**Amaç:** Sipariş iptal.

### Queries

#### GetRadiologyOrderByIdQuery
**Ne:** Radyoloji siparişi detay getirme query'si.  
**Neden:** Sipariş detay sorgulama için gereklidir.  
**Özelliği:** OrderId alır, RadiologyOrderDetailDTO döner.  
**Kim Kullanacak:** Radyoloji, hekimler.  
**Amaç:** Sipariş sorgulama.

#### GetPatientRadiologyOrdersQuery
**Ne:** Hasta radyoloji siparişlerini getirme query'si.  
**Neden:** Hasta radyoloji geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<RadiologyOrderSummaryDTO> döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Hasta radyoloji geçmişi.

#### GetRadiologyAppointmentsQuery
**Ne:** Radyoloji randevularını getirme query'si.  
**Neden:** Randevu takibi için gereklidir.  
**Özelliği:** DeviceId, date alır, List<RadiologyAppointmentDTO> döner.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Randevular.

#### GetRadiologyReportQuery
**Ne:** Radyoloji raporu getirme query'si.  
**Neden:** Rapor sorgulama için gereklidir.  
**Özelliği:** ReportId alır, RadiologyReportDTO döner.  
**Kim Kullanacak:** Hekimler, hasta.  
**Amaç:** Rapor sorgulama.

#### GetStudyImagesQuery
**Ne:** Çalışma görüntülerini getirme query'si.  
**Neden:** Görüntü sorgulama için gereklidir.  
**Özelliği:** StudyId alır, List<ImageDTO> döner.  
**Kim Kullanacak:** Radyoloji uzmanları, hekimler.  
**Amaç:** Görüntü sorgulama.

#### SearchRadiologyTestsQuery
**Ne:** Radyoloji testi arama query'si.  
**Neden:** Test arama için gereklidir.  
**Özelliği:** SearchTerm, category alır, List<RadiologyTestDTO> döner.  
**Kim Kullanacak:** Hekimler, radyoloji.  
**Amaç:** Test arama.

#### GetDeviceScheduleQuery
**Ne:** Cihaz programını getirme query'si.  
**Neden:** Cihaz müsaitlik takibi için gereklidir.  
**Özelliği:** DeviceId, date alır, DeviceScheduleDTO döner.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Cihaz programı.

### Application Services

#### IRadiologyOrderService
**Ne:** Radyoloji sipariş servisi.  
**Neden:** Sipariş operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateOrder, ScheduleOrder, CancelOrder metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Radyoloji sipariş yönetimi.

#### IRadiologyStudyService
**Ne:** Radyoloji çalışma servisi.  
**Neden:** Çalışma operasyonlarının kapsüllenmesi.  
**Özelliği:** StartStudy, CompleteStudy, GetStudy metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Radyoloji çalışma yönetimi.

#### IRadiologyReportService
**Ne:** Radyoloji rapor servisi.  
**Neden:** Rapor operasyonlarının kapsüllenmesi.  
**Özelliği:** RecordReport, VerifyReport, GetReport metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Radyoloji rapor yönetimi.

### API Endpoints

#### POST /api/v1/radiology/orders
**Ne:** Radyoloji siparişi oluşturma endpoint'i.  
**Neden:** Radyoloji test talebi.  
**Özelliği:** CreateRadiologyOrderCommand alır, RadiologyOrderDTO döner.  
**Kim Kullanacak:** Poliklinik, yatan hasta, acil servis.  
**Amaç:** Sipariş oluşturma.

#### GET /api/v1/radiology/orders/{id}
**Ne:** Sipariş detay endpoint'i.  
**Neden:** Sipariş sorgulama.  
**Özelliği:** OrderId alır, RadiologyOrderDetailDTO döner.  
**Kim Kullanacak:** Radyoloji, hekimler.  
**Amaç:** Sipariş sorgulama.

#### POST /api/v1/radiology/orders/{id}/schedule
**Ne:** Radyoloji randevusu oluşturma endpoint'i.  
**Neden:** Randevu planlama.  
**Özelliği:** ScheduleRadiologyCommand alır, RadiologyAppointmentDTO döner.  
**Kim Kullanacak:** Radyoloji personeli.  
**Amaç:** Randevu oluşturma.

#### PUT /api/v1/radiology/studies/{id}/start
**Ne:** Çalışma başlatma endpoint'i.  
**Neden:** Çalışma başlatma.  
**Özelliği:** StudyId alır, SuccessResponse döner.  
**Kim Kullanacak:** Radyoloji teknisyenleri.  
**Amaç:** Çalışma başlatma.

#### PUT /api/v1/radiology/studies/{id}/complete
**Ne:** Çalışma tamamlama endpoint'i.  
**Neden:** Çalışma tamamlama.  
**Özelliği:** CompleteRadiologyStudyCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Radyoloji teknisyenleri.  
**Amaç:** Çalışma tamamlama.

#### POST /api/v1/radiology/reports
**Ne:** Radyoloji raporu kaydetme endpoint'i.  
**Neden:** Rapor kaydı.  
**Özelliği:** RecordRadiologyReportCommand alır, RadiologyReportDTO döner.  
**Kim Kullanacak:** Radyoloji uzmanları.  
**Amaç:** Rapor kaydetme.

#### PUT /api/v1/radiology/reports/{id}/verify
**Ne:** Radyoloji raporu doğrulama endpoint'i.  
**Neden:** Rapor doğrulama.  
**Özelliği:** VerifyRadiologyReportCommand alır, RadiologyReportDTO döner.  
**Kim Kullanacak:** Radyoloji uzmanları.  
**Amaç:** Rapor doğrulama.

#### GET /api/v1/radiology/orders/pending
**Ne:** Bekleyen radyoloji siparişleri endpoint'i.  
**Neden:** İş listesi.  
**Özelliği:** Priority alır, List<RadiologyOrderDTO> döner.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Bekleyen siparişler.

#### GET /api/v1/radiology/devices/{id}/schedule
**Ne:** Cihaz programı endpoint'i.  
**Neden:** Cihaz müsaitlik.  
**Özelliği:** DeviceId, date alır, DeviceScheduleDTO döner.  
**Kim Kullanacak:** Radyoloji.  
**Amaç:** Cihaz programı.

#### GET /api/v1/radiology/tests/search
**Ne:** Radyoloji test arama endpoint'i.  
**Neden:** Test arama.  
**Özelliği:** SearchTerm alır, List<RadiologyTestDTO> döner.  
**Kim Kullanacak:** Hekimler.  
**Amaç:** Test arama.

---

## SPRINT 21: Stabilizasyon

### Sprint Hedefi
FAZ 3'ün stabilizasyon sprint'idir. Tüm modüller entegre edilecek, test edilecek ve production'a hazır hale getirilecektir.

### Yapılacak İşler

#### Entegrasyon Testleri
- Laboratory, Radiology modülleri arası entegrasyon testleri
- LIS/PACS entegrasyon testleri
- Hekim-order-sonuç akış testleri

#### Performans Testleri
- Load test: 200 concurrent user (Lab+Rad)
- Stress test: 1000 concurrent user
- DICOM görüntü transfer performance test

#### Dokümantasyon
- API dokümantasyonu güncellemesi
- LIS/PACS entegrasyon kılavuzu

---

## FAZ 3 ÖZETİ

### Tamamlanacak Modüller

| Modül | Sprint | Öncelik | Bağımlılıklar |
|-------|--------|---------|---------------|
| Laboratory | 18-19 | Critical | Patient, Outpatient, Inpatient |
| Radiology | 20 | Critical | Patient, Outpatient, Inpatient |

### Kritik Başarı Kriterleri

1. Laboratuvar sipariş, numune alma ve sonuç girişi çalışıyor olmalı
2. Dış laboratuvar entegrasyonu çalışıyor olmalı
3. Radyoloji sipariş, randevu ve çalışma yönetimi çalışıyor olmalı
4. PACS entegrasyonu çalışıyor olmalı
5. Kritik sonuç/bulgu uyarı sistemi çalışıyor olmalı
6. Laboratuvar ve radyoloji raporları oluşturulabiliyor olmalı
7. Unit test coverage %80'in üzerinde olmalı
