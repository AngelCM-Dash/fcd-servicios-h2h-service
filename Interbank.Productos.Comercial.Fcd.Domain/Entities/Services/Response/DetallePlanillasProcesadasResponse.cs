namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class DetallePlanillasProcesadasResponse
    {
        public string? NumeroPlanilla { get; set; }
        public int NumeroSecuenciaPlanilla { get; set; }
        public string? NumeroOperacion { get; set; }
        public string? NumeroSecuenciaOperacion { get; set; }
        public string? CodigoUnico { get; set; }
        public string? NumeroInstruccion { get; set; }
        public string? CodigoRetorno { get; set; }
        public string? DescripcionMensajeError { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? ImporteDesembolso { get; set; }
        public string? CodigoFlagExterno { get; set; }
        public string? NumeroLogExterno { get; set; }
        public string? ImporteComisionCCI { get; set; }
        public string? ImporteComisionIB { get; set; }
        public string? ImporteDesembolsoCCI { get; set; }
        public string? CodigoEstadoPago { get; set; }
        public string? CodigoEnvioCorreo { get; set; }
    }
}
