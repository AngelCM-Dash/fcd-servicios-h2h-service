using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.SqlLoader
{
    public class SqlLoaderService : ISqlLoaderService
    {
        private readonly ILogger<SqlLoaderService> _logger;
        private readonly IEncrypterService _encrypter;

        public SqlLoaderService(ILogger<SqlLoaderService> logger, IEncrypterService encrypter)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(encrypter);

            _logger = logger;
            _encrypter = encrypter;
        }

        public int ExecuteSQLLoader(string rutaArchivoData, string pathData, string ConfigPs)
        {
            try
            {
                _logger.LogInformation("Iniciando ejecución de SQLLoader. rutaArchivoData: {RutaArchivoData}, pathData: {PathData}", rutaArchivoData, pathData);

                var pathCTL = rutaArchivoData.Replace(".ctl", ".ctl");
                var pathLog = rutaArchivoData.Replace(".ctl", ".log");

                var argumentos = " control=" + pathCTL + " userid = " + _encrypter.Decrypt(ConfigPs) + " data = " + pathData + " log = " + pathLog;
                _logger.LogDebug("Argumentos para sqlldr: {Argumentos}", argumentos);

                ProcessStartInfo psi = new ProcessStartInfo("sqlldr");  //NOSONAR           
                psi.Arguments = argumentos;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                using (Process? proc = Process.Start(psi))
                {
                    if (proc == null)
                    {
                        _logger.LogError("El proceso SqlLoader no pudo iniciarse.");
                        throw new InvalidOperationException("El proceso SqlLoader no pudo iniciarse.");
                    }
                    proc.WaitForExit();
                    _logger.LogInformation("SQLLoader finalizó con código de salida: {ExitCode}", proc.ExitCode);
                    return proc.ExitCode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: Metodo ExecuteSQLLoader: {Mensaje}", ex.Message);
                throw new ThrowException("36", "Error: Metodo ExecuteSQLLoader ( " + ex.Message + ")", ex);
            }
        }
    }
}
