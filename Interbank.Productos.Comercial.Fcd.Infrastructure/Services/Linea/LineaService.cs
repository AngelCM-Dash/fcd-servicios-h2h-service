using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Common;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ApiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Lineas
{
    [ExcludeFromCodeCoverage]
    public class LineaService : ILineaService
    {
        private readonly ICommonService _commonService;
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly IEncrypterService _encrypter;

        public LineaService(ICommonService commonService, IUtilitariosRepository utilitariosRepository, IEncrypterService encrypter)
        {
            ArgumentNullException.ThrowIfNull(commonService);
            ArgumentNullException.ThrowIfNull(utilitariosRepository);
            ArgumentNullException.ThrowIfNull(encrypter);

            _commonService = commonService;
            _utilitariosRepository = utilitariosRepository;
            _encrypter = encrypter;
        }

        public async Task<LineaOperacionResponse> ConsultaLineas(LineaOperacionRequest lineaOperacionRequest)
        {
            var tokenAuthorization = await _commonService.GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeConsulta, CanalWBC);
            if (tokenAuthorization.AccesoToken == null)
            {
                throw new ArgumentNullException($"{tokenAuthorization.AccesoToken}", "La respuesta del servidor está vacía.");
            }

            var paramLineas = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioConsultaLineas);

            foreach (var parametro in paramLineas)
            {
                if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret)
                {
                    parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                }
            }

            var urlApi = paramLineas
            .Where(param => param.DESCRIPCION == $"{UrlApic}")
            .Select(param => param.DESCRIPCIONCORTA)
            .FirstOrDefault();

            if (string.IsNullOrEmpty(urlApi))
            {
                throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
            }

            using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramLineas, HeaderAuthorization, tokenAuthorization.AccesoToken))
            {
                var jsonSolicitud = JsonConvert.SerializeObject(lineaOperacionRequest);
                var response = await client.PostAsync(urlApi, new StringContent(jsonSolicitud, Encoding.UTF8, ContentTypeApplicationJson));

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        throw new InvalidOperationException("La respuesta del servidor está vacía o no se pudo deserializar correctamente.");
                    }

                    var responseObject = JsonConvert.DeserializeObject<LineaOperacionResponse>(responseBody);
                    if (responseObject == null)
                    {
                        throw new InvalidOperationException("El objeto de respuesta es nulo.");
                    }

                    return responseObject;
                }
                else
                {
                    throw new HttpRequestException($"Error al obtener la respuesta del servidor. Código de estado: {response.StatusCode}");
                }
            }
        }

        public async Task<List<DetalleReservaLineaResponse>> ObtenerDetalleReservaLinea(string numeroPlanilla)
        {
            var tokenAuthorization = await _commonService.GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeConsulta, CanalWBC);
            if (tokenAuthorization.AccesoToken == null)
            {
                throw new ArgumentNullException($"{tokenAuthorization.AccesoToken}", "La respuesta del servidor está vacía.");
            }

            var paramLineas = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioConsultaDetalleReservaLineas);

            foreach (var parametro in paramLineas)
            {
                if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret)
                {
                    parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                }
            }

            var urlApi = paramLineas
                .Where(param => param.DESCRIPCION == $"{UrlApic}")
                .Select(param => param.DESCRIPCIONCORTA)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(urlApi))
            {
                urlApi = urlApi.Replace("disbursementNumber", numeroPlanilla);
            }
            else
            {
                throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
            }

            using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramLineas, HeaderAuthorization, tokenAuthorization.AccesoToken))
            {
                var response = await client.GetAsync(urlApi);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var perfilResponse = JsonConvert.DeserializeObject<List<DetalleReservaLineaResponse>>(content);

                    if (perfilResponse != null)
                    {
                        return perfilResponse;
                    }
                    else
                    {
                        throw new ArgumentNullException($"{perfilResponse}", "La respuesta del servidor está vacía.");
                    }
                }
                else if (response.StatusCode == (HttpStatusCode)209)
                {
                    return new List<DetalleReservaLineaResponse>();
                }
                else
                {
                    throw new HttpRequestException($"Error al realizar la solicitud HTTP. Código de estado: {response.StatusCode}");
                }
            }
        }

        public async Task<BaseResponse> LiberacionReserva(LiberacionReservaRequest liberacionReservaRequest)
        {
            try
            {
                var tokenAuthorization = await _commonService.GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeTransaccion, CanalWBC);
                if (tokenAuthorization.AccesoToken == null)
                {
                    throw new ArgumentNullException($"{tokenAuthorization.AccesoToken}", "La respuesta del servidor está vacía.");
                }

                var paramLiberacion = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicLiberacionLineasWBC);

                foreach (var parametro in paramLiberacion)
                {
                    if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret)
                    {
                        parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                    }
                }

                var urlApi = paramLiberacion
                .Where(param => param.DESCRIPCION == $"{UrlApic}")
                .Select(param => param.DESCRIPCIONCORTA)
                .FirstOrDefault();

                if (string.IsNullOrEmpty(urlApi))
                {
                    throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
                }

                using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramLiberacion, HeaderAuthorization, tokenAuthorization.AccesoToken))
                {

                    urlApi = urlApi.Replace("{reservationId}", liberacionReservaRequest.CodigoSolicitud.ToString());

                    var response = await client.DeleteAsync(urlApi);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseService = new BaseResponse
                        {
                            CodigoRespuesta = 32,
                            MensajeRespuesta = "Respuesta Correcta"
                        };

                        return responseService;
                    }
                    else
                    {
                        var responseService = new BaseResponse
                        {
                            CodigoRespuesta = 36,
                            MensajeRespuesta = "Respuesta Correcta"
                        };

                        return responseService;
                    }
                }
            }
            catch (Exception)
            {
                throw new HttpRequestException($"Error al realizar la solicitud HTTP");
            }
        }

    }
}
