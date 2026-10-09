using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using System.Data;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Validaciones
{
    public class ValidarFacturasService : IValidarFacturasService
    {
        private readonly ISftpService _sftpService;

        public ValidarFacturasService(ISftpService sftpService)
        {
            ArgumentNullException.ThrowIfNull(sftpService);

            _sftpService = sftpService;
        }

        public SftpConnectionRequest CrearSftpConnectionRequest(List<DapperParametro> datosConfig, string nombreArchivo)
        {
            var sftpConnectionRequests = new SftpConnectionRequest();

            sftpConnectionRequests.FlagAccesoPPk = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == 1)?.DESCRIPCIONCORTA);
            sftpConnectionRequests.Password = datosConfig.Find(p => p.NUMEROORDEN == 2)?.DESCRIPCIONCORTA;
            sftpConnectionRequests.PrivateKeyLocalFilePath = datosConfig.Find(p => p.NUMEROORDEN == 3)?.DESCRIPCIONCORTA;
            sftpConnectionRequests.Host = datosConfig.Find(p => p.NUMEROORDEN == 4)?.DESCRIPCIONCORTA;
            sftpConnectionRequests.Port = Convert.ToInt32(datosConfig.Find(p => p.NUMEROORDEN == 5)?.DESCRIPCIONCORTA);
            sftpConnectionRequests.Username = datosConfig.Find(p => p.NUMEROORDEN == 6)?.DESCRIPCIONCORTA;
            sftpConnectionRequests.RemotePath = datosConfig.Find(p => p.NUMEROORDEN == 7)?.DESCRIPCIONCORTA;
            sftpConnectionRequests.FileName = nombreArchivo;
            sftpConnectionRequests.ProjectDirectory = "";

            return sftpConnectionRequests;
        }

        public DataTable InsertarProcesoDetallePlanilla(int numeroSecuencia, string[] contenido)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("NumeroSecuencia", typeof(string));
            dataTable.Columns.Add("NumeroDocumentoFisico", typeof(string));
            dataTable.Columns.Add("CodigoCLienteProveedor", typeof(string));
            dataTable.Columns.Add("CodigoTipoDocumentoCobranza", typeof(string));
            dataTable.Columns.Add("TipoDocumentoCobranza", typeof(string));
            dataTable.Columns.Add("NumeroDocumentoIdentidad", typeof(string));
            dataTable.Columns.Add("FlagDuplicado", typeof(string));

            string[] contenidoSinPrimeraFila = contenido.Skip(1).ToArray();

            foreach (var rowDetalle in contenidoSinPrimeraFila)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["NumeroSecuencia"] = numeroSecuencia;
                dataRow["NumeroDocumentoFisico"] = rowDetalle.Substring(71, 20).Trim();
                dataRow["CodigoCLienteProveedor"] = rowDetalle.Substring(406, 10).Trim();
                dataRow["CodigoTipoDocumentoCobranza"] = ObtenerCodigoDocCobranza(rowDetalle.Substring(70, 1).Trim());
                dataRow["TipoDocumentoCobranza"] = rowDetalle.Substring(70, 1).Trim();
                dataRow["NumeroDocumentoIdentidad"] = rowDetalle.Substring(150, 15).Trim();
                dataRow["FlagDuplicado"] = 0;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public async Task<string> GenerarYSubirArchivoTxtAsync(List<ValidatePlanillaResponse> request, string nombreArchivo, SftpConnectionRequest sftpConfig, string remotePathUpload)
        {
            nombreArchivo = Path.GetFileNameWithoutExtension(nombreArchivo);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"{nombreArchivo}_Doc_Duplicados_{timestamp}.txt";
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                foreach (var documento in request)
                {
                    string numeroDocumentoFisico = (documento.NumeroDocumentoFisico ?? string.Empty).PadRight(20).Substring(0, 20);
                    string tipoDocumentoCobranza = (documento.TipoDocumentoCobranza ?? string.Empty).Trim().PadRight(1).Substring(0, 1);
                    string numeroDocumentoIdentidad = (documento.NumeroDocumentoIdentidad ?? string.Empty).PadRight(20).Substring(0, 20);

                    string filler = " ";

                    string linea = numeroDocumentoFisico + filler + tipoDocumentoCobranza + filler + numeroDocumentoIdentidad;
                    await writer.WriteLineAsync(linea);
                }

                await writer.FlushAsync();

                memoryStream.Position = 0;

                await _sftpService.Conectar(sftpConfig.Host ?? string.Empty, sftpConfig.Port, sftpConfig.Username ?? string.Empty, sftpConfig.Password ?? string.Empty, sftpConfig.PrivateKeyLocalFilePath ?? string.Empty, Convert.ToBoolean(sftpConfig.FlagAccesoPPk));
                await _sftpService.ValidarRutaDirectorioRemotaSftp(remotePathUpload);
                await _sftpService.ValidarArchivoExistenteSftp(fileName);
                await _sftpService.SubirArchivoSftp(memoryStream, remotePathUpload + fileName);
                await _sftpService.Desconectar();

                return $"{remotePathUpload}|{fileName}";
            }
        }

        public static string ObtenerCodigoDocCobranza(string observacion)
        {
            if (codigoTipoDocumentoCobranza.TryGetValue(observacion, out var codigo))
            {
                return codigo;
            }
            return "00";
        }

        private static readonly Dictionary<string, string> codigoTipoDocumentoCobranza = new Dictionary<string, string>()
        {
            { "F", "54" },
            { "G", "55" },
            { "C", "56" },
            { "K", "57" },
            { "D", "58" },
            { "E", "59" },

        };
    }
}

