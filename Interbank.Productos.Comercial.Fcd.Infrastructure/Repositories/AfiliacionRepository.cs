using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories
{
    public class AfiliacionRepository : IAfiliacionRepository
    {
        private readonly ILogger<AfiliacionRepository> _logger;
        private readonly IOracleConnectionFactory _oracleConnectionFactory;
        private readonly IDapperExecutor _dapperExecutor;

        public AfiliacionRepository(
            ILogger<AfiliacionRepository> logger,
            IOracleConnectionFactory oracleConnectionFactory,
            IDapperExecutor dapperExecutor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(oracleConnectionFactory);
            ArgumentNullException.ThrowIfNull(dapperExecutor);

            _logger = logger;
            _oracleConnectionFactory = oracleConnectionFactory;
            _dapperExecutor = dapperExecutor;
        }

        public async Task<int> ObtenerAfiliacion_H2H(DapperAfiliacionProveedor afiliacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                int Encontrado = 0;
                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_CODIGOUNICOACEPTANTE", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.codigoUnicoAceptante);
                parameters.Add("piv_CODIGOUNICO", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.codigoUnico);
                parameters.Add("piv_TIPOAFILIACION", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipoAfiliacion);
                parameters.Add("piv_CODIGOPRODUCTO", OracleDbType.Int64, ParameterDirection.Input, afiliacion.codigoProducto);
                parameters.Add(OracleParameterNames.PovCantidad, OracleDbType.RefCursor, ParameterDirection.Output, Encontrado);
                IEnumerable<dynamic>? dr = (await _dapperExecutor.QueryAsync(connection, OracleProcedures.ValidarAfiliacionExiste, parameters, commandType: CommandType.StoredProcedure)).ToList();
                foreach (var rows in dr)
                {
                    var fields = rows as IDictionary<string, object>;
                    if (fields != null)
                    {
                        Encontrado = Convert.ToInt32(fields["TOTALAFILIACION"].ToString());
                    }
                }
                return Encontrado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerAfiliacion_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<int> ObtenerProveedorCliente_H2H(DapperAfiliacionProveedor afiliacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                int Encontrado = 0;
                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_CODIGOUNICO", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.codigoUnico);
                parameters.Add("piv_CODIGOTIPOCLIENTE", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipoAfiliacion);
                parameters.Add(OracleParameterNames.PovCantidad, OracleDbType.RefCursor, ParameterDirection.Output, Encontrado);
                IEnumerable<dynamic>? dr = (await _dapperExecutor.QueryAsync(connection, OracleProcedures.ValidarProveedorAfiliacion, parameters, commandType: CommandType.StoredProcedure)).ToList();
                foreach (var rows in dr)
                {
                    var fields = rows as IDictionary<string, object>;
                    if (fields != null)
                    {
                        Encontrado = Convert.ToInt32(fields["TOTALCLIENTE"].ToString());
                    }
                }
                return Encontrado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ObtenerProveedorCliente_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<AfiliacionResponse> RegistrarAfiliacionProveedorFCD_H2H(DapperAfiliacionProveedor afiliacion)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_documentoDuplicado", OracleDbType.Int64, ParameterDirection.Input, afiliacion.documentoDuplicado);
                parameters.Add("piv_tipoMaxLote", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipoMaxLote);
                parameters.Add("piv_tipoMaxProv", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipoMaxProv);
                parameters.Add("piv_montoMaxLote", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.montoMaxLote);
                parameters.Add("piv_montoMaxProv", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.montoMaxProv);
                parameters.Add("piv_nombreContacto1", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.nombreContacto1);
                parameters.Add("piv_emailContacto1", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.emailContacto1);
                parameters.Add("piv_cargoContacto1", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.cargoContacto1);
                parameters.Add("piv_telefono1", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.telefono1);
                parameters.Add("piv_telefono2", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.telefono2);
                parameters.Add("piv_nombreContacto2", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.nombreContacto2);
                parameters.Add("piv_emailContacto2", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.emailContacto2);
                parameters.Add("piv_nombreContacto3", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.nombreContacto3);
                parameters.Add("piv_emailContacto3", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.emailContacto3);
                parameters.Add("piv_tipoComision", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipoComision);
                parameters.Add("piv_montoComision", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.montoComision);
                parameters.Add("piv_ampliacionPago", OracleDbType.Int64, ParameterDirection.Input, afiliacion.ampliacionPago);
                parameters.Add("piv_codigoUnicoAceptante", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.codigoUnicoAceptante);
                parameters.Add("piv_estadoProveedor", OracleDbType.Int64, ParameterDirection.Input, afiliacion.estadoProveedor);
                parameters.Add("piv_tipomonedasoles", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipomonedasoles);
                parameters.Add("piv_tipomonedadolar", OracleDbType.Int64, ParameterDirection.Input, afiliacion.tipomonedadolar);
                parameters.Add("piv_numeroCtaSoles", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroCtaSoles);
                parameters.Add("piv_numeroCtaDolares", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroCtaDolares);
                parameters.Add("piv_tasaSoles", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.tasaSoles);
                parameters.Add("piv_tasaDolares", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.tasaDolares);
                parameters.Add("piv_portes", OracleDbType.Int64, ParameterDirection.Input, afiliacion.portes);
                parameters.Add("piv_numeroLineaCliente", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroLineaCliente);
                parameters.Add("piv_razonSocialAceptante", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.razonSocialAceptante);
                parameters.Add("piv_numeroLineaAceptante", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroLineaAceptante);
                parameters.Add("piv_montoMinSolesAceptante", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.montoMinSolesAceptante);
                parameters.Add("piv_montoMinDolaresAceptante", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.montoMinDolaresAceptante);
                parameters.Add("piv_numeroLineaProveedor", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroLineaProveedor);
                parameters.Add("piv_codigoProveedor", OracleDbType.Int32, ParameterDirection.Input, afiliacion.codigoProveedor);
                parameters.Add("piv_codigoCliente", OracleDbType.Int32, ParameterDirection.Input, afiliacion.codigoCliente);
                parameters.Add("piv_codigoProducto", OracleDbType.Int64, ParameterDirection.Input, afiliacion.codigoProducto);
                parameters.Add("piv_codigoEstadoAfiliacion", OracleDbType.Int64, ParameterDirection.Input, afiliacion.codigoEstadoAfiliacion);
                parameters.Add("piv_tipoAfiliacion", OracleDbType.Int32, ParameterDirection.Input, afiliacion.tipoAfiliacion);
                parameters.Add("piv_usuarioRegistro", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.usuarioRegistro);
                parameters.Add("piv_fecharegistro", OracleDbType.Date, ParameterDirection.Input, afiliacion.fecharegistro);
                parameters.Add("piv_codigoUnico", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.codigoUnico);
                parameters.Add("piv_razonSocial", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.razonSocial);
                parameters.Add("piv_numeroLinea", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroLinea);
                parameters.Add("piv_codigoTipoDocumento", OracleDbType.Int64, ParameterDirection.Input, afiliacion.codigoTipoDocumento);
                parameters.Add("piv_numeroDocumento", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.numeroDocumento);
                parameters.Add("piv_documentoAuxiliarCliente", OracleDbType.Varchar2, ParameterDirection.Input, afiliacion.documentoAuxiliarCliente);
                parameters.Add("piv_tasaClientesoles", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.tasaClientesoles);
                parameters.Add("piv_tasaClienteDolar", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.tasaClienteDolar);
                parameters.Add("piv_validaCuentaSoles", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.validaCuentaSoles);
                parameters.Add("piv_ValidaCuentaDolares", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.validaCuentaDolares);
                parameters.Add("piv_DesembolsoCuentaSoles", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.desembolsoAutoSoles);
                parameters.Add("piv_DesemboloCuentaDolares", OracleDbType.Decimal, ParameterDirection.Input, afiliacion.desembolsoAutoDolar);

                parameters.Add(OracleParameterNames.PovCantidad, OracleDbType.RefCursor, ParameterDirection.Output, afiliacion.codigoAfiliacion);

                IEnumerable<dynamic>? dr = (await _dapperExecutor.QueryAsync(connection, OracleProcedures.InsertaAfiliacionProveedor, parameters, commandType: CommandType.StoredProcedure)).ToList();
                foreach (var rows in dr)
                {
                    var fields = rows as IDictionary<string, object>;
                    if (fields != null)
                    {
                        afiliacion.codigoAfiliacion = Convert.ToInt32(fields["CODIGOAFILIACION"].ToString());
                    }
                }

                return new AfiliacionResponse
                {
                    CodigoRespuesta = afiliacion.codigoAfiliacion.ToString(),
                    MensajeRespuesta = "Se realizo el proceso correctamente"

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarAfiliacionProveedorFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }

        public async Task<AfiliacionResponse> RegistrarProveedorFCD_H2H(DapperProveedor proveedor)
        {
            try
            {
                using var connection = _oracleConnectionFactory.CrearConexionBaseDatos();
                int CODIGOCLIENTE = 0;
                var parameters = new OracleDynamicParameters();
                parameters.Add("piv_CODIGOCLIENTE", OracleDbType.Int64, ParameterDirection.Input, proveedor.codigoCliente);
                parameters.Add("piv_CODIGOUNICO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigoUnico);
                parameters.Add("piv_CODIGOTIPODOCUMENTO", OracleDbType.Int64, ParameterDirection.Input, proveedor.codigoTipoDocumento);
                parameters.Add("piv_NUMERODOCUMENTO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.numeroDocumento);
                parameters.Add("piv_DIRECCION", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.direccion);
                parameters.Add("piv_DISTRITO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.distrito);
                parameters.Add("piv_PROVINCIA", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.provincia);
                parameters.Add("piv_DEPARTAMENTO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.departamento);
                parameters.Add("piv_SEGMENTO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.segmento);
                parameters.Add("piv_CODIGOEJECUTIVO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigoEjecutivo);
                parameters.Add("piv_NOMBREEJECUTIVO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.nombreEjecutivo);
                parameters.Add("piv_RAZONSOCIALCLIENTE", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.razonSocialCliente);
                parameters.Add("piv_CODIGOTIPOCLIENTE", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigotipoCliente);
                parameters.Add("piv_BANCA", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.banca);
                parameters.Add("piv_RATINGEMPRESA", OracleDbType.Decimal, ParameterDirection.Input, proveedor.ratingEmpresa);
                parameters.Add("piv_CIIU", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.ciiu);
                parameters.Add("piv_CODIGOTIENDA", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigoTienda);
                parameters.Add("piv_NOMBRETIENDA", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.nombreTienda);
                parameters.Add("piv_CLASIFICACIONSBS", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.clasificacionSbs);
                parameters.Add("piv_CLASIFICACIONFEVE", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.clasificacionFeve);
                parameters.Add("piv_CODIGOGRUPO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigoGrupo);
                parameters.Add("piv_NOMBREGRUPO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.nombreGrupo);
                parameters.Add("piv_CODIGOUSUARIOREGISTRO", OracleDbType.Varchar2, ParameterDirection.Input, proveedor.codigoUsuarioRegistro);
                parameters.Add(OracleParameterNames.PoiCodCliente, OracleDbType.RefCursor, ParameterDirection.Output, proveedor.codigoCliente);

                IEnumerable<dynamic>? dr = (await _dapperExecutor.QueryAsync(connection, OracleProcedures.InsertarProveedorAutomatico, parameters, commandType: CommandType.StoredProcedure)).ToList();
                foreach (var rows in dr)
                {
                    var fields = rows as IDictionary<string, object>;
                    if (fields != null)
                    {
                        CODIGOCLIENTE = Convert.ToInt32(fields["CODIGOCLIENTE"].ToString());
                    }
                }

                return new AfiliacionResponse
                {
                    CodigoRespuesta = CODIGOCLIENTE.ToString(),
                    MensajeRespuesta = "Se realizo el proceso correctamente"

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrarProveedorFCD_H2H - Error en base de datos");
                throw new InvalidOperationException(Messages.ErrorBaseDatos, ex);
            }
        }
    }
}
