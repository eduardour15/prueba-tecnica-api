using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Data.Interceptors;
using PruebaTecnica.Api.Repositories;
using PruebaTecnica.Api.Repositories.Interfaces;
using PruebaTecnica.Api.Services;
using PruebaTecnica.Api.Services.Interfaces;
using PruebaTecnica.Api.Settings;

namespace PruebaTecnica.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioActualService, UsuarioActualService>();
        services.AddScoped<AuditoriaInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

            options.UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditoriaInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICoberturaRepository, CoberturaRepository>();
        services.AddScoped<IVehiculoRepository, VehiculoRepository>();
        services.AddScoped<IPolizaRepository, PolizaRepository>();
        services.AddScoped<IBitacoraAuditoriaRepository, BitacoraAuditoriaRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PolizaSettings>(configuration.GetSection(PolizaSettings.Seccion));

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ICoberturaService, CoberturaService>();
        services.AddScoped<IPolizaService, PolizaService>();

        return services;
    }
}