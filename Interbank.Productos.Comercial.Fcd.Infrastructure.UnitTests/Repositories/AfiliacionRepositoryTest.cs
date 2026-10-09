using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class AfiliacionRepositoryTest
    {
        private readonly Mock<ILogger<AfiliacionRepository>> _loggerMock;
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IDapperExecutor> _dapperExecutorMock;

        private readonly AfiliacionRepository _repository;

        public AfiliacionRepositoryTest()
        {
            _loggerMock = new Mock<ILogger<AfiliacionRepository>>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new AfiliacionRepository(
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object);
        }

        [Fact]
        public async Task ObtenerAfiliacion_H2H_WhenSuccess_ReturnsCount()
        {
            // Arrange
            var afiliacion = new DapperAfiliacionProveedor { codigoUnicoAceptante = "A1", codigoUnico = "C1" };
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            var queryResult = new List<dynamic> { new Dictionary<string, object> { { "TOTALAFILIACION", 3 } } };
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       OracleProcedures.ValidarAfiliacionExiste,
                                                       It.IsAny<object>(),
                                                       CommandType.StoredProcedure))
                               .ReturnsAsync(queryResult);

            // Act
            var result = await _repository.ObtenerAfiliacion_H2H(afiliacion);

            // Assert
            Assert.Equal(3, result);
            _dapperExecutorMock.Verify(d => d.QueryAsync(mockConnection.Object,
                                                         OracleProcedures.ValidarAfiliacionExiste,
                                                         It.IsAny<object>(),
                                                         CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerAfiliacion_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var afiliacion = new DapperAfiliacionProveedor();
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       It.IsAny<string>(),
                                                       It.IsAny<object>(),
                                                       It.IsAny<CommandType?>()))
                               .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerAfiliacion_H2H(afiliacion));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            //Verificamos que se hizo LogError correctamente
            VerifyLogError("ObtenerAfiliacion_H2H - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerProveedorCliente_H2H_WhenSuccess_ReturnsCount()
        {
            var afiliacion = new DapperAfiliacionProveedor { codigoUnico = "C1", tipoAfiliacion = 1 };
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            var queryResult = new List<dynamic> { new Dictionary<string, object> { { "TOTALCLIENTE", 5 } } };
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       OracleProcedures.ValidarProveedorAfiliacion,
                                                       It.IsAny<object>(),
                                                       CommandType.StoredProcedure))
                               .ReturnsAsync(queryResult);

            var result = await _repository.ObtenerProveedorCliente_H2H(afiliacion);

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task ObtenerProveedorCliente_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            var afiliacion = new DapperAfiliacionProveedor();
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            // Arrange
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       It.IsAny<string>(),
                                                       It.IsAny<object>(),
                                                       It.IsAny<CommandType?>()))
                               .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerProveedorCliente_H2H(afiliacion));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            //Verificamos que se hizo LogError correctamente
            VerifyLogError("ObtenerProveedorCliente_H2H - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarAfiliacionProveedorFCD_H2H_WhenSuccess_ReturnsAfiliacionResponse()
        {
            var afiliacion = new DapperAfiliacionProveedor { codigoAfiliacion = 0 };
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            var queryResult = new List<dynamic> { new Dictionary<string, object> { { "CODIGOAFILIACION", 100 } } };
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       OracleProcedures.InsertaAfiliacionProveedor,
                                                       It.IsAny<object>(),
                                                       CommandType.StoredProcedure))
                               .ReturnsAsync(queryResult);

            var response = await _repository.RegistrarAfiliacionProveedorFCD_H2H(afiliacion);

            Assert.Equal("100", response.CodigoRespuesta);
            Assert.Equal("Se realizo el proceso correctamente", response.MensajeRespuesta);
        }

        [Fact]
        public async Task RegistrarAfiliacionProveedorFCD_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            var afiliacion = new DapperAfiliacionProveedor();
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            // Arrange
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       It.IsAny<string>(),
                                                       It.IsAny<object>(),
                                                       It.IsAny<CommandType?>()))
                               .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarAfiliacionProveedorFCD_H2H(afiliacion));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            //Verificamos que se hizo LogError correctamente
            VerifyLogError("RegistrarAfiliacionProveedorFCD_H2H - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarProveedorFCD_H2H_WhenSuccess_ReturnsAfiliacionResponse()
        {
            var proveedor = new DapperProveedor { codigoCliente = 0 };
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            var queryResult = new List<dynamic> { new Dictionary<string, object> { { "CODIGOCLIENTE", 200 } } };
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       OracleProcedures.InsertarProveedorAutomatico,
                                                       It.IsAny<object>(),
                                                       CommandType.StoredProcedure))
                               .ReturnsAsync(queryResult);

            var response = await _repository.RegistrarProveedorFCD_H2H(proveedor);

            Assert.Equal("200", response.CodigoRespuesta);
            Assert.Equal("Se realizo el proceso correctamente", response.MensajeRespuesta);
        }

        [Fact]
        public async Task RegistrarProveedorFCD_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            var proveedor = new DapperProveedor();
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos()).Returns(mockConnection.Object);

            // Arrange
            _dapperExecutorMock.Setup(d => d.QueryAsync(mockConnection.Object,
                                                       It.IsAny<string>(),
                                                       It.IsAny<object>(),
                                                       It.IsAny<CommandType?>()))
                               .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarProveedorFCD_H2H(proveedor));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            //Verificamos que se hizo LogError correctamente
            VerifyLogError("RegistrarProveedorFCD_H2H - Error en base de datos");
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
