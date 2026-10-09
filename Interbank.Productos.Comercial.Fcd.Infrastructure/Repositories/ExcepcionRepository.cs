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
    public class ExcepcionRepository : IExcepcionRepository
    {
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _databaseExecutor;
        private readonly ITypeConvertionManager _typeConversionManager;
        private readonly ILogger<ExcepcionRepository> _logger;

        public ExcepcionRepository(
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor databaseExecutor,
            ITypeConvertionManager typeConvertionManager,
            ILogger<ExcepcionRepository> logger)
        {
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(databaseExecutor);
            ArgumentNullException.ThrowIfNull(typeConvertionManager);
            ArgumentNullException.ThrowIfNull(logger);

            _oracleConnectionFactory = oracleConnectionFactory;
            _databaseExecutor = databaseExecutor;
            _typeConversionManager = typeConvertionManager;
            _logger = logger;
        }


        public async Task<List<DapperComisionProveedor>> ConsultaCalculoInteresComision(int secuencia)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();

                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_CodigoSecuencia", OracleDbType.Int64, ParameterDirection.Input, secuencia);
                parameters.Add(OracleParameterNames.PCursor, OracleDbType.RefCursor, ParameterDirection.Output);

                // Ejecutar SP con Dapper
                var result = await _databaseExecutor.QueryAsync(connection, OracleProcedures.CalculoInteresComision, parameters, commandType: CommandType.StoredProcedure);

                // Mapear resultados a DTO
                return result.Select(x => new DapperComisionProveedor
                {
                    CodigoProveedor = _typeConversionManager.AnyToString(x.CODIGOUNICOPROVEEDOR),
                    ProveedorImporteDescuento = _typeConversionManager.AnyToDecimal(x.IMPORTEDESCUENTO),
                    ProveedorImportePortes = _typeConversionManager.AnyToDecimal(x.IMPORTEPORTES),
                    Total = _typeConversionManager.AnyToDecimal(x.TOTAL)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConsultaCalculoInteresComision - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }



        public async Task<int> GeneraCodigoSecuenciaInteresComision()
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();

                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PoRespuesta, OracleDbType.Int32, ParameterDirection.Output, 0);

                await _databaseExecutor.ExecuteAsync(connection, OracleProcedures.ObtenerCodigoSecuenciaCalculo, parameters, commandType: CommandType.StoredProcedure);

                var codigoSecuencia = parameters.Get<int>("PO_RESPUESTA");

                return codigoSecuencia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeneraCodigoSecuenciaInteresComision - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}

