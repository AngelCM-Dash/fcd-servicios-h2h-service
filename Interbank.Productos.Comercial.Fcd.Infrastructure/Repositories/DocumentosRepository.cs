using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    public class DocumentosRepository : IDocumentosRepository
    {
        private readonly ILogger<DocumentosRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        public DocumentosRepository(
            ILogger<DocumentosRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);

            _logger = logger;
            _oracleConnectionFactory = oracleConnectionFactory;
            _dapperExecutor = dapperExecutor;
        }

        public async Task RechazarDocumentosDiferidos(string numeroPlanilla, string numeroLinea, string lineaObservacion, string fechaAdelanto)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_NumeroLinea", OracleDbType.Varchar2, ParameterDirection.Input, numeroLinea);
                parameters.Add("piv_LineaObservacion", OracleDbType.Varchar2, ParameterDirection.Input, lineaObservacion);
                DateTime fecha = DateTime.ParseExact(fechaAdelanto, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                parameters.Add("piv_FechaAdelanto", OracleDbType.Date, ParameterDirection.Input, fecha);
                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.RechazoDocumentoDiferidos, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RechazarDocumentosDiferidos - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task RechazarDocumentosDistribuido(string numeroPlanilla, string observacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
                parameters.Add("piv_Observacion", OracleDbType.Varchar2, ParameterDirection.Input, observacion);
                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.RechazoDocumentoDistribuido, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RechazarDocumentosDistribuido - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
