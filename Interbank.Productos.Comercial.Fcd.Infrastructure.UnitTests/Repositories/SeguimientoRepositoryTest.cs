using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NextSIT.Utility;
using System.Data;
using System.Dynamic;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class SeguimientoRepositoryTest
    {
        private readonly Mock<ITypeConvertionManager> _typeConversionManagerMock = new();
        private readonly Mock<ILogger<SeguimientoRepository>> _loggerMock = new();
        private readonly Mock<IDapperExecutor> _dapperExecutorMock = new();
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock = new();

        private readonly SeguimientoRepository _repository;

        public SeguimientoRepositoryTest()
        {
            _typeConversionManagerMock = new Mock<ITypeConvertionManager>();
            _loggerMock = new Mock<ILogger<SeguimientoRepository>>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new SeguimientoRepository(
                _typeConversionManagerMock.Object,
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object);
        }

        [Fact]
        public async Task InsertaSeguimientoCabecera_WhenSuccess_DoesNotThrow()
        {
            var command = new DapperSeguimientoCabecera();

            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.InsertaSeguimientoCabecera(command));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaSeguimientoCabH2HW,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task InsertaSeguimientoCabecera_WhenFails_ThrowsInvalidOperationException()
        {
            var command = new DapperSeguimientoCabecera();
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.InsertaSeguimientoCabecera(command));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("InsertaSeguimientoCabecera - Error en base de datos");
        }

        [Fact]
        public async Task InsertaSeguimientoDetalle_WhenSuccess_DoesNotThrow()
        {
            var command = new DapperSeguimientoDetalle();

            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.InsertaSeguimientoDetalle(command));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaSeguimientoDetH2HW,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task InsertaSeguimientoDetalle_WhenFails_ThrowsInvalidOperationException()
        {
            var command = new DapperSeguimientoDetalle();
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.InsertaSeguimientoDetalle(command));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("InsertaSeguimientoDetalle - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerIdSeguimientoxPlanilla_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.IDDETALLE = 10;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConversionManagerMock
                .Setup(x => x.AnyToInteger(It.IsAny<object>()))
                .Returns(10);

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.ObtenerIdSeguimientoxPlanilla("123"));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerSeguimientoIDxPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerIdSeguimientoxPlanilla_WhenNoResults_ReturnsZero()
        {
            // Arrange
            var emptyResult = new List<dynamic>();

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _repository.ObtenerIdSeguimientoxPlanilla("123");

            // Assert
            Assert.Equal(0, result);
        }



        [Fact]
        public async Task ObtenerIdSeguimientoxPlanilla_WhenFails_ThrowsInvalidOperationException()
        {
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerIdSeguimientoxPlanilla("123"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerIdSeguimientoxPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerSeguimientoDetalleCabecera_WhenSuccess_ReturnsList()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.IDSEGUIMIENTO = 1m;
            row.IDDETALLE = 2m;
            row.FECHAREGISTRO = "20240101";
            row.NOMBREARCHIVO = "archivo.txt";
            row.NROPLANILLA = "123";
            row.NOMBREMETODO = "MetodoX";
            row.CAPAOBSERVACION = "Servicio";
            row.DETALLEOBSERVACION = "OK";
            row.IDESTACION = 5m;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConversionManagerMock.Setup(x => x.AnyToDecimal(It.IsAny<object>()))
                .Returns((object o) => Convert.ToDecimal(o));

            _typeConversionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>()))
                .Returns((object o) => o?.ToString() ?? string.Empty);

            // Act
            var result = await _repository.ObtenerSeguimientoDetalleCabecera("20240101", "20240131", "archivo.txt", "123");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var item = result[0];

            Assert.Equal(1m, item.IdSeguimiento);
            Assert.Equal(2m, item.IdDetalle);
            Assert.Equal("20240101", item.FechaRegistro);
            Assert.Equal("archivo.txt", item.NombreArchivo);
            Assert.Equal("123", item.NroPlanilla);
            Assert.Equal("MetodoX", item.NombreMetodo);
            Assert.Equal("Servicio", item.CapaObservacion);
            Assert.Equal("OK", item.DetalleObservacion);
            Assert.Equal(5m, item.IdEstacion);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerDetalleCabecera,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerSeguimientoDetalleCabecera_WhenFails_ThrowsInvalidOperationException()
        {
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception("Simulated DB error"));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerSeguimientoDetalleCabecera(null, null, null, null));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal("Simulated DB error", ex.InnerException!.Message);

            VerifyLogError("ObtenerSeguimientoDetalleCabecera - Error en base de datos");
        }


        [Fact]
        public async Task ObtenerTipoEjecucionSeguimientoxIdDetalle_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.TIPOEJECUCION = "R";

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.ObtenerTipoEjecucionSeguimientoxIdDetalle(1));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerTipoEjecucionxIdDetalle,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }


        [Fact]
        public async Task ObtenerTipoEjecucionSeguimientoxIdDetalle_WhenFails_ThrowsInvalidOperationException()
        {
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerTipoEjecucionSeguimientoxIdDetalle(1));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerTipoEjecucionSeguimientoxIdDetalle - Error en base de datos");
        }

        private void VerifyLogError(string expectedMessage)
        {
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


    }
}
