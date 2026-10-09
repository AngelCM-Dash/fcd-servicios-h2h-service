using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;
using Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{CuAceptante}")]
        [ProducesResponseType(typeof(IEnumerable<ClienteAfiliacionVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ClienteAfiliacionVM>>> GetSuppliersByAcceptor(string CuAceptante, string? codProducto)
        {
            var query = new GetSuppliersListQuery(CuAceptante, codProducto);
            var proveedores = await _mediator.Send(query);
            return Ok(proveedores);
        }

        [HttpPost("AfiliacionProveedor")]
        public async Task<ActionResult<IEnumerable<AfiliacionResponse>>> GetSuppliersByAcceptor([FromBody] CreateAfiliacionCommand command)
        {
            _logger.LogInformation("Inicio de Proceso: AfiliacionClienteProveedor. CU Cliente: {CodigoUnicoAceptante} - CU Proveedor {Proveedor.CodigoUnico}", command.CodigoUnico, command.Proveedor?.CodigoUnico);
            var proveedores = await _mediator.Send(command);
            return Ok(proveedores);
        }

    }
}
