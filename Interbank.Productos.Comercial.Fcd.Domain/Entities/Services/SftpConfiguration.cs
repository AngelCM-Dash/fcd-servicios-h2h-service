namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services
{
    public class SftpConfiguration
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RemotePath { get; set; }
        public string? FileName { get; set; }

    }
}
