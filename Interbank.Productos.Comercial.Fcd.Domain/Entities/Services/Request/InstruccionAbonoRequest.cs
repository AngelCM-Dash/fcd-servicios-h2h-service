using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class InstruccionAbonoRequest
    {
        [JsonProperty("payrollNumber")]
        public string? NumeroPlanilla { get; set; }
        [JsonProperty("payrollSequence")]
        public int NumeroSecuencia { get; set; }
        [JsonProperty("customerId")]
        public string? CodigoUnico { get; set; }
        [JsonProperty("processingDate")]
        public string? FechaProceso { get; set; }
    }
}
