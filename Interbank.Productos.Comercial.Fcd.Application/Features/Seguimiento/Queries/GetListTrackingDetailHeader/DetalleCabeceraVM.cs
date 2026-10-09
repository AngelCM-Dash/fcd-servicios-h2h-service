namespace Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class DetalleCabeceraVM
    {
        public decimal IdSeguimiento { get; set; }
        public decimal IdDetalle { get; set; }
        public string? FechaRegistro { get; set; }
        public string? NombreArchivo { get; set; }
        public string? NroPlanilla { get; set; }
        public string? NombreMetodo { get; set; }
        public string? CapaObservacion { get; set; }
        public string? DetalleObservacion { get; set; }
        public decimal IdEstacion { get; set; }

    }
}
