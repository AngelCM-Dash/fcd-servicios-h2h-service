namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class ProcesarTramasRequest
    {
        public bool? FlagMonitor { get; set; }
        public string? NumeroPlanilla { get; set; }
        public int NumeroSecuencia { get; set; }
        public int TipoProcesamiento { get; set; }
    }
}
