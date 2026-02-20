# FAZ 4: Muhasebe ve Raporlama Modülleri
## Sprint 22-24 Detaylı Domain Task Listesi

---

## SPRINT 22: Accounting Module

### Sprint Hedefi
Bu sprint, muhasebe modülünü kapsamaktadır. Gelir-gider yönetimi, muhasebe kayıtları, bütçe takibi ve mali raporlama işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. AccountingEntry Entity
**Ne:** Muhasebe kaydı entity'si, tüm muhasebe işlemlerini temsil eder.  
**Neden:** Muhasebe takibinin temel birimidir.  
**Özelliği:** EntryNumber, EntryDate, EntryType (Debit, Credit), AccountCode, AccountName, Amount, Currency, Description, ReferenceType, ReferenceId, TenantId, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Muhasebe, finans.  
**Amaç:** Muhasebe kayıt yönetimi.

#### 2. ChartOfAccounts Entity
**Ne:** Hesap planı entity'si, muhasebe hesap planını temsil eder.  
**Neden:** Standart muhasebe yapısı için gereklidir.  
**Özelliği:** AccountCode, AccountName, AccountType (Asset, Liability, Equity, Revenue, Expense), ParentCode, IsActive, Level, OpeningBalance özelliklerine sahiptir.  
**Kim Kullanacak:** Muhasebe.  
**Amaç:** Hesap planı yönetimi.

#### 3. Budget Entity
**Ne:** Bütçe entity'si, departman ve proje bütçelerini temsil eder.  
**Neden:** Bütçe takibi için gereklidir.  
**Özelliği:** BudgetCode, DepartmentId, BudgetYear, BudgetType, TotalAmount, AllocatedAmount, SpentAmount, RemainingAmount, Status, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Hastane yönetimi, muhasebe.  
**Amaç:** Bütçe yönetimi.

#### 4. Expense Entity
**Ne:** Gider entity'si, hastane giderlerini temsil eder.  
**Neden:** Gider takibi için gereklidir.  
**Özelliği:** ExpenseNumber, ExpenseDate, ExpenseType, Category, Amount, Currency, Description, DepartmentId, ApprovedBy, PaymentStatus, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Muhasebe, tedarik.  
**Amaç:** Gider yönetimi.

#### 5. Revenue Entity
**Ne:** Gelir entity'si, hastane gelirlerini temsil eder.  
**Neden:** Gelir takibi için gereklidir.  
**Özelliği:** RevenueNumber, RevenueDate, RevenueType, Source, Amount, Currency, InvoiceId, DepartmentId, Status, CreatedBy, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Muhasebe, faturalama.  
**Amaç:** Gelir yönetimi.

### Domain Events

#### AccountingEntryCreatedEvent
**Ne:** Muhasebe kaydı oluşturulduğunda tetiklenen event'tir.  
**Neden:** Kayıt duyurusu için gereklidir.  
**Özelliği:** EntryId, accountCode, amount, entryType içerir.  
**Kim Kullanacak:** Audit modülü.  
**Amaç:** Kayıt duyurusu.

#### BudgetExceededAlertEvent
**Ne:** Bütçe aşıldığında tetiklenen event'tir.  
**Neden:** Bütçe uyarısı için gereklidir.  
**Özelliği:** BudgetId, spentAmount, budgetAmount, exceededPercentage içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Bütçe uyarısı.

### Commands

#### CreateAccountingEntryCommand
**Ne:** Muhasebe kaydı oluşturma command'i.  
**Neden:** Kayıt için gereklidir.  
**Özelliği:** EntryDate, entryType, accountCode, amount, description, referenceId, referenceType parametreleri alır.  
**Kim Kullanacak:** Muhasebe personeli.  
**Amaç:** Kayıt oluşturma.

#### CreateBudgetCommand
**Ne:** Bütçe oluşturma command'i.  
**Neden:** Bütçe kaydı için gereklidir.  
**Özelliği:** DepartmentId, budgetYear, totalAmount, budgetType parametreleri alır.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Bütçe oluşturma.

#### RecordExpenseCommand
**Ne:** Gider kaydetme command'i.  
**Neden:** Gider kaydı için gereklidir.  
**Özelliği:** ExpenseDate, expenseType, category, amount, departmentId, description parametreleri alır.  
**Kim Kullanacak:** Muhasebe, tedarik.  
**Amaç:** Gider kaydetme.

#### RecordRevenueCommand
**Ne:** Gelir kaydetme command'i.  
**Neden:** Gelir kaydı için gereklidir.  
**Özelliği:** RevenueDate, revenueType, source, amount, invoiceId parametreleri alır.  
**Kim Kullanacak:** Muhasebe.  
**Amaç:** Gelir kaydetme.

### Queries

#### GetAccountingEntriesQuery
**Ne:** Muhasebe kayıtlarını getirme query'si.  
**Neden:** Kayıt sorgulama için gereklidir.  
**Özelliği:** AccountCode, dateRange alır, PaginatedResult<AccountingEntryDTO> döner.  
**Kim Kullanacak:** Muhasebe.  
**Amaç:** Kayıt sorgulama.

#### GetBudgetStatusQuery
**Ne:** Bütçe durumu getirme query'si.  
**Neden:** Bütçe takibi için gereklidir.  
**Özelliği:** DepartmentId, budgetYear alır, BudgetStatusDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Bütçe durumu.

#### GetExpenseSummaryQuery
**Ne:** Gider özeti getirme query'si.  
**Neden:** Gider raporlaması için gereklidir.  
**Özelliği:** DateRange, category alır, ExpenseSummaryDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Gider özeti.

#### GetRevenueSummaryQuery
**Ne:** Gelir özeti getirme query'si.  
**Neden:** Gelir raporlaması için gereklidir.  
**Özelliği:** DateRange alır, RevenueSummaryDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Gelir özeti.

### API Endpoints

#### POST /api/v1/accounting/entries
**Ne:** Muhasebe kaydı endpoint'i.  
**Neden:** Kayıt oluşturma.  
**Özelliği:** CreateAccountingEntryCommand alır, AccountingEntryDTO döner.  
**Kim Kullanacak:** Muhasebe personeli.  
**Amaç:** Kayıt oluşturma.

#### GET /api/v1/accounting/entries
**Ne:** Muhasebe kayıtları endpoint'i.  
**Neden:** Kayıt sorgulama.  
**Özelliği:** AccountCode, dateRange alır, PaginatedResult döner.  
**Kim Kullanacak:** Muhasebe.  
**Amaç:** Kayıt sorgulama.

#### POST /api/v1/accounting/budgets
**Ne:** Bütçe oluşturma endpoint'i.  
**Neden:** Bütçe kaydı.  
**Özelliği:** CreateBudgetCommand alır, BudgetDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Bütçe oluşturma.

#### GET /api/v1/accounting/budgets/{departmentId}/status
**Ne:** Bütçe durumu endpoint'i.  
**Neden:** Bütçe takibi.  
**Özelliği:** DepartmentId alır, BudgetStatusDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Bütçe durumu.

---

## SPRINT 23-24: Reporting Module

### Sprint Hedefi
Bu sprint, raporlama modülünü kapsamaktadır. İş zekası, yönetim raporları, hasta raporları ve özel rapor tasarımcısı işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. ReportDefinition Entity
**Ne:** Rapor tanımı entity'si, sistemdeki rapor şablonlarını temsil eder.  
**Neden:** Rapor yönetimi için gereklidir.  
**Özelliği:** ReportCode, ReportName, ReportCategory, ReportType, Definition, Parameters, DataSource, IsActive, CreatedBy, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Hastane yönetimi, rapor yöneticileri.  
**Amaç:** Rapor tanım yönetimi.

#### 2. ReportSchedule Entity
**Ne:** Rapor schedule entity'si, otomatik rapor zamanlamalarını temsil eder.  
**Neden:** Scheduled raporlar için gereklidir.  
**Özelliği:** ScheduleId, ReportId, ScheduleType (Daily, Weekly, Monthly), CronExpression, Recipients, Format, Status, LastRunAt, NextRunAt, CreatedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Rapor zamanlama yönetimi.

#### 3. ReportExecution Entity
**Ne:** Rapor çalıştırma entity'si, çalıştırılan rapor kayıtlarını temsil eder.  
**Neden:** Rapor takibi için gereklidir.  
**Özelliği:** ExecutionId, ReportId, Parameters, ExecutedAt, ExecutedBy, Status, ExecutionTime, RowCount, FilePath, ErrorMessage özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Rapor çalıştırma takibi.

### Domain Events

#### ReportGeneratedEvent
**Ne:** Rapor oluşturulduğunda tetiklenen event'tir.  
**Neden:** Rapor duyurusu için gereklidir.  
**Özelliği:** ExecutionId, reportId, executedBy, rowCount içerir.  
**Kim Kullanacak:** Notification modülü.  
**Amaç:** Rapor duyurusu.

### Commands

#### CreateReportDefinitionCommand
**Ne:** Rapor tanımı oluşturma command'i.  
**Neden:** Rapor kaydı için gereklidir.  
**Özelliği:** ReportName, reportCategory, definition, parameters, dataSource parametreleri alır.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Rapor oluşturma.

#### ExecuteReportCommand
**Ne:** Rapor çalıştırma command'i.  
**Neden:** Rapor üretimi için gereklidir.  
**Özelliği:** ReportId, parameters, format parametreleri alır.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Rapor çalıştırma.

#### ScheduleReportCommand
**Ne:** Rapor zamanlama command'i.  
**Neden:** Scheduled rapor için gereklidir.  
**Özelliği:** ReportId, scheduleType, cronExpression, recipients, format parametreleri alır.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Rapor zamanlama.

### Queries

#### GetReportDefinitionsQuery
**Ne:** Rapor tanımlarını getirme query'si.  
**Neden:** Rapor listesi için gereklidir.  
**Özelliği:** Category alır, List<ReportDefinitionDTO> döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Rapor listesi.

#### GetReportExecutionQuery
**Ne:** Rapor çalıştırma detay getirme query'si.  
**Neden:** Çalıştırma takibi için gereklidir.  
**Özelliği:** ExecutionId alır, ReportExecutionDTO döner.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Çalıştırma sorgulama.

### API Endpoints

#### POST /api/v1/reports/definitions
**Ne:** Rapor tanımı oluşturma endpoint'i.  
**Neden:** Rapor kaydı.  
**Özelliği:** CreateReportDefinitionCommand alır, ReportDefinitionDTO döner.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Rapor oluşturma.

#### POST /api/v1/reports/execute
**Ne:** Rapor çalıştırma endpoint'i.  
**Neden:** Rapor üretimi.  
**Özelliği:** ExecuteReportCommand alır, ExecutionResult döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Rapor çalıştırma.

#### POST /api/v1/reports/schedule
**Ne:** Rapor zamanlama endpoint'i.  
**Neden:** Zamanlama kaydı.  
**Özelliği:** ScheduleReportCommand alır, ScheduleDTO döner.  
**Kim Kullanacak:** Rapor yöneticileri.  
**Amaç:** Rapor zamanlama.

---

# FAZ 5: İnsan Kaynakları ve Kalite Modülleri
## Sprint 26-30 Detaylı Domain Task Listesi

---

## SPRINT 26: HR Module - Employee Management

### Sprint Hedefi
Bu sprint, İK modülünün personel yönetimi kısmını kapsamaktadır. Personel kayıt, bordro, devam takibi ve izin yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. Employee Entity
**Ne:** Personel entity'si, hastane çalışanlarını temsil eder.  
**Neden:** Personel yönetiminin temel birimidir.  
**Özelliği:** EmployeeNumber, TurkishId, FirstName, LastName, Gender, DateOfBirth, ContactInfo, DepartmentId, Position, EmploymentType (FullTime, PartTime, Contract), StartDate, EndDate, Status, Salary, IBAN, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** İK, departman yöneticileri.  
**Amaç:** Personel yönetimi.

#### 2. Attendance Entity
**Ne:** Devam entity'si, personel devam takibini temsil eder.  
**Neden:** Devam takibi için gereklidir.  
**Özelliği:** EmployeeId, AttendanceDate, CheckInTime, CheckOutTime, WorkHours, OvertimeHours, Status (Present, Absent, Late, Leave), Notes, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** İK, departman yöneticileri.  
**Amaç:** Devam takibi.

#### 3. LeaveRequest Entity
**Ne:** İzin talebi entity'si, personel izin taleplerini temsil eder.  
**Neden:** İzin takibi için gereklidir.  
**Özelliği:** RequestNumber, EmployeeId, LeaveType (Annual, Sick, Unpaid, Maternity, Paternity), StartDate, EndDate, TotalDays, Reason, Status (Pending, Approved, Rejected), ApprovedBy, ApprovedDate, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** İK, departman yöneticileri.  
**Amaç:** İzin yönetimi.

### Commands

#### CreateEmployeeCommand
**Ne:** Personel oluşturma command'i.  
**Neden:** Personel kaydı için gereklidir.  
**Özelliği:** TurkishId, firstName, lastName, departmentId, position, startDate, salary parametreleri alır.  
**Kim Kullanacak:** İK personeli.  
**Amaç:** Personel oluşturma.

#### RecordAttendanceCommand
**Ne:** Devam kaydetme command'i.  
**Neden:** Devam kaydı için gereklidir.  
**Özelliği:** EmployeeId, attendanceDate, checkInTime, checkOutTime parametreleri alır.  
**Kim Kullanacak:** İK, personel (self-service).  
**Amaç:** Devam kaydetme.

#### SubmitLeaveRequestCommand
**Ne:** İzin talebi command'i.  
**Neden:** İzin talebi için gereklidir.  
**Özelliği:** EmployeeId, leaveType, startDate, endDate, reason parametreleri alır.  
**Kim Kullanacak:** Personel.  
**Amaç:** İzin talebi.

#### ApproveLeaveRequestCommand
**Ne:** İzin onaylama command'i.  
**Neden:** İzin onayı için gereklidir.  
**Özelliği:** RequestId, approvedBy, notes parametreleri alır.  
**Kim Kullanacak:** Departman yöneticileri.  
**Amaç:** İzin onaylama.

### Queries

#### GetEmployeesQuery
**Ne:** Personel listesi query'si.  
**Neden:** Personel sorgulama için gereklidir.  
**Özelliği:** DepartmentId, status alır, List<EmployeeDTO> döner.  
**Kim Kullanacak:** İK, yöneticiler.  
**Amaç:** Personel listesi.

#### GetAttendanceRecordsQuery
**Ne:** Devam kayıtları query'si.  
**Neden:** Devam takibi için gereklidir.  
**Özelliği:** EmployeeId, dateRange alır, List<AttendanceDTO> döner.  
**Kim Kullanacak:** İK, yöneticiler.  
**Amaç:** Devam kayıtları.

#### GetLeaveBalanceQuery
**Ne:** İzin bakiyesi query'si.  
**Neden:** İzin takibi için gereklidir.  
**Özelliği:** EmployeeId alır, LeaveBalanceDTO döner.  
**Kim Kullanacak:** Personel, İK.  
**Amaç:** İzin bakiyesi.

### API Endpoints

#### POST /api/v1/hr/employees
**Ne:** Personel oluşturma endpoint'i.  
**Neden:** Personel kaydı.  
**Özelliği:** CreateEmployeeCommand alır, EmployeeDTO döner.  
**Kim Kullanacak:** İK personeli.  
**Amaç:** Personel oluşturma.

#### GET /api/v1/hr/employees
**Ne:** Personel listesi endpoint'i.  
**Neden:** Personel sorgulama.  
**Özelliği:** DepartmentId alır, List<EmployeeDTO> döner.  
**Kim Kullanacak:** İK.  
**Amaç:** Personel listesi.

#### POST /api/v1/hr/attendance
**Ne:** Devam kaydetme endpoint'i.  
**Neden:** Devam kaydı.  
**Özelliği:** RecordAttendanceCommand alır, AttendanceDTO döner.  
**Kim Kullanacak:** İK.  
**Amaç:** Devam kaydetme.

#### POST /api/v1/hr/leave-requests
**Ne:** İzin talebi endpoint'i.  
**Neden:** İzin talebi.  
**Özelliği:** SubmitLeaveRequestCommand alır, LeaveRequestDTO döner.  
**Kim Kullanacak:** Personel.  
**Amaç:** İzin talebi.

#### PUT /api/v1/hr/leave-requests/{id}/approve
**Ne:** İzin onaylama endpoint'i.  
**Neden:** İzin onayı.  
**Özelliği:** RequestId alır, SuccessResponse döner.  
**Kim Kullanacak:** Yöneticiler.  
**Amaç:** İzin onaylama.

---

## SPRINT 27-30: Quality, Document & Notification Modules

### Sprint Hedefi
Bu sprint, kalite, doküman ve bildirim modüllerini kapsamaktadır. Kalite belgelendirme, doküman yönetimi ve sistem bildirimleri işlevleri geliştirilecektir.

### Kalite Modülü Domain Entity'leri

#### 1. QualityIndicator Entity
**Ne:** Kalite göstergesi entity'si, kalite metriklerini temsil eder.  
**Neden:** Kalite takibi için gereklidir.  
**Özelliği:** IndicatorCode, IndicatorName, Category, TargetValue, MeasurementUnit, Frequency, IsActive, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Kalite yöneticileri.  
**Amaç:** Kalite göstergesi yönetimi.

#### 2. QualityRecord Entity
**Ne:** Kalite kaydı entity'si, kalite ölçümlerini temsil eder.  
**Neden:** Kalite veri takibi için gereklidir.  
**Özelliği:** RecordId, IndicatorId, RecordDate, ActualValue, DeviationFromTarget, Status, RecordedBy, Notes özelliklerine sahiptir.  
**Kim Kullanacak:** Kalite yöneticileri.  
**Amaç:** Kalite kayıt yönetimi.

### Doküman Modülü Domain Entity'leri

#### 1. Document Entity
**Ne:** Doküman entity'si, sistem dokümanlarını temsil eder.  
**Neden:** Doküman yönetimi için gereklidir.  
**Özelliği:** DocumentNumber, Title, DocumentType, Category, FilePath, FileSize, Version, Status, EffectiveDate, ExpiryDate, CreatedBy, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Tüm kullanıcılar.  
**Amaç:** Doküman yönetimi.

#### 2. DocumentApproval Entity
**Ne:** Doküman onay entity'si, doküman onay süreçlerini temsil eder.  
**Neden:** Onay takibi için gereklidir.  
**Özelliği:** DocumentId, ApproverId, ApprovalStatus, ApprovalDate, Comments özelliklerine sahiptir.  
**Kim Kullanacak:** Yöneticiler.  
**Amaç:** Onay yönetimi.

### Bildirim Modülü Domain Entity'leri

#### 1. NotificationTemplate Entity
**Ne:** Bildirim şablonu entity'si, bildirim formatlarını temsil eder.  
**Neden:** Bildirim yönetimi için gereklidir.  
**Özelliği:** TemplateCode, TemplateName, Channel (Email, SMS, Push, InApp), Subject, Body, Variables, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Bildirim yöneticileri.  
**Amaç:** Şablon yönetimi.

#### 2. Notification Entity
**Ne:** Bildirim entity'si, gönderilen bildirimleri temsil eder.  
**Neden:** Bildirim takibi için gereklidir.  
**Özelliği:** NotificationId, UserId, TemplateId, Channel, Subject, Body, Status (Pending, Sent, Failed), SentAt, ReadAt, ErrorMessage, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem, kullanıcılar.  
**Amaç:** Bildirim yönetimi.

### Commands

#### CreateDocumentCommand
**Ne:** Doküman oluşturma command'i.  
**Neden:** Doküman kaydı için gereklidir.  
**Özelliği:** Title, documentType, category, filePath, effectiveDate parametreleri alır.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Doküman oluşturma.

#### CreateNotificationCommand
**Ne:** Bildirim oluşturma command'i.  
**Neden:** Bildirim gönderimi için gereklidir.  
**Özelliği:** UserId, templateId, channel, variables parametreleri alır.  
**Kim Kullanacak:** Sistem, yetkili kullanıcılar.  
**Amaç:** Bildirim oluşturma.

#### RecordQualityDataCommand
**Ne:** Kalite verisi kaydetme command'i.  
**Neden:** Kalite ölçümü için gereklidir.  
**Özelliği:** IndicatorId, recordDate, actualValue, notes parametreleri alır.  
**Kim Kullanacak:** Kalite yöneticileri.  
**Amaç:** Kalite verisi kaydetme.

### Queries

#### GetDocumentsQuery
**Ne:** Doküman sorgulama query'si.  
**Neden:** Doküman arama için gereklidir.  
**Özelliği:** Category, documentType alır, List<DocumentDTO> döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Doküman sorgulama.

#### GetNotificationsQuery
**Ne:** Bildirim sorgulama query'si.  
**Neden:** Bildirim takibi için gereklidir.  
**Özelliği:** UserId, status alır, List<NotificationDTO> döner.  
**Kim Kullanacak:** Kullanıcılar.  
**Amaç:** Bildirim sorgulama.

#### GetQualityDashboardQuery
**Ne:** Kalite dashboard query'si.  
**Neden:** Kalite görünümü için gereklidir.  
**Özelliği:** DateRange alır, QualityDashboardDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Kalite dashboard.

### API Endpoints

#### POST /api/v1/documents
**Ne:** Doküman oluşturma endpoint'i.  
**Neden:** Doküman kaydı.  
**Özelliği:** CreateDocumentCommand alır, DocumentDTO döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Doküman oluşturma.

#### GET /api/v1/documents
**Ne:** Doküman listesi endpoint'i.  
**Neden:** Doküman sorgulama.  
**Özelliği:** Category alır, List<DocumentDTO> döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Doküman listesi.

#### POST /api/v1/notifications
**Ne:** Bildirim oluşturma endpoint'i.  
**Neden:** Bildirim gönderimi.  
**Özelliği:** CreateNotificationCommand alır, NotificationDTO döner.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Bildirim oluşturma.

#### GET /api/v1/notifications/user/{userId}
**Ne:** Kullanıcı bildirimleri endpoint'i.  
**Neden:** Bildirim listesi.  
**Özelliği:** UserId alır, List<NotificationDTO> döner.  
**Kim Kullanacak:** Kullanıcılar.  
**Amaç:** Bildirim listesi.

#### POST /api/v1/quality/records
**Ne:** Kalite verisi kaydetme endpoint'i.  
**Neden:** Kalite ölçümü.  
**Özelliği:** RecordQualityDataCommand alır, QualityRecordDTO döner.  
**Kim Kullanacak:** Kalite yöneticileri.  
**Amaç:** Kalite verisi kaydetme.

#### GET /api/v1/quality/dashboard
**Ne:** Kalite dashboard endpoint'i.  
**Neden:** Kalite görünümü.  
**Özelliği:** DateRange alır, QualityDashboardDTO döner.  
**Kim Kullanacak:** Hastane yönetimi.  
**Amaç:** Kalite dashboard.

---

# FAZ 6: Entegrasyon ve İzleme Modülleri
## Sprint 31-36 Detaylı Domain Task Listesi

---

## SPRINT 31-32: Integration Module

### Sprint Hedefi
Bu sprint, entegrasyon modülünü kapsamaktadır. Harici sistem entegrasyonları, HL7/FHIR desteği, KPS entegrasyonu ve API yönetimi işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. IntegrationEndpoint Entity
**Ne:** Entegrasyon endpoint entity'si, harici sistem bağlantılarını temsil eder.  
**Neden:** Entegrasyon yönetimi için gereklidir.  
**Özelliği:** EndpointCode, SystemName, EndpointType (HL7, FHIR, REST, SOAP), Url, AuthenticationType, Credentials, IsActive, LastSyncAt, Status, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Entegrasyon endpoint yönetimi.

#### 2. IntegrationLog Entity
**Ne:** Entegrasyon log entity'si, entegrasyon işlem kayıtlarını temsil eder.  
**Neden:** Entegrasyon takibi için gereklidir.  
**Özelliği:** LogId, EndpointId, Direction (In, Out), MessageType, Payload, Status, ErrorMessage, ProcessingTime, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Entegrasyon log yönetimi.

#### 3. MessageMapping Entity
**Ne:** Mesaj mapping entity'si, veri dönüşüm kurallarını temsil eder.  
**Neden:** Veri eşleştirme için gereklidir.  
**Özelliği:** MappingCode, SourceFormat, TargetFormat, MappingRules, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Entegrasyon yöneticileri.  
**Amaç:** Mapping yönetimi.

### Commands

#### CreateIntegrationEndpointCommand
**Ne:** Entegrasyon endpoint oluşturma command'i.  
**Neden:** Endpoint kaydı için gereklidir.  
**Özelliği:** SystemName, endpointType, url, authenticationType parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Endpoint oluşturma.

#### SendIntegrationMessageCommand
**Ne:** Entegrasyon mesajı gönderme command'i.  
**Neden:** Harici sistemle veri paylaşımı için gereklidir.  
**Özelliği:** EndpointId, messageType, payload parametreleri alır.  
**Kim Kullanacak:** Modüller.  
**Amaç:** Mesaj gönderme.

### Queries

#### GetIntegrationLogsQuery
**Ne:** Entegrasyon logları query'si.  
**Neden:** Entegrasyon takibi için gereklidir.  
**Özelliği:** EndpointId, dateRange, status alır, PaginatedResult<IntegrationLogDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Log sorgulama.

#### GetEndpointStatusQuery
**Ne:** Endpoint durumu query'si.  
**Neden:** Bağlantı durumu için gereklidir.  
**Özelliği:** EndpointId alır, EndpointStatusDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Durum sorgulama.

### API Endpoints

#### POST /api/v1/integration/endpoints
**Ne:** Endpoint oluşturma endpoint'i.  
**Neden:** Endpoint kaydı.  
**Özelliği:** CreateIntegrationEndpointCommand alır, EndpointDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Endpoint oluşturma.

#### GET /api/v1/integration/endpoints
**Ne:** Endpoint listesi endpoint'i.  
**Neden:** Endpoint listesi.  
**Özelliği:** Boş parametre alır, List<EndpointDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Endpoint listesi.

#### GET /api/v1/integration/logs
**Ne:** Entegrasyon logları endpoint'i.  
**Neden:** Log sorgulama.  
**Özelliği:** EndpointId, status alır, PaginatedResult döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Log sorgulama.

---

## SPRINT 33-34: Monitoring Module

### Sprint Hedefi
Bu sprint, izleme modülünü kapsamaktadır. Sistem sağlık izleme, performans metrikleri, alert yönetimi ve dashboard işlevleri geliştirilecektir.

### Domain Entity'leri

#### 1. HealthCheck Entity
**Ne:** Sağlık kontrolü entity'si, sistem bileşen sağlık durumlarını temsil eder.  
**Neden:** Sağlık takibi için gereklidir.  
**Özelliği:** ComponentName, ComponentType, Status, LastCheckAt, ResponseTime, ErrorMessage özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Sağlık kontrolü.

#### 2. Metric Entity
**Ne:** Metrik entity'si, sistem metriklerini temsil eder.  
**Neden:** Performans takibi için gereklidir.  
**Özelliği:** MetricName, MetricType, Value, Unit, Tags, RecordedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Metrik yönetimi.

#### 3. Alert Entity
**Ne:** Uyarı entity'si, sistem uyarılarını temsil eder.  
**Neden:** Alert takibi için gereklidir.  
**Özelliği:** AlertId, AlertType, Severity, Component, Message, Status, TriggeredAt, AcknowledgedAt, AcknowledgedBy, ResolvedAt, ResolvedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem, operatörler.  
**Amaç:** Alert yönetimi.

### Commands

#### CreateAlertCommand
**Ne:** Alert oluşturma command'i.  
**Neden:** Alert kaydı için gereklidir.  
**Özelliği:** AlertType, severity, component, message parametreleri alır.  
**Kim Kullanacak:** Sistem.  
**Amaç:** Alert oluşturma.

#### AcknowledgeAlertCommand
**Ne:** Alert onaylama command'i.  
**Neden:** Alert onayı için gereklidir.  
**Özelliği:** AlertId, acknowledgedBy parametreleri alır.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Alert onaylama.

#### ResolveAlertCommand
**Ne:** Alert çözümleme command'i.  
**Neden:** Alert çözümü için gereklidir.  
**Özelliği:** AlertId, resolvedBy, resolutionNotes parametreleri alır.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Alert çözümleme.

### Queries

#### GetSystemHealthQuery
**Ne:** Sistem sağlık durumu query'si.  
**Neden:** Sağlık takibi için gereklidir.  
**Özelliği:** Boş parametre alır, SystemHealthDTO döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Sağlık durumu.

#### GetActiveAlertsQuery
**Ne:** Aktif uyarıları query'si.  
**Neden:** Alert takibi için gereklidir.  
**Özelliği:** Severity, component alır, List<AlertDTO> döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Aktif alertler.

#### GetMetricsQuery
**Ne:** Metrik sorgulama query'si.  
**Neden:** Performans takibi için gereklidir.  
**Özelliği:** MetricName, dateRange alır, List<MetricDTO> döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Metrik sorgulama.

### API Endpoints

#### GET /api/v1/monitoring/health
**Ne:** Sistem sağlık endpoint'i.  
**Neden:** Sağlık durumu.  
**Özelliği:** Boş parametre alır, SystemHealthDTO döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Sağlık durumu.

#### GET /api/v1/monitoring/alerts
**Ne:** Aktif uyarılar endpoint'i.  
**Neden:** Alert listesi.  
**Özelliği:** Severity alır, List<AlertDTO> döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Alert listesi.

#### PUT /api/v1/monitoring/alerts/{id}/acknowledge
**Ne:** Alert onaylama endpoint'i.  
**Neden:** Alert onayı.  
**Özelliği:** AlertId alır, SuccessResponse döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Alert onaylama.

#### GET /api/v1/monitoring/metrics
**Ne:** Metrikler endpoint'i.  
**Neden:** Performans takibi.  
**Özelliği:** MetricName alır, List<MetricDTO> döner.  
**Kim Kullanacak:** Operatörler.  
**Amaç:** Metrik sorgulama.

---

## SPRINT 35-36: Data Warehouse & API Gateway

### Sprint Hedefi
Bu sprint, veri ambarı ve API gateway modüllerini kapsamaktadır. Veri ambarı ETL, API gateway yönetimi ve multi-hospital orchestration işlevleri geliştirilecektir.

### Data Warehouse Domain Entity'leri

#### 1. DataWarehouseTable Entity
**Ne:** Veri ambarı tablosu entity'si, analitik tabloları temsil eder.  
**Neden:** Analitik yapı için gereklidir.  
**Özelliği:** TableName, TableType (Fact, Dimension), SourceSystem, RefreshSchedule, IsActive, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** Tablo yönetimi.

#### 2. ETLJob Entity
**Ne:** ETL işi entity'si, veri aktarım işlerini temsil eder.  
**Neden:** ETL takibi için gereklidir.  
**Özelliği:** JobId, JobName, SourceTable, TargetTable, JobType (Full, Incremental), Schedule, Status, LastRunAt, NextRunAt, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** ETL yönetimi.

### API Gateway Domain Entity'leri

#### 1. APIRoute Entity
**Ne:** API route entity'si, gateway rotalarını temsil eder.  
**Neden:** Route yönetimi için gereklidir.  
**Özelliği:** RouteId, Path, Method, BackendService, RateLimit, Timeout, IsActive, CreatedAt, UpdatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Route yönetimi.

#### 2. APIKey Entity
**Ne:** API anahtarı entity'si, harici erişim anahtarlarını temsil eder.  
**Neden:** API güvenliği için gereklidir.  
**Özelliği:** KeyId, KeyValue, ClientName, ExpiryDate, RateLimit, IsActive, CreatedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Harici sistemler.  
**Amaç:** API anahtar yönetimi.

### Multi-Hospital Domain Entity'leri

#### 1. Hospital Entity
**Ne:** Hastane entity'si, grup hastane yapısındaki hastaneleri temsil eder.  
**Neden:** Multi-hospital yönetimi için gereklidir.  
**Özelliği:** HospitalCode, HospitalName, GroupId, Address, Phone, Type, Status, IsActive özelliklerine sahiptir.  
**Kim Kullanacak:** Grup yönetimi.  
**Amaç:** Hastane yönetimi.

#### 2. GroupConfiguration Entity
**Ne:** Grup konfigürasyon entity'si, grup geneli ayarları temsil eder.  
**Neden:** Grup yönetimi için gereklidir.  
**Özelliği:** GroupId, ConfigKey, ConfigValue, IsInherited, HospitalId özelliklerine sahiptir.  
**Kim Kullanacak:** Grup yöneticileri.  
**Amaç:** Grup konfigürasyon yönetimi.

### Commands

#### CreateETLJobCommand
**Ne:** ETL işi oluşturma command'i.  
**Neden:** ETL kaydı için gereklidir.  
**Özelliği:** JobName, sourceTable, targetTable, jobType, schedule parametreleri alır.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** ETL işi oluşturma.

#### CreateAPIRouteCommand
**Ne:** API route oluşturma command'i.  
**Neden:** Route kaydı için gereklidir.  
**Özelliği:** Path, method, backendService, rateLimit parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Route oluşturma.

#### CreateAPIKeyCommand
**Ne:** API anahtarı oluşturma command'i.  
**Neden:** Anahtar kaydı için gereklidir.  
**Özelliği:** ClientName, expiryDate, rateLimit parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** API anahtarı oluşturma.

### Queries

#### GetETLJobStatusQuery
**Ne:** ETL işi durumu query'si.  
**Neden:** ETL takibi için gereklidir.  
**Özelliği:** JobId alır, ETLJobStatusDTO döner.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** ETL durumu.

#### GetHospitalGroupQuery
**Ne:** Hastane grubu sorgulama query'si.  
**Neden:** Grup takibi için gereklidir.  
**Özelliği:** GroupId alır, GroupDTO döner.  
**Kim Kullanacak:** Grup yöneticileri.  
**Amaç:** Grup sorgulama.

### API Endpoints

#### POST /api/v1/etl/jobs
**Ne:** ETL işi oluşturma endpoint'i.  
**Neden:** ETL kaydı.  
**Özelliği:** CreateETLJobCommand alır, ETLJobDTO döner.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** ETL işi oluşturma.

#### GET /api/v1/etl/jobs/{id}/status
**Ne:** ETL işi durumu endpoint'i.  
**Neden:** ETL takibi.  
**Özelliği:** JobId alır, ETLJobStatusDTO döner.  
**Kim Kullanacak:** Veri ambarı yöneticileri.  
**Amaç:** ETL durumu.

#### POST /api/v1/gateway/routes
**Ne:** API route oluşturma endpoint'i.  
**Neden:** Route kaydı.  
**Özelliği:** CreateAPIRouteCommand alır, APIRouteDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Route oluşturma.

#### POST /api/v1/gateway/keys
**Ne:** API anahtarı oluşturma endpoint'i.  
**Neden:** Anahtar kaydı.  
**Özelliği:** CreateAPIKeyCommand alır, APIKeyDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** API anahtarı oluşturma.

#### GET /api/v1/hospitals/group/{groupId}
**Ne:** Hastane grubu endpoint'i.  
**Neden:** Grup takibi.  
**Özelliği:** GroupId alır, GroupDTO döner.  
**Kim Kullanacak:** Grup yöneticileri.  
**Amaç:** Grup sorgulama.

---

## FAZ 4-5-6 ÖZETİ

### Tamamlanacak Modüller

| Modül | Sprint | Öncelik | Bağımlılıklar |
|-------|--------|---------|---------------|
| Accounting | 22 | Critical | Billing, Inventory |
| Reporting | 23-24 | High | Tüm modüller |
| HR | 26 | High | Identity |
| Quality | 27 | High | - |
| Document | 28 | Medium | - |
| Notification | 29 | Medium | Identity |
| Integration | 31-32 | High | Tüm modüller |
| Monitoring | 33-34 | High | - |
| DataWarehouse | 35 | High | Tüm modüller |
| APIGateway | 36 | High | - |

### Kritik Başarı Kriterleri

1. Muhasebe kayıt ve bütçe takibi çalışıyor olmalı
2. Raporlama sistemi çalışıyor olmalı
3. Personel ve izin yönetimi çalışıyor olmalı
4. Kalite göstergeleri takip ediliyor olmalı
5. Entegrasyon endpoint'leri çalışıyor olmalı
6. Monitoring ve alert sistemi çalışıyor olmalı
7. Data warehouse yapısı oluşturulmuş olmalı
8. API gateway çalışıyor olmalı
