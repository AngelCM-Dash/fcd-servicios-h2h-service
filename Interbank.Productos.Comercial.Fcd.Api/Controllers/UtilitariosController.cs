using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetTokenAuthorizationByChannel;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using System.Data;
using System.Security.Cryptography;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class UtilitariosController : ControllerBase
    {
        private readonly IEncrypterService _encrypter;
        private readonly IFileService _fileService;
        private readonly ILineaService _lineaService;
        private readonly ICommonService _commonService;
        private readonly ISftpService _sftpService;
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UtilitariosController> _logger;

        public UtilitariosController(IEncrypterService encrypter, ILogger<UtilitariosController> logger, IFileService fileService, ILineaService lineaService, ICommonService commonService, ISftpService sftpService, IUtilitariosRepository utilitariosRepository, IMediator mediator)
        {
            _encrypter = encrypter;
            _fileService = fileService;
            _lineaService = lineaService;
            _commonService = commonService;
            _sftpService = sftpService;
            _utilitariosRepository = utilitariosRepository;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("GenerateSecretKey")]
        public IActionResult GenerateSecretKey(int bytes)
        {
            if (bytes != 16 && bytes != 24 && bytes != 32)
            {
                return BadRequest("Tamaño inválido. Use 16, 24 o 32 bytes (AES-128, AES-192, AES-256).");
            }

            byte[] keyBytes = new byte[bytes];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            string secretKeyBase64 = Convert.ToBase64String(keyBytes);

            return Ok(new
            {
                Bytes = bytes,
                Bits = bytes * 8,
                SecretKey = secretKeyBase64
            });
        }

        [HttpGet("Encrypter")]
        public string Encrypter(string idCadena)
        {

            var Encrypter = _encrypter.Encrypt(idCadena);

            return Encrypter;
        }

        [HttpGet("Decrypt")]
        public string Decrypt(string idCadena)
        {

            var decrypt = _encrypter.Decrypt(idCadena);

            return decrypt;
        }

        [HttpGet("EncrypterKey")]
        public string EncrypterKey(string idCadena, string key)
        {
            var Encrypter = _encrypter.EncryptKey(idCadena, key);

            return Encrypter;
        }

        [HttpGet("DecryptKey")]
        public string DecryptKey(string idCadena, string key)
        {
            var decrypt = _encrypter.DecryptKey(idCadena, key);

            return decrypt;
        }

        [HttpGet("FileOperation")]
        public IActionResult FileOperation([FromQuery] string sourceFilePath, [FromQuery] string destinationFolder, [FromQuery] int typeOperation)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return BadRequest("La ruta de origen es obligatoria.");
            }

            try
            {
                return typeOperation switch
                {
                    1 => Ok(_fileService.CopyFileOrDirectory(sourceFilePath, destinationFolder)),
                    2 => Ok(_fileService.MoveFileOrDirectory(sourceFilePath, destinationFolder)),
                    3 => Ok(_fileService.DeleteFileOrDirectory(sourceFilePath)),
                    _ => BadRequest("Tipo de operación no válido. Usa 1 para copiar, 2 para mover y 3 para eliminar.")
                };
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }

        [HttpPost("ObtenerDetalleReservaLinea")]
        public async Task<IActionResult> ObtenerDetalleReservaLinea([FromBody] string filePath)
        {
            var consut = await _lineaService.ObtenerDetalleReservaLinea(filePath);

            return Ok(consut);

        }

        [HttpPost("ValidaConexionSFTP")]
        public async Task<ActionResult> ValidaConexionSFTP()
        {
            try
            {
                List<DapperParametro> datosDominio = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioConfiguracionSFTPCargaMasiva);
                if (datosDominio.Count == 0)
                {
                    throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
                }

                var flag = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.FlagRed)?.DESCRIPCIONCORTA);
                string? password = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPassword)?.DESCRIPCIONCORTA;
                var privateKeyLocalFilePath = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePathKey)?.DESCRIPCIONCORTA;


                string? Host = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpHost)?.DESCRIPCIONCORTA;
                int Port = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPort)?.DESCRIPCIONCORTA);
                string? Username = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpUsername)?.DESCRIPCIONCORTA;
                string? Password = flag == 0 ? _encrypter.Decrypt(password ?? "") : privateKeyLocalFilePath;
                string? RemotePath = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePath)?.DESCRIPCIONCORTA;


                var config = new SftpConfiguration()
                {
                    Port = Port,
                    Host = Host,
                    Username = Username,
                    Password = Password,
                    RemotePath = RemotePath

                };

                var contenido = _sftpService.ValidarConexionSftp(config, flag);

                return Ok(contenido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Excepción al ejecutar el archivo: {ex.Message}");
            }
        }

        [HttpPost("TestConnectionSFTP")]
        public Task<ActionResult> TestConnectionSFTP(SftpConfiguration request)
        {
            try
            {
                var config = new SftpConfiguration()
                {
                    Port = request.Port,
                    Host = request.Host,
                    Username = request.Username,
                    Password = request.Password,
                    RemotePath = request.RemotePath
                };

                var contenido = _sftpService.ValidarConexionSftp(config, 1);
                return Task.FromResult<ActionResult>(Ok(contenido));
            }
            catch (Exception ex)
            {
                return Task.FromResult<ActionResult>(StatusCode(500, $"Excepción al ejecutar el archivo: {ex.Message}"));
            }
        }

        [HttpPost("ActualizarParametroDominio")]
        public async Task<IActionResult> ActualizarParametroDominio(DapperParametro request)
        {
            try
            {
                await _utilitariosRepository.ActualizarParametroPorCodigoDominioAndNumOrden(request.CODIGODOMINIO, request.DESCRIPCIONCORTA ?? string.Empty, request.NUMEROORDEN);

                var respuesta = new BaseResponse
                {
                    CodigoRespuesta = 32,
                    MensajeRespuesta = "Ok"
                };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost("NotificacionASSI")]
        public async Task<BaseResponse> NotificacionASSI(NotificacionAssiRequest request, int flagNotificacion)
        {
            try
            {
                var respuesta = new BaseResponse();

                if (flagNotificacion == (int)TipoNotificacion.NotificacionAssiIFX)
                {
                    respuesta = await _commonService.NotificacionAssi(request);

                    if (respuesta.CodigoRespuesta == 32)
                    {
                        _logger.LogError("IFX ASSI Notifico lo siguiente para la planilla {NumeroPlanilla} : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion} - Evento: {Evento}",
                            request.NumeroPlanilla, request.CodigoRespuesta, request.Descripcion, request.CodigoEvento);
                        return respuesta;
                    }
                }
                else
                {
                    respuesta = await _commonService.NotificacionBackAssi(request);

                    if (respuesta.CodigoRespuesta == 32)
                    {
                        _logger.LogError("BACK APIM ASSI Notifico lo siguiente para la planilla {NumeroPlanilla} : CodigoRespuesta: {CodigoRespuesta} - Descripcion: {Descripcion} - Evento: {Evento}",
                        request.NumeroPlanilla, request.CodigoRespuesta, request.Descripcion, request.CodigoEvento);
                        return respuesta;
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en NotificacionASSI para la planilla {NumeroPlanilla}", request?.NumeroPlanilla);

                return new BaseResponse
                {
                    CodigoRespuesta = 36,
                    MensajeRespuesta = ex.Message
                };
            }
        }

        [HttpGet("ObtenerTokenPorCanal")]
        public async Task<ActionResult<IEnumerable<TokenAuthorizationVM>>> GetSuppliersByAcceptor(int tipoScope, string? canal)
        {
            var query = new GetTokenAuthorizationByQuery()
            {
                TipoScope = tipoScope,
                Canal = canal
            };

            var proveedores = await _mediator.Send(query);
            return Ok(proveedores);
        }

        [HttpGet("VerificarActualzacion")]
        public ActionResult VerificarActualzacion()
        {
            var query = "ACTUALIZACION LIBERACION DE LINEAS V2";
            return Ok(query);
        }

        [HttpPost("listar-archivos")]
        public async Task<IActionResult> ListarArchivosOrdenados()
        {
            try
            {
                List<DapperParametro> datosDominio = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioConfiguracionSFTPCargaMasiva);
                if (datosDominio.Count == 0)
                {
                    throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
                }

                var flag = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.FlagRed)?.DESCRIPCIONCORTA);
                string? password = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPassword)?.DESCRIPCIONCORTA;
                var privateKeyLocalFilePath = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePathKey)?.DESCRIPCIONCORTA;


                string? Host = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpHost)?.DESCRIPCIONCORTA;
                int Port = Convert.ToInt32(datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPort)?.DESCRIPCIONCORTA);
                string? Username = datosDominio.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpUsername)?.DESCRIPCIONCORTA;
                string? Password = flag == 0 ? _encrypter.Decrypt(password ?? "") : privateKeyLocalFilePath;
                string? RemotePath = "/IN/bcv/h2h/disbursements/request-to-fcd/";

                var config = new SftpConfiguration()
                {
                    Port = Port,
                    Host = Host,
                    Username = Username,
                    Password = Password,
                    RemotePath = RemotePath

                };

                var contenido = _sftpService.ValidarConexionSftp(config, flag);

                var archivos = ObtenerArchivosOrdenadosPorFecha(config);
                return Ok(archivos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar archivos SFTP");
                return BadRequest(ex.Message);
            }
        }

        private static List<string> ObtenerArchivosOrdenadosPorFecha(SftpConfiguration config)
        {
            var privateKeyFile = new PrivateKeyFile(config.Password ?? string.Empty);
            var authMethod = new PrivateKeyAuthenticationMethod(
                config.Username,
                new[] { privateKeyFile }
            );

            var connectionInfo = new ConnectionInfo(
                config.Host,
                config.Port,
                config.Username,
                authMethod
            );

            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();

                var archivos = client
                    .ListDirectory(config.RemotePath ?? string.Empty)
                    .Where(f => f.IsRegularFile)
                    .OrderByDescending(f => f.LastWriteTime)
                    .Select(f => f.Name)
                    .ToList();

                client.Disconnect();
                return archivos;
            }
        }
    }
}
