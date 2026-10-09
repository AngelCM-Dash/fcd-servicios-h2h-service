using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence
{
    public interface IAfiliacionRepository
    {
        Task<int> ObtenerAfiliacion_H2H(DapperAfiliacionProveedor afiliacion);
        Task<int> ObtenerProveedorCliente_H2H(DapperAfiliacionProveedor afiliacion);
        Task<AfiliacionResponse> RegistrarAfiliacionProveedorFCD_H2H(DapperAfiliacionProveedor afiliacion);
        Task<AfiliacionResponse> RegistrarProveedorFCD_H2H(DapperProveedor proveedor);
    }
}
