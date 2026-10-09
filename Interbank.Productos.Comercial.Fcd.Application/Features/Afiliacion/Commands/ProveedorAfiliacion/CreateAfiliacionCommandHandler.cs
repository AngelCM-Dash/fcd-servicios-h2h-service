using Interbank.Productos.Comercial.Fcd.Application.Constant;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class CreateAfiliacionCommandHandler : IRequestHandler<CreateAfiliacionCommand, AfiliacionResponse>
    {
        private readonly ILogger<CreateAfiliacionCommandHandler> _logger;
        private readonly IAfiliacionRepository _afiliacionRepository;
        private readonly IValidation _handlerValidation;
        public CreateAfiliacionCommandHandler(ILogger<CreateAfiliacionCommandHandler> logger, IAfiliacionRepository afiliacionRepository, IValidation validation)
        {
            _logger = logger;
            _afiliacionRepository = afiliacionRepository;
            _handlerValidation = validation;
        }

        public async Task<AfiliacionResponse> Handle(CreateAfiliacionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string IdentificadorProcesoLog = request.CodigoUnico + "-" + request.Proveedor?.CodigoUnico;
                _logger.LogInformation("{TrazaCargaI} - Handle -> NombreArchivo: {IdentificadorProcesoLog}", TrazaConstante.TrazaCargaI, IdentificadorProcesoLog);

                _handlerValidation.ValidationExceptionIfThereAreErrors();

                int verificacionAfiliacion = await _afiliacionRepository.ObtenerAfiliacion_H2H(new DapperAfiliacionProveedor()
                {
                    codigoUnicoAceptante = request.CodigoUnico,
                    codigoUnico = request.Proveedor?.CodigoUnico,
                    tipoAfiliacion = request.TipoAfiliacion,
                    codigoProducto = request.CodigoProducto
                });
                AfiliacionResponse rptaAfiliacion = new AfiliacionResponse();
                if (verificacionAfiliacion == 0)
                {
                    int verificacionProveedor = await _afiliacionRepository.ObtenerProveedorCliente_H2H(new DapperAfiliacionProveedor()
                    {
                        codigoUnico = request.Proveedor?.CodigoUnico,
                        codigoTipoDocumento = request.Proveedor?.CodigoTipoCliente,
                    });
                    if (verificacionProveedor == 0)
                    {
                        AfiliacionResponse result = await _afiliacionRepository.RegistrarProveedorFCD_H2H(new DapperProveedor()
                        {
                            codigoCliente = request.Proveedor?.CodigoCliente,
                            codigoUnico = request.Proveedor?.CodigoUnico,
                            codigoTipoDocumento = request.Proveedor?.Codigotipodocumento,
                            numeroDocumento = request.Proveedor?.NumeroDocumento,
                            direccion = request.Proveedor?.IdDireccion,
                            distrito = request.Proveedor?.IdDistrito,
                            provincia = request.Proveedor?.IdProvincia,
                            departamento = request.Proveedor?.IdDepartamento,
                            segmento = request.Proveedor?.Segmento,
                            codigoEjecutivo = request.Proveedor?.CodigoEjecutivo,
                            nombreEjecutivo = request.Proveedor?.NombreEjecutivo,
                            razonSocialCliente = request.Proveedor?.RazonSocial,
                            codigotipoCliente = request.Proveedor?.CodigoTipoCliente,
                            banca = request.Proveedor?.banca,
                            ratingEmpresa = request.Proveedor?.RatingEmpresa,
                            ciiu = request.Proveedor?.IdCiiu,
                            codigoTienda = request.Proveedor?.CodigoTienda,
                            nombreTienda = request.Proveedor?.NombreTienda,
                            clasificacionSbs = request.Proveedor?.ClasificacionSbs,
                            clasificacionFeve = request.Proveedor?.ClasificacionFeve,
                            codigoGrupo = request.Proveedor?.CodigoGrupo,
                            nombreGrupo = request.Proveedor?.NombreGrupo,
                            codigoUsuarioRegistro = request.UsuarioRegistro
                        });
                        if (Convert.ToInt64(result.CodigoRespuesta) > 0)
                        {
                            rptaAfiliacion = await _afiliacionRepository.RegistrarAfiliacionProveedorFCD_H2H(new DapperAfiliacionProveedor()
                            {
                                documentoDuplicado = request.DocumentoDuplicado,
                                tipoMaxLote = request.TipoMaxLote,
                                tipoMaxProv = request.TipoMaxProv,
                                montoMaxLote = request.MontoMaxLote,
                                montoMaxProv = request.MontoMaxProv,
                                nombreContacto1 = request.NombreContacto1,
                                emailContacto1 = request.EmailContacto1,
                                cargoContacto1 = request.CargoContacto1,
                                telefono1 = request.Telefono1,
                                telefono2 = request.Telefono2,
                                nombreContacto2 = request.NombreContacto2,
                                emailContacto2 = request.EmailContacto2,
                                nombreContacto3 = request.NombreContacto3,
                                emailContacto3 = request.EmailContacto3,
                                tipoComision = request.TipoComision,
                                montoComision = request.MontoComision,
                                ampliacionPago = request.AmpliacionPago,
                                codigoUnicoAceptante = request.CodigoUnico,
                                estadoProveedor = request.EstadoProveedor,
                                tipomonedasoles = request.Tipomonedasoles,
                                tipomonedadolar = request.Tipomonedadolar,
                                numeroCtaSoles = request.NumeroCtaSoles,
                                numeroCtaDolares = request.NumeroCtaDolares,
                                tasaSoles = request.TasaSoles,
                                tasaDolares = request.TasaDolares,
                                portes = request.Portes,
                                numeroLineaCliente = request.NumeroLineaCliente,
                                razonSocialAceptante = request.RazonSocial,
                                numeroLineaAceptante = request.NumeroLineaAceptante,
                                montoMinSolesAceptante = request.MontoMinSolesAceptante,
                                montoMinDolaresAceptante = request.MontoMinDolaresAceptante,
                                numeroLineaProveedor = request.NumeroLineaProveedor,
                                codigoProveedor = Convert.ToInt32(result.CodigoRespuesta),
                                codigoCliente = request.CodigoCliente,
                                codigoProducto = request.CodigoProducto,
                                codigoEstadoAfiliacion = request.CodigoEstadoAfiliacion,
                                tipoAfiliacion = request.TipoAfiliacion,
                                usuarioRegistro = request.UsuarioRegistro,
                                fecharegistro = DateTime.Now,
                                codigoUnico = request.Proveedor?.CodigoUnico,
                                razonSocial = request.Proveedor?.RazonSocial,
                                numeroLinea = request.NumeroLinea,
                                codigoTipoDocumento = request.CodigoTipoDocumento,
                                numeroDocumento = request.NumeroDocumento,
                                documentoAuxiliarCliente = request.DocumentoAuxiliarCliente,
                                tasaClientesoles = request.TasaClientesoles,
                                tasaClienteDolar = request.TasaClienteDolar,
                                validaCuentaSoles = request.ValidaCuentaSoles,
                                validaCuentaDolares = request.ValidaCuentaDolares,
                                fechaIngreso = DateTime.Now,
                                fechaUltimaActualizacion = DateTime.Now,
                                desembolsoAutoSoles = request.DesembolsoAutoSoles,
                                desembolsoAutoDolar = request.DesembolsoAutoDolar
                            });
                        }
                    }
                    else
                    {
                        return new AfiliacionResponse
                        {
                            CodigoRespuesta = "36",
                            MensajeRespuesta = "Ya existe el proveedor solicitado, verificar",
                            NombreContacto = "",
                            CorreoContacto = "",
                            CodigoAfiliacion = 0,
                            RazonSocialCliente = "",
                            RazonSocialProveedor = "",
                            NumeroDocumentoCliente = "",
                            NumeroDocumentoProveedor = ""
                        };
                    }

                }
                else
                {
                    return new AfiliacionResponse
                    {
                        CodigoRespuesta = "36",
                        MensajeRespuesta = "Ya existe la afiliacion del cliente y proveedor para el producto",
                        NombreContacto = "",
                        CorreoContacto = "",
                        CodigoAfiliacion = 0,
                        RazonSocialCliente = "",
                        RazonSocialProveedor = "",
                        NumeroDocumentoCliente = "",
                        NumeroDocumentoProveedor = ""
                    };
                }
                return new AfiliacionResponse
                {
                    CodigoRespuesta = "32",
                    MensajeRespuesta = rptaAfiliacion.MensajeRespuesta,
                    NombreContacto = request.NombreContacto1,
                    CorreoContacto = request.EmailContacto1,
                    CodigoAfiliacion = Convert.ToInt32(rptaAfiliacion.CodigoRespuesta),
                    RazonSocialCliente = request.RazonSocial,
                    RazonSocialProveedor = request.Proveedor?.RazonSocial,
                    NumeroDocumentoCliente = request.NumeroDocumento,
                    NumeroDocumentoProveedor = request.Proveedor?.NumeroDocumento
                };
            }
            catch (Exception ex)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                var codigoProveedor = request.Proveedor != null ? request.Proveedor.CodigoUnico : string.Empty;

                _logger.LogError(
                    ex,
                    "Ocurrió un error al intentar realizar la afiliación entre {CodigoUnicoAceptante} y {CodigoUnicoProveedor} a las {Timestamp}.",
                    request.CodigoUnico,
                    codigoProveedor,
                    timestamp
                );

                throw new ThrowException("36", "Ocurrio un error al intentar realizar la afiliacion " + ex.Message);
            }
        }


    }
}
