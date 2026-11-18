using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> ObtenerPorIdSoloLecturaAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistePorIdentificacionAsync(string identificacion, int? excluirId = null,
        CancellationToken cancellationToken = default);

    Task<bool> TienePolizasAsync(int id, CancellationToken cancellationToken = default);
    Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    void Actualizar(Cliente cliente);
    void Eliminar(Cliente cliente);
}