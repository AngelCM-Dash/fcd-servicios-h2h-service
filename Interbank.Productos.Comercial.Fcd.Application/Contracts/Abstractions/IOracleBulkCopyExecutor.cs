using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions
{
    public interface IOracleBulkCopyExecutor
    {
        void WriteToServer(IDbConnection connection, DataTable table, string destinationTable);
    }
}
