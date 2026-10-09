namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class CargaMasivaRequest
    {
        public int? CodigoProducto { get; set; }
        public string? CanalAtencion { get; set; }
        public int? CodigoUsuario { get; set; }
        public string? Usuario { get; set; }
        public int? CodigoPerfilUsuario { get; set; }
        public int? CodigoTienda { get; set; }
        public string? Tienda { get; set; }
        public bool? ContratoMarco { get; set; }
        public string? RutaArchivo { get; set; }
        public string? NombreArchivo { get; set; }
        public string? CodigoUnico { get; set; }
        public DateTime? FechaValor { get; set; }
        public List<AdicionalCRequest>? Adicional { get; set; }
    }

    public class AdicionalCRequest
    {
        public string? Campo { get; set; }
    }
}

