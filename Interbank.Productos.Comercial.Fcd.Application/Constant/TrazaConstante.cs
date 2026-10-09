namespace Interbank.Productos.Comercial.Fcd.Application.Constant
{
    public static class TrazaConstante
    {
        public const string TrazaCargaDocsI = " Inicio : CargaMasivaPlanillas - CreatePlanillasCommandHandler - Registrar_SqlLoaderDocumentosFCD_H2H";
        public const string TrazaCargaDocsF = " Fin : CargaMasivaPlanillas - CreatePlanillasCommandHandler - Registrar_SqlLoaderDocumentosFCD_H2H";

        public const string TrazaCargaI = " Inicio : CargaMasivaPlanillas - CreatePlanillasCommandHandler ";
        public const string TrazaCargaF = " Fin : CargaMasivaPlanillas - CreatePlanillasCommandHandler ";
        public const string TrazaCargaE = " Error : CargaMasivaPlanillas - CreatePlanillasCommandHandler ";

        public const string TrazaObtConfigSFTP_I = " Inicio : CargaMasivaPlanillas - ObtenerConfiguracionSFTP_H2H ";
        public const string TrazaObtConfigSFTP_F = " Fin : CargaMasivaPlanillas - ObtenerConfiguracionSFTP_H2H ";
        public const string TrazaObtConfigSFTP_E = " Error : CargaMasivaPlanillas - ObtenerConfiguracionSFTP_H2H ";

        public const string TrazaObtConfigCTL_I = " Inicio : CargaMasivaPlanillas - ObtenerConfiguracionCTL ";
        public const string TrazaObtConfigCTL_F = " Fin : CargaMasivaPlanillas - ObtenerConfiguracionCTL ";
        public const string TrazaObtConfigCTL_E = " Error : CargaMasivaPlanillas - ObtenerConfiguracionCTL ";

        public const string TrazaGeneraArchivoCTL_I = " Inicio : CargaMasivaPlanillas - GenerarArchivoCtlCargaMasiva";
        public const string TrazaGeneraArchivoCTL_F = " Fin : CargaMasivaPlanillas - GenerarArchivoCtlCargaMasiva";
        public const string TrazaGeneraArchivoCTL_E = " Error : CargaMasivaPlanillas - GenerarArchivoCtlCargaMasiva";

        public const string TrazaExecuteSqlLoader_I = " Inicio : CargaMasivaPlanillas - ExecuteSQLLoader";
        public const string TrazaExecuteSqlLoader_F = " Fin : CargaMasivaPlanillas - ExecuteSQLLoader";
        public const string TrazaExecuteSqlLoader_E = " Error : CargaMasivaPlanillas - ExecuteSQLLoader";

        public const string TrazaBulkInsertDocumentos_I = " Inicio : CargaMasivaPlanillas - Bulkado Documento";
        public const string TrazaBulkInsertDocumentos_F = " Fin : CargaMasivaPlanillas - Bulkado Documento";
        public const string TrazaBulkInsertDocumentos_E = " Error : CargaMasivaPlanillas - Bulkado Documento";

        public const string TrazaRegistroDocumentos_I = " Inicio : CargaMasivaPlanillas - RegistraDocumentosPlanilla";
        public const string TrazaRegistroDocumentos_F = " Fin : CargaMasivaPlanillas - RegistraDocumentosPlanilla";
        public const string TrazaRegistroDocumentos_E = " Error : CargaMasivaPlanillas - RegistraDocumentosPlanilla";

        public const string TrazaRechazoPlanilla_I = " Inicio : CargaMasivaPlanillas - RechazoPlanilla";
        public const string TrazaRechazoPlanilla_F = " Fin : CargaMasivaPlanillas - RechazoPlanilla";
        public const string TrazaRechazoPlanilla_E = " Error : CargaMasivaPlanillas - RechazoPlanilla";

        public const string TrazaRegistraDocCuota_I = " Inicio : CargaMasivaPlanillas - RegistrarDocCuota_DocEstadoFCD_H2H";
        public const string TrazaRegistraDocCuota_F = " Fin : CargaMasivaPlanillas - RegistrarDocCuota_DocEstadoFCD_H2H";
        public const string TrazaRegistraDocCuota_E = " Error : CargaMasivaPlanillas - RegistrarDocCuota_DocEstadoFCD_H2H";

        public const string TrazaNotificacionAssi_I = " Inicio : CargaMasivaPlanillas - NotificacionAssi";
        public const string TrazaNotificacionAssi_F = " Fin : CargaMasivaPlanillas - NotificacionAssi";
        public const string TrazaNotificacionAssi_E = " Error : CargaMasivaPlanillas - NotificacionAssi";

        public const string TrazaCargaPlaI = " Inicio : CargaMasivaPlanillas - RegistrarPlanillaFCD_H2H";
        public const string TrazaCargaPlaF = " Fin : CargaMasivaPlanillas - RegistrarPlanillaFCD_H2H";
        public const string TrazaCargaPlaE = " Error : CargaMasivaPlanillas - RegistrarPlanillaFCD_H2H";

        public const string TrazaEncolamientoPlanillaI = " Inicio : CargaMasivaPlanillas - Enqueue";
        public const string TrazaEncolamientoPlanillaF = " Fin : CargaMasivaPlanillas - Enqueue";
        public const string TrazaEncolamientoPlanillaE = " Error : CargaMasivaPlanillas - Enqueue";



        public const string TrazaErrorCargaPlanilla = " Error : CargaMasivaPlanillas - CreatePlanillasCommandHandler - RegistrarPlanilla / Registrar Documentos";

        public const string TrazaDatosPlanillaSFTP_I = " Inicio: CreatePlanillasCommandHandler - ObtenerDatosPlanillaSFTP ";
        public const string TrazaDatosPlanillaSFTP_F = " Fin : CreatePlanillasCommandHandler - ObtenerDatosPlanillaSFTP ";
        public const string TrazaDatosPlanillaSFTP_E = "Error : CreatePlanillasCommandHandler - ObtenerDatosPlanillaSFTP";

        public const string TrazaDatosPlanillaNAS_I = " Inicio: CreatePlanillasCommandHandler - ObtenerDatosPlanillaNAS ";
        public const string TrazaDatosPlanillaNAS_F = " Fin : CreatePlanillasCommandHandler - ObtenerDatosPlanillaNAS ";
        public const string TrazaDatosPlanillaNAS_E = "Error : CreatePlanillasCommandHandler - ObtenerDatosPlanillaNAS";

        public const string TrazaObtenerIDSeguimiento = "Inicio:ObtenerIdSeguimiento-ActualizarPlanillaHandler";
        public const string TrazaActualizacionPlanillaI = "Inicio:ActualizacionPlanillas-ActualizarPlanillaHandler";
        public const string TrazaActualizacionPlanillaF = "Fin:ActualizacionPlanillas-ActualizarPlanillaHandler";
        public const string TrazaActualizacionPlanillaE = "Error:ActualizacionPlanillas-ActualizarPlanillaHandler";
        public const string TrazaAObtenerConfiguracionDesembolsoI = "Inicio:ObtenerConfiguracionDesembolso-ActualizarPlanillaHandler";
        public const string TrazaAObtenerConfiguracionDesembolsoF = "Fin:ObtenerConfiguracionDesembolso-ActualizarPlanillaHandler";

        public const string TrazaAObtenerPlanillaSecuenciaI = "Inicio:TrazaAObtenerPlanillaSecuenciaI-ActualizarPlanillaHandler";
        public const string TrazaAObtenerPlanillaSecuenciaF = "Fin:TrazaAObtenerPlanillaSecuenciaF-ActualizarPlanillaHandler";

        public const string TrazaAObtenerPlanillaSecuenciaE = "Error:TrazaAObtenerPlanillaSecuenciaE-ActualizarPlanillaHandler";
        public const string TrazaAGenerarTramasDesembolsoI = "Inicio:TrazaAGenerarTramasDesembolsoI-ActualizarPlanillaHandler";
        public const string TrazaAGenerarTramasDesembolsoF = "Fin:TrazaAGenerarTramasDesembolsoF-ActualizarPlanillaHandler";
        public const string TrazaAGenerarTramasDesembolsoE = "Error:TrazaAGenerarTramasDesembolsoF-ActualizarPlanillaHandler";
        public const string TrazaAObtenerFlujoDesembolsoI = "Inicio:TrazaAObtenerFlujoDesembolsoI-ActualizarPlanillaHandler";
        public const string TrazaAObtenerFlujoDesembolsoF = "Fin:TrazaAObtenerFlujoDesembolsoF-ActualizarPlanillaHandler";


        public const string TrazaEscribirFileServerI = "Inicio:TrazaEscribirFileServerI-ActualizarPlanillaHandler";
        public const string TrazaEscribirFileServerF = "Fin:TrazaEscribirFileServerF-ActualizarPlanillaHandler";
        public const string TrazaTransferirArchivoHostI = "Inicio:TrazaTransferirArchivoHostI-ActualizarPlanillaHandler";
        public const string TrazaTransferirArchivoHostF = "Fin:TrazaTransferirArchivoHostI-ActualizarPlanillaHandler";
        public const string TrazaEjecutarDesembolsoDb2I = "Inicio:TrazaEjecutarDesembolsoDb2I-ActualizarPlanillaHandler";
        public const string TrazaEjecutarDesembolsoDb2F = "Fin:TrazaEjecutarDesembolsoDb2F-ActualizarPlanillaHandler";


        public const string TrazaVerificarAfiliacion_I = " Inicio : CargaMasivaPlanillas - ObtenerConfiguracionSFTP_H2H ";
        public const string TrazaVerificarAfiliacion_F = " Fin : CargaMasivaPlanillas - ObtenerConfiguracionSFTP_H2H ";

        public const string TrazaErrorCargaMasiva = " Error : CargaMasiva- CreatePlanillasCommandHandler - Error en el proceso";

        //DESEMBOLSO TRAZAS
        public const string TrazaDesembolsoMasivoI = "Inicio : DesembolsoMasivo - ActualizarPlanillaHandler";
        public const string TrazaDesembolsoMasivoF = "Fin : DesembolsoMasivo - ActualizarPlanillaHandler";
        public const string TrazaDesembolsoMasivoE = "Error : DesembolsoMasivo - ActualizarPlanillaHandler";

        public const string TrazaObtenerParamDesembolsoI = "Inicio : DesembolsoMasivo - ObtenerParametrosDesembolso";
        public const string TrazaObtenerParamDesembolsoF = "Fin : DesembolsoMasivo - ObtenerParametrosDesembolso";
        public const string TrazaObtenerParamDesembolsoE = "Inicio : DesembolsoMasivo - ObtenerParametrosDesembolso";

        public const string TrazaSecuenciaPlanillaI = "Inicio : DesembolsoMasivo - ObtenerPlanillaSecuencia";
        public const string TrazaSecuenciaPlanillaF = "Fin : DesembolsoMasivo - ObtenerPlanillaSecuencia";
        public const string TrazaSecuenciaPlanillaE = "Error : DesembolsoMasivo - ObtenerPlanillaSecuencia";

        public const string TrazaActualizaPlanillaDesembolsoI = "Inicio : DesembolsoMasivo - ActualizarPlanilla";
        public const string TrazaActualizaPlanillaDesembolsoF = "Fin : DesembolsoMasivo - ActualizarPlanilla";
        public const string TrazaActualizaPlanillaDesembolsoE = "Error : DesembolsoMasivo - ActualizarPlanilla";

        public const string TrazaGenerarTramasDesembolsoI = "Inicio : DesembolsoMasivo - GenerarTramas";
        public const string TrazaGenerarTramasDesembolsoF = "Fin : DesembolsoMasivo - GenerarTramas";
        public const string TrazaGenerarTramasDesembolsoE = "Error : DesembolsoMasivo - GenerarTramas";

        public const string TrazaObtenerFlujoDesembolsoI = "Inicio : DesembolsoMasivo - ProcesarFlujoDesembolso";
        public const string TrazaObtenerFlujoDesembolsoF = "Fin : DesembolsoMasivo - ProcesarFlujoDesembolso";
        public const string TrazaObtenerFlujoDesembolsoE = "Error : DesembolsoMasivo - ProcesarFlujoDesembolso";

        public const string TrazaProcesarNuevoFlujoDesembolsoI = "Inicio : DesembolsoMasivo - ProcesarNuevoFlujoDesembolso";
        public const string TrazaProcesarNuevoFlujoDesembolsoF = "Fin : DesembolsoMasivo - ProcesarNuevoFlujoDesembolso";
        public const string TrazaProcesarNuevoFlujoDesembolsoE = "Error : DesembolsoMasivo - ProcesarNuevoFlujoDesembolso";

        public const string TrazaRegistraTablaMaesI = "Inicio : DesembolsoMasivo - ProcesarTramasTRM";
        public const string TrazaRegistraTablaMaesF = "Fin : DesembolsoMasivo - ProcesarTramasTRM";
        public const string TrazaRegistraTablaMaesE = "Error : DesembolsoMasivo - ProcesarTramasTRM";

        public const string TrazaProcesoAbonoDb2I = "Inicio : DesembolsoMasivo - ProcesoInstruccionAbono";
        public const string TrazaProcesoAbonoDb2F = "Fin : DesembolsoMasivo - ProcesoInstruccionAbono";
        public const string TrazaProcesoAbonoDb2E = "Error : DesembolsoMasivo - ProcesoInstruccionAbono";

        public const string TrazaProcesarEstadoAbonoI = "Inicio : DesembolsoMasivo - ProcesarEstadoAbono";
        public const string TrazaProcesarEstadoAbonoF = "Fin : DesembolsoMasivo - ProcesarEstadoAbono";
        public const string TrazaProcesarEstadoAbonoE = "Error : DesembolsoMasivo - ProcesarEstadoAbono";

        public const string TrazaProcesarAbonoTotalFinalizadoCorrectamenteI = "Inicio : DesembolsoMasivo - ProcesarAbonoTotalFinalizadoCorrectamente";
        public const string TrazaProcesarAbonoTotalFinalizadoCorrectamenteF = "Fin : DesembolsoMasivo - ProcesarAbonoTotalFinalizadoCorrectamente";
        public const string TrazaProcesarAbonoTotalFinalizadoCorrectamenteE = "Error : DesembolsoMasivo - ProcesarAbonoTotalFinalizadoCorrectamente";

        public const string TrazaProcesarAbonoParcialFinalizadoCorrectamenteI = "Inicio : DesembolsoMasivo - ProcesarAbonoParcialFinalizadoCorrectamente";
        public const string TrazaProcesarAbonoParcialFinalizadoCorrectamenteF = "Fin : DesembolsoMasivo - ProcesarAbonoParcialFinalizadoCorrectamente";
        public const string TrazaProcesarAbonoParcialFinalizadoCorrectamenteE = "Error : DesembolsoMasivo - ProcesarAbonoParcialFinalizadoCorrectamente";

        public const string TrazaReintentoAbonoFallidoTotalI = "Inicio : DesembolsoMasivo - ReintentoAbonoFallidoTotal";
        public const string TrazaReintentoAbonoFallidoTotalF = "Fin : DesembolsoMasivo - ReintentoAbonoFallidoTotal";
        public const string TrazaReintentoAbonoFallidoTotalE = "Error : DesembolsoMasivo - ReintentoAbonoFallidoTotal";

        public const string TrazaReintentoAbonoFallidoParcialI = "Inicio : DesembolsoMasivo - ReintentoAbonoFallidoParcial";
        public const string TrazaReintentoAbonoFallidoParcialF = "Fin : DesembolsoMasivo - ReintentoAbonoFallidoParcial";
        public const string TrazaReintentoAbonoFallidoParcialE = "Error : DesembolsoMasivo - ReintentoAbonoFallidoParcial";

        public const string TrazaReintentoAbonoFallidoParcialTotalI = "Inicio : DesembolsoMasivo - ReintentoAbonoFallidoParcialTotal";
        public const string TrazaReintentoAbonoFallidoParcialTotalF = "Fin : DesembolsoMasivo - ReintentoAbonoFallidoParcialTotal";
        public const string TrazaReintentoAbonoFallidoParcialTotalE = "Error : DesembolsoMasivo - ReintentoAbonoFallidoParcialTotal";

        public const string TrazaProcesarAbonoFinalizadoFallidoI = "Inicio : DesembolsoMasivo - ProcesarAbonoFinalizadoFallido";
        public const string TrazaProcesarAbonoFinalizadoFallidoF = "Fin : DesembolsoMasivo - ProcesarAbonoFinalizadoFallido";
        public const string TrazaProcesarAbonoFinalizadoFallidoE = "Error : DesembolsoMasivo - ProcesarAbonoFinalizadoFallido";

        public const string TrazaProcesarAbonoPorErrorI = "Inicio : DesembolsoMasivo - ProcesarAbonoPorError";
        public const string TrazaProcesarAbonoPorErrorF = "Fin : DesembolsoMasivo - ProcesarAbonoPorError";
        public const string TrazaProcesarAbonoPorErrorE = "Error : DesembolsoMasivo - ProcesarAbonoPorError";

        public const string TrazaNotificacionAssiDesembolsoI = " Inicio : DesembolsoMasivo - NotificacionAssi";
        public const string TrazaNotificacionAssiDesembolsoF = " Fin : DesembolsoMasivo - NotificacionAssi";
        public const string TrazaNotificacionAssiDesembolsoE = " Error : DesembolsoMasivo - NotificacionAssi";

        //PROCESO VALIDACION DESEMBOLSO
        public const string TrazaValidaAbonoDesembolsoI = "Inicio : ValidarEstadoProcesoAbono - ValidacionAbonoDesembolso";
        public const string TrazaValidaAbonoDesembolsoF = "Fin : ValidarEstadoProcesoAbono - ValidacionAbonoDesembolso";
        public const string TrazaValidaAbonoDesembolsoE = "Error : ValidarEstadoProcesoAbono - ValidacionAbonoDesembolso";

        public const string TrazaValidaReintentoInicialI = "Inicio : ValidarEstadoProcesoAbono - ProcesarIntentoInicial";
        public const string TrazaValidaReintentoInicialF = "Fin : ValidarEstadoProcesoAbono - ProcesarIntentoInicial";
        public const string TrazaValidaReintentoInicialE = "Error : ValidarEstadoProcesoAbono - ProcesarIntentoInicial";

        public const string TrazaValidaReintentoIntermedioI = "Inicio : ValidarEstadoProcesoAbono - ProcesarReintentoIntermedio";
        public const string TrazaValidaReintentoIntermedioF = "Fin : ValidarEstadoProcesoAbono - ProcesarReintentoIntermedio";
        public const string TrazaValidaReintentoIntermedioE = "Error : ValidarEstadoProcesoAbono - ProcesarReintentoIntermedio";

        public const string TrazaValidaReintentoFinalI = "Inicio : ValidarEstadoProcesoAbono - ProcesarReintentoFinal";
        public const string TrazaValidaReintentoFinalF = "Fin : ValidarEstadoProcesoAbono - ProcesarReintentoFinal";
        public const string TrazaValidaReintentoFinalE = "Error : ValidarEstadoProcesoAbono - ProcesarReintentoFinal";

        public const string TrazaValidaReintentoAbonoFallidoParcialTotalI = "Inicio : ValidarEstadoProcesoAbono - ReintentoAbonoFallidoParcialTotal";
        public const string TrazaValidaReintentoAbonoFallidoParcialTotalF = "Fin : ValidarEstadoProcesoAbono - ReintentoAbonoFallidoParcialTotal";
        public const string TrazaValidaReintentoAbonoFallidoParcialTotalE = "Error : ValidarEstadoProcesoAbono - ReintentoAbonoFallidoParcialTotal";

        public const string TrazaDesembolsoAbonoI = "Inicio : ValidarEstadoProcesoAbono - ProcesaDesembolsoAbono";
        public const string TrazaDesembolsoAbonoF = "Fin : ValidarEstadoProcesoAbono - ProcesaDesembolsoAbono";
        public const string TrazaDesembolsoAbonoE = "Error : ValidarEstadoProcesoAbono - ProcesaDesembolsoAbono";







        //MONITOR TRAZAS

        public const string TrazaProcesarPlanillasI = "Inicio : Monitor - ProcesarPlanillas";
        public const string TrazaProcesarPlanillasF = "Fin : Monitor - ProcesarPlanillas";
        public const string TrazaProcesarPlanillasE = "Error : Monitor - ProcesarPlanillas";

        public const string TrazaObtenerDetallePlanillasProcesadasDB2I = "Inicio : Monitor - ObtenerDetallePlanillasProcesadasDB2";
        public const string TrazaObtenerDetallePlanillasProcesadasDB2F = "Fin : Monitor - ObtenerDetallePlanillasProcesadasDB2";
        public const string TrazaObtenerDetallePlanillasProcesadasDB2E = "Error : Monitor - ObtenerDetallePlanillasProcesadasDB2";

        public const string TrazaInsertarProcesoDetallePlanillaI = "Inicio : Monitor - InsertarProcesoDetallePlanilla";
        public const string TrazaInsertarProcesoDetallePlanillaF = "Fin : Monitor - InsertarProcesoDetallePlanilla";
        public const string TrazaInsertarProcesoDetallePlanillaE = "Error : Monitor - InsertarProcesoDetallePlanilla";

        public const string TrazaRegistrarTramasProcesadasI = "Inicio : Monitor - RegistrarTramasProcesadas";
        public const string TrazaRegistrarTramasProcesadasF = "Fin : Monitor - RegistrarTramasProcesadas";
        public const string TrazaRegistrarTramasProcesadasE = "Error : Monitor - RegistrarTramasProcesadas";

        public const string TrazaRegistrarMovimientosPlanillaProcesadaI = "Inicio : Monitor - RegistrarMovimientosPlanillaProcesada";
        public const string TrazaRegistrarMovimientosPlanillaProcesadaF = "Fin : Monitor - RegistrarMovimientosPlanillaProcesada";
        public const string TrazaRegistrarMovimientosPlanillaProcesadaE = "Error : Monitor - RegistrarMovimientosPlanillaProcesada";

        public const string TrazaActualizarPlanillasProcesadasDB2I = "Inicio : Monitor - ActualizarPlanillasProcesadasDB2";
        public const string TrazaActualizarPlanillasProcesadasDB2F = "Fin : Monitor - ActualizarPlanillasProcesadasDB2";
        public const string TrazaActualizarPlanillasProcesadasDB2E = "Error : Monitor - ActualizarPlanillasProcesadasDB2";

        public const string TrazaRegistrarMovimientosEnLineasWBCI = "Inicio : Monitor - RegistrarMovimientosEnLineasWBC";
        public const string TrazaRegistrarMovimientosEnLineasWBCF = "Fin : Monitor - RegistrarMovimientosEnLineasWBC";
        public const string TrazaRegistrarMovimientosEnLineasWBCE = "Error : Monitor - RegistrarMovimientosEnLineasWBC";

        public const string TrazaObtenerDatosDesembolsoI = "Inicio : Monitor - ObtenerDatosDesembolso";
        public const string TrazaObtenerDatosDesembolsoF = "Fin : Monitor - ObtenerDatosDesembolso";
        public const string TrazaObtenerDatosDesembolsoE = "Error : Monitor - ObtenerDatosDesembolso";

        public const string TrazaObtenerReservasPlanillasI = "Inicio : Monitor - ObtenerReservasPlanillas";
        public const string TrazaObtenerReservasPlanillasF = "Fin : Monitor - ObtenerReservasPlanillas";
        public const string TrazaObtenerReservasPlanillasE = "Error : Monitor - ObtenerReservasPlanillas";

        public const string TrazaEliminaReservaDistribuidoI = "Inicio : Monitor - EliminaReservaDistribuido";
        public const string TrazaEliminaReservaDistribuidoF = "Fin : Monitor - EliminaReservaDistribuido";
        public const string TrazaEliminaReservaDistribuidoE = "Error : Monitor - EliminaReservaDistribuido";

        public const string TrazaObtenerMensajeErrorMonitorPorPlanillaI = "Inicio : Monitor - ObtenerMensajeErrorMonitorPorPlanilla";
        public const string TrazaObtenerMensajeErrorMonitorPorPlanillaF = "Fin : Monitor - ObtenerMensajeErrorMonitorPorPlanilla";
        public const string TrazaObtenerMensajeErrorMonitorPorPlanillaE = "Error : Monitor - ObtenerMensajeErrorMonitorPorPlanilla";

        public const string TrazaObtenerDatosConsumoLineaI = "Inicio : Monitor - ObtenerDatosConsumoLinea";
        public const string TrazaObtenerDatosConsumoLineaF = "Fin : Monitor - ObtenerDatosConsumoLinea";
        public const string TrazaObtenerDatosConsumoLineaE = "Error : Monitor - ObtenerDatosConsumoLinea";

        public const string TrazaRegistrarPlanillaDietarioI = "Inicio : Monitor - RegistrarPlanillaDietario";
        public const string TrazaRegistrarPlanillaDietarioF = "Fin : Monitor - RegistrarPlanillaDietario";
        public const string TrazaRegistrarPlanillaDietarioE = "Error : Monitor - RegistrarPlanillaDietario";

        public const string TrazaGenerarArchivoPlanoI = "Inicio : Monitor - GenerarArchivoPlano";
        public const string TrazaGenerarArchivoPlanoF = "Fin : Monitor - GenerarArchivoPlano";
        public const string TrazaGenerarArchivoPlanoE = "Error : Monitor - GenerarArchivoPlano";

        public const string TrazaNotificacionAssiMonitorI = " Inicio : Monitor - NotificacionAssi";
        public const string TrazaNotificacionAssiMonitorF = " Fin : Monitor - NotificacionAssi";
        public const string TrazaNotificacionAssiMonitorE = " Error : Monitor - NotificacionAssi";



        //VALIDAR DOCUMENTOS DUPLICADOS 
        public const string TrazaValidarDocumentosDuplicadosI = "Inicio : ValidarDocumentosDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaValidarDocumentosDuplicadosF = "Fin : ValidarDocumentosDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaValidarDocumentosDuplicadosE = "Error : ValidarDocumentosDuplicados - ValidateDocumentCommandHandler";

        public const string TrazaValidarPermiteDuplicadosoI = "Inicio : ValidarPermiteDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaValidarPermiteDuplicadosF = "Fin : ValidarPermiteDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaValidarPermiteDuplicadosE = "Error : ValidarPermiteDuplicados - ValidateDocumentCommandHandler";

        public const string TrazaInsertarDetallePlanillaBulkI = "Inicio : InsertarDetallePlanillaBulk - ValidateDocumentCommandHandler";
        public const string TrazaInsertarDetallePlanillaBulkF = "Fin : InsertarDetallePlanillaBulk - ValidateDocumentCommandHandler";
        public const string TrazaInsertarDetallePlanillaBulkE = "Error : InsertarDetallePlanillaBulk - ValidateDocumentCommandHandler";

        public const string TrazaGenerarNotificacionDocumentosDuplicadosI = "Inicio : GenerarNotificacionDocumentosDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaGenerarNotificacionDocumentosDuplicadosF = "Fin : GenerarNotificacionDocumentosDuplicados - ValidateDocumentCommandHandler";
        public const string TrazaGenerarNotificacionDocumentosDuplicadosE = "Error : GenerarNotificacionDocumentosDuplicados - ValidateDocumentCommandHandler";

        public const string TrazaDuplicadosNotificacionAssiI = "Inicio : DuplicadosNotificacionAssi - ValidateDocumentCommandHandler";
        public const string TrazaDuplicadosNotificacionAssiF = "Fin : DuplicadosNotificacionAssi - ValidateDocumentCommandHandler";
        public const string TrazaDuplicadosNotificacionAssiE = "Error : DuplicadosNotificacionAssi - ValidateDocumentCommandHandler";
    }
}
