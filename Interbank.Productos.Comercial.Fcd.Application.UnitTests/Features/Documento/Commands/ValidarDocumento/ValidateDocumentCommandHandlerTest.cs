using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentCommandHandlerTest
    {
        private readonly Mock<IUtilitariosRepository> _utilitariosRepositoryMock = new();
        private readonly Mock<IPlanillasRepository> _planillasRepositoryMock = new();
        private readonly Mock<ITrazaService> _trazaServiceMock = new();
        private readonly Mock<INotificacionService> _notificacionServiceMock = new();
        private readonly Mock<IValidarFacturasService> _validarFacturasService = new();
        private readonly Mock<ISftpService> _sftpService = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ValidateDocumentCommandHandler>> _loggerMock = new();

        private ValidateDocumentDependencies CreateDependencies()
        {
            return new ValidateDocumentDependencies
            {
                UtilitariosRepository = _utilitariosRepositoryMock.Object,
                PlanillasRepository = _planillasRepositoryMock.Object,
                TrazaService = _trazaServiceMock.Object,
                NotificacionService = _notificacionServiceMock.Object,
                ValidarFacturasService = _validarFacturasService.Object,
                SftpService = _sftpService.Object
            };
        }

        [Fact]
        public async Task Handle_Filtro1_ProcesaDocumentoDuplicado()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU1", 1, "archivo.txt", 1);
            _trazaServiceMock.Setup(x => x.RegistrarCabeceraTraza(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(1);
            _planillasRepositoryMock.Setup(x => x.ValidarPermiteDuplicados(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(0);
            _planillasRepositoryMock.Setup(x => x.GeneraCodigoSecuenciaDocumentosDuplicados()).ReturnsAsync(1);
            _utilitariosRepositoryMock.Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>())).ReturnsAsync(new List<DapperParametro>());
            _planillasRepositoryMock.Setup(x => x.ObtenerDocumentosDuplicados(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new List<DapperDocumentosDuplicados>());
            _mapperMock.Setup(x => x.Map<List<ValidatePlanillaResponse>>(It.IsAny<List<DapperDocumentosDuplicados>>()))
                .Returns(new List<ValidatePlanillaResponse>());
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
            Assert.Equal("Respuesta correcta", result.MensajeRespuesta);
        }

        [Fact]
        public async Task Handle_Filtro1_ProcesaDocumentoDuplicado_PermiteDuplicados()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU2", 1, "archivo2.txt", 1);
            _trazaServiceMock.Setup(x => x.RegistrarCabeceraTraza(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(1);
            _planillasRepositoryMock.Setup(x => x.ValidarPermiteDuplicados(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(1);
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
        }

        [Fact]
        public async Task Handle_FiltroDistinto1_ProcesaFacturaPendiente_SinFacturas()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU3", 1, "archivo3.txt", 2);
            _planillasRepositoryMock.Setup(x => x.ValidarFacturaCargo(It.IsAny<string>())).ReturnsAsync(0);
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
        }

        [Fact]
        public async Task Handle_FiltroDistinto1_ProcesaFacturaPendiente_ConFacturas()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU4", 1, "archivo4.txt", 2);
            _planillasRepositoryMock.Setup(x => x.ValidarFacturaCargo(It.IsAny<string>())).ReturnsAsync(2);
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
        }

        [Fact]
        public async Task ProcesarDocumentoDuplicado_ThrowsException_EnviaNotificacion()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU5", 1, "archivo5.txt", 1);
            _trazaServiceMock.Setup(x => x.RegistrarCabeceraTraza(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(1);
            _planillasRepositoryMock.Setup(x => x.ValidarPermiteDuplicados(It.IsAny<string>(), It.IsAny<int?>())).ThrowsAsync(new System.Exception("ErrorPermiteDuplicados"));
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
        }

        [Fact]
        public async Task VerificarFacturaPendiente_ThrowsException_EnviaNotificacion()
        {
            // Arrange
            var request = new ValidateDocumentCommand("CU6", 1, "archivo6.txt", 2);
            _planillasRepositoryMock.Setup(x => x.ValidarFacturaCargo(It.IsAny<string>())).ThrowsAsync(new System.Exception("ErrorFacturaCargo"));
            _notificacionServiceMock.Setup(x => x.NotificacionAssi(It.IsAny<NotificacionAssiRequest>(), It.IsAny<decimal>())).ReturnsAsync(32);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.CodigoRespuesta);
        }

        private ValidateDocumentCommandHandler CreateHandler()
        {
            var deps = CreateDependencies();

            return new ValidateDocumentCommandHandler(
                deps,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

    }
}
