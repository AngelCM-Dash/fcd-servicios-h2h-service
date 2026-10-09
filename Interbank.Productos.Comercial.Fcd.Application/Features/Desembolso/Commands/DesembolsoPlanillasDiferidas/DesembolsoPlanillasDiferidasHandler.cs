using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas
{
    [ExcludeFromCodeCoverage]
    public class DesembolsoPlanillasDiferidasHandler : IRequestHandler<DesembolsoPlanillasDiferidasCommand, DesembolsoResponse>
    {
        private readonly DesembolsoPlanillasDiferidasDependencies _dependencias;
        private readonly ILogger<DesembolsoPlanillasDiferidasHandler> _logger;
        public DesembolsoPlanillasDiferidasHandler(DesembolsoPlanillasDiferidasDependencies deps, ILogger<DesembolsoPlanillasDiferidasHandler> logger)
        {
            _dependencias = deps;
            _logger = logger;
        }

        public Task<DesembolsoResponse> Handle(DesembolsoPlanillasDiferidasCommand request, CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                _logger.LogInformation("1.0.- Inicio - Desembolso Diferido");

                _logger.LogDebug("1.2.- Desembolso Diferido - Se Procesa la planilla: {Planilla} con numeroLinea: {NumeroLinea}", request.numeroPlanilla, request.numeroLinea);
                string strNroPla = request.numeroPlanilla ?? "";
                string strNroIns = request.numeroInstruccion ?? "";
                string strNroLin = request.numeroLinea ?? "";

                var dtTramas = await _dependencias.DesembolsoRepository.GenerarTramaDiferidos(strNroPla, strNroIns, strNroLin);

                if (dtTramas != null && dtTramas.Rows.Count > 0)
                {
                    int strNumeroPlanillaSEQ = Convert.ToInt32(dtTramas.Rows[0]["NumeroPlanillaSEQ"]?.ToString());
                    var resul = String.Format("1|{0}", strNumeroPlanillaSEQ);
                    var strAuxiliar = resul.Split("|");

                    if (strAuxiliar != null && strAuxiliar[0] == "1")
                    {
                        await ProcesarTramasDiferidos(request, strNroPla, strNumeroPlanillaSEQ);
                    }
                }
                _logger.LogInformation("2.0.- Fin - Desembolso Diferido");

            }, cancellationToken);

            var resultado = new DesembolsoResponse()
            {
                CodigoRespuesta = "00",
                MensajeRespuesta = "OK",
            };

            return Task.FromResult(resultado);
        }

        public static decimal CheckDecimal(object pobjValue)
        {
            decimal decSalida = 0;

            if (pobjValue != DBNull.Value && pobjValue != null)
            {
                string strValue = pobjValue.ToString()?.Replace(",", string.Empty) ?? "";

                if (!decimal.TryParse(strValue, out decSalida))
                {
                    decSalida = 0m;
                }
            }

            return decSalida;
        }

        private async Task<DapperParametroDesembolsoH2H> ObtenerParametrosDesembolso()
        {
            var parametrosDb2 = await _dependencias.UtilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioDesembolsoH2H);
            return _dependencias.DesembolsoService.ObtenerParametrosDesembolso(parametrosDb2);
        }

        private async Task ProcesarTramasDiferidos(DesembolsoPlanillasDiferidasCommand request, string strNroPla, int strNumeroPlanillaSEQ)
        {
            var idCabeceraSeguimiento = await _dependencias.SeguimientoRepository.ObtenerIdSeguimientoxPlanilla(request.numeroPlanilla ?? "");

            var dtTramasDiferidos = await _dependencias.DesembolsoRepository.ObtieneTramaDiferidos(strNroPla, strNumeroPlanillaSEQ);

            if (dtTramasDiferidos.Rows.Count > 0)
            {
                var rows = dtTramasDiferidos.Rows;
                var strCodigoProceso = rows[0]["CodigoProceso"].ToString() ?? "";

                if (strCodigoProceso.Equals("TRM"))
                {
                    var abono = new InstruccionAbonoResponse();
                    var parametrosDb2 = await ObtenerParametrosDesembolso();
                    await _dependencias.DesembolsoAbonoService.InsertaDataFcdPagoMasiMaeBulk(abono, request.numeroPlanilla ?? string.Empty, parametrosDb2, dtTramasDiferidos, idCabeceraSeguimiento, 0, strNumeroPlanillaSEQ);

                    var infoPlanilla = await _dependencias.PlanillasRepository.ObtenerInformacionPlanilla(strNroPla);

                    DateTime fechaActual = DateTime.Now;
                    string fechaFormateada = fechaActual.ToString("yyyyMMdd");

                    var param = new InstruccionAbonoRequest { NumeroPlanilla = strNroPla, CodigoUnico = infoPlanilla.CodigoUnico, FechaProceso = fechaFormateada, NumeroSecuencia = strNumeroPlanillaSEQ };
                    abono = await _dependencias.DesembolsoService.ProcesoInstruccionAbono(param);

                    if (abono.SrvResponseMessage == "PROCESO DESEMBOLSOS OK")
                    {
                        _logger.LogDebug("1.19.1.- Desembolso Diferido - {SrvResponseMessage} | Número de registros procesados: {Processed} | Número de registros extornados: {Returned} para la planilla: {Planilla} con numeroLinea: {NumeroLinea}",
                        abono.SrvResponseMessage, abono.registrationNumberProcessed, abono.returnedRegistrationNumber, request.numeroPlanilla, request.numeroLinea);
                    }
                    else
                    {
                        _logger.LogDebug("1.19.2.- Desembolso Diferido - {SrvResponseMessage} | No se pudo realizar el abono para la planilla: {Planilla} con numeroLinea: {NumeroLinea}", abono.SrvResponseMessage, request.numeroPlanilla, request.numeroLinea);
                    }
                }
            }
        }
    }
}


