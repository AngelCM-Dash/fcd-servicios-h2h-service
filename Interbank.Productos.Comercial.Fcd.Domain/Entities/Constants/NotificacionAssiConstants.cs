using System.Collections.Immutable;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants
{
    public class NotificacionAssiConstants
    {
        public const string CargaMasivaPlanilla = "CARGA_MASIVA";
        public const string DesembolsoDistribuido = "DESEMBOLSO_DISTRIBUIDO";
        public const string DesembolsoDiferido = "DESEMBOLSO_DIFERIDO";
        public const string DesembolsoErrado = "DESEMBOLSO_ERRADO";
        public const string ConfirmacionDesembolso = "CONFIRMACION_DE_DESEMBOLSO";
        public const string AutoExcepcionFacturasACargo = "AUTOEXCEPCION_FACTURAS_A_CARGO";
        public const string ReglaDocumentoDuplicados = "REGLA_DOCUMENTOS_DUPLICADOS";
        public const string LiberacionReserva = "ERROR_EN_LIBERACION_RESERVA";

        public enum CodigosEvento
        {
            DesembolsoDistribuido = 1,
            DesembolsoDiferido = 2,
            DesembolsoErrado = 3,
            CargaMasiva = 4,
            ConfirmacionDesembolso = 5,
        }

        public enum TipoNotificacion
        {
            NotificacionAssiIFX = 0,
            NotificacionAssiAPI = 1,
        }

        public enum CodigosRespuesta
        {
            ErrorGenerico = 36,
            Ok = 32,
            DocumentosDuplicados = 38,
            FacturasPendientes = 39,
        }


        public static readonly ImmutableDictionary<int, string> DescripcionesEventos = new Dictionary<int, string>
        {
            { (int)CodigosEvento.DesembolsoDistribuido, "DESEMBOLSO_DISTRIBUIDO" },
            { (int)CodigosEvento.DesembolsoDiferido, "DESEMBOLSO_DIFERIDO" },
            { (int)CodigosEvento.DesembolsoErrado, "DESEMBOLSO_ERRADO" },
            { (int)CodigosEvento.CargaMasiva, "CARGA_MASIVA" },
            { (int)CodigosEvento.ConfirmacionDesembolso, "CONFIRMACION_DE_DESEMBOLSO" }
        }.ToImmutableDictionary();

        public static string ObtenerDescripcionEvento(int codigoEvento)
        {
            if (DescripcionesEventos.TryGetValue(codigoEvento, out string? descripcion))
            {
                return descripcion;
            }
            else
            {
                return "Código de evento no encontrado.";
            }
        }


    }
}
