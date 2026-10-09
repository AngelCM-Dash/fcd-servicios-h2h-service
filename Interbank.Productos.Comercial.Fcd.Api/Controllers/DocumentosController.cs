using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.RechazoDocumentos;
using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DocumentosController> _logger;

        public DocumentosController(IMediator mediator, ILogger<DocumentosController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("ValidarFacturas")]
        public async Task<ActionResult<ValidateDocumentResponse>> validarFacturas([FromBody] ValidateDocumentCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation("Parametros recibidos {@Command} para ValidarFacturas a las {fechaHora}", command, fechaHora);
            var procesarTramasResult = await _mediator.Send(command);
            return Ok(procesarTramasResult);
        }

        [HttpPost("Rechazo")]
        public async Task<ActionResult<ValidateDocumentResponse>> RechazoDocumentos([FromBody] RejectionDocumentsCommand command)
        {
            var procesarTramasResult = await _mediator.Send(command);
            return Ok(procesarTramasResult);
        }
    }
}
