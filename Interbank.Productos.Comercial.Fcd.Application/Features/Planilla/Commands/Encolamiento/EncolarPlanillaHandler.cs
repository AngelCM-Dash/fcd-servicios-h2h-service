using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.Encolamiento
{
    public class EncolarPlanillaHandler : IRequestHandler<EncolarPlanillaCommand, PlanillaResponse>
    {
        private readonly IPlanillaService _planillaService;
        private readonly ILogger<EncolarPlanillaHandler> _logger;

        public EncolarPlanillaHandler(IPlanillaService planillaService, ILogger<EncolarPlanillaHandler> logger)
        {
            _planillaService = planillaService;
            _logger = logger;
        }

        public Task<PlanillaResponse> Handle(EncolarPlanillaCommand request, CancellationToken cancellationToken)
        {
            var param = new CargaMasivaRequest
            {
                CodigoProducto = request.CodigoProducto,
                CanalAtencion = request.CanalAtencion,
                CodigoUsuario = request.CodigoUsuario,
                Usuario = request.Usuario,
                CodigoPerfilUsuario = request.CodigoPerfilUsuario,
                CodigoTienda = request.CodigoUsuario,
                Tienda = request.Tienda,
                ContratoMarco = request.ContratoMarco,
                RutaArchivo = request.RutaArchivo,
                NombreArchivo = request.NombreArchivo,
                CodigoUnico = request.CodigoUnico,
                FechaValor = request.FechaValor,
                Adicional = request.Adicional?.Select(a => new AdicionalCRequest { Campo = a.Campo }).ToList()
            };

            _logger.LogInformation("Iniciando encolamiento de planilla para el producto {CodigoProducto}, usuario {CodigoUsuario}, archivo {NombreArchivo}.",
                param.CodigoProducto, param.CodigoUsuario, param.NombreArchivo);

            _ = Task.Run(() => _planillaService.EncolamientoCargaMasiva(param));

            _logger.LogInformation("Encolamiento de planilla solicitado correctamente para el producto {CodigoProducto}, usuario {CodigoUsuario}.",
                param.CodigoProducto, param.CodigoUsuario);

            var resultado = new PlanillaResponse()
            {
                CodigoRespuesta = "32",
                MensajeRespuesta = "Respuesta Correcta",
                NumeroPlanilla = string.Empty
            };

            return Task.FromResult(resultado);
        }
    }
}
