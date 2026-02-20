using HBYS.Domain.Entities;
using HBYS.Domain.Entities.License;
using HBYS.Domain.Entities.Identity;

namespace HBYS.Domain.Interfaces;

/// <summary>
/// Tenant repository interface.
/// Ne: Tenant veri erişim katmanı için repository pattern arayüzü.
/// Neden: Tenant verilerinin veritabanı işlemleri için gereklidir.
/// Kim Kullanacak: TenantService, ITenantQueryHandler.
/// Amacı: Tenant CRUD operasyonlarının soyutlanması.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Tüm tenant'ları getir
    /// </summary>
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ID'ye göre tenant getir
    /// </summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tenant kodu ile getir
    /// </summary>
    Task<Tenant?> GetByTenantCodeAsync(string tenantCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Yeni tenant ekle
    /// </summary>
    Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tenant güncelle
    /// </summary>
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tenant sil
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// User repository interface.
/// Ne: Kullanıcı veri erişim katmanı için repository pattern arayüzü.
/// Neden: Kullanıcı verilerinin veritabanı işlemleri için gereklidir.
/// Kim Kullanacak: AuthenticationService, UserService.
/// Amacı: User CRUD operasyonlarının soyutlanması.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Tüm kullanıcıları getir
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ID'ye göre kullanıcı getir
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Username ile kullanıcı getir
    /// </summary>
    Task<User?> GetByUserNameAsync(string userName, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Email ile kullanıcı getir
    /// </summary>
    Task<User?> GetByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Yeni kullanıcı ekle
    /// </summary>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Kullanıcı güncelle
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Kullanıcı sil
    /// </summary>
    Task DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Kullanıcının rol ID'lerini getir
    /// </summary>
    Task<IEnumerable<Guid>> GetRoleIdsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Role repository interface.
/// </summary>
public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Permission repository interface.
/// </summary>
public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// License repository interface.
/// </summary>
public interface ILicenseRepository
{
    /// <summary>
    /// Tenant'ın tüm lisanslarını getir
    /// </summary>
    Task<IEnumerable<License>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ID'ye göre lisans getir
    /// </summary>
    Task<License?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Modüle göre lisans getir
    /// </summary>
    Task<License?> GetByModuleAsync(string moduleName, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Yeni lisans ekle
    /// </summary>
    Task<License> AddAsync(License license, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lisans güncelle
    /// </summary>
    Task UpdateAsync(License license, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lisans özelliklerini getir
    /// </summary>
    Task<IEnumerable<LicenseFeature>> GetFeaturesAsync(Guid licenseId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tenant'ın aktif lisans sayısını kontrol et
    /// </summary>
    Task<int> GetActiveLicenseCountAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
