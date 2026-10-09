using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Common;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva
{
    public class CreatePlanillasCommand : PlanillaCommandBase, IRequest<PlanillaResponse>
    {
    }
}
