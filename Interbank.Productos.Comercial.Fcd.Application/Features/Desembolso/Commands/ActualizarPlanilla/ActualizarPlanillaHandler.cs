using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.GenerarTramas;
using Interbank.Productos.Comercial.Fcd.Application.Models.Response;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla
{
    [ExcludeFromCodeCoverage]
    public class ActualizarPlanillaHandler : IRequestHandler<ActualizarPlanillaCommand, DesembolsoResponse>
    {
        private readonly ILogger<ActualizarPlanillaHandler> _logger;
        private readonly ActualizarPlanillaDependencies _dependencias;
        private readonly ITrazaService _trazaService;
        private readonly IMapper _mapper;
        private string numeroPlanillaLog = "";
        private decimal CodigoDetalle = 0;
        public ActualizarPlanillaHandler(
            ILogger<ActualizarPlanillaHandler> logger,
            ActualizarPlanillaDependencies deps,
            ITrazaService trazaService,
            IMapper mapper)
        {
            _logger = logger;
            _dependencias = deps;
            _trazaService = trazaService;
            _mapper = mapper;
        }

        public async Task<DesembolsoResponse> Handle(ActualizarPlanillaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _dependencias.PlanillasRepository.ObtenerInformacionPlanilla(request.NumeroPlanilla ?? string.Empty);

                _logger.LogInformation(
                    "Inicio validación planilla. NumeroPlanilla: {NumeroPlanilla}, Canal: {CanalAtencion}, FechaDesembolso: {FechaDesembolso}",
                    request.NumeroPlanilla,
                    result.CanalAtencion,
                    result.FechaDesembolso
                );

                if (!string.Equals(result.CanalAtencion, "H2H", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "La planilla no pertenece al canal H2HW. El proceso no será ejecutado."
                    );
                }

                var fechaHoy = DateTime.Today;

                if (!result.FechaDesembolso.HasValue)
                {
                    throw new InvalidOperationException(
                        "La planilla no tiene fecha de desembolso registrada."
                    );
                }

                if (result.FechaDesembolso.Value.Date < DateTime.Today)
                {
                    throw new InvalidOperationException(
                        $"La fecha de desembolso debe ser igual o posterior a la fecha actual : {fechaHoy}."
                    );
                }

                numeroPlanillaLog = request.NumeroPlanilla ?? "";

                DesembolsoResponse resultadoFinal;
                var actualizarPlanilla = _mapper.Map<DapperActualizarPlanilla>(request);

                var idCabeceraSeguimiento = await InicializarProceso(request);
                CodigoDetalle = idCabeceraSeguimiento;

                await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaDesembolsoMasivoI, "Inicia Proceso Desembolso Masivo", 1);

                await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaObtenerParamDesembolsoI, "Obtiene Configuracion Parametrica para el Desembolso", 2);
                var parametrosDb2 = await ObtenerParametrosDesembolso();
                await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaObtenerParamDesembolsoF, "Obtuvo Correctamente Configuracion Parametrica para el Desembolso", 2);

                var flagPrueba = int.Parse(parametrosDb2.FlagPruebaDesembolso ?? "0");

                if (flagPrueba != 0)
                {
                    throw new ThrowException("36", "Hubo problemas al generar las tramas");
                }

                var numeroPlanillaSEQ = await ObtenerPlanillaSecuencia(request.NumeroPlanilla ?? "", idCabeceraSeguimiento);

                await ActualizarPlanilla(request, actualizarPlanilla, idCabeceraSeguimiento);

                var dtTramas = await GenerarTramas(request, numeroPlanillaSEQ, idCabeceraSeguimiento);

                if (dtTramas == null)
                {
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoE, $"Hubo problemas al generar las tramas", 5);
                    throw new ThrowException("36", "Hubo problemas al generar las tramas");
                }

                if (dtTramas.Rows.Count > 0)
                {
                    resultadoFinal = await ProcesarFlujoDesembolso(request, dtTramas, idCabeceraSeguimiento, numeroPlanillaSEQ, parametrosDb2);
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoE, $"No se obtuvo resultados para enviar la trama", 5);

                    throw new ThrowException("36", "Hubo problemas al generar las tramas");
                }

                _logger.LogInformation("CodigoRespuesta = {CodigoRespuesta} | MensajeRespuesta = {MensajeRespuesta} | Planilla = {NroPlanilla}",
                resultadoFinal.CodigoRespuesta, resultadoFinal.MensajeRespuesta, request.NumeroPlanilla);
                return new DesembolsoResponse { CodigoRespuesta = resultadoFinal.CodigoRespuesta, MensajeRespuesta = resultadoFinal.MensajeRespuesta };
            }
            catch (Exception ex)
            {
                string mensajeErrorOriginal = ex.Message.ToString();
                string mensajeErrorLiberacion;

                var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);
                var flagPruebaReservas = int.Parse(configuracionDb2.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagLiberacionReserva)?.DESCRIPCIONCORTA ?? String.Empty);

                var resultadoLiberacion = string.Empty;

                if (flagPruebaReservas != 0)
                {
                    var reservasPruebas = request.CodigoReserva ?? new List<int>();

                    resultadoLiberacion = string.Join("|", reservasPruebas);
                }
                else
                {
                    resultadoLiberacion = await EliminaReservaDistribuido(CodigoDetalle, request);
                }

                if (!string.IsNullOrWhiteSpace(resultadoLiberacion))
                {
                    mensajeErrorLiberacion = $"{resultadoLiberacion}";

                    var requestNotificacionLiberacion = new NotificacionAssiRequest(LiberacionReserva, mensajeErrorLiberacion, "36", numeroPlanillaLog, null, null, null);

                    await _dependencias.NotificacionService.NotificacionAssi(requestNotificacionLiberacion, CodigoDetalle);
                }

                _logger.LogError(ex, "Desembolso Distribuido - Sucedio un error : {Error}", ex.Message);

                await _trazaService.RegistrarDetalleTraza(CodigoDetalle, GetType().Name, TrazaConstante.TrazaDesembolsoMasivoE, $"Ocurrió un Error: {ex.ToString()}", 20);

                var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeErrorOriginal, "36", numeroPlanillaLog, null, null, null);

                await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, CodigoDetalle);

                throw new ThrowException("36", ex.Message);
            }
        }

        private async Task<string> EliminaReservaDistribuido(decimal idSeguimiento, ActualizarPlanillaCommand request)
        {
            if (request.CodigoReserva == null || !request.CodigoReserva.Any())
                return string.Empty;

            var reservasFallidas = new ConcurrentBag<int>();

            var tasks = request.CodigoReserva.Select(async codigoReserva =>
            {
                try
                {
                    var paramLiberacion = new LiberacionReservaRequest()
                    {
                        CodigoSolicitud = codigoReserva
                    };

                    _logger.LogInformation(
                        "2.1.3.3.4.3- Libera la reserva de la planilla serv externo {NumeroPlanilla}",
                        request.NumeroPlanilla);

                    await _trazaService.RegistrarDetalleTraza(
                        idSeguimiento,
                        GetType().Name,
                        TrazaConstante.TrazaEliminaReservaDistribuidoI,
                        $"Inicia Proceso de Liberacion para la reserva {codigoReserva}",
                        10);

                    var resultLiberacion = await _dependencias.LineaService
                        .LiberacionReserva(paramLiberacion);

                    if (resultLiberacion.CodigoRespuesta != 32)
                    {
                        reservasFallidas.Add(codigoReserva);
                    }
                }
                catch
                {
                    reservasFallidas.Add(codigoReserva);
                }
            });

            await Task.WhenAll(tasks);

            return string.Join("|", reservasFallidas);
        }

        private async Task ActualizarPlanilla(ActualizarPlanillaCommand request, DapperActualizarPlanilla actualizarPlanilla, int idCabeceraSeguimiento)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizaPlanillaDesembolsoI, "Inicia Proceso Actualizacion de Planilla", 4);

                if (request.CodigoAgrupamiento == null && request.FlagDesembolsoTotal)
                {
                    await _dependencias.DesembolsoRepository.ActualizarPlanillaDistribuidoFCD(actualizarPlanilla);
                }

                if (request.CodigoAgrupamiento != null && !request.FlagDesembolsoTotal)
                {
                    await _dependencias.DesembolsoRepository.ActualizarPlanillaDistribuido(actualizarPlanilla);
                }

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizaPlanillaDesembolsoF, "Termino Correctamente Proceso Actualizacion de Planilla", 4);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaActualizaPlanillaDesembolsoE, $"Ocurrió un Error: {ex.Message}", 4);

            }
        }

        private async Task<string> ObtenerPlanillaSecuencia(string numeroPlanilla, int idCabeceraSeguimiento)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaSecuenciaPlanillaI, "Obtiene Secuencia para el Desembolso", 3);
                var numeroPlanillaSEQ = await _dependencias.DesembolsoRepository.fintNextPlanillaSecuencia(numeroPlanilla);
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaSecuenciaPlanillaF, $"Obtuvo Correctamente la Secuencia: {numeroPlanillaSEQ} para el Desembolso", 3);

                return (numeroPlanillaSEQ);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaAObtenerPlanillaSecuenciaE, "", 8);
                _logger.LogError(ex, "{Traza} - Handle -> NumeroPlanilla: {NumeroPlanilla} Error: {ErrorMessage}",
                TrazaConstante.TrazaAObtenerPlanillaSecuenciaE,
                numeroPlanilla,
                ex.Message);
                return "";
            }
        }

        private async Task<DataTable?> GenerarTramas(ActualizarPlanillaCommand request, string numeroPlanillaSEQ, int idCabeceraSeguimiento)
        {
            try
            {
                var generarTramasCommand = new GenerarTramasCommand
                {
                    numeroPlanilla = request.NumeroPlanilla ?? "",
                    numeroInstruccion = request.CodigoAgrupamiento,
                    numeroPlanillaSecuencia = Convert.ToInt32(numeroPlanillaSEQ),
                    usuarioEjecuta = request.CodigoUsuario ?? "",
                    flagDesembolsar = 1
                };

                var generarTramas = _mapper.Map<DapperGenerarTramasInput>(generarTramasCommand);

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoI, "Inicia Proceso Generacion de Tramas", 5);
                var dtTramas = await _dependencias.DesembolsoRepository.ObtenerTramas(generarTramas);
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoF, "Termino Correctamente Proceso Generacion de Tramas", 5);

                return dtTramas;
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoE, $"Ocurrió un Error: {ex.Message}", 5);
                return null;
            }
        }

        private async Task<DesembolsoResponse> ProcesarTramasTRM(ActualizarPlanillaCommand request, DataTable dtTramas, string numeroPlanillaSEQ, DapperParametroDesembolsoH2H parametrosDb2, int idCabeceraSeguimiento)
        {
            var abono = new InstruccionAbonoResponse();
            int numeroPlanillaSeq = int.Parse(numeroPlanillaSEQ);

            await _dependencias.DesembolsoAbonoService.InsertaDataFcdPagoMasiMaeBulk(abono, request.NumeroPlanilla ?? string.Empty, parametrosDb2, dtTramas, idCabeceraSeguimiento, 0, numeroPlanillaSeq);

            DateTime fechaActual = DateTime.Now;
            string fechaFormateada = fechaActual.ToString(FormatoFecha);

            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2I, "Inicia Proceso Instruccion Abono", 8);
            var param = new InstruccionAbonoRequest { NumeroPlanilla = request.NumeroPlanilla, CodigoUnico = request.CodigoUnico, FechaProceso = fechaFormateada, NumeroSecuencia = numeroPlanillaSeq };

            try
            {
                abono = await _dependencias.DesembolsoService.ProcesoInstruccionAbono(param);
            }
            catch (Exception)
            {
                await _dependencias.DesembolsoAbonoService.ValidarEstadoProcesoAbono(request, abono, (int)NumeroReintentoBaseProcesoAbono.ReintentoCero, (int)TipoReintentoProcesoAbono.Total, numeroPlanillaSeq, idCabeceraSeguimiento, "");
            }


            if (abono.SrvResponseMessage == ProcesoDesembolsosOkMessage)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2F,
                    $"Termino Correctamente Proceso Instruccion Abono : {abono.SrvResponseMessage} | Número de registros procesados: {abono.registrationNumberProcessed} | Número de registros extornados: {abono.returnedRegistrationNumber}", 8);

                _logger.LogInformation("{SrvResponseMessage} | Número de registros procesados: {Processed} | Número de registros extornados: {Returned} | Planilla: {NroPlanilla} ",
                abono.SrvResponseMessage, abono.registrationNumberProcessed, abono.returnedRegistrationNumber, request.NumeroPlanilla);

                await _dependencias.DesembolsoAbonoService.ValidarEstadoProcesoAbono(request, abono, (int)NumeroReintentoBaseProcesoAbono.ReintentoCero, (int)TipoReintentoProcesoAbono.Total, numeroPlanillaSeq, idCabeceraSeguimiento, "");

                return new DesembolsoResponse
                {
                    CodigoRespuesta = "32",
                    MensajeRespuesta = $"{abono.SrvResponseMessage} | Número de registros procesados: {abono.registrationNumberProcessed} | Numero de registros extornados: {abono.returnedRegistrationNumber}"
                };
            }
            else
            {
                await _dependencias.DesembolsoAbonoService.ValidarEstadoProcesoAbono(request, abono, (int)NumeroReintentoBaseProcesoAbono.ReintentoCero, (int)TipoReintentoProcesoAbono.Total, numeroPlanillaSeq, idCabeceraSeguimiento, "");

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2E, "", 11);
                _logger.LogInformation("{Traza} - Handle -> NumeroPlanilla: {NumeroPlanilla}",
                TrazaConstante.TrazaProcesoAbonoDb2E,
                request.NumeroPlanilla);

                _logger.LogInformation("{SrvResponseMessage} | No se pudo realizar el abono", abono.SrvResponseMessage);
                return new DesembolsoResponse
                {
                    CodigoRespuesta = "36",
                    MensajeRespuesta = $"{abono.SrvResponseMessage} | No se pudo realizar el abono"
                };
            }
        }

        private async Task<int> InicializarProceso(ActualizarPlanillaCommand request)
        {
            var idCabeceraSeguimiento = await _dependencias.SeguimientoRepository.ObtenerIdSeguimientoxPlanilla(request.NumeroPlanilla ?? "");
            return idCabeceraSeguimiento;
        }

        private bool InsertarReservaPlanilla(List<int>? CodigoReserva, string numeroPlanilla)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("NumeroPlanilla", typeof(string));
            dataTable.Columns.Add("CodigoReserva", typeof(int));

            if (CodigoReserva != null && CodigoReserva.Count > 0)
            {
                foreach (var codigo in CodigoReserva)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["NumeroPlanilla"] = numeroPlanilla;
                    dataRow["CodigoReserva"] = codigo;
                    dataTable.Rows.Add(dataRow);
                }
            }
            else
            {
                return false;
            }

            if (dataTable.Rows.Count > 0)
            {
                _dependencias.UtilitariosRepository.BulkInsertTable(dataTable, TablasConstants.TablaReservaPlanillaBulkCopy);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<DapperParametroDesembolsoH2H> ObtenerParametrosDesembolso()
        {
            var parametrosDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioDesembolsoH2H);
            return _dependencias.DesembolsoService.ObtenerParametrosDesembolso(parametrosDb2);
        }

        private async Task<DesembolsoResponse> ProcesarNuevoFlujoDesembolso(ActualizarPlanillaCommand request, DataTable dtTramas, string numeroPlanillaSEQ, int idCabeceraSeguimiento, DapperParametroDesembolsoH2H param)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, this.GetType().Name, TrazaConstante.TrazaProcesarNuevoFlujoDesembolsoI, $"Inicio Procesar Nuevo Flujo Desembolso", 6);
            if (request.CodigoReserva?.Count != 0 && request.CodigoReserva != null && request.CodigoReserva.All(codigo => codigo != 0))
            {
                var resultReserva = InsertarReservaPlanilla(request.CodigoReserva, request.NumeroPlanilla ?? "");
                if (!resultReserva)
                {
                    return new DesembolsoResponse
                    {
                        CodigoRespuesta = "36",
                        MensajeRespuesta = "Error al guardar información de la reserva"
                    };
                }
            }

            var result = await ProcesarTramasTRM(request, dtTramas, numeroPlanillaSEQ, param, idCabeceraSeguimiento);

            return new DesembolsoResponse { CodigoRespuesta = result.CodigoRespuesta, MensajeRespuesta = result.MensajeRespuesta };
        }

        private async Task<DesembolsoResponse> ProcesarFlujoDesembolso(ActualizarPlanillaCommand request, DataTable dtTramas, int idCabeceraSeguimiento, string numeroPlanillaSEQ, DapperParametroDesembolsoH2H param)
        {
            CargaArchivoResponse result;
            var rows = dtTramas.Rows;
            var strCodigoProceso = rows[0][CodigoProceso].ToString() ?? "";
            var strRespuestaCodigo = rows[0][RespuestaCodigo].ToString() ?? "";
            var strRespuestaMensaje = rows[0][RespuestaMensaje].ToString() ?? "";

            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, this.GetType().Name, TrazaConstante.TrazaObtenerFlujoDesembolsoI, $"Inicio Procesar Flujo Desembolso valores devueltos en trama: CODIGO_PROCESO: {strCodigoProceso}:::RESPUESTA_CODIGO: {strRespuestaCodigo}:::RESPUESTA_MENSAJE: {strRespuestaMensaje}", 6);
            List<DapperParametro> listDesembolsoFLujo = await _dependencias.DesembolsoRepository.ObtenerFlujoDesembolso("H2H", "FLAGDESEMBOLSO");
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, this.GetType().Name, TrazaConstante.TrazaObtenerFlujoDesembolsoF, $"Parametro devuelto en flujo desembolso: {listDesembolsoFLujo[0].ESTADO.ToString()}", 6);

            if (strCodigoProceso.Equals(TRM))
            {
                return await ProcesarNuevoFlujoDesembolso(request, dtTramas, numeroPlanillaSEQ, idCabeceraSeguimiento, param);
            }
            else
            {
                result = new CargaArchivoResponse()
                {
                    CodigoRespuesta = "36",
                    Resultado = "Registro de Movimientos:" + strCodigoProceso
                };
            }

            return new DesembolsoResponse { CodigoRespuesta = result.CodigoRespuesta, MensajeRespuesta = result.Resultado };
        }
    }
}
