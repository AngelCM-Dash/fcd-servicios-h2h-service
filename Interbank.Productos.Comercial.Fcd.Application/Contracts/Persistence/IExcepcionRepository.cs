using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IExcepcionRepository
    {
        Task<int> GeneraCodigoSecuenciaInteresComision();
        Task<List<DapperComisionProveedor>> ConsultaCalculoInteresComision(int secuencia);
    }
}
