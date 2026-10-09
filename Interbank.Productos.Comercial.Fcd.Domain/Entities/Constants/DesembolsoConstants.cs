namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants
{
    public class DesembolsoConstants
    {
        public const string ProcesoDesembolsosOkMessage = "PROCESO DESEMBOLSOS OK";
        public const string FormatoFecha = "yyyyMMdd";
        public const string CodigoProceso = "CodigoProceso";
        public const string RespuestaCodigo = "RespuestaCodigo";
        public const string RespuestaMensaje = "RespuestaMensaje";
        public const string TRM = "TRM";
        public const string SoapNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string TempuriNamespace = "http://tempuri.org/";

        public enum NumeroReintentoBaseProcesoAbono
        {
            ReintentoCero = 0,
            PrimerReintento = 1,
        }
        public enum EstadoReintentoProcesoAbono
        {
            PorProcesar = 0,
            Enviado = 1,
            Exitoso = 2,
            Fallido = 3,
            Procesado = 4,
            ExitosoParcialFalloDb2 = 5,
            NoRegistradoDb2 = 6,
            FalloGeneracionTrama = 7,
        }

        public enum TipoReintentoProcesoAbono
        {
            Parcial = 1,
            Total = 2,
            ParcialTotal = 3,
        }
    }
}
