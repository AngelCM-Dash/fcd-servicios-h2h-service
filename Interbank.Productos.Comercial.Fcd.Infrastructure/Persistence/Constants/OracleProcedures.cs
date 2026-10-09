namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants
{
    public static class OracleProcedures
    {
        #region PAQUETE FCD ORACLE - PKG_H2HW_TRANSACCION
        public const string PaqueteH2HW_Transaccion = "PKG_H2HW_TRANSACCION";
        public const string createPlanillaCabecera = PaqueteH2HW_Transaccion + ".SP_INS_PLANILLA_H2H";
        public const string GetArchivoPlanilla = PaqueteH2HW_Transaccion + ".SP_SEL_ARCHIVO_PLANILLA_H2H";
        public const string GetCodigoFormaOperacion = PaqueteH2HW_Transaccion + ".SP_SEL_FORMA_OPERACION";
        public const string DocCuotaDocEstado = PaqueteH2HW_Transaccion + ".SP_INS_CARGA_MASIVA2_H2HW";
        public const string RejectPlanilla = PaqueteH2HW_Transaccion + ".SP_RECHAZO_PLANILLA_H2HW";
        public const string createIntermediosDocumentos = PaqueteH2HW_Transaccion + ".SP_INS_TMP_FINAL_H2HW";
        public const string createFinalDocumentos = PaqueteH2HW_Transaccion + ".SP_INS_TAB_FINAL_DOC_H2HW";
        public const string createFinalDocumentos_2 = PaqueteH2HW_Transaccion + ".SP_INS_TEMP_TO_DOCS_H2HW";
        public const string createFinalDocumentos_3 = PaqueteH2HW_Transaccion + ".SP_INS_TEMP_TO_DOCS_H2HW_2";
        public const string GetConfiguracionSFTP = PaqueteH2HW_Transaccion + ".SP_SEL_CONFIGURACION_SFTP";
        public const string GetItemCtlH2H = PaqueteH2HW_Transaccion + ".SP_SEL_CONFIGURACION_CTL";
        public const string ActualizarPlanilla = PaqueteH2HW_Transaccion + ".SP_UPD_DOCPLANILLAVIGENTE";
        public const string GenerarTramas = PaqueteH2HW_Transaccion + ".sp_GET_DesembolsoMasivo";
        public const string SeleccionarNumeroSecPlanilla = PaqueteH2HW_Transaccion + ".SP_SEL_NEXTPLANILLASEC";
        public const string ObtenerFLujoDesembolso = PaqueteH2HW_Transaccion + ".SP_SEL_FLAGFLUJODESEMBOLSO";
        public const string ObtenerConfiguracionGeneralDesembolso = PaqueteH2HW_Transaccion + ".SP_SEL_CONFIGURACIONDESEMBOLSO";
        public const string InsertaSeguimientoDetH2HW = PaqueteH2HW_Transaccion + ".SP_INS_SEGUIMIENTO_DET";
        public const string InsertaSeguimientoCabH2HW = PaqueteH2HW_Transaccion + ".SP_INS_SEGUIMIENTO_CAB";
        public const string ObtenerSeguimientoIDxPlanilla = PaqueteH2HW_Transaccion + ".SP_SEL_IDSEGUIMIENTOXPLANILLA";
        public const string ObtenerInformacionConsumoLineas = PaqueteH2HW_Transaccion + ".sp_get_datosconsumolinea";
        public const string InsertarTramasProcesadas = PaqueteH2HW_Transaccion + ".sp_SET_TramasProcesadas";
        public const string InsertarMovimientosDesembolso = PaqueteH2HW_Transaccion + ".sp_SET_DesembolsoMasivo";
        public const string InsertaDocumentosxPlanilla = PaqueteH2HW_Transaccion + ".SP_INS_DOCUMENTOS_PLANILLA";
        public const string InsertaPlanillaDietario = PaqueteH2HW_Transaccion + ".SP_INS_PLANILLA_DIETARIOS";
        public const string InsertaMensajeErrorMonitor = PaqueteH2HW_Transaccion + ".SP_INS_MONITOR_ERROR";
        public const string RechazoDocumentoDistribuido = PaqueteH2HW_Transaccion + ".SP_RECHAZO_DOCUMENTOS_H2HW";
        public const string InsertaDesembolsoAbonoFallido = PaqueteH2HW_Transaccion + ".SP_SET_DESEMBOLSO_ABONO";
        public const string ActualizarDesembolsoAbonoFallido = PaqueteH2HW_Transaccion + ".SP_UPD_DESEMBOLSO_ABONO";
        public const string GenerarTramasPorProveedor = PaqueteH2HW_Transaccion + ".SP_GET_TRAMAS_DESEM_PROVEEDOR";
        public const string GenerarTramasParcialTotalPorProveedores = PaqueteH2HW_Transaccion + ".SP_GET_TRAMASDESEMPARCIALTOTAL";
        public const string ActualizaObservacionPlanilla = PaqueteH2HW_Transaccion + ".SP_UPD_OBS_PLANILLA";
        public const string InsertaDataBulkInsertForAll = PaqueteH2HW_Transaccion + ".SP_BULK_INSERT_H2H";
        public const string ActualizaParametroPorDominioAndNumOrden = PaqueteH2HW_Transaccion + ".SP_UPD_PARAMETRO";
        #endregion

        #region PAQUETE FCD ORACLE - PKG_H2HW_EXCEPCIONES   
        public const string PaqueteH2HW_Excepciones = "PKG_H2HW_EXCEPCIONES";
        public const string CalculoInteresComision = PaqueteH2HW_Excepciones + ".SP_CALCULO_INTERES_COMISION";
        #endregion

        #region PAQUETE FCD ORACLE - PKG_H2HW_CONSULTAS
        public const string PaqueteH2HW_Consultas = "PKG_H2HW_CONSULTAS";
        public const string ObtenerCodigoSecuenciaCalculo = PaqueteH2HW_Consultas + ".SP_SEL_SECUENCIA_CALCULO";
        public const string ObtenerListadoParametrosPorCodigoDominio = PaqueteH2HW_Consultas + ".SP_SEL_PARAMETROSXDOMINIO";
        public const string ObtenerListadoParametrosDB2PorCodigoDominioDB2 = PaqueteH2HW_Consultas + ".SP_SEL_PARAMETRO_DB2";
        public const string ObtenerDetalleCabecera = PaqueteH2HW_Consultas + ".SP_SEL_DETALLE_CABECERA";
        public const string ObtenerDetallesPlanillaMonitor = PaqueteH2HW_Consultas + ".SP_SEL_DETALLE_PLANILLA";
        public const string ObtenerReservasxPlanilla = PaqueteH2HW_Consultas + ".SP_SEL_RESERVAS_PLANILLA";
        public const string EliminarReservasxPlanilla = PaqueteH2HW_Consultas + ".SP_DEL_RESERVAS_PLANILLA";
        public const string ObtenerCodigoSecuenciaDocumentosDuplicados = PaqueteH2HW_Consultas + ".SP_SEL_SEC_DOC_DUPLICADOS";
        public const string ObtenerDocumentoDuplicados = PaqueteH2HW_Consultas + ".SP_SEL_DOC_DUPLICADOS";
        public const string ValidarPermiteDuplicado = PaqueteH2HW_Consultas + ".SP_SEL_PERMITE_DUPLICADOS";
        public const string ValidarFacturaPendienteCargo = PaqueteH2HW_Consultas + ".SP_SEL_VALIDA_FACTURAS";
        public const string ObtenerDatosPlanilla = PaqueteH2HW_Consultas + ".SP_SEL_INFO_PLANILLA";
        public const string ObtenerDatosDocumento = PaqueteH2HW_Consultas + ".SP_SEL_INFO_DOCUMENTO";
        public const string ObtenerSecuenciasPlanilla = PaqueteH2HW_Consultas + ".SP_SEL_SECUENCIAS_PLANILLA";
        public const string ObtenerMensajeErrorMonitorxPlanilla = PaqueteH2HW_Consultas + ".SP_SEL_MONITOR_ERROR";
        public const string ObtenerEstadoDesembolsoAbonoFallido = PaqueteH2HW_Consultas + ".SP_SEL_ESTADO_DESEMBOLSO_ABONO";
        public const string ObtenerTipoEjecucionxIdDetalle = PaqueteH2HW_Consultas + ".SP_SEL_TIPOEJECUCIONXIDDETALLE";
        public const string ObtenerNumeroOperacionDesembolso = PaqueteH2HW_Consultas + ".SP_SEL_NROOPERACION_DESEMBOLSO";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_H2HW_REGISTRO
        public const string PaqueteH2HW_Registro = "PKG_FCD_REGISTRO";
        public const string ActualizarPlanillaFCD = PaqueteH2HW_Registro + ".sp_upd_DOCPLANILLAVIGENTE";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_FCD_REGISTRO
        public const string PaqueteFCD_Registro = "PKG_FCD_REGISTRO";
        public const string EliminarPlanilla = PaqueteFCD_Registro + ".sp_del_planillasimulador";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_FCD_DESEMBOLSOMASIVO
        public const string PaqueteDesembolsoMasivo = "PKG_FCD_DESEMBOLSOMASIVO";
        public const string FintNextPlanillaSecuencia = PaqueteDesembolsoMasivo + ".SP_SEL_NEXTPLANILLASEC";
        public const string SetTramasProcesadas = PaqueteDesembolsoMasivo + ".sp_SET_TramasProcesadas";
        public const string RegistrarMovimientosDesembolso = PaqueteDesembolsoMasivo + ".sp_SET_DesembolsoMasivo";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_H2HW_DESEMBOLSODIFERIDO
        public const string PaqueteDesembolsoDiferido = "PKG_H2HW_DESEMBOLSODIFERIDO";
        public const string ObtenerListadoPlanillasDiferidas = PaqueteDesembolsoDiferido + ".sp_GET_DiferidoDatos";
        public const string RechazaDocumentoDiferidos = PaqueteDesembolsoDiferido + ".sp_SET_DiferidosError";
        public const string GeneraTramaDiferidos = PaqueteDesembolsoDiferido + ".sp_SET_DiferidoTramas";
        public const string ObtenerTramaDiferidos = PaqueteDesembolsoDiferido + ".sp_GET_DiferidoTramas";
        public const string RechazoDocumentoDiferidos = PaqueteDesembolsoDiferido + ".sp_SET_RechazoDiferidos";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_H2HW_DESEMBOLSOERRADO
        public const string PaqueteDesembolsoErrado = "PKG_H2HW_DESEMBOLSOERRADO";
        public const string ObtenerListadoPlanillasErradas = PaqueteDesembolsoErrado + ".SP_GET_ERRADODATOS";
        public const string GeneraTramaErrados = PaqueteDesembolsoErrado + ".SP_SET_ERRADOTRAMAS";
        public const string ObtenerTramaErrados = PaqueteDesembolsoErrado + ".SP_GET_ERRADOTRAMAS";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_H2HW_PRUEBAS
        public const string PaquetePruebas = "PKG_H2HW_PRUEBAS";
        public const string UpdateTipoCuenta = PaquetePruebas + ".SP_UPD_TIPOCUENTA";
        #endregion

        #region  PAQUETE FCD ORACLE - PKG_FCD_AFILIACION
        public const string PaqueteFCD_AFILIACION = "PKG_FCD_AFILIACION";
        public const string InsertarProveedorAutomatico = PaqueteFCD_AFILIACION + ".sp_ins_CLIENTE_ASSI";
        public const string ValidarAfiliacionExiste = PaqueteFCD_AFILIACION + ".sp_get_Proveedor_ASSI";
        public const string InsertaAfiliacionProveedor = PaqueteFCD_AFILIACION + ".sp_ins_AFILIACION_ASSI";
        public const string ValidarProveedorAfiliacion = PaqueteFCD_AFILIACION + ".sp_get_Validar_ProvInCli_ASSI";
        #endregion
    }
}
