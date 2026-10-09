using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.ErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;
using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;
using Interbank.Productos.Comercial.Fcd.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests
{
    public class ApplicationServiceRegistrationTest
    {
        private readonly IServiceCollection _services;
        private readonly Mock<IConfiguration> _configurationMock;

        public ApplicationServiceRegistrationTest()
        {
            _services = new ServiceCollection();
            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterAllRequiredServices()
        {
            // Act
            _services.AddApplicationServices(_configurationMock.Object);

            // Assert - Verificamos registros simples
            _services.Should().Contain(d => d.ServiceType == typeof(IValidation) && d.ImplementationType == typeof(Validation));
            _services.Should().Contain(d => d.ServiceType == typeof(IDesembolsoAbonoService));
            _services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>));

            // Verificamos que MediatR y AutoMapper se registraron (vía sus servicios base)
            _services.Should().Contain(d => d.ServiceType == typeof(IMediator));
        }

        [Fact]
        public void AddApplicationServices_DependenciesClasses_ShouldResolveCorrectment()
        {
            // 1. Arrange - Registramos los servicios en el contenedor
            _services.AddApplicationServices(_configurationMock.Object);

            // 2. Mockeamos TODAS las interfaces que las lambdas piden mediante GetRequiredService
            // Esto es necesario para que al invocar la lambda de dependencias, no falle
            RegisterMocksForDependencies(_services);

            var serviceProvider = _services.BuildServiceProvider();

            // 3. Act & Assert - Forzamos la ejecución de cada Lambda para cubrir esas líneas

            // Test ActualizarPlanillaDependencies
            var actualizarDeps = serviceProvider.GetService<ActualizarPlanillaDependencies>();
            actualizarDeps.Should().NotBeNull();
            actualizarDeps!.PlanillasRepository.Should().NotBeNull();

            // Test ProcesarTramasDependencies
            var procesarDeps = serviceProvider.GetService<ProcesarTramasDependencies>();
            procesarDeps.Should().NotBeNull();

            // Test DesembolsoAbonoServiceDependencies
            var abonoDeps = serviceProvider.GetService<DesembolsoAbonoServiceDependencies>();
            abonoDeps.Should().NotBeNull();

            // Test DesembolsoPlanillasDiferidasDependencies
            var diferidasDeps = serviceProvider.GetService<DesembolsoPlanillasDiferidasDependencies>();
            diferidasDeps.Should().NotBeNull();

            // Test ValidateDocumentDependencies
            var validateDeps = serviceProvider.GetService<ValidateDocumentDependencies>();
            validateDeps.Should().NotBeNull();
            validateDeps!.SftpService.Should().NotBeNull();
        }

        private static void RegisterMocksForDependencies(IServiceCollection services)
        {
            // ESTA ES LA LÍNEA CLAVE: Agrega el soporte para ILogger y ILogger<T>
            services.AddLogging();

            // Registramos Mocks para cada interfaz requerida por las clases de dependencias
            services.AddScoped(_ => new Mock<IPlanillasRepository>().Object);
            services.AddScoped(_ => new Mock<IDesembolsoRepository>().Object);
            services.AddScoped(_ => new Mock<IUtilitariosRepository>().Object);
            services.AddScoped(_ => new Mock<ISeguimientoRepository>().Object);
            services.AddScoped(_ => new Mock<IDesembolsoService>().Object);
            services.AddScoped(_ => new Mock<INotificacionService>().Object);
            services.AddScoped(_ => new Mock<IMonitorService>().Object);
            services.AddScoped(_ => new Mock<ITrazaService>().Object);
            services.AddScoped(_ => new Mock<IValidarFacturasService>().Object);
            services.AddScoped(_ => new Mock<ISftpService>().Object);
            services.AddScoped(_ => new Mock<ILineaService>().Object);
        }
    }
}
