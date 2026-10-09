using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Ctl
{
    public class CtlService : ICtlService
    {
        private readonly ILogger<CtlService> _logger;

        public CtlService(
            ILogger<CtlService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }

        public string GenerarArchivoCtlCargaMasiva(string nombreTabla, DapperPlanillaCompleta planilla, string archivoOrigen, List<DapperParametroCtl> columnas)
        {
            string strNombreTabla = "DOCUMENTO";

            StringBuilder sbCtl = new StringBuilder();
            sbCtl.AppendLine("OPTIONS (SKIP = 1)");
            sbCtl.AppendLine("LOAD DATA");
            sbCtl.AppendLine("INFILE  '" + planilla.RutaArchivoTemp + archivoOrigen + "'");
            sbCtl.AppendLine("BADFILE '" + planilla.RutaArchivoTemp + planilla.NumeroPlanilla + DateTime.Now.ToString("HH_mm_ss") + ".ERR'");
            sbCtl.AppendLine("");
            sbCtl.AppendLine("INTO TABLE " + strNombreTabla);
            sbCtl.AppendLine("APPEND");
            sbCtl.AppendLine("FIELDS TRAILING NULLCOLS");
            sbCtl.AppendLine("(");
            for (int i = 0; i < columnas.Count; i++)
            {
                string? str = columnas[i].COLUMNA + "          ";
                string? positionPart = (columnas[i].POSICIONINICIAL.ToString() == "0" ? "" : ("position(" + columnas[i].POSICIONINICIAL + " : " + columnas[i].POSICIONFINAL.ToString() + ")"));
                string? additionalPart;

                if (i == 1)
                {
                    additionalPart = columnas[i].ADICIONAL;
                }
                else if (string.IsNullOrEmpty(columnas[i].ADICIONAL))
                {
                    additionalPart = "";
                }
                else
                {
                    additionalPart = "\"" + columnas[i].ADICIONAL + "\"";
                }

                string? additionalPartFinal = (columnas.Count - 1 == i ? "" : ",");

                sbCtl.AppendLine(str + positionPart + additionalPart + additionalPartFinal);
            }

            sbCtl.AppendLine(" )");
            string? strCarpeta = string.Empty;
            strCarpeta = planilla.RutaArchivoTemp;

            if (string.IsNullOrWhiteSpace(strCarpeta))
                throw new ArgumentException("RutaArchivoTemp no puede ser null o vacío");

            if (!Directory.Exists(strCarpeta))
            {
                Directory.CreateDirectory(strCarpeta);
            }

            string strArchivoCtl = String.Concat(strCarpeta, archivoOrigen.Substring(0, archivoOrigen.Length - 3), "ctl");
            _logger.LogInformation("Inicio: Escritura archivo CTL -> {ArchivoCtl}", strArchivoCtl);
            StreamWriter stWriter = new StreamWriter(strArchivoCtl);
            stWriter.Write(sbCtl.ToString());
            stWriter.Close();
            _logger.LogInformation("Fin: Escritura archivo CTL -> {ArchivoCtl}", strArchivoCtl);
            return strArchivoCtl;
        }
    }
}
