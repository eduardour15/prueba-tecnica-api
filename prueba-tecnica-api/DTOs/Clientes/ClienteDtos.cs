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
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nombreLimpio = TextoValidacionHelper.Limpiar(Nombre);
        if (nombreLimpio.Length is < 2 or > 150)
            yield return new ValidationResult(
                "El nombre debe tener entre 2 y 150 caracteres reales.",
                [nameof(Nombre)]);

        var identificacionLimpia = TextoValidacionHelper.Limpiar(Identificacion);
        if (identificacionLimpia.Length is < 5 or > 30)
            yield return new ValidationResult(
                "La identificación debe tener entre 5 y 30 caracteres reales.",
                [nameof(Identificacion)]);

        var correoLimpio = TextoValidacionHelper.Limpiar(Correo);
        if (!TextoValidacionHelper.EsCorreoValido(correoLimpio))
            yield return new ValidationResult(
                "El formato del correo electrónico no es válido o carece de dominio completo.",
                [nameof(Correo)]);
    }
}

public record ActualizarClienteRequest(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    string Nombre,
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    string Identificacion,
    [Required(ErrorMessage = "El correo es obligatorio.")]
    string Correo) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nombreLimpio = TextoValidacionHelper.Limpiar(Nombre);
        if (nombreLimpio.Length is < 2 or > 150)
            yield return new ValidationResult(
                "El nombre debe tener entre 2 y 150 caracteres reales.",
                [nameof(Nombre)]);

        var identificacionLimpia = TextoValidacionHelper.Limpiar(Identificacion);
        if (identificacionLimpia.Length is < 5 or > 30)
            yield return new ValidationResult(
                "La identificación debe tener entre 5 y 30 caracteres reales.",
                [nameof(Identificacion)]);

        var correoLimpio = TextoValidacionHelper.Limpiar(Correo);
        if (!TextoValidacionHelper.EsCorreoValido(correoLimpio))
            yield return new ValidationResult(
                "El formato del correo electrónico no es válido o carece de dominio completo.",
                [nameof(Correo)]);
    }
}

public record ClienteResponse(
    int Id,
    string Nombre,
    string Identificacion,
    string Correo);