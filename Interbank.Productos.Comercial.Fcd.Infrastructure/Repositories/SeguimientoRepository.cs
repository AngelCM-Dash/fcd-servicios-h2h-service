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
    public class SeguimientoRepository : ISeguimientoRepository
    {
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ILogger<SeguimientoRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        public SeguimientoRepository(
            ITypeConvertionManager typeConvertionManager,
            ILogger<SeguimientoRepository> logger,
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

        public async Task<decimal> InsertaSeguimientoCabecera(DapperSeguimientoCabecera command)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_IdDetalle", OracleDbType.Decimal, ParameterDirection.Input, command.IdDetalle);
                parameters.Add("p_NombreArchivo", OracleDbType.Varchar2, ParameterDirection.Input, command.NombreArchivo);
                parameters.Add("p_NroPlanilla", OracleDbType.Varchar2, ParameterDirection.Input, command.NroPlanilla);
                parameters.Add("p_Flag", OracleDbType.Varchar2, ParameterDirection.Input, command.Flag);
                parameters.Add(OracleParameterNames.PResultado, OracleDbType.Decimal, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaSeguimientoCabH2HW, parameters, commandType: CommandType.StoredProcedure);

                var codigoRespuesta = parameters.Get<decimal>(OracleParameterNames.PResultado);

                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertaSeguimientoCabecera - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<decimal> InsertaSeguimientoDetalle(DapperSeguimientoDetalle command)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_IdDetalle", OracleDbType.Decimal, ParameterDirection.Input, command.IdDetalle);
                parameters.Add("p_NombreMetodo", OracleDbType.Varchar2, ParameterDirection.Input, command.NombreMetodo);
                parameters.Add("p_CapaObservacion", OracleDbType.Varchar2, ParameterDirection.Input, command.CapaObservacion);
                parameters.Add("p_DetalleObservacion", OracleDbType.Varchar2, ParameterDirection.Input, command.DetalleObservacion);
                parameters.Add("p_IdEstacion", OracleDbType.Decimal, ParameterDirection.Input, command.IdEstacion);
                parameters.Add(OracleParameterNames.PResultado, OracleDbType.Decimal, ParameterDirection.Output, 0);

                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaSeguimientoDetH2HW, parameters, commandType: CommandType.StoredProcedure);

                var codigoRespuesta = parameters.Get<decimal>(OracleParameterNames.PResultado);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertaSeguimientoDetalle - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ObtenerIdSeguimientoxPlanilla(string numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurCursor, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerSeguimientoIDxPlanilla, parameters, commandType: CommandType.StoredProcedure);

                var firstItem = result.FirstOrDefault();
                return firstItem != null
                    ? _typeConvertionManager.AnyToInteger(firstItem.IDDETALLE)
                    : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerIdSeguimientoxPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }

        }

        public async Task<List<DapperSeguimientoDetalleCabecera>> ObtenerSeguimientoDetalleCabecera(string? fechaDesde, string? fechaHasta, string? nombreArchivo, string? numeroPlanilla)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_FECHADESDE", OracleDbType.Varchar2, ParameterDirection.Input, fechaDesde);
                parameters.Add("PIV_FECHAHASTA", OracleDbType.Varchar2, ParameterDirection.Input, fechaHasta);
                parameters.Add("PIV_NOMBREARCHIVO", OracleDbType.Varchar2, ParameterDirection.Input, nombreArchivo);
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDetalleCabecera, parameters, commandType: CommandType.StoredProcedure);
                var listadoDetCab = result.Select(x => new DapperSeguimientoDetalleCabecera
                {
                    IdSeguimiento = _typeConvertionManager.AnyToDecimal(x.IDSEGUIMIENTO),
                    IdDetalle = _typeConvertionManager.AnyToDecimal(x.IDDETALLE),
                    FechaRegistro = _typeConvertionManager.AnyToString(x.FECHAREGISTRO),
                    NombreArchivo = _typeConvertionManager.AnyToString(x.NOMBREARCHIVO),
                    NroPlanilla = _typeConvertionManager.AnyToString(x.NROPLANILLA),
                    NombreMetodo = _typeConvertionManager.AnyToString(x.NOMBREMETODO),
                    CapaObservacion = _typeConvertionManager.AnyToString(x.CAPAOBSERVACION),
                    DetalleObservacion = _typeConvertionManager.AnyToString(x.DETALLEOBSERVACION),
                    IdEstacion = _typeConvertionManager.AnyToDecimal(x.IDESTACION),
                }).ToList();
                return listadoDetCab;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerSeguimientoDetalleCabecera - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<string> ObtenerTipoEjecucionSeguimientoxIdDetalle(decimal idDetalle)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("PIV_IDDETALLE", OracleDbType.Decimal, ParameterDirection.Input, idDetalle);
                parameters.Add(OracleParameterNames.PocurCursor, OracleDbType.RefCursor, ParameterDirection.Output);
                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerTipoEjecucionxIdDetalle, parameters, commandType: CommandType.StoredProcedure);
                var TipoEjecucion = Convert.ToString(result.Select(x => x.TIPOEJECUCION).FirstOrDefault());
                return TipoEjecucion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerTipoEjecucionSeguimientoxIdDetalle - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
