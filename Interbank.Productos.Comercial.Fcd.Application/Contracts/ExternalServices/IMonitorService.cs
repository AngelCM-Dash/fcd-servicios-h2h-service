using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices
{
    public interface IMonitorService
    {
        Task<List<DapperPlanillaCabeceraDb2>> ObtenerPlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros);
        Task<List<DetallePlanillasProcesadasResponse>> ObtenerDetallePlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, string numeroSecuencia);
        Task<string> ActualizarPlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, string numeroSecuencia, int estadoPlanilla);
        Task<string> ActualizarPlanillasMasiMaesDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia, string estadoRetorno, string estadoProceso);
        Task<MovimientoResponse> ConsumoLineaMovimiento(MovimientoRequest movimientoRequest);
        Task<BaseResponse> EnvioCorreoDietarios(string numeroPlanilla, string numeroInstruccion);
    }
}
