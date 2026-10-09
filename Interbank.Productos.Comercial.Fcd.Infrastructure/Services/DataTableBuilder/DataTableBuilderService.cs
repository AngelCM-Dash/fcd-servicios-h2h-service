using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DataTable;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.DataTableBuilder
{
    [ExcludeFromCodeCoverage]
    public class DataTableBuilderService : IDataTableBuilderService
    {
        private readonly ILogger<DataTableBuilderService> _logger;

        public DataTableBuilderService(ILogger<DataTableBuilderService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }

        public DataTable CrearDataTableDocumento()
        {
            try
            {
                _logger.LogInformation("Creando DataTable para Documentos H2H");
                DataTable table = new DataTable();
                table.Columns.Add(ColumnasDocumentosH2H.NumeroInterno, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.Item, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoClientePlanilla, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroDocumentoFisico, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoMoneda, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.ImporteOriginal, typeof(decimal));
                table.Columns.Add(ColumnasDocumentosH2H.FechaVencimiento, typeof(DateTime));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroPlanilla, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoCliente, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.FechaRegistro, typeof(DateTime));
                table.Columns.Add(ColumnasDocumentosH2H.Protestable, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.FlagCuota, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.FlagCompletado, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroCuotas, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroDocumentoAceptante, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.RazonSocialAceptante, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.TipoDocumentoAceptante, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoTipoDocumentoCobranza, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoTipoAbono, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoTipoCuenta, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroCuenta, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroLinea, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.FechaCargo, typeof(DateTime));
                table.Columns.Add(ColumnasDocumentosH2H.NumeroInstruccion, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.FechaAdelanto, typeof(DateTime));
                table.Columns.Add(ColumnasDocumentosH2H.SaldoActualDocumento, typeof(decimal));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoTipoAdelanto, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.FlagObservado, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.Observacion, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.ReglasValidacion, typeof(string));
                table.Columns.Add(ColumnasDocumentosH2H.CodigoEstado, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.AplicaPortes, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.DiasAmpliacion, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.ValidarCuentaCliente, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.DesembolsoAutomatico, typeof(int));
                table.Columns.Add(ColumnasDocumentosH2H.PorcentajeProrroga, typeof(decimal));
                table.Columns.Add(ColumnasDocumentosH2H.TipoCambioWDC, typeof(decimal));
                _logger.LogInformation("DataTable para Documentos H2H creada exitosamente");
                return table;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear DataTable para Documentos H2H: {Mensaje}", ex.Message);
                return new DataTable();
            }
        }

        public async Task LeerArchivoCargarDataTableDocumento(string filePath, DataTable dataTable, DapperPlanillaCompleta dataPlanilla, List<DapperParametroCtl> dataParametroCtl, List<DapperSecuenciaPlanilla> secuenciasNumeroInterno)
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);

                var dataLines = lines.Skip(1);

                _logger.LogInformation("Leyendo archivo y cargando DataTable para Documentos H2H desde: {FilePath}. Número de líneas a procesar: {LineCount}",
                filePath, dataLines.Count());

                var i = 0;
                var iSecuencias = 0;

                foreach (var line in dataLines)
                {
                    i++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    DataRow row = dataTable.NewRow();

                    string fechaAdelantoStr = line.Substring(497, 9).Trim();
                    var fechaVencimientoStr = DateTime.ParseExact(line.Substring(471, 9).Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    string valueImporteOriginal = line.Substring(101, 15).Trim();
                    decimal numericValue = Convert.ToDecimal(valueImporteOriginal);
                    string numeroinstruccion = line.Substring(384, 21).Trim();
                    row[ColumnasDocumentosH2H.NumeroInterno] = secuenciasNumeroInterno?[iSecuencias]?.NumeroSecuencia?.ToString().PadLeft(10, '0');
                    row[ColumnasDocumentosH2H.Item] = i;
                    row[ColumnasDocumentosH2H.CodigoClientePlanilla] = dataPlanilla.CodigoCliente;
                    row[ColumnasDocumentosH2H.NumeroDocumentoFisico] = line.Substring(71, 20).Trim();
                    row[ColumnasDocumentosH2H.CodigoMoneda] = dataPlanilla.CodigoMoneda;
                    row[ColumnasDocumentosH2H.ImporteOriginal] = numericValue / 100;
                    row[ColumnasDocumentosH2H.FechaVencimiento] = fechaVencimientoStr;
                    row[ColumnasDocumentosH2H.NumeroPlanilla] = dataPlanilla.NumeroPlanilla;
                    row[ColumnasDocumentosH2H.CodigoCliente] = line.Substring(406, 11).Trim();
                    row[ColumnasDocumentosH2H.FechaRegistro] = DateTime.Now;
                    row[ColumnasDocumentosH2H.Protestable] = dataParametroCtl.Where(p => p.ORDEN == 11).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.FlagCuota] = dataParametroCtl.Where(p => p.ORDEN == 12).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.FlagCompletado] = dataParametroCtl.Where(p => p.ORDEN == 13).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.NumeroCuotas] = dataParametroCtl.Where(p => p.ORDEN == 14).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.NumeroDocumentoAceptante] = line.Substring(150, 15).Trim();
                    row[ColumnasDocumentosH2H.RazonSocialAceptante] = line.Substring(513, 101).Trim();
                    row[ColumnasDocumentosH2H.TipoDocumentoAceptante] = line.Substring(510, 3).Trim();
                    row[ColumnasDocumentosH2H.CodigoTipoDocumentoCobranza] = line.Substring(481, 3).Trim();
                    row[ColumnasDocumentosH2H.CodigoTipoAbono] = line.Substring(484, 3).Trim();
                    row[ColumnasDocumentosH2H.CodigoTipoCuenta] = line.Substring(453, 3).Trim();
                    row[ColumnasDocumentosH2H.NumeroCuenta] = line.Substring(417, 20).Trim();
                    row[ColumnasDocumentosH2H.NumeroLinea] = line.Substring(437, 8).Trim();
                    row[ColumnasDocumentosH2H.FechaCargo] = DateTime.ParseExact(line.Substring(488, 9).Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(numeroinstruccion))
                    {
                        row[ColumnasDocumentosH2H.NumeroInstruccion] = numeroinstruccion.PadLeft(18, '0');
                    }
                    else
                    {
                        row[ColumnasDocumentosH2H.NumeroInstruccion] = DBNull.Value;
                    }
                    if (string.IsNullOrEmpty(fechaAdelantoStr) || fechaAdelantoStr.Length != 8)
                    {
                        row[ColumnasDocumentosH2H.FechaAdelanto] = DBNull.Value;
                    }
                    else
                    {
                        try
                        {
                            row[ColumnasDocumentosH2H.FechaAdelanto] = DateTime.ParseExact(fechaAdelantoStr, "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            row[ColumnasDocumentosH2H.FechaAdelanto] = DBNull.Value;
                        }
                    }
                    row[ColumnasDocumentosH2H.SaldoActualDocumento] = numericValue / 100;
                    row[ColumnasDocumentosH2H.CodigoTipoAdelanto] = Convert.ToInt32(line.Substring(506, 3).Trim());
                    row[ColumnasDocumentosH2H.FlagObservado] = dataParametroCtl.Where(p => p.ORDEN == 28).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.Observacion] = DBNull.Value;
                    row[ColumnasDocumentosH2H.ReglasValidacion] = DBNull.Value;
                    row[ColumnasDocumentosH2H.CodigoEstado] = dataParametroCtl.Where(p => p.ORDEN == 31).Select(p => p.ADICIONAL).FirstOrDefault();
                    row[ColumnasDocumentosH2H.AplicaPortes] = line.Substring(446, 2).Trim();
                    row[ColumnasDocumentosH2H.DiasAmpliacion] = line.Substring(448, 3).Trim();
                    row[ColumnasDocumentosH2H.ValidarCuentaCliente] = line.Substring(451, 1).Trim();
                    row[ColumnasDocumentosH2H.DesembolsoAutomatico] = line.Substring(452, 1).Trim();
                    row[ColumnasDocumentosH2H.PorcentajeProrroga] = Convert.ToDecimal(line.Substring(614, 21).Trim());
                    row[ColumnasDocumentosH2H.TipoCambioWDC] = Convert.ToDecimal(line.Substring(637).Trim());
                    dataTable.Rows.Add(row);
                    iSecuencias++;
                }
                _logger.LogInformation("DataTable para Documentos H2H cargada exitosamente con {RowCount} filas", dataTable.Rows.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer archivo y cargar DataTable para Documentos H2H: {Mensaje}", ex.Message);
                throw new ThrowException("36", ex.Message);
            }
        }

        public DataTable GeneraDatatableCalculoInteresComision(JObject parametrosJson, int codSecuencia)
        {
            DataTable tablaParametros = new DataTable();
            tablaParametros.Columns.Add("CodigoSecuencia", typeof(int));
            tablaParametros.Columns.Add("ImporteOriginal", typeof(decimal));
            tablaParametros.Columns.Add("AsumeInteres", typeof(int));
            tablaParametros.Columns.Add("CodigoMoneda", typeof(int));
            tablaParametros.Columns.Add("TasaClienteSoles", typeof(decimal));
            tablaParametros.Columns.Add("TasaClienteDolar", typeof(decimal));
            tablaParametros.Columns.Add("TasaSoles", typeof(decimal));
            tablaParametros.Columns.Add("TasaDolar", typeof(decimal));
            tablaParametros.Columns.Add("FechaCargo", typeof(DateTime));
            tablaParametros.Columns.Add("FechaDesembolso", typeof(DateTime));
            tablaParametros.Columns.Add("CoditoTipoDocumentoCobranza", typeof(int));
            tablaParametros.Columns.Add("CodigoUnicoProveedor", typeof(string));
            tablaParametros.Columns.Add("CodigoProducto", typeof(int));
            tablaParametros.Columns.Add("Portes", typeof(int));
            tablaParametros.Columns.Add("PlazaCuenta", typeof(int));
            tablaParametros.Columns.Add("NrocuentaPrimerosTresDigitos", typeof(int));
            tablaParametros.Columns.Add("codigotipocuenta", typeof(int));

            int maxCantidad = parametrosJson.Values().Max(v => ((JArray)v).Count);

            // Recorrer el objeto JSON de parámetros de entrada
            for (int i = 0; i < maxCantidad; i++)
            {
                DataRow dr = tablaParametros.NewRow();
                dr["CodigoSecuencia"] = codSecuencia;
                dr["ImporteOriginal"] = ObtenerValor(parametrosJson, "ImporteOriginal", i);
                dr["AsumeInteres"] = ObtenerValor(parametrosJson, "AsumeInteres", i);
                dr["CodigoMoneda"] = ObtenerValor(parametrosJson, "CodigoMoneda", i);
                dr["TasaClienteSoles"] = ObtenerValor(parametrosJson, "TasaClienteSoles", i);
                dr["TasaClienteDolar"] = ObtenerValor(parametrosJson, "TasaClienteDolar", i);
                dr["TasaSoles"] = ObtenerValor(parametrosJson, "TasaSoles", i);
                dr["TasaDolar"] = ObtenerValor(parametrosJson, "TasaDolar", i);
                dr["FechaCargo"] = ObtenerValor(parametrosJson, "FechaCargo", i);
                dr["FechaDesembolso"] = ObtenerValor(parametrosJson, "FechaDesembolso", i);
                dr["CoditoTipoDocumentoCobranza"] = ObtenerValor(parametrosJson, "CoditoTipoDocumentoCobranza", i);
                dr["CodigoUnicoProveedor"] = ObtenerValor(parametrosJson, "CodigoUnicoProveedor", i);
                dr["CodigoProducto"] = ObtenerValor(parametrosJson, "CodigoProducto", i);
                dr["Portes"] = ObtenerValor(parametrosJson, "Portes", i);
                dr["PlazaCuenta"] = ObtenerValor(parametrosJson, "PlazaCuenta", i);
                dr["NrocuentaPrimerosTresDigitos"] = ObtenerValor(parametrosJson, "NrocuentaPrimerosTresDigitos", i);
                dr["codigotipocuenta"] = ObtenerValor(parametrosJson, "codigotipocuenta", i);
                tablaParametros.Rows.Add(dr);
            }
            return tablaParametros;
        }

        public DataTable GeneraDataTableProcesoDetallePlanilla(IEnumerable<DetallePlanillasProcesadasResponse> detallesPlanilla)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("NumeroPlanilla", typeof(string));
            dataTable.Columns.Add("NumeroSecuenciaPlanilla", typeof(string));
            dataTable.Columns.Add("NumeroOperacion", typeof(string));
            dataTable.Columns.Add("NumeroSecuenciaOperacion", typeof(string));
            dataTable.Columns.Add("DescripcionMensajeError", typeof(string));
            dataTable.Columns.Add("NumeroDocumento", typeof(string));
            dataTable.Columns.Add("ImporteDesembolso", typeof(string));
            dataTable.Columns.Add("CodigoFlagExterno", typeof(string));
            dataTable.Columns.Add("NumeroLogExterno", typeof(string));
            dataTable.Columns.Add("ImporteComisionCCI", typeof(string));
            dataTable.Columns.Add("ImporteComisionIB", typeof(string));
            dataTable.Columns.Add("ImporteDesembolsoCCI", typeof(string));
            dataTable.Columns.Add("CodigoRetorno", typeof(string));

            foreach (var rowDetalle in detallesPlanilla)
            {
                if (rowDetalle == null) continue;

                DataRow dataRow = dataTable.NewRow();
                dataRow["NumeroPlanilla"] = rowDetalle.NumeroPlanilla?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["NumeroSecuenciaPlanilla"] = rowDetalle.NumeroSecuenciaPlanilla.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["NumeroOperacion"] = rowDetalle.NumeroOperacion?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["NumeroSecuenciaOperacion"] = rowDetalle.NumeroSecuenciaOperacion?.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["DescripcionMensajeError"] = fstrFormatString(rowDetalle.DescripcionMensajeError ?? string.Empty).Trim().PadRight(40, ' ').Substring(0, 40);
                dataRow["NumeroDocumento"] = rowDetalle.NumeroDocumento?.ToString().Trim().PadLeft(10, '0') ?? ProcesarTramasConstants.ImporteCero10;
                dataRow["ImporteDesembolso"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteDesembolso?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["CodigoFlagExterno"] = rowDetalle.CodigoFlagExterno?.ToString().Trim().PadLeft(1, '0') ?? "0";
                dataRow["NumeroLogExterno"] = rowDetalle.NumeroLogExterno?.ToString().Trim().PadLeft(7, '0') ?? ProcesarTramasConstants.ImporteCero7;
                dataRow["ImporteComisionCCI"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteComisionCCI?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["ImporteComisionIB"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteComisionIB?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["ImporteDesembolsoCCI"] = ConvertCommaToDotForDecimal(rowDetalle.ImporteDesembolsoCCI?.ToString().Trim().PadLeft(15, '0') ?? ProcesarTramasConstants.ImporteCero15);
                dataRow["CodigoRetorno"] = rowDetalle.CodigoEstadoPago?.ToString().Trim().PadLeft(2, '0') ?? "00";

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private static string ObtenerValor(JObject parametrosJson, string parametro, int indice)
        {
            if (parametrosJson[parametro] != null)
            {
                var array = (JArray)parametrosJson[parametro]!;
                if (indice < array.Count)
                {
                    return array[indice].ToString();
                }
            }
            return "";
        }

        private static string fstrFormatString(string pstrValor)
        {
            string strReturn;
            try
            {
                strReturn = pstrValor.Replace("&#x0;", " ");
                return strReturn;
            }

            catch (Exception)
            {
                return " ";
            }
        }

        private static string ConvertCommaToDotForDecimal(string input)
        {
            string result;
            try
            {
                result = input.Replace(',', '.');
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
