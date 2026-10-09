using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions
{
    public interface IDapperExecutor
    {
        Task<IEnumerable<dynamic>> QueryAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null);
        Task<IDataReader> ExecuteReaderAsync(IDbConnection connection, string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
        Task ExecuteForAllInsertAsync(IDbConnection connection, object coleccion, string udtTypeName);
    }
}
