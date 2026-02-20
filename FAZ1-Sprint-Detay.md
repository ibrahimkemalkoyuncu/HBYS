# FAZ 1: Ayaktan Hasta Hizmetleri Modülleri
## Sprint 6-10 Detaylı Domain Task Listesi

---

## SPRINT 6: Patient Module - Patient Registration

### Sprint Hedefi
Bu sprint, HBYS sisteminin en temel modülü olan Hasta (Patient) modülünün kayıt ve temel bilgi yönetimi kısmını kapsamaktadır. Hasta kayıt, demografik bilgi yönetimi ve hasta arama işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. Patient Entity
**Ne:** Hasta entity'si, hastaneye başvuran tüm bireyleri temsil eder.  
**Neden:** HBYS'nin merkezi entity'sidir ve tüm hasta işlemleri bu entity üzerinden yapılır.  
**Özelliği:** PatientNumber (unique), TurkishId (encrypted), FirstName, LastName, Gender, DateOfBirth, BloodType, ContactInfo, Address, PhotoUrl, RegistrationDate, Status, IsDeleted özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, poliklinik, acil servis, yatan hasta, laboratuvar, radyoloji, eczane, faturalama ve diğer tüm modüller.  
**Amaç:** Hasta bilgilerinin merkezi yönetimi ve tüm hasta işlemlerinin izlenmesi.

#### 2. PatientContact Entity
**Ne:** Hasta iletişim bilgilerini temsil eder.  
**Neden:** Birden fazla iletişim bilgisi (telefon, e-posta, adres) desteklenmelidir.  
**Özelliği:** PatientId, ContactType (Phone, Email, Address), Value, IsPrimary, IsVerified özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, hasta iletişim, bildirim sistemi.  
**Amaç:** Hasta iletişim bilgilerinin yönetimi.

#### 3. PatientGuardian Entity
**Ne:** Hasta yakını (veli, vasil) bilgilerini temsil eder.  
**Neden:** Özellikle çocuk hastalar ve yasal yetki gerektiren durumlar için gereklidir.  
**Özelliği:** PatientId, GuardianType, FirstName, LastName, TurkishId, Phone, Address, Relationship özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, yatan hasta, yasal işlemler.  
**Amaç:** Hasta yakını bilgilerinin yönetimi.

#### 4. PatientInsurance Entity
**Ne:** Hasta sigorta bilgilerini temsil eder.  
**Neden:** Faturalama için sigorta bilgileri kritiktir.  
**Özelliği:** PatientId, InsuranceType (SGK, Özel Sigorta, Sözleşmeli Kurum), InsuranceProvider, PolicyNumber, ExpiryDate, DiscountRate, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama, hasta kabul, poliklinik.  
**Amaç:** Sigorta bilgilerinin yönetimi.

#### 5. PatientIdentifier Entity
**Ne:** Hasta kimlik belgelerini temsil eder.  
**Neden:** Birden fazla kimlik belgesi desteklenebilir (TC Kimlik, Pasaport, Ehliyet vb.).  
**Özelliği:** PatientId, IdentifierType, DocumentNumber, IssueDate, ExpiryDate, IssuerCountry özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, güvenlik.  
**Amaç:** Kimlik belgesi yönetimi.

### Domain Events

#### PatientRegisteredEvent
**Ne:** Hasta kaydedildiğinde tetiklenen event'tir.  
**Neden:** Diğer modüllerin hasta kaydına tepki vermesini sağlar.  
**Özelliği:** PatientId, patientNumber, tenantId, registeredAt, registeredBy içerir.  
**Kim Kullanacak:** Notification modülü, Audit modülü, Appointment modülü.  
**Amaç:** Hasta kaydının duyurulması.

#### PatientUpdatedEvent
**Ne:** Hasta bilgileri güncellendiğinde tetiklenen event'tir.  
**Neden:** Bilgi güncellemelerinin yayılması gerekir.  
**Özelliği:** PatientId, updatedFields, updatedBy, updatedAt içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Hasta güncelleme duyurusu.

#### PatientVerifiedEvent
**Ne:** Hasta kimliği doğrulandığında tetiklenen event'tir.  
**Neden:** Kimlik doğrulama işleminin loglanması gerekir.  
**Özelliği:** PatientId, verifiedBy, verifiedAt içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Kimlik doğrulama duyurusu.

### Commands

#### RegisterPatientCommand
**Ne:** Yeni hasta kaydetme command'i.  
**Neden:** CQRS pattern ile hasta kaydı için gereklidir.  
**Özelliği:** TurkishId, firstName, lastName, gender, dateOfBirth, contactInfos, insuranceInfo parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Yeni hasta kaydı.

#### UpdatePatientCommand
**Ne:** Hasta bilgilerini güncelleme command'i.  
**Neden:** Hasta bilgi güncellemeleri için gereklidir.  
**Özelliği:** PatientId, firstName, lastName, contactInfos, updateReason parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli, hasta kendisi (portal üzerinden).  
**Amaç:** Hasta bilgi güncelleme.

#### VerifyPatientIdentityCommand
**Ne:** Hasta kimliğini doğrulama command'i.  
**Neden:** Kimlik doğrulama gereksinimi için gereklidir.  
**Özelliği:** PatientId, documentType, documentNumber, verifiedBy parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Hasta kimlik doğrulama.

#### AddPatientInsuranceCommand
**Ne:** Hasta sigortası ekleme command'i.  
**Neden:** Sigorta bilgisi ekleme gereksinimi için gereklidir.  
**Özelliği:** PatientId, insuranceType, provider, policyNumber, discountRate parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli, faturalama personeli.  
**Amaç:** Sigorta bilgisi ekleme.

#### UpdatePatientInsuranceCommand
**Ne:** Hasta sigortasını güncelleme command'i.  
**Neden:** Sigorta bilgisi güncelleme gereksinimi için gereklidir.  
**Özelliği:** InsuranceId, newProvider, newPolicyNumber, newDiscountRate parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Sigorta bilgisi güncelleme.

#### AddPatientGuardianCommand
**Ne:** Hasta yakını ekleme command'i.  
**Neden:** Yakın bilgisi ekleme gereksinimi için gereklidir.  
**Özelliği:** PatientId, guardianType, firstName, lastName, phone, relationship parametreleri alır.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Yakın bilgisi ekleme.

#### DeactivatePatientCommand
**Ne:** Hasta kaydını pasif etme command'i.  
**Neden:** Hasta kaydı deaktivasyonu gerektiğinde kullanılır.  
**Özelliği:** PatientId, reason, deactivatedBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Hasta deaktifleme.

### Queries

#### GetPatientByIdQuery
**Ne:** ID'ye göre hasta getirme query'si.  
**Neden:** Hasta detay sayfası için gereklidir.  
**Özelliği:** PatientId alır, PatientDetailDTO döner.  
**Kim Kullanacak:** Tüm modüller (hasta bilgisi gerektiğinde).  
**Amaç:** Hasta sorgulama.

#### GetPatientByTurkishIdQuery
**Ne:** TC Kimlik No'ya göre hasta getirme query'si.  
**Neden:** TC Kimlik ile arama için gereklidir.  
**Özelliği:** TurkishId alır, PatientDTO döner.  
**Kim Kullanacak:** Hasta kabul, poliklinik.  
**Amaç:** TC Kimlik ile hasta sorgulama.

#### SearchPatientsQuery
**Ne:** Hasta arama query'si.  
**Neden:** Hasta arama ekranı için gereklidir.  
**Özelliği:** SearchTerm (ad, soyad, hasta no, TC), filters, pagination alır, PaginatedResult<PatientListDTO> döner.  
**Kim Kullanacak:** Tüm hasta işlemi yapan personel.  
**Amaç:** Hasta arama.

#### GetPatientAppointmentsQuery
**Ne:** Hasta randevularını getirme query'si.  
**Neden:** Hasta randevu geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hasta kabul, poliklinik.  
**Amaç:** Hasta randevuları sorgulama.

#### GetPatientVisitHistoryQuery
**Ne:** Hasta ziyaret geçmişini getirme query'si.  
**Neden:** Hasta muayene geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<VisitSummaryDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri, hasta kendisi.  
**Amaç:** Hasta geçmişi sorgulama.

#### GetPatientInsurancesQuery
**Ne:** Hasta sigorta bilgilerini getirme query'si.  
**Neden:** Sigorta sorgulama için gereklidir.  
**Özelliği:** PatientId alır, List<InsuranceDTO> döner.  
**Kim Kullanacak:** Faturalama, hasta kabul.  
**Amaç:** Sigorta sorgulama.

### Application Services

#### IPatientRegistrationService
**Ne:** Hasta kayıt servisi.  
**Neden:** Kayıt operasyonlarının kapsüllenmesi.  
**Özelliği:** RegisterPatient, UpdatePatient, VerifyIdentity, DeactivatePatient metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Hasta kayıt yönetimi.

#### IPatientQueryService
**Ne:** Hasta sorgulama servisi.  
**Neden:** Sorgu operasyonlarının kapsüllenmesi.  
**Özelliği:** GetPatient, SearchPatients, GetPatientHistory metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Hasta sorgulama yönetimi.

#### IPatientInsuranceService
**Ne:** Hasta sigorta servisi.  
**Neden:** Sigorta operasyonlarının kapsüllenmesi.  
**Özelliği:** AddInsurance, UpdateInsurance, GetInsurances metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Sigorta yönetimi.

### API Endpoints

#### POST /api/v1/patients
**Ne:** Hasta kaydetme endpoint'i.  
**Neden:** Yeni hasta kaydı.  
**Özelliği:** RegisterPatientCommand alır, PatientDTO döner.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Hasta kaydetme.

#### GET /api/v1/patients/{id}
**Ne:** Hasta detay endpoint'i.  
**Neden:** Hasta bilgi sorgulama.  
**Özelliği:** PatientId alır, PatientDetailDTO döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Hasta sorgulama.

#### PUT /api/v1/patients/{id}
**Ne:** Hasta güncelleme endpoint'i.  
**Neden:** Hasta bilgi güncelleme.  
**Özelliği:** UpdatePatientCommand alır, PatientDTO döner.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Hasta güncelleme.

#### GET /api/v1/patients/search
**Ne:** Hasta arama endpoint'i.  
**Neden:** Hasta arama.  
**Özelliği:** SearchPatientsQuery alır, PaginatedResult döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Hasta arama.

#### POST /api/v1/patients/{id}/verify
**Ne:** Hasta kimlik doğrulama endpoint'i.  
**Neden:** Kimlik doğrulama.  
**Özelliği:** VerifyPatientIdentityCommand alır, VerificationResult döner.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Kimlik doğrulama.

#### GET /api/v1/patients/{id}/insurances
**Ne:** Hasta sigortaları endpoint'i.  
**Neden:** Sigorta sorgulama.  
**Özelliği:** PatientId alır, List<InsuranceDTO> döner.  
**Kim Kullanacak:** Faturalama, hasta kabul.  
**Amaç:** Sigorta sorgulama.

#### POST /api/v1/patients/{id}/insurances
**Ne:** Hasta sigortası ekleme endpoint'i.  
**Neden:** Sigorta ekleme.  
**Özelliği:** AddPatientInsuranceCommand alır, InsuranceDTO döner.  
**Kim Kullanacak:** Hasta kabul personeli.  
**Amaç:** Sigorta ekleme.

#### GET /api/v1/patients/{id}/appointments
**Ne:** Hasta randevuları endpoint'i.  
**Neden:** Randevu sorgulama.  
**Özelliği:** PatientId alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hasta kabul, hasta.  
**Amaç:** Randevu sorgulama.

---

## SPRINT 7: Patient Module - Patient Medical Information

### Sprint Hedefi
Bu sprint, Hasta modülünün tıbbi bilgi yönetimi kısmını kapsamaktadır. Alerji, kronik hastalık, ilaç kullanımı, aşı ve tıbbi özet bilgileri geliştirilecektir.

### Domain Entity'leri

#### 1. PatientAllergy Entity
**Ne:** Hasta alerji bilgilerini temsil eder.  
**Neden:** Güvenli tedavi için alerji bilgileri kritiktir.  
**Özelliği:** PatientId, AllergyType, Allergen, Severity, Reaction, OnsetDate, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, acil servis, yatan hasta, eczane.  
**Amaç:** Alerji bilgilerinin yönetimi.

#### 2. PatientChronicDisease Entity
**Ne:** Hasta kronik hastalık bilgilerini temsil eder.  
**Neden:** Kronik hastalık takibi için gereklidir.  
**Özelliği:** PatientId, DiseaseCode, DiseaseName, DiagnosisDate, Status, Notes, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, yatan hasta.  
**Amaç:** Kronik hastalık yönetimi.

#### 3. PatientMedication Entity
**Ne:** Hasta mevcut ilaç kullanım bilgilerini temsil eder.  
**Neden:** İlaç etkileşimi ve tedavi takibi için gereklidir.  
**Özelliği:** PatientId, MedicationName, Dosage, Frequency, StartDate, EndDate, PrescribedBy, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, eczane, yatan hasta.  
**Amaç:** İlaç kullanım yönetimi.

#### 4. PatientVaccination Entity
**Ne:** Hasta aşı bilgilerini temsil eder.  
**Neden:** Aşı takibi için gereklidir.  
**Özelliği:** PatientId, VaccineName, VaccineType, DoseNumber, VaccinationDate, NextDoseDate, AdministeredBy, BatchNumber özelliklerine sahiptir.  
**Kim Kullanacak:** Çocuk sağlığı, aile hekimliği.  
**Amaç:** Aşı yönetimi.

#### 5. PatientMedicalSummary Entity
**Ne:** Hasta tıbbi özet bilgilerini temsil eder.  
**Neden:** Hızlı tıbbi özet görüntüleme için gereklidir.  
**Özelliği:** PatientId, SummaryText, BloodType, RhFactor, Height, Weight, LastUpdated, UpdatedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Tüm klinik modüller.  
**Amaç:** Tıbbi özet yönetimi.

#### 6. PatientMedicalHistory Entity
**Ne:** Hasta tıbbi geçmiş bilgilerini temsil eder.  
**Neden:** Geçmiş tedavilerin takibi için gereklidir.  
**Özelliği:** PatientId, EventType, EventDate, Description, Diagnosis, Treatment, AttendingPhysician, Department özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, yatan hasta.  
**Amaç:** Tıbbi geçmiş yönetimi.

### Domain Events

#### AllergyAddedEvent
**Ne:** Alerji eklendiğinde tetiklenen event'tir.  
**Neden:** Alerji ekleme duyurusu için gereklidir.  
**Özelliği:** PatientId, allergyId, allergen, addedBy içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Alerji ekleme duyurusu.

#### ChronicDiseaseDiagnosedEvent
**Ne:** Kronik hastalık tanısı konulduğunda tetiklenen event'tir.  
**Neden:** Tanı duyurusu için gereklidir.  
**Özelliği:** PatientId, diseaseId, diseaseName, diagnosedAt, diagnosedBy içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Kronik hastalık duyurusu.

### Commands

#### AddPatientAllergyCommand
**Ne:** Hasta alerjisi ekleme command'i.  
**Neden:** Alerji kaydı için gereklidir.  
**Özelliği:** PatientId, allergyType, allergen, severity, reaction parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri, hemşireler.  
**Amaç:** Alerji ekleme.

#### UpdatePatientAllergyCommand
**Ne:** Hasta alerjisi güncelleme command'i.  
**Neden:** Alerji bilgi güncellemesi için gereklidir.  
**Özelliği:** AllergyId, newSeverity, newReaction parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Alerji güncelleme.

#### AddChronicDiseaseCommand
**Ne:** Kronik hastalık ekleme command'i.  
**Neden:** Kronik hastalık kaydı için gereklidir.  
**Özelliği:** PatientId, diseaseCode, diseaseName, diagnosisDate, notes parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Kronik hastalık ekleme.

#### AddPatientMedicationCommand
**Ne:** Hasta ilacı ekleme command'i.  
**Neden:** İlaç kullanım kaydı için gereklidir.  
**Özelliği:** PatientId, medicationName, dosage, frequency, startDate, prescribedBy parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri, eczane.  
**Amaç:** İlaç ekleme.

#### UpdateMedicalSummaryCommand
**Ne:** Tıbbi özet güncelleme command'i.  
**Neden:** Tıbbi özet güncellemesi için gereklidir.  
**Özelliği:** PatientId, summaryText, height, weight, updatedBy parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tıbbi özet güncelleme.

#### AddMedicalHistoryEventCommand
**Ne:** Tıbbi geçmiş olayı ekleme command'i.  
**Neden:** Geçmiş olay kaydı için gereklidir.  
**Özelliği:** PatientId, eventType, eventDate, description, diagnosis, treatment parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tıbbi geçmiş ekleme.

### Queries

#### GetPatientAllergiesQuery
**Ne:** Hasta alerjilerini getirme query'si.  
**Neden:** Alerji sorgulama için gereklidir.  
**Özelliği:** PatientId alır, List<AllergyDTO> döner.  
**Kim Kullanacak:** Poliklinik, acil, eczane.  
**Amaç:** Alerji sorgulama.

#### GetPatientChronicDiseasesQuery
**Ne:** Hasta kronik hastalıklarını getirme query'si.  
**Neden:** Kronik hastalık sorgulama için gereklidir.  
**Özelliği:** PatientId alır, List<ChronicDiseaseDTO> döner.  
**Kim Kullanacak:** Poliklinik, yatan hasta.  
**Amaç:** Kronik hastalık sorgulama.

#### GetPatientMedicationsQuery
**Ne:** Hasta ilaçlarını getirme query'si.  
**Neden:** İlaç sorgulama için gereklidir.  
**Özelliği:** PatientId alır, List<MedicationDTO> döner.  
**Kim Kullanacak:** Poliklinik, eczane.  
**Amaç:** İlaç sorgulama.

#### GetPatientMedicalSummaryQuery
**Ne:** Hasta tıbbi özet getirme query'si.  
**Neden:** Tıbbi özet sorgulama için gereklidir.  
**Özelliği:** PatientId alır, MedicalSummaryDTO döner.  
**Kim Kullanacak:** Tüm klinik personel.  
**Amaç:** Tıbbi özet sorgulama.

#### GetPatientMedicalHistoryQuery
**Ne:** Hasta tıbbi geçmiş getirme query'si.  
**Neden:** Tıbbi geçmiş sorgulama için gereklidir.  
**Özelliği:** PatientId, dateRange alır, List<MedicalHistoryDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tıbbi geçmiş sorgulama.

### Application Services

#### IMedicalInformationService
**Ne:** Tıbbi bilgi servisi.  
**Neden:** Tıbbi bilgi operasyonlarının kapsüllenmesi.  
**Özelliği:** AddAllergy, UpdateAllergy, AddChronicDisease, AddMedication, UpdateSummary, AddHistory metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Tıbbi bilgi yönetimi.

### API Endpoints

#### GET /api/v1/patients/{id}/allergies
**Ne:** Hasta alerjileri endpoint'i.  
**Neden:** Alerji sorgulama.  
**Özelliği:** PatientId alır, List<AllergyDTO> döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** Alerji sorgulama.

#### POST /api/v1/patients/{id}/allergies
**Ne:** Hasta alerjisi ekleme endpoint'i.  
**Neden:** Alerji ekleme.  
**Özelliği:** AddPatientAllergyCommand alır, AllergyDTO döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** Alerji ekleme.

#### GET /api/v1/patients/{id}/chronic-diseases
**Ne:** Hasta kronik hastalıkları endpoint'i.  
**Neden:** Kronik hastalık sorgulama.  
**Özelliği:** PatientId alır, List<ChronicDiseaseDTO> döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** Kronik hastalık sorgulama.

#### GET /api/v1/patients/{id}/medications
**Ne:** Hasta ilaçları endpoint'i.  
**Neden:** İlaç sorgulama.  
**Özelliği:** PatientId alır, List<MedicationDTO> döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** İlaç sorgulama.

#### GET /api/v1/patients/{id}/medical-summary
**Ne:** Hasta tıbbi özet endpoint'i.  
**Neden:** Tıbbi özet sorgulama.  
**Özelliği:** PatientId alır, MedicalSummaryDTO döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** Tıbbi özet sorgulama.

#### PUT /api/v1/patients/{id}/medical-summary
**Ne:** Hasta tıbbi özet güncelleme endpoint'i.  
**Neden:** Tıbbi özet güncelleme.  
**Özelliği:** UpdateMedicalSummaryCommand alır, MedicalSummaryDTO döner.  
**Kim Kullanacak:** Klinik personel.  
**Amaç:** Tıbbi özet güncelleme.

---

## SPRINT 8: Appointment Module

### Sprint Hedefi
Bu sprint, randevu yönetim modülünü kapsamaktadır. Randevu oluşturma, iptal etme, yeniden planlama ve randevu takvimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. Appointment Entity
**Ne:** Randevu entity'si, hasta-hekim randevularını temsil eder.  
**Neden:** Hastane operasyonlarının temelini oluşturur ve kaynak planlaması için gereklidir.  
**Özelliği:** AppointmentNumber, PatientId, PhysicianId, DepartmentId, AppointmentDate, StartTime, EndTime, AppointmentType (Checkup, FollowUp, Procedure), Status (Scheduled, Confirmed, Completed, Cancelled, NoShow), Reason, Notes, CreatedBy, CreatedAt, ConfirmedAt, CancelledAt, CancelledBy, CancelledReason özelliklerine sahiptir.  
**Kim Kullanacak:** Hasta kabul, poliklinik, hasta, hekim sekreterleri.  
**Amaç:** Randevu yönetimi.

#### 2. AppointmentSlot Entity
**Ne:** Randevu slotu entity'si, hekimlerin müsait zaman dilimlerini temsil eder.  
**Neden:** Randevu planlaması için müsaitlik bilgisi gereklidir.  
**Özelliği:** PhysicianId, DepartmentId, DayOfWeek, StartTime, EndTime, SlotDuration, IsActive özellikine sahiptir.  
**Kim Kullanacak:** Randevu sistemi.  
**Amaç:** Müsaitlik yönetimi.

#### 3. AppointmentSchedule Exception Entity
**Ne:** Randevu istisna (tatil, eğitim vb.) bilgilerini temsil eder.  
**Neden:** Planlı olmayan durumların yönetimi için gereklidir.  
**Özelliği:** PhysicianId, ExceptionDate, ExceptionType (Holiday, Training, Sick), StartTime, EndTime, Reason özelliklerine sahiptir.  
**Kim Kullanacak:** Randevu sistemi.  
**Amaç:** İstisna yönetimi.

#### 4. AppointmentQueue Entity
**Ne:** Sıra numarası entity'si, günlük hasta sırasını temsil eder.  
**Neden:** Poliklinik sıra yönetimi için gereklidir.  
**Özelliği:** AppointmentId, QueueNumber, CalledAt, CalledBy, CompletedAt, WaitingTime özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, hasta kabul.  
**Amaç:** Sıra yönetimi.

### Domain Events

#### AppointmentCreatedEvent
**Ne:** Randevu oluşturulduğunda tetiklenen event'tir.  
**Neden:** Randevu oluşumunun duyurulması gerekir.  
**Özelliği:** AppointmentId, patientId, physicianId, appointmentDate, createdAt içerir.  
**Kim Kullanacak:** Notification modülü, Patient modülü.  
**Amaç:** Randevu oluşum duyurusu.

#### AppointmentConfirmedEvent
**Ne:** Randevu onaylandığında tetiklenen event'tir.  
**Neden:** Onay duyurusu için gereklidir.  
**Özelliği:** AppointmentId, confirmedAt, confirmedBy içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Randevu onay duyurusu.

#### AppointmentCancelledEvent
**Ne:** Randevu iptal edildiğinde tetiklenen event'tir.  
**Neden:** İptal duyurusu için gereklidir.  
**Özelliği:** AppointmentId, cancelledAt, cancelledBy, reason içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Randevu iptal duyurusu.

#### AppointmentCompletedEvent
**Ne:** Randevu tamamlandığında tetiklenen event'tir.  
**Neden:** Tamamlanma duyurusu için gereklidir.  
**Özelliği:** AppointmentId, completedAt, completedBy içerir.  
**Kim Kullanacak:** Patient modülü, Billing modülü.  
**Amaç:** Randevu tamamlanma duyurusu.

#### AppointmentReminderEvent
**Ne:** Randevu hatırlatması için tetiklenen event'tir.  
**Neden:** Hasta bilgilendirme için gereklidir.  
**Özelliği:** AppointmentId, patientId, reminderTime, appointmentTime içerir.  
**Kim Kullanacak:** Notification modülü, Background Jobs.  
**Amaç:** Randevu hatırlatma.

### Commands

#### CreateAppointmentCommand
**Ne:** Randevu oluşturma command'i.  
**Neden:** Randevu kaydı için gereklidir.  
**Özelliği:** PatientId, physicianId, departmentId, appointmentDate, startTime, appointmentType, reason parametreleri alır.  
**Kim Kullanacak:** Hasta kabul, hasta (portal), hekim sekreteri.  
**Amaç:** Randevu oluşturma.

#### ConfirmAppointmentCommand
**Ne:** Randevu onaylama command'i.  
**Neden:** Randevu onayı için gereklidir.  
**Özelliği:** AppointmentId, confirmedBy parametreleri alır.  
**Kim Kullanacak:** Hasta, hekim sekreteri.  
**Amaç:** Randevu onaylama.

#### CancelAppointmentCommand
**Ne:** Randevu iptal etme command'i.  
**Neden:** Randevu iptali için gereklidir.  
**Özelliği:** AppointmentId, cancelledBy, reason parametreleri alır.  
**Kim Kullanacak:** Hasta, hekim sekreteri, sistem (otomatik).  
**Amaç:** Randevu iptal etme.

#### RescheduleAppointmentCommand
**Ne:** Randevu yeniden planlama command'i.  
**Neden:** Randevu tarih değişikliği için gereklidir.  
**Özelliği:** AppointmentId, newDate, newTime, rescheduledBy, reason parametreleri alır.  
**Kim Kullanacak:** Hasta, hekim sekreteri.  
**Amaç:** Randevu yeniden planlama.

#### CompleteAppointmentCommand
**Ne:** Randevu tamamlama command'i.  
**Neden:** Randevu tamamlama için gereklidir.  
**Özelliği:** AppointmentId, completedBy, notes parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimi.  
**Amaç:** Randevu tamamlama.

#### MarkNoShowCommand
**Ne:** Gelmedi işaretleme command'i.  
**Neden:** Gelmeyen hastayı işaretlemek için gereklidir.  
**Özelliği:** AppointmentId, markedBy parametreleri alır.  
**Kim Kullanacak:** Poliklinik.  
**Amaç:** Gelmedi işaretleme.

#### UpdateAppointmentSlotCommand
**Ne:** Randevu slotu güncelleme command'i.  
**Neden:** Müsaitlik güncellemesi için gereklidir.  
**Özelliği:** PhysicianId, dayOfWeek, startTime, endTime, slotDuration parametreleri alır.  
**Kim Kullanacak:** Hekim sekreteri, sistem yöneticisi.  
**Amaç:** Slot güncelleme.

#### CreateAppointmentExceptionCommand
**Ne:** Randevu istisnası oluşturma command'i.  
**Neden:** İstisna ekleme için gereklidir.  
**Özelliği:** PhysicianId, exceptionDate, exceptionType, startTime, endTime, reason parametreleri alır.  
**Kim Kullanacak:** Hekim sekreteri.  
**Amaç:** İstisna oluşturma.

### Queries

#### GetAvailableSlotsQuery
**Ne:** Müsait slotları getirme query'si.  
**Neden:** Randevu planlama için müsaitlik sorgulama gereklidir.  
**Özelliği:** PhysicianId, departmentId, date alır, List<TimeSlotDTO> döner.  
**Kim Kullanacak:** Hasta, hasta kabul.  
**Amaç:** Müsaitlik sorgulama.

#### GetAppointmentByIdQuery
**Ne:** Randevu detay getirme query'si.  
**Neden:** Randevu detay sorgulama için gereklidir.  
**Özelliği:** AppointmentId alır, AppointmentDetailDTO döner.  
**Kim Kullanacak:** İlgili kullanıcılar.  
**Amaç:** Randevu sorgulama.

#### GetPatientAppointmentsQuery
**Ne:** Hasta randevularını getirme query'si.  
**Neden:** Hasta randevu geçmişi için gereklidir.  
**Özelliği:** PatientId, status, dateRange alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hasta, hasta kabul.  
**Amaç:** Hasta randevuları sorgulama.

#### GetPhysicianAppointmentsQuery
**Ne:** Hekim randevularını getirme query'si.  
**Neden:** Hekim günlük programı için gereklidir.  
**Özelliği:** PhysicianId, date alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hekim, hekim sekreteri.  
**Amaç:** Hekim randevuları sorgulama.

#### GetDepartmentAppointmentsQuery
**Ne:** Departman randevularını getirme query'si.  
**Neden:** Departman planlaması için gereklidir.  
**Özelliği:** DepartmentId, date alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Departman yöneticisi.  
**Amaç:** Departman randevuları sorgulama.

#### GetDailyQueueQuery
**Ne:** Günlük sırayı getirme query'si.  
**Neden:** Günlük sıra görüntüleme için gereklidir.  
**Özelliği:** DepartmentId, physicianId, date alır, List<QueueItemDTO> döner.  
**Kim Kullanacak:** Poliklinik, hasta kabul.  
**Amaç:** Sıra sorgulama.

### Application Services

#### IAppointmentService
**Ne:** Randevu servisi.  
**Neden:** Randevu operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateAppointment, ConfirmAppointment, CancelAppointment, RescheduleAppointment, CompleteAppointment metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Randevu yönetimi.

#### IAvailabilityService
**Ne:** Müsaitlik servisi.  
**Neden:** Müsaitlik operasyonlarının kapsüllenmesi.  
**Özelliği:** GetAvailableSlots, UpdateSlots, CreateException metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Müsaitlik yönetimi.

#### IQueueService
**Ne:** Sıra servisi.  
**Neden:** Sıra operasyonlarının kapsüllenmesi.  
**Özelliği:** GetQueue, CallNext, CompleteCurrent metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Sıra yönetimi.

### API Endpoints

#### POST /api/v1/appointments
**Ne:** Randevu oluşturma endpoint'i.  
**Neden:** Randevu oluşturma.  
**Özelliği:** CreateAppointmentCommand alır, AppointmentDTO döner.  
**Kim Kullanacak:** Hasta kabul, hasta.  
**Amaç:** Randevu oluşturma.

#### GET /api/v1/appointments/{id}
**Ne:** Randevu detay endpoint'i.  
**Neden:** Randevu sorgulama.  
**Özelliği:** AppointmentId alır, AppointmentDetailDTO döner.  
**Kim Kullanacak:** İlgili kullanıcılar.  
**Amaç:** Randevu sorgulama.

#### PUT /api/v1/appointments/{id}/confirm
**Ne:** Randevu onaylama endpoint'i.  
**Neden:** Randevu onaylama.  
**Özelliği:** AppointmentId alır, SuccessResponse döner.  
**Kim Kullanacak:** Hasta.  
**Amaç:** Randevu onaylama.

#### PUT /api/v1/appointments/{id}/cancel
**Ne:** Randevu iptal endpoint'i.  
**Neden:** Randevu iptal.  
**Özelliği:** CancelAppointmentCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Hasta, hekim sekreteri.  
**Amaç:** Randevu iptal.

#### PUT /api/v1/appointments/{id}/reschedule
**Ne:** Randevu yeniden planlama endpoint'i.  
**Neden:** Randevu tarih değişikliği.  
**Özelliği:** RescheduleAppointmentCommand alır, AppointmentDTO döner.  
**Kim Kullanacak:** Hasta, hekim sekreteri.  
**Amaç:** Randevu yeniden planlama.

#### PUT /api/v1/appointments/{id}/complete
**Ne:** Randevu tamamlama endpoint'i.  
**Neden:** Randevu tamamlama.  
**Özelliği:** CompleteAppointmentCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Poliklinik hekimi.  
**Amaç:** Randevu tamamlama.

#### GET /api/v1/appointments/available-slots
**Ne:** Müsait slotlar endpoint'i.  
**Neden:** Müsaitlik sorgulama.  
**Özelliği:** PhysicianId, departmentId, date alır, List<TimeSlotDTO> döner.  
**Kim Kullanacak:** Hasta, hasta kabul.  
**Amaç:** Müsaitlik sorgulama.

#### GET /api/v1/appointments/patient/{patientId}
**Ne:** Hasta randevuları endpoint'i.  
**Neden:** Hasta randevuları sorgulama.  
**Özelliği:** PatientId alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hasta, hasta kabul.  
**Amaç:** Hasta randevuları.

#### GET /api/v1/appointments/physician/{physicianId}/date/{date}
**Ne:** Hekim randevuları endpoint'i.  
**Neden:** Hekim randevuları sorgulama.  
**Özelliği:** PhysicianId, date alır, List<AppointmentDTO> döner.  
**Kim Kullanacak:** Hekim.  
**Amaç:** Hekim randevuları.

#### GET /api/v1/appointments/department/{departmentId}/queue
**Ne:** Departman sırası endpoint'i.  
**Neden:** Sıra sorgulama.  
**Özelliği:** DepartmentId, date alır, List<QueueItemDTO> döner.  
**Kim Kullanacak:** Poliklinik, hasta.  
**Amaç:** Sıra sorgulama.

---

## SPRINT 9: Outpatient Module

### Sprint Hedefi
Bu sprint, poliklinik muayene modülünü kapsamaktadır. Muayene kaydı, tanı, tedavi reçetesi ve epikriz işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. OutpatientEncounter Entity
**Ne:** Poliklinik muayene entity'si, ayaktan hasta muayenelerini temsil eder.  
**Neden:** Poliklinik muayene kayıtları için zorunludur.  
**Özelliği:** EncounterNumber, PatientId, AppointmentId, PhysicianId, DepartmentId, EncounterDate, StartTime, EndTime, ChiefComplaint, PresentIllness, VitalSigns, PhysicalExam, Diagnosis, Treatment, FollowUpPlan, Notes, Status, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik hekimleri, hemşireler.  
**Amaç:** Poliklinik muayene yönetimi.

#### 2. Diagnosis Entity
**Ne:** Tanı entity'si, hastalık tanılarını temsil eder.  
**Neden:** ICD-10 kodlaması için gereklidir.  
**Özelliği:** ICDCode, DiagnosisName, DiagnosisType (Primary, Secondary, Additional), IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Tüm klinik modüller.  
**Amaç:** Tanı kodlama yönetimi.

#### 3. OutpatientDiagnosis Entity
**Ne:** Muayene tanı entity'si, her muayeneye ait tanıları temsil eder.  
**Neden:** Muayene-tanı ilişkisi için gereklidir.  
**Özelliği:** EncounterId, DiagnosisId, DiagnosisOrder, DiagnosisType, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene tanı yönetimi.

#### 4. Prescription Entity
**Ne:** Reçete entity'si, hekim reçetelerini temsil eder.  
**Neden:** Eczane entegrasyonu için gereklidir.  
**Özelliği:** PrescriptionNumber, EncounterId, PatientId, PhysicianId, PrescriptionDate, PrescriptionType, Status, IssuedAt, IssuedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik hekimleri, eczane.  
**Amaç:** Reçete yönetimi.

#### 5. PrescriptionItem Entity
**Ne:** Reçete kalemi entity'si, her reçetedeki ilaçları temsil eder.  
**Neden:** Reçete-ilaç ilişkisi için gereklidir.  
**Özelliği:** PrescriptionId, MedicationId, Dosage, Frequency, Duration, Quantity, UsageInstructions, IsPaid, IsProvided özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik hekimleri, eczane.  
**Amaç:** Reçete kalem yönetimi.

#### 6. VitalSign Entity
**Ne:** Vital bulgu entity'si, hasta hayati bulgularını temsil eder.  
**Neden:** Takip için gereklidir.  
**Özelliği:** EncounterId, RecordedAt, BloodPressureSystolic, BloodPressureDiastolic, Pulse, Temperature, RespiratoryRate, OxygenSaturation, RecordedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik, acil, yatan hasta.  
**Amaç:** Vital bulgu yönetimi.

#### 7. PhysicalExam Entity
**Ne:** Fizik muayene entity'si, fizik muayene bulgularını temsil eder.  
**Neden:** Muayene kaydı için gereklidir.  
**Özelliği:** EncounterId, ExamType, ExamResult, Notes, ExaminedBy, ExaminedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Fizik muayene yönetimi.

### Domain Events

#### EncounterCreatedEvent
**Ne:** Muayene oluşturulduğunda tetiklenen event'tir.  
**Neden:** Muayene oluşumunun duyurulması gerekir.  
**Özelliği:** EncounterId, patientId, physicianId, departmentId, encounterDate içerir.  
**Kim Kullanacak:** Billing modülü, Audit modülü.  
**Amaç:** Muayene oluşum duyurusu.

#### DiagnosisRecordedEvent
**Ne:** Tanı kaydedildiğinde tetiklenen event'tir.  
**Neden:** Tanı kaydının duyurulması gerekir.  
**Özelliği:** EncounterId, diagnosisId, icdCode içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Tanı kayıt duyurusu.

#### PrescriptionIssuedEvent
**Ne:** Reçete düzenlendiğinde tetiklenen event'tir.  
**Neden:** Reçete bilgisinin yayılması gerekir.  
**Özelliği:** PrescriptionId, patientId, physicianId, encounterId içerir.  
**Kim Kullanacak:** Pharmacy modülü, Notification modülü.  
**Amaç:** Reçete düzenleme duyurusu.

#### PrescriptionFilledEvent
**Ne:** Reçete eczanede verildiğinde tetiklenen event'tir.  
**Neden:** Reçete verme bilgisinin duyurulması gerekir.  
**Özelliği:** PrescriptionId, filledAt, filledBy içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Reçete verme duyurusu.

### Commands

#### CreateEncounterCommand
**Ne:** Muayene oluşturma command'i.  
**Neden:** Muayene kaydı için gereklidir.  
**Özelliği:** PatientId, appointmentId, physicianId, chiefComplaint, presentIllness parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene oluşturma.

#### UpdateEncounterCommand
**Ne:** Muayene güncelleme command'i.  
**Neden:** Muayene bilgi güncellemesi için gereklidir.  
**Özelliği:** EncounterId, vitalSigns, physicalExam, diagnosis, treatment, followUpPlan parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene güncelleme.

#### AddDiagnosisCommand
**Ne:** Tanı ekleme command'i.  
**Neden:** Tanı kaydı için gereklidir.  
**Özelliği:** EncounterId, diagnosisId, diagnosisType, notes parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tanı ekleme.

#### CreatePrescriptionCommand
**Ne:** Reçete oluşturma command'i.  
**Neden:** Reçete kaydı için gereklidir.  
**Özelliği:** EncounterId, patientId, prescriptionType, prescriptionItems parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Reçete oluşturma.

#### RecordVitalSignsCommand
**Ne:** Vital bulgu kaydetme command'i.  
**Neden:** Vital bulgu kaydı için gereklidir.  
**Özelliği:** EncounterId, bloodPressure, pulse, temperature, respiratoryRate, oxygenSaturation parametreleri alır.  
**Kim Kullanacak:** Poliklinik hemşireleri.  
**Amaç:** Vital bulgu kaydetme.

#### RecordPhysicalExamCommand
**Ne:** Fizik muayene kaydetme command'i.  
**Neden:** Fizik muayene kaydı için gereklidir.  
**Özelliği:** EncounterId, examType, examResult, notes parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Fizik muayene kaydetme.

#### CompleteEncounterCommand
**Ne:** Muayene tamamlama command'i.  
**Neden:** Muayene tamamlama için gereklidir.  
**Özelliği:** EncounterId, completedBy parametreleri alır.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene tamamlama.

### Queries

#### GetEncounterByIdQuery
**Ne:** Muayene detay getirme query'si.  
**Neden:** Muayene detay sorgulama için gereklidir.  
**Özelliği:** EncounterId alır, EncounterDetailDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri, hasta.  
**Amaç:** Muayene sorgulama.

#### GetPatientEncountersQuery
**Ne:** Hasta muayenelerini getirme query'si.  
**Neden:** Hasta muayene geçmişi için gereklidir.  
**Özelliği:** PatientId, dateRange alır, List<EncounterSummaryDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri, hasta.  
**Amaç:** Hasta muayene geçmişi.

#### GetPhysicianEncountersQuery
**Ne:** Hekim muayenelerini getirme query'si.  
**Neden:** Hekim günlük iş yükü için gereklidir.  
**Özelliği:** PhysicianId, date alır, List<EncounterSummaryDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Hekim muayeneleri.

#### GetPrescriptionByIdQuery
**Ne:** Reçete detay getirme query'si.  
**Neden:** Reçete detay sorgulama için gereklidir.  
**Özelliği:** PrescriptionId alır, PrescriptionDetailDTO döner.  
**Kim Kullanacak:** Eczane, hasta.  
**Amaç:** Reçete sorgulama.

#### GetPatientPrescriptionsQuery
**Ne:** Hasta reçetelerini getirme query'si.  
**Neden:** Hasta reçete geçmişi için gereklidir.  
**Özelliği:** PatientId alır, List<PrescriptionDTO> döner.  
**Kim Kullanacak:** Eczane, hasta.  
**Amaç:** Hasta reçeteleri.

#### SearchDiagnosesQuery
**Ne:** Tanı arama query'si.  
**Neden:** ICD kodu arama için gereklidir.  
**Özelliği:** SearchTerm alır, List<DiagnosisDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tanı arama.

### Application Services

#### IEncounterService
**Ne:** Muayene servisi.  
**Neden:** Muayene operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateEncounter, UpdateEncounter, CompleteEncounter, GetEncounter metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Muayene yönetimi.

#### IPrescriptionService
**Ne:** Reçete servisi.  
**Neden:** Reçete operasyonlarının kapsüllenmesi.  
**Özelliği:** CreatePrescription, GetPrescription, FillPrescription metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Reçete yönetimi.

#### IDiagnosisService
**Ne:** Tanı servisi.  
**Neden:** Tanı operasyonlarının kapsüllenmesi.  
**Özelliği:** SearchDiagnoses, GetDiagnosis metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Tanı yönetimi.

### API Endpoints

#### POST /api/v1/outpatient/encounters
**Ne:** Muayene oluşturma endpoint'i.  
**Neden:** Muayene kaydı.  
**Özelliği:** CreateEncounterCommand alır, EncounterDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene oluşturma.

#### GET /api/v1/outpatient/encounters/{id}
**Ne:** Muayene detay endpoint'i.  
**Neden:** Muayene sorgulama.  
**Özelliği:** EncounterId alır, EncounterDetailDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene sorgulama.

#### PUT /api/v1/outpatient/encounters/{id}
**Ne:** Muayene güncelleme endpoint'i.  
**Neden:** Muayene güncelleme.  
**Özelliği:** UpdateEncounterCommand alır, EncounterDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene güncelleme.

#### PUT /api/v1/outpatient/encounters/{id}/complete
**Ne:** Muayene tamamlama endpoint'i.  
**Neden:** Muayene tamamlama.  
**Özelliği:** EncounterId alır, SuccessResponse döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Muayene tamamlama.

#### POST /api/v1/outpatient/encounters/{id}/diagnoses
**Ne:** Tanı ekleme endpoint'i.  
**Neden:** Tanı ekleme.  
**Özelliği:** AddDiagnosisCommand alır, DiagnosisDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tanı ekleme.

#### POST /api/v1/outpatient/prescriptions
**Ne:** Reçete oluşturma endpoint'i.  
**Neden:** Reçete kaydı.  
**Özelliği:** CreatePrescriptionCommand alır, PrescriptionDTO döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Reçete oluşturma.

#### GET /api/v1/outpatient/prescriptions/{id}
**Ne:** Reçete detay endpoint'i.  
**Neden:** Reçete sorgulama.  
**Özelliği:** PrescriptionId alır, PrescriptionDetailDTO döner.  
**Kim Kullanacak:** Eczane, hasta.  
**Amaç:** Reçete sorgulama.

#### GET /api/v1/outpatient/patients/{patientId}/encounters
**Ne:** Hasta muayeneleri endpoint'i.  
**Neden:** Hasta geçmişi.  
**Özelliği:** PatientId alır, List<EncounterSummaryDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Hasta geçmişi.

#### GET /api/v1/outpatient/diagnoses/search
**Ne:** Tanı arama endpoint'i.  
**Neden:** ICD kodu arama.  
**Özelliği:** SearchTerm alır, List<DiagnosisDTO> döner.  
**Kim Kullanacak:** Poliklinik hekimleri.  
**Amaç:** Tanı arama.

#### POST /api/v1/outpatient/encounters/{id}/vital-signs
**Ne:** Vital bulgu kaydetme endpoint'i.  
**Neden:** Vital bulgu kaydı.  
**Özelliği:** RecordVitalSignsCommand alır, VitalSignDTO döner.  
**Kim Kullanacak:** Poliklinik hemşireleri.  
**Amaç:** Vital bulgu kaydetme.

---

## SPRINT 10: Billing Module

### Sprint Hedefi
Bu sprint, faturalama modülünü kapsamaktadır. Hasta hizmet bedeli, sigorta anlaşması, fatura oluşturma ve ödeme yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. Invoice Entity
**Ne:** Fatura entity'si, hastane faturalarını temsil eder.  
**Neden:** Mali işlemlerin temelini oluşturur.  
**Özelliği:** InvoiceNumber, PatientId, TenantId, InvoiceType (Patient, Insurance, Corporate), InvoiceDate, DueDate, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Currency, Status (Draft, Issued, Paid, PartialPaid, Cancelled, Overdue), PaymentStatus, CreatedBy, CreatedAt, IssuedAt, PaidAt, CancelledAt, CancelledBy özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama, hasta kabul, muhasebe.  
**Amaç:** Fatura yönetimi.

#### 2. InvoiceItem Entity
**Ne:** Fatura kalemi entity'si, her fatura kalemini temsil eder.  
**Neden:** Fatura-işlem ilişkisi için gereklidir.  
**Özelliği:** InvoiceId, ServiceType (Examination, Procedure, Test, Medication, Material), ServiceId, ServiceCode, ServiceName, Quantity, UnitPrice, TotalPrice, DiscountRate, IsCoveredByInsurance, InsuranceCoverageRate, PatientPayRate, Status özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Fatura kalem yönetimi.

#### 3. Payment Entity
**Ne:** Ödeme entity'si, yapılan ödemeleri temsil eder.  
**Neden:** Ödeme takibi için gereklidir.  
**Özelliği:** PaymentId, InvoiceId, PaymentDate, PaymentMethod (Cash, CreditCard, BankTransfer, Cheque), Amount, Currency, ReferenceNumber, TransactionId, Status, ReceivedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama, hasta kabul, muhasebe.  
**Amaç:** Ödeme yönetimi.

#### 4. PriceList Entity
**Ne:** Fiyat listesi entity'si, hizmet fiyatlarını temsil eder.  
**Neden:** Fiyat yönetimi için gereklidir.  
**Özelliği:** PriceListCode, Name, EffectiveDate, ExpiryDate, IsActive, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama, hasta kabul.  
**Amaç:** Fiyat listesi yönetimi.

#### 5. PriceListItem Entity
**Ne:** Fiyat listesi kalemi entity'si, her hizmetin fiyatını temsil eder.  
**Neden:** Hizmet-fiyat ilişkisi için gereklidir.  
**Özelliği:** PriceListId, ServiceType, ServiceCode, ServiceName, UnitPrice, Currency özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Fiyat kalem yönetimi.

#### 6. InsuranceAgreement Entity
**Ne:** Sigorta anlaşması entity'si, sigorta şirketleri ile anlaşmaları temsil eder.  
**Neden:** Sigorta indirim oranları için gereklidir.  
**Özelliği:** AgreementNumber, InsuranceProviderId, AgreementType, EffectiveDate, ExpiryDate, CoverageRate, DiscountRate, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Sigorta anlaşma yönetimi.

#### 7. ServicePrice Entity
**Ne:** Hizmet fiyatı entity'si, her hizmet için özel fiyatları temsil eder.  
**Neden:** Hizmet bazlı fiyatlandırma için gereklidir.  
**Özelliği:** ServiceType, ServiceCode, ServiceName, DefaultPrice, MinPrice, MaxPrice, Currency özelliklerine sahiptir.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Hizmet fiyat yönetimi.

### Domain Events

#### InvoiceCreatedEvent
**Ne:** Fatura oluşturulduğunda tetiklenen event'tir.  
**Neden:** Fatura oluşumunun duyurulması gerekir.  
**Özelliği:** InvoiceId, patientId, totalAmount, invoiceType içerir.  
**Kim Kullanacak:** Notification modülü, Accounting modülü.  
**Amaç:** Fatura oluşum duyurusu.

#### InvoiceIssuedEvent
**Ne:** Fatura düzenlendiğinde tetiklenen event'tir.  
**Neden:** Fatura düzenleme duyurusu için gereklidir.  
**Özelliği:** InvoiceId, issuedAt, issuedBy içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Fatura düzenleme duyurusu.

#### PaymentReceivedEvent
**Ne:** Ödeme alındığında tetiklenen event'tir.  
**Neden:** Ödeme duyurusu için gereklidir.  
**Özelliği:** PaymentId, invoiceId, amount, paymentMethod içerir.  
**Kim Kullanacak:** Accounting modülü.  
**Amaç:** Ödeme duyurusu.

#### InvoicePaidEvent
**Ne:** Fatura tamamen ödendiğinde tetiklenen event'tir.  
**Neden:** Ödeme tamamlama duyurusu için gereklidir.  
**Özelliği:** InvoiceId, paidAt, totalPaid içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Fatura ödeme duyurusu.

#### InvoiceCancelledEvent
**Ne:** Fatura iptal edildiğinde tetiklenen event'tir.  
**Neden:** İptal duyurusu için gereklidir.  
**Özelliği:** InvoiceId, cancelledAt, cancelledBy, reason içerir.  
**Kim Kullanacak:** Accounting modülü.  
**Amaç:** Fatura iptal duyurusu.

### Commands

#### CreateInvoiceCommand
**Ne:** Fatura oluşturma command'i.  
**Neden:** Fatura kaydı için gereklidir.  
**Özelliği:** PatientId, invoiceType, invoiceItems parametreleri alır.  
**Kim Kullanacak:** Faturalama personeli, sistem (otomatik).  
**Amaç:** Fatura oluşturma.

#### IssueInvoiceCommand
**Ne:** Fatura düzenleme command'i.  
**Neden:** Fatura düzenleme için gereklidir.  
**Özelliği:** InvoiceId, issuedBy parametreleri alır.  
**Kim Kullanacak:** Faturalama personeli.  
**Amaç:** Fatura düzenleme.

#### CancelInvoiceCommand
**Ne:** Fatura iptal command'i.  
**Neden:** Fatura iptali için gereklidir.  
**Özelliği:** InvoiceId, reason, cancelledBy parametreleri alır.  
**Kim Kullanacak:** Faturalama yöneticisi.  
**Amaç:** Fatura iptal etme.

#### RecordPaymentCommand
**Ne:** Ödeme kaydetme command'i.  
**Neden:** Ödeme kaydı için gereklidir.  
**Özelliği:** InvoiceId, amount, paymentMethod, referenceNumber parametreleri alır.  
**Kim Kullanacak:** Faturalama personeli, hasta kabul.  
**Amaç:** Ödeme kaydetme.

#### CreateRefundCommand
**Ne:** İade oluşturma command'i.  
**Neden:** İade işlemi için gereklidir.  
**Özelliği:** InvoiceId, refundAmount, reason, refundedBy parametreleri alır.  
**Kim Kullanacak:** Faturalama yöneticisi.  
**Amaç:** İade oluşturma.

#### UpdatePriceListCommand
**Ne:** Fiyat listesi güncelleme command'i.  
**Neden:** Fiyat güncellemesi için gereklidir.  
**Özelliği:** PriceListId, items, effectiveDate parametreleri alır.  
**Kim Kullanacak:** Faturalama yöneticisi.  
**Amaç:** Fiyat listesi güncelleme.

### Queries

#### GetInvoiceByIdQuery
**Ne:** Fatura detay getirme query'si.  
**Neden:** Fatura detay sorgulama için gereklidir.  
**Özelliği:** InvoiceId alır, InvoiceDetailDTO döner.  
**Kim Kullanacak:** Faturalama, hasta.  
**Amaç:** Fatura sorgulama.

#### GetPatientInvoicesQuery
**Ne:** Hasta faturalarını getirme query'si.  
**Neden:** Hasta fatura geçmişi için gereklidir.  
**Özelliği:** PatientId, status, dateRange alır, List<InvoiceDTO> döner.  
**Kim Kullanacak:** Faturalama, hasta.  
**Amaç:** Hasta faturaları.

#### GetInvoicePaymentsQuery
**Ne:** Fatura ödemelerini getirme query'si.  
**Neden:** Fatura ödeme geçmişi için gereklidir.  
**Özelliği:** InvoiceId alır, List<PaymentDTO> döner.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Fatura ödemeleri.

#### GetPendingPaymentsQuery
**Ne:** Bekleyen ödemeleri getirme query'si.  
**Neden:** Tahsilat takibi için gereklidir.  
**Özelliği:** TenantId, dateRange alır, List<PendingPaymentDTO> döner.  
**Kim Kullanacak:** Faturalama, muhasebe.  
**Amaç:** Bekleyen ödemeler.

#### GetServicePriceQuery
**Ne:** Hizmet fiyatı getirme query'si.  
**Neden:** Fiyat sorgulama için gereklidir.  
**Özelliği:** ServiceCode, serviceType alır, ServicePriceDTO döner.  
**Kim Kullanacak:** Faturalama, hasta kabul.  
**Amaç:** Hizmet fiyatı sorgulama.

#### GetOverdueInvoicesQuery
**Ne:** Vadesi geçen faturaları getirme query'si.  
**Neden:** Tahsilat takibi için gereklidir.  
**Özelliği:** TenantId alır, List<OverdueInvoiceDTO> döner.  
**Kim Kullanacak:** Faturalama, muhasebe.  
**Amaç:** Vadesi geçen faturalar.

### Application Services

#### IInvoiceService
**Ne:** Fatura servisi.  
**Neden:** Fatura operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateInvoice, IssueInvoice, CancelInvoice, GetInvoice metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Fatura yönetimi.

#### IPaymentService
**Ne:** Ödeme servisi.  
**Neden:** Ödeme operasyonlarının kapsüllenmesi.  
**Özelliği:** RecordPayment, GetPayments, CreateRefund metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Ödeme yönetimi.

#### IPriceService
**Ne:** Fiyat servisi.  
**Neden:** Fiyat operasyonlarının kapsüllenmesi.  
**Özelliği:** GetServicePrice, UpdatePriceList, GetPriceList metotları.  
**Kim Kullanacak:** API Controllers.  
**Amaç:** Fiyat yönetimi.

### API Endpoints

#### POST /api/v1/billing/invoices
**Ne:** Fatura oluşturma endpoint'i.  
**Neden:** Fatura kaydı.  
**Özelliği:** CreateInvoiceCommand alır, InvoiceDTO döner.  
**Kim Kullanacak:** Faturalama personeli.  
**Amaç:** Fatura oluşturma.

#### GET /api/v1/billing/invoices/{id}
**Ne:** Fatura detay endpoint'i.  
**Neden:** Fatura sorgulama.  
**Özelliği:** InvoiceId alır, InvoiceDetailDTO döner.  
**Kim Kullanacak:** Faturalama, hasta.  
**Amaç:** Fatura sorgulama.

#### PUT /api/v1/billing/invoices/{id}/issue
**Ne:** Fatura düzenleme endpoint'i.  
**Neden:** Fatura düzenleme.  
**Özelliği:** InvoiceId alır, SuccessResponse döner.  
**Kim Kullanacak:** Faturalama personeli.  
**Amaç:** Fatura düzenleme.

#### PUT /api/v1/billing/invoices/{id}/cancel
**Ne:** Fatura iptal endpoint'i.  
**Neden:** Fatura iptal.  
**Özelliği:** CancelInvoiceCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Faturalama yöneticisi.  
**Amaç:** Fatura iptal.

#### POST /api/v1/billing/payments
**Ne:** Ödeme kaydetme endpoint'i.  
**Neden:** Ödeme kaydı.  
**Özelliği:** RecordPaymentCommand alır, PaymentDTO döner.  
**Kim Kullanacak:** Faturalama personeli.  
**Amaç:** Ödeme kaydetme.

#### GET /api/v1/billing/invoices/{id}/payments
**Ne:** Fatura ödemeleri endpoint'i.  
**Neden:** Ödeme geçmişi.  
**Özelliği:** InvoiceId alır, List<PaymentDTO> döner.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Ödeme geçmişi.

#### GET /api/v1/billing/patients/{patientId}/invoices
**Ne:** Hasta faturaları endpoint'i.  
**Neden:** Hasta faturaları.  
**Özelliği:** PatientId alır, List<InvoiceDTO> döner.  
**Kim Kullanacak:** Faturalama, hasta.  
**Amaç:** Hasta faturaları.

#### GET /api/v1/billing/prices/{serviceCode}
**Ne:** Hizmet fiyatı endpoint'i.  
**Neden:** Fiyat sorgulama.  
**Özelliği:** ServiceCode alır, ServicePriceDTO döner.  
**Kim Kullanacak:** Faturalama, hasta kabul.  
**Amaç:** Fiyat sorgulama.

#### PUT /api/v1/billing/pricelists/{id}
**Ne:** Fiyat listesi güncelleme endpoint'i.  
**Neden:** Fiyat güncelleme.  
**Özelliği:** UpdatePriceListCommand alır, PriceListDTO döner.  
**Kim Kullanacak:** Faturalama yöneticisi.  
**Amaç:** Fiyat güncelleme.

#### GET /api/v1/billing/overdue
**Ne:** Vadesi geçen faturalar endpoint'i.  
**Neden:** Tahsilat takibi.  
**Özelliği:** TenantId alır, List<OverdueInvoiceDTO> döner.  
**Kim Kullanacak:** Faturalama.  
**Amaç:** Vadesi geçen faturalar.

---

## SPRINT 11: Stabilizasyon

### Sprint Hedefi
FAZ 1'in stabilizasyon sprint'idir. Tüm modüller entegre edilecek, test edilecek ve production'a hazır hale getirilecektir.

### Yapılacak İşler

#### Entegrasyon Testleri
- Patient, Appointment, Outpatient, Billing modülleri arası entegrasyon testleri
- Hasta akış testleri (kayıt -> randevu -> muayene -> fatura)
- Sigorta entegrasyon testleri

#### Performans Testleri
- Load test: 200 concurrent user
- Stress test: 1000 concurrent user
- Appointment slot query performance test

#### Güvenlik Testleri
- Patient data isolation test
- Permission testleri
- Audit log testleri

#### Dokümantasyon
- API dokümantasyonu güncellemesi
- Kullanıcı kılavuzu güncellemesi
- Operasyon kılavuzu güncellemesi

#### Kullanıcı Eğitimi
- Eğitim materyalleri hazırlama
- Demo ortamı kurulumu

---

## FAZ 1 ÖZETİ

### Tamamlanacak Modüller

| Modül | Sprint | Öncelik | Bağımlılıklar |
|-------|--------|---------|---------------|
| Patient | 6-7 | Critical | Identity, Tenant |
| Appointment | 8 | Critical | Patient, Identity |
| Outpatient | 9 | Critical | Patient, Appointment |
| Billing | 10 | Critical | Patient, Outpatient |

### Kritik Başarı Kriterleri

1. Hasta kayıt ve tıbbi bilgi yönetimi çalışıyor olmalı
2. Randevu oluşturma, onaylama, iptal etme, tamamlama çalışıyor olmalı
3. Poliklinik muayene ve reçete oluşturma çalışıyor olmalı
4. Fatura oluşturma ve ödeme kaydetme çalışıyor olmalı
5. Sigorta indirim oranları doğru uygulanıyor olmalı
6. Hasta akış testleri başarılı olmalı
7. Unit test coverage %80'in üzerinde olmalı
