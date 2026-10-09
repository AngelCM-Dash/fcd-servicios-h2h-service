using Dapper;
using IBM.Data.Db2;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Common;
using Newtonsoft.Json;
using NextSIT.Utility;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ApiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Desembolso
{
    [ExcludeFromCodeCoverage]
    public class MonitorService : IMonitorService
    {
        private readonly IEncrypterService _encrypter;
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ICommonService _commonService;
        private readonly IUtilitariosRepository _utilitariosRepository;


        public MonitorService(IEncrypterService encrypter, ITypeConvertionManager typeConvertionManager, ICommonService commonService, IUtilitariosRepository utilitariosRepository)
        {
            ArgumentNullException.ThrowIfNull(encrypter);
            ArgumentNullException.ThrowIfNull(typeConvertionManager);
            ArgumentNullException.ThrowIfNull(commonService);
            ArgumentNullException.ThrowIfNull(utilitariosRepository);

            _encrypter = encrypter;
            _typeConvertionManager = typeConvertionManager;
            _commonService = commonService;
            _utilitariosRepository = utilitariosRepository;
        }

        public async Task<List<DapperPlanillaCabeceraDb2>> ObtenerPlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros)
        {
            try
            {
                var conexion = ObtenerParametrosMonitor(dapperParametros);

                var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioMonitorDB2.PlanillasProcesadasDB2)?.Script ?? "";
                string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2);//NOSONAR

                string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

                using (var connection = new DB2Connection(connectionString))
                {
                    await connection.OpenAsync();

                    var results = await connection.QueryAsync(sqlQuery);//NOSONAR

                    var planillasObservadas = results.Select(x => new DapperPlanillaCabeceraDb2
                    {
                        numeroPlanilla = _typeConvertionManager.AnyToString(x.NU_PLANILLA_A),
                        numeroSecuenciaPlanilla = _typeConvertionManager.AnyToInteger(x.NU_SEC_PLA_A),
                        fechaProcesoPlanilla = _typeConvertionManager.AnyToString(x.FE_PROC_A),
                    }).ToList();

                    return planillasObservadas;
                }
            }
            catch (Exception ex)
            {
                throw new ThrowException(ex.Message);
            }
        }

        public async Task<List<DetallePlanillasProcesadasResponse>> ObtenerDetallePlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, string numeroSecuencia)
        {
            var conexion = ObtenerParametrosMonitor(dapperParametros);

            var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioMonitorDB2.DetallePlanillasProcesadasDB2)?.Script ?? "";
            string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2, numeroPlanilla, numeroSecuencia);//NOSONAR

            string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();

                var results = await connection.QueryAsync(sqlQuery);//NOSONAR

                var planillasObservadas = results.Select(x => new DetallePlanillasProcesadasResponse
                {
                    NumeroPlanilla = _typeConvertionManager.AnyToString(x.NU_PLANILLA),
                    NumeroSecuenciaPlanilla = _typeConvertionManager.AnyToInteger(x.NU_SEC_PLA),
                    CodigoUnico = _typeConvertionManager.AnyToString(x.CO_COD_UNIC),
                    NumeroInstruccion = _typeConvertionManager.AnyToString(x.NU_INSTRUC),
                    NumeroOperacion = _typeConvertionManager.AnyToString(x.NU_OPERA),
                    NumeroSecuenciaOperacion = _typeConvertionManager.AnyToString(x.NU_SEC_OPER),
                    CodigoRetorno = _typeConvertionManager.AnyToString(x.CO_COD_RET),
                    DescripcionMensajeError = _typeConvertionManager.AnyToString(x.DE_DES_MSG_ERR_LOG),
                    NumeroDocumento = _typeConvertionManager.AnyToString(x.NU_NRO_DOCUM),
                    ImporteDesembolso = _typeConvertionManager.AnyToString(x.IM_IMP_DESEM),
                    CodigoFlagExterno = _typeConvertionManager.AnyToString(x.CO_FLAG_EXT_RE),
                    NumeroLogExterno = _typeConvertionManager.AnyToString(x.NU_LOG_EXT),
                    ImporteComisionCCI = _typeConvertionManager.AnyToString(x.IM_IMP_COM_CCI),
                    ImporteComisionIB = _typeConvertionManager.AnyToString(x.IM_IMP_COM_IB),
                    ImporteDesembolsoCCI = _typeConvertionManager.AnyToString(x.IM_IMP_DES_CCI),
                    CodigoEstadoPago = _typeConvertionManager.AnyToString(x.CO_STAT_PAGO),
                    CodigoEnvioCorreo = _typeConvertionManager.AnyToString(x.CO_ENVIO_CORREO),
                }).ToList();

                return planillasObservadas;
            }
        }

        public async Task<string> ActualizarPlanillasProcesadasDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, string numeroSecuencia, int estadoPlanilla)
        {
            var conexion = ObtenerParametrosMonitor(dapperParametros);

            var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioMonitorDB2.ActualizacionPlanillasProcesadasDB2)?.Script ?? "";
            string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2, estadoPlanilla, numeroPlanilla, numeroSecuencia);//NOSONAR

            string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();

                var results = await connection.ExecuteAsync(sqlQuery);//NOSONAR

                var respuesta = results.ToString();

                if (respuesta == "1")
                {
                    return respuesta + "|Se Actualizó los datos en DBD2";
                }
                else
                {
                    return respuesta;
                }
            }
        }

        public async Task<MovimientoResponse> ConsumoLineaMovimiento(MovimientoRequest movimientoRequest)
        {
            var tokenAuthorization = await _commonService.GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeTransaccion, CanalWBC);
            if (tokenAuthorization.AccesoToken == null)
            {
                throw new ArgumentNullException($"{tokenAuthorization.AccesoToken}", "La respuesta del servidor está vacía.");
            }

            var paramConsumo = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicConsumoLineasWBC);

            foreach (var parametro in paramConsumo)
            {
                if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret)
                {
                    parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                }
            }

            var urlApi = paramConsumo
            .Where(param => param.DESCRIPCION == $"{UrlApic}")
            .Select(param => param.DESCRIPCIONCORTA)
            .FirstOrDefault();

            if (string.IsNullOrEmpty(urlApi))
            {
                throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
            }

            using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramConsumo, HeaderAuthorization, tokenAuthorization.AccesoToken))
            {
                var jsonSolicitud = JsonConvert.SerializeObject(movimientoRequest);
                var response = await client.PostAsync(urlApi, new StringContent(jsonSolicitud, Encoding.UTF8, ContentTypeApplicationJson));

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        throw new InvalidOperationException("La respuesta del servidor está vacía o no se pudo deserializar correctamente.");
                    }

                    var responseObject = JsonConvert.DeserializeObject<MovimientoResponse>(responseBody);
                    if (responseObject == null)
                    {
                        throw new InvalidOperationException("El objeto de respuesta es nulo.");
                    }

                    responseObject.CodigoRespuesta = response.Headers.GetValues($"{HeaderSrvResponseCode}").FirstOrDefault() ?? string.Empty;
                    responseObject.MensajeRespuesta = response.Headers.GetValues($"{HeaderSrvResponseMessage}").FirstOrDefault() ?? string.Empty;

                    return responseObject;
                }
                else
                {
                    throw new HttpRequestException($"Error al obtener la respuesta del servidor. Código de estado: {response.StatusCode}");
                }
            }
        }

        public async Task<string> ActualizarPlanillasMasiMaesDB2(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia, string estadoRetorno, string estadoProceso)
        {
            var conexion = ObtenerParametrosMonitor(dapperParametros);

            var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioDesembolsoDB2.ActualizacionEstadosTablaMaes)?.Script ?? "";
            string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2, estadoProceso, estadoRetorno, numeroPlanilla, numeroSecuencia);//NOSONAR

            string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();

                var results = await connection.ExecuteAsync(sqlQuery);//NOSONAR

                var respuesta = results.ToString();

                if (respuesta == "1")
                {
                    return respuesta + "|Se Actualizó los datos en DBD2";
                }
                else
                {
                    return respuesta;
                }
            }
        }

        public async Task<BaseResponse> EnvioCorreoDietarios(string numeroPlanilla, string numeroInstruccion)
        {
            try
            {
                var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioDesembolsoH2H);

                var url = parametros.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioDesembolsoDistribuido.UrlWsFcdExtranet)?.DESCRIPCIONCORTA;
                var action = parametros.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioDesembolsoDistribuido.ActionEnvioCorreoDietarios)?.DESCRIPCIONCORTA;

                XmlDocument soapEnvelopeDocument = new XmlDocument();
                soapEnvelopeDocument.LoadXml(
                "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
                               "xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\" " +
                               "xmlns:xsd=\"http://www.w3.org/1999/XMLSchema\">" +
                    "<soap:Body>" +
                        "<fEnviarCorreo_Electronicos xmlns=\"http://tempuri.org/\">" +
                          "<numeroPlanilla>" + numeroPlanilla + "</numeroPlanilla>" +
                          "<numeroInstruccion>" + numeroInstruccion + "</numeroInstruccion>" +
                        "</fEnviarCorreo_Electronicos>" +
                    "</soap:Body>" +
                "</soap:Envelope>");

                using var httpClient = new HttpClient();

                string xmlString = soapEnvelopeDocument.OuterXml;
                var content = new StringContent(xmlString, Encoding.UTF8, "text/xml");
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                content.Headers.Add("SOAPAction", action);

                var response = await httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseContent);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("soap", SoapNamespace);
                nsmgr.AddNamespace("tempuri", TempuriNamespace);


                XmlNode? resultNode = xmlDoc.SelectSingleNode("//tempuri:fEnviarCorreo_ElectronicosResult", nsmgr);

                if (resultNode != null)
                {
                    if (bool.TryParse(resultNode.InnerText, out bool exito) && exito)
                    {
                        return new BaseResponse
                        {
                            CodigoRespuesta = 32,
                            MensajeRespuesta = resultNode.InnerText.ToString()
                        };
                    }
                    else
                    {
                        return new BaseResponse
                        {
                            CodigoRespuesta = 36,
                            MensajeRespuesta = resultNode.InnerText.ToString()
                        };
                    }
                }
                else
                {
                    return new BaseResponse
                    {
                        CodigoRespuesta = 36,
                        MensajeRespuesta = "Respuesta SOAP vacía o nodo no encontrado"
                    };
                }
            }
            catch (Exception)
            {
                throw new HttpRequestException($"Error al realizar la solicitud HTTP EnvioCorreoDietarios");
            }
        }

        public DapperParametroDesembolsoH2H ObtenerParametrosMonitor(List<DapperParametro> dapperParametros)
        {
            return new DapperParametroDesembolsoH2H
            {
                HostIPDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.IpDb2)?.DESCRIPCIONCORTA ?? "",
                PuertoDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PuertoDb2)?.DESCRIPCIONCORTA ?? "",
                NombreBaseDatosDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.NombreBaseDatosDb2)?.DESCRIPCIONCORTA ?? "",
                UsuarioDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.UsuarioDb2)?.DESCRIPCIONCORTA ?? "",
                PassDb2 = _encrypter.Decrypt(dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PassDb2)?.DESCRIPCIONCORTA ?? ""),
                AmbienteDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.AmbienteDb2)?.DESCRIPCIONCORTA ?? ""
            };
        }
    }
}
