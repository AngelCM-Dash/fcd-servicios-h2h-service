using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence
{
    public class OracleConnectionFactory : IOracleConnectionFactory
    {
        private readonly string _connectionString;

        public OracleConnectionFactory(IConfiguration configuration, IEncrypterService encrypter)
        {
            var encryptedConnection = configuration.GetConnectionString("connectionString")
                ?? throw new InvalidOperationException("ConnectionString no configurado");

            _connectionString = encrypter.Decrypt(encryptedConnection);
        }

        public IDbConnection CrearConexionBaseDatos()
        {
            return new OracleConnection(_connectionString)
            {
                BindByName = true
            };
        }

    }

}
