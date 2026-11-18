using PruebaTecnica.Api.DTOs.Clientes;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Exceptions;
using PruebaTecnica.Api.Helpers;
using PruebaTecnica.Api.Repositories.Interfaces;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IBitacoraAuditoriaRepository _bitacoraRepository;
    private readonly IUsuarioActualService _usuarioActualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IClienteRepository clienteRepository,
        IBitacoraAuditoriaRepository bitacoraRepository,
        IUsuarioActualService usuarioActualService,
        IUnitOfWork unitOfWork,
        ILogger<ClienteService> logger)
    {
        _clienteRepository = clienteRepository;
        _bitacoraRepository = bitacoraRepository;
        _usuarioActualService = usuarioActualService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ClienteResponse>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.ObtenerTodosAsync(cancellationToken);

        return clientes
            .Select(MapearAResponse)
            .ToList();
    }

    public async Task<ClienteResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObtenerPorIdSoloLecturaAsync(id, cancellationToken)
                      ?? throw new NoEncontradoException($"No se encontró el cliente con ID {id}.");

        return MapearAResponse(cliente);
    }

    public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ReglaNegocioException("La solicitud de creación de cliente no puede ser nula.");
        var (nombreLimpio, identificacionNormalizada, correoLimpio) =
            ValidarYSanitizarDatosCliente(request.Nombre, request.Identificacion, request.Correo);

        var yaExiste = await _clienteRepository.ExistePorIdentificacionAsync(
            identificacionNormalizada,
            null,
            cancellationToken);

        if (yaExiste)
            throw new ConflictoException(
                $"Ya existe un cliente registrado con la identificación {identificacionNormalizada}.");

        var cliente = new Cliente
        {
            Nombre = nombreLimpio,
            Identificacion = identificacionNormalizada,
            Correo = correoLimpio
        };

        await _clienteRepository.AgregarAsync(cliente, cancellationToken);

        var bitacora = new BitacoraAuditoria
        {
            Fecha = DateTime.UtcNow,
            Usuario = _usuarioActualService.ObtenerUsuarioActual(),
            Accion = "CREACION_CLIENTE",
            Entidad = nameof(Cliente),
            EntidadId = identificacionNormalizada,
            Detalle = "Registro de nuevo cliente en el catálogo.",
            CorrelationId = _usuarioActualService.ObtenerCorrelationId(),
            IpOrigen = _usuarioActualService.ObtenerIpOrigen()
        };

        await _bitacoraRepository.RegistrarAsync(bitacora, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} creado exitosamente", cliente.Id);

        return MapearAResponse(cliente);
    }

    public async Task<ClienteResponse> ActualizarAsync(
        int id,
        ActualizarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ReglaNegocioException("La solicitud de creación de cliente no puede ser nula.");
        var (nombreLimpio, identificacionNormalizada, correoLimpio) =
            ValidarYSanitizarDatosCliente(request.Nombre, request.Identificacion, request.Correo);

        var cliente = await _clienteRepository.ObtenerPorIdAsync(id, cancellationToken)
                      ?? throw new NoEncontradoException($"No se encontró el cliente con ID {id}.");

        var identificacionDuplicada = await _clienteRepository.ExistePorIdentificacionAsync(
            identificacionNormalizada,
            id,
            cancellationToken);

        if (identificacionDuplicada)
            throw new ConflictoException($"Ya existe otro cliente con la identificación {identificacionNormalizada}.");

        cliente.Nombre = nombreLimpio;
        cliente.Identificacion = identificacionNormalizada;
        cliente.Correo = correoLimpio;

        var bitacora = new BitacoraAuditoria
        {
            Fecha = DateTime.UtcNow,
            Usuario = _usuarioActualService.ObtenerUsuarioActual(),
            Accion = "ACTUALIZACION_CLIENTE",
            Entidad = nameof(Cliente),
            EntidadId = id.ToString(),
            Detalle = $"Actualización de datos para ClienteId={id}.",
            CorrelationId = _usuarioActualService.ObtenerCorrelationId(),
            IpOrigen = _usuarioActualService.ObtenerIpOrigen()
        };

        await _bitacoraRepository.RegistrarAsync(bitacora, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} actualizado exitosamente", cliente.Id);

        return MapearAResponse(cliente);
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(id, cancellationToken)
                      ?? throw new NoEncontradoException($"No se encontró el cliente con ID {id}.");

        var tienePolizas = await _clienteRepository.TienePolizasAsync(id, cancellationToken);
        if (tienePolizas)
            throw new ConflictoException($"No se puede eliminar el cliente {id} porque tiene pólizas asociadas.");

        _clienteRepository.Eliminar(cliente);

        var bitacora = new BitacoraAuditoria
        {
            Fecha = DateTime.UtcNow,
            Usuario = _usuarioActualService.ObtenerUsuarioActual(),
            Accion = "ELIMINACION_CLIENTE",
            Entidad = nameof(Cliente),
            EntidadId = id.ToString(),
            Detalle = $"Eliminación de cliente sin pólizas asociadas (ClienteId={id}).",
            CorrelationId = _usuarioActualService.ObtenerCorrelationId(),
            IpOrigen = _usuarioActualService.ObtenerIpOrigen()
        };

        await _bitacoraRepository.RegistrarAsync(bitacora, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} eliminado exitosamente", id);
    }

    private static ClienteResponse MapearAResponse(Cliente cliente)
    {
        return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Identificacion, cliente.Correo);
    }

    private static (string Nombre, string Identificacion, string Correo) ValidarYSanitizarDatosCliente(
        string? nombre,
        string? identificacion,
        string? correo)
    {
        var nombreLimpio = TextoValidacionHelper.Limpiar(nombre);
        if (nombreLimpio.Length is < 2 or > 150)
            throw new ReglaNegocioException("El nombre debe tener entre 2 y 150 caracteres reales.");

        var identificacionNormalizada = TextoValidacionHelper.Limpiar(identificacion).ToUpperInvariant();
        if (identificacionNormalizada.Length is < 5 or > 30)
            throw new ReglaNegocioException("La identificación debe tener entre 5 y 30 caracteres reales.");

        var correoLimpio = TextoValidacionHelper.Limpiar(correo).ToLowerInvariant();
        if (!TextoValidacionHelper.EsCorreoValido(correoLimpio))
            throw new ReglaNegocioException("El formato del correo electrónico no es válido.");

        return (nombreLimpio, identificacionNormalizada, correoLimpio);
    }
}