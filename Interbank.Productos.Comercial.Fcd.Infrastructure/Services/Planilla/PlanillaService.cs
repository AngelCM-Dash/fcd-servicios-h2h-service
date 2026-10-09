using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Planilla
{
    [ExcludeFromCodeCoverage]
    public class PlanillaService : IPlanillaService
    {
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly ILogger<PlanillaService> _logger;

        public PlanillaService(IUtilitariosRepository utilitariosRepository, ILogger<PlanillaService> logger)
        {
            ArgumentNullException.ThrowIfNull(utilitariosRepository);
            ArgumentNullException.ThrowIfNull(logger);

            _utilitariosRepository = utilitariosRepository;
            _logger = logger;
        }

        public async Task<PlanillaResponse> CargaMasivaPlanilla(CargaMasivaRequest planillaRequest)
        {
            if (planillaRequest == null)
            {
                _logger.LogWarning("El parámetro planillaRequest es nulo en CargaMasivaPlanilla.");
                throw new ArgumentNullException(nameof(planillaRequest));
            }

            _logger.LogInformation("Iniciando CargaMasivaPlanilla.");

            var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioServiciosProcesoCargaMasiva);

            var solicitudUrl = parametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioUrlServiciosEncolamientoCargaMasiva.UrlApiMascara)?.DESCRIPCIONCORTA ?? string.Empty;

            if (string.IsNullOrWhiteSpace(solicitudUrl))
            {
                _logger.LogError("No se encontró la URL de solicitud para CargaMasivaPlanilla.");
                throw new InvalidOperationException("No se encontró la URL de solicitud para CargaMasivaPlanilla.");
            }

            _logger.LogDebug("URL de solicitud para CargaMasivaPlanilla: {SolicitudUrl}", solicitudUrl);

            var response = await HttpClientHelper.EnviarSolicitudHttp<CargaMasivaRequest, PlanillaResponse>(solicitudUrl, planillaRequest);

            _logger.LogInformation("Finalizó CargaMasivaPlanilla.");

            return response;
        }

        public async Task<PlanillaResponse> EncolamientoCargaMasiva(CargaMasivaRequest planillaRequest)
        {
            if (planillaRequest == null)
            {
                _logger.LogWarning("El parámetro planillaRequest es nulo en EncolamientoCargaMasiva.");
                throw new ArgumentNullException(nameof(planillaRequest));
            }

            _logger.LogInformation("Iniciando EncolamientoCargaMasiva.");

            var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioServiciosProcesoCargaMasiva);

            var solicitudUrl = parametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioUrlServiciosEncolamientoCargaMasiva.UrlApiEncolamiento)?.DESCRIPCIONCORTA ?? string.Empty;

            if (string.IsNullOrWhiteSpace(solicitudUrl))
            {
                _logger.LogError("No se encontró la URL de solicitud para EncolamientoCargaMasiva.");
                throw new InvalidOperationException("No se encontró la URL de solicitud para EncolamientoCargaMasiva.");
            }

            _logger.LogDebug("URL de solicitud para EncolamientoCargaMasiva: {SolicitudUrl}", solicitudUrl);

            var response = await HttpClientHelper.EnviarSolicitudHttp<CargaMasivaRequest, PlanillaResponse>(solicitudUrl, planillaRequest);

            _logger.LogInformation("Finalizó EncolamientoCargaMasiva.");

            return response;
        }
    }
}

