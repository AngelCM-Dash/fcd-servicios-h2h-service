namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants
{
    public class ParametroConstants
    {
        public const int DominioConfiguracionSFTPCargaMasiva = 118;
        public const int DominioDesembolsoH2H = 119;
        public const int DominioApicProcesoInstruccionAbonoFCD = 121;
        public const int DominioApicTokenAuthorizationFCD = 122;
        public const int DominioServiciosProcesoDesembolso = 123;
        public const int DominioSftpMonitorFCD = 124;
        public const int DominioApicServiciosNotificacionAssi = 127;
        public const int DominioApicTokenAuthorizationWBC = 128;
        public const int DominioApicConsumoLineasWBC = 129;
        public const int DominioApicLiberacionLineasWBC = 130;
        public const int DominioSftpValidacionDocumentos = 132;
        public const int DominioConsultaLineas = 133;
        public const int DominioGestorValidacionCargaMasiva = 135;
        public const int DominioServiciosProcesoCargaMasiva = 136;
        public const int DominioApiServiciosNotificacionAssi = 137;
        public const int DominioConsultaDetalleReservaLineas = 139;

        public const string CanalH2H = "H2H";
        public const string CanalFCD = "FCD";
        public const string CanalWBC = "WBC";
        public const string CanalWDC = "WDC";
        public const string CanalBIE = "BIE";

        public enum NumOrdenDominioCalculoInteresComision
        {

            SftpHost = 10,
            SftpPort = 11,
            SftpUsername = 12,
            SftpPassword = 13,
            SftpRemotePath = 14,
            SftpRemotePathKey = 15,
            FlagRed = 16,
        }

        public enum NumOrdenDominioSftpMonitor
        {

            FlagRed = 1,
            SftpPassword = 2,
            SftpRemotePathKey = 3,
            SftpHost = 4,
            SftpPort = 5,
            SftpUsername = 6,
            SftpRemotePath = 7,

        }

        public enum NumOrdenDominioApicTokenAuthorization
        {

            UrlApic = 1,
            GranType = 2,
            Username = 3,
            Password = 4,
            ClientId = 5,
            clientSecret = 6,
            ScopeTransaccion = 7,
            ScopeConsulta = 8,

        }

        public enum NumOrdenDominioUrlServiciosEncolamientoDesembolso
        {
            UrlApiMascara = 1,
            UrlApiEncolamiento = 2,
            UrlApiProcesoAbono = 3,
        }

        public enum NumOrdenDominioUrlServiciosEncolamientoCargaMasiva
        {
            UrlApiMascara = 1,
            UrlApiEncolamiento = 2,
            UrlApiProcesoCarga = 3,
        }

        public enum NumOrdenDominioCargaMasiva
        {
            ServerSftp = 1,
            PuertoSftp = 2,
            UserSftp = 3,
            PassSftp = 4,
            RutaTemps = 5,
            RutaLogs = 6,
            RutaArchivoPpk = 7,
            FlagCRed = 8,
            SqlLdrCn = 9,
            FlagCargaDocumentos = 18,
            FlagPruebasCargaMasiva = 19,
        }

        public enum NumOrdenDominioDesembolsoDistribuido
        {
            FlagDesembolso = 1,
            PstrFileServerFcd = 2,
            PstrHostCodigoRes = 3,
            PstrHostSolicitud = 4,
            PstrHostFile = 5,
            PstrHostIP = 6,
            PstrHostUsuario = 7,
            PstrHostPassword = 8,
            PstrHostAmbiente = 9,
            PstrHostDsn = 10,
            PstrHostNumeroJob = 11,
            MontoNetoNegaPos = 12,
            IpDb2 = 13,
            PuertoDb2 = 14,
            NombreBaseDatosDb2 = 15,
            UsuarioDb2 = 16,
            PassDb2 = 17,
            AmbienteDb2 = 18,
            TablaPagoMasiMaesDb2 = 19,
            UrlProcesamientoTramas = 21,
            NumeroReintentoProcesoAbono = 25,
            FlagDesembolsoParcialProveedor = 26,
            FlagPruebaDesembolso = 27,
            FlagPruebaMonitor = 28,
            UrlWsFcdExtranet = 29,
            ActionEnvioCorreoDietarios = 30,
            FlagNotificacionAssi = 31,
            FlagLiberacionReserva = 32,
        }

        public enum NumOrdenDominioMonitorDB2
        {

            PlanillasProcesadasDB2 = 1,
            DetallePlanillasProcesadasDB2 = 2,
            ActualizacionPlanillasProcesadasDB2 = 3,
        }

        public enum NumOrdenDominioDesembolsoDB2
        {
            VerificarPlanillasProcesadasDB2 = 1,
            DetallePlanillaTablaMaes = 2,
            DetallePlanillaTablaPagoMasiMaes = 3,
            ActualizacionEstadosTablaMaes = 4,
        }

        public enum NumOrdenDominioGestorValidacionCargaMasiva
        {
            FlagValidacionFacturas = 1,
        }

        public enum NumOrdenDominioGestorValidacionDesembolsoDistribuido
        {
            FlagValidacionDesembolso = 1,
        }

        public enum NumOrdenValidacionFacturas
        {
            RutaArchivoCargar = 8,
        }
    }
}
