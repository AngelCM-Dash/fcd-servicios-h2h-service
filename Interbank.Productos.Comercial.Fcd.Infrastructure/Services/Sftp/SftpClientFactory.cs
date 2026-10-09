using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Renci.SshNet;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
{
    public class SftpClientFactory : ISftpClientFactory
    {
        public ISftpClientWrapper CreateClient(string host, int port, string username, string password, string rutaClavePpk, bool usePrivateKey)
        {
            AuthenticationMethod authMethod;

            if (usePrivateKey)
            {
                var keyFile = new PrivateKeyFile(rutaClavePpk);
                authMethod = new PrivateKeyAuthenticationMethod(username, keyFile);
            }
            else
            {
                authMethod = new PasswordAuthenticationMethod(username, password);
            }

            var connectionInfo = new ConnectionInfo(host, port, username, authMethod);
            var renciClient = new Renci.SshNet.SftpClient(connectionInfo);

            // Aquí creamos el adapter para cumplir ISftpClient
            var clientAdapter = new RenciSftpClientAdapter(renciClient);

            // Pasamos el adapter al wrapper
            return new SftpClientWrapper(clientAdapter);
        }
    }
}
