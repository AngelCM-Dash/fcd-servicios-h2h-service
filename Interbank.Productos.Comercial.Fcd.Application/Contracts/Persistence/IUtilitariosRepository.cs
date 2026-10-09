using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IUtilitariosRepository
    {
        Task<List<DapperParametro>> ObtenerParametrosPorCodigoDominio(int codigoDominio);
        Task<List<DapperParametroDB2>> ObtenerParametrosDB2PorCodigoDominioDB2(int codigoDominio, int numOrden);
        bool BulkInsertTable(DataTable dt, string nombreTabla);
        Task<bool> ForAllInsertDataObject(DataTable dt, string nombreTabla);
        Task ActualizarParametroPorCodigoDominioAndNumOrden(int codigoDominio, string descripcionCorta, int numOrden);


    }
}
