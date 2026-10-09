using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla
{
    public class ValidatePlanillaCommand : IRequest<string>
    {
        public string? CodigoUnico { get; set; }
        public int? CodigoProducto { get; set; }
        public string? NombreArchivo { get; set; }
    }
}
