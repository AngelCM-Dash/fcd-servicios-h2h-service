using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class DocumentosRepositoryTest
    {
        private readonly Mock<ILogger<DocumentosRepository>> _loggerMock;
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IDapperExecutor> _dapperExecutorMock;

        private readonly DocumentosRepository _repository;

        public DocumentosRepositoryTest()
        {
            _loggerMock = new Mock<ILogger<DocumentosRepository>>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new DocumentosRepository(
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new DocumentosRepository(
                    null!, // logger nulo
                    new Mock<IOracleConnectionFactory>().Object,
                    new Mock<IDapperExecutor>().Object));

            Assert.Equal("logger", ex.ParamName);
        }


        [Fact]
        public void Constructor_NullOracleConnectionFactory_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new DocumentosRepository(
                    new Mock<ILogger<DocumentosRepository>>().Object,
                    null!, // oracleConnectionFactory nulo
                    new Mock<IDapperExecutor>().Object));

            Assert.Equal("oracleConnectionFactory", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullDapperExecutor_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new DocumentosRepository(
                    new Mock<ILogger<DocumentosRepository>>().Object,
                    new Mock<IOracleConnectionFactory>().Object,
                    null!)); // dapperExecutor nulo

            Assert.Equal("dapperExecutor", ex.ParamName);
        }

        [Fact]
        public async Task RechazarDocumentosDiferidos_Valido_EjecutaExecuteAsync()
        {
            // Arrange
            string numeroPlanilla = "PL123";
            string numeroLinea = "LN456";
            string lineaObservacion = "Observacion prueba";
            string fechaAdelanto = "15/02/2026";

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // Act
            await _repository.RechazarDocumentosDiferidos(numeroPlanilla, numeroLinea, lineaObservacion, fechaAdelanto);

            // Assert
            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.RechazoDocumentoDiferidos,
                It.IsAny<OracleDynamicParameters>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RechazarDocumentosDiferidos_ExecuteAsyncFalla_LanzaInvalidOperationException()
        {
            // Arrange
            string numeroPlanilla = "PL123";
            string numeroLinea = "LN456";
            string lineaObservacion = "Observacion prueba";
            string fechaAdelanto = "15/02/2026";

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.RechazoDocumentoDiferidos,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            Func<Task> act = async () =>
                await _repository.RechazarDocumentosDiferidos(numeroPlanilla, numeroLinea, lineaObservacion, fechaAdelanto);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            VerifyLogError("RechazarDocumentosDiferidos - Error en base de datos");
        }

        [Fact]
        public async Task RechazarDocumentosDistribuido_Valido_EjecutaExecuteAsync()
        {
            // Arrange
            string numeroPlanilla = "PL123";
            string observacion = "Observacion prueba";

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // Act
            await _repository.RechazarDocumentosDistribuido(numeroPlanilla, observacion);

            // Assert
            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.RechazoDocumentoDistribuido,
                It.IsAny<OracleDynamicParameters>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RechazarDocumentosDistribuido_ExecuteAsyncFalla_LanzaInvalidOperationException()
        {
            // Arrange
            string numeroPlanilla = "PL123";
            string observacion = "Observacion prueba";

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.RechazoDocumentoDistribuido,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            Func<Task> act = async () =>
                await _repository.RechazarDocumentosDistribuido(numeroPlanilla, observacion);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            VerifyLogError("RechazarDocumentosDistribuido - Error en base de datos");
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
