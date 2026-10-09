using Dapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence
{
    [ExcludeFromCodeCoverage]
    public class DapperExecutor : IDapperExecutor
    {
        public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null)
        {
            return await connection.ExecuteAsync(
                sql,
                parameters,
                commandType: commandType);
        }

        public async Task<IEnumerable<dynamic>> QueryAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null)
        {
            return await connection.QueryAsync(
                sql,
                parameters,
                commandType: commandType);
        }

        public async Task<IDataReader> ExecuteReaderAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            return await connection.ExecuteReaderAsync(
                sql,
                parameters,
                commandType: commandType);
        }

        public async Task ExecuteForAllInsertAsync(IDbConnection connection, object coleccion, string udtTypeName)
        {
            // 1. Cast seguro: Si no es OracleConnection, lanzamos error claro
            if (connection is not OracleConnection oracleConn)
            {
                throw new ArgumentException("La conexión debe ser de tipo OracleConnection", nameof(connection));
            }

            // 2. Crear el comando (Igual a tu lógica original)
            using var cmd = new OracleCommand(OracleProcedures.InsertaDataBulkInsertForAll, oracleConn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // 3. Configurar el parámetro especial de Oracle
            var param = new OracleParameter
            {
                ParameterName = OracleParameterNames.PInsertH2h,
                OracleDbType = OracleDbType.Object, // Esto es lo que rompe los tests si está en el repo
                UdtTypeName = udtTypeName,
                Direction = ParameterDirection.Input,
                Value = coleccion
            };

            cmd.Parameters.Add(param);

            // 4. Ejecución
            await cmd.ExecuteNonQueryAsync();
        }


    }
}
