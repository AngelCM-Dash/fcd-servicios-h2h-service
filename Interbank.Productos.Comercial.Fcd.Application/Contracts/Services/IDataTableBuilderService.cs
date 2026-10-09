using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IDataTableBuilderService
    {
        DataTable CrearDataTableDocumento();
        Task LeerArchivoCargarDataTableDocumento(string filePath, DataTable dataTable, DapperPlanillaCompleta dataPlanilla, List<DapperParametroCtl> dataParametroCtl, List<DapperSecuenciaPlanilla> secuenciasNumeroInterno);
        DataTable GeneraDatatableCalculoInteresComision(JObject parametrosJson, int codSecuencia);
        DataTable GeneraDataTableProcesoDetallePlanilla(IEnumerable<DetallePlanillasProcesadasResponse> detallesPlanilla);
    }
}
