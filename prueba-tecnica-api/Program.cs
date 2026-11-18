using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Data.Interceptors;
using PruebaTecnica.Api.Services;
using PruebaTecnica.Api.Services.Interfaces;
using PruebaTecnica.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();
builder.Services.AddScoped<AuditoriaInterceptor>();
builder.Services.Configure<PolizaSettings>(builder.Configuration.GetSection(PolizaSettings.Seccion));
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                              throw new InvalidOperationException(
                                  "La cadena de conexión 'DefaultConnection' no está configurada.");
    options.UseSqlServer(connectionString)
        .AddInterceptors(serviceProvider.GetRequiredService<AuditoriaInterceptor>());
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Seguros LAFISE API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
