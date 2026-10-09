using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Ctl;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Ctl
{
    public class CtlServiceTest : IDisposable
    {
        private readonly string _tempDir;
        private readonly Mock<ILogger<CtlService>> _loggerMock;
        private readonly CtlService _ctlService;

        public CtlServiceTest()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            _loggerMock = new Mock<ILogger<CtlService>>();
            _ctlService = new CtlService(_loggerMock.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void GenerarArchivoCtlCargaMasiva_Should_Create_CTL_File()
        {
            // Arrange
            var archivoOrigen = "test.csv";
            var planilla = new DapperPlanillaCompleta
            {
                RutaArchivoTemp = _tempDir,
                NumeroPlanilla = "001"
            };

            var columnas = new List<DapperParametroCtl>
            {
                new DapperParametroCtl { COLUMNA = "ID", POSICIONINICIAL = 1, POSICIONFINAL = 5, ADICIONAL = "" },
                new DapperParametroCtl { COLUMNA = "NOMBRE", POSICIONINICIAL = 6, POSICIONFINAL = 25, ADICIONAL = "CHAR(20)" },
                new DapperParametroCtl { COLUMNA = "MONTO", POSICIONINICIAL = 26, POSICIONFINAL = 30, ADICIONAL = "" }
            };

            // Act
            var ctlFilePath = _ctlService.GenerarArchivoCtlCargaMasiva("DOCUMENTO", planilla, archivoOrigen, columnas);

            // Assert
            Assert.True(File.Exists(ctlFilePath));

            var content = File.ReadAllText(ctlFilePath);
            Assert.Contains("LOAD DATA", content);
            Assert.Contains("INTO TABLE DOCUMENTO", content);
            Assert.Contains("ID          position(1 : 5)", content);
            Assert.Contains("NOMBRE          position(6 : 25)CHAR(20)", content);
            Assert.Contains("MONTO          position(26 : 30)", content);

            // Verificar que logger fue llamado
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Inicio: Escritura archivo CTL")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once);
        }

        [Fact]
        public void GenerarArchivoCtlCargaMasiva_Should_Create_Directory_If_Not_Exist()
        {
            // Arrange
            var nonExistingDir = Path.Combine(_tempDir, "newdir");
            var archivoOrigen = "test.csv";

            var planilla = new DapperPlanillaCompleta
            {
                RutaArchivoTemp = nonExistingDir,
                NumeroPlanilla = "001"
            };

            var columnas = new List<DapperParametroCtl>
            {
                new DapperParametroCtl { COLUMNA = "ID", POSICIONINICIAL = 1, POSICIONFINAL = 5, ADICIONAL = "" }
            };

            // Act
            var ctlFilePath = _ctlService.GenerarArchivoCtlCargaMasiva("DOCUMENTO", planilla, archivoOrigen, columnas);

            // Assert
            Assert.True(Directory.Exists(nonExistingDir));
            Assert.True(File.Exists(ctlFilePath));
        }

        [Fact]
        public void GenerarArchivoCtlCargaMasiva_Should_Cover_All_Branches()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            // NO creamos el directorio -> para cubrir CreateDirectory

            var loggerMock = new Mock<ILogger<CtlService>>();
            var service = new CtlService(loggerMock.Object);

            var planilla = new DapperPlanillaCompleta
            {
                RutaArchivoTemp = tempDir + Path.DirectorySeparatorChar,
                NumeroPlanilla = "999"
            };

            var columnas = new List<DapperParametroCtl>
            {
                // 1️⃣ POSICIONINICIAL = 0  -> cubre positionPart vacío
                new DapperParametroCtl
                {
                    COLUMNA = "COL1",
                    POSICIONINICIAL = 0,
                    POSICIONFINAL = 0,
                    ADICIONAL = null // cubre IsNullOrEmpty
                },

                // 2️⃣ i == 1  -> entra en primer if
                new DapperParametroCtl
                {
                    COLUMNA = "COL2",
                    POSICIONINICIAL = 1,
                    POSICIONFINAL = 10,
                    ADICIONAL = "CHAR(10)"
                },

                // 3️⃣ ADICIONAL con valor y i != 1  -> cubre bloque con comillas
                new DapperParametroCtl
                    {
                        COLUMNA = "COL3",
                        POSICIONINICIAL = 11,
                        POSICIONFINAL = 20,
                        ADICIONAL = "DATE \"YYYYMMDD\""
                    }
            };

            // Act
            var result = service.GenerarArchivoCtlCargaMasiva(
                "DOCUMENTO",
                planilla,
                "archivo.csv",
                columnas);

            // Assert

            // 4️⃣ Directorio creado
            Assert.True(Directory.Exists(tempDir));

            // Archivo creado
            Assert.True(File.Exists(result));

            var content = File.ReadAllText(result);

            // positionPart vacío
            Assert.Contains("COL1          ", content);

            // position normal
            Assert.Contains("position(1 : 10)", content);

            // adicional con comillas
            Assert.Contains("YYYYMMDD", content);

            // Limpieza
            Directory.Delete(tempDir, true);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerarArchivoCtlCargaMasiva_Should_Throw_When_RutaArchivoTemp_Is_Invalid(string? ruta)
        {
            var loggerMock = new Mock<ILogger<CtlService>>();
            var service = new CtlService(loggerMock.Object);

            var planilla = new DapperPlanillaCompleta
            {
                RutaArchivoTemp = ruta!,
                NumeroPlanilla = "001"
            };

            var columnas = new List<DapperParametroCtl>
            {
                new DapperParametroCtl
                {
                    COLUMNA = "ID",
                    POSICIONINICIAL = 1,
                    POSICIONFINAL = 5
                }
            };

            Assert.Throws<ArgumentException>(() =>
                service.GenerarArchivoCtlCargaMasiva("DOCUMENTO", planilla, "test.csv", columnas));
        }
    }
}
