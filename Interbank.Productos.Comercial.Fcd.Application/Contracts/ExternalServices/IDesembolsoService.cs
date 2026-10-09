using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.DB2;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices
{
    public interface IDesembolsoService
    {
        Task<InstruccionAbonoResponse> ProcesoInstruccionAbono(InstruccionAbonoRequest instruccionAbonoRequest);
        Task<DesembolsoResponse> DesembolsarPlanilla(DesembolsoRequest desembolsoRequest);
        Task<DesembolsoResponse> EncolamientoDesembolso(DesembolsoRequest desembolsoRequest);
        string InsertDataToDatabase(DataTable dt, DapperParametroDesembolsoH2H dapperParametros);
        string fEjecutarDesembolsoMasivoDB2(string pstrTotalRegistros, string pstrParametro, DapperParametroDesembolsoH2H dapperParametro);
        Task<FcdTablMaes> ObtenerDatosDb2MaesPorPlanillaYSecuencia(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia);
        Task<List<FcdPagoMasiMae>> ObtenerDatosDb2PagoMasiMaesPorPlanillaYSecuencia(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia);
        Task<BaseResponse> ProcesarTramas(ProcesarTramasRequest procesarTramasRequest);
        DapperParametroDesembolsoH2H ObtenerParametrosDesembolso(List<DapperParametro> dapperParametros);

    }
}
