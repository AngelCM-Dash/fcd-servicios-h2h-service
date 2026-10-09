using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions.IErrorHandling;
using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class CreateAfiliacionCommandHandlerTest
    {
        private readonly Mock<IAfiliacionRepository> _afiliacionRepository;
        private readonly Mock<ILogger<CreateAfiliacionCommandHandler>> _logger;
        private readonly Mock<IValidation> _validation;
        private readonly CreateAfiliacionCommandHandler _handler;

        public CreateAfiliacionCommandHandlerTest()
        {
            // 1. Inicializamos los Mocks
            _afiliacionRepository = new Mock<IAfiliacionRepository>();
            _logger = new Mock<ILogger<CreateAfiliacionCommandHandler>>();
            _validation = new Mock<IValidation>();

            // 2. Inyectamos los Mocks en el Handler (.Object)
            _handler = new CreateAfiliacionCommandHandler(
                _logger.Object,
                _afiliacionRepository.Object,
                _validation.Object
            );
        }

        [Fact]
        public async Task Handle_Success_ReturnsCode32()
        {
            // Arrange
            var request = CreateValidRequest();
            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()))
                .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "100" });
            _afiliacionRepository.Setup(x => x.RegistrarAfiliacionProveedorFCD_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "500", MensajeRespuesta = "Exito" });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CodigoRespuesta.Should().Be("32");
            result.CodigoAfiliacion.Should().Be(500);
            result.MensajeRespuesta.Should().Be("Exito");
        }

        [Fact]
        public async Task Handle_ValidationFails_ThrowsException()
        {
            // Arrange
            var request = CreateValidRequest();

            _validation.Setup(v => v.ValidationExceptionIfThereAreErrors())
                       .Throws(new ThrowException("99", "Error de validación"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>()
                     .WithMessage("*Error de validación*");
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_LogsErrorAndThrows()
        {
            // Arrange
            var request = CreateValidRequest();

            // Simulamos que el repositorio lanza una excepción
            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>()
                     .WithMessage("*Ocurrio un error al intentar realizar la afiliacion*");

            // Verificamos que se hizo log de error
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Ocurrió un error al intentar realizar la afiliación")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ExceptionWithProveedor_ShouldCoverTrueTernary()
        {
            // Arrange
            var request = new CreateAfiliacionCommand
            {
                CodigoUnico = "C001",
                Proveedor = new ClienteProveedorAfiliacion { CodigoUnico = "P001" } // SÍ hay proveedor
            };

            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ThrowsAsync(new Exception("Forced Error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>();
        }

        [Fact]
        public async Task Handle_ExceptionWithNullProveedor_ShouldCoverFalseTernary()
        {
            // Arrange
            var request = new CreateAfiliacionCommand
            {
                CodigoUnico = "C001",
                Proveedor = null // PROVEEDOR ES NULL
            };

            // Al ser el proveedor null, el primer log o la validación fallarán 
            // o el repo lanzará error si intentas mapear propiedades nulas.
            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ThrowsAsync(new Exception("Forced Error Null Case"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>();
        }

        [Fact]
        public async Task Handle_LogsInformationOnStart()
        {
            // Arrange
            var request = CreateValidRequest();

            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()))
                                 .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "1" });
            _afiliacionRepository.Setup(x => x.RegistrarAfiliacionProveedorFCD_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                                 .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "100" });

            // Act
            await _handler.Handle(request, CancellationToken.None);

            var identificadorProcesoLog = request.CodigoUnico + "-" + (request.Proveedor != null ? request.Proveedor.CodigoUnico : string.Empty);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains(identificadorProcesoLog)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()), // Func con Exception? para .NET 6+
                Times.Once
            );

        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_LogErrorAndThrows()
        {
            // Arrange
            var request = CreateValidRequest();

            // Simulamos que el repositorio lanza un error
            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert: Verificamos que se lanza la excepción personalizada
            await act.Should().ThrowAsync<ThrowException>()
                     .WithMessage("*Ocurrio un error al intentar realizar la afiliacion*DB Error*");

            // Verificamos que se llamó al log de error
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Ocurrió un error al intentar realizar la afiliación")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }




        [Fact]
        public async Task Handle_WhenProveedorIsNull_ShouldNotThrowAndLogInformation()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Proveedor = null; // <-- simulamos Proveedor nulo

            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()))
                                 .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "1" });
            _afiliacionRepository.Setup(x => x.RegistrarAfiliacionProveedorFCD_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                                 .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "100" });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            // Verificamos que el log de información se ejecutó, aunque Proveedor sea null
            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains(request.CodigoUnico + "-")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }


        [Fact]
        public async Task Handle_AfiliacionExists_ReturnsCode36WithSpecificMessage()
        {
            // Arrange
            var request = CreateValidRequest();
            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.CodigoRespuesta.Should().Be("36");
            result.MensajeRespuesta.Should().Contain("Ya existe la afiliacion");
        }

        [Fact]
        public async Task Handle_ProveedorExists_ReturnsCode36WithSpecificMessage()
        {
            // Arrange
            var request = CreateValidRequest();
            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(0);
            _afiliacionRepository.Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.CodigoRespuesta.Should().Be("36");
            result.MensajeRespuesta.Should().Contain("Ya existe el proveedor solicitado");
        }

        [Fact]
        public async Task Handle_WhenProveedorDoesNotExist_ShouldCallRegistrarProveedor()
        {
            // Arrange
            var request = new CreateAfiliacionCommand
            {
                CodigoUnico = "123",
                Proveedor = new ClienteProveedorAfiliacion { CodigoUnico = "P001" }
            };

            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(0); // Pasa primer IF

            _afiliacionRepository
                .Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(0); // <--- ESTO cubre la rama del "0"

            _afiliacionRepository
                .Setup(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()))
                .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "1" });

            _afiliacionRepository
                .Setup(x => x.RegistrarAfiliacionProveedorFCD_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(new AfiliacionResponse { CodigoRespuesta = "10", MensajeRespuesta = "Creado" });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.CodigoRespuesta.Should().Be("32");
            _afiliacionRepository.Verify(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProveedorAlreadyExists_ShouldReturnCode36()
        {
            // Arrange
            var request = new CreateAfiliacionCommand
            {
                CodigoUnico = "123",
                Proveedor = new ClienteProveedorAfiliacion { CodigoUnico = "P001" }
            };

            _afiliacionRepository
                .Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(0); // Pasa primer IF

            _afiliacionRepository
                .Setup(x => x.ObtenerProveedorCliente_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ReturnsAsync(1); // <--- ESTO cubre la rama del ELSE (no es 0)

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.CodigoRespuesta.Should().Be("36");
            result.MensajeRespuesta.Should().Contain("Ya existe el proveedor solicitado");

            // Verificamos que NO se llamó al registro ya que el proveedor ya existe
            _afiliacionRepository.Verify(x => x.RegistrarProveedorFCD_H2H(It.IsAny<DapperProveedor>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Exception_ThrowsThrowException()
        {
            // Arrange
            var request = CreateValidRequest();
            _afiliacionRepository.Setup(x => x.ObtenerAfiliacion_H2H(It.IsAny<DapperAfiliacionProveedor>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert estilo Fluent
            await act.Should().ThrowAsync<ThrowException>()
                .WithMessage("*Ocurrio un error al intentar realizar la afiliacion*DB Error*");
        }

        private static CreateAfiliacionCommand CreateValidRequest()
        {
            return new CreateAfiliacionCommand
            {
                CodigoUnico = "12345678",
                Proveedor = new ClienteProveedorAfiliacion
                {
                    CodigoUnico = "87654321",
                    RazonSocial = "Proveedor Test"
                },
                NombreContacto1 = "Juan",
                EmailContacto1 = "juan@test.com"
            };
        }
    }
}
