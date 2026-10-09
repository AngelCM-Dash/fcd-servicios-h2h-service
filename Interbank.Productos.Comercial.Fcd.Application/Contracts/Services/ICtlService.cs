using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface ICtlService
    {
        string GenerarArchivoCtlCargaMasiva(string nombreTabla, DapperPlanillaCompleta planilla, string archivoOrigen, List<DapperParametroCtl> columnas);
    }
}
