namespace Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class AfiliacionResponse
    {
        public string? CodigoRespuesta { get; set; }
        public string? MensajeRespuesta { get; set; }
        public string? NombreContacto { get; set; }
        public string? CorreoContacto { get; set; }
        public int? CodigoAfiliacion { get; set; }
        public string? RazonSocialCliente { get; set; }
        public string? RazonSocialProveedor { get; set; }
        public string? NumeroDocumentoCliente { get; set; }
        public string? NumeroDocumentoProveedor { get; set; }
    }
}
