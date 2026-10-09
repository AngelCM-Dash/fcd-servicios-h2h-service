using Interbank.Productos.Comercial.Fcd.Domain.Entities.Base;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla
{
    public class ActualizarPlanillaCommand : DesembolsarPlanillaBase, IRequest<DesembolsoResponse>
    {
    }
}