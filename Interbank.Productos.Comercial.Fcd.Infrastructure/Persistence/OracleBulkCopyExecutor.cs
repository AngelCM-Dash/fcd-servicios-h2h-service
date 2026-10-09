using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence
{
    public class OracleBulkCopyExecutor : IOracleBulkCopyExecutor
    {
        public void WriteToServer(IDbConnection connection, DataTable table, string destinationTable)
        {
            if (connection is OracleConnection oracleConn)
            {
                using var bulkCopy = new OracleBulkCopy(oracleConn);
                bulkCopy.DestinationTableName = destinationTable;

                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                bulkCopy.WriteToServer(table);
            }
            else
            {
                throw new InvalidOperationException("La conexión proporcionada no es una conexión válida de Oracle.");
            }
        }
    }
}
