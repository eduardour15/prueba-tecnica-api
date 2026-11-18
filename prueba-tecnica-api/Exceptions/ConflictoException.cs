namespace PruebaTecnica.Api.Exceptions;

public class ConflictoException : Exception
{
    public ConflictoException(string mensaje) : base(mensaje)
    {
    }
}