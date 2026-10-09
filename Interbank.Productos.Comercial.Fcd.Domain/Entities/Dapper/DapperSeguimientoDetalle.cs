namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperSeguimientoDetalle
    {
        public decimal IdDetalle { get; set; }
        public string? NombreMetodo { get; set; }
        public string? CapaObservacion { get; set; }
        public string? DetalleObservacion { get; set; }
        public decimal? IdEstacion { get; set; }

    }
}
