//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
//using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
//using Microsoft.Extensions.Logging;
//using System.Diagnostics;

//namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.SqlLoader
//{
//    public class SqlLoaderService : ISqlLoaderService
//    {
//        private readonly ILogger<SqlLoaderService> _logger;
//        private readonly IEncrypterService _encrypter;

//        public SqlLoaderService(ILogger<SqlLoaderService> logger, IEncrypterService encrypter)
//        {
//            ArgumentNullException.ThrowIfNull(logger);
//            ArgumentNullException.ThrowIfNull(encrypter);

//            _logger = logger;
//            _encrypter = encrypter;
//        }

//        public int ExecuteSQLLoader(string rutaArchivoData, string pathData, string ConfigPs)
//        {
//            try
//            {
//                _logger.LogInformation("Iniciando ejecución de SQLLoader. rutaArchivoData: {RutaArchivoData}, pathData: {PathData}", rutaArchivoData, pathData);

//                var pathCTL = rutaArchivoData.Replace(".ctl", ".ctl");
//                var pathLog = rutaArchivoData.Replace(".ctl", ".log");

//                var argumentos = " control=" + pathCTL + " userid = " + _encrypter.Decrypt(ConfigPs) + " data = " + pathData + " log = " + pathLog;
//                _logger.LogDebug("Argumentos para sqlldr: {Argumentos}", argumentos);

//                ProcessStartInfo psi = new ProcessStartInfo("sqlldr");  //NOSONAR           
//                psi.Arguments = argumentos;
//                psi.WindowStyle = ProcessWindowStyle.Hidden;

//                using (Process? proc = Process.Start(psi))
//                {
//                    if (proc == null)
//                    {
//                        _logger.LogError("El proceso SqlLoader no pudo iniciarse.");
//                        throw new InvalidOperationException("El proceso SqlLoader no pudo iniciarse.");
//                    }
//                    proc.WaitForExit();
//                    _logger.LogInformation("SQLLoader finalizó con código de salida: {ExitCode}", proc.ExitCode);
//                    return proc.ExitCode;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error: Metodo ExecuteSQLLoader: {Mensaje}", ex.Message);
//                throw new ThrowException("36", "Error: Metodo ExecuteSQLLoader ( " + ex.Message + ")", ex);
//            }
//        }
//    }
//}

using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Microsoft.Extensions.Logging;
using System;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.SqlLoader
{
    // SOLO PRUEBAS LOCALES: no invoca sqlldr ni descifra credenciales.
    public class SqlLoaderService : ISqlLoaderService
    {
        private readonly ILogger<SqlLoaderService> _logger;
        private readonly IEncrypterService _encrypter;

        // 0: éxito (exit code 0), 1: finaliza con código de salida 1,
        // 2: Process.Start devuelve null, 3: excepción de ejecución.
        public static int MockEscenario = 0;

        public SqlLoaderService(ILogger<SqlLoaderService> logger, IEncrypterService encrypter)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(encrypter);
            _logger = logger;
            _encrypter = encrypter; // Compatibilidad DI; nunca se invoca Decrypt.
        }

        public int ExecuteSQLLoader(string rutaArchivoData, string pathData, string ConfigPs)
        {
            try
            {
                _logger.LogInformation("Iniciando ejecución de SQLLoader. rutaArchivoData: {RutaArchivoData}, pathData: {PathData}", rutaArchivoData, pathData);

                var pathCTL = rutaArchivoData.Replace(".ctl", ".ctl");
                var pathLog = rutaArchivoData.Replace(".ctl", ".log");

                // Mantiene el template ORIGINAL LogDebug sin exponer credenciales.
                var argumentos = " control=" + pathCTL + " userid = <MOCK_SIN_CREDENCIALES> data = " + pathData + " log = " + pathLog;
                _logger.LogDebug("Argumentos para sqlldr: {Argumentos}", argumentos);

                if (MockEscenario == 2)
                {
                    _logger.LogError("El proceso SqlLoader no pudo iniciarse.");
                    throw new InvalidOperationException("El proceso SqlLoader no pudo iniciarse.");
                }

                if (MockEscenario == 3)
                    throw new InvalidOperationException("Error simulado durante la ejecución de sqlldr.");

                int exitCode = MockEscenario == 1 ? 1 : 0;
                _logger.LogInformation("SQLLoader finalizó con código de salida: {ExitCode}", exitCode);
                return exitCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: Metodo ExecuteSQLLoader: {Mensaje}", ex.Message);
                throw new ThrowException("36", "Error: Metodo ExecuteSQLLoader ( " + ex.Message + ")", ex);
            }
        }
    }
}
