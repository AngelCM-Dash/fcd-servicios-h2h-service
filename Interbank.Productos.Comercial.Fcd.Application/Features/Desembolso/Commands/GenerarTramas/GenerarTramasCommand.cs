namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.GenerarTramas
{
    public class GenerarTramasCommand
    {
        public required string numeroPlanilla { get; set; }
        public string? numeroInstruccion { get; set; }
        public required int numeroPlanillaSecuencia { get; set; }
        public required string usuarioEjecuta { get; set; }
        public required int flagDesembolsar { get; set; }
    }
}
