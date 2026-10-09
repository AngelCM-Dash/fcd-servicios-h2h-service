using Dapper;
using IBM.Data.Db2;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    public class Db2Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DesembolsoController> _logger;

        public Db2Controller(IMediator mediator, ILogger<DesembolsoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("TestConnection")]
        public IActionResult TestConnection([FromBody] string ConnectionString)
        {
            _logger.LogInformation("Iniciando prueba de conexión a Db2 con la cadena de conexion {CadenaConexion}", ConnectionString);

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                _logger.LogWarning("La cadena de conexión está vacía o nula.");
                return BadRequest(new { success = false, message = "Cadena de conexión no proporcionada." });
            }

            try
            {
                _logger.LogInformation("Intentando abrir conexión a Db2...");
                using (var connection = new DB2Connection(ConnectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Conexión exitosa a Db2.");
                    return Ok(new { exito = true, mensaje = "Conexión exitosa a DB2." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar conectarse a Db2.");
                return BadRequest(new
                {
                    success = false,
                    message = "Error al conectar con DB2.",
                    error = ex.Message
                });
            }
            finally
            {
                _logger.LogInformation("Finalizó la prueba de conexión a Db2.");
            }
        }

        [HttpGet("DetallePlanillas")]
        public async Task<IActionResult> DetallePlanillas(string? numeroPlanilla, string? estacion, string? ruta)
        {
            DataTable result = new DataTable();
            var results = new List<Object>();

            string sqlQuery = "";
            string connectionString = "";

            if (estacion == "A")
            {
                connectionString = $"Server=130.30.30.6:5022;Database=DBP0;UID=AFCDUSR1;PWD=AUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = $"Select Tra.NU_PLANILLA,Tra.NU_SEC_PLA,Tra.NU_OPERA,Tra.NU_SEC_OPER,Tra.CO_COD_RET,IfNull(Log.DE_DES_MSG_ERR_LOG, ' ') " +
                        $"as DE_DES_MSG_ERR_LOG,Tra.NU_NRO_DOCUM,Tra.IM_IMP_DESEM,Tra.CO_FLAG_EXT_RE,Tra.NU_LOG_EXT,Tra.IM_IMP_COM_CCI,Tra.IM_IMP_COM_IB," +
                        $"Tra.IM_IMP_DES_CCI,Tra.CO_STAT_PAGO,Tra.CO_ENVIO_CORREO from A.FCD_PAGO_MASI_MAE Tra Left Join A.FCD_TABL_PLAN_LOG Log ON " +
                        $"Log.NU_PLANILLA_LOG  = Tra.NU_PLANILLA And Log.NU_SEC_PLA_LOG = Tra.NU_SEC_PLA And Log.NU_OPERA_LOG = " +
                        $"Tra.NU_OPERA And Log.NU_SEC_OPER_LOG = Tra.NU_SEC_OPER";
                }
                else
                {
                    sqlQuery = $"Select Tra.NU_PLANILLA,Tra.NU_SEC_PLA,Tra.NU_OPERA,Tra.NU_SEC_OPER,Tra.CO_COD_RET,IfNull(Log.DE_DES_MSG_ERR_LOG, ' ') " +
                                $"as DE_DES_MSG_ERR_LOG,Tra.NU_NRO_DOCUM,Tra.IM_IMP_DESEM,Tra.CO_FLAG_EXT_RE,Tra.NU_LOG_EXT,Tra.IM_IMP_COM_CCI,Tra.IM_IMP_COM_IB," +
                                $"Tra.IM_IMP_DES_CCI,Tra.CO_STAT_PAGO,Tra.CO_ENVIO_CORREO from A.FCD_PAGO_MASI_MAE Tra Left Join A.FCD_TABL_PLAN_LOG Log ON " +
                                $"Log.NU_PLANILLA_LOG  = Tra.NU_PLANILLA And Log.NU_SEC_PLA_LOG = Tra.NU_SEC_PLA And Log.NU_OPERA_LOG = " +
                                $"Tra.NU_OPERA And Log.NU_SEC_OPER_LOG = Tra.NU_SEC_OPER  Where Tra.NU_PLANILLA = '{numeroPlanilla}' and Tra.NU_SEC_PLA = 1;";
                }
            }
            else
            {
                connectionString = $"Server=10.130.2.2:5030;Database=DBE0;UID=EFCDUSR1;PWD=EUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM E.FCD_PAGO_MASI_MAE c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM E.FCD_PAGO_MASI_MAE c where c.NU_PLANILLA = '{numeroPlanilla}'";
                }
            }

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync(sqlQuery);

                results = res.ToList();
            }

            return Ok(results);
        }

        [HttpGet("PagoMasiMae")]
        public async Task<IActionResult> FCD_PAGO_MASI_MAE(string? numeroPlanilla, string? estacion, string? ruta)
        {
            DataTable result = new DataTable();
            var results = new List<Object>();

            string sqlQuery = "";
            string connectionString = "";

            if (estacion == "A")
            {
                connectionString = $"Server=130.30.30.6:5022;Database=DBP0;UID=AFCDUSR1;PWD=AUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM A.FCD_PAGO_MASI_MAE c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM A.FCD_PAGO_MASI_MAE c where c.NU_PLANILLA = '{numeroPlanilla}'";
                }
            }
            else
            {
                connectionString = $"Server=10.130.2.2:5030;Database=DBE0;UID=EFCDUSR1;PWD=EUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM E.FCD_PAGO_MASI_MAE c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM E.FCD_PAGO_MASI_MAE c where c.NU_PLANILLA = '{numeroPlanilla}'";
                }
            }

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync(sqlQuery);

                results = res.ToList();
            }

            return Ok(results);
        }

        [HttpGet("TablaMaes")]
        public async Task<IActionResult> FCD_TABL_MAES(string? numeroPlanilla, string? estacion, string? ruta)
        {
            DataTable result = new DataTable();
            var results = new List<Object>();

            string sqlQuery = "";
            string connectionString = "";

            if (estacion == "A")
            {
                connectionString = $"Server=130.30.30.6:5022;Database=DBP0;UID=AFCDUSR1;PWD=AUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM A.FCD_TABL_MAES c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM A.FCD_TABL_MAES c WHERE c.NU_PLANILLA_A = '{numeroPlanilla}'";
                }
            }
            else
            {
                connectionString = $"Server=10.130.2.2:5030;Database=DBE0;UID=EFCDUSR1;PWD=EUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM E.FCD_TABL_MAES c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM E.FCD_TABL_MAES c where c.NU_PLANILLA_A = '{numeroPlanilla}'";
                }
            }

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync(sqlQuery);

                results = res.ToList();
            }

            return Ok(results);
        }

        [HttpGet("TablaPlanillaLog")]
        public async Task<IActionResult> FCD_TABL_PLAN_LOG([FromQuery] string? numeroPlanilla, string? estacion, string? ruta)
        {
            DataTable result = new DataTable();
            var results = new List<Object>();

            string sqlQuery = "";
            string connectionString = "";

            if (estacion == "A")
            {
                connectionString = $"Server=130.30.30.6:5022;Database=DBP0;UID=AFCDUSR1;PWD=AUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM A.FCD_TABL_PLAN_LOG c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM A.FCD_TABL_PLAN_LOG c WHERE c.NU_PLANILLA_LOG = '{numeroPlanilla}'";
                }
            }
            else
            {
                connectionString = $"Server=10.130.2.2:5030;Database=DBE0;UID=EFCDUSR1;PWD=EUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM E.FCD_TABL_PLAN_LOG c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM E.FCD_TABL_PLAN_LOG c where c.NU_PLANILLA_LOG = '{numeroPlanilla}'";
                }
            }

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync(sqlQuery);

                results = res.ToList();
            }

            return Ok(results);
        }

        [HttpGet("Tabla_Planilla")]
        public async Task<IActionResult> FCD_TABL_PLAN([FromQuery] string? numeroPlanilla, string? estacion, string? ruta)
        {
            DataTable result = new DataTable();
            var results = new List<Object>();

            string sqlQuery = "";
            string connectionString = "";

            if (estacion == "A")
            {
                connectionString = $"Server=130.30.30.6:5022;Database=DBP0;UID=AFCDUSR1;PWD=AUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM A.FCD_TABL_PLAN c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM A.FCD_TABL_PLAN c WHERE c.NU_PLANILLA_LOG = '{numeroPlanilla}'";
                }
            }
            else
            {
                connectionString = $"Server=10.130.2.2:5030;Database=DBE0;UID=EFCDUSR1;PWD=EUSRFCD1";

                if (string.IsNullOrEmpty(numeroPlanilla))
                {
                    sqlQuery = "SELECT * FROM E.FCD_TABL_PLAN c";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM E.FCD_TABL_PLAN c where c.NU_PLANILLA_LOG = '{numeroPlanilla}'";
                }
            }

            using (var connection = new DB2Connection(connectionString))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync(sqlQuery);

                results = res.ToList();
            }

            return Ok(results);
        }
    }
}
