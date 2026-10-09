using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response
{
    public class TokenAuthorizationResponse
    {
        [JsonProperty("token_type")]
        public string? TipoToken { get; set; }
        [JsonProperty("access_token")]
        public string? AccesoToken { get; set; }
        [JsonProperty("scope")]
        public string? Alcance { get; set; }
        [JsonProperty("expires_in")]
        public int? Expiracion { get; set; }
        [JsonProperty("consented_on")]
        public int? Consentido { get; set; }
    }
}
