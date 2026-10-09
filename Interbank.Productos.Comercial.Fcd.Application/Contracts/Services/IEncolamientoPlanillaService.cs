using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IEncolamientoPlanillaService
    {
        Task EnqueueAsync(DapperPlanillaCompleta planilla, string archivoOrigen, string datacn, decimal codigo, int flagCargaDocumentos);
    }
}
