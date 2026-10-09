using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Validaciones;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Validaciones
{
    public class ValidarFacturasServiceTest
    {
        [Fact]
        public void Constructor_Should_Throw_When_SftpService_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ValidarFacturasService(null!));
        }

        [Fact]
        public void Constructor_Should_Create_Instance_When_SftpService_Is_Valid()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void CrearSftpConnectionRequest_Should_Map_All_Fields_Correctly()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var datos = new List<DapperParametro>
            {
                new() { NUMEROORDEN = 1, DESCRIPCIONCORTA = "1" },
                new() { NUMEROORDEN = 2, DESCRIPCIONCORTA = "pass" },
                new() { NUMEROORDEN = 3, DESCRIPCIONCORTA = "keypath" },
                new() { NUMEROORDEN = 4, DESCRIPCIONCORTA = "host" },
                new() { NUMEROORDEN = 5, DESCRIPCIONCORTA = "22" },
                new() { NUMEROORDEN = 6, DESCRIPCIONCORTA = "user" },
                new() { NUMEROORDEN = 7, DESCRIPCIONCORTA = "/remote/" }
            };

            var result = service.CrearSftpConnectionRequest(datos, "file.txt");

            Assert.Equal(1, result.FlagAccesoPPk);
            Assert.Equal("pass", result.Password);
            Assert.Equal("file.txt", result.FileName);
        }

        [Fact]
        public void CrearSftpConnectionRequest_Should_Set_Zero_When_FlagAccesoPPk_Not_Found()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var datos = new List<DapperParametro>(); // no existe NUMEROORDEN 1

            var result = service.CrearSftpConnectionRequest(datos, "file.txt");

            Assert.Equal(0, result.FlagAccesoPPk);
        }

        [Fact]
        public void CrearSftpConnectionRequest_Should_Throw_When_Port_Is_Not_Numeric()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var datos = new List<DapperParametro>
            {
                new() { NUMEROORDEN = 5, DESCRIPCIONCORTA = "abc" } // puerto inválido
            };

            Assert.Throws<FormatException>(() =>
                service.CrearSftpConnectionRequest(datos, "file.txt"));
        }

        [Fact]
        public void CrearSftpConnectionRequest_Should_Throw_When_FlagAccesoPPk_Is_Not_Number()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var datos = new List<DapperParametro>
            {
                new DapperParametro
                {
                    NUMEROORDEN = 1,
                    DESCRIPCIONCORTA = "abc" // ← no numérico
                }
            };

            Assert.Throws<FormatException>(() =>
                service.CrearSftpConnectionRequest(datos, "file.txt"));
        }

        [Fact]
        public void InsertarProcesoDetallePlanilla_Should_Return_DataTable_With_Rows()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            string header = new string(' ', 500);
            string detail = header.Insert(70, "F")
                                  .Insert(71, "DOC123456789012345")
                                  .Insert(150, "123456789012345")
                                  .Insert(406, "CLIENTE01");

            var contenido = new[] { header, detail };

            var table = service.InsertarProcesoDetallePlanilla(1, contenido);

            Assert.Single(table.Rows);
        }

        [Fact]
        public void InsertarProcesoDetallePlanilla_Should_Return_Empty_Table_When_Only_Header()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var contenido = new[] { "HEADER" };

            var table = service.InsertarProcesoDetallePlanilla(1, contenido);

            Assert.Empty(table.Rows);
        }

        [Fact]
        public void InsertarProcesoDetallePlanilla_Should_Throw_When_Row_Is_Too_Short()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var contenido = new[] { "HEADER", "short" };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.InsertarProcesoDetallePlanilla(1, contenido));
        }

        [Fact]
        public async Task GenerarYSubirArchivoTxtAsync_Should_Call_Sftp_Methods()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var request = new List<ValidatePlanillaResponse>
            {
                new()
                {
                    NumeroDocumentoFisico = "123",
                    TipoDocumentoCobranza = "F",
                    NumeroDocumentoIdentidad = "999"
                }
            };

            var config = new SftpConnectionRequest
            {
                Host = "host",
                Port = 22,
                Username = "user",
                Password = "pass",
                FlagAccesoPPk = 1
            };

            var result = await service.GenerarYSubirArchivoTxtAsync(
                request,
                "archivo.txt",
                config,
                "/remote/"
            );

            mock.Verify(x => x.Conectar(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()), Times.Once);

            mock.Verify(x => x.SubirArchivoSftp(
                It.IsAny<Stream>(),
                It.IsAny<string>()), Times.Once);

            mock.Verify(x => x.Desconectar(), Times.Once);

            Assert.Contains("/remote/", result);
        }

        [Fact]
        public void ObtenerCodigoDocCobranza_Should_Return_Code_When_Exists()
        {
            var result = ValidarFacturasService.ObtenerCodigoDocCobranza("F");

            Assert.Equal("54", result);
        }

        [Fact]
        public void ObtenerCodigoDocCobranza_Should_Return_00_When_Not_Exists()
        {
            var result = ValidarFacturasService.ObtenerCodigoDocCobranza("X");

            Assert.Equal("00", result);
        }

        [Fact]
        public async Task GenerarYSubirArchivoTxtAsync_Should_Handle_Null_Fields()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var request = new List<ValidatePlanillaResponse>
            {
                new ValidatePlanillaResponse
                {
                    NumeroDocumentoFisico = null,
                    TipoDocumentoCobranza = null,
                    NumeroDocumentoIdentidad = null
                }
            };

            var config = new SftpConnectionRequest
            {
                Host = "host",
                Port = 22,
                Username = "user",
                Password = "pass",
                FlagAccesoPPk = 1
            };

            var result = await service.GenerarYSubirArchivoTxtAsync(
                request,
                "archivo.txt",
                config,
                "/remote/"
            );

            mock.Verify(x => x.SubirArchivoSftp(
                It.IsAny<Stream>(),
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerarYSubirArchivoTxtAsync_Should_Convert_FlagAccesoPPk_To_False()
        {
            var mock = new Mock<ISftpService>();
            var service = new ValidarFacturasService(mock.Object);

            var request = new List<ValidatePlanillaResponse>();

            var config = new SftpConnectionRequest
            {
                Host = null,
                Port = 22,
                Username = null,
                Password = null,
                PrivateKeyLocalFilePath = null,
                FlagAccesoPPk = 0
            };

            await service.GenerarYSubirArchivoTxtAsync(
                request,
                "archivo.txt",
                config,
                null!
            );

            mock.Verify(x => x.Conectar(
                string.Empty,
                22,
                string.Empty,
                string.Empty,
                string.Empty,
                false),
                Times.Once);
        }
    }
}
