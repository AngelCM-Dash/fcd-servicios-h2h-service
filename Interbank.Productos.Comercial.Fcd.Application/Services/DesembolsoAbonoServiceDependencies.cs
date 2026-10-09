using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;

namespace Interbank.Productos.Comercial.Fcd.Application.Services
{
    public class DesembolsoAbonoServiceDependencies
    {
        //Repositorios
        public IUtilitariosRepository UtilitariosRepository { get; init; } = null!;
        public IDesembolsoRepository DesembolsoRepository { get; init; } = null!;

        //Servicios
        public IDesembolsoService DesembolsoService { get; init; } = null!;
        public IMonitorService MonitorService { get; init; } = null!;
        public INotificacionService NotificacionService { get; init; } = null!;

    }
}
