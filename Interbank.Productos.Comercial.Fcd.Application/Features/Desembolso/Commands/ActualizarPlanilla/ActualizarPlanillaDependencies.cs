using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Services;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla
{
    public class ActualizarPlanillaDependencies
    {
        //Repositorios
        public IDesembolsoRepository DesembolsoRepository { get; init; } = null!;
        public IUtilitariosRepository UtilitariosRepository { get; init; } = null!;
        public ISeguimientoRepository SeguimientoRepository { get; init; } = null!;
        public IPlanillasRepository PlanillasRepository { get; init; } = null!;

        //Services
        public IDesembolsoService DesembolsoService { get; init; } = null!;
        public IDesembolsoAbonoService DesembolsoAbonoService { get; init; } = null!;
        public INotificacionService NotificacionService { get; init; } = null!;
        public ILineaService LineaService { get; init; } = null!;
    }
}
