using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Services;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.ActualizarPlanilla
{
    public class ActualizarPlanillaDependenciesTest
    {
        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Arrange: Crear mocks de todas las dependencias
            var mockDesembolsoRepository = new Mock<IDesembolsoRepository>();
            var mockUtilitariosRepository = new Mock<IUtilitariosRepository>();
            var mockSeguimientoRepository = new Mock<ISeguimientoRepository>();
            var mockPlanillasRepository = new Mock<IPlanillasRepository>();
            var mockDesembolsoService = new Mock<IDesembolsoService>();
            var mockDesembolsoAbonoService = new Mock<IDesembolsoAbonoService>();
            var mockNotificacionService = new Mock<INotificacionService>();

            // Act: Crear instancia de la clase de dependencias
            var dependencies = new ActualizarPlanillaDependencies
            {
                DesembolsoRepository = mockDesembolsoRepository.Object,
                UtilitariosRepository = mockUtilitariosRepository.Object,
                SeguimientoRepository = mockSeguimientoRepository.Object,
                PlanillasRepository = mockPlanillasRepository.Object,
                DesembolsoService = mockDesembolsoService.Object,
                DesembolsoAbonoService = mockDesembolsoAbonoService.Object,
                NotificacionService = mockNotificacionService.Object
            };

            // Assert: Verificar que ninguna dependencia sea null
            Assert.NotNull(dependencies.DesembolsoRepository);
            Assert.NotNull(dependencies.UtilitariosRepository);
            Assert.NotNull(dependencies.SeguimientoRepository);
            Assert.NotNull(dependencies.PlanillasRepository);
            Assert.NotNull(dependencies.DesembolsoService);
            Assert.NotNull(dependencies.DesembolsoAbonoService);
            Assert.NotNull(dependencies.NotificacionService);
        }
    }
}
