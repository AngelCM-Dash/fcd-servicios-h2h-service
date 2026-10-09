using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Application.Services.Dependencias;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva
{
    [ExcludeFromCodeCoverage]
    public class CreatePlanillasCommandHandler : HandlerConstructorDependencies<CreatePlanillasCommandHandler>, IRequestHandler<CreatePlanillasCommand, PlanillaResponse>
    {
        private readonly ITrazaService _trazaService;
        private readonly IEncolamientoPlanillaService _encolamientoPlanillaService;
        private readonly INotificacionService _notificacionService;
        private readonly ISftpService _sftpService;
        private string privateKeyLocalFilePath = "";
        private string numeroPlanillaLog = "";
        decimal CodigoDetalleTraza = 0;
        public CreatePlanillasCommandHandler(IServiceProvider serviceProvider, IEncolamientoPlanillaService encolamientoPlanillaService, INotificacionService notificacionService, ISftpService sftpService, ITrazaService trazaService) : base(serviceProvider)
        {
            _trazaService = trazaService;
            _encolamientoPlanillaService = encolamientoPlanillaService;
            _notificacionService = notificacionService;
            _sftpService = sftpService;
        }

        public async Task<PlanillaResponse> Handle(CreatePlanillasCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tipoEjecucion = ObtenerTipoEjecucion(request);

                CodigoDetalleTraza = await _trazaService.RegistrarCabeceraTraza(Convert.ToDecimal(0), request.NombreArchivo, "", tipoEjecucion);

                await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaCargaI, "Inicio Proceso de Carga Masiva", 1);

                await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaObtConfigSFTP_I, "Obtiene la Configuracion del SFTP", 2);
                var datosConfig = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(DominioConfiguracionSFTPCargaMasiva);
                await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaObtConfigSFTP_F, "Obtuvo correctamente la Configuracion del SFTP", 2);

                var flagPrueba = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.FlagPruebasCargaMasiva)?.DESCRIPCIONCORTA);

                if (flagPrueba != 0)
                {
                    throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
                }

                DapperPlanillaCompleta PlanillaC;

                if (datosConfig.Count == 0)
                {
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaObtConfigSFTP_E, "Error al obtener la configuracion del SFTP", 2);
                    throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
                }

                privateKeyLocalFilePath = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.RutaArchivoPpk)?.DESCRIPCIONCORTA ?? string.Empty;

                SftpConnectionRequest sftpConnectionRequest = new SftpConnectionRequest()
                {
                    Host = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.ServerSftp)?.DESCRIPCIONCORTA ?? string.Empty,
                    Port = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.PuertoSftp)?.DESCRIPCIONCORTA),
                    Username = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.UserSftp)?.DESCRIPCIONCORTA ?? string.Empty,
                    Password = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.PassSftp)?.DESCRIPCIONCORTA ?? string.Empty,
                    RemotePath = request.RutaArchivo,
                    FileName = request.NombreArchivo,
                    ProjectDirectory = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.RutaTemps)?.DESCRIPCIONCORTA ?? string.Empty,
                    PrivateKeyLocalFilePath = privateKeyLocalFilePath,
                    FlagAccesoPPk = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.FlagCRed)?.DESCRIPCIONCORTA),
                    CodigoCab = CodigoDetalleTraza,
                    Metodo = GetType().Name
                };

                if (request.Adicional != null && request.Adicional.Any(a => string.Equals(a.Campo, "H2HNAS", StringComparison.OrdinalIgnoreCase)))
                {
                    PlanillaC = await ObtenerDatosPlanillaNAS(sftpConnectionRequest, request.RutaArchivo ?? string.Empty, request.NombreArchivo ?? string.Empty);
                }
                else
                {
                    PlanillaC = await ObtenerDatosPlanillaSFTP(sftpConnectionRequest);
                }

                PlanillaC.CodigoProducto = request.CodigoProducto;
                PlanillaC.Usuario = request.Usuario;
                PlanillaC.CodigoPerfilUsuario = request.CodigoPerfilUsuario;
                PlanillaC.CodigoTienda = request.CodigoTienda;
                PlanillaC.Tienda = request.Tienda;
                PlanillaC.ContratoMarco = request.ContratoMarco;
                PlanillaC.CodigoUnico = request.CodigoUnico;
                PlanillaC.CanalAtencion = request.CanalAtencion;
                PlanillaC.Observacion = string.Concat("Creado Por Carga Masiva ", request.CanalAtencion);
                if (request.FechaValor != null)
                {
                    PlanillaC.FechaDesembolso = request.FechaValor;
                }
                PlanillaC.CodigoFormaOperacion = await _planillaRepository.ObtenerCodigoFormacionFCD_H2H(PlanillaC);
                var planillaEntity = _mapper.Map<DapperPlanillaCompleta>(PlanillaC);
                planillaEntity.NombreArchivo = request.RutaArchivo + request.NombreArchivo;
                string Duplicidad = (await _planillaRepository.VerificarDuplicidadPlanillaFCD_H2H(planillaEntity)).CodigoArchivo ?? "";
                if (Duplicidad == "")
                {
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaCargaPlaI, "Inicia registro de cabecera tabla Planilla", 4);
                    PlanillaC = await _planillaRepository.RegistrarPlanillaFCD_H2H(planillaEntity);
                    await _planillaRepository.ActualizarObservacionPlanilla(PlanillaC.NumeroPlanilla ?? "", PlanillaC.Observacion ?? "");

                    await _trazaService.RegistrarCabeceraTraza(CodigoDetalleTraza, request.NombreArchivo, PlanillaC.NumeroPlanilla, 2);

                    if (!string.IsNullOrEmpty(PlanillaC.NumeroPlanilla))
                    {
                        numeroPlanillaLog = PlanillaC.NumeroPlanilla;
                    }
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaCargaPlaF, "Registro Correctamente la Cabecera", 4);

                    var sqlLdrcn = datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.SqlLdrCn)?.DESCRIPCIONCORTA ?? string.Empty;
                    var flagCargaDocumentos = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCargaMasiva.FlagCargaDocumentos)?.DESCRIPCIONCORTA ?? string.Empty);

                    await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaEncolamientoPlanillaI, "Inicia Proceso de Encolamiento de Planilla", 5);
                    await _encolamientoPlanillaService.EnqueueAsync(PlanillaC, request.NombreArchivo ?? "", sqlLdrcn, CodigoDetalleTraza, flagCargaDocumentos);
                }
                else
                {
                    await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaErrorCargaPlanilla, "Error : Solo se puede enviar la misma planilla en un solo proceso activo", 4);
                    _logger.LogError("Error : Solo se puede enviar la misma planilla en un solo proceso activo");
                    throw new ThrowException("36", "Solo se puede enviar la misma planilla en un solo proceso activo, verificar");
                }

                return new PlanillaResponse
                {
                    NumeroPlanilla = PlanillaC.NumeroPlanilla,
                    CodigoRespuesta = "32",
                    MensajeRespuesta = "OK"
                };
            }
            catch (Exception ex)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (!string.IsNullOrEmpty(numeroPlanillaLog))
                {
                    await _planillaRepository.RechazoPlanilla(numeroPlanillaLog);
                }

                await _trazaService.RegistrarDetalleTraza(CodigoDetalleTraza, GetType().Name, TrazaConstante.TrazaCargaE, $"Error Proceso de Carga Masiva: {ex.ToString()}", 0);
                var requestNotificacion = new NotificacionAssiRequest(CargaMasivaPlanilla, $"{request.NombreArchivo}|Ocurrió un Error: {ex.Message}", "36", numeroPlanillaLog, null, null, null);
                await _notificacionService.NotificacionAssi(requestNotificacion, CodigoDetalleTraza);
                _logger.LogError(ex, "Se realizo la Notificacion - ProcesoCargaMasiva Flujo Erroneo para la planilla {Planilla} a las {Timestamp}", numeroPlanillaLog, timestamp);
                throw new ThrowException("36", ex.Message);
            }
        }

        private static int ObtenerTipoEjecucion(CreatePlanillasCommand request)
        {
            if (request.Adicional == null)
                return 1;

            return request.Adicional.Any(a =>
                string.Equals(a.Campo, "H2HNAS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(a.Campo, "H2HCORE", StringComparison.OrdinalIgnoreCase))
                ? 3
                : 1;
        }
        private async Task<DapperPlanillaCompleta> ObtenerDatosPlanillaNAS(SftpConnectionRequest sftp, string rutaArchivo, string nombreArchivo)
        {
            await _trazaService.RegistrarDetalleTraza(sftp.CodigoCab, sftp.Metodo, TrazaConstante.TrazaDatosPlanillaNAS_I, "", 3);
            string? pathCtlCargaMasiva = _configuration["pathCtlCargaMasiva"] ?? "";
            string[] Contenido;
            char padleft = '0';
            var rutaArchivoTempCompleta = Path.Combine(pathCtlCargaMasiva, nombreArchivo);
            try
            {

                string fullPath = Path.Combine(rutaArchivo ?? string.Empty, nombreArchivo ?? string.Empty);
                if (!File.Exists(fullPath))
                {
                    throw new ThrowException("36", "No existe el archivo en la ruta");
                }

                Contenido = await File.ReadAllLinesAsync(fullPath);
                File.Copy(fullPath, rutaArchivoTempCompleta, true);

                DapperPlanillaCompleta planilla = ConstruirPlanillaDesdeContenido(Contenido, padleft);

                await _trazaService.RegistrarDetalleTraza(
                    sftp.CodigoCab,
                    sftp.Metodo,
                    TrazaConstante.TrazaDatosPlanillaNAS_F,
                    "",
                    3);

                return planilla;

            }
            catch (Exception ex)
            {
                await _trazaService.RegistrarDetalleTraza(sftp.CodigoCab, sftp.Metodo, TrazaConstante.TrazaDatosPlanillaNAS_E, "", 3);
                _logger.LogError(ex, "{Traza} - Error: {MensajeError}", TrazaConstante.TrazaDatosPlanillaSFTP_E, ex.Message);
                throw new ThrowException("36", "Ocurrió un error inesperado al leer el archivo NAS (" + ex.Message + ")");
            }
        }

        private async Task<DapperPlanillaCompleta> ObtenerDatosPlanillaSFTP(SftpConnectionRequest sftp)
        {
            await _trazaService.RegistrarDetalleTraza(sftp.CodigoCab, sftp.Metodo, TrazaConstante.TrazaDatosPlanillaSFTP_I, "Obtiene archivo del SFTP", 3);

            string? pathCtlCargaMasiva = _configuration["pathCtlCargaMasiva"] ?? "";
            string[] Contenido;
            char padleft = '0';
            bool usePrivateKey = sftp.FlagAccesoPPk != 0;

            DateTime fechaCreacion = DateTime.Now;

            string año = fechaCreacion.ToString("yyyy");
            string mes = fechaCreacion.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));
            string dia = fechaCreacion.ToString("dd");

            pathCtlCargaMasiva = $"{pathCtlCargaMasiva}{año}\\{mes}\\{dia}\\";

            try
            {
                privateKeyLocalFilePath = !usePrivateKey ? _encrypter.Decrypt(sftp.Password ?? "") : privateKeyLocalFilePath;
                await _sftpService.Conectar(sftp.Host ?? string.Empty, sftp.Port, sftp.Username ?? string.Empty, sftp.Password ?? string.Empty, privateKeyLocalFilePath, usePrivateKey);
                await _sftpService.ValidarRutaDirectorioRemotaSftp(sftp.RemotePath ?? string.Empty);
                await _sftpService.ValidarArchivoExistenteSftp(sftp.RemotePath + sftp.FileName);
                Contenido = await _sftpService.LeerLineasArchivoSftp(sftp.RemotePath + sftp.FileName);
                await _sftpService.DescargarArchivoSftp(sftp.RemotePath + sftp.FileName, pathCtlCargaMasiva + sftp.FileName);
                await _sftpService.Desconectar();

                DapperPlanillaCompleta planilla = ConstruirPlanillaDesdeContenido(Contenido, padleft);

                // Solo SFTP necesita este dato extra
                planilla.RutaArchivoTemp = pathCtlCargaMasiva;

                await _trazaService.RegistrarDetalleTraza(
                    sftp.CodigoCab,
                    sftp.Metodo,
                    TrazaConstante.TrazaDatosPlanillaSFTP_F,
                    "Obtuvo correctamente el archivo del SFTP",
                    3);

                return planilla;

            }
            catch (ThrowException ex)
            {
                await _trazaService.RegistrarDetalleTraza(sftp.CodigoCab, sftp.Metodo, TrazaConstante.TrazaDatosPlanillaSFTP_E, $"Error al Obtener el archivo del SFTP : {ex.Message}", 3);
                throw new ThrowException("36", "Ocurrio un error al ObtenerDatosPlanillaSFTP al servidor SFTP (" + ex.Message + ")");
            }
        }

        private static decimal ConvertirDeStringADecimal(string Entero, string Decimal)
        {
            string Cadena = string.Concat(Entero, ".", Decimal);
            return Convert.ToDecimal(Cadena, CultureInfo.InvariantCulture);
        }
        private static int CodigoAsumeInteres(string AsumeInteres)
        {
            int CodigoAsumeInteres = 0;
            switch (AsumeInteres)
            {
                case "01":
                case "02":
                    CodigoAsumeInteres = 216;
                    break;
                case "03":
                    CodigoAsumeInteres = 215;
                    break;
            }
            return CodigoAsumeInteres;
        }

        private static int ObtenerModalidadAdelanto(string ModalidadAdelanto)
        {
            int CodigoModalidadAdelanto = 0;

            switch (ModalidadAdelanto)
            {
                case "S":
                    CodigoModalidadAdelanto = 207;
                    break;
                case "C":
                    CodigoModalidadAdelanto = 208;
                    break;
            }
            return CodigoModalidadAdelanto;
        }

        private static int ObtenerCodigoMoneda(string CodigoMonedaArchivo)
        {
            int codigoMoneda = 0;
            switch (CodigoMonedaArchivo)
            {
                case "001":
                    codigoMoneda = 6;
                    break;
                case "010":
                    codigoMoneda = 7;
                    break;
            }
            return codigoMoneda;
        }

        private static int ObtenerTipocuenta(string CodigoProducto)
        {
            int TipoCuenta = 0;
            switch (CodigoProducto)
            {
                case "001":
                    TipoCuenta = 4;
                    break;
                case "002":
                    TipoCuenta = 5;
                    break;
                default:
                    TipoCuenta = 4;
                    break;
            }
            return TipoCuenta;

        }

        private static DapperPlanillaCompleta ConstruirPlanillaDesdeContenido(string[] contenido, char padleft)
        {
            var planilla = new DapperPlanillaCompleta();

            planilla.CodigoMoneda =
                ObtenerCodigoMoneda(contenido[0].Substring(91, 2).Trim().PadLeft(3, padleft));

            planilla.TotalDocumentosPlanilla =
                int.Parse(contenido[0].Substring(50, 6));

            planilla.ImporteTotalPlanilla =
                planilla.CodigoMoneda == 6
                    ? ConvertirDeStringADecimal(contenido[0].Substring(58, 13), contenido[0].Substring(71, 2))
                    : ConvertirDeStringADecimal(contenido[0].Substring(73, 13), contenido[0].Substring(86, 2));

            planilla.CodigoTipoCobranza = 0;
            planilla.InfoTotalDocsPlanilla = planilla.TotalDocumentosPlanilla;
            planilla.ImporteCuenta = 0;

            planilla.CodigotipoCuentaAbono =
                ObtenerTipocuenta(contenido[0].Substring(88, 3).Trim());

            planilla.CodigoCuentaComisiones = planilla.CodigotipoCuentaAbono;
            planilla.CodigotipoCuentaCargo = planilla.CodigotipoCuentaAbono;

            planilla.NumeroCuentaAbono = string.Concat(
                contenido[0].Substring(93, 3).Trim(),
                contenido[0].Substring(96, 20).Trim());

            planilla.NumeroCuentaComisiones = planilla.NumeroCuentaAbono;
            planilla.NumeroCuentaCargo = planilla.NumeroCuentaAbono;

            planilla.AplicaInteresMoratorio = 1;
            planilla.AplicaInteresCompensatorio = 1;

            planilla.CodigoModalidadAdelanto =
                ObtenerModalidadAdelanto(contenido[0].Substring(174, 1).Trim());

            planilla.AsumeInteres =
                CodigoAsumeInteres(contenido[0].Substring(8, 2).Trim());

            planilla.CodigoEstado = 8;
            planilla.FlagControlFlujo = 3;
            planilla.Contenido = contenido;

            return planilla;
        }

    }
}
