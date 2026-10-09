using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList
{
    public class GetSuppliersListQuery : IRequest<List<ClienteAfiliacionVM>>
    {
        public string CodigoUnicoAceptante { get; set; } = string.Empty;
        public string? CodigoProducto { get; set; }

        public GetSuppliersListQuery(string codigounicoaceptante, string? codigoProducto)
        {
            CodigoUnicoAceptante = long.TryParse(codigounicoaceptante, out _) ? codigounicoaceptante.PadLeft(10, '0') : codigounicoaceptante;
            CodigoProducto = codigoProducto;

        }
    }
}
