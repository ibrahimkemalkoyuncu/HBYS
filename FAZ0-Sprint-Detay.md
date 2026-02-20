# FAZ 0: Temel Altyapı Modülleri
## Sprint 1-4 Detaylı Domain Task Listesi

---

## SPRINT 1: Identity Module - User Management

### Sprint Hedefi
Bu sprint, HBYS sisteminin güvenlik temelini oluşturacak Identity modülünün ilk kısmını kapsamaktadır. Kullanıcı yönetimi, rol yetkilendirme ve temel kimlik doğrulama altyapısı bu sprint'te geliştirilecektir.

### Domain Entity'leri

#### 1. User Entity
**Ne:** Kullanıcı entity'si, sisteme erişen tüm bireyleri temsil eder.  
**Neden:** Sistemin temel güvenlik aktörlerinden biri olup, kimlik doğrulama ve yetkilendirme için zorunludur.  
**Özelliği:** Unique identifier, credentials, profile, tenant association özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, modül yöneticileri, tüm son kullanıcılar.  
**Amaç:** Kullanıcıların sisteme güvenli erişimini sağlamak ve kimliklerini yönetmek.

#### 2. Role Entity
**Ne:** Rol entity'si, kullanıcıların yetkilerini gruplandıran mantıksal yapıdır.  
**Neden:** Fine-grained yetkilendirme yerine gruplama yaparak yönetim kolaylığı sağlar.  
**Özelliği:** Name, description, permissions, isSystemRole özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, departman yöneticileri.  
**Amaç:** Kullanıcıların sistem içindeki yetkilerini yönetmek ve erişim kontrolü sağlamak.

#### 3. Permission Entity
**Ne:** İzin entity'si, belirli bir işlemi gerçekleştirme yetkisini temsil eder.  
**Neden:** En düşük seviyeli yetkilendirme birimidir ve RBAC'in temelini oluşturur.  
**Özelliği:** Name, module, description, isActive özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Sistem kaynaklarına erişim yetkilerini tanımlamak ve kontrol etmek.

#### 4. UserRole Entity
**Ne:** UserRole, kullanıcı ile rol arasındaki many-to-many ilişkiyi yönetir.  
**Neden:** Bir kullanıcının birden fazla role sahip olabilmesini sağlar.  
**Özelliği:** UserId, RoleId, assignedAt, assignedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, departman yöneticileri.  
**Amaç:** Kullanıcıların rollere atanmasını ve yönetilmesini sağlamak.

#### 5. RolePermission Entity
**Ne:** RolePermission, rol ile izin arasındaki many-to-many ilişkiyi yönetir.  
**Neden:** Bir rolun birden fazla izne sahip olabilmesini sağlar.  
**Özelliği:** RoleId, PermissionId, grantedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Rollerin izin kümesini tanımlamak ve yönetmek.

### Domain Events

#### UserCreatedEvent
**Ne:** Kullanıcı oluşturulduğunda tetiklenen event'tir.  
**Neden:** Diğer modüllerin kullanıcı oluşumuna tepki vermesini sağlar.  
**Özelliği:** UserId, tenantId, email, createdAt içerir.  
**Kim Kullanacak:** Audit modülü, Notification modülü, Tenant modülü.  
**Amaç:** Kullanıcı oluşumunun sistem genelinde duyurulması ve gerekli işlemlerin tetiklenmesi.

#### RoleAssignedEvent
**Ne:** Kullanıcıya rol atandığında tetiklenen event'tir.  
**Neden:** Yetkilendirme değişikliklerinin takip edilmesini sağlar.  
**Özelliği:** UserId, RoleId, assignedBy, assignedAt içerir.  
**Kim Kullanacak:** Audit modülü, Notification modülü.  
**Amaç:** Rol atama işlemlerinin loglanması ve bildirimlerin gönderilmesi.

### Commands

#### CreateUserCommand
**Ne:** Yeni kullanıcı oluşturmak için kullanılan command'dır.  
**Neden:** Kullanıcı kaydı işleminin CQRS pattern ile yapılmasını sağlar.  
**Özelliği:** Email, password, firstName, lastName, tenantId, roleIds parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri, İK personeli (bazı senaryolarda).  
**Amaç:** Sisteme yeni kullanıcı eklemek.

#### UpdateUserCommand
**Ne:** Mevcut kullanıcı bilgilerini güncellemek için kullanılan command'dır.  
**Neden:** Kullanıcı profil güncellemelerinin yönetilmesini sağlar.  
**Özelliği:** UserId, firstName, lastName, phoneNumber parametreleri alır.  
**Kim Kullanacak:** Kullanıcı kendi profilini güncellerken, yöneticiler başka kullanıcıları güncellerken.  
**Amaç:** Kullanıcı bilgilerinin güncellenmesi.

#### AssignRoleCommand
**Ne:** Kullanıcıya rol atamak için kullanılan command'dır.  
**Neden:** Yetkilendirme işlemlerinin CQRS pattern ile yapılmasını sağlar.  
**Özelliği:** UserId, roleId, assignedBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri, departman yöneticileri.  
**Amaç:** Kullanıcılara rollerin atanması.

#### ChangePasswordCommand
**Ne:** Kullanıcı şifresini değiştirmek için kullanılan command'dır.  
**Neden:** Güvenlik politikalarının uygulanmasını sağlar.  
**Özelliği:** UserId, currentPassword, newPassword parametreleri alır.  
**Kim Kullanacak:** Tüm kullanıcılar (kendi şifreleri için), yöneticiler (reset için).  
**Amaç:** Şifre değişikliği ve resetleme işlemlerinin yönetilmesi.

#### ResetPasswordCommand
**Ne:** Admin tarafından kullanıcı şifresini resetlemek için kullanılan command'dır.  
**Neden:** Unutulan şifre senaryolarının yönetilmesini sağlar.  
**Özelliği:** UserId, newPassword, resetBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Kullanıcı şifresinin admin tarafından sıfırlanması.

### Queries

#### GetUserByIdQuery
**Ne:** ID'ye göre kullanıcı bilgilerini getiren query'dir.  
**Neden:** Kullanıcı detay sayfası ve düzenleme için gereklidir.  
**Özelliği:** UserId parametresi alır, UserDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri, kullanıcı kendi profilini görüntülerken.  
**Amaç:** Kullanıcı bilgilerinin görüntülenmesi.

#### GetUsersByTenantQuery
**Ne:** Tenant'a ait tüm kullanıcıları listeleyen query'dir.  
**Neden:** Kullanıcı yönetim ekranı için gereklidir.  
**Özelliği:** TenantId, pagination parameters alır, PaginatedResult<UserListDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri, departman yöneticileri.  
**Amaç:** Tenant'daki kullanıcıların listelenmesi.

#### GetUserRolesQuery
**Ne:** Kullanıcının sahip olduğu rolleri getiren query'dir.  
**Neden:** Yetkilendirme kontrolü ve kullanıcı detay sayfası için gereklidir.  
**Özelliği:** UserId parametresi alır, List<RoleDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Kullanıcı rollerinin görüntülenmesi.

#### GetAllRolesQuery
**Ne:** Tüm sistem rollerini listeleyen query'dir.  
**Neden:** Rol atama ekranı için gereklidir.  
**Özelliği:** TenantId parametresi alır, List<RoleDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Rollerin listelenmesi.

### Application Services

#### IUserService
**Ne:** Kullanıcı yönetimi işlemlerini kapsülleyen interface'dir.  
**Neden:** Business logic'in controller'dan ayrılmasını sağlar.  
**Özelliği:** CreateUser, UpdateUser, DeleteUser, GetUserById, GetUsersByTenant metotlarını içerir.  
**Kim Kullanacak:** API Controllers, MediatR Handlers.  
**Amaç:** Kullanıcı işlemlerinin merkezi yönetimi.

#### IRoleService
**Ne:** Rol yönetimi işlemlerini kapsülleyen interface'dir.  
**Neden:** Yetkilendirme logic'inin ayrıştırılmasını sağlar.  
**Özelliği:** CreateRole, UpdateRole, AssignRole, RemoveRole, GetRoles metotlarını içerir.  
**Kim Kullanacak:** API Controllers, MediatR Handlers.  
**Amaç:** Rol işlemlerinin merkezi yönetimi.

#### IAuthenticationService
**Ne:** Kimlik doğrulama işlemlerini kapsülleyen interface'dir.  
**Neden:** Authentication logic'in modülerliğini sağlar.  
**Özelliği:** Authenticate, ValidateToken, RefreshToken, Logout metotlarını içerir.  
**Kim Kullanacak:** Auth Controller, JWT Middleware.  
**Amaç:** Kimlik doğrulama işlemlerinin yönetimi.

### API Endpoints

#### POST /api/v1/identity/users
**Ne:** Yeni kullanıcı oluşturma endpoint'i.  
**Neden:** Kullanıcı kayıt işlemi için HTTP endpoint gereklidir.  
**Özelliği:** CreateUserCommand alır, CreatedUserDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** API üzerinden kullanıcı oluşturma.

#### GET /api/v1/identity/users/{id}
**Ne:** Kullanıcı detay getirme endpoint'i.  
**Neden:** Kullanıcı bilgisi sorgulama için HTTP endpoint gereklidir.  
**Özelliği:** UserId alır, UserDTO döner.  
**Kim Kullanacak:** Yetkili tüm kullanıcılar.  
**Amaç:** API üzerinden kullanıcı sorgulama.

#### PUT /api/v1/identity/users/{id}
**Ne:** Kullanıcı güncelleme endpoint'i.  
**Neden:** Kullanıcı bilgi güncelleme için HTTP endpoint gereklidir.  
**Özelliği:** UpdateUserCommand alır, UpdatedUserDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri, kullanıcı kendisi.  
**Amaç:** API üzerinden kullanıcı güncelleme.

#### POST /api/v1/identity/users/{id}/roles
**Ne:** Kullanıcıya rol atama endpoint'i.  
**Neden:** Rol atama işlemi için HTTP endpoint gereklidir.  
**Özelliği:** AssignRoleCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** API üzerinden rol atama.

#### POST /api/v1/identity/auth/login
**Ne:** Kullanıcı giriş endpoint'i.  
**Neden:** Kimlik doğrulama için HTTP endpoint gereklidir.  
**Özelliği:** LoginRequest alır, AuthResponse (JWT token) döner.  
**Kim Kullanacak:** Tüm kullanıcılar.  
**Amaç:** API üzerinden giriş yapma.

#### POST /api/v1/identity/auth/refresh
**Ne:** Token yenileme endpoint'i.  
**Neden:** Refresh token mekanizması için HTTP endpoint gereklidir.  
**Özelliği:** RefreshTokenRequest alır, new AuthResponse döner.  
**Kim Kullanacak:** Oturumu olan tüm kullanıcılar.  
**Amaç:** API üzerinden token yenileme.

---

## SPRINT 2: Identity Module - Authentication & Authorization

### Sprint Hedefi
Bu sprint, Identity modülünün kimlik doğrulama ve yetkilendirme altyapısını tamamlayacaktır. JWT token yönetimi, refresh token mekanizması, password policy ve account lockout özellikleri geliştirilecektir.

### Domain Entity'leri

#### 1. RefreshToken Entity
**Ne:** Refresh token entity'si, süresi dolan access token'ları yenilemek için kullanılır.  
**Neden:** Kullanıcının sürekli şifre girmemesi için güvenli bir mekanizma sağlar.  
**Özelliği:** Token, userId, expiresAt, revokedAt, createdByIp özelliklerine sahiptir.  
**Kim Kullanacak:** AuthenticationService, son kullanıcılar.  
**Amaç:** Uzun süreli oturum yönetimi sağlamak.

#### 2. LoginAttempt Entity
**Ne:** Giriş denemesi entity'si, başarısız giriş denemelerini takip eder.  
**Neden:** Brute-force saldırılarına karşı koruma sağlar.  
**Özelliği:** UserId, ipAddress, attemptedAt, isSuccess, failureReason özelliklerine sahiptir.  
**Kim Kullanacak:** AuthenticationService, SecurityService.  
**Amaç:** Güvenlik politikalarının uygulanması ve denetim.

#### 3. PasswordPolicy Entity
**Ne:** Şifre politikası entity'si, sistem geneli şifre kurallarını tanımlar.  
**Neden:** Güvenlik standartlarının uygulanmasını sağlar.  
**Özelliği:** MinLength, requireUppercase, requireLowercase, requireDigit, requireSpecialChar, expiryDays özelliklerine sahiptir.  
**Kim Kullanacak:** AuthenticationService, UserService.  
**Amaç:** Şifre güvenlik kurallarının yönetimi.

#### 4. UserSession Entity
**Ne:** Kullanıcı oturumu entity'si, aktif oturumları takip eder.  
**Neden:** Multiple device session yönetimi ve concurrent session kontrolü sağlar.  
**Özelliği:** UserId, sessionId, deviceInfo, ipAddress, loginAt, lastActivityAt, isActive özelliklerine sahiptir.  
**Kim Kullanacak:** AuthenticationService, SessionService.  
**Amaç:** Oturum yönetimi ve güvenlik izleme.

### Domain Events

#### UserLoginEvent
**Ne:** Kullanıcı giriş yaptığında tetiklenen event'tir.  
**Neden:** Login audit trail için gereklidir.  
**Özelliği:** UserId, tenantId, ipAddress, deviceInfo, loginAt içerir.  
**Kim Kullanacak:** Audit modülü, SessionService.  
**Amaç:** Giriş işlemlerinin loglanması.

#### UserLogoutEvent
**Ne:** Kullanıcı çıkış yaptığında tetiklenen event'tir.  
**Neden:** Logout audit trail için gereklidir.  
**Özelliği:** UserId, sessionId, logoutAt içerir.  
**Kim Kullanacak:** Audit modülü, SessionService.  
**Amaç:** Çıkış işlemlerinin loglanması.

#### PasswordChangedEvent
**Ne:** Şifre değiştirildiğinde tetiklenen event'tir.  
**Neden:** Güvenlik denetimi için gereklidir.  
**Özelliği:** UserId, changedAt, changedBy içerir.  
**Kim Kullanacak:** Audit modülü, Notification modülü.  
**Amaç:** Şifre değişikliklerinin loglanması ve bildirim gönderimi.

#### AccountLockedEvent
**Ne:** Hesap kilitlendiğinde tetiklenen event'tir.  
**Neden:** Güvenlik olayı olarak değerlendirilir.  
**Özelliği:** UserId, lockReason, lockedAt içerir.  
**Kim Kullanacak:** Audit modülü, Notification modülü, SecurityService.  
**Amaç:** Kilitlenme işlemlerinin loglanması ve bildirim gönderimi.

### Commands

#### LoginCommand
**Ne:** Kullanıcı giriş command'i.  
**Neden:** CQRS pattern ile giriş işleminin yapılmasını sağlar.  
**Özelliği:** Email, password, deviceInfo parametreleri alır.  
**Kim Kullanacak:** Son kullanıcılar.  
**Amaç:** Kullanıcı kimlik doğrulama.

#### LogoutCommand
**Ne:** Kullanıcı çıkış command'i.  
**Neden:** CQRS pattern ile çıkış işleminin yapılmasını sağlar.  
**Özelliği:** SessionId, userId parametreleri alır.  
**Kim Kullanacak:** Son kullanıcılar.  
**Amaç:** Kullanıcı oturumunu sonlandırma.

#### ChangePasswordCommand
**Ne:** Şifre değiştirme command'i (Sprint 1'den devam).  
**Neden:** Mevcut şifre ile değiştirme.  
**Özelliği:** UserId, currentPassword, newPassword parametreleri alır.  
**Kim Kullanacak:** Son kullanıcılar.  
**Amaç:** Şifre değişikliği.

#### ResetPasswordCommand
**Ne:** Şifre resetleme command'i (Sprint 1'den devam).  
**Neden:** Admin veya forgot password senaryosu.  
**Özelliği:** UserId, newPassword, resetBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri, son kullanıcılar (forgot password).  
**Amaç:** Şifre sıfırlama.

#### LockAccountCommand
**Ne:** Hesap kilitleme command'i.  
**Neden:** Güvenlik politikası ihlalinde hesap kilitleme.  
**Özelliği:** UserId, lockReason, lockedBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri, otomatik (brute-force koruması).  
**Amaç:** Hesap kilitleme.

#### UnlockAccountCommand
**Ne:** Hesap kilidini açma command'i.  
**Neden:** Kilitli hesabın açılması.  
**Özelliği:** UserId, unlockedBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Hesap kilidini açma.

#### TerminateSessionCommand
**Ne:** Oturum sonlandırma command'i.  
**Neden:** Belirli bir oturumu uzaktan sonlandırma.  
**Özelliği:** SessionId, terminatedBy parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri, kullanıcı kendisi.  
**Amaç:** Oturum yönetimi.

### Queries

#### GetActiveSessionsQuery
**Ne:** Kullanıcının aktif oturumlarını getiren query.  
**Neden:** Session yönetimi ve güvenlik görünümü için gereklidir.  
**Özelliği:** UserId parametresi alır, List<SessionDTO> döner.  
**Kim Kullanacak:** Son kullanıcılar (kendi oturumları için), yöneticiler.  
**Amaç:** Aktif oturumların listelenmesi.

#### GetLoginHistoryQuery
**Ne:** Kullanıcının giriş geçmişini getiren query.  
**Neden:** Güvenlik denetimi için gereklidir.  
**Özelliği:** UserId, dateRange parametreleri alır, PaginatedResult<LoginHistoryDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri, kullanıcı kendi geçmişi için.  
**Amaç:** Giriş geçmişinin görüntülenmesi.

#### ValidateTokenQuery
**Ne:** Token geçerliliğini kontrol eden query.  
**Neden:** Middleware'de token doğrulama için kullanılır.  
**Özelliği:** Token parametresi alır, TokenValidationResult döner.  
**Kim Kullanacak:** JWT Middleware.  
**Amaç:** Token doğrulama.

### Application Services

#### IAuthenticationService
**Ne:** Kimlik doğrulama servisi.  
**Neden:** Authentication logic'in kapsüllenmesi.  
**Özelliği:** Login, Logout, ValidateToken, RefreshToken metotları.  
**Kim Kullanacak:** Auth Controller.  
**Amaç:** Kimlik doğrulama işlemleri.

#### IPasswordService
**Ne:** Şifre yönetim servisi.  
**Neden:** Şifre politikası ve validation logic'i.  
**Özelliği:** HashPassword, ValidatePassword, CheckPasswordStrength metotları.  
**Kim Kullanacak:** AuthenticationService, UserService.  
**Amaç:** Şifre işlemleri.

#### ISessionService
**Ne:** Oturum yönetim servisi.  
**Neden:** Session lifecycle yönetimi.  
**Özelliği:** CreateSession, TerminateSession, GetActiveSessions, TerminateAllUserSessions metotları.  
**Kim Kullanacak:** AuthenticationService, API Controllers.  
**Amaç:** Oturum yönetimi.

#### ISecurityService
**Ne:** Güvenlik servisi.  
**Neden:** Güvenlik politikaları ve lockout yönetimi.  
**Özelliği:** CheckLoginAttempts, LockAccount, UnlockAccount, IsAccountLocked metotları.  
**Kim Kullanacak:** AuthenticationService.  
**Amaç:** Güvenlik kontrolleri.

### API Endpoints

#### POST /api/v1/identity/auth/login
**Ne:** Giriş endpoint'i.  
**Neden:** Kullanıcı kimlik doğrulama.  
**Özelliği:** LoginCommand alır, AuthResponse döner.  
**Kim Kullanacak:** Tüm kullanıcılar.  
**Amaç:** Giriş yapma.

#### POST /api/v1/identity/auth/logout
**Ne:** Çıkış endpoint'i.  
**Neden:** Oturum sonlandırma.  
**Özelliği:** LogoutCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Oturum açan kullanıcılar.  
**Amaç:** Çıkış yapma.

#### POST /api/v1/identity/auth/refresh
**Ne:** Token yenileme endpoint'i.  
**Neden:** Access token yenileme.  
**Özelliği:** RefreshToken alır, AuthResponse döner.  
**Kim Kullanacak:** Oturum açan kullanıcılar.  
**Amaç:** Token yenileme.

#### POST /api/v1/identity/auth/password/change
**Ne:** Şifre değiştirme endpoint'i.  
**Neden:** Şifre değişikliği.  
**Özelliği:** ChangePasswordCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Oturum açan kullanıcılar.  
**Amaç:** Şifre değiştirme.

#### POST /api/v1/identity/auth/password/reset
**Ne:** Şifre resetleme endpoint'i.  
**Neden:** Unutulan şifre senaryosu.  
**Özelliği:** ResetPasswordCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Şifresini unutan kullanıcılar.  
**Amaç:** Şifre sıfırlama.

#### GET /api/v1/identity/auth/sessions
**Ne:** Aktif oturumları listeleme endpoint'i.  
**Neden:** Oturum yönetimi.  
**Özelliği:** GetActiveSessionsQuery alır, List<SessionDTO> döner.  
**Kim Kullanacak:** Oturum açan kullanıcılar.  
**Amaç:** Oturumları listeleme.

#### DELETE /api/v1/identity/auth/sessions/{id}
**Ne:** Oturum sonlandırma endpoint'i.  
**Neden:** Belirli oturumu kapatma.  
**Özelliği:** SessionId alır, SuccessResponse döner.  
**Kim Kullanacak:** Oturum açan kullanıcılar, yöneticiler.  
**Amaç:** Oturum sonlandırma.

#### POST /api/v1/identity/auth/account/lock/{id}
**Ne:** Hesap kilitleme endpoint'i.  
**Neden:** Hesap kilitleme.  
**Özelliği:** LockAccountCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Hesap kilitleme.

#### POST /api/v1/identity/auth/account/unlock/{id}
**Ne:** Hesap kilidini açma endpoint'i.  
**Neden:** Hesap açma.  
**Özelliği:** UnlockAccountCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Hesap kilidini açma.

---

## SPRINT 3: Tenant & License Module

### Sprint Hedefi
Bu sprint, multi-tenant mimarinin temelini oluşturacak Tenant ve License modüllerini kapsamaktadır. Tenant yönetimi, lisanslama altyapısı ve tenant-aware konfigürasyon sistemi geliştirilecektir.

### Domain Entity'leri (Tenant Modülü)

#### 1. Tenant Entity
**Ne:** Tenant entity'si, HBYS sistemini kullanan her hastane/kurumu temsil eder.  
**Neden:** Multi-tenant mimarinin temel birimidir. Her tenant kendi verisi, kullanıcıları ve konfigürasyonu ile izole edilmelidir.  
**Özelliği:** TenantCode (unique), Name, Type (SaaS/OnPrem/Group), Status, CreatedAt, SubscriptionStartDate, SubscriptionEndDate özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, SaaS operasyon ekibi.  
**Amaç:** Tenant'ların sisteme kaydedilmesi ve yönetilmesi.

#### 2. TenantConfiguration Entity
**Ne:** Tenant'a özel konfigürasyon ayarlarını temsil eder.  
**Neden:** Her tenant'ın kendi ayarlarını belirleyebilmesi gerekir (logo, tema, iş saatleri vb.).  
**Özelliği:** TenantId, configKey, configValue, isEncrypted, lastModified özelliklerine sahiptir.  
**Kim Kullanacak:** Tenant yöneticileri, sistem yöneticileri.  
**Amaç:** Tenant-spesifik konfigürasyon yönetimi.

#### 3. TenantSubscription Entity
**Ne:** Tenant abonelik bilgilerini tutar.  
**Neden:** Lisans ve abonelik takibi için gereklidir.  
**Özelliği:** TenantId, planId, startDate, endDate, status, autoRenew özelliklerine sahiptir.  
**Kim Kullanacak:** SaaS operasyon ekibi, billing sistemi.  
**Amaç:** Abonelik yönetimi.

#### 4. TenantModule Entity
**Ne:** Tenant'ın hangi modüllere erişim hakkı olduğunu tutar.  
**Neden:** Module-based licensing için gereklidir.  
**Özelliği:** TenantId, moduleId, isEnabled, licenseKey, expiryDate özelliklerine sahiptir.  
**Kim Kullanacak:** LicenseService, sistem yöneticileri.  
**Amaç:** Modül erişim kontrolü.

### Domain Entity'leri (License Modülü)

#### 1. License Entity
**Ne:** Lisans entity'si, her tenant için geçerli lisans bilgilerini tutar.  
**Neden:** Yazılım lisanslama ve özellik erişim kontrolü için zorunludur.  
**Özelliği:** LicenseKey, tenantId, moduleId, issuedAt, expiresAt, maxUsers, maxPatients, status özelliklerine sahiptir.  
**Kim Kullanacak:** LicenseService, sistem yöneticileri.  
**Amaç:** Lisans yönetimi ve doğrulama.

#### 2. LicensePlan Entity
**Ne:** Lisans planı entity'si, tanımlanmış plan özelliklerini tutar.  
**Neden:** Farklı paketlerin (Basic, Professional, Enterprise) tanımlanması gerekir.  
**Özelliği:** PlanCode, Name, Description, monthlyPrice, maxUsers, maxModules, features (JSON) özelliklerine sahiptir.  
**Kim Kullanacak:** Sales ekibi, sistem yöneticileri.  
**Amaç:** Plan tanımlama ve fiyatlandırma.

#### 3. LicenseAudit Entity
**Ne:** Lisans değişikliklerinin audit trail'ini tutar.  
**Neden:** Lisans ile ilgili tüm değişikliklerin takibi gereklidir.  
**Özelliği:** LicenseId, changeType, oldValue, newValue, changedBy, changedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Audit sistemi, sistem yöneticileri.  
**Amaç:** Lisans değişiklik logları.

### Domain Events

#### TenantCreatedEvent
**Ne:** Tenant oluşturulduğunda tetiklenen event'tir.  
**Neden:** Tenant oluşumunun sistem genelinde duyurulması gerekir.  
**Özelliği:** TenantId, tenantCode, name, tenantType içerir.  
**Kim Kullanacak:** Audit modülü, Notification modülü, Configuration modülü.  
**Amaç:** Tenant oluşumunun duyurulması.

#### TenantActivatedEvent
**Ne:** Tenant aktif edildiğinde tetiklenen event'tir.  
**Neden:** Tenant'ın aktifleştirilmesi bilgisinin yayılması gerekir.  
**Özelliği:** TenantId, activatedBy, activatedAt içerir.  
**Kim Kullanacak:** LicenseService, Notification modülü.  
**Amaç:** Tenant aktifleştirme duyurusu.

#### LicenseIssuedEvent
**Ne:** Lisans verildiğinde tetiklenen event'tir.  
**Neden:** Lisans verme işleminin loglanması gerekir.  
**Özelliği:** LicenseId, tenantId, moduleId, expiresAt içerir.  
**Kim Kullanacak:** Audit modülü, TenantService.  
**Amaç:** Lisans verme duyurusu.

#### LicenseExpiringEvent
**Ne:** Lisansın expire olmak üzere olduğunda tetiklenen event'tir.  
**Neden:** Süre bitiminden önce bildirim gönderilmesi gerekir.  
**Özelliği:** LicenseId, tenantId, daysRemaining içerir.  
**Kim Kullanacak:** Notification modülü, LicenseService.  
**Amaç:** Lisans süre uyarısı.

#### LicenseExpiredEvent
**Ne:** Lisans süresi dolduğunda tetiklenen event'tir.  
**Neden:** Süre dolan lisansın devre dışı bırakılması gerekir.  
**Özelliği:** LicenseId, tenantId, moduleId içerir.  
**Kim Kullanacak:** LicenseService, FeatureFlagService.  
**Amaç:** Lisans expiration işlemi.

### Commands

#### CreateTenantCommand
**Ne:** Yeni tenant oluşturma command'i.  
**Neden:** Yeni hastane/kurum kaydı için gereklidir.  
**Özelliği:** TenantCode, Name, Type, adminUserId parametreleri alır.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant oluşturma.

#### UpdateTenantCommand
**Ne:** Tenant bilgilerini güncelleme command'i.  
**Neden:** Tenant bilgi güncellemeleri için gereklidir.  
**Özelliği:** TenantId, Name, Status parametreleri alır.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant güncelleme.

#### ActivateTenantCommand
**Ne:** Tenant'ı aktif etme command'i.  
**Neden:** Yeni tenant'ın aktifleştirilmesi gerekir.  
**Özelliği:** TenantId, activatedBy parametreleri alır.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant aktifleştirme.

#### DeactivateTenantCommand
**Ne:** Tenant'ı pasif etme command'i.  
**Neden:** Tenant deaktivasyonu gerektiğinde kullanılır.  
**Özelliği:** TenantId, reason, deactivatedBy parametreleri alır.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant deaktifletirme.

#### IssueLicenseCommand
**Ne:** Lisans verme command'i.  
**Neden:** Tenant'a lisans atama için gereklidir.  
**Özelliği:** TenantId, moduleId, planId, durationDays, maxUsers parametreleri alır.  
**Kim Kullanacak:** License yöneticileri.  
**Amaç:** Lisans verme.

#### RevokeLicenseCommand
**Ne:** Lisans iptal command'i.  
**Neden:** Lisans iptal işlemi için gereklidir.  
**Özelliği:** LicenseId, reason, revokedBy parametreleri alır.  
**Kim Kullanacak:** License yöneticileri.  
**Amaç:** Lisans iptal etme.

#### UpdateTenantConfigurationCommand
**Ne:** Tenant konfigürasyonunu güncelleme command'i.  
**Neden:** Tenant ayarlarının değiştirilmesi gerekir.  
**Özelliği:** TenantId, configKey, configValue parametreleri alır.  
**Kim Kullanacak:** Tenant yöneticileri.  
**Amaç:** Konfigürasyon güncelleme.

### Queries

#### GetTenantByIdQuery
**Ne:** Tenant detay getirme query'si.  
**Neden:** Tenant bilgisi sorgulama için gereklidir.  
**Özelliği:** TenantId alır, TenantDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri, tenant yöneticileri.  
**Amaç:** Tenant sorgulama.

#### GetTenantByCodeQuery
**Ne:** Tenant'ı code ile getirme query'si.  
**Neden:** Subdomain veya code bazlı sorgulama için gereklidir.  
**Özelliği:** TenantCode alır, TenantDTO döner.  
**Kim Kullanacak:** Middleware (tenant resolution için).  
**Amaç:** Tenant code sorgulama.

#### GetAllTenantsQuery
**Ne:** Tüm tenantları listeleme query'si.  
**Neden:** Tenant yönetim ekranı için gereklidir.  
**Özelliği:** Pagination ve filter parametreleri alır, PaginatedResult<TenantListDTO> döner.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant listeleme.

#### GetTenantLicenseQuery
**Ne:** Tenant lisans bilgilerini getirme query'si.  
**Neden:** Lisans sorgulama için gereklidir.  
**Özelliği:** TenantId alır, TenantLicenseDTO döner.  
**Kim Kullanacak:** Tenant yöneticileri, License yöneticileri.  
**Amaç:** Lisans sorgulama.

#### GetExpiringLicensesQuery
**Ne:** Expire olmak üzere olan lisansları getirme query'si.  
**Neden:** Proaktif bildirim için gereklidir.  
**Özelliği:** DaysAhead parametresi alır, List<ExpiringLicenseDTO> döner.  
**Kim Kullanacak:** License yöneticileri.  
**Amaç:** Expiring license sorgulama.

#### GetTenantConfigurationQuery
**Ne:** Tenant konfigürasyonunu getirme query'si.  
**Neden:** Konfigürasyon değerleri için gereklidir.  
**Özelliği:** TenantId, configKey alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Uygulama katmanı.  
**Amaç:** Konfigürasyon okuma.

#### ValidateLicenseQuery
**Ne:** Lisans geçerliliğini kontrol eden query.  
**Neden:** Feature flag kontrolü için gereklidir.  
**Özelliği:** TenantId, moduleId alır, LicenseValidationResult döner.  
**Kim Kullanacak:** FeatureFlagService, Middleware.  
**Amaç:** Lisans doğrulama.

### Application Services

#### ITenantService
**Ne:** Tenant yönetim servisi.  
**Neden:** Tenant operasyonlarının kapsüllenmesi.  
**Özelliği:** CreateTenant, UpdateTenant, ActivateTenant, DeactivateTenant, GetTenant metotları.  
**Kim Kullanacak:** API Controllers, Background Jobs.  
**Amaç:** Tenant yönetimi.

#### ILicenseService
**Ne:** Lisans yönetim servisi.  
**Neden:** Lisans operasyonlarının kapsüllenmesi.  
**Özelliği:** IssueLicense, RevokeLicense, ValidateLicense, GetLicense metotları.  
**Kim Kullanacak:** FeatureFlagService, API Controllers.  
**Amaç:** Lisans yönetimi.

#### ITenantConfigurationService
**Ne:** Konfigürasyon yönetim servisi.  
**Neden:** Tenant konfigürasyon yönetimi.  
**Özelliği:** GetConfiguration, SetConfiguration, GetAllConfigurations metotları.  
**Kim Kullanacak:** Uygulama katmanı.  
**Amaç:** Konfigürasyon yönetimi.

#### IFeatureFlagService
**Ne:** Feature flag servisi.  
**Neden:** Lisans tabanlı özellik kontrolü.  
**Özelliği:** IsFeatureEnabled, GetEnabledFeatures, EvaluateFeature metotları.  
**Kim Kullanacak:** Uygulama katmanı, Middleware.  
**Amaç:** Feature flag yönetimi.

### API Endpoints

#### POST /api/v1/tenants
**Ne:** Tenant oluşturma endpoint'i.  
**Neden:** Yeni tenant kaydı.  
**Özelliği:** CreateTenantCommand alır, TenantDTO döner.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant oluşturma.

#### GET /api/v1/tenants/{id}
**Ne:** Tenant detay endpoint'i.  
**Neden:** Tenant bilgi sorgulama.  
**Özelliği:** TenantId alır, TenantDTO döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Tenant sorgulama.

#### PUT /api/v1/tenants/{id}
**Ne:** Tenant güncelleme endpoint'i.  
**Neden:** Tenant bilgi güncelleme.  
**Özelliği:** UpdateTenantCommand alır, TenantDTO döner.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant güncelleme.

#### POST /api/v1/tenants/{id}/activate
**Ne:** Tenant aktifleştirme endpoint'i.  
**Neden:** Tenant aktifleştirme.  
**Özelliği:** TenantId alır, SuccessResponse döner.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant aktifleştirme.

#### POST /api/v1/tenants/{id}/deactivate
**Ne:** Tenant deaktifleme endpoint'i.  
**Neden:** Tenant deaktifleme.  
**Özelliği:** DeactivateTenantCommand alır, SuccessResponse döner.  
**Kim Kullanacak:** SaaS operasyon ekibi.  
**Amaç:** Tenant deaktifleme.

#### GET /api/v1/tenants/{id}/licenses
**Ne:** Tenant lisansları endpoint'i.  
**Neden:** Lisans sorgulama.  
**Özelliği:** TenantId alır, List<LicenseDTO> döner.  
**Kim Kullanacak:** Tenant yöneticileri.  
**Amaç:** Lisans sorgulama.

#### POST /api/v1/licenses
**Ne:** Lisans verme endpoint'i.  
**Neden:** Lisans atama.  
**Özelliği:** IssueLicenseCommand alır, LicenseDTO döner.  
**Kim Kullanacak:** License yöneticileri.  
**Amaç:** Lisans verme.

#### DELETE /api/v1/licenses/{id}
**Ne:** Lisans iptal endpoint'i.  
**Neden:** Lisans iptal.  
**Özelliği:** LicenseId alır, SuccessResponse döner.  
**Kim Kullanacak:** License yöneticileri.  
**Amaç:** Lisans iptal.

#### GET /api/v1/tenants/{id}/configuration
**Ne:** Tenant konfigürasyonu endpoint'i.  
**Neden:** Konfigürasyon okuma.  
**Özelliği:** TenantId alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Tenant yöneticileri.  
**Amaç:** Konfigürasyon okuma.

#### PUT /api/v1/tenants/{id}/configuration
**Ne:** Tenant konfigürasyonu güncelleme endpoint'i.  
**Neden:** Konfigürasyon güncelleme.  
**Özelliği:** UpdateTenantConfigurationCommand alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Tenant yöneticileri.  
**Amaç:** Konfigürasyon güncelleme.

#### GET /api/v1/licenses/validate
**Ne:** Lisans doğrulama endpoint'i.  
**Neden:** Lisans geçerlilik kontrolü.  
**Özelliği:** TenantId, moduleId alır, ValidationResult döner.  
**Kim Kullanacak:** Feature flag sistemi.  
**Amaç:** Lisans doğrulama.

---

## SPRINT 4: Audit & Configuration Module

### Sprint Hedefi
Bu sprint, denetim ve konfigürasyon modüllerini kapsamaktadır. Immutable audit log sistemi ve merkezi konfigürasyon yönetimi geliştirilecektir.

### Domain Entity'leri (Audit Modülü)

#### 1. AuditLog Entity
**Ne:** Audit log entity'si, tüm sistem işlemlerinin immutable kaydını tutar.  
**Neden:** Yasal gereklilik ve güvenlik denetimi için zorunludur. WORM (Write Once Read Many) prensibi ile çalışır.  
**Özelliği:** Id, tenantId, userId, action, entityType, entityId, oldValue, newValue, ipAddress, userAgent, timestamp, isDeleted özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, denetçiler, yasal uyum ekipleri.  
**Amaç:** Tüm veri değişikliklerinin izlenebilirliğini sağlamak.

#### 2. AuditTrail Entity
**Ne:** Audit trail entity'si, entity bazlı değişiklik zincirini tutar.  
**Neden:** Bir kaydın tüm yaşam döngüsünün takibi için gereklidir.  
**Özelliği:** EntityType, entityId, version, changeType, changedBy, changedAt özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, denetçiler.  
**Amaç:** Entity yaşam döngüsü takibi.

#### 3. AuditConfiguration Entity
**Ne:** Audit konfigürasyonu, hangi entity'lerin denetleneceğini belirler.  
**Neden:** Tüm entity'ler yerine kritik entity'lerin denetlenmesi performans sağlar.  
**Özelliği:** EntityType, isEnabled, logOldValue, logNewValue, retentionDays özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Audit kapsamı yönetimi.

### Domain Entity'leri (Configuration Modülü)

#### 1. SystemConfiguration Entity
**Ne:** Sistem geneli konfigürasyon ayarlarını tutar.  
**Neden:** Uygulama ayarlarının merkezi yönetimi için gereklidir.  
**Özelliği:** ConfigKey (unique), configValue, configType, description, isEncrypted, lastModified, modifiedBy özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri, uygulama katmanı.  
**Amaç:** Sistem konfigürasyon yönetimi.

#### 2. ModuleConfiguration Entity
**Ne:** Modül bazlı konfigürasyonları tutar.  
**Neden:** Her modülün kendi ayarlarını yönetebilmesi için gereklidir.  
**Özelliği:** ModuleId, configKey, configValue, isEncrypted, lastModified özelliklerine sahiptir.  
**Kim Kullanacak:** Modül yöneticileri.  
**Amaç:** Modül konfigürasyon yönetimi.

#### 3. ConfigurationHistory Entity
**Ne:** Konfigürasyon değişiklik geçmişini tutar.  
**Neden:** Konfigürasyon değişikliklerinin takibi için gereklidir.  
**Özelliği:** ConfigurationId, oldValue, newValue, changedBy, changedAt, changeReason özelliklerine sahiptir.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon geçmişi takibi.

### Domain Events

#### AuditLogCreatedEvent
**Ne:** Audit log oluşturulduğunda tetiklenen event'tir.  
**Neden:** Audit log oluşumunun sistem genelinde duyurulması gerekir.  
**Özelliği:** AuditLogId, tenantId, action, entityType içerir.  
**Kim Kullanacak:** NotificationService (kritik aksiyonlar için).  
**Amaç:** Audit log oluşum duyurusu.

#### ConfigurationChangedEvent
**Ne:** Konfigürasyon değiştirildiğinde tetiklenen event'tir.  
**Neden:** Konfigürasyon değişikliğinin yayılması gerekir.  
**Özelliği:** ConfigKey, oldValue, newValue, changedBy, changedAt içerir.  
**Kim Kullanacak:** ConfigurationService (cache invalidation için).  
**Amaç:** Konfigürasyon değişiklik duyurusu.

#### CriticalActionDetectedEvent
**Ne:** Kritik bir aksiyon tespit edildiğinde tetiklenen event'tir.  
**Neden:** Kritik işlemler için özel izleme gerekir.  
**Özelliği:** ActionType, userId, tenantId, details içerir.  
**Kim Kullanacak:** SecurityService, NotificationService.  
**Amaç:** Kritik aksiyon uyarısı.

### Commands

#### CreateAuditLogCommand (Internal)
**Ne:** Audit log oluşturma command'i.  
**Neden:** Command handler'lar tarafından çağrılır.  
**Özelliği:** TenantId, userId, action, entityType, entityId, oldValue, newValue parametreleri alır.  
**Kim Kullanacak:** Tüm command handler'lar (automatically).  
**Amaç:** Audit kaydı oluşturma.

#### UpdateSystemConfigurationCommand
**Ne:** Sistem konfigürasyonunu güncelleme command'i.  
**Neden:** Konfigürasyon güncellemeleri için gereklidir.  
**Özelliği:** ConfigKey, newValue, changeReason parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon güncelleme.

#### UpdateModuleConfigurationCommand
**Ne:** Modül konfigürasyonunu güncelleme command'i.  
**Neden:** Modül ayar güncellemeleri için gereklidir.  
**Özelliği:** ModuleId, configKey, newValue parametreleri alır.  
**Kim Kullanacak:** Modül yöneticileri.  
**Amaç:** Modül konfigürasyon güncelleme.

#### EnableAuditCommand
**Ne:** Entity için audit'i aktif etme command'i.  
**Neden:** Audit kapsamı değişikliği için gereklidir.  
**Özelliği:** EntityType, enableOldValue, enableNewValue parametreleri alır.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Audit aktifleştirme.

### Queries

#### SearchAuditLogsQuery
**Ne:** Audit log arama query'si.  
**Neden:** Denetim ve sorun giderme için gereklidir.  
**Özelliği:** TenantId, userId, action, entityType, dateRange parametreleri alır, PaginatedResult<AuditLogDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri, denetçiler.  
**Amaç:** Audit log sorgulama.

#### GetEntityHistoryQuery
**Ne:** Entity değişiklik geçmişini getiren query.  
**Neden:** Bir kaydın tüm değişikliklerini görmek için gereklidir.  
**Özelliği:** EntityType, entityId alır, List<EntityHistoryDTO> döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Entity geçmişi sorgulama.

#### GetAuditSummaryQuery
**Ne:** Audit özet raporu query'si.  
**Neden:** Dashboard ve raporlama için gereklidir.  
**Özelliği:** TenantId, dateRange alır, AuditSummaryDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Audit özet raporu.

#### GetSystemConfigurationQuery
**Ne:** Sistem konfigürasyonu getirme query'si.  
**Neden:** Konfigürasyon değeri okuma için gereklidir.  
**Özelliği:** ConfigKey alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Uygulama katmanı.  
**Amaç:** Konfigürasyon okuma.

#### GetAllSystemConfigurationsQuery
**Ne:** Tüm sistem konfigürasyonlarını getirme query'si.  
**Neden:** Konfigürasyon yönetim ekranı için gereklidir.  
**Özelliği:** Filter parametreleri alır, PaginatedResult<ConfigurationDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon listeleme.

#### GetConfigurationHistoryQuery
**Ne:** Konfigürasyon geçmişi query'si.  
**Neden:** Değişiklik takibi için gereklidir.  
**Özelliği:** ConfigKey alır, List<ConfigurationHistoryDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon geçmişi.

### Application Services

#### IAuditService
**Ne:** Audit yönetim servisi.  
**Neden:** Audit operasyonlarının kapsüllenmesi.  
**Özelliği:** LogAction, GetAuditLogs, GetEntityHistory, GetAuditSummary metotları.  
**Kim Kullanacak:** Tüm command handlers (auto), API Controllers.  
**Amaç:** Audit yönetimi.

#### IConfigurationService
**Ne:** Konfigürasyon yönetim servisi.  
**Neden:** Konfigürasyon operasyonlarının kapsüllenmesi.  
**Özelliği:** GetConfiguration, SetConfiguration, GetAllConfigurations, GetConfigurationHistory metotları.  
**Kim Kullanacak:** Uygulama katmanı.  
**Amaç:** Konfigürasyon yönetimi.

#### IAuditConfigurationService
**Ne:** Audit konfigürasyon servisi.  
**Neden:** Audit ayarlarının yönetimi.  
**Özelliği:** GetAuditConfiguration, SetAuditConfiguration, GetEnabledEntities metotları.  
**Kim Kullanacak:** AuditService.  
**Amaç:** Audit konfigürasyon yönetimi.

### API Endpoints

#### GET /api/v1/audit/logs
**Ne:** Audit log arama endpoint'i.  
**Neden:** Log sorgulama.  
**Özelliği:** SearchAuditLogsQuery alır, PaginatedResult döner.  
**Kim Kullanacak:** Sistem yöneticileri, denetçiler.  
**Amaç:** Audit log sorgulama.

#### GET /api/v1/audit/entities/{type}/{id}/history
**Ne:** Entity geçmişi endpoint'i.  
**Neden:** Entity değişiklik geçmişi.  
**Özelliği:** EntityType, entityId alır, List döner.  
**Kim Kullanacak:** Yetkili kullanıcılar.  
**Amaç:** Entity geçmişi.

#### GET /api/v1/audit/summary
**Ne:** Audit özet endpoint'i.  
**Neden:** Özet rapor.  
**Özelliği:** GetAuditSummaryQuery alır, AuditSummaryDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Audit özet.

#### GET /api/v1/configuration/system
**Ne:** Sistem konfigürasyonu endpoint'i.  
**Neden:** Konfigürasyon okuma.  
**Özelliği:** ConfigKey alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon okuma.

#### PUT /api/v1/configuration/system
**Ne:** Sistem konfigürasyonu güncelleme endpoint'i.  
**Neden:** Konfigürasyon güncelleme.  
**Özelliği:** UpdateSystemConfigurationCommand alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon güncelleme.

#### GET /api/v1/configuration/system/all
**Ne:** Tüm konfigürasyonlar endpoint'i.  
**Neden:** Konfigürasyon listeleme.  
**Özelliği:** GetAllSystemConfigurationsQuery alır, PaginatedResult döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon listeleme.

#### GET /api/v1/configuration/system/{key}/history
**Ne:** Konfigürasyon geçmişi endpoint'i.  
**Neden:** Değişiklik takibi.  
**Özelliği:** ConfigKey alır, List<ConfigurationHistoryDTO> döner.  
**Kim Kullanacak:** Sistem yöneticileri.  
**Amaç:** Konfigürasyon geçmişi.

#### GET /api/v1/configuration/modules/{moduleId}
**Ne:** Modül konfigürasyonu endpoint'i.  
**Neden:** Modül konfigürasyon okuma.  
**Özelliği:** ModuleId alır, List<ConfigurationDTO> döner.  
**Kim Kullanacak:** Modül yöneticileri.  
**Amaç:** Modül konfigürasyon okuma.

#### PUT /api/v1/configuration/modules/{moduleId}
**Ne:** Modül konfigürasyonu güncelleme endpoint'i.  
**Neden:** Modül konfigürasyon güncelleme.  
**Özelliği:** UpdateModuleConfigurationCommand alır, ConfigurationDTO döner.  
**Kim Kullanacak:** Modül yöneticileri.  
**Amaç:** Modül konfigürasyon güncelleme.

---

## SPRINT 5: Stabilizasyon

### Sprint Hedefi
FAZ 0'ın stabilizasyon sprint'idir. Tüm modüller entegre edilecek, test edilecek ve production'a hazır hale getirilecektir.

### Yapılacak İşler

#### Entegrasyon Testleri
- Identity, Tenant, License, Audit, Configuration modülleri arası entegrasyon testleri
- Multi-tenant izolasyon testleri
- License feature flag testleri

#### Performans Testleri
- Load test: 100 concurrent user
- Stress test: 500 concurrent user
- Endurance test: 4 saat sürekli yük

#### Güvenlik Testleri
- Penetration test
- SQL injection test
- XSS test
- JWT token validation test

#### Dokümantasyon
- API dokümantasyonu (Swagger/OpenAPI)
- Deployment kılavuzu
- Operasyon kılavuzu
- Kullanıcı kılavuzu

#### Deployment Hazırlığı
- CI/CD pipeline kurulumu
- Production environment hazırlığı
- Backup/restore prosedürleri
- Monitoring dashboard kurulumu

---

## FAZ 0 ÖZETİ

### Tamamlanacak Modüller

| Modül | Sprint | Öncelik | Bağımlılıklar |
|-------|--------|---------|---------------|
| Identity | 1-2 | Critical | Yok |
| Tenant | 3 | Critical | Identity |
| License | 3 | Critical | Tenant |
| Audit | 4 | Critical | Identity, Tenant |
| Configuration | 4 | High | Tenant |

### Kritik Başarı Kriterleri

1. Kullanıcı kimlik doğrulama ve yetkilendirme çalışıyor olmalı
2. Multi-tenant izolasyon sağlanmış olmalı
3. Lisans tabanlı feature flag sistemi çalışıyor olmalı
4. Tüm işlemler audit log'lanıyor olmalı
5. Konfigürasyon merkezi olarak yönetiliyor olmalı
6. Unit test coverage %80'in üzerinde olmalı
7. API response time P95 < 200ms olmalı
