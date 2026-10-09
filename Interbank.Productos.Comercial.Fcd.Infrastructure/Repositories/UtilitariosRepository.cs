using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DataTable;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.OracleCustomTypeMapping;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using NextSIT.Utility;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    public class UtilitariosRepository : IUtilitariosRepository
    {
        private readonly ITypeConvertionManager _typeConversionManager;
        private readonly ILogger<UtilitariosRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;
        private readonly IOracleBulkCopyExecutor _bulkCopyExecutor;

        public UtilitariosRepository(
            ITypeConvertionManager typeConversionManager,
            ILogger<UtilitariosRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor,
            IOracleBulkCopyExecutor bulkCopyExecutor)
        {
            ArgumentNullException.ThrowIfNull(typeConversionManager);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);
            ArgumentNullException.ThrowIfNull(bulkCopyExecutor);

            _typeConversionManager = typeConversionManager;
            _logger = logger;
            _oracleConnectionFactory = oracleConnectionFactory;
            _dapperExecutor = dapperExecutor;
            _bulkCopyExecutor = bulkCopyExecutor;
        }

        public async Task<List<DapperParametroDB2>> ObtenerParametrosDB2PorCodigoDominioDB2(int codigoDominio, int numOrden)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CODIGODOMINIODB2", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
                parameters.Add("PIV_NUMORDEN", OracleDbType.Int64, ParameterDirection.Input, numOrden);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerListadoParametrosDB2PorCodigoDominioDB2, parameters, commandType: CommandType.StoredProcedure);

                var ListadoParametro = result.Select(x => new DapperParametroDB2
                {
                    CodigoParametroDB2 = _typeConversionManager.AnyToInteger(x.CODIGOPARAMETRODB2),
                    CodigoDominioDB2 = _typeConversionManager.AnyToInteger(x.CODIGODOMINIODB2),
                    Descripcion = _typeConversionManager.AnyToString(x.DESCRIPCION),
                    Script = _typeConversionManager.AnyToString(x.SCRIPT),
                    Estado = _typeConversionManager.AnyToInteger(x.ESTADO),
                    NumOrden = _typeConversionManager.AnyToInteger(x.NUMORDEN)
                }).ToList();

                return ListadoParametro;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerParametrosDB2PorCodigoDominioDB2 - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<List<DapperParametro>> ObtenerParametrosPorCodigoDominio(int codigoDominio)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CODIGODOMINIO", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerListadoParametrosPorCodigoDominio, parameters, commandType: CommandType.StoredProcedure);
                var ListadoParametro = result.Select(x => new DapperParametro
                {

                    CODIGOPARAMETRO = _typeConversionManager.AnyToInteger(x.CODIGOPARAMETRO),
                    CODIGODOMINIO = _typeConversionManager.AnyToInteger(x.CODIGODOMINIO),
                    DESCRIPCION = _typeConversionManager.AnyToString(x.DESCRIPCION),
                    ESTADO = _typeConversionManager.AnyToInteger(x.ESTADO),
                    CODIGOREFERENCIA = _typeConversionManager.AnyToString(x.CODIGOREFERENCIA),
                    NUMEROORDEN = _typeConversionManager.AnyToInteger(x.NUMEROORDEN),
                    DESCRIPCIONCORTA = _typeConversionManager.AnyToString(x.DESCRIPCIONCORTA),
                    CODIGOREFERENCIAPARAMETRO = _typeConversionManager.AnyToInteger(x.CODIGOREFERENCIAPARAMETRO),
                    OPCION1 = _typeConversionManager.AnyToString(x.OPCION1),
                }).ToList();

                return ListadoParametro;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerParametrosPorCodigoDominio - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public bool BulkInsertTable(DataTable dt, string nombreTabla)
        {
            using var cn = _oracleConnectionFactory.CrearConexionBaseDatos();
            cn.Open();

            using var transaction = cn.BeginTransaction();

            try
            {
                _bulkCopyExecutor.WriteToServer(cn, dt, nombreTabla);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "BulkInsertTable - Error en base de datos");
                throw new InvalidOperationException("Error en base de datos", ex);
            }
        }

        public async Task<bool> ForAllInsertDataObject(DataTable dt, string nombreTabla)
        {
            try
            {
                // 1. DataTable a Lista (Lógica de negocio)
                var documentos = ConvertirDataTableADocumentos(dt);
                var coleccion = new CabDocumentosH2H(documentos);

                // 2. Conexión estándar
                using var cn = _oracleConnectionFactory.CrearConexionBaseDatos();
                await ((DbConnection)cn).OpenAsync();

                // 3. Ejecución delegada (Lógica de infraestructura)
                await _dapperExecutor.ExecuteForAllInsertAsync(cn, coleccion, nombreTabla);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForAllInsertDataObject - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        private static DocumentoH2H[] ConvertirDataTableADocumentos(DataTable dt)
        {
            return dt.AsEnumerable().Select(row => new DocumentoH2H
            {

                NumeroInterno = row.Field<string>(ColumnasDocumentosH2H.NumeroInterno),
                Item = row.Field<int?>(ColumnasDocumentosH2H.Item),
                CodigoClientePlanilla = row.Field<int?>(ColumnasDocumentosH2H.CodigoClientePlanilla),
                NumeroDocumentoFisico = row.Field<string>(ColumnasDocumentosH2H.NumeroDocumentoFisico),
                CodigoMoneda = row.Field<int?>(ColumnasDocumentosH2H.CodigoMoneda),
                ImporteOriginal = row.Field<decimal?>(ColumnasDocumentosH2H.ImporteOriginal),
                FechaVencimiento = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaVencimiento),
                NumeroPlanilla = row.Field<string>(ColumnasDocumentosH2H.NumeroPlanilla),
                CodigoCliente = row.Field<int>(ColumnasDocumentosH2H.CodigoCliente),
                FechaRegistro = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaRegistro),
                Protestable = row.Field<int?>(ColumnasDocumentosH2H.Protestable),
                FlagCuota = row.Field<int?>(ColumnasDocumentosH2H.FlagCuota),
                FlagCompletado = row.Field<int?>(ColumnasDocumentosH2H.FlagCompletado),
                NumeroCuotas = row.Field<int?>(ColumnasDocumentosH2H.NumeroCuotas),
                NumeroDocumentoAceptante = row.Field<string>(ColumnasDocumentosH2H.NumeroDocumentoAceptante),
                RazonSocialAceptante = row.Field<string>(ColumnasDocumentosH2H.RazonSocialAceptante),
                TipoDocumentoAceptante = row.Field<int?>(ColumnasDocumentosH2H.TipoDocumentoAceptante),
                CodigoTipoDocumentoCobranza = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoDocumentoCobranza),
                CodigoTipoAbono = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoAbono),
                CodigoTipoCuenta = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoCuenta),
                NumeroCuenta = row.Field<string>(ColumnasDocumentosH2H.NumeroCuenta),
                NumeroLinea = row.Field<string>(ColumnasDocumentosH2H.NumeroLinea),
                FechaCargo = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaCargo),
                NumeroInstruccion = row.Field<string>(ColumnasDocumentosH2H.NumeroInstruccion),
                FechaAdelanto = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaAdelanto),
                SaldoActualDocumento = row.Field<decimal?>(ColumnasDocumentosH2H.SaldoActualDocumento),
                CodigoTipoAdelanto = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoAdelanto),
                FlagObservado = row.Field<int?>(ColumnasDocumentosH2H.FlagObservado),
                Observacion = row.Field<string>(ColumnasDocumentosH2H.Observacion),
                ReglasValidacion = row.Field<string>(ColumnasDocumentosH2H.ReglasValidacion),
                CodigoEstado = row.Field<int?>(ColumnasDocumentosH2H.CodigoEstado),
                AplicaPortes = row.Field<int?>(ColumnasDocumentosH2H.AplicaPortes),
                DiasAmpliacion = row.Field<int?>(ColumnasDocumentosH2H.DiasAmpliacion),
                ValidarCuentaCliente = row.Field<int?>(ColumnasDocumentosH2H.ValidarCuentaCliente),
                DesembolsoAutomatico = row.Field<int?>(ColumnasDocumentosH2H.DesembolsoAutomatico),
                PorcentajeProrroga = row.Field<decimal?>(ColumnasDocumentosH2H.PorcentajeProrroga),
                TipoCambioWDC = row.Field<decimal?>(ColumnasDocumentosH2H.TipoCambioWDC),
            }).ToArray();
        }

        public async Task ActualizarParametroPorCodigoDominioAndNumOrden(int codigoDominio, string descripcionCorta, int numOrden)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_CODIGODOMINIO", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
                parameters.Add("PIV_DESCRIPCIONCORTA", OracleDbType.Varchar2, ParameterDirection.Input, descripcionCorta);
                parameters.Add("PIV_NUMEROORDEN", OracleDbType.Int64, ParameterDirection.Input, numOrden);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ActualizaParametroPorDominioAndNumOrden, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarParametroPorCodigoDominioAndNumOrden - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
