using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento
{
    public class EncolarDesembolsoHandler : IRequestHandler<EncolarDesembolsoCommand, DesembolsoResponse>
    {
        private readonly IEncolamientoDesembolsoService _encolamientoDesembolsoService;
        private readonly ILogger<EncolarDesembolsoHandler> _logger;

        public EncolarDesembolsoHandler(IEncolamientoDesembolsoService encolamientoDesembolsoService, ILogger<EncolarDesembolsoHandler> logger)
        {
            _encolamientoDesembolsoService = encolamientoDesembolsoService;
            _logger = logger;
        }

        public async Task<DesembolsoResponse> Handle(EncolarDesembolsoCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _encolamientoDesembolsoService.EnqueueAsync(request);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _logger.LogInformation("Enqueue {Timestamp} - Desembolso encolado correctamente.", timestamp);
            }, cancellationToken);

            var resultado = new DesembolsoResponse()
            {
                CodigoRespuesta = "32",
                MensajeRespuesta = "Respuesta Correcta"
            };

            return resultado;
        }
    }
}
