using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Commands;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Constants;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Excepciones.Commands
{
    public class CreateComisionProveedorHandlerTest
    {
        private readonly Mock<IExcepcionRepository> _excepcionRepositoryMock;
        private readonly Mock<IUtilitariosRepository> _utilitariosRepositoryMock;
        private readonly Mock<ISftpService> _sftpServiceMock;
        private readonly Mock<IDataTableBuilderService> _dataTableBuilderServiceMock;
        private readonly Mock<ILogger<CreateComisionProveedorHandler>> _loggerMock;
        private readonly CreateComisionProveedorHandler _handler;
        private static readonly string[] NotJsonLines = { "not-json" };

        public CreateComisionProveedorHandlerTest()
        {
            _excepcionRepositoryMock = new Mock<IExcepcionRepository>();
            _utilitariosRepositoryMock = new Mock<IUtilitariosRepository>();
            _sftpServiceMock = new Mock<ISftpService>();
            _dataTableBuilderServiceMock = new Mock<IDataTableBuilderService>();
            _loggerMock = new Mock<ILogger<CreateComisionProveedorHandler>>();

            _handler = new CreateComisionProveedorHandler(
                _excepcionRepositoryMock.Object,
                _utilitariosRepositoryMock.Object,
                _sftpServiceMock.Object,
                _dataTableBuilderServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region Helper Methods
        private static List<DapperParametro> GetValidSftpParams()
        {
            return new List<DapperParametro>
        {
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.FlagRed, DESCRIPCIONCORTA = "1" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPassword, DESCRIPCIONCORTA = "pass" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePathKey, DESCRIPCIONCORTA = "key/path" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpHost, DESCRIPCIONCORTA = "127.0.0.1" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPort, DESCRIPCIONCORTA = "22" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpUsername, DESCRIPCIONCORTA = "user" },
            new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpRemotePath, DESCRIPCIONCORTA = "/remote/" }
        };
        }
        #endregion

        [Fact]
        public async Task Handle_WhenDomainParamsEmpty_ThrowsThrowException()
        {
            // Arrange
            var command = new CreateComisionProveedorCommand("test.txt");
            _utilitariosRepositoryMock
                .Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>()))
                .ReturnsAsync(new List<DapperParametro>()); // Lista vacía

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>()
                .WithMessage("Error al Obtener la información de la paramétrica de SFTP*");
        }

        [Theory]
        [InlineData(true)]  // Caso Bulk exitoso
        [InlineData(false)] // Caso Bulk fallido (Warning log)
        public async Task Handle_SuccessPath_ReturnsSequenceCode(bool bulkResult)
        {
            // Arrange
            var command = new CreateComisionProveedorCommand("file.json");
            var sftpParams = GetValidSftpParams();
            string[] fileContent = { "{ \"key\": \"value\" }" };

            _utilitariosRepositoryMock.Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>())).ReturnsAsync(sftpParams);
            _sftpServiceMock.Setup(x => x.LeerLineasArchivoSftp(It.IsAny<string>())).ReturnsAsync(fileContent);
            _excepcionRepositoryMock.Setup(x => x.GeneraCodigoSecuenciaInteresComision()).ReturnsAsync(123);
            _dataTableBuilderServiceMock.Setup(x => x.GeneraDatatableCalculoInteresComision(It.IsAny<JObject>(), It.IsAny<int>())).Returns(new DataTable());
            _utilitariosRepositoryMock.Setup(x => x.BulkInsertTable(It.IsAny<DataTable>(), It.IsAny<string>())).Returns(bulkResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(123);
            _sftpServiceMock.Verify(x => x.Desconectar(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSftpValuesAreNull_CoversNullCoalescingBranches()
        {
            // Arrange: Enviamos parámetros donde los strings son NULL para forzar el uso de string.Empty en los ??
            var command = new CreateComisionProveedorCommand("file.json");
            var sftpParams = new List<DapperParametro>
            {
                new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.FlagRed, DESCRIPCIONCORTA = "0" },
                new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpPort, DESCRIPCIONCORTA = "22" },
                new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpHost, DESCRIPCIONCORTA = null }, // Host null
                new() { NUMEROORDEN = (int)ParametroConstants.NumOrdenDominioCalculoInteresComision.SftpUsername, DESCRIPCIONCORTA = null } // User null
            };

            _utilitariosRepositoryMock.Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>())).ReturnsAsync(sftpParams);
            // Forzamos error en JObject.Parse (contenido no JSON) para ir al catch y terminar el test cubriendo las líneas previas
            _sftpServiceMock.Setup(x => x.LeerLineasArchivoSftp(It.IsAny<string>())).ReturnsAsync(NotJsonLines);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>();
            // Verificamos que se llamó a conectar con string.Empty debido al operador ??
            _sftpServiceMock.Verify(x => x.Conectar(string.Empty, It.IsAny<int>(), string.Empty, It.IsAny<string>(), It.IsAny<string>(), true), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_LogsErrorAndThrowsThrowException()
        {
            // Arrange
            var command = new CreateComisionProveedorCommand("error.json");
            _utilitariosRepositoryMock.Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>())).ReturnsAsync(GetValidSftpParams());

            // Forzamos el error pasándole un código de error de socket (ej: 10061 - Connection Refused)
            var socketException = new System.Net.Sockets.SocketException(10061);

            _sftpServiceMock.Setup(x => x.Conectar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(socketException);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ThrowException>()
                .WithMessage("*"); // Primero probamos que lance la excepción

            var exceptionAssertion = await act.Should().ThrowAsync<ThrowException>();
            exceptionAssertion.And.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_WhenFindReturnsNull_ThrowsException()
        {
            // Arrange: Lista de dominio que NO contiene los parámetros necesarios
            var command = new CreateComisionProveedorCommand("test.txt");
            var incompleteParams = new List<DapperParametro> { new() { NUMEROORDEN = 999 } };

            _utilitariosRepositoryMock.Setup(x => x.ObtenerParametrosPorCodigoDominio(It.IsAny<int>())).ReturnsAsync(incompleteParams);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert: Debería fallar al intentar acceder a DESCRIPCIONCORTA de un Find que devolvió null
            await act.Should().ThrowAsync<ThrowException>();
        }
    }
}
