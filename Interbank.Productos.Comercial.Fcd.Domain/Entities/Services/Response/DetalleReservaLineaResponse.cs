using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class DetalleReservaLineaResponse
    {
        [JsonProperty("requestId")]
        public long CodigoSolicitud { get; set; }
        [JsonProperty("creditLine")]
        public string? LineaCredito { get; set; }
        [JsonProperty("customerId")]
        public string? CodigoUnico { get; set; }
        [JsonProperty("currencyId")]
        public string? CodigoMoneda { get; set; }
        [JsonProperty("operationLineAmount")]
        public decimal LineaMontoOperacion { get; set; }
        [JsonProperty("currencyIdOriginal")]
        public string? CodigoMonedaOriginal { get; set; }
        [JsonProperty("operationLineAmountOriginal")]
        public decimal LineaMontoOperacionOriginal { get; set; }
    }
}
