namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Base
{
    public class ReservaLineaBase
    {
        public int? CodigoSolicitud { get; set; }
        public string? LcNumeroLinea { get; set; }
        public string? DesembolsoNumeroOperacion { get; set; }
        public string? CodigoMoneda { get; set; }
        public string? CodigoUnico { get; set; }
        public string? LocacionLinea1 { get; set; }
        public decimal? LocacionMonto1 { get; set; }
        public string? LocacionLinea2 { get; set; }
        public decimal? LocacionMonto2 { get; set; }
        public string? LocacionLinea3 { get; set; }
        public decimal? LocacionMonto3 { get; set; }
        public string? LocacionLinea4 { get; set; }
        public decimal? LocacionMonto4 { get; set; }
        public decimal? LineaMontoOperacion { get; set; }
    }
}
