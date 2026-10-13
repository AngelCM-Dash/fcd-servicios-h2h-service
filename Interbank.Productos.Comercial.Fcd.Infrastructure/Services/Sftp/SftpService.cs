//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
//using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
//using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services;
//using Microsoft.Extensions.Logging;
//using Renci.SshNet;

//namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
//{
//    public class SftpService : ISftpService, IDisposable
//    {
//        private ISftpClientWrapper _client = null!;
//        private readonly ISftpClientFactory _factory; // Inyectamos la fábrica
//        private readonly ILogger<SftpService> _logger;
//        private bool _disposed;

//        public SftpService(ILogger<SftpService> logger, ISftpClientFactory factory)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
//        }

//        public async Task Conectar(string host, int puerto, string usuario, string contraseña, string rutaClavePrivada, bool flagClave)
//        {
//            try
//            {
//                if (_client != null && _client.IsConnected)
//                {
//                    _client.Disconnect();
//                    _client.Dispose();
//                }

//                _client = _factory.CreateClient(host, puerto, usuario, contraseña, rutaClavePrivada, flagClave);

//                _logger.LogInformation("Conectando al servidor SFTP {Host}:{Puerto}...", host, puerto);
//                await Task.Run(() => _client.Connect());
//                _logger.LogInformation("Conexión establecida exitosamente.");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al conectar al servidor SFTP.");
//                throw new ThrowException("36", $"Ocurrio un error al conectarse al servidor SFTP ({ex.Message})");
//            }
//        }

//        public async Task<bool> ValidarRutaDirectorioRemotaSftp(string rutaRemota)
//        {
//            if (_client == null) return false;

//            try
//            {
//                _logger.LogInformation("Validando existencia de directorio remoto: {Ruta}", rutaRemota);

//                await Task.Run(() => _client.ChangeDirectory(rutaRemota));

//                _logger.LogInformation("Directorio validado exitosamente: {Ruta}", rutaRemota);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al validar la ruta remota: {Ruta}", rutaRemota);
//                throw new ThrowException("36", "No existe directorio Correcto (" + rutaRemota + " - " + ex.Message + ")");
//            }
//        }

//        public async Task<bool> ValidarArchivoExistenteSftp(string rutaRemota)
//        {
//            if (_client == null || !_client.IsConnected) return false;

//            try
//            {
//                _logger.LogInformation("Verificando existencia del archivo: {Ruta}", rutaRemota);
//                return await Task.Run(() => _client.Exists(rutaRemota));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al verificar la existencia del archivo: {Ruta}", rutaRemota);
//                throw new ThrowException("36", "No Existe archivo en la ruta especificado (" + rutaRemota + " - " + ex.Message + ")");
//            }
//        }

//        public async Task<string[]> LeerLineasArchivoSftp(string rutaRemota)
//        {
//            if (_client == null || !_client.IsConnected) return Array.Empty<string>();

//            try
//            {
//                _logger.LogInformation("Leyendo archivo remoto línea por línea: {Ruta}", rutaRemota);
//                return await Task.Run(() => _client.ReadAllLines(rutaRemota));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al leer líneas desde el archivo remoto: {Ruta}", rutaRemota);
//                throw new ThrowException("36", "Error al intentar leer el contenido del archivo (" + ex.Message + ")");
//            }
//        }

//        public async Task DescargarArchivoSftp(string rutaRemota, string rutaLocal)
//        {
//            if (_client == null || !_client.IsConnected)
//            {
//                _logger.LogError("No hay conexión activa al servidor SFTP para descargar archivo.");
//                return;
//            }

//            try
//            {
//                _logger.LogInformation("Descargando archivo desde '{Remota}' a '{Local}'", rutaRemota, rutaLocal);

//                var directorioLocal = Path.GetDirectoryName(rutaLocal);
//                if (!string.IsNullOrEmpty(directorioLocal) && !Directory.Exists(directorioLocal))
//                    Directory.CreateDirectory(directorioLocal);

//                await Task.Run(() =>
//                {
//                    using var archivoLocal = File.OpenWrite(rutaLocal);
//                    _client.DownloadFile(rutaRemota, archivoLocal);
//                });

//                _logger.LogInformation("Archivo descargado exitosamente a '{Local}'", rutaLocal);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al descargar el archivo desde '{Remota}'", rutaRemota);
//                throw new ThrowException("36", $"Error al descargar el archivo desde {rutaRemota} a {rutaLocal} - ({ex.Message})");
//            }
//        }

//        public async Task SubirArchivoSftp(Stream archivo, string rutaRemota)
//        {
//            if (_client == null || !_client.IsConnected)
//            {
//                _logger.LogError("No hay conexión activa al servidor SFTP para subir archivo.");
//                return;
//            }

//            try
//            {
//                _logger.LogInformation("Subiendo archivo a '{Remota}'", rutaRemota);

//                // Subida asíncrona
//                await Task.Run(() => _client.UploadFile(archivo, rutaRemota));

//                _logger.LogInformation("Archivo subido exitosamente a '{Remota}'", rutaRemota);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al subir el archivo a '{Remota}'", rutaRemota);
//                throw new ThrowException(
//                    "36",
//                    $"Error al subir el archivo al SFTP en la rutaRemota {rutaRemota} - ( {ex.Message} )"
//                );
//            }
//        }

//        public async Task Desconectar()
//        {
//            if (_client == null || !_client.IsConnected)
//            {
//                _logger.LogError("No hay conexión activa al servidor SFTP para desconectar.");
//                return;
//            }

//            try
//            {
//                _logger.LogInformation("Desconectando del servidor SFTP...");
//                await Task.Run(() => _client.Disconnect());
//                _logger.LogInformation("Desconexión exitosa.");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al desconectar del servidor SFTP.");
//                throw new ThrowException("36", "Error al desconectar");
//            }
//        }

//        public async Task<bool> ValidarConexionSftp(SftpConfiguration config, int flag)
//        {
//            try
//            {
//                return await Task.Run(() =>
//                {
//                    if (flag == 1)
//                    {
//                        var privateKeyFile = new PrivateKeyFile(config.Password ?? "");
//                        var privateKeyFileProvider = new PrivateKeyFile[] { privateKeyFile };
//                        var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(config.Username, privateKeyFileProvider);
//                        var connectionInfo = new ConnectionInfo(config.Host, config.Port, config.Username, privateKeyAuthenticationMethod);

//                        using var client = new SftpClient(connectionInfo);
//                        client.Connect();
//                        client.Disconnect();
//                        return true;
//                    }
//                    else
//                    {
//                        using var client = new SftpClient(config.Host ?? "", config.Port, config.Username ?? "", config.Password ?? "");
//                        client.Connect();
//                        client.Disconnect();
//                        return true;
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error en la conexión al SFTP. InnerException: {InnerException}", ex.InnerException?.Message);
//                return false;
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (_disposed) return;
//            if (disposing)
//            {
//                _client?.Dispose();
//            }
//            _disposed = true;
//        }
//    }
//}


using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp
{
    public class SftpService : ISftpService, IDisposable
    {
        private readonly ISftpClientFactory _factory;
        private readonly ILogger<SftpService> _logger;
        private bool _disposed;

        // =============================================
        // CONFIGURACION TEMPORAL DE MOCKS
        // =============================================

        // false = Todos los métodos OK
        // true  = Habilitar errores simulados
        public static bool MockError = false;

        // TODOS, Conectar, ValidarRuta, ValidarArchivo,
        // LeerArchivo, Descargar, Subir, Desconectar,
        // ValidarConexionSftp, Dispose
        public static string MockMetodoError = "TODOS";

        public SftpService(ILogger<SftpService> logger, ISftpClientFactory factory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private static bool DebeSimularError(string metodo)
        {
            return MockError &&
                (string.Equals(MockMetodoError, "TODOS", StringComparison.OrdinalIgnoreCase)
                || string.Equals(MockMetodoError, metodo, StringComparison.OrdinalIgnoreCase));
        }

        // =============================================
        // 1. CONECTAR
        // =============================================

        public Task Conectar(string host, int puerto, string usuario, string contraseña, string rutaClavePrivada, bool flagClave)
        {
            try
            {
                // MOCK: No crea cliente ni conexión real.

                _logger.LogInformation("Conectando al servidor SFTP {Host}:{Puerto}...", host, puerto);

                if (DebeSimularError("Conectar"))
                    throw new Exception("Error simulado de conexión SFTP");

                _logger.LogInformation("Conexión establecida exitosamente.");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar al servidor SFTP.");

                throw new ThrowException("36", $"Ocurrio un error al conectarse al servidor SFTP ({ex.Message})");
            }
        }

        // =============================================
        // 2. VALIDAR DIRECTORIO REMOTO
        // =============================================

        public Task<bool> ValidarRutaDirectorioRemotaSftp(string rutaRemota)
        {
            try
            {
                _logger.LogInformation("Validando existencia de directorio remoto: {Ruta}", rutaRemota);

                // MOCK: No ejecuta ChangeDirectory.

                if (DebeSimularError("ValidarRuta"))
                    throw new Exception("Error simulado de directorio remoto");

                _logger.LogInformation("Directorio validado exitosamente: {Ruta}", rutaRemota);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar la ruta remota: {Ruta}", rutaRemota);

                throw new ThrowException("36", "No existe directorio Correcto (" + rutaRemota + " - " + ex.Message + ")");
            }
        }

        // =============================================
        // 3. VALIDAR ARCHIVO EXISTENTE
        // =============================================

        public Task<bool> ValidarArchivoExistenteSftp(string rutaRemota)
        {
            try
            {
                _logger.LogInformation("Verificando existencia del archivo: {Ruta}", rutaRemota);

                // MOCK: No ejecuta Exists.

                if (DebeSimularError("ValidarArchivo"))
                    throw new Exception("Error simulado de archivo remoto");

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del archivo: {Ruta}", rutaRemota);

                throw new ThrowException("36", "No Existe archivo en la ruta especificado (" + rutaRemota + " - " + ex.Message + ")");
            }
        }

        // =============================================
        // 4. LEER LINEAS DE ARCHIVO
        // =============================================

        public async Task<string[]> LeerLineasArchivoSftp(string rutaRemota)
        {
            try
            {
                _logger.LogInformation("Leyendo archivo remoto línea por línea: {Ruta}", rutaRemota);

                // MOCK: No lee archivos del servidor.

                if (DebeSimularError("LeerArchivo"))
                    throw new Exception("Error simulado al leer archivo SFTP");

                // MOCK: Carpeta docs relativa a la raíz del proyecto.
                string directorioLocal = Path.Combine(Directory.GetCurrentDirectory(), "docs");
                string nombreArchivo = "2052243424907PEN0000000080000002026100921022900000.txt";
                // Construir la ruta local.
                string rutaArchivoLocal = Path.Combine(directorioLocal, nombreArchivo);

                // Leer el archivo local.
                string[] lineas = await File.ReadAllLinesAsync(rutaArchivoLocal);

                return lineas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer líneas desde el archivo remoto: {Ruta}", rutaRemota);

                throw new ThrowException("36", "Error al intentar leer el contenido del archivo (" + ex.Message + ")");
            }
        }

        // =============================================
        // 5. DESCARGAR ARCHIVO
        // =============================================

        public Task DescargarArchivoSftp(string rutaRemota, string rutaLocal)
        {
            try
            {
                _logger.LogInformation("Descargando archivo desde '{Remota}' a '{Local}'", rutaRemota, rutaLocal);

                // MOCK: No crea directorios ni descarga archivos.

                if (DebeSimularError("Descargar"))
                    throw new Exception("Error simulado de descarga SFTP");

                _logger.LogInformation("Archivo descargado exitosamente a '{Local}'", rutaLocal);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar el archivo desde '{Remota}'", rutaRemota);

                throw new ThrowException("36", $"Error al descargar el archivo desde {rutaRemota} a {rutaLocal} - ({ex.Message})");
            }
        }

        // =============================================
        // 6. SUBIR ARCHIVO
        // =============================================

        public Task SubirArchivoSftp(Stream archivo, string rutaRemota)
        {
            try
            {
                _logger.LogInformation("Subiendo archivo a '{Remota}'", rutaRemota);

                // MOCK: No ejecuta UploadFile.

                if (DebeSimularError("Subir"))
                    throw new Exception("Error simulado al subir archivo SFTP");

                _logger.LogInformation("Archivo subido exitosamente a '{Remota}'", rutaRemota);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir el archivo a '{Remota}'", rutaRemota);

                throw new ThrowException("36", $"Error al subir el archivo al SFTP en la rutaRemota {rutaRemota} - ( {ex.Message} )");
            }
        }

        // =============================================
        // 7. DESCONECTAR
        // =============================================

        public Task Desconectar()
        {
            try
            {
                _logger.LogInformation("Desconectando del servidor SFTP...");

                // MOCK: No ejecuta Disconnect.

                if (DebeSimularError("Desconectar"))
                    throw new Exception("Error simulado al desconectar SFTP");

                _logger.LogInformation("Desconexión exitosa.");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desconectar del servidor SFTP.");

                throw new ThrowException("36", "Error al desconectar");
            }
        }

        // =============================================
        // 8. VALIDAR CONEXION SFTP
        // =============================================

        public Task<bool> ValidarConexionSftp(SftpConfiguration config, int flag)
        {
            try
            {
                // MOCK: No crea SftpClient ni PrivateKeyFile.
                // No realiza Connect ni Disconnect.

                if (DebeSimularError("ValidarConexionSftp"))
                    throw new Exception("Error simulado en conexión SFTP");

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la conexión al SFTP. InnerException: {InnerException}", ex.InnerException?.Message);

                return Task.FromResult(false);
            }
        }

        // =============================================
        // 9. DISPOSE
        // =============================================

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            // MOCK: No libera ningún cliente SFTP real.
            _disposed = true;
        }
    }
}

