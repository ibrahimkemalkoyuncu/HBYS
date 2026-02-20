using HBYS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HBYS.Application.Services.Tenant;

/// <summary>
/// Tenant Context Accessor interface.
/// Ne: Multi-tenant mimaride tenant bilgisine erişim için arayüz.
/// Neden: Tenant izolasyonu için gereklidir.
/// Kim Kullanacak: Repository sınıfları, Handler sınıfları.
/// Amacı: Tenant ID'nin HTTP context üzerinden erişilebilir kılınması.
/// </summary>
public interface ITenantContextAccessor
{
    /// <summary>
    /// Mevcut tenant ID
    /// </summary>
    Guid? TenantId { get; }
    
    /// <summary>
    /// Mevcut tenant kodu
    /// </summary>
    string? TenantCode { get; }
    
    /// <summary>
    /// Tenant ID ayarla
    /// </summary>
    void SetTenant(Guid tenantId, string tenantCode);
    
    /// <summary>
    /// Temizle
    /// </summary>
    void Clear();
}

/// <summary>
/// Tenant Context Accessor implementation.
/// Ne: Tenant bilgisinin HTTP context üzerinden yönetimi.
/// Kim Kullanacak: Middleware, Repository.
/// </summary>
public class TenantContextAccessor : ITenantContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    private const string TenantIdKey = "TenantId";
    private const string TenantCodeKey = "TenantCode";

    public TenantContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items[TenantIdKey] is Guid tenantId)
            {
                return tenantId;
            }
            return null;
        }
    }

    public string? TenantCode
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Items[TenantCodeKey] as string;
        }
    }

    public void SetTenant(Guid tenantId, string tenantCode)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Items[TenantIdKey] = tenantId;
            context.Items[TenantCodeKey] = tenantCode;
        }
    }

    public void Clear()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Items.Remove(TenantIdKey);
            context.Items.Remove(TenantCodeKey);
        }
    }
}

/// <summary>
/// Tenant Context Middleware.
/// Ne: HTTP request'ten tenant bilgisini çıkaran middleware.
/// Kim Kullanacak: Pipeline.
/// </summary>
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantRepository _tenantRepository;

    public TenantContextMiddleware(
        RequestDelegate next,
        ITenantRepository tenantRepository)
    {
        _next = next;
        _tenantRepository = tenantRepository;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextAccessor tenantAccessor)
    {
        // Tenant code'u header veya subdomain'den al
        var tenantCode = context.Request.Headers["X-Tenant-Code"].FirstOrDefault()
            ?? context.Request.Host.Host.Split('.').FirstOrDefault();

        if (!string.IsNullOrEmpty(tenantCode))
        {
            var tenant = await _tenantRepository.GetByTenantCodeAsync(tenantCode);
            if (tenant != null && tenant.IsActive)
            {
                tenantAccessor.SetTenant(tenant.Id, tenant.TenantCode);
            }
        }

        await _next(context);
    }
}
