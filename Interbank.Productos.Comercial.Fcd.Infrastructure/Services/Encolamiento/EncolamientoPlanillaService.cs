using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.NotificacionAssiConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encolamiento
{
    [ExcludeFromCodeCoverage]
    public class EncolamientoPlanillaService : BackgroundService, IEncolamientoPlanillaService
    {
        private readonly Channel<(DapperPlanillaCompleta, string archivoOrigen, string datacn, decimal codigo, int flagCargaDocumentos)> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EncolamientoPlanillaService> _logger;
        private int _enColaCount;

        public EncolamientoPlanillaService(
            IServiceProvider serviceProvider,
            ILogger<EncolamientoPlanillaService> logger)
        {
            _channel = Channel.CreateUnbounded<(DapperPlanillaCompleta, string, string, decimal, int)>();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _enColaCount = 0;
        }

        public async Task EnqueueAsync(DapperPlanillaCompleta planilla, string archivoOrigen, string datacn, decimal codigo, int flagCargaDocumentos)
        {
            await _channel.Writer.WriteAsync((planilla, archivoOrigen, datacn, codigo, flagCargaDocumentos));
            _logger.LogInformation("Elemento encolado para la planilla: {Planilla}", planilla.NumeroPlanilla);
            IncrementarContador();
            LogEstadoCola();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                await ProcesarItem(item, stoppingToken);
            }
        }

        private async Task ProcesarItem((DapperPlanillaCompleta planilla, string archivoOrigen, string datacn, decimal codigo, int flagCargaDocumentos) item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var (planilla, archivoOrigen, datacn, codigo, flagCargaDocumentos) = item;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            _logger.LogInformation("Procesando planilla {Planilla} a las {Timestamp}", planilla.NumeroPlanilla, timestamp);

            using var scope = _serviceProvider.CreateScope();
            var planillaRepository = scope.ServiceProvider.GetRequiredService<IPlanillasRepository>();
            var ctlService = scope.ServiceProvider.GetRequiredService<ICtlService>();
            var sqlLoaderService = scope.ServiceProvider.GetRequiredService<ISqlLoaderService>();
            var datatableBuilderService = scope.ServiceProvider.GetRequiredService<IDataTableBuilderService>();
            var utilitarios = scope.ServiceProvider.GetRequiredService<IUtilitariosRepository>();
            var trazaService = scope.ServiceProvider.GetRequiredService<ITrazaService>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

            try
            {
                await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaObtConfigCTL_I, "Obtiene configuracion CTL", 6);
                var configuracionCtl = await planillaRepository.ObtenerConfiguracionCTL(planilla);

                if (configuracionCtl.Count == 0)
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaObtConfigCTL_E, "Error al obtener la configuracion del CTL CargaMasiva", 6);
                    throw new ThrowException("36", "Error al Obtener la información de la paramétrica de SFTP (ObtenerConfiguracionSFTP_H2H)");
                }
                await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaObtConfigCTL_F, "Obtuvo correctamente configuracion CTL", 6);

                var ctlArchivo = "";
                try
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaGeneraArchivoCTL_I, "Genera Archivo Ctl", 7);

                    ctlArchivo = ctlService.GenerarArchivoCtlCargaMasiva(TablasConstants.TablaDocumento, planilla, archivoOrigen, configuracionCtl);

                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaGeneraArchivoCTL_F, "Genero correctamente Archivo Ctl", 7);
                }
                catch (Exception ex)
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaGeneraArchivoCTL_E, $"Error al generar archivo CTL: {ex.Message}", 7);

                }

                int resultadoSqlLoader = 0;
                var cantidadDocumentos = planilla.TotalDocumentosPlanilla;
                _logger.LogInformation("Cantidad Documentos Planilla igual a: {CantidadDocumentos}", cantidadDocumentos);


                if (cantidadDocumentos > 2 && flagCargaDocumentos == 0)
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaExecuteSqlLoader_I, "Inicia Proceso de registro de Documentos", 8);
                    resultadoSqlLoader = sqlLoaderService.ExecuteSQLLoader(ctlArchivo, planilla.RutaArchivoTemp + archivoOrigen, datacn);
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaExecuteSqlLoader_F, $"Finalizo correctamente Proceso de registro de Documentos con estado {resultadoSqlLoader}", 8);
                }
                else
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaBulkInsertDocumentos_I, "Inicia Proceso de registro de TMP_DOCUMENTOS_H2H", 8);
                    var secuencias = await planillaRepository.ObtenerSecuenciaPlanilla(cantidadDocumentos);
                    DataTable dataTable = datatableBuilderService.CrearDataTableDocumento();
                    await datatableBuilderService.LeerArchivoCargarDataTableDocumento(planilla.RutaArchivoTemp + archivoOrigen, dataTable, planilla, configuracionCtl, secuencias);
                    var resultadoBulk = await utilitarios.ForAllInsertDataObject(dataTable, TablasConstants.TablaDocumentosTempH2HForAll);
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaBulkInsertDocumentos_F, $"Finalizo correctamente Proceso de registro de TMP_DOCUMENTOS_H2H con estado {resultadoBulk}", 8);

                    int resultCargaDocumentos = 1;
                    _logger.LogInformation("Código de Carga Documentos Base: {ResultCargaDocumentos}", resultCargaDocumentos);

                    if (resultadoBulk)
                    {
                        await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRegistroDocumentos_I, "Inicia Proceso de registro tabla DOCUMENTO", 8);
                        resultCargaDocumentos = await planillaRepository.RegistraDocumentosPlanilla(planilla.NumeroPlanilla ?? "");
                        await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRegistroDocumentos_F, $"Finalizo correctamente Proceso de registro tabla DOCUMENTO con estado {resultCargaDocumentos}", 8);
                    }
                    else
                    {
                        await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaBulkInsertDocumentos_E, $"Error en el registro de TMP_DOCUMENTOS_H2H", 8);
                    }

                    resultadoSqlLoader = resultCargaDocumentos;
                }

                if (resultadoSqlLoader != 0)
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRechazoPlanilla_I, "Inicia Proceso de rechazo de planilla", 9);
                    await planillaRepository.RechazoPlanilla(planilla.NumeroPlanilla ?? "");
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRechazoPlanilla_F, "Finalizo correctamente Proceso de rechazo de planilla", 9);
                    throw new ThrowException("36", "Ocurrio un Error al realizar la Carga masiva de documentos");
                }
                else
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRegistraDocCuota_I, "Inicia Proceso de actualizacion de estados de Planilla", 9);
                    await planillaRepository.RegistrarDocCuota_DocEstadoFCD_H2H(planilla);
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRegistraDocCuota_F, "Finalizo correctamente Proceso de actualizacion de planilla", 9);
                }


                _logger.LogInformation("RegistroDocumentos finalizado para la planilla {Planilla}", planilla.NumeroPlanilla);

                await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaNotificacionAssi_I, "Inicia Proceso de notificacion ASSI", 10);

                var requestNotificacion = new NotificacionAssiRequest(CargaMasivaPlanilla, $"{archivoOrigen}|OK", "32", planilla.NumeroPlanilla, null, null, null);

                var respuestaNotificacion = await notificacionService.NotificacionAssi(requestNotificacion, codigo);

                if (respuestaNotificacion == 32)
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaNotificacionAssi_F, "Finalizo correctamente Proceso de notificacion ASSI", 10);
                }
                else
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaNotificacionAssi_F, "Error Proceso de notificacion ASSI", 10);
                }

                await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaCargaF, "Finalizo correctamente Proceso de Carga Masiva", 1);

                DecrementarContador();
                LogEstadoCola();
            }
            catch (Exception ex)
            {
                var timestampE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _logger.LogError(ex, "Error Proceso de Carga Masiva a las {Timestamp}", timestampE);
                if (!string.IsNullOrEmpty(planilla.NumeroPlanilla))
                {
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRechazoPlanilla_I, "Inicia Proceso de rechazo de planilla", 9);
                    await planillaRepository.RechazoPlanilla(planilla.NumeroPlanilla);
                    await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaRechazoPlanilla_F, "Finalizo correctamente Proceso de rechazo de planilla", 9);
                }

                await trazaService.RegistrarDetalleTraza(codigo, GetType().Name, TrazaConstante.TrazaCargaE, $"Error Proceso de Carga Masiva: {ex.ToString()}", 0);

                var requestNotificacion = new NotificacionAssiRequest(CargaMasivaPlanilla, $"{archivoOrigen}|Ocurrió un Error: {ex.Message}", "36", planilla.NumeroPlanilla, null, null, null);

                await notificacionService.NotificacionAssi(requestNotificacion, codigo);
            }
        }

        private void IncrementarContador()
        {
            _enColaCount++;
        }

        private void DecrementarContador()
        {
            _enColaCount--;
        }

        private void LogEstadoCola()
        {
            _logger.LogInformation("Elementos en cola Enqueue - ProcesoCargaMasiva: {EnColaCount}", _enColaCount);
        }

    }
}
