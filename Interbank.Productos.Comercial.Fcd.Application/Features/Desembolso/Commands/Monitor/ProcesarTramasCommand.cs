using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasCommand : IRequest<ProcesarTramasResponse>
    {
        public bool? FlagMonitor { get; set; }
        public string? NumeroPlanilla { get; set; }
        public int NumeroSecuencia { get; set; }
        public int TipoProcesamiento { get; set; }
    }
}
