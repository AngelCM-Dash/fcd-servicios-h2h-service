namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperTmpDesembolsoAbono
    {
        public string? NumeroPlanilla { get; set; }
        public int NumeroSecuencia { get; set; }
        public string? CodigoUnicoProveedor { get; set; }
        public int NumeroRegistrosProcesados { get; set; }
        public int NumeroReintento { get; set; }
        public int EstadoReintento { get; set; }
        public int TipoReintento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
