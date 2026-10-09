namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class DesembolsoRequest
    {
        public required string numeroPlanilla { get; set; }
        public int? estadoPlanilla { get; set; }
        public int? estadoDocumento { get; set; }
        public string? codigoAgrupamiento { get; set; }
        public int? codigoPerfilUsuario { get; set; }
        public string? codigoUsuario { get; set; }
        public string? nombreUsuarioRegistro { get; set; }
        public string? canalAtencion { get; set; }
        public string? comentario { get; set; }
        public string? codigoTienda { get; set; }
        public Boolean flagDesembolsoTotal { get; set; }
        public string? codigoUnico { get; set; }
        public List<int>? CodigoReserva { get; set; }
    }
}
