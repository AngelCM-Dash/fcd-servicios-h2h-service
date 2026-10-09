using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using NextSIT.Utility;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{

    public class PlanillasRepository : IPlanillasRepository
    {
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ILogger<PlanillasRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        public PlanillasRepository(
            ITypeConvertionManager typeConvertionManager,
            ILogger<PlanillasRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor)
        {
            ArgumentNullException.ThrowIfNull(typeConvertionManager);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);

            _typeConvertionManager = typeConvertionManager;
            _logger = logger;
            _oracleConnectionFactory = oracleConnectionFactory;
            _dapperExecutor = dapperExecutor;
        }

        public async Task<DapperPlanillaCompleta> RegistrarPlanillaFCD_H2H(DapperPlanillaCompleta planilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("pii_CODIGOPRODUCTO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoProducto);
                parameters.Add("pii_CODIGOMONEDA", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoMoneda);
                parameters.Add("pii_TOTALDOCUMENTOSPLANILLA", OracleDbType.Int64, ParameterDirection.Input, planilla.TotalDocumentosPlanilla);
                parameters.Add("pin_IMPORTETOTALPLANILLA", OracleDbType.Decimal, ParameterDirection.Input, planilla.ImporteTotalPlanilla);
                parameters.Add("piv_USUARIOPRESENTACION", OracleDbType.Varchar2, ParameterDirection.Input, planilla.Usuario);
                parameters.Add("pii_CODIGOTIPOCUENTAABONO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigotipoCuentaAbono);
                parameters.Add("piv_NUMEROCUENTAABONO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroCuentaAbono);
                parameters.Add("pii_CODIGOESTADO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoEstado);
                parameters.Add("pii_CODIGOTIENDA", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoTienda);
                parameters.Add("piv_TIENDARECEPTORA", OracleDbType.Varchar2, ParameterDirection.Input, planilla.Tienda);
                parameters.Add("pii_CODIGOUNICO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoUnico);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, "");
                parameters.Add("piv_CANALATENCION", OracleDbType.Varchar2, ParameterDirection.Input, planilla.CanalAtencion);
                parameters.Add("piv_NOMBREUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.Usuario);
                parameters.Add("pii_CODIGOPERFILUSUARIO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.CodigoPerfilUsuario);
                parameters.Add("pii_FLAGCONTROLFLUJO", OracleDbType.Int64, ParameterDirection.Input, planilla.FlagControlFlujo);
                parameters.Add("pii_CODIGOFORMAOPERACION", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoFormaOperacion);
                parameters.Add("pii_CONTRATOMARCO", OracleDbType.Int64, ParameterDirection.Input, planilla.ContratoMarco == true ? 1 : 0);
                parameters.Add("pii_CODIGOTIPODOCCOBRANZA", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoTipoCobranza);
                parameters.Add("pii_CODIGOTIPOCUENTACARGO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigotipoCuentaCargo);
                parameters.Add("piv_NUMEROCUENTACARGO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroCuentaCargo);
                parameters.Add("pii_APLICAINTERESMORATORIO", OracleDbType.Int64, ParameterDirection.Input, planilla.AplicaInteresMoratorio);
                parameters.Add("pii_TOTALDOCUMENTOSREGISTRADOS", OracleDbType.Int64, ParameterDirection.Input, planilla.TotalDocumentosPlanilla);
                parameters.Add("pin_IMPORTETOTALREGISTRADO", OracleDbType.Decimal, ParameterDirection.Input, planilla.ImporteTotalRegistrado);
                parameters.Add("piv_USUARIODESEMBOLSO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.Usuario);
                parameters.Add("pii_APLICAINTERESCOMPENSATORIO", OracleDbType.Int64, ParameterDirection.Input, planilla.AplicaInteresCompensatorio);
                parameters.Add("pii_CODIGOMODALIDADPRODUCTO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoModalidadProducto);
                parameters.Add("pii_CODIGOTIPOADELANTO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoTipoAdelanto);
                parameters.Add("pii_CODIGOMODALIDADADELANTO", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoModalidadAdelanto);
                parameters.Add("pid_FECHAADELANTO", OracleDbType.Date, ParameterDirection.Input, planilla.FechaAdelanto);
                parameters.Add("pii_CODIGOTIPOCUENTACOMISIONES", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigotipoCuentaAbono);
                parameters.Add("piv_NUMEROCUENTACOMISIONES", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroCuentaAbono);
                parameters.Add("piv_CUFACTOR", OracleDbType.Varchar2, ParameterDirection.Input, planilla.CuFactor);
                parameters.Add("piv_CUBANCOEMISOR", OracleDbType.Varchar2, ParameterDirection.Input, planilla.CuBancoEmisor);
                parameters.Add("piv_PLAZACUENTA", OracleDbType.Varchar2, ParameterDirection.Input, planilla.PlazaCuenta);
                parameters.Add("pid_FECHADESEMBOLSO", OracleDbType.Date, ParameterDirection.Input, planilla.FechaDesembolso);
                parameters.Add("pii_ASUMEINTERES", OracleDbType.Int64, ParameterDirection.Input, planilla.AsumeInteres);
                parameters.Add("pii_INFOTOTALDOCPLANILLA", OracleDbType.Decimal, ParameterDirection.Input, planilla.InfoTotalDocsPlanilla);
                parameters.Add("pin_INFOIMPORTETOTALPLANILLA", OracleDbType.Decimal, ParameterDirection.Input, planilla.ImporteTotalPlanilla);
                parameters.Add("pin_COMISIONIBK", OracleDbType.Decimal, ParameterDirection.Input, planilla.ComisionIBK);
                parameters.Add("pin_COMISIONFACTOR", OracleDbType.Decimal, ParameterDirection.Input, planilla.ComisionFactor);
                parameters.Add("pin_IMPORTEFLAT", OracleDbType.Decimal, ParameterDirection.Input, planilla.ImporteFlat);
                parameters.Add("piv_NUMEROOPERACION", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroOperacion);
                parameters.Add("pii_CODMONEDAWDC", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoMonedaWDC);
                parameters.Add("pii_CODMONEDACTA", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoMonedaCta);
                parameters.Add("pin_IMPORTEWDC", OracleDbType.Decimal, ParameterDirection.Input, planilla.ImporteWDC);
                parameters.Add("pin_IMPORTECTA", OracleDbType.Int64, ParameterDirection.Input, planilla.ImporteCta);
                parameters.Add("pin_TASADESCUENTO", OracleDbType.Decimal, ParameterDirection.Input, planilla.TasaDescuento);
                parameters.Add("pii_ITEMDEUDOR", OracleDbType.Int64, ParameterDirection.Input, planilla.ItemDeudor);
                parameters.Add("pin_TIPOCAMBIOWDC", OracleDbType.Decimal, ParameterDirection.Input, planilla.TipoCambioWDC);
                parameters.Add("pv_NROLINEAFACTOR", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroLineaFactor);
                parameters.Add("pv_NROLINEACLIENTE", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroLineaCliente);
                parameters.Add("pv_COSTOFONDO", OracleDbType.Decimal, ParameterDirection.Input, planilla.CostoFondo);
                parameters.Add("PV_ARCHIVO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NombreArchivo);
                parameters.Add(OracleParameterNames.PovNumeroPlanilla, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.createPlanillaCabecera, parameters, commandType: CommandType.StoredProcedure);

                var fila = result.FirstOrDefault();

                if (fila != null)
                {
                    planilla.NumeroPlanilla = _typeConvertionManager.AnyToString(fila.NUMEROPLANILLA);
                    planilla.CodigoCliente = _typeConvertionManager.AnyToInteger(fila.CODIGOCLIENTE);
                }
                return planilla;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarPlanillaFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ObtenerCodigoFormacionFCD_H2H(DapperPlanillaCompleta planilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PII_CODIGOPRODUCTO", OracleDbType.Int32, ParameterDirection.Input, planilla.CodigoProducto);
                parameters.Add(OracleParameterNames.PovCodigoFormaOperacion, OracleDbType.RefCursor, ParameterDirection.Output);

                var dr = await _dapperExecutor.QueryAsync(connection, OracleProcedures.GetCodigoFormaOperacion, parameters, commandType: CommandType.StoredProcedure);
                int CodigoFormaOperacion = _typeConvertionManager.AnyToInteger(dr.Select(x => x.CODIGOFORMAOPERACION).FirstOrDefault());
                return CodigoFormaOperacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerCodigoFormacionFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RegistrarDocCuota_DocEstadoFCD_H2H(DapperPlanillaCompleta planilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroPlanilla);
                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.DocCuotaDocEstado, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarDocCuota_DocEstadoFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DapperPlanillaCompleta> VerificarDuplicidadPlanillaFCD_H2H(DapperPlanillaCompleta planilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PV_ARCHIVO", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NombreArchivo);
                parameters.Add(OracleParameterNames.PovCodigoArchivo, OracleDbType.RefCursor, ParameterDirection.Output);

                var dr = await _dapperExecutor.QueryAsync(connection, OracleProcedures.GetArchivoPlanilla, parameters, commandType: CommandType.StoredProcedure);
                planilla.CodigoArchivo = _typeConvertionManager.AnyToString(dr.Select(x => x.CODIGOARCHIVO).FirstOrDefault());
                return planilla;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "VerificarDuplicidadPlanillaFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperDetallePlanillaMonitor>> ObtenerDetallePlanillaMonitor(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDetallesPlanillaMonitor, parameters, commandType: CommandType.StoredProcedure);
                var listaDetalle = result.Select(x => new DapperDetallePlanillaMonitor
                {
                    NumeroInterno = _typeConvertionManager.AnyToString(x.NUMEROINTERNO),
                    NumeroDocumentoAceptante = _typeConvertionManager.AnyToString(x.NUMERODOCUMENTOACEPTANTE),
                    TipoOperacion = _typeConvertionManager.AnyToString(x.TIPOOPERACION),
                    NumeroDocumentoFisico = _typeConvertionManager.AnyToString(x.NUMERODOCUMENTOFISICO),
                    Estado = _typeConvertionManager.AnyToString(x.ESTADO),
                    Observacion = _typeConvertionManager.AnyToString(x.OBSERVACION)
                }).ToList();
                return listaDetalle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerDetallePlanillaMonitor - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperReservasPlanilla>> ObtenerReservasPlanillas(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerReservasxPlanilla, parameters, commandType: CommandType.StoredProcedure);
                var listaDetalle = result.Select(x => new DapperReservasPlanilla
                {
                    CodigoReserva = _typeConvertionManager.AnyToInteger(x.CODIGORESERVA),
                }).ToList();
                return listaDetalle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerReservasPlanillas - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> EliminarReservaPlanilla(string numeroPlanilla, int codigoReserva)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("PIV_CODIGORESERVA", OracleDbType.Int64, ParameterDirection.Input, codigoReserva);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int32, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.EliminarReservasxPlanilla, parameters, commandType: CommandType.StoredProcedure);

                var resultado = parameters.Get<int>(OracleParameterNames.PoRespuesta);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EliminarReservaPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> GeneraCodigoSecuenciaDocumentosDuplicados()
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int32, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ObtenerCodigoSecuenciaDocumentosDuplicados, parameters, commandType: CommandType.StoredProcedure);

                var codigoSecuencia = parameters.Get<int>(OracleParameterNames.PoRespuesta);

                return codigoSecuencia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeneraCodigoSecuenciaDocumentosDuplicados - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperDocumentosDuplicados>> ObtenerDocumentosDuplicados(string codigoUnico, int codigoSecuencia)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.pivCodigoUnico, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnico);
                parameters.Add(OracleParameterNames.pivCodigoSecuencia, OracleDbType.Int64, ParameterDirection.Input, codigoSecuencia);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDocumentoDuplicados, parameters, commandType: CommandType.StoredProcedure);
                var listaDocDuplicados = result.Select(x => new DapperDocumentosDuplicados
                {
                    NumeroDocumentoFisico = _typeConvertionManager.AnyToString(x.NUMERODOCUMENTOFISICO),
                    TipoDocumentoCobranza = _typeConvertionManager.AnyToString(x.TIPODOCUMENTOCOBRANZA),
                    NumeroDocumentoIdentidad = _typeConvertionManager.AnyToString(x.NUMERODOCUMENTOIDENTIDAD)
                }).ToList();
                return listaDocDuplicados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerDocumentosDuplicados - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ValidarFacturaCargo(string codigoUnico)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.pivCodigoUnico, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnico);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int32, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ValidarFacturaPendienteCargo, parameters, commandType: CommandType.StoredProcedure);
                var result = parameters.Get<int>(OracleParameterNames.PoRespuesta);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ValidarFacturaCargo - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ValidarPermiteDuplicados(string codigoUnico, int? codProducto)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.pivCodigoUnico, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnico);
                parameters.Add(OracleParameterNames.pivCodigoProducto, OracleDbType.Int64, ParameterDirection.Input, codProducto);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int32, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ValidarPermiteDuplicado, parameters, commandType: CommandType.StoredProcedure);
                var result = parameters.Get<int>(OracleParameterNames.PoRespuesta);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ValidarPermiteDuplicados - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DapperPlanillaCompleta> ObtenerInformacionPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDatosPlanilla, parameters, commandType: CommandType.StoredProcedure);

                var item = result.FirstOrDefault();

                if (item == null)
                {
                    throw new InvalidOperationException("No se encontró información para la planilla.");
                }

                return new DapperPlanillaCompleta
                {
                    NumeroPlanilla = _typeConvertionManager.AnyToString(item.NUMEROPLANILLA),
                    FechaDesembolso = _typeConvertionManager.AnyToDateTime(item.FECHADESEMBOLSO, true),
                    CodigoUnico = _typeConvertionManager.AnyToString(item.CODIGOUNICO),
                    CodigoCliente = _typeConvertionManager.AnyToInteger(item.CODIGOCLIENTE),
                    CodigoProducto = _typeConvertionManager.AnyToInteger(item.CODIGOPRODUCTO),
                    CodigoMoneda = _typeConvertionManager.AnyToInteger(item.CODIGOMONEDA),
                    TotalDocumentosPlanilla = _typeConvertionManager.AnyToInteger(item.TOTALDOCUMENTOSPLANILLA),
                    ImporteTotalPlanilla = _typeConvertionManager.AnyToDecimal(item.IMPORTETOTALPLANILLA),
                    CanalAtencion = _typeConvertionManager.AnyToString(item.CANALATENCION),
                    Observacion = _typeConvertionManager.AnyToString(item.OBSERVACION),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerInformacionPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperSecuenciaPlanilla>> ObtenerSecuenciaPlanilla(int cantidadSecuencias)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CANTIDADSECUENCIAS", OracleDbType.Int64, ParameterDirection.Input, cantidadSecuencias);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerSecuenciasPlanilla, parameters, commandType: CommandType.StoredProcedure);

                var listadoSecuencias = result.Select(x => new DapperSecuenciaPlanilla
                {
                    NumeroSecuencia = _typeConvertionManager.AnyToString(x.NUMEROINTERNO)
                }).ToList();
                return listadoSecuencias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerSecuenciaPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> RegistraDocumentosPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int64, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaDocumentosxPlanilla, parameters, commandType: CommandType.StoredProcedure);

                int resultado = parameters.Get<int>(OracleParameterNames.PoRespuesta);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistraDocumentosPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperPlanillaCompleta>> ObtenerInformacionDocumentoPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDatosDocumento, parameters, commandType: CommandType.StoredProcedure);

                var listadoInfoDocumentos = result.Select(x => new DapperPlanillaCompleta
                {
                    CodigoProducto = _typeConvertionManager.AnyToInteger(x.CODIGOPRODUCTO),
                    CodigoUnico = _typeConvertionManager.AnyToString(x.CODIGOUNICO),
                    NumeroLineaCliente = _typeConvertionManager.AnyToString(x.NUMEROLINEA),
                    ImporteTotalRegistrado = _typeConvertionManager.AnyToDecimal(x.IMPORTETOTAL),
                    CodigoMoneda = _typeConvertionManager.AnyToInteger(x.CODIGOMONEDA),
                }).ToList();
                return listadoInfoDocumentos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerInformacionDocumentoPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RechazoPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.RejectPlanilla, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RechazoPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperParametroCtl>> ObtenerConfiguracionCTL(DapperPlanillaCompleta planilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CANAL", OracleDbType.Varchar2, ParameterDirection.Input, planilla.CanalAtencion);
                parameters.Add("PII_CODIGOCLIENTE", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoCliente);
                parameters.Add("PII_CODIGOMONEDA", OracleDbType.Int64, ParameterDirection.Input, planilla.CodigoMoneda);
                parameters.Add("PII_NUMEROPLANILLA", OracleDbType.Varchar2, ParameterDirection.Input, planilla.NumeroPlanilla);
                parameters.Add("POV_COLUMNASCTL", OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.GetItemCtlH2H, parameters, commandType: CommandType.StoredProcedure);

                var listaConfiguracionCtl = result.Select(x => new DapperParametroCtl
                {
                    CORRELATIVO = _typeConvertionManager.AnyToInteger(x.CORRELATIVO),
                    ORDEN = _typeConvertionManager.AnyToInteger(x.ORDEN),
                    COLUMNA = _typeConvertionManager.AnyToString(x.COLUMNA),
                    POSICIONINICIAL = _typeConvertionManager.AnyToInteger(x.POSICIONINICIAL),
                    POSICIONFINAL = _typeConvertionManager.AnyToInteger(x.POSICIONFINAL),
                    ADICIONAL = _typeConvertionManager.AnyToString(x.ADICIONAL)
                }).ToList();

                return listaConfiguracionCtl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerConfiguracionCTL - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task ActualizarObservacionPlanilla(string numeroPlanilla, string observacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_Observacion", OracleDbType.Varchar2, ParameterDirection.Input, observacion);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ActualizaObservacionPlanilla, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarObservacionPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
