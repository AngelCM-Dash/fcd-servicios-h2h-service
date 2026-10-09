using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Newtonsoft.Json;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Common
{
    public static class HttpClientHelper
    {
        public static async Task<TResponse> EnviarSolicitudHttp<TRequest, TResponse>(string url, TRequest request) where TResponse : class
        {
            using (var client = new HttpClient())
            {
                var jsonSolicitud = JsonConvert.SerializeObject(request);
                var response = await client.PostAsync(url, new StringContent(jsonSolicitud, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseObj = JsonConvert.DeserializeObject<TResponse>(content);
                    if (responseObj != null)
                    {
                        return responseObj;
                    }
                    else
                    {
                        throw new InvalidOperationException("La respuesta del servidor está vacía o no se pudo deserializar correctamente.");
                    }
                }
                else
                {
                    throw new HttpRequestException($"Error al realizar la solicitud HTTP. Código de estado: {response.StatusCode}");
                }
            }
        }

        public static HttpClient PrepararHeaderHttpClient(IEnumerable<DapperParametro> parametros, string headerAuthorization, string tokenAcceso)
        {
            var requestBodyDict = parametros
                .Where(parametro => parametro.DESCRIPCION != null && parametro.DESCRIPCIONCORTA != null)
                .ToDictionary(parametro => parametro.DESCRIPCION!, parametro => parametro.DESCRIPCIONCORTA!);

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // NOSONAR
            };

            var client = new HttpClient(handler);

            if (!string.IsNullOrWhiteSpace(headerAuthorization) && !headerAuthorization.Equals("No Auth", StringComparison.OrdinalIgnoreCase))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(headerAuthorization, tokenAcceso);
            }

            foreach (var kvp in requestBodyDict)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }

            return client;
        }

    }
}
