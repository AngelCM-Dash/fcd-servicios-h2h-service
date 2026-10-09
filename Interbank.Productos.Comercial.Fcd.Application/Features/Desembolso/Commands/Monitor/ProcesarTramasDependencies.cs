using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasDependencies
    {
        //Repositorios
        public IDesembolsoRepository DesembolsoRepository { get; init; } = null!;
        public IUtilitariosRepository UtilitariosRepository { get; init; } = null!;
        public ISeguimientoRepository SeguimientoRepository { get; init; } = null!;
        public IPlanillasRepository PlanillasRepository { get; init; } = null!;

        //Services
        public IMonitorService MonitorService { get; init; } = null!;
        public INotificacionService NotificacionService { get; init; } = null!;
        public ILineaService LineaService { get; init; } = null!;
    }
}
