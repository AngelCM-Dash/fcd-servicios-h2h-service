using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla
{
    public class DesembolsoPlanillaHandler : IRequestHandler<DesembolsoPlanillaCommand, DesembolsoResponse>
    {
        private readonly ILogger<DesembolsoPlanillaHandler> _logger;
        private readonly IDesembolsoService _desembolsoService;

        public DesembolsoPlanillaHandler(ILogger<DesembolsoPlanillaHandler> logger, IDesembolsoService desembolsoService)
        {
            _logger = logger;
            _desembolsoService = desembolsoService;
        }

        public async Task<DesembolsoResponse> Handle(DesembolsoPlanillaCommand request, CancellationToken cancellationToken)
        {
            var param = new DesembolsoRequest
            {
                numeroPlanilla = request.NumeroPlanilla ?? "",
                estadoPlanilla = request.EstadoPlanilla,
                estadoDocumento = request.EstadoDocumento,
                codigoAgrupamiento = request.CodigoAgrupamiento,
                codigoPerfilUsuario = request.CodigoPerfilUsuario,
                codigoUsuario = request.CodigoUsuario,
                nombreUsuarioRegistro = request.NombreUsuarioRegistro,
                canalAtencion = request.CanalAtencion,
                comentario = request.Comentario,
                codigoTienda = request.CodigoTienda,
                flagDesembolsoTotal = request.FlagDesembolsoTotal,
                codigoUnico = request.CodigoUnico,
                CodigoReserva = request.CodigoReserva
            };

            var actualizarPlanillaResult = await _desembolsoService.EncolamientoDesembolso(param);

            return actualizarPlanillaResult;

        }
    }
}

