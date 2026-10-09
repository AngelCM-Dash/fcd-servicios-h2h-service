using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Query.ConsultaNroOperacionDesembolso;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Interbank.Productos.Comercial.Fcd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class DesembolsoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMonitorService _monitorService;
        private readonly ILogger<DesembolsoController> _logger;

        public DesembolsoController(IMediator mediator, IMonitorService monitorService, ILogger<DesembolsoController> logger)
        {
            _mediator = mediator;
            _monitorService = monitorService;
            _logger = logger;
        }

        [HttpPost("desembolsarPlanilla")]
        public async Task<ActionResult<DesembolsoResponse>> ProcesoDesembolsoPlanilla([FromBody] DesembolsoPlanillaCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation("Parametros recibidos {@Command} para DesembolsarPlanilla a las {fechaHora}", command, fechaHora);

            var actualizarPlanillaResult = await _mediator.Send(command);

            return Ok(actualizarPlanillaResult);
        }

        [HttpPost("Encolamiento")]
        public async Task<ActionResult<DesembolsoResponse>> EncolarDesembolso([FromBody] EncolarDesembolsoCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation("Parametros recibidos {@Command} para EncolarDesembolso a las {fechaHora}", command, fechaHora);

            var encolarDesembolso = await _mediator.Send(command);

            return Ok(encolarDesembolso);

        }

        [HttpPost("ProcesoAbono")]
        public async Task<ActionResult<DesembolsoResponse>> ProcesarAbono([FromBody] ActualizarPlanillaCommand command)
        {
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation("Parametros recibidos {@Command} para ProcesarAbono a las {fechaHora}", command, fechaHora);

            var actualizarPlanillaResult = await _mediator.Send(command);

            return Ok(actualizarPlanillaResult);
        }

        [HttpPost("ProcesarTramas")]
        public async Task<ActionResult<ProcesarTramasResponse>> procesarTramas([FromBody] ProcesarTramasCommand command)
        {
            var procesarTramasResult = await _mediator.Send(command);
            return Ok(procesarTramasResult);
        }

        [HttpPost("Diferidos")]
        public async Task<ActionResult<ProcesarTramasResponse>> desembolsoDiferidos([FromBody] DesembolsoPlanillasDiferidasCommand command)
        {
            var actualizarPlanillaResult = await _mediator.Send(command);

            return Ok(actualizarPlanillaResult);
        }

        [HttpPost("EnvioCorreoDietarios")]
        public async Task<ActionResult<ProcesarTramasResponse>> EnvioCorreoDietarios([FromBody] string NumeroPlanilla)
        {
            var procesarTramasResult = await _monitorService.EnvioCorreoDietarios(NumeroPlanilla, "");
            return Ok(procesarTramasResult);
        }

        [HttpGet("NumeroOperacion")]
        public async Task<ActionResult<ProcesarTramasResponse>> ConsultaNumeroOperacion([FromQuery] GetNroOperacionMovimientoQuery query)
        {
            var procesarTramasResult = await _mediator.Send(query);
            return Ok(procesarTramasResult);
        }

    }
}
