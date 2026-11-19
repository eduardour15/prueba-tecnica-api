using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PruebaTecnica.Api.DTOs.Clientes;
using PruebaTecnica.Api.DTOs.Polizas;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Exceptions;
using PruebaTecnica.Api.Helpers;
using PruebaTecnica.Api.Repositories.Interfaces;
using PruebaTecnica.Api.Services.Interfaces;
using PruebaTecnica.Api.Settings;

namespace PruebaTecnica.Api.Services;

public class PolizaService : IPolizaService
{
    private readonly IPolizaRepository _polizaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICoberturaRepository _coberturaRepository;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IBitacoraAuditoriaRepository _bitacoraRepository;
    private readonly IUsuarioActualService _usuarioActualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PolizaSettings _settings;
    private readonly ILogger<PolizaService> _logger;

    public PolizaService(
        IPolizaRepository polizaRepository,
        IClienteRepository clienteRepository,
        ICoberturaRepository coberturaRepository,
        IVehiculoRepository vehiculoRepository,
        IBitacoraAuditoriaRepository bitacoraRepository,
        IUsuarioActualService usuarioActualService,
        IUnitOfWork unitOfWork,
        IOptions<PolizaSettings> settings,
        ILogger<PolizaService> logger)
    {
        _polizaRepository = polizaRepository;
        _clienteRepository = clienteRepository;
        _coberturaRepository = coberturaRepository;
        _vehiculoRepository = vehiculoRepository;
        _bitacoraRepository = bitacoraRepository;
        _usuarioActualService = usuarioActualService;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<PolizaResponse> EmitirPolizaAsync(
        EmitirPolizaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ReglaNegocioException("La solicitud de emisión no puede ser nula.");

        if (request.ClienteId <= 0) throw new ReglaNegocioException("El ClienteId debe ser mayor a cero.");

        if (request.Vehiculo is null) throw new ReglaNegocioException("Los datos del vehículo son obligatorios.");

        if (request.CoberturaIds is null || request.CoberturaIds.Count == 0)
            throw new ReglaNegocioException("Debe seleccionar al menos una cobertura válida.");

        var placaNormalizada = TextoValidacionHelper.Limpiar(request.Vehiculo.Placa).ToUpperInvariant();
        if (!TextoValidacionHelper.EsPlacaValida(placaNormalizada))
            throw new ReglaNegocioException("La placa del vehículo es obligatoria y debe tener un formato válido.");

        var marcaLimpia = TextoValidacionHelper.Limpiar(request.Vehiculo.Marca);
        if (marcaLimpia.Length is < 2 or > 60)
            throw new ReglaNegocioException("La marca del vehículo debe tener entre 2 y 60 caracteres reales.");

        var modeloLimpio = TextoValidacionHelper.Limpiar(request.Vehiculo.Modelo);
        if (modeloLimpio.Length is < 1 or > 60)
            throw new ReglaNegocioException("El modelo del vehículo debe tener entre 1 y 60 caracteres reales.");

        if (request.Vehiculo.ValorComercial <= 0m)
            throw new ReglaNegocioException("El valor comercial del vehículo debe ser mayor a cero.");

        //Validación de antigüedad y año máximo del vehículo
        var ahoraUtc = DateTime.UtcNow;
        var anioActualUtc = ahoraUtc.Year;

        if (request.Vehiculo.Anio > anioActualUtc + 1)
            throw new ReglaNegocioException(
                $"El año del vehículo ({request.Vehiculo.Anio}) no puede ser superior a {anioActualUtc + 1}.");

        var antiguedadAnios = anioActualUtc - request.Vehiculo.Anio;
        if (antiguedadAnios > _settings.MaxAntiguedadVehiculoAnios)
            throw new ReglaNegocioException(
                $"No se puede emitir una póliza para un vehículo con más de {_settings.MaxAntiguedadVehiculoAnios} años de antigüedad.");

        //Validar existencia del cliente
        var cliente = await _clienteRepository.ObtenerPorIdSoloLecturaAsync(request.ClienteId, cancellationToken)
                      ?? throw new NoEncontradoException($"No se encontró el cliente con ID {request.ClienteId}.");

        //Validar que el cliente no tenga ya una póliza activa para la misma placa
        var existePolizaActiva = await _polizaRepository.ExistePolizaActivaPorClienteYPlacaAsync(
            request.ClienteId,
            placaNormalizada,
            cancellationToken);

        if (existePolizaActiva)
            throw new ConflictoException(
                $"El cliente {request.ClienteId} ya posee una póliza activa para el vehículo con placa {placaNormalizada}.");
        
        bool existePolizaActivaEnPlaca = await _polizaRepository.ExistePolizaActivaPorPlacaAsync(
            placaNormalizada,
            cancellationToken);

        if (existePolizaActivaEnPlaca)
            throw new ConflictoException(
                $"Ya existe una póliza activa para el vehículo con placa {placaNormalizada}.");

        //Validar y obtener coberturas seleccionadas
        var coberturaIdsUnicos = request.CoberturaIds.Distinct().ToList();
        if (coberturaIdsUnicos.Any(id => id <= 0))
        {
            throw new ReglaNegocioException("Todos los IDs de cobertura deben ser mayores a cero.");
        }
        if (coberturaIdsUnicos.Count == 0)
            throw new ReglaNegocioException("Debe seleccionar al menos una cobertura válida.");

        var coberturas = await _coberturaRepository.ObtenerPorIdsAsync(
            coberturaIdsUnicos,
            cancellationToken);

        if (coberturas.Count != coberturaIdsUnicos.Count)
        {
            var idsFaltantes = coberturaIdsUnicos
                .Except(coberturas.Select(c => c.Id))
                .ToList();

            throw new NoEncontradoException(
                $"No se encontraron las siguientes coberturas: {string.Join(", ", idsFaltantes)}.");
        }

        //Reutilizar vehículo existente por placa o registrar uno nuevo
        var vehiculo = await _vehiculoRepository.ObtenerPorPlacaAsync(placaNormalizada, cancellationToken);
        if (vehiculo is null)
        {
            vehiculo = new Vehiculo
            {
                Placa = placaNormalizada,
                Marca = marcaLimpia,
                Modelo = modeloLimpio,
                Anio = request.Vehiculo.Anio,
                ValorComercial = request.Vehiculo.ValorComercial
            };
            await _vehiculoRepository.AgregarAsync(vehiculo, cancellationToken);
        }
        else
        {
            bool datosIncompatibles =
                !string.Equals(vehiculo.Marca, marcaLimpia, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(vehiculo.Modelo, modeloLimpio, StringComparison.OrdinalIgnoreCase) ||
                vehiculo.Anio != request.Vehiculo.Anio;

            if (datosIncompatibles)
            {
                throw new ConflictoException(
                    $"La placa {placaNormalizada} ya se encuentra registrada para otro vehículo ({vehiculo.Marca} {vehiculo.Modelo} {vehiculo.Anio}).");
            }

            vehiculo.ValorComercial = request.Vehiculo.ValorComercial;
        }

        //Cálculos financieros y número único exclusivamente en el servidor
        var sumaTasas = coberturas.Sum(c => c.Tasa);
        var sumaAsegurada = request.Vehiculo.ValorComercial;
        var primaTotal = Math.Round(
            sumaAsegurada * (sumaTasas / 100m),
            2,
            MidpointRounding.AwayFromZero);
        if (primaTotal <= 0m)
        {
            throw new ReglaNegocioException(
                "La prima total calculada debe ser mayor a cero. El valor comercial o las coberturas no generan prima asegurable.");
        }

        var numeroPoliza = await GenerarNumeroPolizaAsync(ahoraUtc, cancellationToken);

        var poliza = new Poliza
        {
            NumeroPoliza = numeroPoliza,
            ClienteId = cliente.Id,
            Vehiculo = vehiculo,
            FechaEmision = ahoraUtc,
            SumaAsegurada = sumaAsegurada,
            PrimaTotal = primaTotal,
            Activa = true,
            PolizaCoberturas = coberturas
                .Select(c => new PolizaCobertura
                {
                    CoberturaId = c.Id,
                    TasaAplicada = c.Tasa
                })
                .ToList()
        };

        await _polizaRepository.AgregarAsync(poliza, cancellationToken);

        var bitacora = new BitacoraAuditoria
        {
            Fecha = ahoraUtc,
            Usuario = _usuarioActualService.ObtenerUsuarioActual(),
            Accion = "EMISION_POLIZA",
            Entidad = nameof(Poliza),
            EntidadId = numeroPoliza,
            Detalle =
                $"ClienteId={cliente.Id}, Placa={placaNormalizada}, SumaAsegurada={sumaAsegurada:F2}, PrimaTotal={primaTotal:F2}, Coberturas={coberturas.Count}",
            CorrelationId = _usuarioActualService.ObtenerCorrelationId(),
            IpOrigen = _usuarioActualService.ObtenerIpOrigen()
        };

        await _bitacoraRepository.RegistrarAsync(bitacora, cancellationToken);
        
        try
        {
            await _unitOfWork.GuardarCambiosAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (EsViolacionUnicaSql(ex))
        {
            throw new ConflictoException(
                $"Ya existe una operación o póliza activa registrada para la placa {placaNormalizada}.");
        }

        _logger.LogInformation(
            "Póliza {NumeroPoliza} emitida exitosamente para ClienteId {ClienteId} con PrimaTotal {PrimaTotal}",
            poliza.NumeroPoliza,
            poliza.ClienteId,
            poliza.PrimaTotal);

        return new PolizaResponse(
            poliza.Id,
            poliza.NumeroPoliza,
            poliza.FechaEmision,
            poliza.SumaAsegurada,
            poliza.PrimaTotal,
            poliza.Activa,
            new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Identificacion, cliente.Correo),
            new VehiculoResponse(vehiculo.Id, vehiculo.Placa, vehiculo.Marca, vehiculo.Modelo, vehiculo.Anio,
                vehiculo.ValorComercial),
            coberturas.Select(c => new CoberturaAplicadaResponse(c.Id, c.Nombre, c.Tasa)).ToList());
    }

    public async Task<IReadOnlyList<PolizaResponse>> ObtenerHistorialAsync(
        CancellationToken cancellationToken = default)
    {
        var polizas = await _polizaRepository.ObtenerTodasConDetalleAsync(cancellationToken);

        return polizas
            .Select(MapearPolizaAResponse)
            .ToList();
    }

    public async Task<PolizaResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var poliza = await _polizaRepository.ObtenerPorIdConDetalleAsync(id, cancellationToken)
                     ?? throw new NoEncontradoException($"No se encontró la póliza con ID {id}.");

        return MapearPolizaAResponse(poliza);
    }

    public async Task AnularPolizaAsync(int id, CancellationToken cancellationToken = default)
    {
        var poliza = await _polizaRepository.ObtenerPorIdAsync(id, cancellationToken)
                     ?? throw new NoEncontradoException($"No se encontró la póliza con ID {id}.");

        if (!poliza.Activa) throw new ConflictoException($"La póliza {poliza.NumeroPoliza} ya se encuentra anulada.");

        poliza.Activa = false;

        var bitacora = new BitacoraAuditoria
        {
            Fecha = DateTime.UtcNow,
            Usuario = _usuarioActualService.ObtenerUsuarioActual(),
            Accion = "ANULACION_POLIZA",
            Entidad = nameof(Poliza),
            EntidadId = poliza.NumeroPoliza,
            Detalle = $"Anulación lógica de póliza Id={poliza.Id}.",
            CorrelationId = _usuarioActualService.ObtenerCorrelationId(),
            IpOrigen = _usuarioActualService.ObtenerIpOrigen()
        };

        await _bitacoraRepository.RegistrarAsync(bitacora, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        _logger.LogInformation(
            "Póliza {NumeroPoliza} (Id {PolizaId}) anulada exitosamente",
            poliza.NumeroPoliza,
            poliza.Id);
    }

    private async Task<string> GenerarNumeroPolizaAsync(DateTime fechaUtc, CancellationToken cancellationToken)
    {
        DateTime inicioDiaUtc = fechaUtc.Date;
        DateTime finDiaUtc = inicioDiaUtc.AddDays(1);

        int cantidadHoy = await _polizaRepository.ContarEmitidasEnRangoUtcAsync(
            inicioDiaUtc,
            finDiaUtc,
            cancellationToken);

        string prefijo = TextoValidacionHelper.Limpiar(_settings.PrefijoNumeroPoliza).ToUpperInvariant();
        if (string.IsNullOrEmpty(prefijo))
        {
            prefijo = "POL";
        }
        else if (prefijo.Length > 6)
        {
            prefijo = prefijo[..6];
        }

        int correlativo = cantidadHoy + 1;
        string sufijoUnico = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        string numero = $"{prefijo}-{fechaUtc:yyyyMMdd}-{correlativo:D6}-{sufijoUnico}";

        return numero.Length <= 30 ? numero : numero[..30];
    }

    private static PolizaResponse MapearPolizaAResponse(Poliza poliza)
    {
        return new PolizaResponse(
            poliza.Id,
            poliza.NumeroPoliza,
            poliza.FechaEmision,
            poliza.SumaAsegurada,
            poliza.PrimaTotal,
            poliza.Activa,
            new ClienteResponse(
                poliza.Cliente.Id,
                poliza.Cliente.Nombre,
                poliza.Cliente.Identificacion,
                poliza.Cliente.Correo),
            new VehiculoResponse(
                poliza.Vehiculo.Id,
                poliza.Vehiculo.Placa,
                poliza.Vehiculo.Marca,
                poliza.Vehiculo.Modelo,
                poliza.Vehiculo.Anio,
                poliza.Vehiculo.ValorComercial),
            poliza.PolizaCoberturas
                .Select(pc => new CoberturaAplicadaResponse(
                    pc.CoberturaId,
                    pc.Cobertura.Nombre,
                    pc.TasaAplicada))
                .ToList());
    }
    private static bool EsViolacionUnicaSql(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx && sqlEx.Number is 2601 or 2627;
}