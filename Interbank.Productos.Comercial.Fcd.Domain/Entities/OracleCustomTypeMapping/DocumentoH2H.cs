using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.OracleCustomTypeMapping
{
    [OracleCustomTypeMapping("TOD_DOCUMENTOS_H2H")]
    public class DocumentoH2H : IOracleCustomType, IOracleCustomTypeFactory, INullable
    {
        [OracleObjectMapping("NUMEROINTERNO")]
        public string? NumeroInterno { get; set; }

        [OracleObjectMapping("CODIGOCLIENTE")]
        public int CodigoCliente { get; set; }

        [OracleObjectMapping("ITEM")]
        public int? Item { get; set; }

        [OracleObjectMapping("NUMEROPLANILLA")]
        public string? NumeroPlanilla { get; set; }

        [OracleObjectMapping("CODIGODEPARTAMENTO")]
        public int? CodigoDepartamento { get; set; }

        [OracleObjectMapping("CODIGOPROVINCIA")]
        public int? CodigoProvincia { get; set; }

        [OracleObjectMapping("CODIGODISTRITO")]
        public int? CodigoDistrito { get; set; }

        [OracleObjectMapping("PLAZAREMESA")]
        public string? PlazaRemesa { get; set; }

        [OracleObjectMapping("NUMERODOCUMENTOFISICO")]
        public string? NumeroDocumentoFisico { get; set; }

        [OracleObjectMapping("CODIGOMONEDA")]
        public int? CodigoMoneda { get; set; }

        [OracleObjectMapping("IMPORTEORIGINAL")]
        public decimal? ImporteOriginal { get; set; }

        [OracleObjectMapping("FECHAVENCIMIENTO")]
        public DateTime? FechaVencimiento { get; set; }

        [OracleObjectMapping("FECHAVENCIMIENTO2")]
        public DateTime? FechaVencimiento2 { get; set; }

        [OracleObjectMapping("DIRECCION")]
        public string? Direccion { get; set; }

        [OracleObjectMapping("PROTESTABLE")]
        public int? Protestable { get; set; }

        [OracleObjectMapping("PORCENTAJEDESEMBOLSO")]
        public decimal? PorcentajeDesembolso { get; set; }

        [OracleObjectMapping("CODIGOTIPODOCUMENTOCOBRANZA")]
        public int? CodigoTipoDocumentoCobranza { get; set; }

        [OracleObjectMapping("CODIGOTIPOABONO")]
        public int? CodigoTipoAbono { get; set; }

        [OracleObjectMapping("CODIGOTIPOCUENTA")]
        public int? CodigoTipoCuenta { get; set; }

        [OracleObjectMapping("NUMEROCUENTA")]
        public string? NumeroCuenta { get; set; }

        [OracleObjectMapping("FECHACARGO")]
        public DateTime? FechaCargo { get; set; }

        [OracleObjectMapping("FECHADESEMBOLSO")]
        public DateTime? FechaDesembolso { get; set; }

        [OracleObjectMapping("FECHAULTIMAMODIFICACION")]
        public DateTime? FechaUltimaModificacion { get; set; }

        [OracleObjectMapping("FECHAULTIMARENOVACION")]
        public DateTime? FechaUltimaRenovacion { get; set; }

        [OracleObjectMapping("NUMERORENOVACIONES")]
        public int? NumeroRenovaciones { get; set; }

        [OracleObjectMapping("PROTESTO")]
        public int? Protesto { get; set; }

        [OracleObjectMapping("CODIGOESTADO")]
        public int? CodigoEstado { get; set; }

        [OracleObjectMapping("CODIGOSITUACION")]
        public int? CodigoSituacion { get; set; }

        [OracleObjectMapping("CODIGOPOSTAL")]
        public string? CodigoPostal { get; set; }

        [OracleObjectMapping("CODIGOUSUARIOREGISTRO")]
        public string? CodigoUsuarioRegistro { get; set; }

        [OracleObjectMapping("FECHAREGISTRO")]
        public DateTime? FechaRegistro { get; set; }

        [OracleObjectMapping("OBSERVACION")]
        public string? Observacion { get; set; }

        [OracleObjectMapping("FLAGCOMPLETADO")]
        public int? FlagCompletado { get; set; }

        [OracleObjectMapping("FLAGOBSERVADO")]
        public int? FlagObservado { get; set; }

        [OracleObjectMapping("FLAGCUOTA")]
        public int? FlagCuota { get; set; }

        [OracleObjectMapping("NUMEROCUOTAS")]
        public int? NumeroCuotas { get; set; }

        [OracleObjectMapping("PLAZAPAGO")]
        public int? PlazaPago { get; set; }

        [OracleObjectMapping("INTERESDESCUENTO")]
        public decimal? InteresDescuento { get; set; }

        [OracleObjectMapping("SALDOACTUALDOCUMENTO")]
        public decimal? SaldoActualDocumento { get; set; }

        [OracleObjectMapping("FECHAPROTESTO")]
        public DateTime? FechaProtesto { get; set; }

        [OracleObjectMapping("DIASPROTESTADOS")]
        public int? DiasProtestados { get; set; }

        [OracleObjectMapping("CODIGOESTADOANTERIOR")]
        public int? CodigoEstadoAnterior { get; set; }

        [OracleObjectMapping("FECHACAMBIOESTADO")]
        public DateTime? FechaCambioEstado { get; set; }

        [OracleObjectMapping("NUMEROLINEA")]
        public string? NumeroLinea { get; set; }

        [OracleObjectMapping("SEGMENTOACTUALCLIENTE")]
        public string? SegmentoActualCliente { get; set; }

        [OracleObjectMapping("SEGMENTOANTERIORCLIENTE")]
        public string? SegmentoAnteriorCliente { get; set; }

        [OracleObjectMapping("FECHADEVOLUCION")]
        public DateTime? FechaDevolucion { get; set; }

        [OracleObjectMapping("FECHAACEPTACION")]
        public DateTime? FechaAceptacion { get; set; }

        [OracleObjectMapping("FECHACANCELACION")]
        public DateTime? FechaCancelacion { get; set; }

        [OracleObjectMapping("NUMERODOCUMENTOACEPTANTE")]
        public string? NumeroDocumentoAceptante { get; set; }

        [OracleObjectMapping("RAZONSOCIALACEPTANTE")]
        public string? RazonSocialAceptante { get; set; }

        [OracleObjectMapping("FLAGPENDIENTECOMCOB")]
        public int? FlagPendienteComCob { get; set; }

        [OracleObjectMapping("TIPODOCUMENTOACEPTANTE")]
        public int? TipoDocumentoAceptante { get; set; }

        [OracleObjectMapping("NUMEROASOCIADO")]
        public string? NumeroAsociado { get; set; }

        [OracleObjectMapping("FLAGRECHAZADO2DIGITACION")]
        public int? FlagRechazado2Digitacion { get; set; }

        [OracleObjectMapping("FECHAEXTORNO")]
        public DateTime? FechaExtorno { get; set; }

        [OracleObjectMapping("FECHAREINGRESO")]
        public DateTime? FechaReingreso { get; set; }

        [OracleObjectMapping("NUMEROINSTRUCCION")]
        public string? NumeroInstruccion { get; set; }

        [OracleObjectMapping("CODIGOTIPOADELANTO")]
        public int? CodigoTipoAdelanto { get; set; }

        [OracleObjectMapping("FECHAADELANTO")]
        public DateTime? FechaAdelanto { get; set; }

        [OracleObjectMapping("PORCENTAJEPRORROGA")]
        public decimal? PorcentajeProrroga { get; set; }

        [OracleObjectMapping("INTERESPRORROGA")]
        public decimal? InteresProrroga { get; set; }

        [OracleObjectMapping("APLICAPORTES")]
        public int? AplicaPortes { get; set; }

        [OracleObjectMapping("DEUDOR")]
        public string? Deudor { get; set; }

        [OracleObjectMapping("IMPORTECOMISIONIBK")]
        public decimal? ImporteComisionIBK { get; set; }

        [OracleObjectMapping("IMPORTECOMISIONFACTOR")]
        public decimal? ImporteComisionFactor { get; set; }

        [OracleObjectMapping("TIPOCAMBIOWDC")]
        public decimal? TipoCambioWDC { get; set; }

        [OracleObjectMapping("TIPOCAMBIOAFILIA")]
        public decimal? TipoCambioAfilia { get; set; }

        [OracleObjectMapping("CODMONEDAAFILIA")]
        public int? CodMonedaAfilia { get; set; }

        [OracleObjectMapping("COMISIONCOMPARTIDA")]
        public decimal? ComisionCompartida { get; set; }

        [OracleObjectMapping("FECHAVALOR")]
        public DateTime? FechaValor { get; set; }

        [OracleObjectMapping("FECHAMODIFIN")]
        public DateTime? FechaModifin { get; set; }

        [OracleObjectMapping("CLASIFICACIONOPERACION")]
        public string? ClasificacionOperacion { get; set; }

        [OracleObjectMapping("CODIGOSITUACIONANTERIOR")]
        public int? CodigoSituacionAnterior { get; set; }

        [OracleObjectMapping("EXPOSICIONOPERACION")]
        public string? ExposicionOperacion { get; set; }

        [OracleObjectMapping("SALDOANTERIOR")]
        public decimal? SaldoAnterior { get; set; }

        [OracleObjectMapping("SITUACIONOPERACION")]
        public int? SituacionOperacion { get; set; }

        [OracleObjectMapping("REGLASVALIDACION")]
        public string? ReglasValidacion { get; set; }

        [OracleObjectMapping("CODIGOESTADOAUX")]
        public int? CodigoEstadoAux { get; set; }

        [OracleObjectMapping("CODIGOESTADOAUXANTE")]
        public int? CodigoEstadoAuxAnte { get; set; }

        [OracleObjectMapping("FECCAMBIOESTADOAUX")]
        public DateTime? FecCambioEstadoAux { get; set; }

        [OracleObjectMapping("CODIGOTIPOCUENTADESEMBOLSO")]
        public int? CodigoTipoCuentaDesembolso { get; set; }

        [OracleObjectMapping("NUMEROCUENTADESEMBOLSO")]
        public string? NumeroCuentaDesembolso { get; set; }

        [OracleObjectMapping("VALIDARCUENTACLIENTE")]
        public int? ValidarCuentaCliente { get; set; }

        [OracleObjectMapping("NUMEROINTERNO_TMP")]
        public string? NumeroInternoTmp { get; set; }

        [OracleObjectMapping("CODIGOCLIENTEPLANILLA")]
        public int? CodigoClientePlanilla { get; set; }

        [OracleObjectMapping("DIASAMPLIACION")]
        public int? DiasAmpliacion { get; set; }

        [OracleObjectMapping("CODIGOESTADOCONTABLE")]
        public int? CodigoEstadoContable { get; set; }

        [OracleObjectMapping("CODIGOESTADOCONTABLEANTE")]
        public int? CodigoEstadoContableAnte { get; set; }

        [OracleObjectMapping("FECCAMBIOESTADOCONTABLE")]
        public DateTime? FecCambioEstadoContable { get; set; }

        [OracleObjectMapping("DESEMBOLSOAUTOMATICO")]
        public int? DesembolsoAutomatico { get; set; }

        // Implementación de INullable
        private bool _isNull;

        public bool IsNull => _isNull;

        public static DocumentoH2H Null => new DocumentoH2H() { _isNull = true };

        public void FromCustomObject(OracleConnection con, object udt)
        {
            if (IsNull)
                return;

            OracleUdt.SetValue(con, udt, "NUMEROINTERNO", NumeroInterno);
            OracleUdt.SetValue(con, udt, "CODIGOCLIENTE", CodigoCliente);
            OracleUdt.SetValue(con, udt, "ITEM", Item);
            OracleUdt.SetValue(con, udt, "NUMEROPLANILLA", NumeroPlanilla);
            OracleUdt.SetValue(con, udt, "CODIGODEPARTAMENTO", CodigoDepartamento);
            OracleUdt.SetValue(con, udt, "CODIGOPROVINCIA", CodigoProvincia);
            OracleUdt.SetValue(con, udt, "CODIGODISTRITO", CodigoDistrito);
            OracleUdt.SetValue(con, udt, "PLAZAREMESA", PlazaRemesa);
            OracleUdt.SetValue(con, udt, "NUMERODOCUMENTOFISICO", NumeroDocumentoFisico);
            OracleUdt.SetValue(con, udt, "CODIGOMONEDA", CodigoMoneda);
            OracleUdt.SetValue(con, udt, "IMPORTEORIGINAL", ImporteOriginal);
            OracleUdt.SetValue(con, udt, "FECHAVENCIMIENTO", FechaVencimiento);
            OracleUdt.SetValue(con, udt, "FECHAVENCIMIENTO2", FechaVencimiento2);
            OracleUdt.SetValue(con, udt, "DIRECCION", Direccion);
            OracleUdt.SetValue(con, udt, "PROTESTABLE", Protestable);
            OracleUdt.SetValue(con, udt, "PORCENTAJEDESEMBOLSO", PorcentajeDesembolso);
            OracleUdt.SetValue(con, udt, "CODIGOTIPODOCUMENTOCOBRANZA", CodigoTipoDocumentoCobranza);
            OracleUdt.SetValue(con, udt, "CODIGOTIPOABONO", CodigoTipoAbono);
            OracleUdt.SetValue(con, udt, "CODIGOTIPOCUENTA", CodigoTipoCuenta);
            OracleUdt.SetValue(con, udt, "NUMEROCUENTA", NumeroCuenta);
            OracleUdt.SetValue(con, udt, "FECHACARGO", FechaCargo);
            OracleUdt.SetValue(con, udt, "FECHADESEMBOLSO", FechaDesembolso);
            OracleUdt.SetValue(con, udt, "FECHAULTIMAMODIFICACION", FechaUltimaModificacion);
            OracleUdt.SetValue(con, udt, "FECHAULTIMARENOVACION", FechaUltimaRenovacion);
            OracleUdt.SetValue(con, udt, "NUMERORENOVACIONES", NumeroRenovaciones);
            OracleUdt.SetValue(con, udt, "PROTESTO", Protesto);
            OracleUdt.SetValue(con, udt, "CODIGOESTADO", CodigoEstado);
            OracleUdt.SetValue(con, udt, "CODIGOSITUACION", CodigoSituacion);
            OracleUdt.SetValue(con, udt, "CODIGOPOSTAL", CodigoPostal);
            OracleUdt.SetValue(con, udt, "CODIGOUSUARIOREGISTRO", CodigoUsuarioRegistro);
            OracleUdt.SetValue(con, udt, "FECHAREGISTRO", FechaRegistro);
            OracleUdt.SetValue(con, udt, "OBSERVACION", Observacion);
            OracleUdt.SetValue(con, udt, "FLAGCOMPLETADO", FlagCompletado);
            OracleUdt.SetValue(con, udt, "FLAGOBSERVADO", FlagObservado);
            OracleUdt.SetValue(con, udt, "FLAGCUOTA", FlagCuota);
            OracleUdt.SetValue(con, udt, "NUMEROCUOTAS", NumeroCuotas);
            OracleUdt.SetValue(con, udt, "PLAZAPAGO", PlazaPago);
            OracleUdt.SetValue(con, udt, "INTERESDESCUENTO", InteresDescuento);
            OracleUdt.SetValue(con, udt, "SALDOACTUALDOCUMENTO", SaldoActualDocumento);
            OracleUdt.SetValue(con, udt, "FECHAPROTESTO", FechaProtesto);
            OracleUdt.SetValue(con, udt, "DIASPROTESTADOS", DiasProtestados);
            OracleUdt.SetValue(con, udt, "CODIGOESTADOANTERIOR", CodigoEstadoAnterior);
            OracleUdt.SetValue(con, udt, "FECHACAMBIOESTADO", FechaCambioEstado);
            OracleUdt.SetValue(con, udt, "NUMEROLINEA", NumeroLinea);
            OracleUdt.SetValue(con, udt, "SEGMENTOACTUALCLIENTE", SegmentoActualCliente);
            OracleUdt.SetValue(con, udt, "SEGMENTOANTERIORCLIENTE", SegmentoAnteriorCliente);
            OracleUdt.SetValue(con, udt, "FECHADEVOLUCION", FechaDevolucion);
            OracleUdt.SetValue(con, udt, "FECHAACEPTACION", FechaAceptacion);
            OracleUdt.SetValue(con, udt, "FECHACANCELACION", FechaCancelacion);
            OracleUdt.SetValue(con, udt, "NUMERODOCUMENTOACEPTANTE", NumeroDocumentoAceptante);
            OracleUdt.SetValue(con, udt, "RAZONSOCIALACEPTANTE", RazonSocialAceptante);
            OracleUdt.SetValue(con, udt, "FLAGPENDIENTECOMCOB", FlagPendienteComCob);
            OracleUdt.SetValue(con, udt, "TIPODOCUMENTOACEPTANTE", TipoDocumentoAceptante);
            OracleUdt.SetValue(con, udt, "NUMEROASOCIADO", NumeroAsociado);
            OracleUdt.SetValue(con, udt, "FLAGRECHAZADO2DIGITACION", FlagRechazado2Digitacion);
            OracleUdt.SetValue(con, udt, "FECHAEXTORNO", FechaExtorno);
            OracleUdt.SetValue(con, udt, "FECHAREINGRESO", FechaReingreso);
            OracleUdt.SetValue(con, udt, "NUMEROINSTRUCCION", NumeroInstruccion);
            OracleUdt.SetValue(con, udt, "CODIGOTIPOADELANTO", CodigoTipoAdelanto);
            OracleUdt.SetValue(con, udt, "FECHAADELANTO", FechaAdelanto);
            OracleUdt.SetValue(con, udt, "PORCENTAJEPRORROGA", PorcentajeProrroga);
            OracleUdt.SetValue(con, udt, "INTERESPRORROGA", InteresProrroga);
            OracleUdt.SetValue(con, udt, "APLICAPORTES", AplicaPortes);
            OracleUdt.SetValue(con, udt, "DEUDOR", Deudor);
            OracleUdt.SetValue(con, udt, "IMPORTECOMISIONIBK", ImporteComisionIBK);
            OracleUdt.SetValue(con, udt, "IMPORTECOMISIONFACTOR", ImporteComisionFactor);
            OracleUdt.SetValue(con, udt, "TIPOCAMBIOWDC", TipoCambioWDC);
            OracleUdt.SetValue(con, udt, "TIPOCAMBIOAFILIA", TipoCambioAfilia);
            OracleUdt.SetValue(con, udt, "CODMONEDAAFILIA", CodMonedaAfilia);
            OracleUdt.SetValue(con, udt, "COMISIONCOMPARTIDA", ComisionCompartida);
            OracleUdt.SetValue(con, udt, "FECHAVALOR", FechaValor);
            OracleUdt.SetValue(con, udt, "FECHAMODIFIN", FechaModifin);
            OracleUdt.SetValue(con, udt, "CLASIFICACIONOPERACION", ClasificacionOperacion);
            OracleUdt.SetValue(con, udt, "CODIGOSITUACIONANTERIOR", CodigoSituacionAnterior);
            OracleUdt.SetValue(con, udt, "EXPOSICIONOPERACION", ExposicionOperacion);
            OracleUdt.SetValue(con, udt, "SALDOANTERIOR", SaldoAnterior);
            OracleUdt.SetValue(con, udt, "SITUACIONOPERACION", SituacionOperacion);
            OracleUdt.SetValue(con, udt, "REGLASVALIDACION", ReglasValidacion);
            OracleUdt.SetValue(con, udt, "CODIGOESTADOAUX", CodigoEstadoAux);
            OracleUdt.SetValue(con, udt, "CODIGOESTADOAUXANTE", CodigoEstadoAuxAnte);
            OracleUdt.SetValue(con, udt, "FECCAMBIOESTADOAUX", FecCambioEstadoAux);
            OracleUdt.SetValue(con, udt, "CODIGOTIPOCUENTADESEMBOLSO", CodigoTipoCuentaDesembolso);
            OracleUdt.SetValue(con, udt, "NUMEROCUENTADESEMBOLSO", NumeroCuentaDesembolso);
            OracleUdt.SetValue(con, udt, "VALIDARCUENTACLIENTE", ValidarCuentaCliente);
            OracleUdt.SetValue(con, udt, "NUMEROINTERNO_TMP", NumeroInternoTmp);
            OracleUdt.SetValue(con, udt, "CODIGOCLIENTEPLANILLA", CodigoClientePlanilla);
            OracleUdt.SetValue(con, udt, "DIASAMPLIACION", DiasAmpliacion);
            OracleUdt.SetValue(con, udt, "CODIGOESTADOCONTABLE", CodigoEstadoContable);
            OracleUdt.SetValue(con, udt, "CODIGOESTADOCONTABLEANTE", CodigoEstadoContableAnte);
            OracleUdt.SetValue(con, udt, "FECCAMBIOESTADOCONTABLE", FecCambioEstadoContable);
            OracleUdt.SetValue(con, udt, "DESEMBOLSOAUTOMATICO", DesembolsoAutomatico);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            // Si necesitas leer de Oracle hacia .NET, lo haces aquí
            NumeroInterno = (string)OracleUdt.GetValue(con, udt, "NUMEROINTERNO");
            CodigoCliente = (int)OracleUdt.GetValue(con, udt, "CODIGOCLIENTE");
            Item = (int?)OracleUdt.GetValue(con, udt, "ITEM");
            NumeroPlanilla = (string)OracleUdt.GetValue(con, udt, "NUMEROPLANILLA");
            CodigoDepartamento = (int?)OracleUdt.GetValue(con, udt, "CODIGODEPARTAMENTO");
            CodigoProvincia = (int?)OracleUdt.GetValue(con, udt, "CODIGOPROVINCIA");
            CodigoDistrito = (int?)OracleUdt.GetValue(con, udt, "CODIGODISTRITO");
            PlazaRemesa = (string)OracleUdt.GetValue(con, udt, "PLAZAREMESA");
            NumeroDocumentoFisico = (string)OracleUdt.GetValue(con, udt, "NUMERODOCUMENTOFISICO");
            CodigoMoneda = (int?)OracleUdt.GetValue(con, udt, "CODIGOMONEDA");
            ImporteOriginal = (decimal?)OracleUdt.GetValue(con, udt, "IMPORTEORIGINAL");
            FechaVencimiento = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAVENCIMIENTO");
            FechaVencimiento2 = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAVENCIMIENTO2");
            Direccion = (string)OracleUdt.GetValue(con, udt, "DIRECCION");
            Protestable = (int?)OracleUdt.GetValue(con, udt, "PROTESTABLE");
            PorcentajeDesembolso = (decimal?)OracleUdt.GetValue(con, udt, "PORCENTAJEDESEMBOLSO");
            CodigoTipoDocumentoCobranza = (int?)OracleUdt.GetValue(con, udt, "CODIGOTIPODOCUMENTOCOBRANZA");
            CodigoTipoAbono = (int?)OracleUdt.GetValue(con, udt, "CODIGOTIPOABONO");
            CodigoTipoCuenta = (int?)OracleUdt.GetValue(con, udt, "CODIGOTIPOCUENTA");
            NumeroCuenta = (string)OracleUdt.GetValue(con, udt, "NUMEROCUENTA");
            FechaCargo = (DateTime?)OracleUdt.GetValue(con, udt, "FECHACARGO");
            FechaDesembolso = (DateTime?)OracleUdt.GetValue(con, udt, "FECHADESEMBOLSO");
            FechaUltimaModificacion = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAULTIMAMODIFICACION");
            FechaUltimaRenovacion = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAULTIMARENOVACION");
            NumeroRenovaciones = (int?)OracleUdt.GetValue(con, udt, "NUMERORENOVACIONES");
            Protesto = (int?)OracleUdt.GetValue(con, udt, "PROTESTO");
            CodigoEstado = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADO");
            CodigoSituacion = (int?)OracleUdt.GetValue(con, udt, "CODIGOSITUACION");
            CodigoPostal = (string)OracleUdt.GetValue(con, udt, "CODIGOPOSTAL");
            CodigoUsuarioRegistro = (string)OracleUdt.GetValue(con, udt, "CODIGOUSUARIOREGISTRO");
            FechaRegistro = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAREGISTRO");
            Observacion = (string)OracleUdt.GetValue(con, udt, "OBSERVACION");
            FlagCompletado = (int?)OracleUdt.GetValue(con, udt, "FLAGCOMPLETADO");
            FlagObservado = (int?)OracleUdt.GetValue(con, udt, "FLAGOBSERVADO");
            FlagCuota = (int?)OracleUdt.GetValue(con, udt, "FLAGCUOTA");
            NumeroCuotas = (int?)OracleUdt.GetValue(con, udt, "NUMEROCUOTAS");
            PlazaPago = (int?)OracleUdt.GetValue(con, udt, "PLAZAPAGO");
            InteresDescuento = (decimal?)OracleUdt.GetValue(con, udt, "INTERESDESCUENTO");
            SaldoActualDocumento = (decimal?)OracleUdt.GetValue(con, udt, "SALDOACTUALDOCUMENTO");
            FechaProtesto = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAPROTESTO");
            DiasProtestados = (int?)OracleUdt.GetValue(con, udt, "DIASPROTESTADOS");
            CodigoEstadoAnterior = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADOANTERIOR");
            FechaCambioEstado = (DateTime?)OracleUdt.GetValue(con, udt, "FECHACAMBIOESTADO");
            NumeroLinea = (string)OracleUdt.GetValue(con, udt, "NUMEROLINEA");
            SegmentoActualCliente = (string)OracleUdt.GetValue(con, udt, "SEGMENTOACTUALCLIENTE");
            SegmentoAnteriorCliente = (string)OracleUdt.GetValue(con, udt, "SEGMENTOANTERIORCLIENTE");
            FechaDevolucion = (DateTime?)OracleUdt.GetValue(con, udt, "FECHADEVOLUCION");
            FechaAceptacion = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAACEPTACION");
            FechaCancelacion = (DateTime?)OracleUdt.GetValue(con, udt, "FECHACANCELACION");
            NumeroDocumentoAceptante = (string)OracleUdt.GetValue(con, udt, "NUMERODOCUMENTOACEPTANTE");
            RazonSocialAceptante = (string)OracleUdt.GetValue(con, udt, "RAZONSOCIALACEPTANTE");
            FlagPendienteComCob = (int?)OracleUdt.GetValue(con, udt, "FLAGPENDIENTECOMCOB");
            TipoDocumentoAceptante = (int?)OracleUdt.GetValue(con, udt, "TIPODOCUMENTOACEPTANTE");
            NumeroAsociado = (string)OracleUdt.GetValue(con, udt, "NUMEROASOCIADO");
            FlagRechazado2Digitacion = (int?)OracleUdt.GetValue(con, udt, "FLAGRECHAZADO2DIGITACION");
            FechaExtorno = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAEXTORNO");
            FechaReingreso = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAREINGRESO");
            NumeroInstruccion = (string)OracleUdt.GetValue(con, udt, "NUMEROINSTRUCCION");
            CodigoTipoAdelanto = (int?)OracleUdt.GetValue(con, udt, "CODIGOTIPOADELANTO");
            FechaAdelanto = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAADELANTO");
            PorcentajeProrroga = (decimal?)OracleUdt.GetValue(con, udt, "PORCENTAJEPRORROGA");
            InteresProrroga = (decimal?)OracleUdt.GetValue(con, udt, "INTERESPRORROGA");
            AplicaPortes = (int?)OracleUdt.GetValue(con, udt, "APLICAPORTES");
            Deudor = (string)OracleUdt.GetValue(con, udt, "DEUDOR");
            ImporteComisionIBK = (decimal?)OracleUdt.GetValue(con, udt, "IMPORTECOMISIONIBK");
            ImporteComisionFactor = (decimal?)OracleUdt.GetValue(con, udt, "IMPORTECOMISIONFACTOR");
            TipoCambioWDC = (decimal?)OracleUdt.GetValue(con, udt, "TIPOCAMBIOWDC");
            TipoCambioAfilia = (decimal?)OracleUdt.GetValue(con, udt, "TIPOCAMBIOAFILIA");
            CodMonedaAfilia = (int?)OracleUdt.GetValue(con, udt, "CODMONEDAAFILIA");
            ComisionCompartida = (decimal?)OracleUdt.GetValue(con, udt, "COMISIONCOMPARTIDA");
            FechaValor = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAVALOR");
            FechaModifin = (DateTime?)OracleUdt.GetValue(con, udt, "FECHAMODIFIN");
            ClasificacionOperacion = (string)OracleUdt.GetValue(con, udt, "CLASIFICACIONOPERACION");
            CodigoSituacionAnterior = (int?)OracleUdt.GetValue(con, udt, "CODIGOSITUACIONANTERIOR");
            ExposicionOperacion = (string)OracleUdt.GetValue(con, udt, "EXPOSICIONOPERACION");
            SaldoAnterior = (decimal?)OracleUdt.GetValue(con, udt, "SALDOANTERIOR");
            SituacionOperacion = (int?)OracleUdt.GetValue(con, udt, "SITUACIONOPERACION");
            ReglasValidacion = (string)OracleUdt.GetValue(con, udt, "REGLASVALIDACION");
            CodigoEstadoAux = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADOAUX");
            CodigoEstadoAuxAnte = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADOAUXANTE");
            FecCambioEstadoAux = (DateTime?)OracleUdt.GetValue(con, udt, "FECCAMBIOESTADOAUX");
            CodigoTipoCuentaDesembolso = (int?)OracleUdt.GetValue(con, udt, "CODIGOTIPOCUENTADESEMBOLSO");
            NumeroCuentaDesembolso = (string)OracleUdt.GetValue(con, udt, "NUMEROCUENTADESEMBOLSO");
            ValidarCuentaCliente = (int?)OracleUdt.GetValue(con, udt, "VALIDARCUENTACLIENTE");
            NumeroInternoTmp = (string)OracleUdt.GetValue(con, udt, "NUMEROINTERNO_TMP");
            CodigoClientePlanilla = (int?)OracleUdt.GetValue(con, udt, "CODIGOCLIENTEPLANILLA");
            DiasAmpliacion = (int?)OracleUdt.GetValue(con, udt, "DIASAMPLIACION");
            CodigoEstadoContable = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADOCONTABLE");
            CodigoEstadoContableAnte = (int?)OracleUdt.GetValue(con, udt, "CODIGOESTADOCONTABLEANTE");
            FecCambioEstadoContable = (DateTime?)OracleUdt.GetValue(con, udt, "FECCAMBIOESTADOCONTABLE");
            DesembolsoAutomatico = (int?)OracleUdt.GetValue(con, udt, "DESEMBOLSOAUTOMATICO");
        }
        public IOracleCustomType CreateObject() => new DocumentoH2H();
    }

    [OracleCustomTypeMapping("CAB_DOCUMENTOS_H2H")]
    public class CabDocumentosH2H : IOracleCustomType, IOracleCustomTypeFactory, IEnumerable<DocumentoH2H>, INullable
    {
        private List<DocumentoH2H> _list = new List<DocumentoH2H>();
        private bool _isNull;

        public bool IsNull => _isNull;
        public static CabDocumentosH2H Null => new CabDocumentosH2H() { _isNull = true };

        public CabDocumentosH2H(IEnumerable<DocumentoH2H> documentos)
        {
            if (documentos != null)
                _list.AddRange(documentos);
        }

        public CabDocumentosH2H() { }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, _list.ToArray());
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            DocumentoH2H[] array = (DocumentoH2H[])OracleUdt.GetValue(con, udt, 0);
            _list = new List<DocumentoH2H>(array);
        }

        public IOracleCustomType CreateObject() => new CabDocumentosH2H();

        public void Add(DocumentoH2H item) => _list.Add(item);
        public IEnumerator<DocumentoH2H> GetEnumerator() => _list.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _list.GetEnumerator();
        public DocumentoH2H[] ToArray() => _list.ToArray();
    }
}
