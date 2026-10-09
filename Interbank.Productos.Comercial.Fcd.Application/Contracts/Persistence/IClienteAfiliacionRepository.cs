using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persintence
{
    public interface IClienteAfiliacionRepository : IAsyncRepository<EFClienteAfiliacion>
    {
        Task<IEnumerable<EFClienteAfiliacion>> GetSuppliersByAcceptor(string cuAceptante, int? CodProducto);
    }
}
