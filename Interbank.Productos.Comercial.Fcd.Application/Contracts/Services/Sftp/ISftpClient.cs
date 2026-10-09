namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp
{
    public interface ISftpClient
    {
        bool IsConnected { get; }
        void Connect();
        void Disconnect();
        void ChangeDirectory(string path);
        bool Exists(string path);
        string[] ReadAllLines(string path);
        void UploadFile(Stream file, string path);
        void DownloadFile(string path, Stream destination);
        void Dispose();
    }
}
