namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class BaseResponse
    {
        public int CodigoRespuesta { get; set; }
        public string MensajeRespuesta { get; set; } = string.Empty;
    }
}
