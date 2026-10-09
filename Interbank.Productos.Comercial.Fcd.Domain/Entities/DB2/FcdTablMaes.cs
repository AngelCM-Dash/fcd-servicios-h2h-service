namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.DB2
{
    public class FcdTablMaes
    {
        public string? NumeroPlanilla { get; set; }
        public int NumeroSecuenciaPlanilla { get; set; }
        public string? CodigoUnico { get; set; }
        public string? FechaProceso { get; set; }
        public int NumeroRegistro { get; set; }
        public int NumeroRegistroProceso { get; set; }
        public string? HoraInicialProceso { get; set; }
        public string? HoraFinalProceso { get; set; }
        public string? CodigoEstatusRetorno { get; set; }
        public string? CodigoEstatusProceso { get; set; }
        public string? DescripcionMensaje { get; set; }
    }
}
