using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class MovimientoResponse
    {
        public string? CodigoRespuesta { get; set; }
        public string? MensajeRespuesta { get; set; } = string.Empty;
        [JsonProperty("Movement")]
        public List<MovimientoResponseBody>? Movimientos { get; set; }
    }
    public class MovimientoResponseBody
    {
        [JsonProperty("creditLine")]
        public string? NumeroLinea { get; set; } = string.Empty;
        [JsonProperty("movementNumber")]
        public string? NumeroMovimiento { get; set; } = string.Empty;
        [JsonProperty("observation")]
        public string? Observacion { get; set; } = string.Empty;
    }


}
