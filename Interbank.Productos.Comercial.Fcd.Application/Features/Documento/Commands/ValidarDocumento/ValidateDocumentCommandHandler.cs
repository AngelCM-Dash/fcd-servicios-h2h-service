using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentCommandHandler : IRequestHandler<ValidateDocumentCommand, BaseResponse>
    {
        private readonly ValidateDocumentDependencies _dependencias;
        private readonly IMapper _mapper;
        private readonly ILogger<ValidateDocumentCommandHandler> _logger;
        decimal CodigoDetalleTraza = 0;

        public ValidateDocumentCommandHandler(ValidateDocumentDependencies deps, IMapper mapper, ILogger<ValidateDocumentCommandHandler> logger)
        {
            _dependencias = deps;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(ValidateDocumentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse
            {
                CodigoRespuesta = 32,
                MensajeRespuesta = "Respuesta correcta"
            };

            CodigoDetalleTraza = await _dependencias.TrazaService.RegistrarCabeceraTraza(Convert.ToDecimal(0), request.NombreArchivo, "", 1);

            if (request.Filtro == 1)
            {
                _logger.LogInformation("1.1- Inicio el Proceso de Documentos Duplicados para el archivo {NombreArchivo} con Codigo Unico {CodigoUnico}", request.NombreArchivo, request.CodigoUnico);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaDesembolsoMasivoI, "Inicia Proceso Validar Documentos Duplicados", 1);
                        await ProcesarDocumentoDuplicado(request);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en ProcesarDocumentoDuplicado: {Message}", ex.Message);
                        await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaDesembolsoMasivoE, $"Error Validar Documentos Duplicados {ex.Message}", 1);
                        await EnviarNotificacion(ReglaDocumentoDuplicados, $"{request.NombreArchivo}|Ocurrio un Error: {ex.Message}", "36");
                    }
                }, cancellationToken);
            }
            else
            {
                _logger.LogInformation("1.1- Inicio el Proceso de Validacion Factura Pendiente a Cargo para el Codigo Unico {CodigoUnico}", request.CodigoUnico);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await VerificarFacturaPendiente(request);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en VerificarFacturaPendiente: {Message}", ex.Message);

                        await EnviarNotificacion(AutoExcepcionFacturasACargo, $"{request.NombreArchivo}|Ocurrio un Error: {ex.Message}", "36");
                    }
                }, cancellationToken);
            }

            return response;
        }

        private async Task ProcesarDocumentoDuplicado(ValidateDocumentCommand request)
        {
            _logger.LogInformation("1.2- Valida si permite Documentos Duplicados para el archivo {NombreArchivo}", request.NombreArchivo);
            await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaValidarPermiteDuplicadosoI, "Valida si permite duplicados", 2);
            var resultPermiteDuplicados = await _dependencias.PlanillasRepository.ValidarPermiteDuplicados(request.CodigoUnico ?? "", request.CodigoProducto);

            if (resultPermiteDuplicados != 1)
            {
                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaValidarPermiteDuplicadosF, $"{request.NombreArchivo}|La afiliación no permite documentos duplicados", 2);
                List<DapperParametro> datosConfig = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpValidacionDocumentos);

                var sftpRemotePathUpload = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenValidacionFacturas.RutaArchivoCargar)?.DESCRIPCIONCORTA;

                var sftpConnectionRequest = _dependencias.ValidarFacturasService.CrearSftpConnectionRequest(datosConfig, request.NombreArchivo ?? "");

                await _dependencias.SftpService.Conectar(sftpConnectionRequest.Host ?? string.Empty, sftpConnectionRequest.Port, sftpConnectionRequest.Username ?? string.Empty, sftpConnectionRequest.Password ?? string.Empty, sftpConnectionRequest.PrivateKeyLocalFilePath ?? string.Empty, Convert.ToBoolean(1));
                await _dependencias.SftpService.ValidarRutaDirectorioRemotaSftp(sftpConnectionRequest.RemotePath ?? string.Empty);
                await _dependencias.SftpService.ValidarArchivoExistenteSftp(sftpConnectionRequest.RemotePath + sftpConnectionRequest.FileName);
                string[] result = await _dependencias.SftpService.LeerLineasArchivoSftp(sftpConnectionRequest.RemotePath + sftpConnectionRequest.FileName);
                await _dependencias.SftpService.Desconectar();

                var codigosecuencia = await _dependencias.PlanillasRepository.GeneraCodigoSecuenciaDocumentosDuplicados();

                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaInsertarDetallePlanillaBulkI, $"Inicia Proceso de Insercion Detalle Planilla en TMP_DOCUMENTOS_DUPLICADOS", 3);

                var dataTable = _dependencias.ValidarFacturasService.InsertarProcesoDetallePlanilla(codigosecuencia, result);

                if (dataTable.Rows.Count > 0)
                {
                    _dependencias.UtilitariosRepository.BulkInsertTable(dataTable, "TMP_DOCUMENTOS_DUPLICADOS");
                }
                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaInsertarDetallePlanillaBulkF, $"Termino Proceso de Insercion Detalle Planilla en TMP_DOCUMENTOS_DUPLICADOS", 3);


                var resultadoDuplicado = await _dependencias.PlanillasRepository.ObtenerDocumentosDuplicados(request.CodigoUnico ?? "", codigosecuencia);
                var documentos = _mapper.Map<List<ValidatePlanillaResponse>>(resultadoDuplicado);

                await GenerarNotificacionDocumentosDuplicados(documentos, request, sftpConnectionRequest, sftpRemotePathUpload ?? string.Empty);
            }
            else
            {
                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaValidarPermiteDuplicadosF, $"{request.NombreArchivo}|La afiliación si permite documentos duplicados", 2);
                _logger.LogInformation("1.3- Fin Notifica que si se permite Documentos duplicados para el archivo {NombreArchivo}", request.NombreArchivo);
                await EnviarNotificacion(ReglaDocumentoDuplicados, $"{request.NombreArchivo}|La afiliación si permite documentos duplicados", "32");
            }
        }

        private async Task VerificarFacturaPendiente(ValidateDocumentCommand request)
        {
            _logger.LogInformation("1.2- Inicio Valida si CodigoUnico {CodigoUnico} tiene Factura Pendiente a Cargo", request.CodigoUnico);
            var resultFacturasPendientes = await _dependencias.PlanillasRepository.ValidarFacturaCargo(request.CodigoUnico ?? "");

            if (resultFacturasPendientes > 0)
            {
                _logger.LogDebug("1.3 - Procede a notificar que el cliente {CodigoUnico} tiene facturas a cargo para el archivo {NombreArchivo}",
                request.CodigoUnico, request.NombreArchivo);
                await EnviarNotificacion(AutoExcepcionFacturasACargo, $"{request.NombreArchivo}|Cliente {request.CodigoUnico} con facturas pendiente a cargo para el archivo ingresado", "38");
            }
            else
            {
                _logger.LogDebug("1.3 - Notifica que el cliente {CodigoUnico} no tiene facturas a cargo para el archivo {NombreArchivo}",
                request.CodigoUnico, request.NombreArchivo);
                await EnviarNotificacion(AutoExcepcionFacturasACargo, $"{request.NombreArchivo}|Cliente {request.CodigoUnico} sin factura pendiente a cargo para el archivo ingresado", "32");
            }
            _logger.LogInformation("1.2- Fin Valida si CodigoUnico {CodigoUnico} tiene Factura Pendiente a Cargo", request.CodigoUnico);
        }

        private async Task GenerarNotificacionDocumentosDuplicados(List<ValidatePlanillaResponse> documentos, ValidateDocumentCommand request, SftpConnectionRequest sftpConfig, string remotePathUpload)
        {
            if (documentos.Count > 0)
            {
                _logger.LogInformation($"1.9- Genera y sube el archivo al SFTP");
                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaGenerarNotificacionDocumentosDuplicadosI, "Inicia Proceso de Generacion de Archivo", 4);

                var resultArchivo = await _dependencias.ValidarFacturasService.GenerarYSubirArchivoTxtAsync(documentos, request.NombreArchivo ?? "", sftpConfig, remotePathUpload);

                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaGenerarNotificacionDocumentosDuplicadosF, $"Proceso de generación de archivo finalizado. Ruta: {resultArchivo}", 4);

                string[] parts = resultArchivo.Split('|');
                string remotePath = parts[0];
                string fileName = parts[1];

                var requestNotificacion = new NotificacionAssiRequest(ReglaDocumentoDuplicados, $"{request.NombreArchivo}|Documentos duplicados para el archivo ingresado", "39", null, null, fileName, remotePath);

                await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaDuplicadosNotificacionAssiI, "Inicia Notificacion Assi", 5);
                var respuesaNotificacion = await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, 0);

                if (respuesaNotificacion == 32)
                {
                    await _dependencias.TrazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaDuplicadosNotificacionAssiI, $"{request.NombreArchivo}|Documentos duplicados para el archivo ingresado en la ruta {remotePath} con nombre {fileName}", 5);
                    _logger.LogInformation("Notifico lo siguiente : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion}",
                    "39", $"{request.NombreArchivo}|Documentos duplicados para el archivo ingresado en la ruta {remotePath} con nombre {fileName}");
                }
            }
            else
            {
                _logger.LogInformation($"1.10- Fin Notifica que no hubo Observaciones");
                await EnviarNotificacion(ReglaDocumentoDuplicados, $"{request.NombreArchivo}|Sin Observaciones", "32");
            }
        }

        private async Task EnviarNotificacion(string codigoEvento, string descripcion, string codigoRespuesta)
        {
            var requestNotificacion = new NotificacionAssiRequest(codigoEvento, descripcion, codigoRespuesta, null, null, null, null);

            var respuesaNotificacion = await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, 0);

            if (respuesaNotificacion == 32)
            {
                _logger.LogInformation("Notifico lo siguiente : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion}",
                codigoRespuesta, descripcion);
            }
        }
    }
}
