namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp
{
    public interface ISftpClientFactory
    {
        ISftpClientWrapper CreateClient(string host, int port, string username, string password, string rutaClavePpk, bool usePrivateKey);
    }
}
