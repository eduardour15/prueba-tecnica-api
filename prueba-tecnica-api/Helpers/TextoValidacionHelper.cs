using System.Globalization;
using System.Text.RegularExpressions;
using PruebaTecnica.Api.Exceptions;

namespace PruebaTecnica.Api.Helpers;

public static partial class TextoValidacionHelper
{
    public static string Limpiar(string? valor)
    {
        if (valor is null) return string.Empty;

        var sinInvisibles = new string(valor
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.Format && !char.IsControl(c))
            .ToArray());

        return sinInvisibles.Trim();
    }

    public static bool EsPlacaValida(string placaLimpia)
    {
        return !string.IsNullOrEmpty(placaLimpia) && PlacaRegex().IsMatch(placaLimpia);
    }

    public static bool EsCorreoValido(string correoLimpio)
    {
        return !string.IsNullOrEmpty(correoLimpio) &&
               correoLimpio.Length <= 150 &&
               CorreoRegex().IsMatch(correoLimpio);
    }

    [GeneratedRegex(@"^(?=.*\d)[A-Za-z]{1,3}-?[A-Za-z0-9]{3,6}$")]
    private static partial Regex PlacaRegex();

    [GeneratedRegex(@"^(?!.*\.\.)[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,}$")]
    private static partial Regex CorreoRegex();
}