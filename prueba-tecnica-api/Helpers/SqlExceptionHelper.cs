using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnica.Api.Helpers;

internal static class SqlExceptionHelper
{
    public static bool EsViolacionUnica(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx && sqlEx.Number is 2601 or 2627;

    public static bool EsViolacionClaveForanea(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx && sqlEx.Number == 547;
}
