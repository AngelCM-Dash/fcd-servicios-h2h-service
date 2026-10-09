using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using NextSIT.Utility;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{

    public class DesembolsoRepository : IDesembolsoRepository
    {
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ILogger<DesembolsoRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        public DesembolsoRepository(
            ITypeConvertionManager typeConvertionManager,
            ILogger<DesembolsoRepository> logger,
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

        public async Task ActualizarPlanillaDistribuido(DapperActualizarPlanilla actualizarPlanilla)
        {

            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.NumeroPlanilla);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CodigoAgrupamiento);
                parameters.Add("pii_CODIGOPERFILUSUARIO", OracleDbType.Int32, ParameterDirection.Input, actualizarPlanilla.CodigoPerfilUsuario);
                parameters.Add("piv_CODIGOUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CodigoUsuario);
                parameters.Add("piv_NOMBREUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.NombreUsuarioRegistro);
                parameters.Add("piv_COMENTARIO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.Comentario);
                parameters.Add("piv_CANALATENCION", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CanalAtencion);
                parameters.Add("PIV_CODIGOTIENDA", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CodigoTienda);
                parameters.Add(OracleParameterNames.PivRetorno, OracleDbType.Int32, ParameterDirection.Output);
                await _dapperExecutor.QueryAsync(connection, OracleProcedures.ActualizarPlanilla, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarPlanillaDistribuido - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }

        }

        public async Task ActualizarPlanillaDistribuidoFCD(DapperActualizarPlanilla actualizarPlanilla)
        {

            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.NumeroPlanilla);
                parameters.Add("pii_CODIGOPERFILUSUARIO", OracleDbType.Int32, ParameterDirection.Input, actualizarPlanilla.CodigoPerfilUsuario);
                parameters.Add("piv_CODIGOUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CodigoUsuario);
                parameters.Add("piv_NOMBREUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.NombreUsuarioRegistro);
                parameters.Add("piv_COMENTARIO", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.Comentario);
                parameters.Add("piv_CANALATENCION", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CanalAtencion);
                parameters.Add("piv_CODIGOTIENDA", OracleDbType.Varchar2, ParameterDirection.Input, actualizarPlanilla.CodigoTienda);
                await _dapperExecutor.QueryAsync(connection, OracleProcedures.ActualizarPlanillaFCD, parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarPlanillaDistribuidoFCD - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }

        }

        public async Task<string> fintNextPlanillaSecuencia(string NumeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, NumeroPlanilla);
                parameters.Add("poi_SECUENCIA", OracleDbType.RefCursor, ParameterDirection.Output);
                var dr = (await _dapperExecutor.QueryAsync(connection, OracleProcedures.SeleccionarNumeroSecPlanilla, parameters, commandType: CommandType.StoredProcedure)).ToList();
                int CodigoRetorno = Convert.ToInt32(dr.Select(x => x.SECUENCIA).FirstOrDefault());
                return CodigoRetorno.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fintNextPlanillaSecuencia - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DataTable> ObtenerTramas(DapperGenerarTramasInput generarTramasInput)
        {

            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                DataTable dt = new DataTable();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.NumeroPlanilla);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.NumeroInstruccion);
                parameters.Add("pii_NumeroPlanillaSecuencia", OracleDbType.Int32, ParameterDirection.Input, generarTramasInput.NumeroPlanillaSecuencia);
                parameters.Add("piv_UsuarioLogin", OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.UsuarioEjecuta);
                parameters.Add("pii_Desembolsar", OracleDbType.Int32, ParameterDirection.Input, generarTramasInput.FlagDesembolsar);
                parameters.Add(OracleParameterNames.PocTrama, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.ExecuteReaderAsync(connection, OracleProcedures.GenerarTramas, parameters, commandType: CommandType.StoredProcedure);
                dt.Load(result);
                return dt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerTramas - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }

        }

        public async Task<List<DapperParametro>> ObtenerFlujoDesembolso(string codigoReferencia, string descripcionCorta)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CODIGOREFERENCIA", OracleDbType.Varchar2, ParameterDirection.Input, codigoReferencia);
                parameters.Add("PIV_DESCRIPCIONCORTA", OracleDbType.Varchar2, ParameterDirection.Input, descripcionCorta);
                parameters.Add(OracleParameterNames.PocurConfig, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerFLujoDesembolso, parameters, commandType: CommandType.StoredProcedure);
                var ListadoParametro = result.Select(x => new DapperParametro
                {
                    CODIGOPARAMETRO = _typeConvertionManager.AnyToInteger(x.CODIGOPARAMETRO),
                    CODIGODOMINIO = _typeConvertionManager.AnyToInteger(x.CODIGODOMINIO),
                    DESCRIPCION = _typeConvertionManager.AnyToString(x.DESCRIPCION),
                    ESTADO = _typeConvertionManager.AnyToInteger(x.ESTADO),
                    CODIGOREFERENCIA = _typeConvertionManager.AnyToString(x.CODIGOREFERENCIA),
                    NUMEROORDEN = _typeConvertionManager.AnyToInteger(x.NUMEROORDEN),
                    DESCRIPCIONCORTA = _typeConvertionManager.AnyToString(x.DESCRIPCIONCORTA),
                    CODIGOREFERENCIAPARAMETRO = _typeConvertionManager.AnyToInteger(x.CODIGOREFERENCIAPARAMETRO),
                    OPCION1 = _typeConvertionManager.AnyToString(x.OPCION1),
                }).ToList();

                return ListadoParametro;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerFlujoDesembolso - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperConsumoLinea>> ObtenerDatosDesembolso(string numeroPlanilla, int numeroPlanillaSEQ, string tipoQuery)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroPlanillaSEQ", OracleDbType.Int64, ParameterDirection.Input, numeroPlanillaSEQ);
                parameters.Add("piv_TipoQuery", OracleDbType.Varchar2, ParameterDirection.Input, tipoQuery);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerInformacionConsumoLineas, parameters, commandType: CommandType.StoredProcedure);

                var listadoMovimientos = result.Select(x => new DapperConsumoLinea
                {
                    NumeroInstruccion = _typeConvertionManager.AnyToString(x.NUMEROINSTRUCCION),
                    NumeroLinea = _typeConvertionManager.AnyToString(x.NUMEROLINEA),
                    Neteo = _typeConvertionManager.AnyToDecimal(x.NETEO),
                    Tipo = _typeConvertionManager.AnyToString(x.TIPO),
                    CodigoMonedaWBC = _typeConvertionManager.AnyToInteger(x.CODIGOMONEDAWBC),
                    NumeroOperacion = _typeConvertionManager.AnyToString(x.NUMEROOPERACION)
                }).ToList();
                return listadoMovimientos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerDatosDesembolso - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DapperRespuestaProcesoTramas> RegistrarTramasProcesadadas(string numeroPlanilla, int planillaSecuencia, string usuario)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroSecuenciaPlanilla", OracleDbType.Int64, ParameterDirection.Input, planillaSecuencia);
                parameters.Add("piv_Usuario", OracleDbType.Varchar2, ParameterDirection.Input, usuario);
                parameters.Add(OracleParameterNames.PoTramaProcesada, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.InsertarTramasProcesadas, parameters, commandType: CommandType.StoredProcedure);

                var respuesta = result.Select(x => new DapperRespuestaProcesoTramas
                {
                    RespuestaCodigo = _typeConvertionManager.AnyToInteger(x.RESPUESTACODIGO),
                    RespuestaMensaje = _typeConvertionManager.AnyToString(x.RESPUESTAMENSAJE)
                }).FirstOrDefault();

                if (respuesta == null)
                {
                    throw new InvalidOperationException("La respuesta del servidor está vacía.");
                }

                return respuesta;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarTramasProcesadadas - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DapperRespuestaProcesoTramas> RegistrarMovimientos(string numeroPlanilla, int planillaSecuencia, string usuario, string numeroInstruccion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("pii_NumeroPlanillaSEQ", OracleDbType.Int64, ParameterDirection.Input, planillaSecuencia);
                parameters.Add("piv_UsuarioRegistro", OracleDbType.Varchar2, ParameterDirection.Input, usuario);
                parameters.Add(OracleParameterNames.PoTramaProcesada, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.InsertarMovimientosDesembolso, parameters, commandType: CommandType.StoredProcedure);

                var respuesta = result.Select(x => new DapperRespuestaProcesoTramas
                {
                    RespuestaCodigo = _typeConvertionManager.AnyToInteger(x.RESPUESTACODIGO),
                    RespuestaMensaje = _typeConvertionManager.AnyToString(x.RESPUESTAMENSAJE)
                }).FirstOrDefault();

                if (respuesta == null)
                {
                    throw new InvalidOperationException("La respuesta del servidor está vacía.");
                }

                return respuesta;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarMovimientos - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperPlanillasDiferidas>> ObtenerPlanillasDiferidas(string tipoQuery)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_Query", OracleDbType.Varchar2, ParameterDirection.Input, tipoQuery);
                parameters.Add(OracleParameterNames.PocDatos, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerListadoPlanillasDiferidas, parameters, commandType: CommandType.StoredProcedure);
                var listadoPlanillasDiferidas = result.Select(x => new DapperPlanillasDiferidas
                {
                    NumeroPlanilla = _typeConvertionManager.AnyToString(x.NUMEROPLANILLA),
                    NumeroInstruccion = _typeConvertionManager.AnyToString(x.NUMEROINSTRUCCION),
                    NumeroLinea = _typeConvertionManager.AnyToInteger(x.NUMEROLINEA),
                    ImportePlanilla = _typeConvertionManager.AnyToDecimal(x.IMPORTEPLANILLA),
                }).ToList();
                return listadoPlanillasDiferidas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerPlanillasDiferidas - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RechazarDocumentosDiferidos(string numeroPlanilla, string numeroInstruccion, string numeroLinea, string lineaObservacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, numeroInstruccion);
                parameters.Add("piv_NumeroLinea", OracleDbType.Varchar2, ParameterDirection.Input, numeroLinea);
                parameters.Add("piv_LineaObservacion", OracleDbType.Varchar2, ParameterDirection.Input, lineaObservacion);
                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.RechazaDocumentoDiferidos, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RechazarDocumentosDiferidos - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DataTable> GenerarTramaDiferidos(string numeroPlanilla, string numeroInstruccion, string numeroLinea)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                DataTable dt = new DataTable();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, numeroInstruccion);
                parameters.Add("piv_NumeroLinea", OracleDbType.Int32, ParameterDirection.Input, numeroLinea);
                parameters.Add(OracleParameterNames.PocRespuesta, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.ExecuteReaderAsync(connection, OracleProcedures.GeneraTramaDiferidos, parameters, commandType: CommandType.StoredProcedure);
                dt.Load(result);
                return dt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenerarTramaDiferidos - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DataTable> ObtieneTramaDiferidos(string numeroPlanilla, int numeroPlanillaSecuencia)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                DataTable dt = new DataTable();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("pii_NumeroPlanillaSEQ", OracleDbType.Int32, ParameterDirection.Input, numeroPlanillaSecuencia);
                parameters.Add(OracleParameterNames.PocTramas, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.ExecuteReaderAsync(connection, OracleProcedures.ObtenerTramaDiferidos, parameters, commandType: CommandType.StoredProcedure);
                dt.Load(result);
                return dt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtieneTramaDiferidos - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> RegistrarPlanillaDietario(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Decimal, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaPlanillaDietario, parameters, commandType: CommandType.StoredProcedure);

                var codigoRespuesta = parameters.Get<int>(OracleParameterNames.PoRespuesta);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarPlanillaDietario - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RegistrarMensajeErrorMonitor(string numeroPlanilla, int numeroPlanillaSecuencia, string observacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("PIV_NUMEROSECUENCIA", OracleDbType.Int64, ParameterDirection.Input, numeroPlanillaSecuencia);
                parameters.Add("PIV_OBSERVACION", OracleDbType.Varchar2, ParameterDirection.Input, observacion);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaMensajeErrorMonitor, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarMensajeErrorMonitor - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ObtenerMensajeErrorMonitorPorPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Decimal, ParameterDirection.Output, 0);

                await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerMensajeErrorMonitorxPlanilla, parameters, commandType: CommandType.StoredProcedure);

                var codigoRespuesta = parameters.Get<int>(OracleParameterNames.PoRespuesta);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerMensajeErrorMonitorPorPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RegistrarDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor, int numeroReintento, int estadoReintento, int tipoReintento, int registroProcesoAbono)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroSecuencia", OracleDbType.Int64, ParameterDirection.Input, numeroSecuencia);
                parameters.Add(OracleParameterNames.pivCodigoUnicoProveedor, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnicoProveedor);
                parameters.Add("piv_NumeroRegistrosProcesados", OracleDbType.Int64, ParameterDirection.Input, registroProcesoAbono);
                parameters.Add("piv_NumeroReintento", OracleDbType.Int64, ParameterDirection.Input, numeroReintento);
                parameters.Add("piv_EstadoReintento", OracleDbType.Int64, ParameterDirection.Input, estadoReintento);
                parameters.Add("piv_TipoReintento", OracleDbType.Int64, ParameterDirection.Input, tipoReintento);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaDesembolsoAbonoFallido, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarDesembolsoAbono - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task ActualizarEstadoDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor, int estadoReintento, int registroProcesoAbono)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroSecuencia", OracleDbType.Int64, ParameterDirection.Input, numeroSecuencia);
                parameters.Add(OracleParameterNames.pivCodigoUnicoProveedor, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnicoProveedor);
                parameters.Add("piv_NumeroRegistrosProcesados", OracleDbType.Int64, ParameterDirection.Input, registroProcesoAbono);
                parameters.Add("piv_EstadoReintento", OracleDbType.Int64, ParameterDirection.Input, estadoReintento);
                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ActualizarDesembolsoAbonoFallido, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarEstadoDesembolsoAbono - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperTmpDesembolsoAbono>> ObtenerDesembolsoAbono(string numeroPlanilla, int numeroSecuencia, string codigoUnicoProveedor)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroSecuencia", OracleDbType.Int64, ParameterDirection.Input, numeroSecuencia);
                parameters.Add(OracleParameterNames.pivCodigoUnicoProveedor, OracleDbType.Varchar2, ParameterDirection.Input, codigoUnicoProveedor);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerEstadoDesembolsoAbonoFallido, parameters, commandType: CommandType.StoredProcedure);

                var listadoPlanillasErradas = result.Select(x => new DapperTmpDesembolsoAbono
                {
                    NumeroPlanilla = _typeConvertionManager.AnyToString(x.NUMEROPLANILLA),
                    NumeroSecuencia = _typeConvertionManager.AnyToInteger(x.NUMEROSECUENCIA),
                    NumeroRegistrosProcesados = _typeConvertionManager.AnyToInteger(x.NUMEROREGISTROSPROCESADOS),
                    CodigoUnicoProveedor = _typeConvertionManager.AnyToString(x.CODIGOUNICOPROVEEDOR),
                    NumeroReintento = _typeConvertionManager.AnyToInteger(x.NUMEROREINTENTO),
                    EstadoReintento = _typeConvertionManager.AnyToInteger(x.ESTADOREINTENTO),
                    TipoReintento = _typeConvertionManager.AnyToInteger(x.TIPOREINTENTO),
                    FechaRegistro = _typeConvertionManager.AnyToDateTime(x.FECHAREGISTRO, true),
                    FechaActualizacion = _typeConvertionManager.AnyToDateTime(x.FECHACTUALIZACION, false),
                }).ToList();
                return listadoPlanillasErradas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerDesembolsoAbono - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<DataTable> ObtenerTramasParcialTotalPorProveedores(DapperGenerarTramasInput generarTramasInput, int tipoTramaParcial)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                DataTable dt = new DataTable();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.NumeroPlanilla);
                parameters.Add(OracleParameterNames.PivNumeroInstruccion, OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.NumeroInstruccion);
                parameters.Add(OracleParameterNames.pivCodigoUnicoProveedor, OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.CodigoUnicoProveedor);
                parameters.Add("pii_NumeroPlanillaSecuencia", OracleDbType.Int32, ParameterDirection.Input, generarTramasInput.NumeroPlanillaSecuencia);
                parameters.Add("piv_UsuarioLogin", OracleDbType.Varchar2, ParameterDirection.Input, generarTramasInput.UsuarioEjecuta);
                parameters.Add("pii_Desembolsar", OracleDbType.Int32, ParameterDirection.Input, generarTramasInput.FlagDesembolsar);
                parameters.Add(OracleParameterNames.PocTrama, OracleDbType.RefCursor, ParameterDirection.Output);
                var query = string.Empty;

                if (tipoTramaParcial == (int)TipoReintentoProcesoAbono.ParcialTotal)
                {
                    query = OracleProcedures.GenerarTramasParcialTotalPorProveedores;
                }
                else
                {
                    query = OracleProcedures.GenerarTramasPorProveedor;
                }

                var result = await _dapperExecutor.ExecuteReaderAsync(connection, query, parameters, commandType: CommandType.StoredProcedure);
                dt.Load(result);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerTramasParcialTotalPorProveedores - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperNumeroOperacionDesembolso>> ObtenerNroOperacionDesembolso(string numeroPlanilla, string numeroSecuencia)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("PIV_NUMEROPLANILLASEQ", OracleDbType.Int32, ParameterDirection.Input, numeroSecuencia);
                parameters.Add(OracleParameterNames.PocurCursor, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerNumeroOperacionDesembolso, parameters, commandType: CommandType.StoredProcedure);

                var listadoPlanillasErradas = result.Select(x => new DapperNumeroOperacionDesembolso
                {
                    NumeroOperacion = _typeConvertionManager.AnyToString(x.NUMEROOPERACION),
                    NumeroInstruccion = _typeConvertionManager.AnyToString(x.NUMEROINSTRUCCION),
                    NumeroLinea = _typeConvertionManager.AnyToInteger(x.NUMEROLINEA),
                    Neteo = _typeConvertionManager.AnyToDecimal(x.NETEO),
                    Tipo = _typeConvertionManager.AnyToString(x.TIPO),
                    CodigoMonedaWbc = _typeConvertionManager.AnyToInteger(x.CODIGOMONEDAWBC)
                }).ToList();
                return listadoPlanillasErradas;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerNroOperacionDesembolso - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
