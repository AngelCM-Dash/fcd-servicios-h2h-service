using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persintence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    public class ClienteAfiliacionRepository : RepositoryBase<EFClienteAfiliacion>, IClienteAfiliacionRepository
    {
        public ClienteAfiliacionRepository(FcdDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<EFClienteAfiliacion>> GetSuppliersByAcceptor(string cuAceptante, int? CodProducto)
        {
            IQueryable<EFClienteAfiliacion> query;
            if (CodProducto != 99)
            {
                query = _context.Afiliaciones!
                    .Where(c => c.CodigoUnicoAceptante == cuAceptante)
                    .Where(c => !CodProducto.HasValue || c.CodigoProducto == CodProducto.Value);
            }
            else
            {
                query = _context.Afiliaciones!
                    .Where(c => c.CodigoUnico == cuAceptante);
            }

            return await query.ToListAsync();


        }
    }
}
