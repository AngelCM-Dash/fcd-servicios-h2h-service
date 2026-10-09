using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Monitor;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Monitor
{
    //[ExcludeFromCodeCoverage]
    public class ArchivoMonitorService : IArchivoMonitorService
    {
        private readonly ISftpService _sftpService;
        private readonly IPlanillasRepository _planillasRepository;
        private readonly IUtilitariosRepository _utilitariosRepository;
        private readonly ITrazaService _trazaService;

        public ArchivoMonitorService(
            ISftpService sftpService,
            IPlanillasRepository planillasRepository,
            IUtilitariosRepository utilitariosRepository,
            ITrazaService trazaService)
        {
            _sftpService = sftpService;
            _planillasRepository = planillasRepository;
            _utilitariosRepository = utilitariosRepository;
            _trazaService = trazaService;
        }
        public async Task<string> GenerarArchivoPlano(string planilla, string? observacionCabecera, int flujo, int idCabeceraSeguimiento)
        {
            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, "I", "Inicia Generar Archivo Plano", 13);

            var result = await _planillasRepository.ObtenerInformacionPlanilla(planilla);

            var cabecera = GenerarCabecera(planilla, observacionCabecera);
            string? detalle = flujo == 2 ? await GenerarDetalle(planilla) : null;

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, Encoding.UTF8);
            await writer.WriteLineAsync(cabecera);
            if (detalle != null) await writer.WriteAsync(detalle);
            await writer.FlushAsync();
            ms.Position = 0;

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"MONITOR_H2H_{timestamp}_{planilla}_{result.CanalAtencion}.txt";

            var configSft = await ObtenerConfiguracionSftpMonitor();

            await _sftpService.Conectar(configSft.Host, configSft.Puerto, configSft.Usuario, configSft.Password, configSft.ArchivoPpk, configSft.Flag == "1");
            await _sftpService.SubirArchivoSftp(ms, configSft.RutaArchivo + fileName);
            await _sftpService.Desconectar();

            await _trazaService.RegistrarDetalleTraza(idCabeceraSeguimiento, GetType().Name, "F", "Archivo plano generado correctamente", 13);

            return $"ruta/remota/{fileName}";
        }

        private static string GenerarCabecera(string planilla, string? observacionCabecera)
        {
            string FillerCabecera = new string(' ', 95);
            planilla = planilla.PadRight(10);
            string codigo = ObtenerCodigoCabecera(observacionCabecera ?? "");
            string observacion = (observacionCabecera ?? "").PadRight(105);
            observacionCabecera = codigo + " " + observacion;
            return $"{planilla}{FillerCabecera}{observacionCabecera}";
        }

        private async Task<string> GenerarDetalle(string numeroPlanilla)
        {
            var detalles = await _planillasRepository.ObtenerDetallePlanillaMonitor(numeroPlanilla);
            var sb = new StringBuilder();

            foreach (var detalle in detalles)
            {
                string numeroInterno = (detalle.NumeroInterno ?? "").PadRight(10);
                string numdocAceptante = (detalle.NumeroDocumentoAceptante ?? "").PadRight(15);
                string tipoOperacion = detalle.TipoOperacion ?? "";
                string numdocFisico = (detalle.NumeroDocumentoFisico ?? "").PadRight(20);
                string estado = (detalle.Estado ?? "").PadRight(15);

                string codigo = detalle.Estado == "VIGENTE" ? "00000" : ObtenerCodigoDetalle(detalle.Observacion ?? "");
                string observacion = detalle.Estado == "VIGENTE" ? "-" : (detalle.Observacion ?? "").PadRight(105);

                sb.AppendLine($"{numeroInterno}    {numdocAceptante}{tipoOperacion}{numdocFisico}                    {estado}                    {codigo} {observacion}");
            }

            return sb.ToString();
        }

        private async Task<ConfigSftpRequest> ObtenerConfiguracionSftpMonitor()
        {
            var configSftp = await _utilitariosRepository.ObtenerParametrosPorCodigoDominio(ParametroConstants.DominioSftpMonitorFCD);

            string? password = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPassword)?.DESCRIPCIONCORTA;
            string? host = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpHost)?.DESCRIPCIONCORTA;
            int port = Convert.ToInt32(configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpPort)?.DESCRIPCIONCORTA);
            string? username = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpUsername)?.DESCRIPCIONCORTA;
            string? privateKey = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePathKey)?.DESCRIPCIONCORTA;
            bool flag = Convert.ToBoolean(configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.FlagRed)?.DESCRIPCIONCORTA);
            string? remotePath = configSftp.Find(p => p.NUMEROORDEN == (int)ParametroConstants.NumOrdenDominioSftpMonitor.SftpRemotePath)?.DESCRIPCIONCORTA;

            return new ConfigSftpRequest
            {
                Host = host ?? string.Empty,
                Puerto = port,
                Usuario = username ?? string.Empty,
                Password = password ?? string.Empty,
                ArchivoPpk = privateKey ?? string.Empty,
                Flag = flag ? "1" : "0",
                RutaArchivo = remotePath ?? string.Empty
            };
        }

        private static readonly Dictionary<string, string> codigoDescripcionesDetalle = new Dictionary<string, string>()
        {
            { "", ProcesarTramasConstants.CodigoDefault },
            { "-", ProcesarTramasConstants.CodigoDefault },
            { "--", ProcesarTramasConstants.CodigoDefault },
            { "---", ProcesarTramasConstants.CodigoDefault },
            { "COMMAREA TLDO087", "00001" },
            { "ABEND AAL8 EN PROGRAMA TLDOCLI", "00002" },
            { "ARCHIVO NO ABIERTO", "00003" },
            { "CJE - 2042", "00004" },
            { "CJE - 2042BBVA", "00005" },
            { "CLAVE NO EXISTE,VERIFIQUE", "00006" },
            { "CTA CTE PURGADA.VERIFICAR", "00007" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0002 S", "00008" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0003 S", "00009" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0004 S", "00010" },
            { "ERROR EN RECURSO TLDFDDUS 9770FCDE0008 S", "00011" },
            { "ERROR EN TIPO OPERACION CR/DB", "00012" },
            { "ERROR TRAMA SALIDA SYSTEMATICS", "00013" },
            { "NO CUMPLE DIGITO VERIFI.", "00014" },
            { "NO DEFINIDO", "00015" },
            { "OML0 TS9951 F: ARCHIVO ARCHIVO CERRADO I", "00016" },
            { "RECURSO NO DISPONIBLE APLICAC. DEP", "00017" },
            { "SALDO INSUFICIENTE", "00018" },
        };

        private static readonly Dictionary<string, string> codigoDescripcionesCabecera = new Dictionary<string, string>()
        {
            { ProcesarTramasConstants.DesembolsoOK, ProcesarTramasConstants.CodigoDefault },
            { ProcesarTramasConstants.ErrorRegistrarDetallePlanilla, "00001" },
            { ProcesarTramasConstants.ErrorRegistroTramas, "00002" },
            { ProcesarTramasConstants.ErrorActualizarEstadoPlanillaDb2, "00003" },
            { ProcesarTramasConstants.ErrorRegistrarMovimientoDocumentos, "00004" },
        };

        public static string ObtenerCodigoDetalle(string observacion)
        {
            if (codigoDescripcionesDetalle.TryGetValue(observacion, out var codigo))
            {
                return codigo;
            }
            return "00099";
        }

        public static string ObtenerCodigoCabecera(string observacion)
        {
            if (codigoDescripcionesCabecera.TryGetValue(observacion, out var codigo))
            {
                return codigo;
            }
            return "00099";
        }
    }
}
