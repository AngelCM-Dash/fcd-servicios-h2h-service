using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions
{
    public interface IOracleConnectionFactory
    {
        IDbConnection CrearConexionBaseDatos();
    }
}
