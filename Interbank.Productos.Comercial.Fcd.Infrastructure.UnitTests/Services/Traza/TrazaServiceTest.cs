using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Traza;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Traza
{
    public class TrazaServiceTest
    {
        private readonly Mock<ISeguimientoRepository> _repoMock;
        private readonly Mock<ILogger<TrazaService>> _loggerMock;
        private readonly TrazaService _service;

        public TrazaServiceTest()
        {
            _repoMock = new Mock<ISeguimientoRepository>();
            _loggerMock = new Mock<ILogger<TrazaService>>();
            _service = new TrazaService(_repoMock.Object, _loggerMock.Object);
        }

        #region Constructor

        [Fact]
        public void Constructor_Should_Throw_When_Repository_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrazaService(null!, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_When_Logger_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrazaService(_repoMock.Object, null!));
        }

        [Fact]
        public void Constructor_Should_Create_Instance_When_Dependencies_Are_Valid()
        {
            var service = new TrazaService(_repoMock.Object, _loggerMock.Object);
            Assert.NotNull(service);
        }

        #endregion

        #region RegistrarCabeceraTraza

        [Fact]
        public async Task RegistrarCabeceraTraza_Should_Call_Repository_And_Log()
        {
            // Arrange
            _repoMock
                .Setup(x => x.InsertaSeguimientoCabecera(It.IsAny<DapperSeguimientoCabecera>()))
                .ReturnsAsync(10);

            // Act
            var result = await _service.RegistrarCabeceraTraza(
                1,
                "archivo.txt",
                "001",
                1);

            // Assert resultado
            Assert.Equal(10, result);

            // Verificar repo llamado
            _repoMock.Verify(x =>
                x.InsertaSeguimientoCabecera(It.Is<DapperSeguimientoCabecera>(d =>
                    d.IdDetalle == 1 &&
                    d.NombreArchivo == "archivo.txt" &&
                    d.NroPlanilla == "001" &&
                    d.Flag == 1)),
                Times.Once);

            // Verificar log
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("RegistrarCabeceraTraza llamado")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region RegistrarDetalleTraza

        [Fact]
        public async Task RegistrarDetalleTraza_Should_Call_Repository_And_Log()
        {
            // Arrange
            _repoMock
                .Setup(x => x.InsertaSeguimientoDetalle(It.IsAny<DapperSeguimientoDetalle>()))
                .ReturnsAsync(20);

            // Act
            var result = await _service.RegistrarDetalleTraza(
                2,
                "MetodoX",
                "Infra",
                "Detalle prueba",
                5);

            // Assert resultado
            Assert.Equal(20, result);

            // Verificar repo llamado
            _repoMock.Verify(x =>
                x.InsertaSeguimientoDetalle(It.Is<DapperSeguimientoDetalle>(d =>
                    d.IdDetalle == 2 &&
                    d.NombreMetodo == "MetodoX" &&
                    d.CapaObservacion == "Infra" &&
                    d.DetalleObservacion == "Detalle prueba" &&
                    d.IdEstacion == 5)),
                Times.Once);

            // Verificar log
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("RegistrarDetalleTraza llamado")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

    }

    #endregion
}
