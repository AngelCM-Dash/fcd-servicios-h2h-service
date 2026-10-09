using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using ISftpClient = Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp.ISftpClient;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
{
    public class SftpClientWrapper : ISftpClientWrapper
    {
        private readonly ISftpClient _client;
        private bool _disposed;

        public SftpClientWrapper(ISftpClient client)
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

        // Dispose seguro con patrón recomendado
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_client.IsConnected)
                    _client.Disconnect();

                _client.Dispose();
            }

            _disposed = true;
        }
    }
}
