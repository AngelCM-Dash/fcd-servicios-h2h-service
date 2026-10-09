namespace Interbank.Productos.Comercial.Fcd.Application.Models.Request
{
    public class SftpConnectionRequest
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RemotePath { get; set; }
        public string? FileName { get; set; }
        public string? ProjectDirectory { get; set; }
        public string? PrivateKeyLocalFilePath { get; set; }
        public int FlagAccesoPPk { get; set; }
        public decimal CodigoCab { get; set; }
        public string? Metodo { get; set; }

    }
}
