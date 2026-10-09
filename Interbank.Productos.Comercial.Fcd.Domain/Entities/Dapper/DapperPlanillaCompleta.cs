using Interbank.Productos.Comercial.Fcd.Domain.Entities.Base;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper
{
    public class DapperPlanillaCompleta : PlanillaBase
    {
        public int? CodigoProducto { get; set; }
        public string? CanalAtencion { get; set; }
        public string? CodigoUsuario { get; set; }
        public string? Usuario { get; set; }
        public int? CodigoPerfilUsuario { get; set; }
        public int? CodigoTienda { get; set; }
        public string? Tienda { get; set; }
        public bool? ContratoMarco { get; set; }
        public string? CodigoUnico { get; set; }
        public string? RutaArchivo { get; set; }
        public string? NombreArchivo { get; set; }
        public int CodigoMoneda { get; set; }
        public int TotalDocumentosPlanilla { get; set; }
        public int InfoTotalDocsPlanilla { get; set; }
        public decimal ImporteCuenta { get; set; }
        public decimal ImporteTotalPlanilla { get; set; }
        public int CodigotipoCuentaAbono { get; set; }
        public int CodigoCuentaComisiones { get; set; }
        public string? NumeroCuentaAbono { get; set; }
        public string? NumeroCuentaComisiones { get; set; }
        public int CodigoEstado { get; set; }
        public int CodigoCliente { get; set; }
        public int FlagControlFlujo { get; set; }
        public int CodigoFormaOperacion { get; set; }
        public int CodigoTipoCobranza { get; set; }
        public int CodigotipoCuentaCargo { get; set; }
        public string? NumeroCuentaCargo { get; set; }
        public int AplicaInteresMoratorio { get; set; }
        public int AplicaInteresCompensatorio { get; set; }
        public int CodigoModalidadProducto { get; set; }
        public int CodigoTipoAdelanto { get; set; }
        public DateTime FechaAdelanto { get; set; }
        public int CodigoModalidadAdelanto { get; set; }
        public int AsumeInteres { get; set; }
        public string? Observacion { get; set; }
        public string? NumeroLineaCliente { get; set; }
        public DateTime? FechaDesembolso { get; set; }
        public decimal ImporteTotalRegistrado { get; set; }
        public decimal ComisionIBK { get; set; }
        public decimal ComisionFactor { get; set; }
        public string? CuFactor { get; set; }
        public string? CuBancoEmisor { get; set; }
        public string? PlazaCuenta { get; set; }
        public decimal ImporteFlat { get; set; }
        public string? NumeroOperacion { get; set; }
        public int CodigoMonedaWDC { get; set; }
        public int CodigoMonedaCta { get; set; }
        public decimal ImporteWDC { get; set; }
        public int ImporteCta { get; set; }
        public decimal TasaDescuento { get; set; }
        public int ItemDeudor { get; set; }
        public decimal TipoCambioWDC { get; set; }
        public int NumeroLineaFactor { get; set; }
        public decimal CostoFondo { get; set; }
        public string[]? Contenido { get; set; }

        public string? CodigoArchivo { get; set; }
        public string? RutaArchivoTemp { get; set; }
    }
}
