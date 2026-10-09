using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class MovimientoRequest
    {
        [JsonProperty("creditLine")]
        public List<string>? NumeroLinea { get; set; }
        [JsonProperty("type")]
        public List<string>? Tipo { get; set; }
        [JsonProperty("amount")]
        public List<decimal>? Monto { get; set; }
        [JsonProperty("currencyId")]
        public List<int>? CodigoMoneda { get; set; }
        [JsonProperty("operationNumber")]
        public List<string>? NumeroOperacion { get; set; }
        [JsonProperty("balanceOperation")]
        public List<decimal>? SaldoOperacion { get; set; }
    }
}
