using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Base;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento
{
    public class EncolarDesembolsoCommand : DesembolsarPlanillaBase, IRequest<DesembolsoResponse>
    {
    }
}
