using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NextSIT.Utility;
using System.Data;
using System.Dynamic;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class ExcepcionRepositoryTest
    {
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock = new();
        private readonly Mock<IDapperExecutor> _dapperExecutorMock = new();
        private readonly Mock<ITypeConvertionManager> _typeConversionManagerMock = new();
        private readonly Mock<ILogger<ExcepcionRepository>> _loggerMock = new();

        private readonly ExcepcionRepository _repository;

        public ExcepcionRepositoryTest()
        {
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();
            _typeConversionManagerMock = new Mock<ITypeConvertionManager>();
            _loggerMock = new Mock<ILogger<ExcepcionRepository>>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new ExcepcionRepository(
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object,
                _typeConversionManagerMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Constructor_NullOracleConnectionFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ExcepcionRepository(
                    null!,
                    new Mock<IDapperExecutor>().Object,
                    new Mock<ITypeConvertionManager>().Object,
                    new Mock<ILogger<ExcepcionRepository>>().Object));

            Assert.Equal("oracleConnectionFactory", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullDatabaseExecutor_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ExcepcionRepository(
                    new Mock<IOracleConnectionFactory>().Object,
                    null!,
                    new Mock<ITypeConvertionManager>().Object,
                    new Mock<ILogger<ExcepcionRepository>>().Object));

            Assert.Equal("databaseExecutor", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullTypeConversionManager_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ExcepcionRepository(
                    new Mock<IOracleConnectionFactory>().Object,
                    new Mock<IDapperExecutor>().Object,
                    null!,
                    new Mock<ILogger<ExcepcionRepository>>().Object));

            Assert.Equal("typeConvertionManager", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ExcepcionRepository(
                    new Mock<IOracleConnectionFactory>().Object,
                    new Mock<IDapperExecutor>().Object,
                    new Mock<ITypeConvertionManager>().Object,
                     null!));

            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public async Task ConsultaCalculoInteresComision_WhenQueryFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.CalculoInteresComision,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            Func<Task> act = async () =>
                await _repository.ConsultaCalculoInteresComision(1);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            //Verificamos que se hizo LogError correctamente
            VerifyLogError("ConsultaCalculoInteresComision - Error en base de datos");
        }

        [Fact]
        public async Task ConsultaCalculoInteresComision_ReturnsExpectedData()
        {
            // Arrange

            // Resultado simulado de Dapper
            dynamic row1 = new ExpandoObject();
            row1.CODIGOUNICOPROVEEDOR = "123";
            row1.IMPORTEDESCUENTO = 10m;
            row1.IMPORTEPORTES = 5m;
            row1.TOTAL = 15m;

            dynamic row2 = new ExpandoObject();
            row2.CODIGOUNICOPROVEEDOR = "456";
            row2.IMPORTEDESCUENTO = 20m;
            row2.IMPORTEPORTES = 10m;
            row2.TOTAL = 30m;

            var fakeDapperResult = new List<dynamic> { row1, row2 };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.CalculoInteresComision,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(fakeDapperResult);

            // Mock conversion manager
            _typeConversionManagerMock
                .Setup(m => m.AnyToString(It.IsAny<object>()))
                .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConversionManagerMock
                .Setup(m => m.AnyToDecimal(It.IsAny<object>()))
                .Returns((object o) => Convert.ToDecimal(o));

            // Act
            var result = await _repository.ConsultaCalculoInteresComision(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("123", result[0].CodigoProveedor);
            Assert.Equal(15m, result[0].Total);

            Assert.Equal("456", result[1].CodigoProveedor);
            Assert.Equal(30m, result[1].Total);

            // Verificamos que QueryAsync fue llamado una vez
            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.CalculoInteresComision,
                It.IsAny<OracleDynamicParameters>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task GeneraCodigoSecuenciaInteresComision_ReturnsCorrectValue()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(d => d.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerCodigoSecuenciaCalculo,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(1)
                .Callback<IDbConnection, string, object?, CommandType?>((conn, sp, paramObj, ct) =>
                {
                    var param = paramObj as OracleDynamicParameters;

                    // Simulamos que Oracle devuelve 42 en el parámetro de salida
                    param?.Set(OracleParameterNames.PoRespuesta, 42);
                });

            // Act
            var result = await _repository.GeneraCodigoSecuenciaInteresComision();

            // Assert
            Assert.Equal(42, result);

            _dapperExecutorMock.Verify(d => d.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerCodigoSecuenciaCalculo,
                It.IsAny<OracleDynamicParameters>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task GeneraCodigoSecuenciaInteresComision_WhenExecuteFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(d => d.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerCodigoSecuenciaCalculo,
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.GeneraCodigoSecuenciaInteresComision());

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            //Verificar que se hizo LogError correctamente
            VerifyLogError("GeneraCodigoSecuenciaInteresComision - Error en base de datos");
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
