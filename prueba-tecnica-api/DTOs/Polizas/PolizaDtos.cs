using System.ComponentModel.DataAnnotations;
using PruebaTecnica.Api.DTOs.Clientes;
using PruebaTecnica.Api.Helpers;

namespace PruebaTecnica.Api.DTOs.Polizas;

public record VehiculoRequest(
    [ Required(ErrorMessage = "La placa del vehículo es obligatoria.")]
    string Placa,
    [ Required(ErrorMessage = "La marca del vehículo es obligatoria.")]
    string Marca,
    [ Required(ErrorMessage = "El modelo del vehículo es obligatorio.")]
    string Modelo,
    [ Range(1900, 2100, ErrorMessage = "El año del vehículo debe estar entre 1900 y 2100.")]
    int Anio,
    [
        Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "El valor comercial debe ser mayor a 0.")]
    decimal ValorComercial) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var placaLimpia = TextoValidacionHelper.Limpiar(Placa);
        if (!TextoValidacionHelper.EsPlacaValida(placaLimpia))
            yield return new ValidationResult(
                "La placa debe iniciar con 1 a 3 letras, guion opcional y 3 a 6 caracteres alfanuméricos (ej. M123456, ABC-123).",
                [nameof(Placa)]);

        var marcaLimpia = TextoValidacionHelper.Limpiar(Marca);
        if (marcaLimpia.Length is < 2 or > 60)
            yield return new ValidationResult(
                "La marca debe tener entre 2 y 60 caracteres reales.",
                [nameof(Marca)]);

        var modeloLimpio = TextoValidacionHelper.Limpiar(Modelo);
        if (modeloLimpio.Length is < 1 or > 60)
            yield return new ValidationResult(
                "El modelo debe tener entre 1 y 60 caracteres reales.",
                [nameof(Modelo)]);
    }
}

public record EmitirPolizaRequest(
    [ Range(1, int.MaxValue, ErrorMessage = "El ClienteId debe ser mayor a 0.")]
    int ClienteId,
    [ Required(ErrorMessage = "Los datos del vehículo son obligatorios.")]
    VehiculoRequest Vehiculo,
    [ Required(ErrorMessage = "Debe enviar al menos una cobertura.")]
    [ MinLength(1, ErrorMessage = "Debe seleccionar al menos una cobertura.")]
    IReadOnlyCollection<int> CoberturaIds) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CoberturaIds is not null && CoberturaIds.Any(id => id <= 0))
            yield return new ValidationResult(
                "Todos los IDs de cobertura deben ser mayores a cero.",
                [nameof(CoberturaIds)]);
    }
}

public record VehiculoResponse(
    int Id,
    string Placa,
    string Marca,
    string Modelo,
    int Anio,
    decimal ValorComercial);

public record CoberturaAplicadaResponse(
    int CoberturaId,
    string Nombre,
    decimal TasaAplicada);

public record PolizaResponse(
    int Id,
    string NumeroPoliza,
    DateTime FechaEmision,
    decimal SumaAsegurada,
    decimal PrimaTotal,
    bool Activa,
    ClienteResponse Cliente,
    VehiculoResponse Vehiculo,
    IReadOnlyCollection<CoberturaAplicadaResponse> Coberturas);