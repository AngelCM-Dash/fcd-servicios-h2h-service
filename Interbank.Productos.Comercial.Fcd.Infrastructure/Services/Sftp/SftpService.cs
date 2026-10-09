using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
{
    public class SftpService : ISftpService, IDisposable
    {
        private ISftpClientWrapper _client = null!;
        private readonly ISftpClientFactory _factory; // Inyectamos la fábrica
        private readonly ILogger<SftpService> _logger;
        private bool _disposed;

        public SftpService(ILogger<SftpService> logger, ISftpClientFactory factory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task Conectar(string host, int puerto, string usuario, string contraseña, string rutaClavePrivada, bool flagClave)
        {
            try
            {
                if (_client != null && _client.IsConnected)
                {
                    _client.Disconnect();
                    _client.Dispose();
                }

                _client = _factory.CreateClient(host, puerto, usuario, contraseña, rutaClavePrivada, flagClave);

                _logger.LogInformation("Conectando al servidor SFTP {Host}:{Puerto}...", host, puerto);
                await Task.Run(() => _client.Connect());
                _logger.LogInformation("Conexión establecida exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar al servidor SFTP.");
                throw new ThrowException("36", $"Ocurrio un error al conectarse al servidor SFTP ({ex.Message})");
            }
        }

        public async Task<bool> ValidarRutaDirectorioRemotaSftp(string rutaRemota)
        {
            if (_client == null) return false;

            try
            {
                _logger.LogInformation("Validando existencia de directorio remoto: {Ruta}", rutaRemota);

                await Task.Run(() => _client.ChangeDirectory(rutaRemota));

                _logger.LogInformation("Directorio validado exitosamente: {Ruta}", rutaRemota);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar la ruta remota: {Ruta}", rutaRemota);
                throw new ThrowException("36", "No existe directorio Correcto (" + rutaRemota + " - " + ex.Message + ")");
            }
        }

        public async Task<bool> ValidarArchivoExistenteSftp(string rutaRemota)
        {
            if (_client == null || !_client.IsConnected) return false;

            try
            {
                _logger.LogInformation("Verificando existencia del archivo: {Ruta}", rutaRemota);
                return await Task.Run(() => _client.Exists(rutaRemota));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del archivo: {Ruta}", rutaRemota);
                throw new ThrowException("36", "No Existe archivo en la ruta especificado (" + rutaRemota + " - " + ex.Message + ")");
            }
        }

        public async Task<string[]> LeerLineasArchivoSftp(string rutaRemota)
        {
            if (_client == null || !_client.IsConnected) return Array.Empty<string>();

            try
            {
                _logger.LogInformation("Leyendo archivo remoto línea por línea: {Ruta}", rutaRemota);
                return await Task.Run(() => _client.ReadAllLines(rutaRemota));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer líneas desde el archivo remoto: {Ruta}", rutaRemota);
                throw new ThrowException("36", "Error al intentar leer el contenido del archivo (" + ex.Message + ")");
            }
        }

        public async Task DescargarArchivoSftp(string rutaRemota, string rutaLocal)
        {
            if (_client == null || !_client.IsConnected)
            {
                _logger.LogError("No hay conexión activa al servidor SFTP para descargar archivo.");
                return;
            }

            try
            {
                _logger.LogInformation("Descargando archivo desde '{Remota}' a '{Local}'", rutaRemota, rutaLocal);

                var directorioLocal = Path.GetDirectoryName(rutaLocal);
                if (!string.IsNullOrEmpty(directorioLocal) && !Directory.Exists(directorioLocal))
                    Directory.CreateDirectory(directorioLocal);

                await Task.Run(() =>
                {
                    using var archivoLocal = File.OpenWrite(rutaLocal);
                    _client.DownloadFile(rutaRemota, archivoLocal);
                });

                _logger.LogInformation("Archivo descargado exitosamente a '{Local}'", rutaLocal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar el archivo desde '{Remota}'", rutaRemota);
                throw new ThrowException("36", $"Error al descargar el archivo desde {rutaRemota} a {rutaLocal} - ({ex.Message})");
            }
        }

        public async Task SubirArchivoSftp(Stream archivo, string rutaRemota)
        {
            if (_client == null || !_client.IsConnected)
            {
                _logger.LogError("No hay conexión activa al servidor SFTP para subir archivo.");
                return;
            }

            try
            {
                _logger.LogInformation("Subiendo archivo a '{Remota}'", rutaRemota);

                // Subida asíncrona
                await Task.Run(() => _client.UploadFile(archivo, rutaRemota));

                _logger.LogInformation("Archivo subido exitosamente a '{Remota}'", rutaRemota);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir el archivo a '{Remota}'", rutaRemota);
                throw new ThrowException(
                    "36",
                    $"Error al subir el archivo al SFTP en la rutaRemota {rutaRemota} - ( {ex.Message} )"
                );
            }
        }

        public async Task Desconectar()
        {
            if (_client == null || !_client.IsConnected)
            {
                _logger.LogError("No hay conexión activa al servidor SFTP para desconectar.");
                return;
            }

            try
            {
                _logger.LogInformation("Desconectando del servidor SFTP...");
                await Task.Run(() => _client.Disconnect());
                _logger.LogInformation("Desconexión exitosa.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desconectar del servidor SFTP.");
                throw new ThrowException("36", "Error al desconectar");
            }
        }

        public async Task<bool> ValidarConexionSftp(SftpConfiguration config, int flag)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (flag == 1)
                    {
                        var privateKeyFile = new PrivateKeyFile(config.Password ?? "");
                        var privateKeyFileProvider = new PrivateKeyFile[] { privateKeyFile };
                        var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(config.Username, privateKeyFileProvider);
                        var connectionInfo = new ConnectionInfo(config.Host, config.Port, config.Username, privateKeyAuthenticationMethod);

                        using var client = new SftpClient(connectionInfo);
                        client.Connect();
                        client.Disconnect();
                        return true;
                    }
                    else
                    {
                        using var client = new SftpClient(config.Host ?? "", config.Port, config.Username ?? "", config.Password ?? "");
                        client.Connect();
                        client.Disconnect();
                        return true;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la conexión al SFTP. InnerException: {InnerException}", ex.InnerException?.Message);
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _client?.Dispose();
            }
            _disposed = true;
        }
    }
}
