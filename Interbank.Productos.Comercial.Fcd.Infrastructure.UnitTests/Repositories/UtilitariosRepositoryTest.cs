using IInterbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.OracleCustomTypeMapping;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NextSIT.Utility;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class UtilitariosRepositoryTest
    {
        private readonly Mock<ITypeConvertionManager> _typeConversionManagerMock = new();
        private readonly Mock<ILogger<UtilitariosRepository>> _loggerMock = new();
        private readonly Mock<IDapperExecutor> _dapperExecutorMock = new();
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock = new();
        private readonly Mock<IOracleBulkCopyExecutor> _bulkCopyExecutor = new();

        private readonly UtilitariosRepository _repository;

        public UtilitariosRepositoryTest()
        {
            _typeConversionManagerMock = new Mock<ITypeConvertionManager>();
            _loggerMock = new Mock<ILogger<UtilitariosRepository>>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();
            _bulkCopyExecutor = new Mock<IOracleBulkCopyExecutor>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new UtilitariosRepository(
                _typeConversionManagerMock.Object,
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object,
                _bulkCopyExecutor.Object);
        }


        //TEST ObtenerParametrosDB2PorCodigoDominioDB2
        [Fact]
        public async Task ObtenerParametrosDB2PorCodigoDominioDB2_ReturnsExpectedData()
        {
            // Arrange

            // Resultado fake de Dapper
            dynamic row1 = new ExpandoObject();
            row1.CODIGOPARAMETRODB2 = 1;
            row1.CODIGODOMINIODB2 = 100;
            row1.DESCRIPCION = "Descripcion 1";
            row1.SCRIPT = "Script 1";
            row1.ESTADO = 1;
            row1.NUMORDEN = 10;

            dynamic row2 = new ExpandoObject();
            row2.CODIGOPARAMETRODB2 = 2;
            row2.CODIGODOMINIODB2 = 200;
            row2.DESCRIPCION = "Descripcion 2";
            row2.SCRIPT = "Script 2";
            row2.ESTADO = 0;
            row2.NUMORDEN = 20;

            var fakeResult = new List<dynamic> { row1, row2 };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(), // puedes poner el SP exacto si quieres más precisión
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(fakeResult);

            _typeConversionManagerMock
                .Setup(m => m.AnyToInteger(It.IsAny<object>()))
                .Returns((object o) => Convert.ToInt32(o));

            _typeConversionManagerMock
                .Setup(m => m.AnyToString(It.IsAny<object>()))
                .Returns((object o) => o?.ToString() ?? string.Empty);

            // Act
            var result = await _repository.ObtenerParametrosDB2PorCodigoDominioDB2(100, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].CodigoParametroDB2);
            Assert.Equal("Descripcion 1", result[0].Descripcion);
            Assert.Equal(10, result[0].NumOrden);

            Assert.Equal(2, result[1].CodigoParametroDB2);
            Assert.Equal("Descripcion 2", result[1].Descripcion);
            Assert.Equal(20, result[1].NumOrden);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<OracleDynamicParameters>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerParametrosDB2PorCodigoDominioDB2_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(d => d.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<OracleDynamicParameters>(),
                    CommandType.StoredProcedure))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.ObtenerParametrosDB2PorCodigoDominioDB2(100, 10));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            // ✅ Verificar que se hizo LogError correctamente
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("ObtenerParametrosDB2PorCodigoDominioDB2 - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //TEST ObtenerParametrosPorCodigoDominio
        [Fact]
        public async Task ObtenerParametrosPorCodigoDominio_ReturnsExpectedData()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.CODIGOPARAMETRO = 1;
            row.CODIGODOMINIO = 10;
            row.DESCRIPCION = "Descripcion Test";
            row.ESTADO = 1;
            row.CODIGOREFERENCIA = "REF01";
            row.NUMEROORDEN = 5;
            row.DESCRIPCIONCORTA = "Desc Corta";
            row.CODIGOREFERENCIAPARAMETRO = 99;
            row.OPCION1 = "Opcion";

            var fakeResult = new List<dynamic> { row };

            // Configurar mocks existentes
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConversionManagerMock.Setup(m => m.AnyToInteger(It.IsAny<object>()))
                                      .Returns((object o) => Convert.ToInt32(o));
            _typeConversionManagerMock.Setup(m => m.AnyToString(It.IsAny<object>()))
                                      .Returns((object o) => o?.ToString() ?? string.Empty);

            // Act
            var result = await _repository.ObtenerParametrosPorCodigoDominio(10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var item = result[0];

            Assert.Equal(1, item.CODIGOPARAMETRO);
            Assert.Equal(10, item.CODIGODOMINIO);
            Assert.Equal("Descripcion Test", item.DESCRIPCION);
            Assert.Equal(1, item.ESTADO);
            Assert.Equal("REF01", item.CODIGOREFERENCIA);
            Assert.Equal(5, item.NUMEROORDEN);
            Assert.Equal("Desc Corta", item.DESCRIPCIONCORTA);
            Assert.Equal(99, item.CODIGOREFERENCIAPARAMETRO);
            Assert.Equal("Opcion", item.OPCION1);

            _dapperExecutorMock.Verify(e => e.QueryAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CommandType?>()), Times.Once);
        }

        //TEST BulkInsertTable
        [Fact]
        public async Task ObtenerParametrosPorCodigoDominio_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(d => d.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.ObtenerParametrosPorCodigoDominio(10));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            // Verificar logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("ObtenerParametrosPorCodigoDominio - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void BulkInsertTable_WhenSuccess_ReturnsTrue()
        {
            // Arrange
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();

            connectionMock.Setup(c => c.BeginTransaction())
                          .Returns(transactionMock.Object);

            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos())
                       .Returns(connectionMock.Object);

            var dt = new DataTable();
            dt.Columns.Add("COL1");

            // Act
            var result = _repository.BulkInsertTable(dt, "TABLA_TEST");

            // Assert
            Assert.True(result);

            _bulkCopyExecutor.Verify(b =>
                b.WriteToServer(It.IsAny<IDbConnection>(), It.IsAny<DataTable>(), It.IsAny<string>()),
                Times.Once);
        }

        //TEST ForAllInsertDataObject
        [Fact]
        public async Task ForAllInsertDataObject_WhenSuccess_ReturnsTrue()
        {
            // 1. Mocks de las interfaces de conexión
            var connectionMock = new Mock<DbConnection>();

            var factoryMock = new Mock<IOracleConnectionFactory>();
            factoryMock.Setup(f => f.CrearConexionBaseDatos())
                       .Returns(connectionMock.Object);

            // 2. Mock del Executor (donde delegaste la lógica de Oracle)
            var dapperExecutorMock = new Mock<IDapperExecutor>();

            // 3. Mock del Logger y otros
            var loggerMock = new Mock<ILogger<UtilitariosRepository>>();

            // 4. Instancia del repositorio con sus dependencias
            var repo = new UtilitariosRepository(
                Mock.Of<ITypeConvertionManager>(),
                loggerMock.Object,
                factoryMock.Object,
                dapperExecutorMock.Object, // Aquí pasamos el mock que verificaremos
                Mock.Of<IOracleBulkCopyExecutor>()
            );

            // 5. Datos de entrada
            var dt = new DataTable();
            dt.Columns.Add("COL1");
            string nombreTabla = "MI_UDT_ORACLE";

            // --- ACT ---
            var result = await repo.ForAllInsertDataObject(dt, nombreTabla);

            // --- ASSERT ---

            // Verificamos que el resultado sea true
            Assert.True(result);

            // Verificamos que se llamó al OpenAsync de la conexión
            // Nota: Al ser async, Moq verifica la invocación del token de cancelación por defecto
            connectionMock.Verify(c => c.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verificamos que se llamó al Executor con los parámetros correctos
            dapperExecutorMock.Verify(e =>
                e.ExecuteForAllInsertAsync(
                    connectionMock.Object,
                    It.IsAny<CabDocumentosH2H>(),
                    nombreTabla
                ),
                Times.Once);
        }

        [Fact]
        public void BulkInsertTable_WhenErrorOccurs_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();

            // Simular transacción
            connectionMock.Setup(c => c.BeginTransaction())
                          .Returns(transactionMock.Object);

            // La fábrica de conexiones devuelve nuestra conexión mock
            _connectionFactoryMock.Setup(f => f.CrearConexionBaseDatos())
                                  .Returns(connectionMock.Object);

            // El BulkCopyExecutor lanza excepción para simular fallo
            var exception = new Exception("Error simulado");
            _bulkCopyExecutor.Setup(b => b.WriteToServer(
                                        It.IsAny<IDbConnection>(),
                                        It.IsAny<DataTable>(),
                                        It.IsAny<string>()))
                             .Throws(exception);

            // ACT & ASSERT
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _repository.BulkInsertTable(new DataTable(), "TEST"));

            // Verificamos que la excepción contenga la inner exception original
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Error simulado", ex.InnerException!.Message);

            transactionMock.Verify(t => t.Rollback(), Times.Once);

            // Verificar logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("BulkInsertTable - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task ForAllInsertDataObject_WhenException_ThrowsInvalidOperationException()
        {
            // Arrange
            _connectionFactoryMock
                .Setup(f => f.CrearConexionBaseDatos())
                .Throws(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ForAllInsertDataObject(new DataTable(), "MI_UDT_ORACLE"));

            // Verificamos que la excepción contenga la inner exception original
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            // Verificar logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("ForAllInsertDataObject - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }


        //TEST ActualizarParametroPorCodigoDominioAndNumOrden
        [Fact]
        public async Task ActualizarParametroPorCodigoDominioAndNumOrden_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(f => f.CrearConexionBaseDatos())
                .Returns(mockConnection.Object);

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1); // Simula éxito

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.ActualizarParametroPorCodigoDominioAndNumOrden(10, "DESC", 5));

            // ASSERT
            Assert.Null(exception);

            // Se llamó al procedimiento correcto
            _dapperExecutorMock.Verify(e => e.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ActualizaParametroPorDominioAndNumOrden,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once
            );
        }

        [Fact]
        public async Task ActualizarParametroPorCodigoDominioAndNumOrden_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var mockConnection = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(f => f.CrearConexionBaseDatos())
                .Returns(mockConnection.Object);

            // Simular fallo en ExecuteAsync
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ActualizarParametroPorCodigoDominioAndNumOrden(10, "DESC", 5));

            // Verificar que la excepción contiene la inner exception correcta
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            // Verificar logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("ActualizarParametroPorCodigoDominioAndNumOrden - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

    }
}
