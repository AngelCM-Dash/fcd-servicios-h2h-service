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
    public class PlanillasRepositoryTest
    {
        private readonly Mock<ITypeConvertionManager> _typeConvertionManagerMock = new();
        private readonly Mock<ILogger<PlanillasRepository>> _loggerMock = new();
        private readonly Mock<IDapperExecutor> _dapperExecutorMock = new();
        private readonly Mock<IOracleConnectionFactory> _connectionFactoryMock = new();

        private readonly PlanillasRepository _repository;

        public PlanillasRepositoryTest()
        {
            _typeConvertionManagerMock = new Mock<ITypeConvertionManager>();
            _loggerMock = new Mock<ILogger<PlanillasRepository>>();
            _dapperExecutorMock = new Mock<IDapperExecutor>();
            _connectionFactoryMock = new Mock<IOracleConnectionFactory>();

            var connectionMock = new Mock<IDbConnection>();
            _connectionFactoryMock
                .Setup(x => x.CrearConexionBaseDatos())
                .Returns(connectionMock.Object);

            _repository = new PlanillasRepository(
                _typeConvertionManagerMock.Object,
                _loggerMock.Object,
                _connectionFactoryMock.Object,
                _dapperExecutorMock.Object);
        }

        [Fact]
        public async Task RegistrarPlanillaFCD_H2H_WhenSuccess_ReturnsPlanillaConDatosAsignadosTrue()
        {
            // Arrange
            var planillaInput = new DapperPlanillaCompleta
            {
                CodigoProducto = 1,
                CodigoMoneda = 1,
                TotalDocumentosPlanilla = 10,
                ImporteTotalPlanilla = 1000,
                Usuario = "testuser",
                CodigotipoCuentaAbono = 123,
                NumeroCuentaAbono = "1111111111",
                CodigoEstado = 1,
                CodigoTienda = 101,
                Tienda = "Tienda1",
                CodigoUnico = "06666666",
                CanalAtencion = "Canal",
                CodigoPerfilUsuario = 32,
                FlagControlFlujo = 1,
                CodigoFormaOperacion = 1,
                ContratoMarco = true,
                CodigoTipoCobranza = 1,
                CodigotipoCuentaCargo = 321,
                NumeroCuentaCargo = "2222222222",
                AplicaInteresMoratorio = 0,
                ImporteTotalRegistrado = 1000m,
                AplicaInteresCompensatorio = 0,
                CodigoModalidadProducto = 1,
                CodigoTipoAdelanto = 2,
                CodigoModalidadAdelanto = 3,
                FechaAdelanto = DateTime.Now,
                CuFactor = "CUF",
                CuBancoEmisor = "BANCO",
                PlazaCuenta = "PLAZA",
                FechaDesembolso = DateTime.Now,
                AsumeInteres = 0,
                InfoTotalDocsPlanilla = 10,
                ComisionIBK = 5m,
                ComisionFactor = 2m,
                ImporteFlat = 100m,
                NumeroOperacion = "OP123",
                CodigoMonedaWDC = 1,
                CodigoMonedaCta = 1,
                ImporteWDC = 500m,
                ImporteCta = 500,
                TasaDescuento = 1.5m,
                ItemDeudor = 1,
                TipoCambioWDC = 3.5m,
                NumeroLineaFactor = 1,
                NumeroLineaCliente = "2",
                CostoFondo = 100m,
                NombreArchivo = "archivo.txt",
                NumeroPlanilla = "PLAN123"
            };


            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.NUMEROPLANILLA = "PLAN123";
            row.CODIGOCLIENTE = 789;
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.createPlanillaCabecera,
                    It.IsAny<object>(),
                    It.Is<CommandType?>(c => c == CommandType.StoredProcedure)))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToInteger(It.IsAny<object>()))
            .Returns((object o) => Convert.ToInt32(o));

            // Act
            var result = await _repository.RegistrarPlanillaFCD_H2H(planillaInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PLAN123", result.NumeroPlanilla);
            Assert.Equal(789, result.CodigoCliente);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.createPlanillaCabecera,
                It.IsAny<object>(),
                It.Is<CommandType?>(c => c == CommandType.StoredProcedure)),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarPlanillaFCD_H2H_WhenSuccess_ReturnsPlanillaConDatosAsignadosFalse()
        {
            // Arrange
            var planillaInput = new DapperPlanillaCompleta
            {
                CodigoProducto = 1,
                CodigoMoneda = 1,
                TotalDocumentosPlanilla = 10,
                ImporteTotalPlanilla = 1000,
                Usuario = "testuser",
                CodigotipoCuentaAbono = 123,
                NumeroCuentaAbono = "1111111111",
                CodigoEstado = 1,
                CodigoTienda = 101,
                Tienda = "Tienda1",
                CodigoUnico = "06666666",
                CanalAtencion = "Canal",
                CodigoPerfilUsuario = 32,
                FlagControlFlujo = 1,
                CodigoFormaOperacion = 1,
                ContratoMarco = false,
                CodigoTipoCobranza = 1,
                CodigotipoCuentaCargo = 321,
                NumeroCuentaCargo = "2222222222",
                AplicaInteresMoratorio = 0,
                ImporteTotalRegistrado = 1000m,
                AplicaInteresCompensatorio = 0,
                CodigoModalidadProducto = 1,
                CodigoTipoAdelanto = 2,
                CodigoModalidadAdelanto = 3,
                FechaAdelanto = DateTime.Now,
                CuFactor = "CUF",
                CuBancoEmisor = "BANCO",
                PlazaCuenta = "PLAZA",
                FechaDesembolso = DateTime.Now,
                AsumeInteres = 0,
                InfoTotalDocsPlanilla = 10,
                ComisionIBK = 5m,
                ComisionFactor = 2m,
                ImporteFlat = 100m,
                NumeroOperacion = "OP123",
                CodigoMonedaWDC = 1,
                CodigoMonedaCta = 1,
                ImporteWDC = 500m,
                ImporteCta = 500,
                TasaDescuento = 1.5m,
                ItemDeudor = 1,
                TipoCambioWDC = 3.5m,
                NumeroLineaFactor = 1,
                NumeroLineaCliente = "2",
                CostoFondo = 100m,
                NombreArchivo = "archivo.txt",
                NumeroPlanilla = "PLAN123"
            };

            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.NUMEROPLANILLA = "PLAN123";
            row.CODIGOCLIENTE = 789;
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.createPlanillaCabecera,
                    It.IsAny<object>(),
                    It.Is<CommandType?>(c => c == CommandType.StoredProcedure)))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToInteger(It.IsAny<object>()))
            .Returns((object o) => Convert.ToInt32(o));

            // Act
            var result = await _repository.RegistrarPlanillaFCD_H2H(planillaInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PLAN123", result.NumeroPlanilla);
            Assert.Equal(789, result.CodigoCliente);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.createPlanillaCabecera,
                It.IsAny<object>(),
                It.Is<CommandType?>(c => c == CommandType.StoredProcedure)),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarPlanillaFCD_H2H_WhenQueryFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var planillaInput = new DapperPlanillaCompleta
            {
                CodigoProducto = 1,
                CodigoMoneda = 1,
                TotalDocumentosPlanilla = 10,
                ImporteTotalPlanilla = 1000,
                Usuario = "testuser",
                CodigotipoCuentaAbono = 123,
                NumeroCuentaAbono = "1111111111",
                CodigoEstado = 1,
                CodigoTienda = 101,
                Tienda = "Tienda1",
                CodigoUnico = "06666666",
                CanalAtencion = "Canal",
                CodigoPerfilUsuario = 32,
                FlagControlFlujo = 1,
                CodigoFormaOperacion = 1,
                ContratoMarco = true,
                CodigoTipoCobranza = 1,
                CodigotipoCuentaCargo = 321,
                NumeroCuentaCargo = "2222222222",
                AplicaInteresMoratorio = 0,
                ImporteTotalRegistrado = 1000m,
                AplicaInteresCompensatorio = 0,
                CodigoModalidadProducto = 1,
                CodigoTipoAdelanto = 2,
                CodigoModalidadAdelanto = 3,
                FechaAdelanto = DateTime.Now,
                CuFactor = "CUF",
                CuBancoEmisor = "BANCO",
                PlazaCuenta = "PLAZA",
                FechaDesembolso = DateTime.Now,
                AsumeInteres = 0,
                InfoTotalDocsPlanilla = 10,
                ComisionIBK = 5m,
                ComisionFactor = 2m,
                ImporteFlat = 100m,
                NumeroOperacion = "OP123",
                CodigoMonedaWDC = 1,
                CodigoMonedaCta = 1,
                ImporteWDC = 500m,
                ImporteCta = 500,
                TasaDescuento = 1.5m,
                ItemDeudor = 1,
                TipoCambioWDC = 3.5m,
                NumeroLineaFactor = 1,
                NumeroLineaCliente = "2",
                CostoFondo = 100m,
                NombreArchivo = "archivo.txt",
                NumeroPlanilla = "PLAN123"
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
                _repository.RegistrarPlanillaFCD_H2H(planillaInput));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException.Message);

            VerifyLogError("RegistrarPlanillaFCD_H2H - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerCodigoFormacionFCD_H2H_WhenSuccess_ReturnsCodigoFormaOperacion()
        {
            // Arrange
            var planilla = new DapperPlanillaCompleta { CodigoProducto = 1 };

            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.CODIGOFORMAOPERACION = 456;
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetCodigoFormaOperacion,
                    It.IsAny<object>(),
                    It.Is<CommandType?>(c => c == CommandType.StoredProcedure)))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
                .Setup(x => x.AnyToInteger(It.IsAny<object>()))
                .Returns((object o) => Convert.ToInt32(o));

            // Act
            var result = await _repository.ObtenerCodigoFormacionFCD_H2H(planilla);

            // Assert
            Assert.Equal(456, result);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.GetCodigoFormaOperacion,
                It.IsAny<object>(),
                It.Is<CommandType?>(c => c == CommandType.StoredProcedure)),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerCodigoFormacionFCD_H2H_WhenQueryThrows_ThrowsInvalidOperationException()
        {
            // Arrange
            var planilla = new DapperPlanillaCompleta { CodigoProducto = 1 };
            var simulatedException = new Exception(Messages.ErrorBaseDatos);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetCodigoFormaOperacion,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(simulatedException);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerCodigoFormacionFCD_H2H(planilla));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(simulatedException, ex.InnerException);

            VerifyLogError("ObtenerCodigoFormacionFCD_H2H - Error en base de datos");
        }

        [Fact]
        public async Task RegistrarDocCuota_DocEstadoFCD_H2H_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE
            var planilla = new DapperPlanillaCompleta
            {
                NumeroPlanilla = "PLAN123"
            };

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.DocCuotaDocEstado,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RegistrarDocCuota_DocEstadoFCD_H2H(planilla));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.DocCuotaDocEstado,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RegistrarDocCuota_DocEstadoFCD_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var planilla = new DapperPlanillaCompleta
            {
                NumeroPlanilla = "PLAN123"
            };

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.DocCuotaDocEstado,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistrarDocCuota_DocEstadoFCD_H2H(planilla));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("RegistrarDocCuota_DocEstadoFCD_H2H - Error en base de datos");
        }

        [Fact]
        public async Task VerificarDuplicidadPlanillaFCD_H2H_WhenSuccess_ReturnsPlanillaConCodigoArchivo()
        {
            // ARRANGE
            var planilla = new DapperPlanillaCompleta
            {
                NombreArchivo = "ArchivoTest"
            };

            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.CODIGOARCHIVO = "COD123";
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetArchivoPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
                .Setup(x => x.AnyToString(It.IsAny<object>()))
                .Returns((object o) => o?.ToString() ?? string.Empty);

            // ACT
            var result = await _repository.VerificarDuplicidadPlanillaFCD_H2H(planilla);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("COD123", result.CodigoArchivo);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.GetArchivoPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task VerificarDuplicidadPlanillaFCD_H2H_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var planilla = new DapperPlanillaCompleta
            {
                NombreArchivo = "ArchivoTest"
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetArchivoPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new InvalidOperationException(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.VerificarDuplicidadPlanillaFCD_H2H(planilla));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("VerificarDuplicidadPlanillaFCD_H2H - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerDetallePlanillaMonitor_WhenSuccess_ReturnsListaDetalle()
        {
            // ARRANGE
            var numeroPlanilla = "PLAN123";

            // Simula el resultado de Dapper como lista de diccionarios dinámicos
            dynamic row = new ExpandoObject();
            row.NUMEROINTERNO = "INT001";
            row.NUMERODOCUMENTOACEPTANTE = "DOC001";
            row.TIPOOPERACION = "TIPO1";
            row.NUMERODOCUMENTOFISICO = "FIS001";
            row.ESTADO = "OK";
            row.OBSERVACION = "Sin observaciones";

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDetallesPlanillaMonitor,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
                .Setup(x => x.AnyToString(It.IsAny<object>()))
                .Returns((object o) => o?.ToString() ?? string.Empty);

            // ACT
            var result = await _repository.ObtenerDetallePlanillaMonitor(numeroPlanilla);

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);

            var item = result[0];
            Assert.Equal("INT001", item.NumeroInterno);
            Assert.Equal("DOC001", item.NumeroDocumentoAceptante);
            Assert.Equal("TIPO1", item.TipoOperacion);
            Assert.Equal("FIS001", item.NumeroDocumentoFisico);
            Assert.Equal("OK", item.Estado);
            Assert.Equal("Sin observaciones", item.Observacion);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerDetallesPlanillaMonitor,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ObtenerDetallePlanillaMonitor_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var numeroPlanilla = "PLAN123";

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDetallesPlanillaMonitor,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT 
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerDetallePlanillaMonitor(numeroPlanilla));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ObtenerDetallePlanillaMonitor - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerReservasPlanillas_WhenSuccess_ReturnsListaDetalle()
        {
            // ARRANGE
            var numeroPlanilla = "PLAN123";

            // Simula el resultado de Dapper como lista de diccionarios dinámicos
            dynamic row = new ExpandoObject();
            row.CODIGORESERVA = 123;

            var fakeResult = new List<dynamic> { row };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerReservasxPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
                .Setup(x => x.AnyToInteger(It.IsAny<object>()))
                .Returns((object o) => Convert.ToInt32(o));

            // ACT
            var result = await _repository.ObtenerReservasPlanillas(numeroPlanilla);

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(123, result[0].CodigoReserva);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerReservasxPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerReservasPlanillas_WhenThrowsException_LogsAndThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception("DB error"));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.ObtenerReservasPlanillas("PLANILLA123"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException!.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ObtenerReservasPlanillas - Error en base de datos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task EliminarReservaPlanilla_WhenSuccess_ReturnsResultado()
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
                _repository.EliminarReservaPlanilla("123", 122));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.EliminarReservasxPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task EliminarReservaPlanilla_WhenException_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.EliminarReservaPlanilla("PLANILLA123", 456));

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("EliminarReservaPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task GeneraCodigoSecuenciaDocumentosDuplicados_WhenSuccess_ReturnsResultado()
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
                _repository.GeneraCodigoSecuenciaDocumentosDuplicados());

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerCodigoSecuenciaDocumentosDuplicados,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task GeneraCodigoSecuenciaDocumentosDuplicados_WhenException_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.GeneraCodigoSecuenciaDocumentosDuplicados());

            // ASSERT
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("GeneraCodigoSecuenciaDocumentosDuplicados - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerDocumentosDuplicados_WhenSuccess_ReturnsLista()
        {
            // ARRANGE
            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.NUMERODOCUMENTOFISICO = "DOC123";
            row.TIPODOCUMENTOCOBRANZA = "TIPO1";
            row.NUMERODOCUMENTOIDENTIDAD = "ID456";
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDocumentoDuplicados,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            // ACT
            var result = await _repository.ObtenerDocumentosDuplicados("CU123", 1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("DOC123", result[0].NumeroDocumentoFisico);
            Assert.Equal("TIPO1", result[0].TipoDocumentoCobranza);
            Assert.Equal("ID456", result[0].NumeroDocumentoIdentidad);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerDocumentoDuplicados,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerDocumentosDuplicados_WhenThrowsException_LogsAndRethrows()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDocumentoDuplicados,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerDocumentosDuplicados("CU123", 1));

            VerifyLogError("ObtenerDocumentosDuplicados - Error en base de datos");
        }

        [Fact]
        public async Task ValidarFacturaCargo_WhenSuccess_ReturnsResult()
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
                _repository.ValidarFacturaCargo("0444"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ValidarFacturaPendienteCargo,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ValidarFacturaCargo_WhenThrowsException_LogsAndRethrows()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ValidarFacturaPendienteCargo,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.ValidarFacturaCargo("0444"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ValidarFacturaCargo - Error en base de datos");
        }

        [Fact]
        public async Task ValidarPermiteDuplicados_WhenSuccess_ReturnsResult()
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
                _repository.ValidarPermiteDuplicados("0444", 34));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ValidarPermiteDuplicado,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ValidarPermiteDuplicados_WhenThrowsException_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ValidarPermiteDuplicado,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.ValidarPermiteDuplicados("0444", 34));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ValidarPermiteDuplicados - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerInformacionPlanilla_WhenSuccess_ReturnsPlanilla()
        {
            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.NUMEROPLANILLA = "PL123";
            row.FECHADESEMBOLSO = DateTime.Today;
            row.CODIGOUNICO = "CU123";
            row.CODIGOCLIENTE = 100;
            row.CODIGOPRODUCTO = 10;
            row.CODIGOMONEDA = 1;
            row.TOTALDOCUMENTOSPLANILLA = 5;
            row.IMPORTETOTALPLANILLA = 1000.50;
            row.CANALATENCION = "Canal1";
            row.OBSERVACION = "Obs";
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDatosPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToInteger(It.IsAny<object>()))
            .Returns((object o) => Convert.ToInt32(o));

            _typeConvertionManagerMock
            .Setup(x => x.AnyToDecimal(It.IsAny<object>()))
            .Returns((object o) => Convert.ToDecimal(o));

            var result = await _repository.ObtenerInformacionPlanilla("PL123");



            Assert.NotNull(result);
            Assert.Equal("PL123", result.NumeroPlanilla);
        }

        [Fact]
        public async Task ObtenerInformacionPlanilla_WhenThrowsException_ThrowsInvalidOperationException()
        {
            // Arrange
            var fakeResult = new List<dynamic>(); // ← lista vacía


            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDatosPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync((fakeResult));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerInformacionPlanilla("CU123"));

            // Assert
            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);

            // Verifica que el error original fue el del null
            Assert.NotNull(ex.InnerException);
            Assert.Equal("No se encontró información para la planilla.", ex.InnerException!.Message);

            VerifyLogError("ObtenerInformacionPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerSecuenciaPlanilla_WhenSuccess_ReturnsLista()
        {
            // ARRANGE
            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.NUMEROINTERNO = "NUMINTER123";
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerSecuenciasPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            // ACT
            var result = await _repository.ObtenerSecuenciaPlanilla(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("NUMINTER123", result[0].NumeroSecuencia);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerSecuenciasPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerSecuenciaPlanilla_WhenThrowsException_LogsAndRethrows()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerSecuenciasPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerSecuenciaPlanilla(1));

            VerifyLogError("ObtenerSecuenciaPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task RegistraDocumentosPlanilla_WhenSuccess_ReturnsLista()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.InsertaDocumentosxPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RegistraDocumentosPlanilla("123"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.InsertaDocumentosxPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RegistraDocumentosPlanilla_WhenThrowsException_LogsAndRethrows()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.InsertaDocumentosxPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RegistraDocumentosPlanilla("123"));

            VerifyLogError("RegistraDocumentosPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerInformacionDocumentoPlanilla_WhenSuccess_ReturnsPlanilla()
        {
            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.CODIGOPRODUCTO = 34;
            row.CODIGOUNICO = "CU123";
            row.NUMEROLINEA = "100";
            row.IMPORTETOTAL = 1000.50;
            row.CODIGOMONEDA = 1;
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDatosDocumento,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToInteger(It.IsAny<object>()))
            .Returns((object o) => Convert.ToInt32(o));

            _typeConvertionManagerMock
            .Setup(x => x.AnyToDecimal(It.IsAny<object>()))
            .Returns((object o) => Convert.ToDecimal(o));

            var result = await _repository.ObtenerInformacionDocumentoPlanilla("PL123");

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(34, result[0].CodigoProducto);
            Assert.Equal("CU123", result[0].CodigoUnico);
            Assert.Equal("100", result[0].NumeroLineaCliente);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ObtenerDatosDocumento,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerInformacionDocumentoPlanilla_WhenThrowsException_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ObtenerDatosDocumento,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerInformacionDocumentoPlanilla("CU123"));

            VerifyLogError("ObtenerInformacionDocumentoPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task RechazoPlanilla_DocEstadoFCD_H2H_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.RejectPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.RechazoPlanilla("123"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.RejectPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task RechazoPlanilla_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.RejectPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.RechazoPlanilla("123"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("RechazoPlanilla - Error en base de datos");
        }

        [Fact]
        public async Task ObtenerConfiguracionCTL_WhenSuccess_ReturnsPlanilla()
        {
            // Arrange
            var planillaInput = new DapperPlanillaCompleta
            {
                CanalAtencion = "Canal",
                CodigoCliente = 1,
                CodigoMoneda = 1,
                NumeroPlanilla = "1234"
            };


            var fakeResult = new List<dynamic>();
            dynamic row = new ExpandoObject();
            row.CORRELATIVO = 1;
            row.ORDEN = 1;
            row.COLUMNA = "100";
            row.POSICIONINICIAL = 2;
            row.POSICIONFINAL = 15;
            row.ADICIONAL = "DATA";
            fakeResult.Add(row);

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetItemCtlH2H,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(fakeResult);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToString(It.IsAny<object>()))
            .Returns((object o) => o?.ToString() ?? string.Empty);

            _typeConvertionManagerMock
            .Setup(x => x.AnyToInteger(It.IsAny<object>()))
            .Returns((object o) => Convert.ToInt32(o));

            var result = await _repository.ObtenerConfiguracionCTL(planillaInput);

            // ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].CORRELATIVO);
            Assert.Equal(1, result[0].ORDEN);
            Assert.Equal("100", result[0].COLUMNA);
            Assert.Equal(2, result[0].POSICIONINICIAL);
            Assert.Equal(15, result[0].POSICIONFINAL);
            Assert.Equal("DATA", result[0].ADICIONAL);

            _dapperExecutorMock.Verify(x => x.QueryAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.GetItemCtlH2H,
                It.IsAny<object>(),
                CommandType.StoredProcedure), Times.Once);
        }

        [Fact]
        public async Task ObtenerConfiguracionCTL_WhenThrowsException_ThrowsInvalidOperationException()
        {
            // Arrange
            var planillaInput = new DapperPlanillaCompleta
            {
                CanalAtencion = "Canal",
                CodigoCliente = 1,
                CodigoMoneda = 1,
                NumeroPlanilla = "1234"
            };

            _dapperExecutorMock
                .Setup(x => x.QueryAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.GetItemCtlH2H,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ObtenerConfiguracionCTL(planillaInput));

            VerifyLogError("ObtenerConfiguracionCTL - Error en base de datos");
        }

        [Fact]
        public async Task ActualizarObservacionPlanilla_DocEstadoFCD_H2H_WhenSuccess_DoesNotThrow()
        {
            // ARRANGE

            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ActualizaObservacionPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            // ACT
            var exception = await Record.ExceptionAsync(() =>
                _repository.ActualizarObservacionPlanilla("123", "obs"));

            // ASSERT
            Assert.Null(exception);

            _dapperExecutorMock.Verify(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                OracleProcedures.ActualizaObservacionPlanilla,
                It.IsAny<object>(),
                CommandType.StoredProcedure),
                Times.Once);
        }

        [Fact]
        public async Task ActualizarObservacionPlanilla_WhenFails_ThrowsInvalidOperationException()
        {
            // ARRANGE
            _dapperExecutorMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    OracleProcedures.ActualizaObservacionPlanilla,
                    It.IsAny<object>(),
                    It.IsAny<CommandType?>()))
                .ThrowsAsync(new Exception(Messages.ErrorBaseDatos));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.ActualizarObservacionPlanilla("123", "obs"));

            Assert.Equal(Messages.ErrorBaseDatos, ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal(Messages.ErrorBaseDatos, ex.InnerException!.Message);

            VerifyLogError("ActualizarObservacionPlanilla - Error en base de datos");
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
