using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Commands;
using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    public class ExcepcionesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExcepcionesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("CalculoInteresComision/{nombrearchivo}")]
        public async Task<IActionResult> CalculoInteresComision(string nombrearchivo)
        {
            var command = new CreateComisionProveedorCommand(nombrearchivo);
            var respuesta = await _mediator.Send(command);

            var query = new GetCalculoInteresComisionQuery(respuesta);
            var respuestaCalculoComision = await _mediator.Send(query);

            return Ok(respuestaCalculoComision);

        }
    }
}
