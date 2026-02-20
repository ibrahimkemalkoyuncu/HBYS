namespace HBYS.Domain.Entities.Identity;

/// <summary>
/// Kullanıcı entity sınıfı.
/// Ne: Sistem kullanıcılarını temsil eden entity sınıfıdır.
/// Neden: Kimlik doğrulama ve yetkilendirme için gereklidir.
/// Özelliği: UserName, Email, PasswordHash, IsActive, LastLoginAt, FailedLoginAttempts, LockedUntil, TenantId özelliklerine sahiptir.
/// Kim Kullanacak: UserRepository, AuthenticationService, AuthorizationService.
/// Amacı: Kullanıcı verilerinin domain model olarak yönetilmesi.
/// </summary>
public class User : BaseTenantEntity
{
    /// <summary>
    /// Kullanıcı adı
    /// </summary>
    public string UserName { get; private set; } = string.Empty;
    
    /// <summary>
    /// E-posta adresi
    /// </summary>
    public string Email { get; private set; } = string.Empty;
    
    /// <summary>
    /// Şifre hash'i
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Son giriş tarihi
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }
    
    /// <summary>
    /// Başarısız giriş deneme sayısı
    /// </summary>
    public int FailedLoginAttempts { get; private set; }
    
    /// <summary>
    /// Kilitlenme tarihi
    /// </summary>
    public DateTime? LockedUntil { get; private set; }
    
    /// <summary>
    /// Personel ID (opsiyonel)
    /// </summary>
    public Guid? EmployeeId { get; private set; }
    
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    private User() { }

    /// <summary>
    /// Factory method - Yeni kullanıcı oluşturma
    /// </summary>
    public static User Create(
        string userName,
        string email,
        string passwordHash,
        Guid tenantId,
        Guid? employeeId = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username is required", nameof(userName));
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        return new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true,
            FailedLoginAttempts = 0,
            TenantId = tenantId,
            EmployeeId = employeeId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Şifre değiştir
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Başarısız giriş denemesi
    /// </summary>
    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 30)
    {
        FailedLoginAttempts++;
        
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }
    }

    /// <summary>
    /// Başarılı giriş
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    /// <summary>
    /// Kilitlenme
    /// </summary>
    public void Lock(DateTime until)
    {
        LockedUntil = until;
    }

    /// <summary>
    /// Kilidi aç
    /// </summary>
    public void Unlock()
    {
        LockedUntil = null;
        FailedLoginAttempts = 0;
    }

    /// <summary>
    /// Deaktive et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Aktive et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kilitli mi?
    /// </summary>
    public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

    /// <summary>
    /// Giriş yapabilir mi?
    /// </summary>
    public bool CanLogin()
    {
        return IsActive && !IsLocked;
    }
}

/// <summary>
/// Kullanıcı rol ilişkisi
/// </summary>
public class UserRole : BaseTenantEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId, Guid tenantId)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            TenantId = tenantId
        };
    }
}

/// <summary>
/// Rol entity sınıfı.
/// Ne: Kullanıcı rollerini temsil eden entity sınıfıdır.
/// Neden: Yetkilendirme için gereklidir.
/// </summary>
public class Role : BaseTenantEntity
{
    public string RoleName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Role() { }

    public static Role Create(string roleName, string description, Guid tenantId)
    {
        return new Role
        {
            RoleName = roleName,
            Description = description,
            IsActive = true,
            TenantId = tenantId
        };
    }
}

/// <summary>
/// Yetki (Permission) entity sınıfı.
/// Ne: Sistem yetkilerini temsil eden entity sınıfıdır.
/// Neden: Granüler yetkilendirme için gereklidir.
/// </summary>
public class Permission : BaseTenantEntity
{
    public string PermissionName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Permission() { }

    public static Permission Create(string permissionName, string description, string module, Guid tenantId)
    {
        return new Permission
        {
            PermissionName = permissionName,
            Description = description,
            Module = module,
            IsActive = true,
            TenantId = tenantId
        };
    }
}

/// <summary>
/// Rol-Yetki ilişkisi
/// </summary>
public class RolePermission : BaseTenantEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission() { }

    public static RolePermission Create(Guid roleId, Guid permissionId, Guid tenantId)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            TenantId = tenantId
        };
    }
}
