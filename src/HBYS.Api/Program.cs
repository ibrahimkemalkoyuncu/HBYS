using HBYS.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace HBYS.Api;

/// <summary>
/// HBYS API Başlangıç Sınıfı
/// Ne: Uygulama başlangıç noktası.
/// Neden: Servis konfigürasyonu ve middleware pipeline'ı için gereklidir.
/// Kim Kullanacak: Sunucu, Kullanıcı arayüzü.
/// Amacı: API'nin başlatılması.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Serilog konfigürasyonu
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "HBYS.Api")
            .WriteTo.Console(outputTemplate: 
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("HBYS API başlatılıyor...");

            var builder = WebApplication.CreateBuilder(args);

            // Serilog entegrasyonu
            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Swagger - Swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() 
                { 
                    Title = "HBYS API", 
                    Version = "v1",
                    Description = "Hastane Bilgi Yönetim Sistemi API - Afney Software House"
                });
            });
            
            // DbContext - SQL Server
            builder.Services.AddDbContext<HbysDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // Multi-tenant service
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HBYS.Application.Services.Tenant.ITenantContextAccessor, 
                HBYS.Application.Services.Tenant.TenantContextAccessor>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSerilogRequestLogging();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HBYS API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            app.MapControllers();

            // Database ensure created (SQL Server için)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HbysDbContext>();
                Log.Information("Veritabanı kontrol ediliyor...");
                dbContext.Database.EnsureCreated();
                Log.Information("Veritabanı hazır.");
            }

            Log.Information("HBYS API başarıyla başlatıldı.");
            Log.Information("Swagger: http://localhost:5292/swagger");
            Log.Information("Health check: GET /api/tenants/health");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Uygulama başlatılırken hata oluştu.");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
