using Microsoft.AspNetCore.Mvc;

namespace HBYS.Api.Controllers;

/// <summary>
/// Lisans Yönetimi Controller'ı
/// Ne: Lisans işlemleri için API endpoint'lerini barındıran controller sınıfıdır.
/// Neden: Lisans aktivasyonu, doğrulama ve özellik yönetimi için API erişimi sağlamak amacıyla oluşturulmuştur.
/// Özelliği: Tenant bazlı lisans kontrolü ve feature flag yönetimi sunar.
/// Kim Kullanacak: Frontend uygulaması, Lisans yönetimi paneli, Sistem yöneticileri.
/// Amacı: HBYS lisanslarının yönetilmesi ve doğrulanması.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(ILogger<LicensesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Lisans bilgilerini getir
    /// Ne: Mevcut tenant'a ait lisans detaylarını getiren endpoint.
    /// Neden: Lisans durumu görüntüleme ve özellik kontrolü için gereklidir.
    /// Kim Kullanacak: Admin paneli, Lisans yönetimi ekranı, Sistem başlangıcı.
    /// Amacı: Tenant lisans bilgilerinin görüntülenmesi.
    /// </summary>
    [HttpGet]
    public IActionResult GetLicense()
    {
        _logger.LogInformation("Getting license info");

        return Ok(new
        {
            licenseKey = "DEMO-LICENSE-2024-XXXX",
            isActive = true,
            expiresAt = DateTime.UtcNow.AddYears(1),
            maxUsers = 100,
            maxPatients = 10000,
            features = new[]
            {
                "outpatient",
                "inpatient",
                "laboratory",
                "radiology",
                "pharmacy",
                "billing",
                "accounting"
            },
            module = "HBYS Enterprise",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Lisans doğrula
    /// Ne: Girilen lisans anahtarının geçerli olup olmadığını kontrol eden endpoint.
    /// Neden: Yeni kurulum veya lisans yenileme işlemleri için gereklidir.
    /// Kim Kullanacak: Kurulum sihirbazı, Lisans aktivasyon ekranı.
    /// Amacı: Lisans anahtarının geçerliliğinin kontrol edilmesi.
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateLicense([FromBody] ValidateLicenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LicenseKey))
        {
            return BadRequest(new { error = "Lisans anahtarı zorunludur." });
        }

        _logger.LogInformation("Validating license: {LicenseKey}", request.LicenseKey);

        // Gerçek lisans doğrulama servisisi burada çağırılacak
        var isValid = request.LicenseKey.StartsWith("HBYS-");

        if (!isValid)
        {
            return BadRequest(new { error = "Geçersiz lisans anahtarı." });
        }

        return Ok(new
        {
            isValid = true,
            message = "Lisans geçerli."
        });
    }

    /// <summary>
    /// Özellik kontrolü yap
    /// Ne: Belirli bir özelliğin lisans kapsamında olup olmadığını kontrol eden endpoint.
/// Neden: Feature flag tabanlı özellik kontrolü için gereklidir.
/// Kim Kullanacak: Frontend uygulaması, Servis katmanı.
/// Amacı: Lisans özelliklerinin programatik olarak kontrol edilmesi.
/// </summary>
    [HttpGet("features/{featureName}")]
    public IActionResult CheckFeature(string featureName)
    {
        _logger.LogInformation("Checking feature: {FeatureName}", featureName);

        // Gerçek lisans servisinden özellik kontrolü yapılacak
        var allowedFeatures = new[] { "outpatient", "inpatient", "laboratory", "radiology" };
        var isEnabled = allowedFeatures.Contains(featureName.ToLower());

        return Ok(new
        {
            feature = featureName,
            isEnabled = isEnabled,
            message = isEnabled ? "Özellik aktif." : "Özellik lisans kapsamında değil."
        });
    }

    /// <summary>
    /// Lisans yenile
    /// Ne: Mevcut lisansı yenileyen endpoint.
/// Neden: Lisans süresi dolduğunda veya uzatılmak istendiğinde kullanılır.
/// Kim Kullanacak: Lisans yönetimi paneli, Sistem yöneticileri.
/// Amacı: Lisans süresinin uzatılması.
/// </summary>
    [HttpPost("renew")]
    public IActionResult RenewLicense([FromBody] RenewLicenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LicenseKey))
        {
            return BadRequest(new { error = "Lisans anahtarı zorunludur." });
        }

        _logger.LogInformation("Renewing license: {LicenseKey}", request.LicenseKey);

        return Ok(new
        {
            message = "Lisans başarıyla yenilendi.",
            newExpiryDate = DateTime.UtcNow.AddYears(1)
        });
    }
}

/// <summary>
/// Lisans doğrulama istek modeli
/// Ne: Lisans doğrulama endpoint'i için gerekli input modeli.
/// Neden: API üzerinden lisans anahtarı almak için gereklidir.
/// Kim Kullanacak: Kurulum sihirbazı, Lisans aktivasyon ekranı.
/// Amacı: Lisans doğrulama parametrelerini taşımak.
/// </summary>
public class ValidateLicenseRequest
{
    public string LicenseKey { get; set; } = string.Empty;
}

/// <summary>
/// Lisans yenileme istek modeli
/// Ne: Lisans yenileme endpoint'i için gerekli input modeli.
/// Neden: API üzerinden lisans yenileme bilgisi almak için gereklidir.
/// Kim Kullanacak: Lisans yönetimi paneli.
/// Amacı: Lisans yenileme parametrelerini taşımak.
/// </summary>
public class RenewLicenseRequest
{
    public string LicenseKey { get; set; } = string.Empty;
}
