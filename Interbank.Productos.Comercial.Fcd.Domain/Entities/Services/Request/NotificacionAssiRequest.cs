using Newtonsoft.Json;

namespace Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request
{
    public class NotificacionAssiRequest
    {
        [JsonProperty("eventCode")]
        public string? CodigoEvento { get; set; }

        [JsonProperty("description")]
        public string? Descripcion { get; set; }

        [JsonProperty("responseCode")]
        public string? CodigoRespuesta { get; set; }

        [JsonProperty("payrollNumber")]
        public string? NumeroPlanilla { get; set; }

        [JsonProperty("groupingCodeNumber")]
        public string? NumeroCodigoAgrupacion { get; set; }

        [JsonProperty("fileName")]
        public string? NombreArchivo { get; set; }

        [JsonProperty("filePath")]
        public string? RutaArchivo { get; set; }



        public NotificacionAssiRequest(
                string? codigoEvento,
                string? descripcion,
                string? codigoRespuesta,
                string? numeroPlanilla,
                string? numeroCodigoAgrupacion,
                string? nombreArchivo,
                string? rutaArchivo)
        {
            CodigoEvento = codigoEvento;
            Descripcion = descripcion;
            CodigoRespuesta = codigoRespuesta;
            NumeroPlanilla = numeroPlanilla;
            NumeroCodigoAgrupacion = numeroCodigoAgrupacion;
            NombreArchivo = nombreArchivo;
            RutaArchivo = rutaArchivo;
        }
    }
}
