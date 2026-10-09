using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ApiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.TokenAuthorizationConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Desembolso
{
    [ExcludeFromCodeCoverage]
    public class CommonService : ICommonService
    {
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly IEncrypterService _encrypter;
        private readonly ILogger<CommonService> _logger;

        public CommonService(IUtilitariosRepository utilitariosRepository, IEncrypterService encrypter, ILogger<CommonService> logger)
        {
            ArgumentNullException.ThrowIfNull(utilitariosRepository);
            ArgumentNullException.ThrowIfNull(encrypter);
            ArgumentNullException.ThrowIfNull(logger);

            _utilitariosRepository = utilitariosRepository;
            _encrypter = encrypter;
            _logger = logger;
        }

        public async Task<TokenAuthorizationResponse> GenerarTokenAuthorization(int tipoScope, string Canal)
        {
            try
            {
                List<DapperParametro> paramToken;

                if (Canal == CanalFCD)
                {
                    paramToken = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicTokenAuthorizationFCD);
                }
                else if (Canal == CanalWBC)
                {
                    paramToken = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicTokenAuthorizationWBC);
                }
                else
                {
                    paramToken = new List<DapperParametro>();
                }

                string keyToRemove = tipoScope == (int)NumOrdenDominioApicTokenAuthorization.ScopeTransaccion ? ScopeConsulta : ScopeTransaccion;

                var requestBodyDict = paramToken.Where(parametro => parametro.DESCRIPCION != keyToRemove && parametro.DESCRIPCIONCORTA != null)
                .ToDictionary(parametro => parametro.DESCRIPCION!, parametro => parametro.DESCRIPCIONCORTA!);

                string keyToReplace = keyToRemove == ScopeConsulta ? ScopeTransaccion : ScopeConsulta;
                var urlApi = requestBodyDict[$"{UrlApic}"];

                if (requestBodyDict.TryGetValue(keyToReplace, out var valueToReplace))
                {
                    requestBodyDict.Remove(keyToReplace);

                    requestBodyDict[$"{ScopeBase}"] = valueToReplace;
                }

                requestBodyDict[$"{Username}"] = _encrypter.Decrypt(requestBodyDict[$"{Username}"]);
                requestBodyDict[$"{Password}"] = _encrypter.Decrypt(requestBodyDict[$"{Password}"]);
                requestBodyDict[$"{ClientID}"] = _encrypter.Decrypt(requestBodyDict[$"{ClientID}"]);
                requestBodyDict[$"{ClientSecret}"] = _encrypter.Decrypt(requestBodyDict[$"{ClientSecret}"]);

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; } // NOSONAR
                };

                using (var client = new HttpClient(handler))
                {
                    var requestBody = new FormUrlEncodedContent(requestBodyDict.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)));

                    var response = await client.PostAsync(urlApi, requestBody);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseService = JsonConvert.DeserializeObject<TokenAuthorizationResponse>(content);
                        if (responseService == null)
                        {
                            throw new InvalidOperationException("La deserialización del token de autorización devolvió un objeto nulo.");
                        }
                        return responseService;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error al obtener la respuesta del servidor Código de estado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error al realizar la solicitud HTTP. {ex.Message}");
            }
        }

        public async Task<BaseResponse> NotificacionAssi(NotificacionAssiRequest notificacionAssiRequest)
        {
            try
            {
                var tokenAuthorization = await GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeTransaccion, CanalFCD);

                if (tokenAuthorization.AccesoToken == null)
                {
                    var responseService = new BaseResponse
                    {
                        CodigoRespuesta = 36,
                        MensajeRespuesta = "No se pudo generar el token"
                    };
                    return responseService;
                }

                var paramNotifi = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicServiciosNotificacionAssi);

                foreach (var parametro in paramNotifi)
                {
                    if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret || parametro.DESCRIPCION == AzureSubscriptionKey)
                    {
                        parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                    }
                }

                var urlApi = paramNotifi
                .Where(param => param.DESCRIPCION == $"{UrlApic}")
                .Select(param => param.DESCRIPCIONCORTA)
                .FirstOrDefault();

                if (string.IsNullOrEmpty(urlApi))
                {
                    throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
                }

                using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramNotifi, HeaderAuthorization, tokenAuthorization.AccesoToken))
                {
                    return await PostJsonAndValidateAsync(
                        client,
                        urlApi,
                        notificacionAssiRequest,
                        ContentTypeApplicationJson);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el llamado al servicio Apic IFX Notificacion Assi al procesar la solicitud");
                throw new HttpRequestException("Error al realizar la solicitud HTTP en NotificacionAssi", ex);
            }
        }

        public async Task<BaseResponse> NotificacionBackAssi(NotificacionAssiRequest notificacionAssiRequest)
        {
            try
            {
                var paramNotifiOriginal = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApiServiciosNotificacionAssi);

                var paramNotifi = paramNotifiOriginal
                    .Where(p => p.NUMEROORDEN != 3)
                    .ToList();

                foreach (var parametro in paramNotifi)
                {
                    if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret || parametro.DESCRIPCION == AzureSubscriptionKey)
                    {
                        parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                    }
                }

                var urlApi = paramNotifi
                .Where(param => param.DESCRIPCION == $"{UrlApic}")
                .Select(param => param.DESCRIPCIONCORTA)
                .FirstOrDefault();

                if (string.IsNullOrEmpty(urlApi))
                {
                    throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
                }

                using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramNotifi, AuthorizationNoAuth, string.Empty))
                {
                    return await PostJsonAndValidateAsync(
                        client,
                        urlApi,
                        notificacionAssiRequest,
                        ContentTypeApplicationJson);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el llamado al servicio Apic Notificacion Assi al procesar la solicitud");
                throw new HttpRequestException("Error al realizar la solicitud HTTP en NotificacionBackAssi", ex);
            }
        }

        private static async Task<BaseResponse> PostJsonAndValidateAsync(HttpClient client, string url, object request, string contentType)
        {
            var json = JsonConvert.SerializeObject(request);
            using var content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new BaseResponse
                {
                    CodigoRespuesta = 32,
                    MensajeRespuesta = "Respuesta Correcta"
                };
            }

            var busCode = response.Headers.TryGetValues(HeaderBusResponseCode, out var code)
                ? code.FirstOrDefault()
                : null;

            var busMessage = response.Headers.TryGetValues(HeaderBusResponseMessage, out var msg)
                ? msg.FirstOrDefault()
                : null;

            throw new InvalidOperationException(
                $"Error al obtener la respuesta del servidor Código de estado: {response.StatusCode}, " +
                $"Codigo Respuesta Bus: {busCode} - Codigo Mensaje Bus {busMessage}");
        }
    }
}
