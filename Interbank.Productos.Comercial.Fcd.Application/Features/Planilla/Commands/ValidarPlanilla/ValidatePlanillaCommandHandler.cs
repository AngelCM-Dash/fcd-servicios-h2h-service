using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using MediatR;
using Microsoft.Extensions.Logging;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla
{
    public class ValidatePlanillaCommandHandler : IRequestHandler<ValidatePlanillaCommand, string>
    {
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly IPlanillasRepository _planillasRepository;
        private readonly INotificacionService _notificacionService;
        private readonly IValidarFacturasService _validarFacturasService;
        private readonly ISftpService _sftpService;
        private readonly IMapper _mapper;
        private readonly ILogger<ValidatePlanillaCommandHandler> _logger;

        public ValidatePlanillaCommandHandler(IUtilitariosRepository utilitariosRepository, IPlanillasRepository planillasRepository, INotificacionService notificacionService, IValidarFacturasService validarFacturasService, ISftpService sftpService, IMapper mapper, ILogger<ValidatePlanillaCommandHandler> logger)
        {
            _utilitariosRepository = utilitariosRepository;
            _planillasRepository = planillasRepository;
            _notificacionService = notificacionService;
            _validarFacturasService = validarFacturasService;
            _sftpService = sftpService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> Handle(ValidatePlanillaCommand request, CancellationToken cancellationToken)
        {
            string resultado;

            _logger.LogInformation("1.0 - Inicio el Proceso de Documentos Duplicados para el archivo {ArchivoNombre} con Codigo Unico {CodigoUnico}", request.NombreArchivo, request.CodigoUnico);

            try
            {
                var respuesta = await ProcesarDocumentoDuplicado(request);
                resultado = respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                        ex,
                        "Procede a notificar que ocurrió un error al validar los Documentos Duplicados para el archivo {NombreArchivo}",
                        request.NombreArchivo
                        );

                await EnviarNotificacion($"{request.NombreArchivo}|Ocurrió un Error: {ex.Message}", "36");
                return "2";

            }


            _logger.LogInformation("1.1 - Inicio el Proceso de Validacion Factura Pendiente a Cargo para el {CodigoUnico}", request.CodigoUnico);

            try
            {
                var respuesta2 = await VerificarFacturaPendiente(request);
                resultado = resultado + "|" + respuesta2;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                        ex,
                        "Procede a notificar que ocurrió un error al validar las Facturas pendiente a cargo para el archivo {NombreArchivo}",
                        request.NombreArchivo
                    );

                await EnviarNotificacion($"{request.NombreArchivo}|Ocurrio un Error: {ex.Message}", "36");
                return "2";
            }

            return resultado;
        }

        private async Task<string> ProcesarDocumentoDuplicado(ValidatePlanillaCommand request)
        {
            _logger.LogInformation("1.0.1 - Valida si permite Documentos Duplicados para el archivo {NombreArchivo}", request.NombreArchivo);
            var resultPermiteDuplicados = await _planillasRepository.ValidarPermiteDuplicados(request.CodigoUnico ?? "", request.CodigoProducto);

            if (resultPermiteDuplicados != 1)
            {
                _logger.LogInformation("1.0.2 - Obtiene configuración SFTP del dominio {DominioSftpValidacionDocumentos}", ParametroConstants.DominioSftpValidacionDocumentos);
                List<DapperParametro> datosConfig = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpValidacionDocumentos);

                var sftpRemotePathUpload = datosConfig.Find(p => p.NUMEROORDEN == 8)?.DESCRIPCIONCORTA;

                var sftpConnectionRequest = _validarFacturasService.CrearSftpConnectionRequest(datosConfig, request.NombreArchivo ?? "");

                _logger.LogDebug("1.0.3 - Obtiene el archivo {NombreArchivo} con la configuración del SFTP", request.NombreArchivo);

                await _sftpService.Conectar(sftpConnectionRequest.Host ?? string.Empty, sftpConnectionRequest.Port, sftpConnectionRequest.Username ?? string.Empty, sftpConnectionRequest.Password ?? string.Empty, sftpConnectionRequest.PrivateKeyLocalFilePath ?? string.Empty, Convert.ToBoolean(1));
                await _sftpService.ValidarRutaDirectorioRemotaSftp(sftpConnectionRequest.RemotePath ?? string.Empty);
                await _sftpService.ValidarArchivoExistenteSftp(sftpConnectionRequest.RemotePath + sftpConnectionRequest.FileName);
                string[] result = await _sftpService.LeerLineasArchivoSftp(sftpConnectionRequest.RemotePath + sftpConnectionRequest.FileName);
                await _sftpService.Desconectar();

                _logger.LogDebug("1.0.4 - Genera código de secuencia para el archivo {NombreArchivo}", request.NombreArchivo);
                var codigosecuencia = await _planillasRepository.GeneraCodigoSecuenciaDocumentosDuplicados();

                _logger.LogDebug("1.0.5 - Inserta información del archivo {NombreArchivo} en la base de datos", request.NombreArchivo);
                var dataTable = _validarFacturasService.InsertarProcesoDetallePlanilla(codigosecuencia, result);

                if (dataTable.Rows.Count > 0)
                {
                    _utilitariosRepository.BulkInsertTable(dataTable, "TMP_DOCUMENTOS_DUPLICADOS");
                }

                _logger.LogDebug("1.0.6 - Obtiene los documentos duplicados del archivo {NombreArchivo}", request.NombreArchivo);
                var resultadoDuplicado = await _planillasRepository.ObtenerDocumentosDuplicados(request.CodigoUnico ?? "", codigosecuencia);
                var documentos = _mapper.Map<List<ValidatePlanillaResponse>>(resultadoDuplicado);

                _logger.LogInformation("1.0.7 - Genera y sube el archivo de respuesta al SFTP");

                if (documentos.Count > 0)
                {
                    _logger.LogInformation("1.0.8 - Inicio Proceso de Notificacion");
                    await GenerarNotificacionDocumentosDuplicados(documentos, request, sftpConnectionRequest, sftpRemotePathUpload ?? string.Empty);
                    return "1";
                }
                else
                {
                    _logger.LogInformation("1.0.8 - No hubo observacion de duplicidad de documentos para el archivo {NombreArchivo}", request.NombreArchivo);
                    return "0";
                }
            }
            else
            {
                _logger.LogInformation("1.0.2 - La afiliación si permite documentos duplicados para el archivo ingresado {NombreArchivo}", request.NombreArchivo);
                return "0";
            }
        }


        private async Task<string> VerificarFacturaPendiente(ValidatePlanillaCommand request)
        {
            _logger.LogInformation("1.1.1 - Valida si CodigoUnico {CodigoUnico} tiene facturas a cargo", request.CodigoUnico);
            var resultFacturasPendientes = await _planillasRepository.ValidarFacturaCargo(request.CodigoUnico ?? "");

            _logger.LogInformation("1.1.2 - CodigoUnico {CodigoUnico} cuenta con {FacturasPendientes} facturas a cargo", request.CodigoUnico, resultFacturasPendientes);

            if (resultFacturasPendientes > 0)
            {
                _logger.LogInformation("1.1.3 - Procede a notificar que el cliente {CodigoUnico} tiene facturas a cargo para el archivo {NombreArchivo}",
                request.CodigoUnico, request.NombreArchivo);
                await EnviarNotificacion($"{request.NombreArchivo}|Cliente {request.CodigoUnico} con facturas pendiente a cargo para el archivo ingresado", "38");
                return "1";
            }
            else
            {
                _logger.LogInformation("1.1.3 - Notifica que el cliente {CodigoUnico} no tiene facturas a cargo para el archivo {NombreArchivo}",
                request.CodigoUnico, request.NombreArchivo);
                return "0";
            }
        }

        private async Task GenerarNotificacionDocumentosDuplicados(List<ValidatePlanillaResponse> documentos, ValidatePlanillaCommand request, SftpConnectionRequest sftpConfig, string remotePathUpload)
        {
            _logger.LogInformation($"1.0.8.1 - Procede a generar archivo txt de documentos duplicados");
            var resultArchivo = await _validarFacturasService.GenerarYSubirArchivoTxtAsync(documentos, request.NombreArchivo ?? "", sftpConfig, remotePathUpload);

            _logger.LogDebug("1.0.8.2 - Genero archivo txt en la ruta {ResultArchivo}", resultArchivo);


            string[] parts = resultArchivo.Split('|');
            string remotePath = parts[0];
            string fileName = parts[1];

            var requestNotificacion = new NotificacionAssiRequest(CargaMasivaPlanilla, $"{request.NombreArchivo}|Documentos duplicados para el archivo ingresado", "39", null, null, fileName, remotePath);

            _logger.LogDebug("1.0.8.3 - Procede a notificar los documentos duplicados para el archivo {NombreArchivo}", request.NombreArchivo);
            var respuesaNotificacion = await _notificacionService.NotificacionAssi(requestNotificacion, 0);

            if (respuesaNotificacion == 32)
            {
                _logger.LogInformation("Fin Notifico lo siguiente : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion} | Documentos duplicados para el archivo ingresado - NombreArchivoDocumentoDuplicado: {NombreArchivoDocumentoDuplicado} - RutaArchivo : {RutaArchivo}",
                39, request.NombreArchivo, fileName, remotePath);
            }
        }

        private async Task EnviarNotificacion(string descripcion, string codigoRespuesta)
        {
            var requestNotificacion = new NotificacionAssiRequest(CargaMasivaPlanilla, descripcion, codigoRespuesta, null, null, null, null);

            var respuesaNotificacion = await _notificacionService.NotificacionAssi(requestNotificacion, 0);

            if (respuesaNotificacion == 32)
            {
                _logger.LogInformation("Notifico lo siguiente : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion}",
                codigoRespuesta, descripcion);
            }
        }
    }
}
