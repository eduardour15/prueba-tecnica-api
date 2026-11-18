namespace PruebaTecnica.Api.Exceptions;

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}