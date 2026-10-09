using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class GetListTrackingDetailQuery : IRequest<List<DetalleCabeceraVM>>
    {
        public string? fechaDesde { get; set; }
        public string? fechaHasta { get; set; }
        public string? nombreArchivo { get; set; }
        public string? numeroPlanilla { get; set; }

        public GetListTrackingDetailQuery(string? FechaDesde, string? FechaHasta, string? NombreArchivo, string? NumeroPlanilla)
        {
            fechaDesde = FechaDesde;
            fechaHasta = FechaHasta;
            nombreArchivo = NombreArchivo;
            numeroPlanilla = NumeroPlanilla;

        }
    }
}
