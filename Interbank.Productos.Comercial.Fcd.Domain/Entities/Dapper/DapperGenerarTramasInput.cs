namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperGenerarTramasInput
    {
        public required string NumeroPlanilla { get; set; }
        public string? NumeroInstruccion { get; set; }
        public string? CodigoUnicoProveedor { get; set; }
        public required int NumeroPlanillaSecuencia { get; set; }
        public required string UsuarioEjecuta { get; set; }
        public required int FlagDesembolsar { get; set; }





    }
}
