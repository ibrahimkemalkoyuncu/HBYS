namespace HBYS.Domain.Entities;

/// <summary>
/// Base entity sınıfı.
/// Ne: Tüm entity'lerin kalıtım alacağı temel sınıftır.
/// Neden: Ortak özelliklerin paylaşımı için gereklidir.
/// Özelliği: Id özelliğine sahiptir.
/// Kim Kullanacak: Tüm entity sınıfları.
/// Amacı: Domain model taban sınıfı oluşturma.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Audit entity sınıfı.
/// Ne: Audit trail için kullanılan temel sınıftır.
/// Neden: Değişiklik takibi için gereklidir.
/// Özelliği: CreatedBy, CreatedAt, UpdatedBy, UpdatedAt özelliklerine sahiptir.
/// Kim Kullanacak: Audit gerektiren entity sınıfları.
/// Amacı: Audit bilgisi sağlama.
/// </summary>
public abstract class BaseAuditEntity : BaseEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Tenant entity sınıfı.
/// Ne: Multi-tenant uygulamalarda tenant bilgisi için kullanılan temel sınıftır.
/// Neden: Tenant izolasyonu için gereklidir.
/// Özelliği: TenantId özelliğine sahiptir.
/// Kim Kullanacak: Tüm tenant-specific entity sınıfları.
/// Amacı: Tenant bilgisi sağlama.
/// </summary>
public abstract class BaseTenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}

/// <summary>
/// Full audit tenant entity.
/// Ne: Tenant + Audit özelliklerini birleştiren entity.
/// Kim Kullanacak: Tüm tenant-specific audit entity sınıfları.
/// </summary>
public abstract class BaseAuditTenantEntity : BaseTenantEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
