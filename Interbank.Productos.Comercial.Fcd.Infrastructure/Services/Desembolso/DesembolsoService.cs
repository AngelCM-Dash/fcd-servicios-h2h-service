using Dapper;
using IBM.Data.Db2;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.DB2;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextSIT.Utility;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ApiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Desembolso
{
    [ExcludeFromCodeCoverage]
    public class DesembolsoService : IDesembolsoService
    {
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ICommonService _commonService;
        private readonly IEncrypterService _encrypter;
        private readonly ILogger<DesembolsoService> _logger;

        public DesembolsoService(IUtilitariosRepository utilitariosRepository, ITypeConvertionManager typeConvertionManager, ICommonService commonService, IEncrypterService encrypter, ILogger<DesembolsoService> logger)
        {
            ArgumentNullException.ThrowIfNull(utilitariosRepository);
            ArgumentNullException.ThrowIfNull(typeConvertionManager);
            ArgumentNullException.ThrowIfNull(commonService);
            ArgumentNullException.ThrowIfNull(encrypter);
            ArgumentNullException.ThrowIfNull(logger);

            _utilitariosRepository = utilitariosRepository;
            _typeConvertionManager = typeConvertionManager;
            _commonService = commonService;
            _encrypter = encrypter;
            _logger = logger;
        }

        public async Task<DesembolsoResponse> DesembolsarPlanilla(DesembolsoRequest desembolsoRequest)
        {
            var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioServiciosProcesoDesembolso);

            var solicitudUrl = parametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioUrlServiciosEncolamientoDesembolso.UrlApiProcesoAbono)?.DESCRIPCIONCORTA ?? "";//BALANCEADOR

            var response = await HttpClientHelper.EnviarSolicitudHttp<DesembolsoRequest, DesembolsoResponse>(solicitudUrl, desembolsoRequest);

            return response;
        }

        public async Task<InstruccionAbonoResponse> ProcesoInstruccionAbono(InstruccionAbonoRequest instruccionAbonoRequest)
        {
            try
            {
                var tokenAuthorization = await _commonService.GenerarTokenAuthorization((int)NumOrdenDominioApicTokenAuthorization.ScopeTransaccion, CanalFCD);
                if (tokenAuthorization.AccesoToken == null)
                {
                    throw new ArgumentNullException($"{tokenAuthorization.AccesoToken}", "La respuesta del servidor está vacía.");
                }

                var paramAbono = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioApicProcesoInstruccionAbonoFCD);

                foreach (var parametro in paramAbono)
                {
                    if (parametro.DESCRIPCION == SubscriptionKey || parametro.DESCRIPCION == SubscriptionSecret)
                    {
                        parametro.DESCRIPCIONCORTA = _encrypter.Decrypt(parametro.DESCRIPCIONCORTA ?? "");
                    }
                }

                var urlApi = paramAbono
                .Where(param => param.DESCRIPCION == $"{UrlApic}")
                .Select(param => param.DESCRIPCIONCORTA)
                .FirstOrDefault();

                if (string.IsNullOrEmpty(urlApi))
                {
                    throw new InvalidOperationException("No se encontró la URL API en los parámetros de consumo.");
                }

                using (var client = HttpClientHelper.PrepararHeaderHttpClient(paramAbono, HeaderAuthorization, tokenAuthorization.AccesoToken))
                {

                    var jsonSolicitud = JsonConvert.SerializeObject(instruccionAbonoRequest);
                    var response = await client.PostAsync(urlApi, new StringContent(jsonSolicitud, Encoding.UTF8, ContentTypeApplicationJson));

                    if (response.IsSuccessStatusCode)
                    {
                        return await ProcesarRespuestaExitosaProcesoInstruccionAbono(response);
                    }
                    else
                    {
                        return await ProcesarRespuestaErrorProcesoInstruccionAbono(response);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error al realizar la solicitud HTTP en el ProcesoInstruccionAbono: {ex.Message}");
            }
        }

        private static async Task<InstruccionAbonoResponse> ProcesarRespuestaExitosaProcesoInstruccionAbono(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            var bodyObject = string.IsNullOrWhiteSpace(responseBody) ? null : JsonConvert.DeserializeObject<InstruccionAbonoResponseBody>(responseBody);

            return new InstruccionAbonoResponse
            {
                BusResponseCode = response.Headers.GetValues($"{HeaderBusResponseCode}").FirstOrDefault(),
                BusResponseMessage = response.Headers.GetValues($"{HeaderBusResponseMessage}").FirstOrDefault(),
                SrvResponseCode = response.Headers.GetValues($"{HeaderSrvResponseCode}").FirstOrDefault(),
                SrvResponseMessage = response.Headers.GetValues($"{HeaderSrvResponseMessage}").FirstOrDefault(),
                registrationNumberProcessed = bodyObject?.RegistrationNumberProcessed ?? 0,
                returnedRegistrationNumber = bodyObject?.ReturnedRegistrationNumber ?? 0,
            };
        }

        private static async Task<InstruccionAbonoResponse> ProcesarRespuestaErrorProcesoInstruccionAbono(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                return new InstruccionAbonoResponse
                {
                    BusResponseCode = response.Headers.GetValues(HeaderBusResponseCode).FirstOrDefault(),
                    BusResponseMessage = response.Headers.GetValues(HeaderBusResponseMessage).FirstOrDefault(),
                    SrvResponseCode = response.Headers.GetValues(HeaderSrvResponseCode).FirstOrDefault(),
                    SrvResponseMessage = response.Headers.GetValues(HeaderSrvResponseMessage).FirstOrDefault()
                };
            }

            var errorObject = JsonConvert.DeserializeAnonymousType(content, new
            {
                httpCode = "",
                httpMessage = "",
                moreInformation = ""
            });

            if (errorObject == null)
                throw new InvalidOperationException("No se pudo deserializar el objeto de error.");

            return new InstruccionAbonoResponse
            {
                SrvResponseCode = errorObject.httpCode,
                SrvResponseMessage = errorObject.moreInformation
            };
        }

        public async Task<DesembolsoResponse> EncolamientoDesembolso(DesembolsoRequest desembolsoRequest)
        {
            var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioServiciosProcesoDesembolso);

            var solicitudUrl = parametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioUrlServiciosEncolamientoDesembolso.UrlApiEncolamiento)?.DESCRIPCIONCORTA ?? "";

            var response = await HttpClientHelper.EnviarSolicitudHttp<DesembolsoRequest, DesembolsoResponse>(solicitudUrl, desembolsoRequest);

            return response;
        }

        public string InsertDataToDatabase(DataTable dt, DapperParametroDesembolsoH2H dapperParametros)
        {
            _logger.LogInformation("Inicio: Desembolso Distribuido - Proceso de insercion en DB2 tabla PAGOMASIMAES");
            string connectionString = $"Server={dapperParametros.HostIPDb2}:{dapperParametros.PuertoDb2};Database={dapperParametros.NombreBaseDatosDb2};UID={dapperParametros.UsuarioDb2};PWD={dapperParametros.PassDb2}";

            try
            {
                using (var connection = new DB2Connection(connectionString))
                {
                    connection.Open();

                    using (var salesCopy = new DB2BulkCopy(connection, DB2BulkCopyOptions.TableLock | DB2BulkCopyOptions.Default))
                    {
                        salesCopy.DestinationTableName = $"{dapperParametros.AmbienteDb2}.{dapperParametros.TablaPagoMasiMaesDb2}";

                        salesCopy.WriteToServer(dt);
                    }

                    _logger.LogInformation("Fin: Desembolso Distribuido - Proceso de insercion en DB2 tabla PAGOMASIMAES");
                    return "Inserción/Modificación de datos de forma masiva en tabla A.FCD_TABL_MAES";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en metodo InsertDataToDatabase mientras se ejecutaba X operación");
                throw new InvalidOperationException("Error en metodo InsertDataToDatabase", ex);
            }

        }


        public string fEjecutarDesembolsoMasivoDB2(string pstrTotalRegistros, string pstrParametro, DapperParametroDesembolsoH2H dapperParametro)
        {
            string pstrHostPassword = dapperParametro.HostPassword ?? "";

            string objResultado = "";
            try
            {

                int intResultado = 0;
                string strCodError = string.Empty;
                string strSp = $"{dapperParametro.HostAmbiente}.{dapperParametro.HostNumeroJob}";
                string connectionString = $"Server={dapperParametro.HostIP}:{dapperParametro.PuertoDb2};Database={dapperParametro.HostDsn};UID={dapperParametro.HostUsuario};PWD={pstrHostPassword}";

                using (DB2Connection objCon = new DB2Connection(connectionString))
                {
                    objCon.Open();
                    using (DB2Transaction trans = objCon.BeginTransaction())
                    using (DB2Command cmd = objCon.CreateCommand())
                    {
                        string procCall = "CALL " + strSp + " (@PARM1, @PARM2, @PARM3)";//NOSONAR
                        cmd.Transaction = trans;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = procCall;//NOSONAR
                        cmd.CommandTimeout = 60;

                        cmd.Parameters.Add(new DB2Parameter("@PARM1", DB2Type.Char, 4) { Direction = ParameterDirection.Input, Value = pstrTotalRegistros.PadLeft(4, '0') });
                        cmd.Parameters.Add(new DB2Parameter("@PARM2", DB2Type.Char, 70) { Direction = ParameterDirection.Input, Value = pstrParametro });
                        cmd.Parameters.Add(new DB2Parameter("@PARM3", DB2Type.Integer, 4) { Direction = ParameterDirection.Output });

                        cmd.ExecuteNonQuery();


                        strCodError = cmd.Parameters["@PARM3"].Value.ToString() ?? "";
                        intResultado = strCodError.Equals("9") ? 1 : 0;
                        objResultado = $"{intResultado}|{strCodError}";

                        trans.Commit();
                    }
                }
                return objResultado;
            }
            catch (Exception)
            {
                return "0|Error en fEjecutarDesembolsoMasivoDB2. revise log";
                throw;
            }
        }

        public async Task<FcdTablMaes> ObtenerDatosDb2MaesPorPlanillaYSecuencia(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia)
        {
            try
            {
                var conexion = ObtenerParametrosDesembolso(dapperParametros);
                var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioDesembolsoDB2.DetallePlanillaTablaMaes)?.Script ?? "";
                string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2, numeroPlanilla, numeroSecuencia);//NOSONAR

                string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

                using (var connection = new DB2Connection(connectionString))
                {
                    await connection.OpenAsync();

                    var resultQuery = await connection.QueryAsync(sqlQuery);//NOSONAR

                    var planillasObservadas = resultQuery.Select(x => new FcdTablMaes
                    {
                        NumeroPlanilla = _typeConvertionManager.AnyToString(x.NU_PLANILLA_A),
                        NumeroSecuenciaPlanilla = _typeConvertionManager.AnyToInteger(x.NU_SEC_PLA_A),
                        CodigoUnico = _typeConvertionManager.AnyToString(x.CO_COD_UNIC_A),
                        FechaProceso = _typeConvertionManager.AnyToString(x.FE_PROC_A),
                        NumeroRegistro = _typeConvertionManager.AnyToInteger(x.NU_REGIST_A),
                        NumeroRegistroProceso = _typeConvertionManager.AnyToInteger(x.NU_REG_PROC_A),
                        HoraInicialProceso = _typeConvertionManager.AnyToString(x.HO_PROC_INI_A),
                        HoraFinalProceso = _typeConvertionManager.AnyToString(x.HO_PROC_FIN_A),
                        CodigoEstatusRetorno = _typeConvertionManager.AnyToString(x.CO_STAT_RETO_A),
                        CodigoEstatusProceso = _typeConvertionManager.AnyToString(x.CO_STAT_PROC_A),
                        DescripcionMensaje = _typeConvertionManager.AnyToString(x.DE_DES_MSG_ERR_A),
                    }).FirstOrDefault();

                    if (planillasObservadas == null)
                    {
                        throw new InvalidOperationException("No se encontró ninguna planilla con esos parámetros.");
                    }

                    return planillasObservadas;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("Error en base de datos", ex);
            }
        }

        public async Task<BaseResponse> ProcesarTramas(ProcesarTramasRequest procesarTramasRequest)
        {
            var parametros = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioDesembolsoH2H);

            var solicitudUrl = parametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.UrlProcesamientoTramas)?.DESCRIPCIONCORTA ?? "";

            var response = await HttpClientHelper.EnviarSolicitudHttp<ProcesarTramasRequest, BaseResponse>(solicitudUrl, procesarTramasRequest);

            return response;

        }

        public async Task<List<FcdPagoMasiMae>> ObtenerDatosDb2PagoMasiMaesPorPlanillaYSecuencia(List<DapperParametroDB2> dapperParametrosDB2, List<DapperParametro> dapperParametros, string numeroPlanilla, int numeroSecuencia)
        {
            try
            {
                _logger.LogInformation("Inicio de ObtenerDatosDb2PagoMasiMaesPorPlanillaYSecuencia. Planilla: {NumeroPlanilla}, Secuencia: {NumeroSecuencia}", numeroPlanilla, numeroSecuencia);
                var conexion = ObtenerParametrosDesembolso(dapperParametros);

                var DB2Queries = dapperParametrosDB2.Find(x => x.NumOrden == (int)NumOrdenDominioDesembolsoDB2.DetallePlanillaTablaPagoMasiMaes)?.Script ?? "";
                string sqlQuery = string.Format(DB2Queries, conexion.AmbienteDb2, numeroPlanilla, numeroSecuencia);//NOSONAR

                string connectionString = $"Server={conexion.HostIPDb2}:{conexion.PuertoDb2};Database={conexion.NombreBaseDatosDb2};UID={conexion.UsuarioDb2};PWD={conexion.PassDb2}";

                _logger.LogInformation("Cadena de conexión construida y consulta SQL generada correctamente. Intentando abrir conexión y ejecutar consulta.");
                using (var connection = new DB2Connection(connectionString))
                {
                    await connection.OpenAsync();
                    var resultQuery = await connection.QueryAsync(sqlQuery);//NOSONAR
                    var planillasObservadas = resultQuery.Select(x => new FcdPagoMasiMae
                    {
                        CodigoRetorno = _typeConvertionManager.AnyToString(x.CO_COD_RET),
                        CodigoTransaccion = _typeConvertionManager.AnyToString(x.CO_COD_TRAN),
                        CodigoPrograma = _typeConvertionManager.AnyToString(x.CO_COD_PROG),
                        CodigoUsuario = _typeConvertionManager.AnyToString(x.CO_COD_USUA),
                        NumeroOperacion = _typeConvertionManager.AnyToString(x.NU_OPERA),
                        NumeroSecuenciaOperacion = _typeConvertionManager.AnyToInteger(x.NU_SEC_OPER),
                        FechaProceso = _typeConvertionManager.AnyToString(x.FE_PROC),
                        NumeroPlanilla = _typeConvertionManager.AnyToString(x.NU_PLANILLA),
                        NumeroSecuenciaPlanilla = _typeConvertionManager.AnyToInteger(x.NU_SEC_PLA),
                        CodigoUnico = _typeConvertionManager.AnyToString(x.CO_COD_UNIC),
                        CodigoRegistroEmpleado = _typeConvertionManager.AnyToString(x.CO_REG_EMP),
                        CodigoTiendaOrigen = _typeConvertionManager.AnyToString(x.CO_TDA_ORIG),
                        //ABONO
                        CodigoTipoCuentaAbono = _typeConvertionManager.AnyToString(x.CO_TIP_CTA_CR),
                        CodigoBancoAbono = _typeConvertionManager.AnyToString(x.CO_CTRL1_CR),
                        CodigoMonedaAbono = _typeConvertionManager.AnyToString(x.CO_CTRL2_CR),
                        CodigoTiendaAbono = _typeConvertionManager.AnyToString(x.CO_CTRL3_CR),
                        CodigoCategoriaAbono = _typeConvertionManager.AnyToString(x.CO_CTRL4_CR),
                        NumeroCuentaAbono = _typeConvertionManager.AnyToString(x.NU_ACCT_CR),
                        //CARGO
                        CodigoTipoCuentaCargo = _typeConvertionManager.AnyToString(x.CO_TIP_CTA_DB),
                        CodigoBancoCargo = _typeConvertionManager.AnyToString(x.CO_CTRL1_DB),
                        CodigoMonedaCargo = _typeConvertionManager.AnyToString(x.CO_CTRL2_DB),
                        CodigoTiendaCargo = _typeConvertionManager.AnyToString(x.CO_CTRL3_DB),
                        CodigoCategoriaCargo = _typeConvertionManager.AnyToString(x.CO_CTRL4_DB),
                        NumeroCuentaCargo = _typeConvertionManager.AnyToString(x.NU_ACCT_DB),
                        //GENERICO
                        FlagExtorno = _typeConvertionManager.AnyToString(x.CO_EXTOR),
                        DescripcionCorta = _typeConvertionManager.AnyToString(x.DE_SHORT_DESC),
                        ImporteAbono = _typeConvertionManager.AnyToDecimal(x.IM_MONT_CR),
                        ImporteCargo = _typeConvertionManager.AnyToDecimal(x.IM_MONT_DB),
                        FlagCobroForzoso = _typeConvertionManager.AnyToString(x.CO_COBR_FORZ),
                        FlagCobroParcial = _typeConvertionManager.AnyToString(x.CO_COBR_PARC),
                        FlagTipoCambio = _typeConvertionManager.AnyToString(x.CO_FLAG_OCC),
                        CodigoMoneda = _typeConvertionManager.AnyToString(x.CO_CTA_MON_CF),
                        CodigoClaseTipoCambio = _typeConvertionManager.AnyToString(x.CO_CTA_CLA_TC),
                        ImporteTipoCambio = _typeConvertionManager.AnyToDecimal(x.IM_CTA_TC_CF),
                        ImporteEquivalente = _typeConvertionManager.AnyToDecimal(x.IM_CTA_IMP_EQUIV),
                        NumeroLogExtorno = _typeConvertionManager.AnyToInteger(x.NU_LOG_EXT),
                        FlagGlosaProducto = _typeConvertionManager.AnyToString(x.CO_FLAG_PROC),
                        GlosaProducto = _typeConvertionManager.AnyToString(x.DE_DESC_DESEM),
                        CodigoBancoCci = _typeConvertionManager.AnyToString(x.CO_CCI_CTL1),
                        //CUENTA CCI
                        CodigoMonedaCci = _typeConvertionManager.AnyToString(x.CO_CCI_CTL2),
                        CodigoTiendaCci = _typeConvertionManager.AnyToString(x.CO_CCI_CTL3),
                        CodigoCategoriaCci = _typeConvertionManager.AnyToString(x.CO_CCI_CTL4),
                        NumeroCuentaOrdenanteCci = _typeConvertionManager.AnyToString(x.CO_CCI_CTA_ORDE),
                        CodigoUnicoOrdenanteCci = _typeConvertionManager.AnyToString(x.CO_COD_UNI_ORDE),
                        NombreOrdenanteCci = _typeConvertionManager.AnyToString(x.DE_NOMB_ORDE),
                        CodigoUnicoBeneficiarioCci = _typeConvertionManager.AnyToString(x.CO_COD_UNI_BENE),
                        CodigoMonedaCciBcr = _typeConvertionManager.AnyToString(x.CO_CCI_MONE),
                        ImporteCci = _typeConvertionManager.AnyToDecimal(x.IM_IMP_CCI),
                        NumeroCuentaProveedorCci = _typeConvertionManager.AnyToString(x.NU_CTA_CCI),
                        NombreBeneficiarioCci = _typeConvertionManager.AnyToString(x.DE_NOMB_BENE),
                        FlagNeteo = _typeConvertionManager.AnyToString(x.CO_FLAG_NETEA),
                        TipodocumentoAceptanteBcr = _typeConvertionManager.AnyToString(x.CO_BCR_TIP_DOC_A),
                        NumeroDocumentoAceptanteBcr = _typeConvertionManager.AnyToString(x.CO_BCR_NRO_DOC_A),
                        TipodocumentoGiradorBcr = _typeConvertionManager.AnyToString(x.CO_BCR_TIP_DOC_G),
                        NumeroDocumentoGiradorBcr = _typeConvertionManager.AnyToString(x.CO_BCR_NRO_DOC_G),
                        //AGREGADOS
                        CodigoEstatusPago = _typeConvertionManager.AnyToString(x.CO_STAT_PAGO),
                        CodigoEstatusRetorno = _typeConvertionManager.AnyToString(x.CO_STAT_RETO),
                        DescripcionMensaje = _typeConvertionManager.AnyToString(x.DE_DES_MSG_ERR),
                        NumeroDocumento = _typeConvertionManager.AnyToString(x.NU_NRO_DOCUM),
                        ImporteDesembolsado = _typeConvertionManager.AnyToDecimal(x.IM_IMP_DESEM),
                        FlagExtornoRetorno = _typeConvertionManager.AnyToString(x.CO_FLAG_EXT_RE),
                        NumeroLogExtornoRetorno = _typeConvertionManager.AnyToInteger(x.NU_LOG_EXT_RE),
                        ImporteComisionCci = _typeConvertionManager.AnyToDecimal(x.IM_IMP_COM_CCI),
                        ImporteComisionIb = _typeConvertionManager.AnyToDecimal(x.IM_IMP_COM_IB),
                        ImporteDesembolsadoCci = _typeConvertionManager.AnyToDecimal(x.IM_IMP_DES_CCI),
                        HoraFinalProceso = _typeConvertionManager.AnyToString(x.HO_PROC_FIN),
                        NumeroInstruccion = _typeConvertionManager.AnyToString(x.NU_INSTRUC),
                        CodigoCliente = _typeConvertionManager.AnyToString(x.CO_CLIENTE),
                        CodigoTipoProceso = _typeConvertionManager.AnyToString(x.CO_TIPO_PROCESO),
                        CodigoEnvioCorreo = _typeConvertionManager.AnyToString(x.CO_ENVIO_CORREO),
                    }).ToList();

                    if (planillasObservadas == null)
                    {
                        _logger.LogError("No se encontraron planillas con los parámetros dados.");
                        throw new InvalidOperationException("No se encontró ninguna planilla con esos parámetros.");
                    }
                    _logger.LogInformation("Se obtuvieron un total de: {Count} planillas.", planillasObservadas.Count);
                    return planillasObservadas;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en metodo ObtenerDatosDb2PagoMasiMaesPorPlanillaYSecuencia con Planilla: {NumeroPlanilla}, Secuencia: {NumeroSecuencia}", numeroPlanilla, numeroSecuencia);
                throw new ArgumentNullException($"Error en base de datos {ex.Message}", ex);
            }
        }

        public DapperParametroDesembolsoH2H ObtenerParametrosDesembolso(List<DapperParametro> dapperParametros)
        {
            return new DapperParametroDesembolsoH2H
            {
                FlagDesembolso = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagDesembolso)?.DESCRIPCIONCORTA ?? "",
                HostFileServer = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrFileServerFcd)?.DESCRIPCIONCORTA ?? "",
                HostCodigoRes = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostCodigoRes)?.DESCRIPCIONCORTA ?? "",
                HostSolicitud = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostSolicitud)?.DESCRIPCIONCORTA ?? "",
                HostFile = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostFile)?.DESCRIPCIONCORTA ?? "",
                HostIP = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostIP)?.DESCRIPCIONCORTA ?? "",
                HostUsuario = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostUsuario)?.DESCRIPCIONCORTA ?? "",
                HostPassword = _encrypter.Decrypt(dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostPassword)?.DESCRIPCIONCORTA ?? ""),
                HostAmbiente = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostAmbiente)?.DESCRIPCIONCORTA ?? "",
                HostDsn = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostDsn)?.DESCRIPCIONCORTA ?? "",
                HostNumeroJob = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PstrHostNumeroJob)?.DESCRIPCIONCORTA ?? "",
                MontoNetoNegaPos = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.MontoNetoNegaPos)?.DESCRIPCIONCORTA ?? "",
                HostIPDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.IpDb2)?.DESCRIPCIONCORTA ?? "",
                PuertoDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PuertoDb2)?.DESCRIPCIONCORTA ?? "",
                NombreBaseDatosDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.NombreBaseDatosDb2)?.DESCRIPCIONCORTA ?? "",
                UsuarioDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.UsuarioDb2)?.DESCRIPCIONCORTA ?? "",
                PassDb2 = _encrypter.Decrypt(dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.PassDb2)?.DESCRIPCIONCORTA ?? ""),
                AmbienteDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.AmbienteDb2)?.DESCRIPCIONCORTA ?? "",
                TablaPagoMasiMaesDb2 = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.TablaPagoMasiMaesDb2)?.DESCRIPCIONCORTA ?? "",
                FlagPruebaDesembolso = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagPruebaDesembolso)?.DESCRIPCIONCORTA ?? "",
                FlagPruebaMonitor = dapperParametros.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioDesembolsoDistribuido.FlagPruebaMonitor)?.DESCRIPCIONCORTA ?? "",
            };
        }
    }
}


