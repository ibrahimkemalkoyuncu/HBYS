# FAZ 2: Yatan Hasta ve Acil Hizmetler Modülleri
## Sprint 12-16 Detaylı Domain Task Listesi

---

## SPRINT 12: Inpatient Module - Admission & Discharge

### Sprint Hedefi
Bu sprint, yatan hasta modülünün kabul ve taburculuk işlemlerini kapsamaktadır. Hasta yatışı, oda/yatak yönetimi ve taburculuk işlemleri geliştirilecektir.

### Domain Entity'leri

#### 1. InpatientAdmission Entity
**Ne:** Yatan hasta kabul entity'si, hastaneye yatırılan hastaları temsil eder.  
**Neden:** Yatan hasta sürecinin başlangıcını oluşturur ve tüm yatış süreci bu kayıt üzerinden takip edilir.  
**Özelliği:** AdmissionNumber, PatientId, PhysicianId, DepartmentId, RoomId, BedId, AdmissionDate, AdmissionTime, PlannedDischargeDate, ActualDischargeDate, AdmissionType (Planned, Emergency), Diagnosis, TreatmentPlan, Status (Admitted, InTreatment, Discharged, Transferred), AdmittedBy, DischargeType, DischargeSummary, CreatedAt, CreatedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, yatan hasta servisleri, hekimler.  
**Amaç:** Yatan hasta kabul yönetimi.

#### 2. Room Entity
**Ne:** Oda entity'si, hastane odalarını temsil eder.  
**Neden:** Oda kapasitesi ve doluluk takibi için gereklidir.  
**Özelliği:** RoomNumber, DepartmentId, RoomType (Single, Double, Triple, Ward), Floor, Building, Capacity, CurrentOccupancy, Status (Available, Occupied, Maintenance, Cleaning), Features (AC, TV, WC), DailyRate, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, yatan hasta servisleri.  
**Amaç:** Oda yönetimi.

#### 3. Bed Entity
**Ne:** Yatak entity'si, odalardaki yatakları temsil eder.  
**Neden:** Yatak doluluk ve hasta yeri takibi için gereklidir.  
**Özelliği:** BedNumber, RoomId, BedType (Manual, Electric), Status (Available, Occupied, Reserved, Maintenance), IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, yatan hasta servisleri.  
**Amaç:** Yatak yönetimi.

#### 4. InpatientTransfer Entity
**Ne:** Yatan hasta transfer entity'si, servisler arası transferleri temsil eder.  
**Neden:** Hasta transferlerinin takibi için gereklidir.  
**Özelliği:** AdmissionId, FromDepartmentId, ToDepartmentId, FromRoomId, ToRoomId, TransferDate, TransferReason, TransferredBy, ApprovedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Yatan hasta servisleri.  
**Amaç:** Transfer yönetimi.

#### 5. Discharge Entity
**Ne:** Taburculuk entity'si, hasta taburculuk bilgilerini temsil eder.  
**Neden:** Taburculuk sürecinin detaylı takibi için gereklidir.  
**Özelliği:** AdmissionId, DischargeDate, DischargeTime, DischargeType (Recovered, Transfer, Deceased, AgainstAdvice), DischargeSummary, FollowUpInstructions, NextAppointmentDate, MedicationOn Discharge, DischargeBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Yatan hasta servisleri, hekimler.  
**Amaç:** Taburculuk yönetimi.

### Domain Events

#### PatientAdmittedEvent
**Ne:** Hasta yatırıldığında tetiklenen event'tir.  
**Neden:** Yatış duyurusu için gereklidir.  
**Özelliği:** AdmissionId, patientId, roomId, bedId, admittedAt içerir.  
**Kim Kullanacak:** Notification modülü, Billing modülü, Ward modülü.  
**Amaç:** Yatış duyurusu.

#### PatientDischargedEvent
**Ne:** Hasta taburcu edildiğinde tetiklenen event'tir.  
**Neden:** Taburculuk duyurusu için gereklidir.  
**Özelliği:** AdmissionId, patientId, dischargedAt, dischargeType içerir.  
**Kim Kullanacak:** Billing modülü, Notification modülü, Pharmacy modülü.  
**Amaç:** Taburculuk duyurusu.

#### PatientTransferredEvent
**Ne:** Hasta transfer edildiğinde tetiklenen event'tir.  
**Neden:** Transfer duyurusu için gereklidir.  
**Özelliği:** AdmissionId, fromDepartmentId, toDepartmentId, transferredAt içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Transfer duyurusu.

#### RoomStatusChangedEvent
**Ne:** Oda durumu değiştiğinde tetiklenen event'tir.  
**Neden:** Oda durumu değişikliği duyurusu için gereklidir.  
**Özelliği:** RoomId, oldStatus, newStatus, changedAt içerir.  
**Kim Kullanacak:** Inventory modülü.  
**Amaç:** Oda durumu duyurusu.

### Commands

#### AdmitPatientCommand
**Ne:** Hasta yatırma command'i.  
**Neden:** Yatan hasta kaydı için gereklidir.  
**Özelliği:** PatientId, physicianId, departmentId, roomId, bedId, admissionType, diagnosis parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli, acil servis.  
**Amaç:** Hasta yatırma.

#### TransferPatientCommand
**Ne:** Hasta transfer command'i.  
**Neden:** Servisler arası transfer için gereklidir.  
**Özelliği:** AdmissionId, toDepartmentId, toRoomId, toBedId, transferReason parametreleri alır.  
**Kim Kullanacak:** Yatan hasta servisi hekimleri.  
**Amaç:** Hasta transfer.

#### DischargePatientCommand
**Ne:** Hasta taburcu command'i.  
**Neden:** Taburculuk işlemi için gereklidir.  
**Özelliği:** AdmissionId, dischargeType, dischargeSummary, followUpInstructions, nextAppointmentDate parametreleri alır.  
**Kim Kullanacak:** Yatan hasta servisi hekimleri.  
**Amaç:** Hasta taburcu.

#### CancelAdmissionCommand
**Ne:** Yatış iptal command'i.  
**Neden:** Yatış iptal işlemi için gereklidir.  
**Özelliği:** AdmissionId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** Hasta kabul yöneticisi.  
**Amaç:** Yatış iptal.

#### UpdateRoomStatusCommand
**Ne:** Oda durumu güncelleme command'i.  
**Neden:** Oda durumu güncellemesi için gereklidir.  
**Özelliği:** RoomId, newStatus, reason parametreleri alır.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Oda durumu güncelleme.

#### ReserveBedCommand
**Ne:** Yatak rezervasyon command'i.  
**Neden:** Önceden yatak rezervasyonu için gereklidir.  
**Özelliği:** PatientId, departmentId, requestedDate, duration parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Yatak rezervasyon.

### Queries

#### GetAdmissionByIdQuery
**Ne:** Kabul detay getirme query'si.  
**Neden:** Kabul detay sorgulama için gereklidir.  
**Özelliği:** AdmissionId alır, AdmissionDetailDTO döner.  
**Kim Kullanacak:** Yatan hasta servisi, hekimler.  
**Amaç:** Kabul sorgulama.

#### GetPatientAdmissionsQuery
**Ne:** Hasta yatış geçmişini getirme query'si.  
**Neden:** Hasta yatış geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<AdmissionSummaryDTO> döner.  
**Kim Kullanacak:** Yatan hasta servisi.  
**Amaç:** Hasta yatış geçmişi.

#### GetAvailableBedsQuery
**Ne:** Müsait yatakları getirme query'si.  
**Neden:** Yatak sorgulama için gereklidir.  
**Özelliği:** DepartmentId, roomType alır, List<AvailableBedDTO> döner.  
**Kim Kullanacak:** Hasta kabul.  
**Amaç:** Yatak sorgulama.

#### GetDepartmentOccupancyQuery
**Ne:** Departman doluluk oranını getirme query'si.  
**Neden:** Doluluk takibi için gereklidir.  
**Özelliği:** DepartmentId, date alır, OccupancyDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Doluluk takibi.

#### GetActiveInpatientsQuery
**Ne:** Aktif yatan hastaları getirme query'si.  
**Neden:** Aktif hasta takibi için gereklidir.  
**Özelliği:** DepartmentId alır, List<ActiveInpatientDTO> döner.  
**Kim Kullanacak:** Servis hemşireleri, hekimler.  
**Amaç:** Aktif hasta takibi.

#### GetRoomDetailsQuery
**Ne:** Oda detay getirme query'si.  
**Neden:** Oda bilgisi için gereklidir.  
**Özelliği:** RoomId alır, RoomDetailDTO döner.  
**Kim Kullanacak:** Hasta kabul, servis.  
**Amaç:** Oda sorgulama.

### Application Services

#### IAdmissionService
**Ne:** Kabul servisi.  
**Neden:** Kabul operasyonlarının kapsüllenmesi.  
**Özelliği:** AdmitPatient, TransferPatient, DischargePatient, CancelAdmission metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Kabul yönetimi.

#### IRoomBedService
**Ne:** Oda-yatak servisi.  
**Neden:** Oda-yatak operasyonlarının kapsüllenmesi.  
**Özelliği:** GetAvailableBeds, UpdateRoomStatus, ReserveBed metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Oda-yatak yönetimi.

### API Endpoints

#### POST /api/v1/inpatient/admissions
**Ne:** Hasta yatırma endpoint'i.  
**Neden:** Yatan hasta kaydı.  
**Özelliği:** AdmitPatientCommand alır, AdmissionDTO döner.  
**Kim Kullanacak:** Hasta kabul, acil servis.  
**Amaç:** Hasta yatırma.

#### GET /api/v1/inpatient/admissions/{id}
**Ne:** Kabul detay endpoint'i.  
**Neden:** Kabul sorgulama.  
**Özelliği:** AdmissionId alır, AdmissionDetailDTO döner.  
**Kim Kullanacak:** Yatan hasta servisi.  
**Amaç:** Kabul sorgulama.

#### PUT /api/v1/inpatient/admissions/{id}/transfer
**Ne:** Hasta transfer endpoint'i.  
**Neden:** Hasta transfer.  
**Özelliği:** TransferPatientCommand alır, AdmissionDTO döner.  
**Kim Kullanacak:** Servis hekimleri.  
**Amaç:** Hasta transfer.

#### PUT /api/v1/inpatient/admissions/{id}/discharge
**Ne:** Hasta taburcu endpoint'i.  
**Neden:** Taburculuk.  
**Özelliği:** DischargePatientCommand alır, DischargeDTO döner.  
**Kim Kullanacak:** Servis hekimleri.  
**Amaç:** Taburculuk.

#### GET /api/v1/inpatient/beds/available
**Ne:** Müsait yataklar endpoint'i.  
**Neden:** Yatak sorgulama.  
**Özelliği:** DepartmentId alır, List<AvailableBedDTO> döner.  
**Kim Kullanacak:** Hasta kabul.  
**Amaç:** Yatak sorgulama.

#### GET /api/v1/inpatient/departments/{id}/occupancy
**Ne:** Departman doluluk endpoint'i.  
**Neden:** Doluluk takibi.  
**Özelliği:** DepartmentId alır, OccupancyDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Doluluk takibi.

#### GET /api/v1/inpatient/departments/{id}/active-patients
**Ne:** Aktif yatan hastalar endpoint'i.  
**Neden:** Aktif hasta listesi.  
**Özelliği:** DepartmentId alır, List<ActiveInpatientDTO> döner.  
**Kim Kullanacak:** Servis.  
**Amaç:** Aktif hasta listesi.

---

## SPRINT 13: Inpatient Module - Daily Care & Orders

### Sprint Hedefi
Bu sprint, yatan hasta modülünün günlük bakım ve tedavi order yönetimini kapsamaktadır. Günlük vizit, tedavi order'ları, hemşirelik bakımı ve hasta takip işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. DailyRound Entity
**Ne:** Günlük vizit entity'si, hekim günlük vizit kayıtlarını temsil eder.  
**Neden:** Günlük hasta takibi için gereklidir.  
**Özelliği:** AdmissionId, PhysicianId, RoundDate, RoundTime, ChiefComplaint, PhysicalExamFindings, VitalSignsSummary, ProgressNotes, Orders, NextRoundPlan, CreatedAt, CreatedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Günlük vizit yönetimi.

#### 2. TreatmentOrder Entity
**Ne:** Tedavi order entity'si, hekim tedavi orderlarını temsil eder.  
**Neden:** Tedavi sürecinin takibi için zorunludur.  
**Özelliği:** OrderNumber, AdmissionId, PatientId, PhysicianId, OrderType (Medication, Procedure, Diet, Lab, Imaging), ServiceId, StartDate, EndDate, Frequency, Dosage, Instructions, Status (Pending, InProgress, Completed, Cancelled), OrderedAt, OrderedBy, CompletedAt, CompletedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Yatan hasta hekimleri, hemşireler, eczane, laboratuvar.  
**Amaç:** Tedavi order yönetimi.

#### 3. NursingCare Entity
**Ne:** Hemşirelik bakım entity'si, hemşirelik aktivitelerini temsil eder.  
**Neden:** Bakım takibi için gereklidir.  
**Özelliği:** AdmissionId, NurseId, CareDate, CareTime, CareType, Description, Outcome, Duration, Status, CreatedAt, CreatedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Hemşirelik bakım yönetimi.

#### 4. VitalSignRecord Entity
**Ne:** Vital bulgu kaydı entity'si, yatan hastadan alınan vital bulguları temsil eder.  
**Neden:** Sürekli takip için gereklidir.  
**Özelliği:** AdmissionId, RecordedAt, BloodPressureSystolic, BloodPressureDiastolic, Pulse, Temperature, RespiratoryRate, OxygenSaturation, Weight, UrineOutput, RecordedBy, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Vital bulgu takibi.

#### 5. PatientNutrition Entity
**Ne:** Hasta beslenme entity'si, hasta diyet ve beslenme bilgilerini temsil eder.  
**Neden:** Beslenme takibi için gereklidir.  
**Özelliği:** AdmissionId, DietType, MealPlan, StartDate, EndDate, Calories, Allergies, Preferences, Status, OrderedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Diyetisyen, servis hemşireleri.  
**Amaç:** Beslenme yönetimi.

#### 6. Consultation Entity
**Ne:** Konsültasyon entity'si, diğer branş hekimlerinden konsültasyon taleplerini temsil eder.  
**Neden:** Konsültasyon sürecinin takibi için gereklidir.  
**Özelliği:** ConsultationNumber, AdmissionId, RequestedDepartmentId, RequestedPhysicianId, RequestReason, RequestedAt, RequestedBy, RespondedAt, Response, ResponseNotes, Status (Pending, Responded, Accepted, Rejected), CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Konsültasyon yönetimi.

### Domain Events

#### TreatmentOrderCreatedEvent
**Ne:** Tedavi order oluşturulduğunda tetiklenen event'tir.  
**Neden:** Order duyurusu için gereklidir.  
**Özelliği:** OrderId, admissionId, orderType, serviceId içerir.  
**Kim Kullanacak:** Pharmacy modülü, Lab modülü, Nursing modülü.  
**Amaç:** Order duyurusu.

#### TreatmentOrderCompletedEvent
**Ne:** Tedavi order tamamlandığında tetiklenen event'tir.  
**Neden:** Tamamlanma duyurusu için gereklidir.  
**Özelliği:** OrderId, completedAt, completedBy içerir.  
**Kim Kullanacak:** Billing modülü.  
**Amaç:** Order tamamlanma duyurusu.

#### VitalSignsRecordedEvent
**Ne:** Vital bulgu kaydedildiğinde tetiklenen event'tir.  
**Neden:** Kritik değerler için uyarı gereklidir.  
**Özelliği:** AdmissionId, recordedAt, vitalValues içerir.  
**Kim Kullanacak:** Monitoring modülü.  
**Amaç:** Vital bulgu duyurusu.

#### ConsultationRequestedEvent
**Ne:** Konsültasyon istendiğinde tetiklenen event'tir.  
**Neden:** Konsültasyon duyurusu için gereklidir.  
**Özelliği:** ConsultationId, admissionId, requestedDepartment içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Konsültasyon duyurusu.

### Commands

#### CreateTreatmentOrderCommand
**Ne:** Tedavi order oluşturma command'i.  
**Neden:** Tedavi order kaydı için gereklidir.  
**Özelliği:** AdmissionId, orderType, serviceId, dosage, frequency, startDate, endDate parametreleri alır.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Tedavi order oluşturma.

#### UpdateTreatmentOrderCommand
**Ne:** Tedavi order güncelleme command'i.  
**Neden:** Order güncellemesi için gereklidir.  
**Özelliği:** OrderId, newDosage, newFrequency, newEndDate parametreleri alır.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Tedavi order güncelleme.

#### CancelTreatmentOrderCommand
**Ne:** Tedavi order iptal command'i.  
**Neden:** Order iptali için gereklidir.  
**Özelliği:** OrderId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Tedavi order iptal.

#### CompleteTreatmentOrderCommand
**Ne:** Tedavi order tamamlama command'i.  
**Neden:** Order tamamlama için gereklidir.  
**Özelliği:** OrderId, completedBy parametreleri alır.  
**Kim Kullanacak:** Hemşireler, eczane, laboratuvar.  
**Amaç:** Tedavi order tamamlama.

#### RecordVitalSignsCommand
**Ne:** Vital bulgu kaydetme command'i.  
**Neden:** Vital bulgu kaydı için gereklidir.  
**Özelliği:** AdmissionId, bloodPressure, pulse, temperature, respiratoryRate, oxygenSaturation, weight parametreleri alır.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Vital bulgu kaydetme.

#### RecordNursingCareCommand
**Ne:** Hemşirelik bakım kaydetme command'i.  
**Neden:** Bakım kaydı için gereklidir.  
**Özelliği:** AdmissionId, careType, description, outcome, duration parametreleri alır.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Bakım kaydetme.

#### CreateConsultationCommand
**Ne:** Konsültasyon oluşturma command'i.  
**Neden:** Konsültasyon talebi için gereklidir.  
**Özelliği:** AdmissionId, requestedDepartmentId, requestReason parametreleri alır.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Konsültasyon talebi.

#### RespondToConsultationCommand
**Ne:** Konsültasyon cevaplama command'i.  
**Neden:** Konsültasyon cevabı için gereklidir.  
**Özelliği:** ConsultationId, response, responseNotes parametreleri alır.  
**Kim Kullanacak:** Konsült edilen hekim.  
**Amaç:** Konsültasyon cevabı.

### Queries

#### GetActiveTreatmentOrdersQuery
**Ne:** Aktif tedavi orderlarını getirme query'si.  
**Neden:** Aktif order takibi için gereklidir.  
**Özelliği:** AdmissionId alır, List<TreatmentOrderDTO> döner.  
**Kim Kullanacak:** Hemşireler, eczane.  
**Amaç:** Aktif order takibi.

#### GetPatientVitalSignsQuery
**Ne:** Hasta vital bulgularını getirme query'si.  
**Neden:** Vital takip için gereklidir.  
**Özelliği:** AdmissionId, dateRange alır, List<VitalSignDTO> döner.  
**Kim Kullanacak:** Hekimler, hemşireler.  
**Amaç:** Vital bulgu takibi.

#### GetPatientNursingCaresQuery
**Ne:** Hasta bakımlarını getirme query'si.  
**Neden:** Bakım takibi için gereklidir.  
**Özelliği:** AdmissionId alır, List<NursingCareDTO> döner.  
**Kim Kullanacak:** Hemşireler.  
**Amaç:** Bakım takibi.

#### GetConsultationsByAdmissionQuery
**Ne:** Hasta konsültasyonlarını getirme query'si.  
**Neden:** Konsültasyon takibi için gereklidir.  
**Özelliği:** AdmissionId alır, List<ConsultationDTO> döner.  
**Kim Kullanacak:** İlgili hekimler.  
**Amaç:** Konsültasyon takibi.

#### GetPendingConsultationsQuery
**Ne:** Bekleyen konsültasyonları getirme query'si.  
**Neden:** Bekleyen talepler için gereklidir.  
**Özelliği:** DepartmentId alır, List<ConsultationDTO> döner.  
**Kim Kullanacak:** Konsültasyon istenen hekimler.  
**Amaç:** Bekleyen konsültasyonlar.

### Application Services

#### ITreatmentOrderService
**Ne:** Tedavi order servisi.  
**Neden:** Order operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateOrder, UpdateOrder, CancelOrder, CompleteOrder metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Tedavi order yönetimi.

#### INursingService
**Ne:** Hemşirelik servisi.  
**Neden:** Hemşirelik operasyonlarının kapsüllenmesi.  
**Özelliği:** RecordVitalSigns, RecordNursingCare, GetCares metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Hemşirelik yönetimi.

#### IConsultationService
**Ne:** Konsültasyon servisi.  
**Neden:** Konsültasyon operasyonlarının kapsüllenmesi.  
**Özelliği:** RequestConsultation, RespondToConsultation, GetConsultations metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Konsültasyon yönetimi.

### API Endpoints

#### POST /api/v1/inpatient/orders
**Ne:** Tedavi order oluşturma endpoint'i.  
**Neden:** Order kaydı.  
**Özelliği:** CreateTreatmentOrderCommand alır, TreatmentOrderDTO döner.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Order oluşturma.

#### GET /api/v1/inpatient/orders/{id}
**Ne:** Order detay endpoint'i.  
**Neden:** Order sorgulama.  
**Özelliği:** OrderId alır, TreatmentOrderDTO döner.  
**Kim Kullanacak:** İlgili personel.  
**Amaç:** Order sorgulama.

#### PUT /api/v1/inpatient/orders/{id}/cancel
**Ne:** Order iptal endpoint'i.  
**Neden:** Order iptal.  
**Özelliği:** CancelTreatmentOrderCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Order iptal.

#### PUT /api/v1/inpatient/orders/{id}/complete
**Ne:** Order tamamlama endpoint'i.  
**Neden:** Order tamamlama.  
**Özelliği:** CompleteTreatmentOrderCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Hemşireler, eczane.  
**Amaç:** Order tamamlama.

#### POST /api/v1/inpatient/vital-signs
**Ne:** Vital bulgu kaydetme endpoint'i.  
**Neden:** Vital bulgu kaydı.  
**Özelliği:** RecordVitalSignsCommand alır, VitalSignDTO döner.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Vital bulgu kaydetme.

#### GET /api/v1/inpatient/admissions/{id}/vital-signs
**Ne:** Hasta vital bulguları endpoint'i.  
**Neden:** Vital bulgu takibi.  
**Özelliği:** AdmissionId alır, List<VitalSignDTO> döner.  
**Kim Kullanacak:** Hekimler.  
**Amaç:** Vital bulgu takibi.

#### POST /api/v1/inpatient/nursing-cares
**Ne:** Hemşirelik bakım kaydetme endpoint'i.  
**Neden:** Bakım kaydı.  
**Özelliği:** RecordNursingCareCommand alır, NursingCareDTO döner.  
**Kim Kullanacak:** Servis hemşireleri.  
**Amaç:** Bakım kaydetme.

#### POST /api/v1/inpatient/consultations
**Ne:** Konsültasyon talebi endpoint'i.  
**Neden:** Konsültasyon talebi.  
**Özelliği:** CreateConsultationCommand alır, ConsultationDTO döner.  
**Kim Kullanacak:** Yatan hasta hekimleri.  
**Amaç:** Konsültasyon talebi.

#### PUT /api/v1/inpatient/consultations/{id}/respond
**Ne:** Konsültasyon cevaplama endpoint'i.  
**Neden:** Konsültasyon cevabı.  
**Özelliği:** RespondToConsultationCommand alır, ConsultationDTO döner.  
**Kim Kullanacak:** Konsült edilen hekim.  
**Amaç:** Konsültasyon cevabı.

---

## SPRINT 14: Emergency Module

### Sprint Hedefi
Bu sprint, acil servis modülünü kapsamaktadır. Acil hasta kabul, triyaj, acil müdahale ve acil servis yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. EmergencyAdmission Entity
**Ne:** Acil servis kabul entity'si, acil servise başvuran hastaları temsil eder.  
**Neden:** Acil sürecin takibi için zorunludur.  
**Özelliği:** EmergencyNumber, PatientId, AdmissionDate, AdmissionTime, ChiefComplaint, TriageLevel (Red, Orange, Yellow, Green, Blue), TriageNotes, VitalSigns, InitialAssessment, Status (Waiting, InTreatment, Discharged, Admitted, Expired, Transferred), DischargedAt, DischargedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Acil servis personeli, hekimler.  
**Amaç:** Acil servis kabul yönetimi.

#### 2. TriageRecord Entity
**Ne:** Triyaj kaydı entity'si, hasta triyaj değerlendirmelerini temsil eder.  
**Neden:** Acil önceliklendirme için gereklidir.  
**Özelliği:** EmergencyAdmissionId, TriageDate, TriageTime, TriageLevel, VitalSigns, GlasgowComaScale, PainScore, AssessmentNotes, TriageNurseId, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Acil hemşireleri.  
**Amaç:** Triyaj yönetimi.

#### 3. EmergencyIntervention Entity
**Ne:** Acil müdahale entity'si, yapılan acil müdahaleleri temsil eder.  
**Neden:** Müdahale takibi için gereklidir.  
**Özelliği:** EmergencyAdmissionId, InterventionType, InterventionTime, Description, Outcome, PerformedBy, EquipmentUsed, Status, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Acil servis hekimleri, hemşireleri.  
**Amaç:** Acil müdahale yönetimi.

#### 4. EmergencyObservation Entity
**Ne:** Gözlem kaydı entity'si, acil servis gözlem sürecini temsil eder.  
**Neden:** Gözlem takibi için gereklidir.  
**Özelliği:** EmergencyAdmissionId, ObservationStartTime, ObservationEndTime, Observations, VitalSignsRecordings, Decision, DischargeOrAdmissionNotes, ObservedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Acil servis hekimleri.  
**Amaç:** Gözlem yönetimi.

### Domain Events

#### EmergencyAdmissionCreatedEvent
**Ne:** Acil servis kabul oluşturulduğunda tetiklenen event'tir.  
**Neden:** Kabul duyurusu için gereklidir.  
**Özelliği:** EmergencyId, patientId, chiefComplaint, triageLevel içerir.  
**Kim Kullanacak:** Notification modülü, Triage modülü.  
**Amaç:** Acil kabul duyurusu.

#### TriageCompletedEvent
**Ne:** Triyaj tamamlandığında tetiklenen event'tir.  
**Neden:** Triyaj duyurusu için gereklidir.  
**Özelliği:** TriageId, emergencyId, triageLevel içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Triyaj duyurusu.

#### EmergencyDischargedEvent
**Ne:** Acil servis taburcu edildiğinde tetiklenen event'tir.  
**Neden:** Taburculuk duyurusu için gereklidir.  
**Özelliği:** EmergencyId, patientId, dischargeType, dischargedAt içerir.  
**Kim Kullanacak:** Billing modülü, Notification modülü.  
**Amaç:** Taburculuk duyurusu.

#### CriticalVitalSignsEvent
**Ne:** Kritik vital bulgular tespit edildiğinde tetiklenen event'tir.  
**Neden:** Kritik durum uyarısı için gereklidir.  
**Özelliği:** EmergencyId, vitalValues, detectedAt içerir.  
**Kim Kullanacak:** Notification modülü, CodeBlueTeam.  
**Amaç:** Kritik uyarı.

### Commands

#### CreateEmergencyAdmissionCommand
**Ne:** Acil servis kabul oluşturma command'i.  
**Neden:** Acil hasta kaydı için gereklidir.  
**Özelliği:** PatientId, chiefComplaint, admissionType parametreleri alır.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Acil kabul oluşturma.

#### PerformTriageCommand
**Ne:** Triyaj yapma command'i.  
**Neden:** Triyaj kaydı için gereklidir.  
**Özelliği:** EmergencyAdmissionId, triageLevel, vitalSigns, painScore, notes parametreleri alır.  
**Kim Kullanacak:** Acil hemşireleri.  
**Amaç:** Triyaj yapma.

#### RecordEmergencyInterventionCommand
**Ne:** Acil müdahale kaydetme command'i.  
**Neden:** Müdahale kaydı için gereklidir.  
**Özelliği:** EmergencyAdmissionId, interventionType, description, outcome parametreleri alır.  
**Kim Kullanacak:** Acil hekimleri, hemşireleri.  
**Amaç:** Müdahale kaydetme.

#### StartObservationCommand
**Ne:** Gözlem başlatma command'i.  
**Neden:** Gözlem süreci başlatmak için gereklidir.  
**Özelliği:** EmergencyAdmissionId, initialNotes parametreleri alır.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Gözlem başlatma.

#### EndObservationCommand
**Ne:** Gözlem sonlandırma command'i.  
**Neden:** Gözlem sürecini sonlandırmak için gereklidir.  
**Özelliği:** EmergencyAdmissionId, observations, decision parametreleri alır.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Gözlem sonlandırma.

#### DischargeFromEmergencyCommand
**Ne:** Acil servis taburcu command'i.  
**Neden:** Acil hasta taburcu etmek için gereklidir.  
**Özelliği:** EmergencyAdmissionId, dischargeType, notes parametreleri alır.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Acil taburcu.

#### AdmitFromEmergencyCommand
**Ne:** Acil servis yatış command'i.  
**Neden:** Acil hastayı yatırmak için gereklidir.  
**Özelliği:** EmergencyAdmissionId, targetDepartmentId, targetRoomId parametreleri alır.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Acil yatış.

### Queries

#### GetActiveEmergencyCasesQuery
**Ne:** Aktif acil vakaları getirme query'si.  
**Neden:** Acil servis takibi için gereklidir.  
**Özelliği:** Status alır, List<EmergencyCaseDTO> döner.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Aktif vaka takibi.

#### GetEmergencyCaseByIdQuery
**Ne:** Acil vaka detay getirme query'si.  
**Neden:** Vaka detay sorgulama için gereklidir.  
**Özelliği:** EmergencyId alır, EmergencyCaseDetailDTO döner.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Vaka sorgulama.

#### GetTriageQueueQuery
**Ne:** Triyaj sırasını getirme query'si.  
**Neden:** Triyaj önceliklendirme için gereklidir.  
**Özelliği:** TriageLevel alır, List<TriageQueueItemDTO> döner.  
**Kim Kullanacak:** Acil hemşireleri.  
**Amaç:** Triyaj sırası.

#### GetEmergencyStatisticsQuery
**Ne:** Acil servis istatistiklerini getirme query'si.  
**Neden:** İstatistiksel analiz için gereklidir.  
**Özelliği:** DateRange alır, EmergencyStatisticsDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** İstatistik sorgulama.

### Application Services

#### IEmergencyService
**Ne:** Acil servis servisi.  
**Neden:** Acil servis operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateAdmission, PerformTriage, RecordIntervention, DischargeFromEmergency metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Acil servis yönetimi.

#### ITriageService
**Ne:** Triyaj servisi.  
**Neden:** Triyaj operasyonlarının kapsüllenmesi.  
**Özelliği:** PerformTriage, GetTriageQueue, GetTriageHistory metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Triyaj yönetimi.

### API Endpoints

#### POST /api/v1/emergency/admissions
**Ne:** Acil servis kabul endpoint'i.  
**Neden:** Acil hasta kaydı.  
**Özelliği:** CreateEmergencyAdmissionCommand alır, EmergencyAdmissionDTO döner.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Acil kabul oluşturma.

#### GET /api/v1/emergency/admissions/{id}
**Ne:** Acil vaka detay endpoint'i.  
**Neden:** Vaka sorgulama.  
**Özelliği:** EmergencyId alır, EmergencyCaseDetailDTO döner.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Vaka sorgulama.

#### POST /api/v1/emergency/admissions/{id}/triage
**Ne:** Triyaj endpoint'i.  
**Neden:** Triyaj yapma.  
**Özelliği:** PerformTriageCommand alır, TriageRecordDTO döner.  
**Kim Kullanacak:** Acil hemşireleri.  
**Amaç:** Triyaj yapma.

#### POST /api/v1/emergency/admissions/{id}/interventions
**Ne:** Acil müdahale endpoint'i.  
**Neden:** Müdahale kaydı.  
**Özelliği:** RecordEmergencyInterventionCommand alır, InterventionDTO döner.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Müdahale kaydetme.

#### GET /api/v1/emergency/active-cases
**Ne:** Aktif vakalar endpoint'i.  
**Neden:** Aktif vaka takibi.  
**Özelliği:** Status alır, List<EmergencyCaseDTO> döner.  
**Kim Kullanacak:** Acil servis personeli.  
**Amaç:** Aktif vaka takibi.

#### GET /api/v1/emergency/queue/triage
**Ne:** Triyaj sırası endpoint'i.  
**Neden:** Triyaj sırası.  
**Özelliği:** Boş parametre alır, List<TriageQueueItemDTO> döner.  
**Kim Kullanacak:** Acil hemşireleri.  
**Amaç:** Triyaj sırası.

#### PUT /api/v1/emergency/admissions/{id}/discharge
**Ne:** Acil taburcu endpoint'i.  
**Neden:** Taburculuk.  
**Özelliği:** DischargeFromEmergencyCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Acil taburcu.

#### PUT /api/v1/emergency/admissions/{id}/admit
**Ne:** Acil yatış endpoint'i.  
**Neden:** Yatış.  
**Özelliği:** AdmitFromEmergencyCommand alır, AdmissionDTO döner.  
**Kim Kullanacak:** Acil hekimleri.  
**Amaç:** Acil yatış.

---

## SPRINT 15: Pharmacy Module

### Sprint Hedefi
Bu sprint, eczane modülünü kapsamaktadır. İlaç yönetimi, reçete karşılama, ilaç dağıtım ve stok takibi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. Medication Entity
**Ne:** İlaç entity'si, sistemdeki tüm ilaçları temsil eder.  
**Neden:** İlaç bilgi yönetimi için zorunludur.  
**Özelliği:** MedicationCode, Barcode, Name, GenericName, DrugForm (Tablet, Capsule, Syrup, Injection, Cream), Strength, Manufacturer, ActiveIngredients, StorageConditions, IsControlledDrug, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane, poliklinik, yatan hasta, acil servis.  
**Amaç:** İlaç ana veri yönetimi.

#### 2. PharmacyInventory Entity
**Ne:** Eczane stok entity'si, eczanedeki ilaç stoklarını temsil eder.  
**Neden:** Stok takibi için gereklidir.  
**Özelliği:** MedicationId, BatchNumber, ExpiryDate, QuantityInStock, UnitCost, StorageLocation, Status (Available, Reserved, Expired, Quarantine), ReceivedDate, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Eczane stok yönetimi.

#### 3. MedicationDispense Entity
**Ne:** İlaç dağıtım entity'si, hastaya verilen ilaçları temsil eder.  
**Neden:** Dağıtım takibi için gereklidir.  
**Özelliği:** DispenseNumber, PrescriptionId, PatientId, MedicationId, Quantity, DosageInstructions, DispenseDate, DispenseTime, DispensedBy, Status (Pending, Dispensed, Returned, Cancelled), Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İlaç dağıtım yönetimi.

#### 4. MedicationReturn Entity
**Ne:** İlaç iade entity'si, iade edilen ilaçları temsil eder.  
**Neden:** İade takibi için gereklidir.  
**Özelliği:** ReturnNumber, DispenseId, ReturnDate, ReturnQuantity, ReturnReason, ProcessedBy, Status (Pending, Approved, Rejected), CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İlaç iade yönetimi.

#### 5. ControlledDrugRecord Entity
**Ne:** Kontrollü ilaç kaydı entity'si, raporlu ilaç takibini temsil eder.  
**Neden:** Yasal gereklilik için zorunludur.  
**Özelliği:** RecordNumber, MedicationId, PatientId, PrescriptionId, Quantity, DoctorName, Institution, IssueDate, ExpiryDate, Status, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Kontrollü ilaç takibi.

### Domain Events

#### PrescriptionReceivedEvent
**Ne:** Reçete alındığında tetiklenen event'tir.  
**Neden:** Reçete işleme başlangıcı için gereklidir.  
**Özelliği:** PrescriptionId, receivedAt, receivedBy içerir.  
**Kim Kullanacak:** Pharmacy modülü.  
**Amaç:** Reçete alma duyurusu.

#### MedicationDispensedEvent
**Ne:** İlaç verildiğinde tetiklenen event'tir.  
**Neden:** Dağıtım duyurusu için gereklidir.  
**Özelliği:** DispenseId, patientId, medicationId, quantity içerir.  
**Kim Kullanacak:** Inventory modülü, Audit modülü.  
**Amaç:** Dağıtım duyurusu.

#### LowStockAlertEvent
**Ne:** Düşük stok uyarısı tetiklenen event'tir.  
**Neden:** Stok kritik seviyeye düştüğünde uyarı gereklidir.  
**Özelliği:** MedicationId, currentStock, minimumStock, reorderLevel içerir.  
**Kim Kullanacak:** Inventory modülü, Notification modülü.  
**Amaç:** Stok uyarısı.

#### ExpiryWarningEvent
**Ne:** Son kullanma tarihi yaklaşan ilaç uyarısı tetiklenen event'tir.  
**Neden:** Son kullanma tarihi kontrolü için gereklidir.  
**Özelliği:** MedicationId, batchNumber, expiryDate, daysRemaining içerir.  
**Kim Kullanacak:** Inventory modülü.  
**Amaç:** expiry uyarısı.

### Commands

#### ReceivePrescriptionCommand
**Ne:** Reçete alma command'i.  
**Neden:** Reçete işleme başlatmak için gereklidir.  
**Özelliği:** PrescriptionId, receivedBy parametreleri alır.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Reçete alma.

#### DispenseMedicationCommand
**Ne:** İlaç dağıtma command'i.  
**Neden:** İlaç dağıtımı için gereklidir.  
**Özelliği:** PrescriptionId, patientId, medicationId, quantity, instructions parametreleri alır.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İlaç dağıtma.

#### ReturnMedicationCommand
**Ne:** İlaç iade command'i.  
**Neden:** İade işlemi için gereklidir.  
**Özelliği:** DispenseId, returnQuantity, returnReason parametreleri alır.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İlaç iade.

#### UpdateInventoryCommand
**Ne:** Stok güncelleme command'i.  
**Neden:** Stok güncellemesi için gereklidir.  
**Özelliği:** MedicationId, batchNumber, newQuantity, updateReason parametreleri alır.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Stok güncelleme.

#### RecordControlledDrugCommand
**Ne:** Kontrollü ilaç kaydetme command'i.  
**Neden:** Kontrollü ilaç kaydı için gereklidir.  
**Özelliği:** MedicationId, patientId, prescriptionId, quantity, doctorName parametreleri alır.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Kontrollü ilaç kaydetme.

### Queries

#### GetPrescriptionItemsQuery
**Ne:** Reçete kalemlerini getirme query'si.  
**Neden:** Reçete işleme için gereklidir.  
**Özelliği:** PrescriptionId alır, List<PrescriptionItemDTO> döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Reçete kalemleri sorgulama.

#### GetMedicationStockQuery
**Ne:** İlaç stoğu getirme query'si.  
**Neden:** Stok sorgulama için gereklidir.  
**Özelliği:** MedicationId alır, MedicationStockDTO döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Stok sorgulama.

#### GetLowStockMedicationsQuery
**Ne:** Düşük stoklu ilaçları getirme query'si.  
**Neden:** Stok uyarısı için gereklidir.  
**Özelliği:** TenantId alır, List<LowStockMedicationDTO> döner.  
**Kim Kullanacak:** Eczane yöneticisi.  
**Amaç:** Düşük stoklu ilaçlar.

#### GetExpiringMedicationsQuery
**Ne:** Son kullanma tarihi yaklaşan ilaçları getirme query'si.  
**Neden:** expiry takibi için gereklidir.  
**Özelliği:** DaysAhead alır, List<ExpiringMedicationDTO> döner.  
**Kim Kullanacak:** Eczane yöneticisi.  
**Amaç:** Expiring ilaçlar.

#### GetPatientMedicationHistoryQuery
**Ne:** Hasta ilaç geçmişini getirme query'si.  
**Neden:** İlaç geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<MedicationDispenseDTO> döner.  
**Kim Kullanacak:** Hekimler, eczane.  
**Amaç:** İlaç geçmişi sorgulama.

### Application Services

#### IPharmacyService
**Ne:** Eczane servisi.  
**Neden:** Eczane operasyonlarının kapsüllenmesi.  
**Özelliği:** ReceivePrescription, DispenseMedication, ReturnMedication metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Eczane yönetimi.

#### IInventoryService
**Ne:** Stok servisi.  
**Neden:** Stok operasyonlarının kapsüllenmesi.  
**Özelliği:** GetStock, UpdateStock, GetLowStock, GetExpiring metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Stok yönetimi.

### API Endpoints

#### POST /api/v1/pharmacy/prescriptions/{id}/receive
**Ne:** Reçete alma endpoint'i.  
**Neden:** Reçete işleme başlatma.  
**Özelliği:** PrescriptionId alır, SuccessResponse döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Reçete alma.

#### POST /api/v1/pharmacy/dispense
**Ne:** İlaç dağıtım endpoint'i.  
**Neden:** İlaç dağıtımı.  
**Özelliği:** DispenseMedicationCommand alır, DispenseDTO döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İlaç dağıtımı.

#### GET /api/v1/pharmacy/inventory/{medicationId}
**Ne:** İlaç stoğu endpoint'i.  
**Neden:** Stok sorgulama.  
**Özelliği:** MedicationId alır, MedicationStockDTO döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** Stok sorgulama.

#### GET /api/v1/pharmacy/inventory/low-stock
**Ne:** Düşük stoklu ilaçlar endpoint'i.  
**Neden:** Stok uyarısı.  
**Özelliği:** Boş parametre alır, List<LowStockMedicationDTO> döner.  
**Kim Kullanacak:** Eczane yöneticisi.  
**Amaç:** Düşük stoklu ilaçlar.

#### GET /api/v1/pharmacy/inventory/expiring
**Ne:** Expiring ilaçlar endpoint'i.  
**Neden:** expiry takibi.  
**Özelliği:** DaysAhead alır, List<ExpiringMedicationDTO> döner.  
**Kim Kullanacak:** Eczane yöneticisi.  
**Amaç:** Expiring ilaçlar.

#### POST /api/v1/pharmacy/return
**Ne:** İlaç iade endpoint'i.  
**Neden:** İade işlemi.  
**Özelliği:** ReturnMedicationCommand alır, ReturnDTO döner.  
**Kim Kullanacak:** Eczane personeli.  
**Amaç:** İade işlemi.

---

## SPRINT 16: Inventory & Procurement Module

### Sprint Hedefi
Bu sprint, envanter ve tedarik modüllerini kapsamaktadır. Sarf malzeme yönetimi, stok takibi, tedarik siparişi ve tedarikçi yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. InventoryItem Entity
**Ne:** Envanter kalemi entity'si, sarf malzemeleri ve tıbbi malzemeleri temsil eder.  
**Neden:** Envanter takibi için zorunludur.  
**Özelliği:** ItemCode, ItemName, ItemType (Medical, NonMedical, Equipment), Category, Unit, MinStockLevel, ReorderLevel, MaxStockLevel, UnitCost, StorageLocation, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Eczane, tedarik, departmanlar.  
**Amaç:** Envanter ana veri yönetimi.

#### 2. Stock Entity
**Ne:** Stok entity'si, her malzemenin mevcut stok durumunu temsil eder.  
**Neden:** Stok takibi için gereklidir.  
**Özelliği:** ItemId, BatchNumber, ExpiryDate, Quantity, UnitCost, WarehouseLocation, Status, ReceivedDate, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Tedarik, departmanlar.  
**Amaç:** Stok yönetimi.

#### 3. StockTransaction Entity
**Ne:** Stok hareketi entity'si, stok giriş-çıkış kayıtlarını temsil eder.  
**Neden:** Stok hareket takibi için gereklidir.  
**Özelliği:** TransactionId, ItemId, TransactionType (Purchase, Consumption, Transfer, Adjustment, Return), Quantity, UnitCost, ReferenceId, ReferenceType, PerformedBy, TransactionDate, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Tedarik, departmanlar.  
**Amaç:** Stok hareket yönetimi.

#### 4. Supplier Entity
**Ne:** Tedarikçi entity'si, malzeme tedarikçilerini temsil eder.  
**Neden:** Tedarikçi yönetimi için gereklidir.  
**Özelliği:** SupplierCode, SupplierName, ContactPerson, Email, Phone, Address, TaxNumber, BankAccount, PaymentTerms, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Tedarikçi yönetimi.

#### 5. PurchaseOrder Entity
**Ne:** Satın alma siparişi entity'si, tedarik siparişlerini temsil eder.  
**Neden:** Satın alma sürecinin takibi için gereklidir.  
**Özelliği:** OrderNumber, SupplierId, OrderDate, ExpectedDeliveryDate, ActualDeliveryDate, Status (Draft, Submitted, Approved, Partial, Received, Cancelled), TotalAmount, Currency, CreatedBy, ApprovedBy, Notes, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Satın alma siparişi yönetimi.

#### 6. PurchaseOrderItem Entity
**Ne:** Sipariş kalemi entity'si, her siparişteki kalemleri temsil eder.  
**Neden:** Sipariş-kalem ilişkisi için gereklidir.  
**Özelliği:** OrderId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, Status, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş kalem yönetimi.

### Domain Events

#### StockReceivedEvent
**Ne:** Stok girişi yapıldığında tetiklenen event'tir.  
**Neden:** Stok girişi duyurusu için gereklidir.  
**Özelliği:** TransactionId, itemId, quantity, receivedAt içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Stok girişi duyurusu.

#### StockConsumedEvent
**Ne:** Stok tüketildiğinde tetiklenen event'tir.  
**Neden:** Tüketim duyurusu için gereklidir.  
**Özelliği:** TransactionId, itemId, quantity, consumedBy içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Tüketim duyurusu.

#### LowStockAlertEvent
**Ne:** Düşük stok uyarısı tetiklenen event'tir.  
**Neden:** Stok kritik seviyeye düştüğünde uyarı gereklidir.  
**Özelliği:** ItemId, currentStock, reorderLevel içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Stok uyarısı.

#### PurchaseOrderCreatedEvent
**Ne:** Satın alma siparişi oluşturulduğunda tetiklenen event'tir.  
**Neden:** Sipariş duyurusu için gereklidir.  
**Özelliği:** OrderId, supplierId, totalAmount içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Sipariş duyurusu.

#### PurchaseOrderReceivedEvent
**Ne:** Sipariş teslim alındığında tetiklenen event'tir.  
**Neden:** Teslim duyurusu için gereklidir.  
**Özelliği:** OrderId, receivedAt, receivedBy içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Teslim duyurusu.

### Commands

#### CreatePurchaseOrderCommand
**Ne:** Satın alma siparişi oluşturma command'i.  
**Neden:** Sipariş kaydı için gereklidir.  
**Özelliği:** SupplierId, items, expectedDeliveryDate, notes parametreleri alır.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş oluşturma.

#### ApprovePurchaseOrderCommand
**Ne:** Sipariş onaylama command'i.  
**Neden:** Sipariş onayı için gereklidir.  
**Özelliği:** OrderId, approvedBy parametreleri alır.  
**Kim Kullanacak:** Yöneticiler.  
**Amaç:** Sipariş onaylama.

#### ReceivePurchaseOrderCommand
**Ne:** Sipariş teslim alma command'i.  
**Neden:** Teslim alma için gereklidir.  
**Özelliği:** OrderId, receivedItems, receivedBy parametreleri alır.  
**Kim Kullanacak:** Tedarik personeli.  
**Amaç:** Sipariş teslim alma.

#### CancelPurchaseOrderCommand
**Ne:** Sipariş iptal command'i.  
**Neden:** Sipariş iptali için gereklidir.  
**Özelliği:** OrderId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş iptal.

#### RecordStockConsumptionCommand
**Ne:** Stok tüketimi kaydetme command'i.  
**Neden:** Tüketim kaydı için gereklidir.  
**Özelliği:** ItemId, quantity, departmentId, referenceId, referenceType parametreleri alır.  
**Kim Kullanacak:** Departmanlar.  
**Amaç:** Tüketim kaydetme.

#### AdjustStockCommand
**Ne:** Stok düzeltme command'i.  
**Neden:** Sayım farkı düzeltmesi için gereklidir.  
**Özelliği:** ItemId, newQuantity, reason, adjustedBy parametreleri alır.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Stok düzeltme.

#### CreateSupplierCommand
**Ne:** Tedarikçi oluşturma command'i.  
**Neden:** Tedarikçi kaydı için gereklidir.  
**Özelliği:** SupplierName, contactPerson, email, phone, address, taxNumber parametreleri alır.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Tedarikçi oluşturma.

### Queries

#### GetInventoryItemsQuery
**Ne:** Envanter kalemlerini getirme query'si.  
**Neden:** Envanter listesi için gereklidir.  
**Özelliği:** Filters, pagination alır, PaginatedResult<InventoryItemDTO> döner.  
**Kim Kullanacak:** Tedarik, departmanlar.  
**Amaç:** Envanter sorgulama.

#### GetStockLevelQuery
**Ne:** Stok seviyesi getirme query'si.  
**Neden:** Stok sorgulama için gereklidir.  
**Özelliği:** ItemId alır, StockLevelDTO döner.  
**Kim Kullanacak:** Departmanlar.  
**Amaç:** Stok sorgulama.

#### GetLowStockItemsQuery
**Ne:** Düşük stoklu kalemleri getirme query'si.  
**Neden:** Stok uyarısı için gereklidir.  
**Özelliği:** TenantId alır, List<LowStockItemDTO> döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Düşük stoklu kalemler.

#### GetStockTransactionsQuery
**Ne:** Stok hareketlerini getirme query'si.  
**Neden:** Hareket takibi için gereklidir.  
**Özelliği:** ItemId, dateRange alır, List<StockTransactionDTO> döner.  
**Kim Kullanacak:** Tedarik, departmanlar.  
**Amaç:** Stok hareketleri.

#### GetPurchaseOrderByIdQuery
**Ne:** Sipariş detay getirme query'si.  
**Neden:** Sipariş detay sorgulama için gereklidir.  
**Özelliği:** OrderId alır, PurchaseOrderDetailDTO döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş sorgulama.

#### GetPendingPurchaseOrdersQuery
**Ne:** Bekleyen siparişleri getirme query'si.  
**Neden:** Bekleyen siparişler için gereklidir.  
**Özelliği:** Status alır, List<PurchaseOrderDTO> döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Bekleyen siparişler.

#### GetSuppliersQuery
**Ne:** Tedarikçileri getirme query'si.  
**Neden:** Tedarikçi listesi için gereklidir.  
**Özelliği:** Filters alır, List<SupplierDTO> döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Tedarikçi listesi.

### Application Services

#### IInventoryService
**Ne:** Envanter servisi.  
**Neden:** Envanter operasyonlarının kapsüllenmesi.  
**Özelliği:** GetItems, GetStockLevel, RecordConsumption, AdjustStock metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Envanter yönetimi.

#### IPurchaseOrderService
**Ne:** Satın alma servisi.  
**Neden:** Satın alma operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateOrder, ApproveOrder, ReceiveOrder, CancelOrder metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Satın alma yönetimi.

#### ISupplierService
**Ne:** Tedarikçi servisi.  
**Neden:** Tedarikçi operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateSupplier, UpdateSupplier, GetSuppliers metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Tedarikçi yönetimi.

### API Endpoints

#### POST /api/v1/inventory/purchase-orders
**Ne:** Sipariş oluşturma endpoint'i.  
**Neden:** Sipariş kaydı.  
**Özelliği:** CreatePurchaseOrderCommand alır, PurchaseOrderDTO döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş oluşturma.

#### GET /api/v1/inventory/purchase-orders/{id}
**Ne:** Sipariş detay endpoint'i.  
**Neden:** Sipariş sorgulama.  
**Özelliği:** OrderId alır, PurchaseOrderDetailDTO döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Sipariş sorgulama.

#### PUT /api/v1/inventory/purchase-orders/{id}/approve
**Ne:** Sipariş onaylama endpoint'i.  
**Neden:** Sipariş onaylama.  
**Özelliği:** OrderId alır, SuccessResponse döner.  
**Kim Kullanacak:** Yöneticiler.  
**Amaç:** Sipariş onaylama.

#### PUT /api/v1/inventory/purchase-orders/{id}/receive
**Ne:** Sipariş teslim alma endpoint'i.  
**Neden:** Teslim alma.  
**Özelliği:** ReceivePurchaseOrderCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Tedarik personeli.  
**Amaç:** Teslim alma.

#### POST /api/v1/inventory/consumption
**Ne:** Stok tüketimi kaydetme endpoint'i.  
**Neden:** Tüketim kaydı.  
**Özelliği:** RecordStockConsumptionCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Departmanlar.  
**Amaç:** Tüketim kaydetme.

#### GET /api/v1/inventory/items
**Ne:** Envanter kalemleri endpoint'i.  
**Neden:** Envanter listesi.  
**Özelliği:** Filters alır, PaginatedResult döner.  
**Kim Kullanacak:** Tedarik, departmanlar.  
**Amaç:** Envanter sorgulama.

#### GET /api/v1/inventory/items/{id}/stock
**Ne:** Stok seviyesi endpoint'i.  
**Neden:** Stok sorgulama.  
**Özelliği:** ItemId alır, StockLevelDTO döner.  
**Kim Kullanacak:** Departmanlar.  
**Amaç:** Stok sorgulama.

#### GET /api/v1/inventory/low-stock
**Ne:** Düşük stoklu kalemler endpoint'i.  
**Neden:** Stok uyarısı.  
**Özelliği:** Boş parametre alır, List<LowStockItemDTO> döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Düşük stoklu kalemler.

#### POST /api/v1/inventory/suppliers
**Ne:** Tedarikçi oluşturma endpoint'i.  
**Neden:** Tedarikçi kaydı.  
**Özelliği:** CreateSupplierCommand alır, SupplierDTO döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Tedarikçi oluşturma.

#### GET /api/v1/inventory/suppliers
**Ne:** Tedarikçiler endpoint'i.  
**Neden:** Tedarikçi listesi.  
**Özelliği:** Filters alır, List<SupplierDTO> döner.  
**Kim Kullanacak:** Tedarik yöneticileri.  
**Amaç:** Tedarikçi listesi.

---

## SPRINT 17: Stabilizasyon

### Sprint Hedefi
FAZ 2'nin stabilizasyon sprint'idir. Tüm modüller entegre edilecek, test edilecek ve production'a hazır hale getirilecektir.

### Yapılacak İşler

#### Entegrasyon Testleri
- Inpatient, Emergency, Pharmacy, Inventory, Procurement modülleri arası entegrasyon testleri
- Hasta akış testleri (acil -> yatan -> taburcu)
- İlaç tedarik zinciri testleri
- Stok-fatura entegrasyon testleri

#### Performans Testleri
- Load test: 300 concurrent user
- Stress test: 1500 concurrent user
- Inventory query performance test
- Emergency triage queue performance test

#### Güvenlik Testleri
- Kontrollü ilaç erişim testleri
- Stok yönetimi yetki testleri

#### Dokümantasyon
- API dokümantasyonu güncellemesi
- Kullanıcı kılavuzu güncellemesi

---

## FAZ 2 ÖZETİ

### Tamamlanacak Modüller

| Modül | Sprint | Öncelik | Bağımlılıklar |
|-------|--------|---------|---------------|
| Inpatient | 12-13 | Critical | Patient, Appointment |
| Emergency | 14 | Critical | Patient |
| Pharmacy | 15 | Critical | Patient, Outpatient, Inventory |
| Inventory | 16 | High | Procurement |
| Procurement | 16 | High | Inventory |

### Kritik Başarı Kriterleri

1. Yatan hasta kabul, transfer ve taburculuk çalışıyor olmalı
2. Tedavi order yönetimi çalışıyor olmalı
3. Acil servis triyaj ve müdahale çalışıyor olmalı
4. Eczane ilaç dağıtım ve stok takibi çalışıyor olmalı
5. Envanter ve tedarik yönetimi çalışıyor olmalı
6. Kontrollü ilaç takibi yapılıyor olmalı
7. Unit test coverage %80'in üzerinde olmalı
