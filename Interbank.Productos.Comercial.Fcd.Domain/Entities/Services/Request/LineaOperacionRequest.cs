using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class LineaOperacionRequest
    {
        [JsonProperty("customerId")]
        public string? CodigoUnico { get; set; }

        [JsonProperty("ruc")]
        public string? Ruc { get; set; }

        [JsonProperty("creditLine")]
        public string? NumeroLinea { get; set; }

        [JsonProperty("idProduct")]
        public int CodigoProducto { get; set; }

        [JsonProperty("StatusCode")]
        public int CodigoEstado { get; set; }
    }
}
