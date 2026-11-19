using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PruebaTecnica.Api.Helpers;

public static partial class TextoValidacionHelper
{
    public static string Limpiar(string? valor)
    {
        if (valor is null) return string.Empty;

        StringBuilder builder = new(valor.Length);
        bool espacioPendiente = false;

        foreach (char c in valor)
        {
            if (char.GetUnicodeCategory(c) == UnicodeCategory.Format)
                continue;

            if (char.IsWhiteSpace(c))
            {
                if (builder.Length > 0)
                    espacioPendiente = true;
                continue;
            }

            if (char.IsControl(c))
                continue;

            if (espacioPendiente)
            {
                builder.Append(' ');
                espacioPendiente = false;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    public static string NormalizarPlacaCanonica(string placaLimpia)
    {
        return string.IsNullOrEmpty(placaLimpia)
            ? string.Empty
            : placaLimpia.Replace("-", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
    }

    public static bool EsPlacaValida(string placaLimpia)
    {
        return !string.IsNullOrEmpty(placaLimpia) && PlacaRegex().IsMatch(placaLimpia);
    }

    public static bool EsNombreValido(string nombreLimpio)
    {
        return nombreLimpio.Length is >= 2 and <= 150 &&
               char.IsLetter(nombreLimpio[0]) &&
               nombreLimpio.Count(char.IsLetter) >= 2;
    }

    public static bool EsIdentificacionValida(string identificacionLimpia)
    {
        return identificacionLimpia.Length is >= 5 and <= 30 &&
               IdentificacionRegex().IsMatch(identificacionLimpia) &&
               identificacionLimpia.Count(char.IsLetterOrDigit) >= 4;
    }

    public static bool EsMarcaValida(string marcaLimpia)
    {
        return marcaLimpia.Length is >= 2 and <= 60 &&
               char.IsLetterOrDigit(marcaLimpia[0]) &&
               marcaLimpia.Any(char.IsLetter) &&
               marcaLimpia.Count(char.IsLetterOrDigit) >= 2;
    }

    public static bool EsModeloValido(string modeloLimpio)
    {
        return modeloLimpio.Length is >= 1 and <= 60 &&
               char.IsLetterOrDigit(modeloLimpio[0]) &&
               modeloLimpio.Any(char.IsLetterOrDigit);
    }

    public static bool EsCorreoValido(string correoLimpio)
    {
        return !string.IsNullOrEmpty(correoLimpio) &&
               correoLimpio.Length <= 150 &&
               CorreoRegex().IsMatch(correoLimpio);
    }

    [GeneratedRegex(@"^(?=.*\d)[A-Za-z]{1,3}-?[A-Za-z0-9]{3,6}$")]
    private static partial Regex PlacaRegex();

    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9.\- ]*[A-Za-z0-9]$")]
    private static partial Regex IdentificacionRegex();

    [GeneratedRegex(@"^(?!.*\.\.)[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,}$")]
    private static partial Regex CorreoRegex();
}