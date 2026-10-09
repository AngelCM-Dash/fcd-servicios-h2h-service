namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperNumeroOperacionDesembolso
    {
        public string? NumeroOperacion { get; set; }
        public string? NumeroInstruccion { get; set; }
        public int NumeroLinea { get; set; }
        public decimal Neteo { get; set; }
        public string? Tipo { get; set; }
        public int CodigoMonedaWbc { get; set; }
    }
}
