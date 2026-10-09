using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using System.Diagnostics.CodeAnalysis;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
{
    [ExcludeFromCodeCoverage]
    public class RenciSftpClientAdapter : ISftpClient
    {
        private readonly Renci.SshNet.SftpClient _client;

        public RenciSftpClientAdapter(Renci.SshNet.SftpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public bool IsConnected => _client.IsConnected;
        public void Connect() => _client.Connect();
        public void Disconnect() => _client.Disconnect();
        public void ChangeDirectory(string path) => _client.ChangeDirectory(path);
        public bool Exists(string path) => _client.Exists(path);
        public string[] ReadAllLines(string path) => _client.ReadAllLines(path);
        public void UploadFile(Stream file, string path) => _client.UploadFile(file, path);
        public void DownloadFile(string path, Stream destination) => _client.DownloadFile(path, destination);
        public void Dispose() => _client.Dispose();
    }
}
