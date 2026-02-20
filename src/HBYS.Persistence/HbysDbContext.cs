using HBYS.Domain.Entities;
using HBYS.Domain.Entities.Identity;
using HBYS.Domain.Entities.License;
using HBYS.Domain.Entities.Patient;
using HBYS.Domain.Entities.Appointment;
using HBYS.Domain.Entities.Outpatient;
using HBYS.Domain.Entities.Billing;
using HBYS.Domain.Entities.Inpatient;
using HBYS.Domain.Entities.Emergency;
using HBYS.Domain.Entities.Pharmacy;
using HBYS.Domain.Entities.Inventory;
using HBYS.Domain.Entities.Procurement;
using HBYS.Domain.Entities.Laboratory;
using HBYS.Domain.Entities.Radiology;
using Microsoft.EntityFrameworkCore;

namespace HBYS.Persistence;

/// <summary>
/// HBYS Database Context sınıfı.
/// Ne: Entity Framework Core veritabanı bağlam sınıfıdır.
/// Neden: Veritabanı işlemleri ve entity konfigürasyonları için gereklidir.
/// Özelliği: Tenant, User, Role, Permission, License entity'lerini içerir.
/// Kim Kullanacak: Repository sınıfları, Migration araçları.
/// Amacı: Veritabanı şeması yönetimi ve CRUD operasyonları.
/// </summary>
public class HbysDbContext : DbContext
{
    /// <summary>
    /// Tenant'lar
    /// </summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();
    
    /// <summary>
    /// Kullanıcılar
    /// </summary>
    public DbSet<User> Users => Set<User>();
    
    /// <summary>
    /// Roller
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();
    
    /// <summary>
    /// Yetkiler
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();
    
    /// <summary>
    /// Kullanıcı-Rol ilişkileri
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    
    /// <summary>
    /// Rol-Yetki ilişkileri
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    
    /// <summary>
    /// Lisanslar
    /// </summary>
    public DbSet<License> Licenses => Set<License>();
    
    /// <summary>
    /// Lisans özellikleri
    /// </summary>
    public DbSet<LicenseFeature> LicenseFeatures => Set<LicenseFeature>();

    // Patient Module
    public DbSet<Patient> Patients => Set<Patient>();

    // Appointment Module
    public DbSet<Appointment> Appointments => Set<Appointment>();

    // Outpatient Module
    public DbSet<Outpatient> Outpatients => Set<Outpatient>();

    // Billing Module
    public DbSet<Invoice> Invoices => Set<Invoice>();

    // Inpatient Module
    public DbSet<Inpatient> Inpatients => Set<Inpatient>();

    // Emergency Module
    public DbSet<Emergency> Emergencies => Set<Emergency>();

    // Pharmacy Module
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    // Inventory Module
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    // Procurement Module
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Requisition> Requisitions => Set<Requisition>();
    public DbSet<RequisitionItem> RequisitionItems => Set<RequisitionItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    // Laboratory Module
    public DbSet<LabRequest> LabRequests => Set<LabRequest>();
    public DbSet<LabTest> LabTests => Set<LabTest>();
    public DbSet<LabResult> LabResults => Set<LabResult>();

    // Radiology Module
    public DbSet<RadiologyRequest> RadiologyRequests => Set<RadiologyRequest>();
    public DbSet<RadiologyTest> RadiologyTests => Set<RadiologyTest>();

    /// <summary>
    /// Constructor
    /// </summary>
    public HbysDbContext(DbContextOptions<HbysDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Model konfigürasyonu
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant configuration
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TenantCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.SchemaName).HasMaxLength(50);
            entity.Property(e => e.ConnectionString).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantCode).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => new { e.UserName, e.TenantId }).IsUnique();
            entity.HasIndex(e => new { e.Email, e.TenantId }).IsUnique();
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => new { e.RoleName, e.TenantId }).IsUnique();
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PermissionName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Module).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.PermissionName, e.TenantId }).IsUnique();
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.RoleId, e.TenantId }).IsUnique();
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId, e.TenantId }).IsUnique();
        });

        // License configuration
        modelBuilder.Entity<License>(entity =>
        {
            entity.ToTable("Licenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModuleName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.ModuleName, e.TenantId }).IsUnique();
        });

        // LicenseFeature configuration
        modelBuilder.Entity<LicenseFeature>(entity =>
        {
            entity.ToTable("LicenseFeatures");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.LicenseId, e.FeatureName, e.TenantId }).IsUnique();
        });
    }

    /// <summary>
    /// SaveChanges override - Audit bilgisi ekle
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditableEntities();
        return base.SaveChanges();
    }

    /// <summary>
    /// SaveChangesAsync override - Audit bilgisi ekle
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Audit edilebilir entity'leri güncelle
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<BaseAuditEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
