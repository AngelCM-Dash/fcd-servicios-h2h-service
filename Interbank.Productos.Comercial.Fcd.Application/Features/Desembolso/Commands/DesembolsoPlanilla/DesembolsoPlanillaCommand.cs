using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Base;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla
{
    public class DesembolsoPlanillaCommand : DesembolsarPlanillaBase, IRequest<DesembolsoResponse>
    {
    }
}
