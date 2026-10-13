//using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
//using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DataTable;
//using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
//using Interbank.Productos.Comercial.Fcd.Domain.Entities.OracleCustomTypeMapping;
//using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
//using Microsoft.Extensions.Logging;
//using NextSIT.Utility;
//using Oracle.ManagedDataAccess.Client;
//using System.Data;
//using System.Data.Common;

//namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
//{
//    public class UtilitariosRepository : IUtilitariosRepository
//    {
//        private readonly ITypeConvertionManager _typeConversionManager;
//        private readonly ILogger<UtilitariosRepository> _logger;
//        private readonly IOracleConnectionFactory _oracleConnectionFactory;
//        private readonly IDapperExecutor _dapperExecutor;
//        private readonly IOracleBulkCopyExecutor _bulkCopyExecutor;

//        public UtilitariosRepository(
//            ITypeConvertionManager typeConversionManager,
//            ILogger<UtilitariosRepository> logger,
//            IOracleConnectionFactory oracleConnectionFactory,
//            IDapperExecutor dapperExecutor,
//            IOracleBulkCopyExecutor bulkCopyExecutor)
//        {
//            ArgumentNullException.ThrowIfNull(typeConversionManager);
//            ArgumentNullException.ThrowIfNull(logger);
//            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
//            ArgumentNullException.ThrowIfNull(dapperExecutor);
//            ArgumentNullException.ThrowIfNull(bulkCopyExecutor);

//            _typeConversionManager = typeConversionManager;
//            _logger = logger;
//            _oracleConnectionFactory = oracleConnectionFactory;
//            _dapperExecutor = dapperExecutor;
//            _bulkCopyExecutor = bulkCopyExecutor;
//        }

//        public async Task<List<DapperParametroDB2>> ObtenerParametrosDB2PorCodigoDominioDB2(int codigoDominio, int numOrden)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("PIV_CODIGODOMINIODB2", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
//                parameters.Add("PIV_NUMORDEN", OracleDbType.Int64, ParameterDirection.Input, numOrden);
//                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
//                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerListadoParametrosDB2PorCodigoDominioDB2, parameters, commandType: CommandType.StoredProcedure);

//                var ListadoParametro = result.Select(x => new DapperParametroDB2
//                {
//                    CodigoParametroDB2 = _typeConversionManager.AnyToInteger(x.CODIGOPARAMETRODB2),
//                    CodigoDominioDB2 = _typeConversionManager.AnyToInteger(x.CODIGODOMINIODB2),
//                    Descripcion = _typeConversionManager.AnyToString(x.DESCRIPCION),
//                    Script = _typeConversionManager.AnyToString(x.SCRIPT),
//                    Estado = _typeConversionManager.AnyToInteger(x.ESTADO),
//                    NumOrden = _typeConversionManager.AnyToInteger(x.NUMORDEN)
//                }).ToList();

//                return ListadoParametro;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ObtenerParametrosDB2PorCodigoDominioDB2 - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        public async Task<List<DapperParametro>> ObtenerParametrosPorCodigoDominio(int codigoDominio)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("PIV_CODIGODOMINIO", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
//                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
//                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerListadoParametrosPorCodigoDominio, parameters, commandType: CommandType.StoredProcedure);
//                var ListadoParametro = result.Select(x => new DapperParametro
//                {

//                    CODIGOPARAMETRO = _typeConversionManager.AnyToInteger(x.CODIGOPARAMETRO),
//                    CODIGODOMINIO = _typeConversionManager.AnyToInteger(x.CODIGODOMINIO),
//                    DESCRIPCION = _typeConversionManager.AnyToString(x.DESCRIPCION),
//                    ESTADO = _typeConversionManager.AnyToInteger(x.ESTADO),
//                    CODIGOREFERENCIA = _typeConversionManager.AnyToString(x.CODIGOREFERENCIA),
//                    NUMEROORDEN = _typeConversionManager.AnyToInteger(x.NUMEROORDEN),
//                    DESCRIPCIONCORTA = _typeConversionManager.AnyToString(x.DESCRIPCIONCORTA),
//                    CODIGOREFERENCIAPARAMETRO = _typeConversionManager.AnyToInteger(x.CODIGOREFERENCIAPARAMETRO),
//                    OPCION1 = _typeConversionManager.AnyToString(x.OPCION1),
//                }).ToList();

//                return ListadoParametro;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ObtenerParametrosPorCodigoDominio - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        public bool BulkInsertTable(DataTable dt, string nombreTabla)
//        {
//            using var cn = _oracleConnectionFactory.CrearConexionBaseDatos();
//            cn.Open();

//            using var transaction = cn.BeginTransaction();

//            try
//            {
//                _bulkCopyExecutor.WriteToServer(cn, dt, nombreTabla);

//                transaction.Commit();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                transaction.Rollback();
//                _logger.LogError(ex, "BulkInsertTable - Error en base de datos");
//                throw new InvalidOperationException("Error en base de datos", ex);
//            }
//        }

//        public async Task<bool> ForAllInsertDataObject(DataTable dt, string nombreTabla)
//        {
//            try
//            {
//                // 1. DataTable a Lista (Lógica de negocio)
//                var documentos = ConvertirDataTableADocumentos(dt);
//                var coleccion = new CabDocumentosH2H(documentos);

//                // 2. Conexión estándar
//                using var cn = _oracleConnectionFactory.CrearConexionBaseDatos();
//                await ((DbConnection)cn).OpenAsync();

//                // 3. Ejecución delegada (Lógica de infraestructura)
//                await _dapperExecutor.ExecuteForAllInsertAsync(cn, coleccion, nombreTabla);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ForAllInsertDataObject - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        private static DocumentoH2H[] ConvertirDataTableADocumentos(DataTable dt)
//        {
//            return dt.AsEnumerable().Select(row => new DocumentoH2H
//            {

//                NumeroInterno = row.Field<string>(ColumnasDocumentosH2H.NumeroInterno),
//                Item = row.Field<int?>(ColumnasDocumentosH2H.Item),
//                CodigoClientePlanilla = row.Field<int?>(ColumnasDocumentosH2H.CodigoClientePlanilla),
//                NumeroDocumentoFisico = row.Field<string>(ColumnasDocumentosH2H.NumeroDocumentoFisico),
//                CodigoMoneda = row.Field<int?>(ColumnasDocumentosH2H.CodigoMoneda),
//                ImporteOriginal = row.Field<decimal?>(ColumnasDocumentosH2H.ImporteOriginal),
//                FechaVencimiento = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaVencimiento),
//                NumeroPlanilla = row.Field<string>(ColumnasDocumentosH2H.NumeroPlanilla),
//                CodigoCliente = row.Field<int>(ColumnasDocumentosH2H.CodigoCliente),
//                FechaRegistro = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaRegistro),
//                Protestable = row.Field<int?>(ColumnasDocumentosH2H.Protestable),
//                FlagCuota = row.Field<int?>(ColumnasDocumentosH2H.FlagCuota),
//                FlagCompletado = row.Field<int?>(ColumnasDocumentosH2H.FlagCompletado),
//                NumeroCuotas = row.Field<int?>(ColumnasDocumentosH2H.NumeroCuotas),
//                NumeroDocumentoAceptante = row.Field<string>(ColumnasDocumentosH2H.NumeroDocumentoAceptante),
//                RazonSocialAceptante = row.Field<string>(ColumnasDocumentosH2H.RazonSocialAceptante),
//                TipoDocumentoAceptante = row.Field<int?>(ColumnasDocumentosH2H.TipoDocumentoAceptante),
//                CodigoTipoDocumentoCobranza = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoDocumentoCobranza),
//                CodigoTipoAbono = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoAbono),
//                CodigoTipoCuenta = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoCuenta),
//                NumeroCuenta = row.Field<string>(ColumnasDocumentosH2H.NumeroCuenta),
//                NumeroLinea = row.Field<string>(ColumnasDocumentosH2H.NumeroLinea),
//                FechaCargo = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaCargo),
//                NumeroInstruccion = row.Field<string>(ColumnasDocumentosH2H.NumeroInstruccion),
//                FechaAdelanto = row.Field<DateTime?>(ColumnasDocumentosH2H.FechaAdelanto),
//                SaldoActualDocumento = row.Field<decimal?>(ColumnasDocumentosH2H.SaldoActualDocumento),
//                CodigoTipoAdelanto = row.Field<int?>(ColumnasDocumentosH2H.CodigoTipoAdelanto),
//                FlagObservado = row.Field<int?>(ColumnasDocumentosH2H.FlagObservado),
//                Observacion = row.Field<string>(ColumnasDocumentosH2H.Observacion),
//                ReglasValidacion = row.Field<string>(ColumnasDocumentosH2H.ReglasValidacion),
//                CodigoEstado = row.Field<int?>(ColumnasDocumentosH2H.CodigoEstado),
//                AplicaPortes = row.Field<int?>(ColumnasDocumentosH2H.AplicaPortes),
//                DiasAmpliacion = row.Field<int?>(ColumnasDocumentosH2H.DiasAmpliacion),
//                ValidarCuentaCliente = row.Field<int?>(ColumnasDocumentosH2H.ValidarCuentaCliente),
//                DesembolsoAutomatico = row.Field<int?>(ColumnasDocumentosH2H.DesembolsoAutomatico),
//                PorcentajeProrroga = row.Field<decimal?>(ColumnasDocumentosH2H.PorcentajeProrroga),
//                TipoCambioWDC = row.Field<decimal?>(ColumnasDocumentosH2H.TipoCambioWDC),
//            }).ToArray();
//        }

//        public async Task ActualizarParametroPorCodigoDominioAndNumOrden(int codigoDominio, string descripcionCorta, int numOrden)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("PIV_CODIGODOMINIO", OracleDbType.Int64, ParameterDirection.Input, codigoDominio);
//                parameters.Add("PIV_DESCRIPCIONCORTA", OracleDbType.Varchar2, ParameterDirection.Input, descripcionCorta);
//                parameters.Add("PIV_NUMEROORDEN", OracleDbType.Int64, ParameterDirection.Input, numOrden);

//                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.ActualizaParametroPorDominioAndNumOrden, parameters, commandType: CommandType.StoredProcedure);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ActualizarParametroPorCodigoDominioAndNumOrden - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }
//    }
//}

using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using NextSIT.Utility;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    // SOLO PRUEBAS LOCALES: todas las operaciones sin Oracle.
    public class UtilitariosRepository : IUtilitariosRepository
    {
        private readonly ILogger<UtilitariosRepository> _logger;

        // Selecciona ERROR por método, sin alterar los LogError originales.
        public static bool MockError = false;
        public static string MockMetodoError = "TODOS";
        // Solo ForAllInsertDataObject y BulkInsertTable: false simula resultado negativo
        // sin excepción; el método BulkInsertTable real únicamente retorna true o lanza.
        public static bool MockForAllResultado = true;
        // Opcional: configura respuestas DB2 para casos de negocio; si no, lista vacía.
        public static List<DapperParametroDB2> MockParametrosDB2 = new();

        // CSV real de parámetros en docs/PARAM01.csv (no colocar credenciales en el código).
        public static string MockNombreArchivoParametros = "PARAM01.csv";

        public UtilitariosRepository(
            ITypeConvertionManager typeConversionManager,
            ILogger<UtilitariosRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor,
            IOracleBulkCopyExecutor bulkCopyExecutor)
        {
            ArgumentNullException.ThrowIfNull(typeConversionManager);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);
            ArgumentNullException.ThrowIfNull(bulkCopyExecutor);
            _logger = logger;
        }

        private static bool DebeSimularError(string metodo) =>
            MockError && (string.Equals(MockMetodoError, "TODOS", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(MockMetodoError, metodo, StringComparison.OrdinalIgnoreCase));

        private static void ProvocarError(string metodo)
        {
            if (DebeSimularError(metodo))
                throw new InvalidOperationException("Error simulado en base de datos Oracle");
        }

        public Task<List<DapperParametroDB2>> ObtenerParametrosDB2PorCodigoDominioDB2(int codigoDominio, int numOrden)
        {
            try
            {
                ProvocarError(nameof(ObtenerParametrosDB2PorCodigoDominioDB2));
                var resultado = MockParametrosDB2
                    .Where(p => p.CodigoDominioDB2 == codigoDominio && p.NumOrden == numOrden)
                    .ToList();
                return Task.FromResult(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerParametrosDB2PorCodigoDominioDB2 - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public Task<List<DapperParametro>> ObtenerParametrosPorCodigoDominio(int codigoDominio)
        {
            try
            {
                ProvocarError(nameof(ObtenerParametrosPorCodigoDominio));
                string archivo = Path.Combine(EncontrarRaizProyecto(), "docs", MockNombreArchivoParametros);
                var lista = LeerParametrosCSV(archivo)
                    .Where(p => p.CODIGODOMINIO == codigoDominio)
                    .ToList();
                return Task.FromResult(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerParametrosPorCodigoDominio - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public bool BulkInsertTable(DataTable dt, string nombreTabla)
        {
            // Originalmente, el try/catch rodeaba la escritura y Commit,
            // mientras que la apertura de conexión estaba fuera del try.
            // En el mock no se abre conexión ni transacción.
            try
            {
                ProvocarError(nameof(BulkInsertTable));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BulkInsertTable - Error en base de datos");
                throw new InvalidOperationException("Error en base de datos", ex);
            }
        }

        public Task<bool> ForAllInsertDataObject(DataTable dt, string nombreTabla)
        {
            try
            {
                ProvocarError(nameof(ForAllInsertDataObject));
                // No crea conexión Oracle ni inserta documentos. Sin validar datos.
                return Task.FromResult(MockForAllResultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForAllInsertDataObject - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public Task ActualizarParametroPorCodigoDominioAndNumOrden(int codigoDominio, string descripcionCorta, int numOrden)
        {
            try
            {
                ProvocarError(nameof(ActualizarParametroPorCodigoDominioAndNumOrden));
                // No altera PARAM01.csv ni datos Oracle.
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActualizarParametroPorCodigoDominioAndNumOrden - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        private static string EncontrarRaizProyecto()
        {
            DirectoryInfo? actual = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (actual != null)
            {
                if (string.Equals(actual.Name, "fcd-servicios-h2h-service", StringComparison.OrdinalIgnoreCase))
                    return actual.FullName;

                string hijo = Path.Combine(actual.FullName, "fcd-servicios-h2h-service");
                if (Directory.Exists(Path.Combine(hijo, "docs")))
                    return hijo;

                actual = actual.Parent;
            }
            throw new DirectoryNotFoundException("No se encontró la raíz fcd-servicios-h2h-service desde el directorio de trabajo actual.");
        }

        private static List<DapperParametro> LeerParametrosCSV(string archivo)
        {
            if (!File.Exists(archivo))
                throw new FileNotFoundException("No se encontró el archivo local de parámetros.", archivo);

            var lista = new List<DapperParametro>();
            using var lector = new TextFieldParser(archivo);
            lector.TextFieldType = FieldType.Delimited;
            lector.SetDelimiters(";");
            lector.HasFieldsEnclosedInQuotes = true;
            var encabezado = lector.ReadFields();
            if (encabezado == null || encabezado.Length != 9 ||
                encabezado[0] != "CODIGOPARAMETRO" || encabezado[6] != "DESCRIPCIONCORTA")
                throw new InvalidDataException("El formato de PARAM01.csv no coincide con el esperado.");

            while (!lector.EndOfData)
            {
                var c = lector.ReadFields();
                if (c == null || c.Length != 9)
                    throw new InvalidDataException("Registro de parámetros CSV inválido.");

                static int Entero(string? valor) => int.TryParse(valor, out var n) ? n : 0;
                static string? Texto(string? valor) => string.IsNullOrEmpty(valor) ? null : valor;

                lista.Add(new DapperParametro
                {
                    CODIGOPARAMETRO = Entero(c[0]),
                    CODIGODOMINIO = Entero(c[1]),
                    DESCRIPCION = Texto(c[2]),
                    ESTADO = Entero(c[3]),
                    CODIGOREFERENCIA = Texto(c[4]),
                    NUMEROORDEN = Entero(c[5]),
                    DESCRIPCIONCORTA = Texto(c[6]),
                    CODIGOREFERENCIAPARAMETRO = Entero(c[7]),
                    OPCION1 = Texto(c[8])
                });
            }
            return lista;
        }
    }
}
