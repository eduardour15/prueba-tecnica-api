using System.ComponentModel.DataAnnotations;
using PruebaTecnica.Api.Helpers;

namespace PruebaTecnica.Api.DTOs.Clientes;

public record CrearClienteRequest(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    string Nombre,
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    string Identificacion,
    [Required(ErrorMessage = "El correo es obligatorio.")]
    string Correo) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        ClienteRequestValidator.Validar(Nombre, Identificacion, Correo);
}

public record ActualizarClienteRequest(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    string Nombre,
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    string Identificacion,
    [Required(ErrorMessage = "El correo es obligatorio.")]
    string Correo) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        ClienteRequestValidator.Validar(Nombre, Identificacion, Correo);
}

internal static class ClienteRequestValidator
{
    public static IEnumerable<ValidationResult> Validar(string? nombre, string? identificacion, string? correo)
    {
        var nombreLimpio = TextoValidacionHelper.Limpiar(nombre);
        if (!TextoValidacionHelper.EsNombreValido(nombreLimpio))
            yield return new ValidationResult(
                "El nombre debe tener entre 2 y 150 caracteres reales, iniciar con letra y contener al menos 2 letras.",
                [nameof(CrearClienteRequest.Nombre)]);

        var identificacionLimpia = TextoValidacionHelper.Limpiar(identificacion);
        if (!TextoValidacionHelper.EsIdentificacionValida(identificacionLimpia))
            yield return new ValidationResult(
                "La identificación debe tener entre 5 y 30 caracteres alfanuméricos válidos.",
                [nameof(CrearClienteRequest.Identificacion)]);

        var correoLimpio = TextoValidacionHelper.Limpiar(correo);
        if (!TextoValidacionHelper.EsCorreoValido(correoLimpio))
            yield return new ValidationResult(
                "El formato del correo electrónico no es válido o carece de dominio completo.",
                [nameof(CrearClienteRequest.Correo)]);
    }
}

public record ClienteResponse(
    int Id,
    string Nombre,
    string Identificacion,
    string Correo);