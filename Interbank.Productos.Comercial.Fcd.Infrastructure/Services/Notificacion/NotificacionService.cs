using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Notificacion
{
    [ExcludeFromCodeCoverage]
    public class NotificacionService : INotificacionService
    {
        private readonly ICommonService _commonService;
        private readonly ISeguimientoRepository _seguimientoRepository;
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly ILogger<NotificacionService> _logger;

        public NotificacionService(ICommonService commonService, ISeguimientoRepository seguimientoRepository, IUtilitariosRepository utilitariosRepository, ILogger<NotificacionService> logger)
        {
            ArgumentNullException.ThrowIfNull(commonService);
            ArgumentNullException.ThrowIfNull(seguimientoRepository);
            ArgumentNullException.ThrowIfNull(utilitariosRepository);
            ArgumentNullException.ThrowIfNull(logger);

            _commonService = commonService;
            _seguimientoRepository = seguimientoRepository;
            _utilitariosRepository = utilitariosRepository;
            _logger = logger;
        }

        public async Task<int> NotificacionAssi(NotificacionAssiRequest notificacionAssiRequest, decimal idDetalle)
        {
            try
            {
                var respuesta = new BaseResponse();
                var paramNotificacion = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);
                var flagNotificacion = int.Parse(paramNotificacion.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagNotificacionAssi)?.DESCRIPCIONCORTA ?? String.Empty);

                var tipoEjecucion = await _seguimientoRepository.ObtenerTipoEjecucionSeguimientoxIdDetalle(idDetalle);

                if (tipoEjecucion != "P")
                {
                    if (flagNotificacion == (int)TipoNotificacion.NotificacionAssiIFX)
                    {
                        respuesta = await _commonService.NotificacionAssi(notificacionAssiRequest);

                        if (respuesta.CodigoRespuesta == 32)
                        {
                            _logger.LogInformation("IFX ASSI Notifico lo siguiente para la planilla {NumeroPlanilla} : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion} - Evento: {Evento}",
                            notificacionAssiRequest.NumeroPlanilla, notificacionAssiRequest.CodigoRespuesta, notificacionAssiRequest.Descripcion, notificacionAssiRequest.CodigoEvento);
                        }
                    }
                    else
                    {
                        respuesta = await _commonService.NotificacionBackAssi(notificacionAssiRequest);

                        if (respuesta.CodigoRespuesta == 32)
                        {
                            _logger.LogInformation("BACK APIM ASSI Notifico lo siguiente para la planilla {NumeroPlanilla} : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion} - Evento: {Evento}",
                            notificacionAssiRequest.NumeroPlanilla, notificacionAssiRequest.CodigoRespuesta, notificacionAssiRequest.Descripcion, notificacionAssiRequest.CodigoEvento);
                        }
                    }
                }
                else
                {
                    respuesta.CodigoRespuesta = 32;
                    _logger.LogInformation("Notificacion Assi en modo H2H-P para la planilla {NumeroPlanilla}", notificacionAssiRequest.NumeroPlanilla);
                }

                return respuesta.CodigoRespuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el llamado al servicio Apic IFX Notificacion Assi para la planilla {NumeroPlanilla}", notificacionAssiRequest.NumeroPlanilla);
                return 36;
            }
        }
    }
}
