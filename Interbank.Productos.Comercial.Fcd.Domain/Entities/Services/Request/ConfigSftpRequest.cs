namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class ConfigSftpRequest
    {
        public string? Host { get; set; }
        public int Puerto { get; set; }
        public string? Usuario { get; set; }
        public string? Password { get; set; }
        public string? ArchivoPpk { get; set; }
        public string? Flag { get; set; }
        public string? RutaArchivo { get; set; }
    }
}
