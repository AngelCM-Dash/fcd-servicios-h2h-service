using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.DataTable.DB2;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.DB2;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Services
{
    [ExcludeFromCodeCoverage]
    public class DesembolsoAbonoService : IDesembolsoAbonoService
    {
        private readonly DesembolsoAbonoServiceDependencies _dependencias;
        private readonly ITrazaService _trazaService;
        private readonly ILogger<DesembolsoAbonoService> _logger;
        private string MensajeAbonoIntentoCero = string.Empty;

        public DesembolsoAbonoService(DesembolsoAbonoServiceDependencies dependencias, ILogger<DesembolsoAbonoService> logger, ITrazaService trazaService)
        {
            _dependencias = dependencias;
            _logger = logger;
            _trazaService = trazaService;
        }

        public async Task ValidarEstadoProcesoAbono(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, int numeroIntento, int tipoReintentoAbono, int numeroPlanillaSeq, int idCabeceraSeguimiento, string codigoUnicoProveedor)
        {
            if (numeroIntento == 0)
                MensajeAbonoIntentoCero = abono.SrvResponseMessage ?? string.Empty;


            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoI, $"[Validación Abono Desembolso] Inicio del proceso.", 9);
                var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);
                var queryConsultaPasoMasi = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(1, 2);
                var context = new ReintentoContext(numeroIntento, tipoReintentoAbono, idCabeceraSeguimiento, numeroPlanillaSeq, codigoUnicoProveedor);

                await ReintentoFalloDB2(request, abono, configuracionDb2, queryConsultaPasoMasi, context);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoE, $"[Error] Se produjo una excepción durante el proceso de validación de abono/desembolso. Detalle: {ex}", 9);
                _logger.LogError(ex, "Error en ValidacionAbonoDesembolso. para Planilla: {NumeroPlanilla}, Secuencia: {NumeroPlanillaSeq}, Intento: {NumeroIntento}", request.NumeroPlanilla, numeroPlanillaSeq, numeroIntento);
                await ActualizarEstadoAbonosFalloDb2(request, abono, numeroPlanillaSeq, codigoUnicoProveedor);

                if (MensajeAbonoIntentoCero == ProcesoDesembolsosOkMessage)
                {
                    var mensajeRespuesta = $"PROCESO DESEMBOLSOS OK | Número de registros procesados: {abono.registrationNumberProcessed} | Numero de registros extornados: {abono.returnedRegistrationNumber}";

                    var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeRespuesta, "32", request.NumeroPlanilla, request.CodigoAgrupamiento, null, null);

                    await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, idCabeceraSeguimiento);

                    ProcesarTramasRequest paramTramas = new ProcesarTramasRequest
                    {
                        NumeroPlanilla = request.NumeroPlanilla ?? "",
                        NumeroSecuencia = numeroPlanillaSeq,
                        FlagMonitor = true,
                        TipoProcesamiento = 3,
                    };

                    await _dependencias.DesembolsoService.ProcesarTramas(paramTramas);
                }
            }
        }

        public async Task ReintentoFalloDB2(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, List<DapperParametro> configuracionDb2, List<DapperParametroDB2> queryConsultaPasoMasi, ReintentoContext param)
        {
            List<DetallePlanillasProcesadasResponse> consultarAbonoPagoMasi = new List<DetallePlanillasProcesadasResponse>();

            int maxReintentos = 2;
            int nroIntento = 0;
            bool success = false;

            while (nroIntento <= maxReintentos && !success)
            {
                try
                {
                    consultarAbonoPagoMasi = await _dependencias.MonitorService.ObtenerDetallePlanillasProcesadasDB2(queryConsultaPasoMasi, configuracionDb2, request.NumeroPlanilla ?? "", param.NumeroPlanillaSeq.ToString());
                    success = true;
                }
                catch (Exception ex)
                {
                    nroIntento++;
                    _logger.LogWarning(ex, "Intento {NroIntento} fallido en ObtenerDetallePlanillasProcesadasDB2 para Planilla: {NumeroPlanilla}, Secuencia: {NumeroPlanillaSeq}", nroIntento, request.NumeroPlanilla, param.NumeroPlanillaSeq);

                    if (nroIntento > maxReintentos)
                    {
                        await _trazaService.RegistrarDetalleTraza(param.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoE, $"[Error - ObtenerDetallePlanillasProcesadasDB2] Se alcanzó el máximo de intentos fallidos. Se continuará con el procesamiento de tramas. Detalle del error: {ex.Message}", 9);

                        if (MensajeAbonoIntentoCero == ProcesoDesembolsosOkMessage)
                        {
                            var mensajeRespuesta = $"PROCESO DESEMBOLSOS OK | Número de registros procesados: {abono.registrationNumberProcessed} | Numero de registros extornados: {abono.returnedRegistrationNumber}";

                            var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeRespuesta, "32", request.NumeroPlanilla, request.CodigoAgrupamiento, null, null);

                            await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, param.IdCabeceraSeguimiento);

                            ProcesarTramasRequest paramTramas = new ProcesarTramasRequest
                            {
                                NumeroPlanilla = request.NumeroPlanilla ?? "",
                                NumeroSecuencia = param.NumeroPlanillaSeq,
                                FlagMonitor = true,
                                TipoProcesamiento = 3,
                            };
                            await _dependencias.DesembolsoService.ProcesarTramas(paramTramas);
                        }
                        else
                        {
                            var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, "No se Pudo Realizar el Abono", "36", request.NumeroPlanilla, null, null, null);

                            await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, param.IdCabeceraSeguimiento);
                        }
                        return;
                    }
                    await Task.Delay(1000);
                }
            }

            await ProcesaReintento(request, abono, configuracionDb2, consultarAbonoPagoMasi, param);
        }

        public async Task ProcesaReintento(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, List<DapperParametro> configuracionDb2, List<DetallePlanillasProcesadasResponse> consultarAbonoPagoMasi, ReintentoContext param)
        {
            var NumeroReintentos = configuracionDb2.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.NumeroReintentoProcesoAbono)?.DESCRIPCIONCORTA ?? String.Empty;
            var FlagDesembolsosParciales = configuracionDb2.FirstOrDefault(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagDesembolsoParcialProveedor)?.DESCRIPCIONCORTA ?? String.Empty;

            int NumeroReintento = Convert.ToInt32(NumeroReintentos);
            int FlagParciales = Convert.ToInt32(FlagDesembolsosParciales);

            int totalDesembolsados = consultarAbonoPagoMasi.Count;
            int exitosos = consultarAbonoPagoMasi.Count(x => x.CodigoEstadoPago == "09");
            int fallidosLogConMensaje = consultarAbonoPagoMasi.Count(x => x.CodigoEstadoPago != "09" && !string.IsNullOrWhiteSpace(x.DescripcionMensajeError));

            if (param.NumeroIntento < NumeroReintento && param.NumeroIntento != 0)
            {
                var parametrosIntentoIntermedio = new ProcesarIntentoIntermedioParams
                {
                    FallidosLogConMensaje = fallidosLogConMensaje,
                    Exitosos = exitosos,
                    TotalDesembolsados = totalDesembolsados,
                    TipoReintentoAbono = param.TipoReintentoAbono,
                    NumeroPlanillaSeq = param.NumeroPlanillaSeq,
                    CodigoUnicoProveedor = param.CodigoUnicoProveedor,
                    IdCabeceraSeguimiento = param.IdCabeceraSeguimiento,

                };

                await ProcesarReintentoIntermedio(request, abono, consultarAbonoPagoMasi, parametrosIntentoIntermedio);
            }
            else if (param.NumeroIntento == 0)
            {
                var parametrosIntentoInicial = new ProcesarIntentoInicialParams
                {
                    Exitosos = exitosos,
                    TotalDesembolsados = totalDesembolsados,
                    FallidosLogConMensaje = fallidosLogConMensaje,
                    FlagDesembolsosParciales = FlagParciales,
                    NumeroPlanillaSeq = param.NumeroPlanillaSeq,
                    IdCabeceraSeguimiento = param.IdCabeceraSeguimiento
                };

                await ProcesarIntentoInicial(request, abono, consultarAbonoPagoMasi, parametrosIntentoInicial);
            }
            else if (param.NumeroIntento == NumeroReintento)
            {
                await ProcesarReintentoFinal(request, abono, param.TipoReintentoAbono, param.NumeroPlanillaSeq.ToString(), param.IdCabeceraSeguimiento);
            }
            else
            {
                await _trazaService.RegistrarDetalleTraza(param.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoE, $"[Validación Abono Desembolso] Proceso Abono Fallido Parcial", 9);
            }
            await _trazaService.RegistrarDetalleTraza(param.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoF, $"[Validación Abono Desembolso] Proceso finalizado correctamente.", 9);
        }

        private async Task ProcesarIntentoInicial(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, List<DetallePlanillasProcesadasResponse> consultarAbonoPagoMasi, ProcesarIntentoInicialParams parametros)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoInicialI, $"[Validación Abono Desembolso Inicial] Inicio del proceso. Cantidad exitosa: {parametros.Exitosos}, Monto total desembolsado: {parametros.TotalDesembolsados}", 10);
                if (parametros.Exitosos == 0 && parametros.FallidosLogConMensaje == 0 || parametros.TotalDesembolsados == 0)
                {
                    await _dependencias.DesembolsoRepository.RegistrarDesembolsoAbono(
                        request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, "",
                        (int)NumeroReintentoBaseProcesoAbono.ReintentoCero,
                        (int)EstadoReintentoProcesoAbono.Fallido,
                        (int)TipoReintentoProcesoAbono.Total,
                        parametros.Exitosos
                    );
                    await ReintentoAbonoFallidoTotal(request, parametros.IdCabeceraSeguimiento, parametros.NumeroPlanillaSeq);
                }
                else if ((parametros.Exitosos + parametros.FallidosLogConMensaje) == parametros.TotalDesembolsados)
                {
                    await _dependencias.DesembolsoRepository.RegistrarDesembolsoAbono(
                        request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, "",
                        (int)NumeroReintentoBaseProcesoAbono.ReintentoCero,
                        (int)EstadoReintentoProcesoAbono.Exitoso,
                        (int)TipoReintentoProcesoAbono.Total,
                        parametros.Exitosos
                    );
                    await ProcesarAbonoTotalFinalizadoCorrectamente(request, abono, parametros.NumeroPlanillaSeq, (int)TipoReintentoProcesoAbono.Total, parametros.IdCabeceraSeguimiento);
                }
                else if (parametros.Exitosos + parametros.FallidosLogConMensaje < parametros.TotalDesembolsados && parametros.FlagDesembolsosParciales == 0)
                {
                    await _dependencias.DesembolsoRepository.RegistrarDesembolsoAbono(
                        request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, "",
                        (int)NumeroReintentoBaseProcesoAbono.ReintentoCero,
                        (int)EstadoReintentoProcesoAbono.Exitoso,
                        (int)TipoReintentoProcesoAbono.ParcialTotal,
                        parametros.Exitosos
                    );
                    await ReintentoAbonoFallidoParcialTotal(request, consultarAbonoPagoMasi, parametros.NumeroPlanillaSeq, parametros.IdCabeceraSeguimiento, (int)TipoReintentoProcesoAbono.ParcialTotal);
                }
                else
                {
                    await _dependencias.DesembolsoRepository.RegistrarDesembolsoAbono(
                        request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, "",
                        (int)NumeroReintentoBaseProcesoAbono.PrimerReintento,
                        (int)EstadoReintentoProcesoAbono.Exitoso,
                        (int)TipoReintentoProcesoAbono.Parcial,
                        parametros.Exitosos
                    );
                    await ReintentoAbonoFallidoParcialTotal(request, consultarAbonoPagoMasi, parametros.NumeroPlanillaSeq, parametros.IdCabeceraSeguimiento, (int)TipoReintentoProcesoAbono.Parcial);
                }
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoInicialF, $"[Validación Abono Desembolso Inicial] Proceso finalizado correctamente. Total exitosos: {parametros.Exitosos}, Monto total desembolsado: {parametros.TotalDesembolsados}", 10);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoInicialE, $"[Error] Excepción en validación de abono desembolso inicial. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 10);
                await ActualizarEstadoAbonosFalloDb2(request, abono, parametros.NumeroPlanillaSeq, "");
                throw;
            }
        }

        private async Task ProcesarReintentoIntermedio(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, List<DetallePlanillasProcesadasResponse> consultarAbonoPagoMasi, ProcesarIntentoIntermedioParams parametros)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoIntermedioI, $"[Validación Reintento Abono Intermedio] Inicio del proceso. Total exitosos: {parametros.Exitosos}, Monto total desembolsado: {parametros.TotalDesembolsados}", 11);
                if ((parametros.Exitosos + parametros.FallidosLogConMensaje) == parametros.TotalDesembolsados)
                {
                    switch ((TipoReintentoProcesoAbono)parametros.TipoReintentoAbono)
                    {
                        case TipoReintentoProcesoAbono.Total:
                            await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, parametros.CodigoUnicoProveedor ?? string.Empty, (int)EstadoReintentoProcesoAbono.Exitoso, parametros.Exitosos);
                            await ProcesarAbonoTotalFinalizadoCorrectamente(request, abono, parametros.NumeroPlanillaSeq, parametros.TipoReintentoAbono, parametros.IdCabeceraSeguimiento);
                            break;

                        case TipoReintentoProcesoAbono.ParcialTotal:
                            await ProcesarAbonoParcialFinalizadoCorrectamente(request, parametros.NumeroPlanillaSeq, parametros.TipoReintentoAbono, parametros.IdCabeceraSeguimiento);
                            await ActualizarEstadoAbonos(consultarAbonoPagoMasi);
                            break;
                    }
                }
                else if ((parametros.Exitosos == 0 && parametros.FallidosLogConMensaje == 0 || parametros.TotalDesembolsados == 0) && parametros.TipoReintentoAbono == (int)TipoReintentoProcesoAbono.Total)
                {
                    await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(request.NumeroPlanilla ?? "", parametros.NumeroPlanillaSeq, parametros.CodigoUnicoProveedor ?? string.Empty, (int)EstadoReintentoProcesoAbono.Fallido, parametros.Exitosos);
                    await ReintentoAbonoFallidoTotal(request, parametros.IdCabeceraSeguimiento, parametros.NumeroPlanillaSeq);
                }
                else if ((parametros.Exitosos + parametros.FallidosLogConMensaje < parametros.TotalDesembolsados || parametros.TotalDesembolsados == 0) && parametros.TipoReintentoAbono == (int)TipoReintentoProcesoAbono.ParcialTotal)
                {
                    await ActualizarEstadoAbonos(consultarAbonoPagoMasi);
                    await ReintentoAbonoFallidoParcialTotal(request, consultarAbonoPagoMasi, parametros.NumeroPlanillaSeq, parametros.IdCabeceraSeguimiento, (int)TipoReintentoProcesoAbono.ParcialTotal);
                }
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoIntermedioF, $"[Validación Abono Desembolso Intermedio] Proceso finalizado correctamente. Total exitosos: {parametros.Exitosos}, Monto total desembolsado: {parametros.TotalDesembolsados}", 11);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(parametros.IdCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoInicialE, $"[Error] Excepción en validación de abono desembolso intermedio. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 11);
                throw;
            }
        }

        private async Task ProcesarReintentoFinal(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, int tipoReintentoAbono, string numeroPlanillaSeq, int idCabeceraSeguimiento)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoFinalI, $"[Validación Reintento Abono Final] Inicio del proceso.", 12);

                var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);
                var queryConsultaPasoMasiLog = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(1, 2);

                List<DetallePlanillasProcesadasResponse> detalleAbonoPagoMasiLog;

                var secuenciaGlobal = "0";
                detalleAbonoPagoMasiLog = await _dependencias.MonitorService.ObtenerDetallePlanillasProcesadasDB2(queryConsultaPasoMasiLog, configuracionDb2, request.NumeroPlanilla ?? "", secuenciaGlobal);

                var TotalDesembolsados = detalleAbonoPagoMasiLog.Count;
                int exitosoDesembolsados = detalleAbonoPagoMasiLog.Count(x => x.CodigoEstadoPago == "09");
                int fallidosLogConMensaje = detalleAbonoPagoMasiLog.Count(x => x.CodigoEstadoPago != "09" && !string.IsNullOrWhiteSpace(x.DescripcionMensajeError));

                var numeroSecuencia = Convert.ToInt32(numeroPlanillaSeq);

                if (exitosoDesembolsados > 0 || fallidosLogConMensaje > 0)
                {
                    switch ((TipoReintentoProcesoAbono)tipoReintentoAbono)
                    {
                        case TipoReintentoProcesoAbono.Total:
                            await ActualizarEstadoAbonos(detalleAbonoPagoMasiLog);
                            await ProcesarAbonoTotalFinalizadoCorrectamente(request, abono, numeroSecuencia, tipoReintentoAbono, idCabeceraSeguimiento);
                            break;

                        case TipoReintentoProcesoAbono.ParcialTotal:
                            await ActualizarEstadoAbonos(detalleAbonoPagoMasiLog);
                            await ProcesarAbonoParcialFinalizadoCorrectamente(request, numeroSecuencia, tipoReintentoAbono, idCabeceraSeguimiento);
                            break;
                    }
                }

                if (TotalDesembolsados == 0)
                {
                    await ActualizarEstadoAbonos(detalleAbonoPagoMasiLog);
                    await ProcesarAbonoFinalizadoFallido(request, idCabeceraSeguimiento);
                }

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoFinalF, $"[Validación Reintento Abono Final] Proceso finalizado correctamente.", 12);

            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoFinalE, $"[Error] Excepción en validación de abono reintento final. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw;
            }
        }

        private async Task ActualizarEstadoAbonos(List<DetallePlanillasProcesadasResponse> abonos)
        {
            foreach (var item in abonos)
            {
                var estado = item.CodigoEstadoPago == "09"
                    ? (int)EstadoReintentoProcesoAbono.Exitoso
                    : (int)EstadoReintentoProcesoAbono.Fallido;

                var cantidadProcesados = estado == (int)EstadoReintentoProcesoAbono.Exitoso ? 1 : 0;

                await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(
                    item.NumeroPlanilla ?? "",
                    item.NumeroSecuenciaPlanilla,
                    item.CodigoUnico ?? "",
                    estado,
                    cantidadProcesados
                );
            }
        }

        private async Task ActualizarEstadoAbonosFalloDb2(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, int numeroPlanillaSeq, string codigoUnicoProveedor)
        {

            await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(
                request.NumeroPlanilla ?? "",
                numeroPlanillaSeq,
                codigoUnicoProveedor ?? "",
                (int)EstadoReintentoProcesoAbono.ExitosoParcialFalloDb2,
                abono.registrationNumberProcessed ?? 0
            );
        }

        private async Task ReintentoAbonoFallidoParcialTotal(ActualizarPlanillaCommand request, List<DetallePlanillasProcesadasResponse> detalleAbonoPagoMasi, int numeroPlanillaSeq, int idCabeceraSeguimiento, int tipoParcial)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoAbonoFallidoParcialTotalI, $"[Reintento Abono Fallido Parcial Total] Inicio del proceso.", 12);

                var listadoNoProcesados = detalleAbonoPagoMasi
                                          .Where(x => x.CodigoEstadoPago != "09" && string.IsNullOrWhiteSpace(x.DescripcionMensajeError))
                                          .ToList();

                var listadoCodigoUnicoNoProcesados = listadoNoProcesados.Select(x => x.CodigoUnico).ToList();

                var ObtenerAbonoFallido = await _dependencias.DesembolsoRepository.ObtenerDesembolsoAbono(request.NumeroPlanilla ?? "", numeroPlanillaSeq, "" ?? String.Empty);
                var maxReintento = ObtenerAbonoFallido.Count > 0 ? ObtenerAbonoFallido.Max(x => x.NumeroReintento) : 0;
                int nuevoReintento = maxReintento + 1;
                var numeroSecuenciaNueva = await _dependencias.DesembolsoRepository.fintNextPlanillaSecuencia(request.NumeroPlanilla ?? String.Empty);
                var secuenciaNueva = Convert.ToInt32(numeroSecuenciaNueva);

                if (listadoCodigoUnicoNoProcesados.Count > 0 && listadoCodigoUnicoNoProcesados != null)
                {
                    var resultInsProv = InsertarAbonoProveedoresFallidos(listadoCodigoUnicoNoProcesados, request.NumeroPlanilla ?? "", secuenciaNueva);

                    if (!resultInsProv)
                    {
                        throw new ThrowException("36", "Error al insertar los códigos únicos de los proveedores fallidos en la tabla temporal.");
                    }
                }

                foreach (var item in listadoNoProcesados)
                {
                    await _dependencias.DesembolsoRepository.RegistrarDesembolsoAbono(request.NumeroPlanilla ?? "", secuenciaNueva, item.CodigoUnico ?? String.Empty, nuevoReintento, (int)EstadoReintentoProcesoAbono.Enviado, (int)TipoReintentoProcesoAbono.ParcialTotal, 0);
                }

                if (tipoParcial == (int)TipoReintentoProcesoAbono.ParcialTotal)
                {
                    await ProcesaDesembolsoAbono(request, secuenciaNueva, nuevoReintento, (int)TipoReintentoProcesoAbono.ParcialTotal, idCabeceraSeguimiento);
                }
                else
                {
                    if (listadoCodigoUnicoNoProcesados != null)
                    {
                        foreach (var item in listadoCodigoUnicoNoProcesados)
                        {
                            await ProcesaDesembolsoAbono(request, secuenciaNueva, nuevoReintento, (int)TipoReintentoProcesoAbono.ParcialTotal, idCabeceraSeguimiento);
                        }
                    }
                }
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoAbonoFallidoParcialTotalF, $"[Reintento Abono Fallido Parcial Total] Proceso finalizado correctamente.", 12);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaReintentoAbonoFallidoParcialTotalE, $"[Error] Reintento Abono Fallido Parcial Total. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw;
            }
        }

        private static DataTable ProcesarData(DataRowCollection dataRows, IReadOnlyList<ColumnMapping> columnMappings)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(columnMappings.Select(c => new DataColumn(c.NombreColumna)).ToArray());

            foreach (DataRow dataRow in dataRows)
            {
                string trama = dataRow["TRAMA"]?.ToString() ?? string.Empty;
                DataRow row = dt.NewRow();
                foreach (var mapeo in columnMappings)
                {
                    string value = trama.Substring(mapeo.IndiceComienzo, mapeo.Longitud);
                    row[mapeo.NombreColumna] = mapeo.Conversion(value);
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        private async Task<bool> VerificarAbonoParciales(ActualizarPlanillaCommand request, int numeroPlanillaSeq)
        {
            bool todosExisten;

            var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);

            var queryConsultaPasoMasi = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(1, 2);
            var detalleAbonoPagoMasiMaes = await _dependencias.MonitorService.ObtenerDetallePlanillasProcesadasDB2(queryConsultaPasoMasi, configuracionDb2, request.NumeroPlanilla ?? "", numeroPlanillaSeq.ToString());
            var listadoNoProcesados = detalleAbonoPagoMasiMaes.Where(x => x.CodigoEstadoPago != "09").ToList();

            var codigosUnicosNoProcesadosDesembolsoOriginal = listadoNoProcesados
                .Select(x => x.CodigoUnico)
                .Distinct()
                .ToList();

            var codigosUnicoProveedorFallidos = detalleAbonoPagoMasiMaes
                .Select(x => x.CodigoUnico)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();

            todosExisten = codigosUnicosNoProcesadosDesembolsoOriginal.All(codigo => codigosUnicoProveedorFallidos.Contains(codigo));

            return todosExisten;
        }

        private bool InsertarAbonoProveedoresFallidos(List<string?> codigoUnicoProveedor, string numeroPlanilla, int numeroSecuencia)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("NumeroPlanilla", typeof(string));
            dataTable.Columns.Add("NumeroSecuencia", typeof(int));
            dataTable.Columns.Add("CodigoUnicoProveedor", typeof(string));

            if (codigoUnicoProveedor != null && codigoUnicoProveedor.Count > 0)
            {
                foreach (var codigoUnico in codigoUnicoProveedor)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["NumeroPlanilla"] = numeroPlanilla;
                    dataRow["NumeroSecuencia"] = numeroSecuencia;
                    dataRow["CodigoUnicoProveedor"] = codigoUnico;
                    dataTable.Rows.Add(dataRow);
                }
            }
            else
            {
                return false;
            }

            if (dataTable.Rows.Count > 0)
            {
                _dependencias.UtilitariosRepository.BulkInsertTable(dataTable, TablasConstants.TablaAbonoProveedoresFallidos);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task ProcesaDesembolsoAbono(ActualizarPlanillaCommand request, int secuenciaProveedor, int numeroIntento, int tipoReintentoAbono, int idCabeceraSeguimiento)
        {
            try
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaDesembolsoAbonoI, $"[Procesa Desembolso Abono] Inicio del proceso.", 12);

                var parametrosDb2 = await ObtenerParametrosDesembolso();
                var abono = new InstruccionAbonoResponse();
                var dtTramaProveedor = new DataTable();
                var generarTramasCommand = new DapperGenerarTramasInput
                {
                    NumeroPlanilla = request.NumeroPlanilla ?? "",
                    NumeroInstruccion = "",
                    CodigoUnicoProveedor = "",
                    NumeroPlanillaSecuencia = secuenciaProveedor,
                    UsuarioEjecuta = request.CodigoUsuario ?? "",
                    FlagDesembolsar = 1
                };

                if (numeroIntento != 0)
                {
                    switch ((TipoReintentoProcesoAbono)tipoReintentoAbono)
                    {
                        case TipoReintentoProcesoAbono.Total:
                            dtTramaProveedor = await _dependencias.DesembolsoRepository.ObtenerTramas(generarTramasCommand);
                            break;

                        case TipoReintentoProcesoAbono.ParcialTotal:
                            dtTramaProveedor = await _dependencias.DesembolsoRepository.ObtenerTramasParcialTotalPorProveedores(generarTramasCommand, (int)TipoReintentoProcesoAbono.ParcialTotal);
                            break;

                        case TipoReintentoProcesoAbono.Parcial:
                            dtTramaProveedor = await _dependencias.DesembolsoRepository.ObtenerTramasParcialTotalPorProveedores(generarTramasCommand, (int)TipoReintentoProcesoAbono.Parcial);
                            break;
                    }
                }

                if (dtTramaProveedor.Rows.Count > 0)
                {
                    var rows = dtTramaProveedor.Rows;
                    var strCodigoProceso = rows[0][CodigoProceso].ToString() ?? "";

                    if (strCodigoProceso.Equals(TRM))
                    {
                        await InsertaDataFcdPagoMasiMaeBulk(abono, request.NumeroPlanilla ?? string.Empty, parametrosDb2, dtTramaProveedor, idCabeceraSeguimiento, 0, secuenciaProveedor);

                        DateTime fechaActual = DateTime.Now;
                        string fechaFormateada = fechaActual.ToString(FormatoFecha);

                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2I, "Inicia Proceso Instruccion Abono", 8);
                        var param = new InstruccionAbonoRequest { NumeroPlanilla = request.NumeroPlanilla, CodigoUnico = request.CodigoUnico, FechaProceso = fechaFormateada, NumeroSecuencia = secuenciaProveedor };
                        abono = await _dependencias.DesembolsoService.ProcesoInstruccionAbono(param);

                        if (abono.SrvResponseMessage == ProcesoDesembolsosOkMessage)
                        {
                            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2F,
                            $"Termino Correctamente Proceso Instruccion Abono : {abono.SrvResponseMessage} | Número de registros procesados: {abono.registrationNumberProcessed} | Número de registros extornados: {abono.returnedRegistrationNumber}", 8);

                            await ValidarEstadoProcesoAbono(request, abono, numeroIntento, tipoReintentoAbono, secuenciaProveedor, idCabeceraSeguimiento, "");
                        }
                        else
                        {
                            await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(request.NumeroPlanilla ?? "", secuenciaProveedor, String.Empty, (int)EstadoReintentoProcesoAbono.Fallido, 0);

                            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesoAbonoDb2E, "", 11);
                            _logger.LogInformation("{Traza} - Handle -> NumeroPlanilla: {NumeroPlanilla}",
                            TrazaConstante.TrazaProcesoAbonoDb2E,
                            request.NumeroPlanilla);
                            await ValidarEstadoProcesoAbono(request, abono, numeroIntento, tipoReintentoAbono, secuenciaProveedor, idCabeceraSeguimiento, "");
                        }
                    }
                }
                else
                {
                    await _dependencias.DesembolsoRepository.ActualizarEstadoDesembolsoAbono(request.NumeroPlanilla ?? "", secuenciaProveedor, String.Empty, (int)EstadoReintentoProcesoAbono.FalloGeneracionTrama, 0);
                    await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaGenerarTramasDesembolsoE, $"Hubo problemas al generar las tramas", 5);
                    throw new InvalidOperationException("Hubo problemas al generar las tramas.");
                }
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaDesembolsoAbonoF, $"[Procesa Desembolso Abono] Proceso finalizado correctamente.", 12);

            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaDesembolsoAbonoE, $"[Error] Excepción en Procesa Desembolso Abono. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw;
            }
        }

        private async Task<DapperParametroDesembolsoH2H> ObtenerParametrosDesembolso()
        {
            var parametrosDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioDesembolsoH2H);
            return _dependencias.DesembolsoService.ObtenerParametrosDesembolso(parametrosDb2);
        }

        private async Task ProcesarAbonoTotalFinalizadoCorrectamente(ActualizarPlanillaCommand request, InstruccionAbonoResponse abono, int numeroPlanillaSeq, int tipoAbono, int idCabeceraSeguimiento)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoTotalFinalizadoCorrectamenteI, $"[Procesa Abono Total Finalizado Correctamente] Inicio del proceso.", 12);
            _logger.LogInformation("Se Procesa el abono para la planilla: {NumeroPlanilla} con TipoAbono: {TipoAbono}", request.NumeroPlanilla, DesembolsoAbonoConstants.Total);
            FcdTablMaes consultarAbonoMaes = new FcdTablMaes();
            ProcesarTramasRequest paramTramas = new ProcesarTramasRequest
            {
                NumeroPlanilla = request.NumeroPlanilla ?? "",
                NumeroSecuencia = numeroPlanillaSeq,
                FlagMonitor = true,
                TipoProcesamiento = tipoAbono,
            };

            _logger.LogDebug("Obtenemos la Configuracion DB2 para la planilla : {NumeroPlanilla} con TipoAbono: {TipoAbono}", request.NumeroPlanilla, DesembolsoAbonoConstants.Total);
            var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);

            try
            {
                _logger.LogDebug("Obtenemos el Query para Consulta DB2 en Tabla FCD_TABL_MAES  para la planilla : {NumeroPlanilla} con TipoAbono: {TipoAbono}", request.NumeroPlanilla, DesembolsoAbonoConstants.Total);
                var queryConsultaPagoMaes = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(2, 2);

                try
                {
                    consultarAbonoMaes = await _dependencias.DesembolsoService.ObtenerDatosDb2MaesPorPlanillaYSecuencia(queryConsultaPagoMaes, configuracionDb2, request.NumeroPlanilla ?? "", numeroPlanillaSeq);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fallo en ObtenerDatosDb2MaesPorPlanillaYSecuencia para la planilla: {NroPlanilla}", request.NumeroPlanilla);
                }

                var queryActualizacionEstadoMaes = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(2, 4);

                if (consultarAbonoMaes != null && (consultarAbonoMaes.CodigoEstatusRetorno != "09" || consultarAbonoMaes.CodigoEstatusProceso != "09"))
                {
                    await _dependencias.MonitorService.ActualizarPlanillasMasiMaesDB2(queryActualizacionEstadoMaes, configuracionDb2, request.NumeroPlanilla ?? "", numeroPlanillaSeq, "09", "09");
                }

                var mensajeRespuesta = $"PROCESO DESEMBOLSOS OK | Número de registros procesados: {abono.registrationNumberProcessed} | Numero de registros extornados: {abono.returnedRegistrationNumber}";

                var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeRespuesta, "32", request.NumeroPlanilla, request.CodigoAgrupamiento, null, null);

                await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, idCabeceraSeguimiento);

                await _dependencias.DesembolsoService.ProcesarTramas(paramTramas);

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoTotalFinalizadoCorrectamenteF, $"[Procesa Abono Total Finalizado Correctamente] Proceso finalizado correctamente.", 12);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo en Metodo ProcesarAbonoTotalFinalizadoCorrectamente para la planilla: {NroPlanilla}", request.NumeroPlanilla);
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoTotalFinalizadoCorrectamenteE, $"[Error] Excepción en Procesa Abono Total Finalizado Correctamente. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw new InvalidOperationException("Ocurrió un error en ProcesarAbonoTotalFinalizadoCorrectamente.", ex);
            }
        }

        private async Task ProcesarAbonoParcialFinalizadoCorrectamente(ActualizarPlanillaCommand request, int numeroPlanillaSeq, int tipoAbono, int idCabeceraSeguimiento)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoParcialFinalizadoCorrectamenteI, $"[Procesa Abono Parcial Finalizado Correctamente] Inicio del proceso.", 12);

            FcdTablMaes consultarAbonoMaes = new FcdTablMaes();
            _logger.LogInformation("Procesa Abono Total Parcial Planilla: {NumeroPlanilla} - TipoAbono: {TipoAbono}", request.NumeroPlanilla, tipoAbono);
            var ExistenParcialesTotalesPorProcesar = await VerificarAbonoParciales(request, numeroPlanillaSeq);

            if (!ExistenParcialesTotalesPorProcesar)
            {
                return;
            }

            int registrosProcesados = 0;

            var obtenerAbonos = await _dependencias.DesembolsoRepository.ObtenerDesembolsoAbono(request.NumeroPlanilla ?? "", 0, "" ?? String.Empty);

            var cantidadRegistrosProcesadosAbonoOriginal = obtenerAbonos
                                                .Where(x => x.NumeroReintento == (int)NumeroReintentoBaseProcesoAbono.ReintentoCero)
                                                .Select(x => x.NumeroRegistrosProcesados)
                                                .FirstOrDefault();

            var cantidadRegistrosProcesadosReintento = obtenerAbonos.Count(x => x.EstadoReintento == (int)EstadoReintentoProcesoAbono.Exitoso
                                                        && x.NumeroReintento != (int)NumeroReintentoBaseProcesoAbono.ReintentoCero);

            registrosProcesados = cantidadRegistrosProcesadosAbonoOriginal + cantidadRegistrosProcesadosReintento;

            var mensajeRespuesta = $"PROCESO DESEMBOLSOS OK | Número de registros procesados: {registrosProcesados} | Numero de registros extornados: 0";

            var configuracionDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioDesembolsoH2H);

            ProcesarTramasRequest paramTramas = new ProcesarTramasRequest
            {
                NumeroPlanilla = request.NumeroPlanilla ?? "",
                NumeroSecuencia = 0,
                FlagMonitor = true,
                TipoProcesamiento = tipoAbono,
            };

            try
            {
                var queryConsultaPagoMaes = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(2, 2);

                try
                {
                    consultarAbonoMaes = await _dependencias.DesembolsoService.ObtenerDatosDb2MaesPorPlanillaYSecuencia(queryConsultaPagoMaes, configuracionDb2, request.NumeroPlanilla ?? "", numeroPlanillaSeq);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fallo en ObtenerDatosDb2MaesPorPlanillaYSecuencia para la planilla: {NroPlanilla}", request.NumeroPlanilla);
                }

                var queryActualizacionEstadoMaes = await _dependencias.UtilitariosRepository.ObtenerParametrosDB2PorCodigoDominioDB2(2, 4);

                if (consultarAbonoMaes != null && (consultarAbonoMaes.CodigoEstatusRetorno != "09" || consultarAbonoMaes.CodigoEstatusProceso != "09"))
                {
                    try
                    {
                        await _dependencias.MonitorService.ActualizarPlanillasMasiMaesDB2(queryActualizacionEstadoMaes, configuracionDb2, request.NumeroPlanilla ?? "", numeroPlanillaSeq, "09", "09");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fallo en ActualizarPlanillasMasiMaesDB2 para la planilla: {NroPlanilla}", request.NumeroPlanilla);
                    }
                }

                var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeRespuesta, "32", request.NumeroPlanilla, request.CodigoAgrupamiento, null, null);

                await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, idCabeceraSeguimiento);

                await _dependencias.DesembolsoService.ProcesarTramas(paramTramas);

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoParcialFinalizadoCorrectamenteF, $"[Procesa Abono Parcial Finalizado Correctamente] Proceso finalizado correctamente.", 12);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoParcialFinalizadoCorrectamenteE, $"[Error] Excepción en Procesa Abono Parcial Finalizado Correctamente. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw new InvalidOperationException("Ocurrió un error en ProcesarAbonoParcialFinalizadoCorrectamente.", ex);
            }
        }

        public async Task InsertaDataFcdPagoMasiMaeBulk(InstruccionAbonoResponse abono, string numeroPlanilla, DapperParametroDesembolsoH2H parametrosDb2, DataTable dtTrama, int idCabeceraSeguimiento, int intento, int numeroPlanillaSeq)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistraTablaMaesI, "Inicia Procesar Tramas TRM", 7);
            var columnMappings = PagoMasiMae.ObtenerColumnasDb2PagoMasiMae();
            var dataTable = ProcesarData(dtTrama.Rows, columnMappings);

            var resultadoInsertPagoMasi = string.Empty;
            int maxReintentos = 2;
            int nroIntento = 0;
            bool success = false;

            while (nroIntento <= maxReintentos && !success)
            {
                try
                {
                    resultadoInsertPagoMasi = _dependencias.DesembolsoService.InsertDataToDatabase(dataTable, parametrosDb2);
                    success = true;
                }
                catch (Exception ex)
                {
                    nroIntento++;
                    _logger.LogWarning(ex, "Intento {NroIntento} fallido en ObtenerDetallePlanillasProcesadasDB2 para Planilla: {NumeroPlanilla}, Secuencia: {NumeroPlanillaSeq}", nroIntento, numeroPlanilla, numeroPlanillaSeq);

                    if (nroIntento > maxReintentos && intento != 0)
                    {
                        await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaValidaAbonoDesembolsoE, $"Maximo intentos fallidos en ObtenerDetallePlanillasProcesadasDB2: {ex.Message}", 9);

                        if (abono.SrvResponseMessage == ProcesoDesembolsosOkMessage)
                        {
                            ProcesarTramasRequest paramTramas = new ProcesarTramasRequest
                            {
                                NumeroPlanilla = numeroPlanilla ?? "",
                                NumeroSecuencia = numeroPlanillaSeq,
                                FlagMonitor = true,
                                TipoProcesamiento = 3,
                            };
                            await _dependencias.DesembolsoService.ProcesarTramas(paramTramas);
                        }
                        return;
                    }

                }
            }

            if (resultadoInsertPagoMasi != "Inserción/Modificación de datos de forma masiva en tabla A.FCD_TABL_MAES")
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistraTablaMaesE, "Error al insertar datos en PagoMasiMae", 7);
                _logger.LogError("{Traza} - Handle -> NumeroPlanilla: {NumeroPlanilla} Error al insertar datos en PagoMasiMae",
                TrazaConstante.TrazaRegistraTablaMaesE,
                numeroPlanilla);

                throw new ThrowException("36", "Error al insertar datos en PagoMasiMae");
            }
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaRegistraTablaMaesF, "Termino Correctamente Procesar Tramas TRM", 7);
        }

        private async Task ReintentoAbonoFallidoTotal(ActualizarPlanillaCommand request, int idCabeceraSeguimiento, int numeroPlanillaSeq)
        {
            try
            {

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaReintentoAbonoFallidoTotalI, $"[Reintento Abono Fallido Total] Inicio del proceso.", 12);
                var numeroSecuenciaNueva = await _dependencias.DesembolsoRepository.fintNextPlanillaSecuencia(request.NumeroPlanilla ?? String.Empty);
                var secuenciaNueva = Convert.ToInt32(numeroSecuenciaNueva);

                var ObtenerAbonoFallido = await _dependencias.DesembolsoRepository.ObtenerDesembolsoAbono(request.NumeroPlanilla ?? "", numeroPlanillaSeq, "" ?? String.Empty);
                var maxReintento = ObtenerAbonoFallido.Count > 0 ? ObtenerAbonoFallido.Max(x => x.NumeroReintento) : 0;
                int nuevoReintento = maxReintento + 1;

                await ProcesaDesembolsoAbono(request, secuenciaNueva, nuevoReintento, (int)TipoReintentoProcesoAbono.Total, idCabeceraSeguimiento);

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaReintentoAbonoFallidoTotalI, $"[Reintento Abono Fallido Total] Proceso finalizado correctamente.", 12);

            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaReintentoAbonoFallidoTotalI, $"[Error] Excepción en validación de abono desembolso final. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 12);
                throw;
            }
        }

        private async Task ProcesarAbonoFinalizadoFallido(ActualizarPlanillaCommand request, int idCabeceraSeguimiento)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoFinalizadoFallidoI, $"[Procesar Abono Finalizado Fallido] Inicio del proceso.", 12);
            _logger.LogInformation("Se Procesa el abono para la planilla: {NumeroPlanilla} con TipoAbono: {TipoAbono}", request.NumeroPlanilla, DesembolsoAbonoConstants.Total);

            try
            {
                var mensajeRespuesta = $"No se realizo ningun abono";

                var requestNotificacion = new NotificacionAssiRequest(DesembolsoDistribuido, mensajeRespuesta, "36", request.NumeroPlanilla, request.CodigoAgrupamiento, null, null);

                await _dependencias.NotificacionService.NotificacionAssi(requestNotificacion, idCabeceraSeguimiento);

                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoFinalizadoFallidoF, $"[Procesar Abono Finalizado Fallido] Proceso finalizado correctamente.", 12);
            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, TrazaConstante.TrazaProcesarAbonoFinalizadoFallidoE, $"[Error] Excepción en validación de abono desembolso final. Tipo: {ex.GetType().Name}. Mensaje: {ex}", 11);
                throw new InvalidOperationException("Ocurrió un error en ProcesarAbonoFinalizadoFallido.", ex);
            }
        }

        public class ProcesarIntentoInicialParams
        {
            public int Exitosos { get; set; }
            public int TotalDesembolsados { get; set; }
            public int FallidosLogConMensaje { get; set; }
            public int FlagDesembolsosParciales { get; set; }
            public int NumeroPlanillaSeq { get; set; }
            public int IdCabeceraSeguimiento { get; set; }
        }

        public class ProcesarIntentoIntermedioParams
        {
            public int FallidosLogConMensaje { get; set; }
            public int Exitosos { get; set; }
            public int TotalDesembolsados { get; set; }
            public int TipoReintentoAbono { get; set; }
            public int NumeroPlanillaSeq { get; set; }
            public string? CodigoUnicoProveedor { get; set; }
            public int IdCabeceraSeguimiento { get; set; }
        }
        public class ReintentoContext
        {
            public int NumeroIntento { get; set; }
            public int TipoReintentoAbono { get; set; }
            public int IdCabeceraSeguimiento { get; set; }
            public int NumeroPlanillaSeq { get; set; }
            public string CodigoUnicoProveedor { get; set; }

            public ReintentoContext(int numeroIntento, int tipoReintentoAbono, int idCabeceraSeguimiento, int numeroPlanillaSeq, string codigoUnicoProveedor)
            {
                NumeroIntento = numeroIntento;
                TipoReintentoAbono = tipoReintentoAbono;
                IdCabeceraSeguimiento = idCabeceraSeguimiento;
                NumeroPlanillaSeq = numeroPlanillaSeq;
                CodigoUnicoProveedor = codigoUnicoProveedor;
            }
        }
    }
}



