using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas
{
    public class DesembolsoPlanillasDiferidasCommand : IRequest<DesembolsoResponse>
    {
        public required string? numeroPlanilla { get; set; }
        public string? numeroInstruccion { get; set; }
        public string? numeroLinea { get; set; }
    }
}
