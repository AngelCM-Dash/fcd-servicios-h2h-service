using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor
{
    //[ExcludeFromCodeCoverage]
    public class ProcesarTramasHandler : IRequestHandler<ProcesarTramasCommand, ProcesarTramasResponse>
    {
        private readonly ILogger<ProcesarTramasHandler> _logger;
        private readonly ProcesarTramasDependencies _dependencias;
        private readonly ITrazaService _trazaService;
        private readonly ISftpService _sftpService;
        public ProcesarTramasHandler(ILogger<ProcesarTramasHandler> logger, ProcesarTramasDependencies dependencies, ITrazaService trazaService, ISftpService sftpService)
        {
            _logger = logger;
            _dependencias = dependencies;
            _trazaService = trazaService;
            _sftpService = sftpService;
        }

        public Task<ProcesarTramasResponse> Handle(ProcesarTramasCommand request, CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                _logger.LogInformation("1.0.- Inicio - Flujo Monitor Procesamiento de Tramas para la planilla: {NroPlanilla}", request.NumeroPlanilla);

                var queryDb2Service = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(1, 0);
                var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);
                var response = await ProcesarPlanillas(request, queryDb2Service, configuracionDb2);

                return response;
            });

            var resultado = new ProcesarTramasResponse()
            {
                CodigoRespuesta = "00",
                MensajeRespuesta = "OK"
            };

            return Task.FromResult(resultado);
        }

        protected async Task<ProcesarTramasResponse> ProcesarPlanillas(ProcesarTramasCommand request, List<DapperParametroDB2> queryDb2Service, List<DapperParametro> configuracionDb2)
        {
            _logger.LogInformation("2.0.- Obtener las Planillas Procesadas");

            if (!String.IsNullOrEmpty(request.NumeroPlanilla) && request.NumeroSecuencia != 0 || request.TipoProcesamiento == (int)TipoReintentoProcesoAbono.Total)
            {
                await ProcesarPlanilla(request, queryDb2Service, configuracionDb2);

                _logger.LogDebug("1.0.- Fin - Flujo Monitor Procesamiento de Tramas - Planillas procesadas correctamente");
                return new ProcesarTramasResponse
                {
                    CodigoRespuesta = "00",
                    MensajeRespuesta = "OK"
                };
            }
            else if (request.TipoProcesamiento == (int)TipoReintentoProcesoAbono.ParcialTotal || request.TipoProcesamiento == (int)TipoReintentoProcesoAbono.Parcial)
            {
                var queryConsultaPasoMasi = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(1, 2);
                var obtenerPlanillasProcesoAbono = await _dependencias.MonitorService.ObtenerDetallePlanillasProcesadasDB2(queryConsultaPasoMasi, configuracionDb2, request.NumeroPlanilla ?? "", request.NumeroSecuencia.ToString());
                var desembolsos = await _dependencias.DesembolsoRepository.ObtenerDesembolsoAbono(request.NumeroPlanilla ?? string.Empty, 0, string.Empty);

                var secuenciasPlanillas = obtenerPlanillasProcesoAbono
                                        .Select(x => x.NumeroSecuenciaPlanilla)
                                        .Distinct()
                                        .ToList();

                var secuenciasDesembolsos = desembolsos
                                            .Select(x => x.NumeroSecuencia)
                                            .ToHashSet();

                var secuenciasFaltantes = secuenciasDesembolsos
                    .Where(seq => !secuenciasPlanillas.Contains(seq))
                    .ToList();

                foreach (var strPlanillaSeq in secuenciasFaltantes)
                {
                    await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(
                        request.NumeroPlanilla ?? string.Empty,
                        strPlanillaSeq,
                        string.Empty,
                        (int)EstadoReintentoProcesoAbono.NoRegistradoDb2,
                        99
                    );
                }

                var secuenciasOrdenadasProcesoAbono = obtenerPlanillasProcesoAbono
                    .GroupBy(x => x.NumeroSecuenciaPlanilla)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        NumeroSecuencia = g.Key,
                        NumeroPlanilla = g.First().NumeroPlanilla
                    })
                    .ToList();

                foreach (var item in secuenciasOrdenadasProcesoAbono)
                {
                    request.NumeroPlanilla = item.NumeroPlanilla;
                    request.NumeroSecuencia = item.NumeroSecuencia;

                    await ProcesarPlanilla(request, queryDb2Service, configuracionDb2);
                    _logger.LogDebug("1.0.- Fin - Flujo Monitor Procesamiento de Tramas - Planillas procesadas correctamente");
                }

            }
            else
            {
                var listaPlanillaProcesadas = await _dependencias.MonitorService.ObtenerPlanillasProcesadasDB2(queryDb2Service, configuracionDb2);

                foreach (var item in listaPlanillaProcesadas)
                {
                    request.NumeroPlanilla = item.numeroPlanilla;
                    request.NumeroSecuencia = item.numeroSecuenciaPlanilla;

                    await ProcesarPlanilla(request, queryDb2Service, configuracionDb2);

                    _logger.LogDebug("1.0.- Fin - Flujo Monitor Procesamiento de Tramas - Planillas procesadas correctamente");
                }

                return new ProcesarTramasResponse
                {
                    CodigoRespuesta = "00",
                    MensajeRespuesta = "OK"
                };
            }

            _logger.LogInformation("1.0.- Fin - Flujo Monitor Procesamiento de Tramas - No hay planillas para procesar");
            return new ProcesarTramasResponse
            {
                CodigoRespuesta = "01",
                MensajeRespuesta = "No hay planillas para procesar"
            };
        }

        protected virtual async Task ProcesarPlanilla(ProcesarTramasCommand request, List<DapperParametroDB2> queryDb2Service, List<DapperParametro> configuracionDb2)
        {
            var idCabeceraSeguimiento = await _dependencias.SeguimientoRepository.ObtenerIdSeguimientoxPlanilla(request.NumeroPlanilla ?? "");
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarPlanillasI, $"Inicia Proceso Procesar Planillas", 1);

            var flagPrueba = int.Parse(configuracionDb2.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagPruebaMonitor)?.DESCRIPCIONCORTA ?? String.Empty);

            if (flagPrueba != 0)
            {
                _logger.LogInformation("2.1.3.- El Detalle de la planilla {NumeroPlanilla} con secuencia {NumeroSecuencia} no tiene detalle (trama retorno)", request.NumeroPlanilla, request.NumeroSecuencia);
                await ManejarErrorPlanilla(
                        request.NumeroPlanilla ?? "",
                        request.NumeroSecuencia,
                        ProcesarTramasConstants.ErrorRegistrarDetallePlanilla,
                        idCabeceraSeguimiento
                        );

                throw new ThrowException("36", ProcesarTramasConstants.ErrorRegistrarDetallePlanilla);
            }

            var strNumeroPlanilla = request.NumeroPlanilla ?? "";
            var strPlanillaSeq = request.NumeroSecuencia;
            var strUsuario = configuracionDb2.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.UsuarioDb2)?.DESCRIPCIONCORTA ?? "";
            try
            {
                await _dependencias.PlanillasRepository.ObtenerInformacionPlanilla(strNumeroPlanilla);

                _logger.LogDebug("2.1.- Inicio de Proceso para la Planilla {NumeroPlanilla} con Secuencia {PlanillaSeq}", strNumeroPlanilla, strPlanillaSeq);

                _logger.LogDebug("2.1.1.- Obtener los detalles de las Planillas");
                List<DetallePlanillasProcesadasResponse> detallesPlanillas = new List<DetallePlanillasProcesadasResponse>();

                try
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDetallePlanillasProcesadasDB2I, $"Inicia Proceso Obtener Detalle Planilla Procesada", 2);
                    detallesPlanillas = await _dependencias.MonitorService.ObtenerDetallePlanillasProcesadasDB2(queryDb2Service, configuracionDb2, strNumeroPlanilla, strPlanillaSeq.ToString());
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDetallePlanillasProcesadasDB2F, $"Termino Correctamente Proceso Obtener Detalle Planilla Procesada", 2);
                }
                catch (Exception ex)
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDetallePlanillasProcesadasDB2E, $"{ProcesarTramasConstants.ErrorObtenerDetallePlanillaDb2}: {ex.ToString()}", 2);
                    _logger.LogError(
                      ex,
                      "2.1.3 - La planilla {NumeroPlanilla} con secuencia {NumeroSecuencia} no tiene detalle en la trama de retorno",
                      request.NumeroPlanilla,
                      request.NumeroSecuencia
                  );
                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    ProcesarTramasConstants.ErrorObtenerDetallePlanillaDb2,
                    idCabeceraSeguimiento
                    );
                }

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaInsertarProcesoDetallePlanillaI, $"Inicia Proceso Insertar Proceso Detalle Planilla", 3);
                if (InsertarProcesoDetallePlanilla(detallesPlanillas))
                {
                    await RegistrarTramasProcesadas(strNumeroPlanilla, strPlanillaSeq, strUsuario, queryDb2Service, configuracionDb2, idCabeceraSeguimiento, detallesPlanillas);
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDetallePlanillasProcesadasDB2E, ProcesarTramasConstants.ErrorRegistrarDetallePlanilla, 3);
                    _logger.LogDebug("2.1.4.- Error al registar el detalle de la planilla {NumeroPlanilla} con secuencia {NumeroSecuencia}", request.NumeroPlanilla, request.NumeroSecuencia);

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    ProcesarTramasConstants.ErrorRegistrarDetallePlanilla,
                    idCabeceraSeguimiento
                    );
                }

                _logger.LogDebug("2.1.- Termino de Proceso para la Planilla {NumeroPlanilla}", strNumeroPlanilla);
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarPlanillasF, $"Termino Correctamente Proceso Procesar Planillas", 1);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarPlanillasE, $"Ocurrió un Error: {ex.ToString()}", 0);
                _logger.LogError(
                        ex,
                        "Error al procesar la planilla {NumeroPlanilla}",
                        strNumeroPlanilla
                    );

            }
        }

        protected virtual async Task RegistrarTramasProcesadas(string strNumeroPlanilla, int strPlanillaSeq, string strUsuario, List<DapperParametroDB2> queryDb2Service, List<DapperParametro> configuracionDb2, int idCabeceraSeguimiento, List<DetallePlanillasProcesadasResponse> detallePlanillas)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarTramasProcesadasI, $"Inicia Proceso Registrar Tramas Procesadas", 4);
            _logger.LogInformation("2.1.3.2.- Registrar Tramas Procesadas");
            var strResultadoTMP = await _dependencias.DesembolsoRepository.RegistrarTramasProcesadadas(strNumeroPlanilla, strPlanillaSeq, strUsuario);
            _logger.LogDebug("2.1.3.2.- {RespuestaMensaje}", strResultadoTMP.RespuestaMensaje);
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarTramasProcesadasF, $"Termino Correctamente Registro Tramas Procesadas codigo de Respuesta: {strResultadoTMP.RespuestaCodigo}", 4);
            if (strResultadoTMP.RespuestaCodigo == 1)
            {
                await RegistrarMovimientosPlanillaProcesada(strNumeroPlanilla, strPlanillaSeq, strUsuario, queryDb2Service, configuracionDb2, idCabeceraSeguimiento, detallePlanillas);
            }
            else
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarTramasProcesadasE, ProcesarTramasConstants.ErrorRegistroTramas, 4);
                _logger.LogDebug("2.1.3.2.- Error en registras las Tramas Procesadas para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    ProcesarTramasConstants.ErrorRegistroTramas,
                    idCabeceraSeguimiento
                );
            }
        }

        protected virtual async Task RegistrarMovimientosPlanillaProcesada(string strNumeroPlanilla, int strPlanillaSeq, string strUsuario, List<DapperParametroDB2> queryDb2Service, List<DapperParametro> configuracionDb2, int idCabeceraSeguimiento, List<DetallePlanillasProcesadasResponse> detallePlanillas)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosPlanillaProcesadaI, $"Inicia Proceso Registrar Movimientos Planilla Procesada", 5);
            _logger.LogInformation("2.1.3.3.1.- Registrar Movimientos de la documentos de la planilla Procesada");
            var strResultadoTMP2 = await _dependencias.DesembolsoRepository.RegistrarMovimientos(strNumeroPlanilla, strPlanillaSeq, strUsuario, string.Empty);
            _logger.LogDebug("2.1.3.3.1.- {RespuestaMensaje}", strResultadoTMP2.RespuestaMensaje);
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosPlanillaProcesadaF, $"Termino Correctamente Proceso Registrar Movimientos Planilla Procesada codigo de Respuesta: {strResultadoTMP2.RespuestaCodigo}", 5);

            if (strResultadoTMP2.RespuestaCodigo == 1)
            {
                _logger.LogDebug("2.1.3.3.2.- Actualizar el estado en DB2 de las planillas procesadas");
                var strResultadoTMP3 = string.Empty;
                try
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizarPlanillasProcesadasDB2I, $"Inicia Proceso Actualizacion de Estado DB2", 6);
                    strResultadoTMP3 = await _dependencias.MonitorService.ActualizarPlanillasProcesadasDB2(queryDb2Service, configuracionDb2, strNumeroPlanilla, strPlanillaSeq.ToString(), 10);
                    _logger.LogDebug("2.1.3.3.2.- {Resultado}", strResultadoTMP3);
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizarPlanillasProcesadasDB2F, $"Termino Correctamente Actualizacion de Estado DB2", 6);
                }
                catch (Exception ex)
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizarPlanillasProcesadasDB2E, $"ERROR AL ACTUALIZAR EL ESTADO DE LA PLANILLA EN DB2: {ex.ToString()}", 6);
                    _logger.LogError(
                        ex,
                        "2.1.3.3.2.- Error al Actualizar el estado en DB2 para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}",
                        strNumeroPlanilla,
                        strPlanillaSeq
                    );

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2,
                    idCabeceraSeguimiento
                    );
                }

                var resultadoActualizacionDb2 = strResultadoTMP3.Split("|");
                if (resultadoActualizacionDb2[0] == "1")
                {
                    var procesados = detallePlanillas.Where(x => x.CodigoEstadoPago == "09").ToList();

                    if (procesados.Count > 0)
                    {
                        await RegistrarMovimientosEnLineasWBC(strNumeroPlanilla, strPlanillaSeq, idCabeceraSeguimiento, detallePlanillas);
                    }
                    else
                    {
                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosConsumoLineaE, $"Genera Archivo Plano por Procesadas igual a 0", 6);

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerReservasPlanillasI, $"Inicia Proceso Liberacion Reserva de Lineas", 9);
                        var resultadoLiberacion = await EliminaReservaDistribuido(idCabeceraSeguimiento, strNumeroPlanilla ?? string.Empty);

                        if (!string.IsNullOrWhiteSpace(resultadoLiberacion))
                        {
                            string mensajeErrorLiberacion;
                            mensajeErrorLiberacion = $"{resultadoLiberacion}";

                            var requestNotificacionLiberacion = new NotificacionAssiRequest(LiberacionReserva, mensajeErrorLiberacion, "36", strNumeroPlanilla, null, null, null);

                            await _dependencias.NotificacionService.NotificacionAssi(requestNotificacionLiberacion, idCabeceraSeguimiento);
                        }

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerReservasPlanillasI, $"Fin Proceso Liberacion Reserva de Lineas", 9);

                        _logger.LogDebug($"2.1.3.3.5.- Genera Archivo Plano");
                        var resultArchivo = await GenerarArchivoPlano(strNumeroPlanilla, ProcesarTramasConstants.DesembolsoOK, 2, idCabeceraSeguimiento);
                        string[] parts = resultArchivo.Split('|');

                        string remotePath = parts[0];
                        string fileName = parts[1];

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorI, $"Inicia Proceso Notificacion Assi", 14);

                        var request = new NotificacionAssiRequest(ConfirmacionDesembolso, "OK", "32", strNumeroPlanilla, null, fileName, remotePath);

                        int respuesaNotificacion = 0;

                        try
                        {
                            respuesaNotificacion = await _dependencias.NotificacionService.NotificacionAssi(request, idCabeceraSeguimiento);

                            if (respuesaNotificacion == 32)
                            {
                                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorF, $"Termino Correctamente Notificacion Assi", 14);
                            }
                            else
                            {
                                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorE, $"Error no se pudo completar el proceso de Notificacion Assi", 14);
                            }
                        }
                        catch (Exception ex)
                        {
                            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorE, $"Error no se pudo completar el proceso de Notificacion Assi : {ex.ToString()}", 14);
                        }
                    }
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizarPlanillasProcesadasDB2E, ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2, 6);
                    _logger.LogDebug("2.1.3.3.2.- Error al Actualizar el estado en DB2 para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2,
                    idCabeceraSeguimiento
                    );
                }
            }
            else
            {
                _logger.LogDebug("2.1.3.3.1.- Error en el registró del Movimiento para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                await ManejarErrorPlanilla(
                strNumeroPlanilla,
                strPlanillaSeq,
                ProcesarTramasConstants.ErrorRegistrarMovimientoDocumentos,
                    idCabeceraSeguimiento
                );
            }
        }

        private async Task ValidarErroresMonitor(int existemensajePlanilla, string strNumeroPlanilla, int strPlanillaSeq, int idCabeceraSeguimiento)
        {
            if (existemensajePlanilla == 0)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarPlanillaDietarioI, $"Inicia Valida Procesamiento Trama", 12);
                var estadoTrama = await validaProcesamientoTrama(strNumeroPlanilla ?? string.Empty);

                if (estadoTrama)
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarPlanillaDietarioI, $"Inicia Proceso Registrar Planilla Dietario", 12);
                    var resultInsDietario = await _dependencias.MonitorService.EnvioCorreoDietarios(strNumeroPlanilla ?? string.Empty, string.Empty);
                    if (resultInsDietario.MensajeRespuesta == "true")
                    {
                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerReservasPlanillasI, $"Inicia Proceso Liberacion Reserva de Lineas", 9);
                        var resultadoLiberacion = await EliminaReservaDistribuido(idCabeceraSeguimiento, strNumeroPlanilla ?? string.Empty);

                        if (!string.IsNullOrWhiteSpace(resultadoLiberacion))
                        {
                            string mensajeErrorLiberacion;
                            mensajeErrorLiberacion = $"{resultadoLiberacion}";

                            var requestNotificacionLiberacion = new NotificacionAssiRequest(LiberacionReserva, mensajeErrorLiberacion, "36", strNumeroPlanilla, null, null, null);

                            await _dependencias.NotificacionService.NotificacionAssi(requestNotificacionLiberacion, idCabeceraSeguimiento);
                        }

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerReservasPlanillasI, $"Fin Proceso Liberacion Reserva de Lineas", 9);

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarPlanillaDietarioF, $"Termino Correctamente Envio Planilla Dietario", 12);
                        _logger.LogDebug("Envio correo dietario para la planilla de Distribuido {NumeroPlanilla}", strNumeroPlanilla);
                    }
                    else
                    {
                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarPlanillaDietarioE, $"Error en el Proceso Envio Planilla Dietario", 12);
                        _logger.LogDebug("No envio correo dietario dietario para la planilla de Distribuido {NumeroPlanilla}", strNumeroPlanilla);

                        await ManejarErrorPlanilla(
                        strNumeroPlanilla ?? "",
                        strPlanillaSeq,
                        ProcesarTramasConstants.ErrorEnivarCorreoDietarios,
                        idCabeceraSeguimiento
                        );

                    }

                    _logger.LogDebug($"2.1.3.3.5.- Genera Archivo Plano");
                    var resultArchivo = await GenerarArchivoPlano(strNumeroPlanilla, ProcesarTramasConstants.DesembolsoOK, 2, idCabeceraSeguimiento);
                    string[] parts = resultArchivo.Split('|');

                    string remotePath = parts[0];
                    string fileName = parts[1];

                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorI, $"Inicia Proceso Notificacion Assi", 14);

                    var request = new NotificacionAssiRequest(ConfirmacionDesembolso, "OK", "32", strNumeroPlanilla, null, fileName, remotePath);

                    int respuesaNotificacion = 0;

                    try
                    {
                        respuesaNotificacion = await _dependencias.NotificacionService.NotificacionAssi(request, idCabeceraSeguimiento);

                        if (respuesaNotificacion == 32)
                        {
                            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorF, $"Termino Correctamente Notificacion Assi", 14);
                        }
                        else
                        {
                            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorE, $"Error no se pudo completar el proceso de Notificacion Assi", 14);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorE, $"Error no se pudo completar el proceso de Notificacion Assi : {ex.ToString()}", 14);
                    }
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarPlanillaDietarioI, $"Valida Procesamiento Trama devolvio {estadoTrama}", 8);
                }
            }
        }

        private async Task RegistrarMovimientosEnLineasWBC(string strNumeroPlanilla, int strPlanillaSeq, int idCabeceraSeguimiento, List<DetallePlanillasProcesadasResponse> detallePlanillas)
        {
            _logger.LogInformation($"2.1.3.3.3.- Inicio registrar el movimiento en Lineas WBC");
            _logger.LogDebug($"2.1.3.3.4.- Obtiene Datos del Desembolso para el consumo de lineas QRY02");
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosConsumoLineaI, $"Inicia Proceso Obtener Datos Desembolso para Movimientos En Lineas WBC QRY02", 7);
            var datoDesembolso_02 = await _dependencias.DesembolsoRepository.ObtenerDatosDesembolso(strNumeroPlanilla, strPlanillaSeq, "QRY02");
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosConsumoLineaF, $"Termino Correctamente Obtener Datos Desembolso para Movimientos En Lineas WBC QRY02", 7);

            if (datoDesembolso_02.Count > 0)
            {
                _logger.LogDebug($"2.1.3.3.4.1- Se registran los movimientos para el consumo de lineas QRY02");
                var resultMov = await RegistrarMovimiento(datoDesembolso_02, idCabeceraSeguimiento);

                if (resultMov.CodigoRespuesta != "32")
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosEnLineasWBCE, $"Error en el Proceso Registrar Movimientos En Lineas WBC", 8);
                    _logger.LogDebug("2.1.3.3.4.1.- Error en el consumo de Lineas para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    "ERROR EN EL CONSUMO DE LINEAS",
                    idCabeceraSeguimiento
                    );
                }
                else
                {
                    await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(strNumeroPlanilla, strPlanillaSeq, "" ?? String.Empty, (int)EstadoReintentoProcesoAbono.Procesado, 99);
                }

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerMensajeErrorMonitorPorPlanillaI, $"Inicia Proceso Validar Planilla con observación", 11);
                var existemensajePlanilla = await _dependencias.DesembolsoRepository.ObtenerMensajeErrorMonitorPorPlanilla(strNumeroPlanilla ?? string.Empty);
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerMensajeErrorMonitorPorPlanillaF, $"Termino Proceso Validar Planilla, observaciones encontradas : {existemensajePlanilla}", 11);

                await ValidarErroresMonitor(existemensajePlanilla, strNumeroPlanilla ?? string.Empty, strPlanillaSeq, idCabeceraSeguimiento);
            }
            else
            {
                var procesados = detallePlanillas.Where(x => x.CodigoEstadoPago == "09").ToList();

                if (procesados.Count > 0)
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosConsumoLineaE, $"Error al obtener informacion para el consumo de Lineas", 7);
                    _logger.LogDebug("2.1.3.3.4.- Error al obtener informacion para el consumo de Lineas para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    "ERROR AL OBTENER INFORMACION PARA EL CONSUMO DE LINEAS",
                    idCabeceraSeguimiento
                    );
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosConsumoLineaE, $"Error al realizar el consumo de Lineas", 7);
                    _logger.LogDebug("2.1.3.3.4.- Error al realizar el consumo de Lineas para la Planilla {NumeroPlanilla} con Secuencia {NumeroSecuencia}", strNumeroPlanilla, strPlanillaSeq);

                    await ManejarErrorPlanilla(
                    strNumeroPlanilla,
                    strPlanillaSeq,
                    "ERROR EN EL PROCESO DE CONSUMO DE LINEAS",
                    idCabeceraSeguimiento
                    );
                }
            }

            _logger.LogDebug($"2.1.3.3.6.- Obtiene Datos del Desembolso para el consumo de lineas QRY08");

            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosDesembolsoI, $"Inicia Proceso Obtener Datos Desembolso para Movimientos En Lineas WBC QRY08", 15);
            var datoDesembolso_08 = await _dependencias.DesembolsoRepository.ObtenerDatosDesembolso(strNumeroPlanilla ?? string.Empty, strPlanillaSeq, "QRY08");
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaObtenerDatosDesembolsoF, $"Termino Correctamente Obtener Datos Desembolso para Movimientos En Lineas WBC QRY08", 15);

            if (datoDesembolso_08.Any(x => !string.IsNullOrWhiteSpace(x.NumeroOperacion)))
            {
                _logger.LogDebug($"2.1.3.3.6.1- Se registran los movimientos para el consumo de lineas QRY08");
                await RegistrarMovimiento(datoDesembolso_08, idCabeceraSeguimiento);
            }
            _logger.LogInformation($"2.1.3.3.3.- Fin de registro de movimientos en Lineas WBC");
        }

        private async Task<bool> validaProcesamientoTrama(string strNumeroPlanilla)
        {
            var desembolsos = await _dependencias.DesembolsoRepository.ObtenerDesembolsoAbono(strNumeroPlanilla, 0, string.Empty);

            if (desembolsos == null || desembolsos.Count == 0)
                return false;

            var secuenciasUnicas = desembolsos
                .Where(d => d.EstadoReintento != (int)EstadoReintentoProcesoAbono.NoRegistradoDb2 && d.EstadoReintento != (int)EstadoReintentoProcesoAbono.FalloGeneracionTrama)
                .Select(d => d.NumeroSecuencia)
                .Distinct()
                .ToList();

            bool todasProcesadas = desembolsos
                .Where(d => d.NumeroPlanilla == strNumeroPlanilla && secuenciasUnicas.Contains(d.NumeroSecuencia))
                .All(d => d.EstadoReintento == (int)EstadoReintentoProcesoAbono.Procesado);

            return todasProcesadas;
        }

        private async Task<MovimientoResponse> RegistrarMovimiento(List<DapperConsumoLinea> datosDesembolso, int idCabeceraSeguimiento)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosEnLineasWBCI, $"Inicia Proceso Registrar Movimientos En Lineas WBC", 8);
                var movimientoRequest = new MovimientoRequest
                {
                    NumeroLinea = datosDesembolso.Select(d => d.NumeroLinea ?? "").ToList(),
                    Tipo = datosDesembolso.Select(d => d.Tipo ?? "").ToList(),
                    Monto = datosDesembolso.Select(d => d.Neteo).ToList(),
                    CodigoMoneda = datosDesembolso.Select(d => d.CodigoMonedaWBC).ToList(),
                    NumeroOperacion = datosDesembolso.Select(d => d.NumeroOperacion ?? "").ToList(),
                    SaldoOperacion = datosDesembolso.Select(d => d.Neteo).ToList()
                };

                var registermovimiento = await _dependencias.MonitorService.ConsumoLineaMovimiento(movimientoRequest);

                if (registermovimiento.CodigoRespuesta == "32")
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosEnLineasWBCF, $"Termino Correctamente Proceso Registrar Movimientos En Lineas WBC", 8);
                    _logger.LogInformation("Se registró satisfactoriamente el movimiento en Lineas WBC");
                    return registermovimiento;
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosEnLineasWBCE, $"Error en el Proceso Registrar Movimientos En Lineas WBC", 8);
                    _logger.LogInformation("No se registró el movimiento en Lineas WBC");
                    return new MovimientoResponse()
                    {
                        CodigoRespuesta = "36",
                        MensajeRespuesta = "Error en el registro",
                        Movimientos = registermovimiento.Movimientos
                    };
                }
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistrarMovimientosEnLineasWBCE, $"Error en el Proceso Registrar Movimientos En Lineas WBC : {ex.Message}", 8);
                return new MovimientoResponse()
                {
                    CodigoRespuesta = "36",
                    MensajeRespuesta = "Error en el registro",
                    Movimientos = new List<MovimientoResponseBody>()
                };
            }
        }

        protected bool InsertarProcesoDetallePlanilla(IEnumerable<DetallePlanillasProcesadasResponse> detallesPlanilla)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("NumeroPlanilla", typeof(string));
            dataTable.Columns.Add("NumeroSecuenciaPlanilla", typeof(string));
            dataTable.Columns.Add("NumeroOperacion", typeof(string));
            dataTable.Columns.Add("NumeroSecuenciaOperacion", typeof(string));
            dataTable.Columns.Add("DescripcionMensajeError", typeof(string));
            dataTable.Columns.Add("NumeroDocumento", typeof(string));
            dataTable.Columns.Add("ImporteDesembolso", typeof(string));
            dataTable.Columns.Add("CodigoFlagExterno", typeof(string));
            dataTable.Columns.Add("NumeroLogExterno", typeof(string));
            dataTable.Columns.Add("ImporteComisionCCI", typeof(string));
            dataTable.Columns.Add("ImporteComisionIB", typeof(string));
            dataTable.Columns.Add("ImporteDesembolsoCCI", typeof(string));
            dataTable.Columns.Add("CodigoRetorno", typeof(string));

            foreach (var rowDetalle in detallesPlanilla)
            {
                if (rowDetalle == null) continue;

                DataRow dataRow = dataTable.NewRow();
                dataRow["NumeroPlanilla"] = rowDetalle.NumeroPlanilla?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["NumeroSecuenciaPlanilla"] = rowDetalle.NumeroSecuenciaPlanilla.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["NumeroOperacion"] = rowDetalle.NumeroOperacion?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["NumeroSecuenciaOperacion"] = rowDetalle.NumeroSecuenciaOperacion?.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["DescripcionMensajeError"] = fstrFormatString(rowDetalle.DescripcionMensajeError ?? string.Empty).Trim().PadRight(40, ' ').Substring(0, 40);
                dataRow["NumeroDocumento"] = rowDetalle.NumeroDocumento?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["ImporteDesembolso"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteDesembolso?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["CodigoFlagExterno"] = rowDetalle.CodigoFlagExterno?.ToString().Trim().PadLeft(1, '0') ?? "0";
                dataRow["NumeroLogExterno"] = rowDetalle.NumeroLogExterno?.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["ImporteComisionCCI"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteComisionCCI?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["ImporteComisionIB"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteComisionIB?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["ImporteDesembolsoCCI"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteDesembolsoCCI?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["CodigoRetorno"] = rowDetalle.CodigoEstadoPago?.ToString().Trim().PadLeft(2, '0') ?? "00";

                dataTable.Rows.Add(dataRow);
            }

            if (dataTable.Rows.Count > 0)
            {
                _dependencias.UtilitariosRepository.BulkInsertTable(dataTable, TablasConstants.TablaTramasProcesadasBulkCopy);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual async Task<string> GenerarArchivoPlano(string? planilla, string? observacionCabecera, int flujo, int idCabeceraSeguimiento)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarArchivoPlanoI, $"Inicia Proceso Generar Archivo Plano", 13);
            var result = _dependencias.PlanillasRepository.ObtenerInformacionPlanilla(planilla ?? "");
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"MONITOR_H2H_{timestamp}_{planilla}_{result.Result.CanalAtencion}.txt";

            var configSftp = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioSftpMonitorFCD);
            var flag = Convert.ToInt32(configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.FlagRed)?.DESCRIPCIONCORTA);
            string? password = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPassword)?.DESCRIPCIONCORTA;
            var privateKeyLocalFilePath = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePathKey)?.DESCRIPCIONCORTA;
            string? Host = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpHost)?.DESCRIPCIONCORTA;
            int Port = Convert.ToInt32(configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPort)?.DESCRIPCIONCORTA);
            string? Username = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpUsername)?.DESCRIPCIONCORTA;
            string? RemotePath = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePath)?.DESCRIPCIONCORTA;


            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                string cabecera = GenerarCabecera(planilla ?? "", observacionCabecera ?? "");
                await writer.WriteLineAsync(cabecera);

                if (flujo == 2)
                {
                    string detalle = await GenerarDetalle(planilla ?? "");
                    await writer.WriteAsync(detalle);
                }
                await writer.FlushAsync();

                memoryStream.Position = 0;

                await _sftpService.Conectar(Host ?? string.Empty, Port, Username ?? string.Empty, password ?? string.Empty, privateKeyLocalFilePath ?? string.Empty, Convert.ToBoolean(flag));
                await _sftpService.ValidarRutaDirectorioRemotaSftp(RemotePath ?? string.Empty);
                await _sftpService.ValidarArchivoExistenteSftp(fileName ?? string.Empty);
                await _sftpService.SubirArchivoSftp(memoryStream, RemotePath + fileName);
                await _sftpService.Desconectar();
            }
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarArchivoPlanoF, $"Termino Correctamente Generar Archivo Plano", 13);

            return $"{RemotePath}|{fileName}";
        }

        private static string GenerarCabecera(string planilla, string observacionCabecera)
        {
            string FillerCabecera = new string(' ', 95);
            planilla = planilla.PadRight(10);
            string codigo = ObtenerCodigoCabecera(observacionCabecera ?? "");
            string observacion = (observacionCabecera ?? "").PadRight(105);
            observacionCabecera = codigo + " " + observacion;
            return $"{planilla}{FillerCabecera}{observacionCabecera}";
        }

        private async Task<string> GenerarDetalle(string numeroPlanilla)
        {
            _logger.LogInformation($"2.1.3.3.5.1- Inicia Obtencion del Detalle para archivo txt");
            var detalles = await _dependencias.PlanillasRepository.ObtenerDetallePlanillaMonitor(numeroPlanilla);
            _logger.LogInformation($"2.1.3.3.5.1- Finaliza Obtencion del Detalle para archivo txt");

            var stringBuilder = new StringBuilder();

            string FillerDetalle1 = new string(' ', 4);
            string FillerDetalle2 = new string(' ', 20);
            string FillerDetalle3 = new string(' ', 20);

            foreach (var detalle in detalles)
            {
                string codigo;
                string observacion;
                string numeroInterno = (detalle.NumeroInterno ?? "").PadRight(10);
                string numdocAceptante = (detalle.NumeroDocumentoAceptante ?? "").PadRight(15);
                string tipoOperacion = (detalle.TipoOperacion ?? "");
                string numdocFisico = (detalle.NumeroDocumentoFisico ?? "").PadRight(20);
                string estado = (detalle.Estado ?? "").PadRight(15);
                if (detalle.Estado == "VIGENTE")
                {
                    codigo = "00000";
                    observacion = "-";
                }
                else
                {
                    codigo = ObtenerCodigoDetalle(detalle.Observacion ?? "");
                    observacion = (detalle.Observacion ?? "").PadRight(105);
                }
                string observacionDetalle = codigo + " " + observacion;

                string lineaDetalle = $"{numeroInterno}{FillerDetalle1}{numdocAceptante}{tipoOperacion}{numdocFisico}{FillerDetalle2}{estado}{FillerDetalle3}{observacionDetalle}";

                stringBuilder.AppendLine(lineaDetalle);
            }
            return stringBuilder.ToString();
        }

        private async Task ManejarErrorPlanilla(string numeroPlanilla, int planillaSeq, string descripcionError, int idCabeceraSeguimiento)
        {
            if (!descripcionError.Contains("ERROR AL REALIZAR LA LIBERACION DE RESERVA", StringComparison.OrdinalIgnoreCase))
            {
                var resultadoLiberacion = await EliminaReservaDistribuido(idCabeceraSeguimiento, numeroPlanilla);

                if (!string.IsNullOrWhiteSpace(resultadoLiberacion))
                {
                    string mensajeErrorLiberacion;
                    mensajeErrorLiberacion = $"{resultadoLiberacion}";

                    var requestNotificacionLiberacion = new NotificacionAssiRequest(LiberacionReserva, mensajeErrorLiberacion, "36", numeroPlanilla, null, null, null);

                    await _dependencias.NotificacionService.NotificacionAssi(requestNotificacionLiberacion, idCabeceraSeguimiento);
                }
            }

            var existemensajePlanilla = await _dependencias.DesembolsoRepository.ObtenerMensajeErrorMonitorPorPlanilla(numeroPlanilla);
            if (existemensajePlanilla == 0)
            {
                await _dependencias.DesembolsoRepository.RegistrarMensajeErrorMonitor(numeroPlanilla, planillaSeq, descripcionError);

                var resultArchivo = await GenerarArchivoPlano(numeroPlanilla, descripcionError, 1, idCabeceraSeguimiento);

                string[] parts = resultArchivo.Split('|');
                string remotePath = parts[0];
                string fileName = parts[1];

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorI, $"Inicia Proceso Notificacion Assi", 14);

                var request = new NotificacionAssiRequest(ConfirmacionDesembolso, descripcionError, "36", numeroPlanilla, null, fileName, remotePath);

                var respuesaNotificacion = await _dependencias.NotificacionService.NotificacionAssi(request, idCabeceraSeguimiento);

                if (respuesaNotificacion == 32)
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorF, $"Termino Correctamente Notificacion Assi", 14);
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaNotificacionAssiMonitorE, $"Error Proceso de notificacion ASSI", 14);
                }
            }
        }

        private async Task<string> EliminaReservaDistribuido(int idSeguimiento, string numeroPlanilla)
        {
            var reservasFallidas = new ConcurrentBag<int>();

            await _trazaService.RegistrarDetalleTraza(idSeguimiento, GetType().Name, TrazaConstante.TrazaEliminaReservaDistribuidoI, $"Obtiene Reservas afiliadas a la planilla", 10);
            var resultReservas = await _dependencias.PlanillasRepository.ObtenerReservasPlanillas(numeroPlanilla ?? string.Empty);
            await _trazaService.RegistrarDetalleTraza(idSeguimiento, GetType().Name, TrazaConstante.TrazaEliminaReservaDistribuidoF, $"Temino Reservas afiliadas a la planilla", 10);

            var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);

            var flagPruebaReservas = int.Parse(
                configuracionDb2.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagLiberacionReserva)
                ?.DESCRIPCIONCORTA ?? "0"
            );

            if (flagPruebaReservas != 0)
            {
                var reservasPruebas = resultReservas
                    .Select(r => r.CodigoReserva)
                    .ToList();

                return string.Join("|", reservasPruebas);
            }

            var tasks = resultReservas.Select(async reserva =>
            {
                try
                {
                    var paramLiberacion = new LiberacionReservaRequest()
                    {
                        CodigoSolicitud = reserva.CodigoReserva
                    };

                    _logger.LogInformation("2.1.3.3.4.3- Libera la reserva de la planilla serv externo {NumeroPlanilla}", numeroPlanilla);
                    await _trazaService.RegistrarDetalleTraza(idSeguimiento, GetType().Name, TrazaConstante.TrazaEliminaReservaDistribuidoI, $"Inicia Proceso de Liberacion para la reserva {reserva.CodigoReserva}", 10);
                    var resultLiberacion = await _dependencias.LineaService.LiberacionReserva(paramLiberacion);

                    if (resultLiberacion.CodigoRespuesta == 32)
                    {
                        await _trazaService.RegistrarDetalleTraza(idSeguimiento, GetType().Name, TrazaConstante.TrazaEliminaReservaDistribuidoF, $"Libero Correctamente la reserva  {reserva.CodigoReserva}", 10);

                        _logger.LogInformation("2.1.3.3.4.4- Elimina la reserva {CodigoReserva} de TMP_RESERVAS_PLANILLA para la planilla {NumeroPlanilla}", paramLiberacion.CodigoSolicitud, numeroPlanilla);
                    }
                    else
                    {
                        reservasFallidas.Add(reserva.CodigoReserva);
                    }

                }
                catch
                {
                    reservasFallidas.Add(reserva.CodigoReserva);
                }
            });
            await Task.WhenAll(tasks);

            return string.Join("|", reservasFallidas);
        }

        public static string fstrFormatString(string pstrValor)
        {
            string strReturn;
            try
            {
                strReturn = pstrValor.Replace("&#x0;", " ");
                return strReturn;
            }

            catch (Exception)
            {
                return " ";
            }
        }

        public static string ConvertCommaToDotForDecimal(string input)
        {
            string result;
            try
            {
                result = input.Replace(',', '.');
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static readonly Dictionary<string, string> codigoDescripcionesDetalle = new Dictionary<string, string>()
        {
            { "", ProcesarTramasConstants.CodigoDefault },
            { "-", ProcesarTramasConstants.CodigoDefault },
            { "--", ProcesarTramasConstants.CodigoDefault },
            { "---", ProcesarTramasConstants.CodigoDefault },
            { "COMMAREA TLDO087", "00001" },
            { "ABEND AAL8 EN PROGRAMA TLDOCLI", "00002" },
            { "ARCHIVO NO ABIERTO", "00003" },
            { "CJE - 2042", "00004" },
            { "CJE - 2042BBVA", "00005" },
            { "CLAVE NO EXISTE,VERIFIQUE", "00006" },
            { "CTA CTE PURGADA.VERIFICAR", "00007" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0002 S", "00008" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0003 S", "00009" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0004 S", "00010" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0008 S", "00011" },
            { "ERROR EN TIPO OPERACION CR/DB", "00012" },
            { "ERROR TRAMA SALIDA SYSTEMATICS", "00013" },
            { "NO CUMPLE DIGITO VERIFI.", "00014" },
            { "NO DEFINIDO", "00015" },
            { "OML0 TS9951 F: ARCHIVO ARCHIVO CERRADO I", "00016" },
            { "RECURSO NO DISPONIBLE APLICAC. DEP", "00017" },
            { "SALDO INSUFICIENTE", "00018" },
        };

        private static readonly Dictionary<string, string> codigoDescripcionesCabecera = new Dictionary<string, string>()
        {
            { ProcesarTramasConstants.DesembolsoOK, ProcesarTramasConstants.CodigoDefault },
            { ProcesarTramasConstants.ErrorRegistrarDetallePlanilla, "00001" },
            { ProcesarTramasConstants.ErrorRegistroTramas, "00002" },
            { ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2, "00003" },
            { ProcesarTramasConstants.ErrorRegistrarMovimientoDocumentos, "00004" },
        };

        public static string ObtenerCodigoDetalle(string observacion)
        {
            if (codigoDescripcionesDetalle.TryGetValue(observacion, out var codigo))
            {
                return codigo;
            }
            return "00099";
        }

        public static string ObtenerCodigoCabecera(string observacion)
        {
            if (codigoDescripcionesCabecera.TryGetValue(observacion, out var codigo))
            {
                return codigo;
            }
            return "00099";
        }

    }
}

