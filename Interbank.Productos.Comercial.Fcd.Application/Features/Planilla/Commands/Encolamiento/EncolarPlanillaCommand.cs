using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.Encolamiento
{
    public class EncolarPlanillaCommand : PlanillaCommandBase, IRequest<PlanillaResponse>
    {
    }
}
