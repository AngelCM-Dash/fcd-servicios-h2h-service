using Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Constants;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NextSIT.Utility;
using System.Data;
using System.Dynamic;
using static Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants.DesembolsoConstants;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class DesembolsoRepositoryTest
    {
        private readonly Mock<ITypeConvertionManager> _typeConvertionManagerMock = new();
        private readonly Mock<ILogger<DesembolsoRepository>> _loggerMock = new();
        private readonly Mock<IDapperExecutor> _dapperExecutorMock = new();
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock = new();

        private readonly DesembolsoRepository _repository;

        public DesembolsoRepositoryTest()
        {
            _typeConvertionManagerMock = new Mock<ITypeConvertionManager>();
            _loggerMock = new Mock<ILogger<DesembolsoRepository>>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new DesembolsoRepository(
                _typeConvertionManagerMock.Object,
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object);
        }

        [Fact]
        public async Task ActualizarPlanillaDistribuido_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            var request = new DapperActualizarPlanilla
            {
                NumeroPlanilla = "123",
                CodigoAgrupamiento = "ABC",
                CodigoPerfilUsuario = 1,
                CodigoUsuario = "USR01",
                NombreUsuarioRegistro = "Usuario Test",
                Comentario = "Comentario",
                CanalAtencion = "WEB",
                CodigoTienda = "TI01",
                EstadoPlanilla = 2,
                EstadoDocumento = 3
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ActualizarPlanilla,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<dynamic>());

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.ActualizarPlanillaDistribuido(request));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ActualizarPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ActualizarPlanillaDistribuido_WhenQueryFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new DapperActualizarPlanilla
            {
                NumeroPlanilla = "123",
                CodigoAgrupamiento = "ABC",
                CodigoPerfilUsuario = 1,
                CodigoUsuario = "USR01",
                NombreUsuarioRegistro = "Usuario Test",
                Comentario = "Comentario",
                CanalAtencion = "WEB",
                CodigoTienda = "TI01",
                EstadoPlanilla = 2,
                EstadoDocumento = 3
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ActualizarPlanillaDistribuido(request));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ActualizarPlanillaDistribuido - Error en base de datos");
        }

        [Fact]
        public async Task ActualizarPlanillaDistribuidoFCD_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            var request = new DapperActualizarPlanilla
            {
                NumeroPlanilla = "123",
                CodigoAgrupamiento = "ABC",
                CodigoPerfilUsuario = 1,
                CodigoUsuario = "USR01",
                NombreUsuarioRegistro = "Usuario Test",
                Comentario = "Comentario",
                CanalAtencion = "WEB",
                CodigoTienda = "TI01",
                EstadoPlanilla = 2,
                EstadoDocumento = 3
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ActualizarPlanillaFCD,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<dynamic>());

            // Act
            var exception = await Record.ExceptionAsync(() =>
                _repository.ActualizarPlanillaDistribuidoFCD(request));

            // Assert
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ActualizarPlanillaFCD,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ActualizarPlanillaDistribuidoFCD_WhenQueryFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new DapperActualizarPlanilla
            {
                NumeroPlanilla = "123",
                CodigoAgrupamiento = "ABC",
                CodigoPerfilUsuario = 1,
                CodigoUsuario = "USR01",
                NombreUsuarioRegistro = "Usuario Test",
                Comentario = "Comentario",
                CanalAtencion = "WEB",
                CodigoTienda = "TI01",
                EstadoPlanilla = 2,
                EstadoDocumento = 3
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ActualizarPlanillaDistribuidoFCD(request));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ActualizarPlanillaDistribuidoFCD - Error en base de datos");
        }

        [Fact]
        public async Task FintNextPlanillaSecuencia_WhenSuccess_ReturnsSecuencia()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.SECUENCIA = 25;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.SeleccionarNumeroSecPlanilla,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(fakeResult);

            // Act
            var result = await _repository.fintNextPlanillaSecuencia("123");

            // Assert
            Assert.Equal("25", result);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.SeleccionarNumeroSecPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task FintNextPlanillaSecuencia_WhenEmptyResult_ReturnsZero()
        {
            // Arrange
            var fakeResult = new List<dynamic>();

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            // Act
            var result = await _repository.fintNextPlanillaSecuencia("123");

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public async Task FintNextPlanillaSecuencia_WhenQueryFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.fintNextPlanillaSecuencia("123"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("fintNextPlanillaSecuencia - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerTramas_WhenSuccess_ReturnsDataTableWithRows()
        {
            // Arrange
            var input = new DapperGenerarTramasInput
            {
                NumeroPlanilla = "123",
                NumeroInstruccion = "ABC",
                NumeroPlanillaSecuencia = 1,
                UsuarioEjecuta = "USR01",
                FlagDesembolsar = 1
            };

            // Creamos un DataTable fake para simular el reader
            var fakeTable = new DataTable();
            fakeTable.Columns.Add("Col1");
            fakeTable.Rows.Add("Valor1");

            var reader = fakeTable.CreateDataReader();

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GenerarTramas,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader);

            // Act
            var result = await _repository.ObtenerTramas(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Rows);
            Assert.Equal("Valor1", result.Rows[0]["Col1"]);
        }

        [Fact]
        public async Task ObtenerTramas_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var input = new DapperGenerarTramasInput
            {
                NumeroPlanilla = "123",
                NumeroInstruccion = "ABC",
                NumeroPlanillaSecuencia = 1,
                UsuarioEjecuta = "USR01",
                FlagDesembolsar = 1
            };

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerTramas(input));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ObtenerTramas - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerFlujoDesembolso_WhenSuccess_ReturnsMappedList()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.CODIGOPARAMETRO = 1;
            row.CODIGODOMINIO = 2;
            row.DESCRIPCION = "Desc";
            row.ESTADO = 1;
            row.CODIGOREFERENCIA = "REF";
            row.NUMEROORDEN = 10;
            row.DESCRIPCIONCORTA = "DC";
            row.CODIGOREFERENCIAPARAMETRO = 5;
            row.OPCION1 = "OP1";

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerFLujoDesembolso,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(fakeResult);

            // Configuramos conversiones
            _typeConvertionManagerMock.Setup(x => x.AnyToInteger(It.IsAny<object>())).Returns<int>(x => Convert.ToInt32(x));
            _typeConvertionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>())).Returns<string>(x => x?.ToString() ?? "");

            // Act
            var result = await _repository.ObtenerFlujoDesembolso("REF", "DC");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].CODIGOPARAMETRO);
            Assert.Equal("Desc", result[0].DESCRIPCION);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerFLujoDesembolso,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerFlujoDesembolso_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerFlujoDesembolso("REF", "DC"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ObtenerFlujoDesembolso - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerDatosDesembolso_WhenSuccess_ReturnsMappedList()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.NUMEROINSTRUCCION = "INST1";
            row.NUMEROLINEA = "LINE1";
            row.NETEO = 100.5m;
            row.TIPO = "TIPO1";
            row.CODIGOMONEDAWBC = 1;
            row.NUMEROOPERACION = "OP1";

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock.Setup(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerInformacionConsumoLineas,
                It.IsAny<object>(),
                CommandType.StoredProcedure))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>())).Returns((object o) => o?.ToString()!);
            _typeConvertionManagerMock.Setup(x => x.AnyToDecimal(It.IsAny<object>())).Returns((object o) => Convert.ToDecimal(o));
            _typeConvertionManagerMock.Setup(x => x.AnyToInteger(It.IsAny<object>())).Returns((object o) => Convert.ToInt32(o));

            // Act
            var result = await _repository.ObtenerDatosDesembolso("PLAN1", 1, "TIPO");

            // Assert
            Assert.Single(result);
            Assert.Equal("INST1", result[0].NumeroInstruccion);
            Assert.Equal("LINE1", result[0].NumeroLinea);
            Assert.Equal(100.5m, result[0].Neteo);
        }

        [Fact]
        public async Task ObtenerDatosDesembolso_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerDatosDesembolso("REF", 1, "X"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ObtenerDatosDesembolso - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarTramasProcesadadas_WhenSuccess_ReturnsRespuesta()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.RESPUESTACODIGO = 1;
            row.RESPUESTAMENSAJE = "OK";

            _dapperExecutorMock.Setup(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertarTramasProcesadas,
                It.IsAny<object>(),
                CommandType.StoredProcedure))
                .ReturnsAsync(new List<dynamic> { row });

            _typeConvertionManagerMock.Setup(x => x.AnyToInteger(It.IsAny<object>())).Returns(1);
            _typeConvertionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>())).Returns("OK");

            // Act
            var result = await _repository.RegistrarTramasProcesadadas("PLAN1", 1, "USR");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.RespuestaCodigo);
            Assert.Equal("OK", result.RespuestaMensaje);
        }

        [Fact]
        public async Task RegistrarTramasProcesadadas_WhenResponseIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var fakeResult = new List<dynamic>(); // ← lista vacía

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarTramasProcesadadas("123", 1, "USR"));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            // Verifica que el error original fue el del null
            Assert.NotNull(ex.InnerException);
            Assert.Equal("La respuesta del servidor está vacía.", ex.InnerException!.Message);

            VerifyLogError("RegistrarTramasProcesadadas - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarMovimientos_WhenSuccess_ReturnsRespuesta()
        {
            // Arrange
            dynamic row = new ExpandoObject();
            row.RESPUESTACODIGO = 1;
            row.RESPUESTAMENSAJE = "OK";

            _dapperExecutorMock.Setup(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertarMovimientosDesembolso,
                It.IsAny<object>(),
                CommandType.StoredProcedure))
                .ReturnsAsync(new List<dynamic> { row });

            _typeConvertionManagerMock.Setup(x => x.AnyToInteger(It.IsAny<object>())).Returns(1);
            _typeConvertionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>())).Returns("OK");

            // Act
            var result = await _repository.RegistrarMovimientos("PLAN1", 1, "USR", "INSTRUC");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.RespuestaCodigo);
            Assert.Equal("OK", result.RespuestaMensaje);
        }

        [Fact]
        public async Task RegistrarMovimientos_WhenResponseIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var fakeResult = new List<dynamic>(); // ← lista vacía

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarMovimientos("123", 1, "USR", "INSTRUC"));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            // Verifica que el error original fue el del null
            Assert.NotNull(ex.InnerException);
            Assert.Equal("La respuesta del servidor está vacía.", ex.InnerException!.Message);

            VerifyLogError("RegistrarMovimientos - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerPlanillasDiferidas_WhenSuccess_ReturnsMappedList()
        {
            dynamic row = new ExpandoObject();
            row.NUMEROPLANILLA = "PLAN1";
            row.NUMEROINSTRUCCION = "INST1";
            row.NUMEROLINEA = 2;
            row.IMPORTEPLANILLA = 500.5m;

            _dapperExecutorMock.Setup(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerListadoPlanillasDiferidas,
                It.IsAny<object>(),
                CommandType.StoredProcedure))
                .ReturnsAsync(new List<dynamic> { row });

            _typeConvertionManagerMock.Setup(x => x.AnyToString(It.IsAny<object>())).Returns((object o) => o?.ToString()!);
            _typeConvertionManagerMock.Setup(x => x.AnyToInteger(It.IsAny<object>())).Returns((object o) => Convert.ToInt32(o));
            _typeConvertionManagerMock.Setup(x => x.AnyToDecimal(It.IsAny<object>())).Returns((object o) => Convert.ToDecimal(o));

            var result = await _repository.ObtenerPlanillasDiferidas("TIPO");

            Assert.Single(result);
            Assert.Equal("PLAN1", result[0].NumeroPlanilla);
        }

        [Fact]
        public async Task ObtenerPlanillasDiferidas_WhenNoData_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerPlanillasDiferidas("PLAN1"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);

            VerifyLogError("ObtenerPlanillasDiferidas - Error en base de datos");
        }

        [Fact]
        public async Task RechazarDocumentosDiferidos_WhenSuccess_CallsExecuteAsync()
        {
            _dapperExecutorMock.Setup(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.RechazaDocumentoDiferidos,
                It.IsAny<object>(),
                CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repository.RechazarDocumentosDiferidos("PLAN1", "INST1", "LINE1", "OBS");

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.RechazaDocumentoDiferidos,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task RechazarDocumentosDiferidos_WhenNoData_ThrowsInvalidOperationException()
        {
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
                _repository.RechazarDocumentosDiferidos("PLAN1", "INST1", "LINE1", "OBS"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);

            VerifyLogError("RechazarDocumentosDiferidos - Error en base de datos");
        }

        [Fact]
        public async Task GenerarTramaDiferidos_WhenSuccess_ReturnsDataTableWithRows()
        {
            // Arrange
            // Creamos un DataTable fake para simular el reader
            var fakeTable = new DataTable();
            fakeTable.Columns.Add("Col1");
            fakeTable.Rows.Add("Valor1");

            var reader = fakeTable.CreateDataReader();

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GeneraTramaDiferidos,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader);

            // Act
            var result = await _repository.GenerarTramaDiferidos("", "", "");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Rows);
            Assert.Equal("Valor1", result.Rows[0]["Col1"]);
        }

        [Fact]
        public async Task GenerarTramaDiferidos_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.GenerarTramaDiferidos("", "", ""));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("GenerarTramaDiferidos - Error en base de datos");
        }

        [Fact]
        public async Task ObtieneTramaDiferidos_WhenSuccess_ReturnsDataTableWithRows()
        {
            // Arrange
            // Creamos un DataTable fake para simular el reader
            var fakeTable = new DataTable();
            fakeTable.Columns.Add("Col1");
            fakeTable.Rows.Add("Valor1");

            var reader = fakeTable.CreateDataReader();

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerTramaDiferidos,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader);

            // Act
            var result = await _repository.ObtieneTramaDiferidos("NRO", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Rows);
            Assert.Equal("Valor1", result.Rows[0]["Col1"]);
        }

        [Fact]
        public async Task ObtieneTramaDiferidos_WhenFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtieneTramaDiferidos("NRO", 1));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ObtieneTramaDiferidos - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarPlanillaDietario_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RegistrarPlanillaDietario("123"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaPlanillaDietario,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarPlanillaDietario_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarPlanillaDietario("123"));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("RegistrarPlanillaDietario - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarMensajeErrorMonitor_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1); // Simula ejecución correcta

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RegistrarMensajeErrorMonitor("123", 1, "Error prueba"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaMensajeErrorMonitor,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarMensajeErrorMonitor_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarMensajeErrorMonitor("123", 1, "Error prueba"));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("RegistrarMensajeErrorMonitor - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerMensajeErrorMonitorPorPlanilla_WhenSuccess_ReturnsCodigoRespuesta()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(new List<dynamic>());

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.ObtenerMensajeErrorMonitorPorPlanilla("123"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerMensajeErrorMonitorxPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerMensajeErrorMonitorPorPlanilla_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerMensajeErrorMonitorPorPlanilla("123"));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerMensajeErrorMonitorPorPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarDesembolsoAbono_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1); // Simula ejecución correcta

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RegistrarDesembolsoAbono(
                    "123", 1, "PROV001", 2, 1, 3, 10));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaDesembolsoAbonoFallido,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarDesembolsoAbono_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarDesembolsoAbono(
                    "123", 1, "PROV001", 2, 1, 3, 10));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("RegistrarDesembolsoAbono - Error en base de datos");
        }

        [Fact]
        public async Task ActualizarEstadoDesembolsoAbono_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1); // Simula ejecución correcta

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.ActualizarEstadoDesembolsoAbono(
                    "123", 1, "PROV001", 1, 10));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ActualizarDesembolsoAbonoFallido,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ActualizarEstadoDesembolsoAbono_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ActualizarEstadoDesembolsoAbono(
                    "123", 1, "PROV001", 1, 10));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ActualizarEstadoDesembolsoAbono - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerDesembolsoAbono_WhenSuccess_ReturnsList()
        {
            // ARRANGE
            dynamic row = new ExpandoObject();
            row.NUMEROPLANILLA = "PL123";
            row.NUMEROSECUENCIA = 1;
            row.NUMEROREGISTROSPROCESADOS = 10;
            row.CODIGOUNICOPROVEEDOR = "PROV001";
            row.NUMEROREINTENTO = 0;
            row.ESTADOREINTENTO = 1;
            row.TIPOREINTENTO = 2;
            row.FECHAREGISTRO = DateTime.Now;
            row.FECHACTUALIZACION = DateTime.Now;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock.Setup(m => m.AnyToString(It.IsAny<object>()))
                                      .Returns((object o) => o?.ToString() ?? string.Empty);
            _typeConvertionManagerMock.Setup(m => m.AnyToInteger(It.IsAny<object>()))
                                      .Returns((object o) => Convert.ToInt32(o));
            _typeConvertionManagerMock.Setup(m => m.AnyToDateTime(It.IsAny<object>(), It.IsAny<bool>()))
                                      .Returns((object o, bool _) => Convert.ToDateTime(o));

            // ACT
            var result = await _repository.ObtenerDesembolsoAbono("PL123", 1, "PROV001");

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);

            var item = result[0];
            Assert.Equal("PL123", item.NumeroPlanilla);
            Assert.Equal(1, item.NumeroSecuencia);
            Assert.Equal(10, item.NumeroRegistrosProcesados);
            Assert.Equal("PROV001", item.CodigoUnicoProveedor);
            Assert.Equal(0, item.NumeroReintento);
            Assert.Equal(1, item.EstadoReintento);
            Assert.Equal(2, item.TipoReintento);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerEstadoDesembolsoAbonoFallido,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerDesembolsoAbono_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerDesembolsoAbono("PL123", 1, "PROV001"));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerDesembolsoAbono - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerTramasParcialTotalPorProveedores_WhenSuccess_ReturnsDataTable()
        {
            // ARRANGE
            var generarTramasInput = new DapperGenerarTramasInput
            {
                NumeroPlanilla = "PL123",
                NumeroInstruccion = "INST001",
                CodigoUnicoProveedor = "PROV001",
                NumeroPlanillaSecuencia = 1,
                UsuarioEjecuta = "USER1",
                FlagDesembolsar = 1
            };

            // Creamos un DataTable fake para simular el reader
            var fakeTable = new DataTable();
            fakeTable.Columns.Add("Col1");
            fakeTable.Rows.Add("Valor1");

            var reader = fakeTable.CreateDataReader();

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GenerarTramasParcialTotalPorProveedores,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader);

            // ACT
            var dt = await _repository.ObtenerTramasParcialTotalPorProveedores(generarTramasInput, (int)TipoReintentoProcesoAbono.ParcialTotal);

            // Assert
            Assert.NotNull(dt);
            Assert.Single(dt.Rows);
            Assert.Equal("Valor1", dt.Rows[0]["Col1"]);
        }

        [Fact]
        public async Task ObtenerTramasParcialTotalPorProveedores_WhenSuccess_ReturnsDataTable2()
        {
            // ARRANGE
            var generarTramasInput = new DapperGenerarTramasInput
            {
                NumeroPlanilla = "PL123",
                NumeroInstruccion = "INST001",
                CodigoUnicoProveedor = "PROV001",
                NumeroPlanillaSecuencia = 1,
                UsuarioEjecuta = "USER1",
                FlagDesembolsar = 1
            };

            // Creamos un DataTable fake para simular el reader
            var fakeTable = new DataTable();
            fakeTable.Columns.Add("Col1");
            fakeTable.Rows.Add("Valor1");

            var reader = fakeTable.CreateDataReader();

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GenerarTramasPorProveedor,
                    It.IsAny<object>(),
                    CommandType.StoredProcedure,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader);

            // ACT
            var dt = await _repository.ObtenerTramasParcialTotalPorProveedores(generarTramasInput, (int)TipoReintentoProcesoAbono.Parcial);

            // Assert
            Assert.NotNull(dt);
            Assert.Single(dt.Rows);
            Assert.Equal("Valor1", dt.Rows[0]["Col1"]);
        }

        [Fact]
        public async Task ObtenerTramasParcialTotalPorProveedores_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var generarTramasInput = new DapperGenerarTramasInput
            {
                NumeroPlanilla = "PL123",
                NumeroInstruccion = "INST001",
                CodigoUnicoProveedor = "PROV001",
                NumeroPlanillaSecuencia = 1,
                UsuarioEjecuta = "USER1",
                FlagDesembolsar = 1
            };

            _dapperExecutorMock
                .Setup(x => x.ExecuteReaderAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerTramasParcialTotalPorProveedores(generarTramasInput, (int)TipoReintentoProcesoAbono.ParcialTotal));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("ObtenerTramasParcialTotalPorProveedores - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerNroOperacionDesembolso_WhenSuccess_ReturnsList()
        {
            // ARRANGE
            dynamic row = new ExpandoObject();
            row.NUMEROOPERACION = "OP123";
            row.NUMEROINSTRUCCION = "INST001";
            row.NUMEROLINEA = 1;
            row.NETEO = 100.50m;
            row.TIPO = "T";
            row.CODIGOMONEDAWBC = 1;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock.Setup(m => m.AnyToString(It.IsAny<object>()))
                                      .Returns((object o) => o?.ToString() ?? string.Empty);
            _typeConvertionManagerMock.Setup(m => m.AnyToInteger(It.IsAny<object>()))
                                      .Returns((object o) => Convert.ToInt32(o));
            _typeConvertionManagerMock.Setup(m => m.AnyToDecimal(It.IsAny<object>()))
                                      .Returns((object o) => Convert.ToDecimal(o));

            // ACT
            var result = await _repository.ObtenerNroOperacionDesembolso("PL123", "1");

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);

            var item = result[0];
            Assert.Equal("OP123", item.NumeroOperacion);
            Assert.Equal("INST001", item.NumeroInstruccion);
            Assert.Equal(1, item.NumeroLinea);
            Assert.Equal(100.50m, item.Neteo);
            Assert.Equal("T", item.Tipo);
            Assert.Equal(1, item.CodigoMonedaWbc);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerNumeroOperacionDesembolso,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerNroOperacionDesembolso_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerNroOperacionDesembolso("PL123", "1"));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerNroOperacionDesembolso - Error en base de datos");
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
