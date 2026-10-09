using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasDependenciesTest
    {
        [Fact]
        public void PuedeCrearInstanciaYAsignarDependencias()
        {
            // Arrange - mocks de interfaces
            var deps = new ProcesarTramasDependencies
            {
                DesembolsoRepository = new Mock<IDesembolsoRepository>().Object,
                UtilitariosRepository = new Mock<IUtilitariosRepository>().Object,
                SeguimientoRepository = new Mock<ISeguimientoRepository>().Object,
                PlanillasRepository = new Mock<IPlanillasRepository>().Object,
                MonitorService = new Mock<IMonitorService>().Object,
                NotificacionService = new Mock<INotificacionService>().Object
            };

            // Assert
            Assert.NotNull(deps.DesembolsoRepository);
            Assert.NotNull(deps.UtilitariosRepository);
            Assert.NotNull(deps.SeguimientoRepository);
            Assert.NotNull(deps.PlanillasRepository);
            Assert.NotNull(deps.MonitorService);
            Assert.NotNull(deps.NotificacionService);
        }
    }
}
