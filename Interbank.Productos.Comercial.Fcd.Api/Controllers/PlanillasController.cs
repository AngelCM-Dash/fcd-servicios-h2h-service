using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetParametersByDomain;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.Encolamiento;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.OrdenarArchivosCtl;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common;
using Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.ParametroConstants;


namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class PlanillasController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<PlanillasController> _logger;
        private readonly IServiceProvider _serviceProvider;
        public PlanillasController(IMediator mediator, ILogger<PlanillasController> logger, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("CargaMasivaPlanillas")]
        public async Task<ActionResult<PlanillaResponse>> CargaMasivaPlanillas([FromBody] EncolarPlanillaCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var dynatraceScope = _logger.BeginScope(CreatePlanillaDynatraceScope("CargaMasivaPlanillas", command));

            _logger.LogInformation("Parametros recibidos {@Command} para la CargaMasivaPlanillas a las {fechaHora}", command, fechaHora);

            var actualizarPlanillaResult = await _mediator.Send(command);

            return Ok(actualizarPlanillaResult);
        }


        [HttpPost("ProcesoCargaMasiva")]
        public async Task<ActionResult<PlanillaResponse>> ProcesoCargaMasiva([FromBody] CreatePlanillasCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var dynatraceScope = _logger.BeginScope(CreatePlanillaDynatraceScope("ProcesoCargaMasiva", command));

            _logger.LogInformation("Parametros recibidos {@Command} para el ProcesoCargaMasiva a las {fechaHora}", command, fechaHora);

            var validator = new CreatePlanillasValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var query = new GetParametersByDomainQuery()
            {
                CodigoDominio = DominioGestorValidacionCargaMasiva
            };

            var responseControlVal = await _mediator.Send(query);

            var flagValidate = Convert.ToInt32(responseControlVal.Find(x => x.NUMEROORDEN == (int)NumOrdenDominioGestorValidacionCargaMasiva.FlagValidacionFacturas)?.DESCRIPCIONCORTA ?? "");

            if (flagValidate != 0)
            {
                var request = new ValidatePlanillaCommand()
                {
                    CodigoProducto = command.CodigoProducto,
                    CodigoUnico = command.CodigoUnico,
                    NombreArchivo = command.NombreArchivo
                };

                var resultado = await _mediator.Send(request);

                if (resultado.Contains(ErroresConstants.ErrorValidacionFactura))
                {
                    _logger.LogInformation("Se detectó un error durante la validación para el archivo: {NombreArchivo}", command.NombreArchivo);
                }
                else if (!resultado.Contains(ErroresConstants.ErrorFlujoOkValidacionFactura))
                {
                    _logger.LogInformation("Inicio de Proceso: CargaMasivaPlanillas. Archivo: {NombreArchivo}", command.NombreArchivo);
                    var response = await _mediator.Send(command);
                    _logger.LogInformation("Fin de Proceso: CargaMasivaPlanillas. Archivo: {NombreArchivo}", command.NombreArchivo);
                }
                else
                {
                    _logger.LogInformation("Salto una o mas validaciones para el archivo: {NombreArchivo}", command.NombreArchivo);
                }
            }
            else
            {
                _logger.LogInformation("Inicio de Proceso: CargaMasivaPlanillas. Archivo: {NombreArchivo}", command.NombreArchivo);
                var response = await _mediator.Send(command);
                _logger.LogInformation("Fin de Proceso: CargaMasivaPlanillas. Archivo: {NombreArchivo}", command.NombreArchivo);
                return Ok(response);
            }

            return Ok(new PlanillaResponse
            {
                CodigoRespuesta = "32",
                MensajeRespuesta = "Respuesta Correcta",
                NumeroPlanilla = ""
            });
        }

        private static Dictionary<string, object> CreatePlanillaDynatraceScope(string accion, PlanillaCommandBase command) => new()
        {
            ["EnviarDynatrace"] = true,
            ["Accion"] = accion,
            ["ArchivoNombre"] = command.NombreArchivo ?? string.Empty,
            ["TransactionId"] = command.CodigoUnico ?? string.Empty,
            ["ProductoCodigo"] = command.CodigoProducto?.ToString() ?? string.Empty,
            ["Canal"] = command.CanalAtencion ?? string.Empty,
            ["EstacionId"] = command.CodigoTienda?.ToString() ?? string.Empty
        };

        [HttpGet("SeguimientoDetalleCabecera")]
        public async Task<ActionResult<IEnumerable<DetalleCabeceraVM>>> ObtenerSeguimientoDetalleCabecera(string? fechaDesde, string? fechaHasta, string? nombreArchivo, string? numeroPlanilla)
        {
            var query = new GetListTrackingDetailQuery(fechaDesde, fechaHasta, nombreArchivo, numeroPlanilla);
            var response = await _mediator.Send(query);
            var cantidadRegistros = response.Count;

            var respuestaConCantidad = new
            {
                CantidadRegistros = cantidadRegistros,
                Detalles = response
            };

            return Ok(respuestaConCantidad);
        }

        [HttpPost("OrdenarArchivosCtlCargaMasiva")]
        public async Task<IActionResult> OrdenarArchivosCtlCargaMasiva(OrdenarArchivosCtlCargaMasivaCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al mover archivos: {ex.Message}");
            }
        }
    }
}
