namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperConsumoLinea
    {
        public string? NumeroInstruccion { get; set; }
        public string? NumeroLinea { get; set; }
        public decimal Neteo { get; set; }
        public string? Tipo { get; set; }
        public int CodigoMonedaWBC { get; set; }
        public string? NumeroOperacion { get; set; }
    }
}
