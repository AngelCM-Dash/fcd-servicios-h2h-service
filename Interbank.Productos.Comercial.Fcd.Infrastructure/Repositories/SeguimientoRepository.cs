//using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
//using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
//using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
//using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
//using Microsoft.Extensions.Logging;
//using NextSIT.Utility;
//using Oracle.ManagedDataAccess.Client;
//using System.Data;

//namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
//{
//    public class SeguimientoRepository : ISeguimientoRepository
//    {
//        private readonly ITypeConvertionManager _typeConvertionManager;
//        private readonly ILogger<SeguimientoRepository> _logger;
//        private readonly IOracleConnectionFactory _oracleConnectionFactory;
//        private readonly IDapperExecutor _dapperExecutor;

//        public SeguimientoRepository(
//            ITypeConvertionManager typeConvertionManager,
//            ILogger<SeguimientoRepository> logger,
//            IOracleConnectionFactory oracleConnectionFactory,
//            IDapperExecutor dapperExecutor)
//        {
//            ArgumentNullException.ThrowIfNull(typeConvertionManager);
//            ArgumentNullException.ThrowIfNull(logger);
//            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
//            ArgumentNullException.ThrowIfNull(dapperExecutor);

//            _typeConvertionManager = typeConvertionManager;
//            _logger = logger;
//            _oracleConnectionFactory = oracleConnectionFactory;
//            _dapperExecutor = dapperExecutor;
//        }

//        public async Task<decimal> InsertaSeguimientoCabecera(DapperSeguimientoCabecera command)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("p_IdDetalle", OracleDbType.Decimal, ParameterDirection.Input, command.IdDetalle);
//                parameters.Add("p_NombreArchivo", OracleDbType.Varchar2, ParameterDirection.Input, command.NombreArchivo);
//                parameters.Add("p_NroPlanilla", OracleDbType.Varchar2, ParameterDirection.Input, command.NroPlanilla);
//                parameters.Add("p_Flag", OracleDbType.Varchar2, ParameterDirection.Input, command.Flag);
//                parameters.Add(OracleParameterNames.PResultado, OracleDbType.Decimal, ParameterDirection.Output, 0);

//                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaSeguimientoCabH2HW, parameters, commandType: CommandType.StoredProcedure);

//                var codigoRespuesta = parameters.Get<decimal>(OracleParameterNames.PResultado);

//                return codigoRespuesta;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "InsertaSeguimientoCabecera - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        public async Task<decimal> InsertaSeguimientoDetalle(DapperSeguimientoDetalle command)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("p_IdDetalle", OracleDbType.Decimal, ParameterDirection.Input, command.IdDetalle);
//                parameters.Add("p_NombreMetodo", OracleDbType.Varchar2, ParameterDirection.Input, command.NombreMetodo);
//                parameters.Add("p_CapaObservacion", OracleDbType.Varchar2, ParameterDirection.Input, command.CapaObservacion);
//                parameters.Add("p_DetalleObservacion", OracleDbType.Varchar2, ParameterDirection.Input, command.DetalleObservacion);
//                parameters.Add("p_IdEstacion", OracleDbType.Decimal, ParameterDirection.Input, command.IdEstacion);
//                parameters.Add(OracleParameterNames.PResultado, OracleDbType.Decimal, ParameterDirection.Output, 0);

//                await _dapperExecutor.ExecuteAsync(connection, OracleProcedures.InsertaSeguimientoDetH2HW, parameters, commandType: CommandType.StoredProcedure);

//                var codigoRespuesta = parameters.Get<decimal>(OracleParameterNames.PResultado);
//                return codigoRespuesta;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "InsertaSeguimientoDetalle - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        public async Task<int> ObtenerIdSeguimientoxPlanilla(string numeroPlanilla)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
//                parameters.Add(OracleParameterNames.PocurCursor, OracleDbType.RefCursor, ParameterDirection.Output);
//                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerSeguimientoIDxPlanilla, parameters, commandType: CommandType.StoredProcedure);

//                var firstItem = result.FirstOrDefault();
//                return firstItem != null
//                    ? _typeConvertionManager.AnyToInteger(firstItem.IDDETALLE)
//                    : 0;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ObtenerIdSeguimientoxPlanilla - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }

//        }

//        public async Task<List<DapperSeguimientoDetalleCabecera>> ObtenerSeguimientoDetalleCabecera(string? fechaDesde, string? fechaHasta, string? nombreArchivo, string? numeroPlanilla)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("PIV_FECHADESDE", OracleDbType.Varchar2, ParameterDirection.Input, fechaDesde);
//                parameters.Add("PIV_FECHAHASTA", OracleDbType.Varchar2, ParameterDirection.Input, fechaHasta);
//                parameters.Add("PIV_NOMBREARCHIVO", OracleDbType.Varchar2, ParameterDirection.Input, nombreArchivo);
//                parameters.Add(OracleParameterNames.PivNumeroPlanilla, OracleDbType.Varchar2, ParameterDirection.Input, numeroPlanilla);
//                parameters.Add(OracleParameterNames.PocurResultado, OracleDbType.RefCursor, ParameterDirection.Output);
//                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerDetalleCabecera, parameters, commandType: CommandType.StoredProcedure);
//                var listadoDetCab = result.Select(x => new DapperSeguimientoDetalleCabecera
//                {
//                    IdSeguimiento = _typeConvertionManager.AnyToDecimal(x.IDSEGUIMIENTO),
//                    IdDetalle = _typeConvertionManager.AnyToDecimal(x.IDDETALLE),
//                    FechaRegistro = _typeConvertionManager.AnyToString(x.FECHAREGISTRO),
//                    NombreArchivo = _typeConvertionManager.AnyToString(x.NOMBREARCHIVO),
//                    NroPlanilla = _typeConvertionManager.AnyToString(x.NROPLANILLA),
//                    NombreMetodo = _typeConvertionManager.AnyToString(x.NOMBREMETODO),
//                    CapaObservacion = _typeConvertionManager.AnyToString(x.CAPAOBSERVACION),
//                    DetalleObservacion = _typeConvertionManager.AnyToString(x.DETALLEOBSERVACION),
//                    IdEstacion = _typeConvertionManager.AnyToDecimal(x.IDESTACION),
//                }).ToList();
//                return listadoDetCab;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ObtenerSeguimientoDetalleCabecera - Error en base de datos");
//                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
//            }
//        }

//        public async Task<string> ObtenerTipoEjecucionSeguimientoxIdDetalle(decimal idDetalle)
//        {
//            try
//            {
//                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
//                var parameters = new OracleDynamicParameters();
//                parameters.Add("PIV_IDDETALLE", OracleDbType.Decimal, ParameterDirection.Input, idDetalle);
//                parameters.Add(OracleParameterNames.PocurCursor, OracleDbType.RefCursor, ParameterDirection.Output);
//                var result = await _dapperExecutor.QueryAsync(connection, OracleProcedures.ObtenerTipoEjecucionxIdDetalle, parameters, commandType: CommandType.StoredProcedure);
//                var TipoEjecucion = Convert.ToString(result.Select(x => x.TIPOEJECUCION).FirstOrDefault());
//                return TipoEjecucion;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ObtenerTipoEjecucionSeguimientoxIdDetalle - Error en base de datos");
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    // MOCK TEMPORAL: sin Oracle. Secuencias compartidas en docs/secuencias_mock.json.
    public class SeguimientoRepository : ISeguimientoRepository
    {
        private readonly ITypeConvertionManager _typeConvertionManager;
        private readonly ILogger<SeguimientoRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        // Configurar antes de las pruebas; no cambiar durante peticiones concurrentes.
        public static bool MockError = false;
        public static string MockMetodoError = "TODOS";

        private static readonly object Sync = new object();
        private static readonly Dictionary<decimal, CabeceraMock> Cabeceras = new Dictionary<decimal, CabeceraMock>();
        private static readonly List<DetalleMock> Detalles = new List<DetalleMock>();

        private sealed class CabeceraMock
        {
            public decimal IdDetalle { get; set; }
            public string NombreArchivo { get; set; } = "";
            public string NroPlanilla { get; set; } = "";
            public string TipoEjecucion { get; set; } = "";
            public DateTime FechaRegistro { get; set; }
        }

        private sealed class DetalleMock
        {
            public decimal IdSeguimiento { get; set; }
            public decimal IdDetalle { get; set; }
            public string NombreMetodo { get; set; } = "";
            public string CapaObservacion { get; set; } = "";
            public string DetalleObservacion { get; set; } = "";
            public decimal IdEstacion { get; set; }
            public DateTime FechaRegistro { get; set; }
        }

        public SeguimientoRepository(
            ITypeConvertionManager typeConvertionManager,
            ILogger<SeguimientoRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor)
        {
            ArgumentNullException.ThrowIfNull(typeConvertionManager);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);

            _typeConvertionManager = typeConvertionManager;
            _logger = logger;
            _oracleConnectionFactory = oracleConnectionFactory;
            _dapperExecutor = dapperExecutor;
        }

        private static bool DebeSimularError(string metodo) =>
            MockError &&
            (string.Equals(MockMetodoError, "TODOS", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(MockMetodoError, metodo, StringComparison.OrdinalIgnoreCase));

        private static void FallarSiCorresponde(string metodo)
        {
            if (DebeSimularError(metodo))
                throw new InvalidOperationException("Error simulado en base de datos Oracle");
        }

        public Task<decimal> InsertaSeguimientoCabecera(DapperSeguimientoCabecera command)
        {
            try
            {
                FallarSiCorresponde(nameof(InsertaSeguimientoCabecera));
                ArgumentNullException.ThrowIfNull(command);
                lock (Sync)
                {
                    decimal id = command.IdDetalle > 0 ? command.IdDetalle : MockSecuenciaService.Siguiente("CodigoDetalle");

                    if (!Cabeceras.TryGetValue(id, out var registro))
                    {
                        registro = new CabeceraMock { IdDetalle = id, FechaRegistro = DateTime.Now };
                        Cabeceras[id] = registro;
                    }
                    registro.NombreArchivo = Convert.ToString(command.NombreArchivo) ?? "";
                    // Una actualización puede traer datos vacíos: conservar lo registrado.
                    string nro = Convert.ToString(command.NroPlanilla) ?? "";
                    if (!string.IsNullOrWhiteSpace(nro)) registro.NroPlanilla = nro;
                    registro.TipoEjecucion = Convert.ToString(command.Flag) ?? "";
                    return Task.FromResult(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertaSeguimientoCabecera - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public Task<decimal> InsertaSeguimientoDetalle(DapperSeguimientoDetalle command)
        {
            try
            {
                FallarSiCorresponde(nameof(InsertaSeguimientoDetalle));
                ArgumentNullException.ThrowIfNull(command);
                lock (Sync)
                {
                    decimal id = MockSecuenciaService.Siguiente("CodigoSeguimiento");
                    Detalles.Add(new DetalleMock
                    {
                        IdSeguimiento = id,
                        IdDetalle = command.IdDetalle,
                        NombreMetodo = Convert.ToString(command.NombreMetodo) ?? "",
                        CapaObservacion = Convert.ToString(command.CapaObservacion) ?? "",
                        DetalleObservacion = Convert.ToString(command.DetalleObservacion) ?? "",
                        IdEstacion = Convert.ToDecimal(command.IdEstacion),
                        FechaRegistro = DateTime.Now
                    });
                    return Task.FromResult(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertaSeguimientoDetalle - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public Task<int> ObtenerIdSeguimientoxPlanilla(string numeroPlanilla)
        {
            try
            {
                FallarSiCorresponde(nameof(ObtenerIdSeguimientoxPlanilla));
                lock (Sync)
                {
                    decimal id = Cabeceras.Values
                        .Where(x => x.NroPlanilla == numeroPlanilla)
                        .OrderByDescending(x => x.FechaRegistro)
                        .Select(x => x.IdDetalle)
                        .FirstOrDefault();
                    return Task.FromResult(id == 0 ? 0 : checked((int)id));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerIdSeguimientoxPlanilla - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public Task<List<DapperSeguimientoDetalleCabecera>> ObtenerSeguimientoDetalleCabecera(
            string? fechaDesde, string? fechaHasta, string? nombreArchivo, string? numeroPlanilla)
        {
            try
            {
                FallarSiCorresponde(nameof(ObtenerSeguimientoDetalleCabecera));
                // Filtros de fechas opcionales en formato dd/MM/yyyy o yyyy-MM-dd.
                DateTime? desde = ParseFechaMock(fechaDesde);
                DateTime? hasta = ParseFechaMock(fechaHasta);
                lock (Sync)
                {
                    var consulta = from detalle in Detalles
                                   join cab in Cabeceras.Values on detalle.IdDetalle equals cab.IdDetalle
                                   where (string.IsNullOrWhiteSpace(nombreArchivo) || cab.NombreArchivo == nombreArchivo)
                                      && (string.IsNullOrWhiteSpace(numeroPlanilla) || cab.NroPlanilla == numeroPlanilla)
                                      && (!desde.HasValue || detalle.FechaRegistro.Date >= desde.Value.Date)
                                      && (!hasta.HasValue || detalle.FechaRegistro.Date <= hasta.Value.Date)
                                   orderby detalle.IdSeguimiento
                                   select new DapperSeguimientoDetalleCabecera
                                   {
                                       IdSeguimiento = detalle.IdSeguimiento,
                                       IdDetalle = detalle.IdDetalle,
                                       FechaRegistro = detalle.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                       NombreArchivo = cab.NombreArchivo,
                                       NroPlanilla = cab.NroPlanilla,
                                       NombreMetodo = detalle.NombreMetodo,
                                       CapaObservacion = detalle.CapaObservacion,
                                       DetalleObservacion = detalle.DetalleObservacion,
                                       IdEstacion = detalle.IdEstacion
                                   };
                    return Task.FromResult(consulta.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerSeguimientoDetalleCabecera - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        private static DateTime? ParseFechaMock(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            string[] formatos = { "dd/MM/yyyy", "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss" };
            if (DateTime.TryParseExact(valor.Trim(), formatos, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var fecha)) return fecha;
            throw new FormatException("Fecha de filtro no válida: " + valor);
        }

        public Task<string> ObtenerTipoEjecucionSeguimientoxIdDetalle(decimal idDetalle)
        {
            try
            {
                FallarSiCorresponde(nameof(ObtenerTipoEjecucionSeguimientoxIdDetalle));
                lock (Sync)
                {
                    return Task.FromResult(Cabeceras.TryGetValue(idDetalle, out var registro)
                        ? registro.TipoEjecucion
                        : "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerTipoEjecucionSeguimientoxIdDetalle - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}

