using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.Exceptions;
using PruebaTecnica.Api.Extensions;
using PruebaTecnica.Api.Helpers;
using PruebaTecnica.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            ValidationProblemDetails problemDetails = new(context.ModelState)
            {
                Type = "https://httpstatuses.com/400",
                Title = "Error de validación en los datos de entrada",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Uno o más campos de la solicitud contienen errores de validación.",
                Instance = context.HttpContext.Request.Path
            };
            problemDetails.Extensions["correlationId"] =
                CorrelationIdHelper.ObtenerDesdeHttpContext(context.HttpContext);

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json", "application/json" }
            };
        };
    });
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Seguros LAFISE S.A. - API de Emisión de Pólizas";
        document.Info.Version = "v1";
        document.Info.Description = "Prototipo funcional para emisión, consulta e historial de pólizas de automóviles.";
        return Task.CompletedTask;
    });
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd(
            "correlationId",
            CorrelationIdHelper.ObtenerDesdeHttpContext(context.HttpContext));
        context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
        if (context.ProblemDetails.Status is int status)
        {
            context.ProblemDetails.Type ??= $"https://httpstatuses.com/{status}";
        }
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Seguros LAFISE API v1");
    });
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();