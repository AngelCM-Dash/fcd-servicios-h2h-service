namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Query.ConsultaNroOperacionDesembolso
{
    public class GetNroOperacionMovimientoQueryVM
    {
        public string? NumeroOperacion { get; set; }
        public string? NumeroInstruccion { get; set; }
        public int NumeroLinea { get; set; }
        public decimal Neteo { get; set; }
        public string? Tipo { get; set; }
        public int CodigoMonedaWbc { get; set; }
    }
}
