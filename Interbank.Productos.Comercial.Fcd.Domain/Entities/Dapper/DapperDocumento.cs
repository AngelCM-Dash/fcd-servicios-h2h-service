using Interbank.Productos.Comercial.Fcd.Domain.Entities.Base;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities
{
    public class DapperDocumento : PlanillaBase
    {
        public int TipoDocumentoCobranza { get; set; }
        public string? NumeroDocumentoFisico { get; set; }
        public string? FechaVecimiento { get; set; }
        public string? CodigoMoneda { get; set; }
        public double ImporteTotal { get; set; }
        public int TipoAbono { get; set; }
        public int TipoCuentaAbono { get; set; }
        public string? OficinaAbono { get; set; }
        public string? CuentaAbono { get; set; }
        public string? CodigoTipoDocumentoIdentidad { get; set; }
        public string? NumerodocumentoIdentidad { get; set; }
        public string? RazonSocial { get; set; }
        public string? FechaCargo { get; set; }
        public string? FechaAdelanto { get; set; }
    }
}
