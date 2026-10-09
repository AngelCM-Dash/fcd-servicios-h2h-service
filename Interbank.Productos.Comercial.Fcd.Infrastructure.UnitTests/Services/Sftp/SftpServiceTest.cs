using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Sftp
{
    public class SftpServiceTest
    {

        private readonly Mock<ISftpClientWrapper> _mockClient;
        private readonly Mock<ISftpClientFactory> _mockFactory;
        private readonly ILogger<SftpService> _logger;

        public SftpServiceTest()
        {
            _mockClient = new Mock<ISftpClientWrapper>();
            _mockFactory = new Mock<ISftpClientFactory>();
            _logger = new LoggerFactory().CreateLogger<SftpService>();
        }

        private SftpService CreateService()
        {
            _mockFactory.Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(_mockClient.Object);

            return new SftpService(_logger, _mockFactory.Object);
        }

        [Fact]
        public async Task Conectar_ShouldCallConnect()
        {
            // Arrange
            var service = CreateService();

            // Act
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Assert
            _mockClient.Verify(c => c.Connect(), Times.Once);
        }

        [Fact]
        public async Task ValidarArchivoExistenteSftp_FileExists_ReturnsTrue()
        {
            // Arrange
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            _mockClient.Setup(c => c.Exists("/ruta/archivo.txt")).Returns(true);
            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Act
            var result = await service.ValidarArchivoExistenteSftp("/ruta/archivo.txt");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarRutaDirectorioRemotaSftp_DirectoryExists_ReturnsTrue()
        {
            // Arrange
            _mockClient.Setup(c => c.ChangeDirectory("/ruta/directorio"));
            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Act
            var result = await service.ValidarRutaDirectorioRemotaSftp("/ruta/directorio");

            // Assert
            Assert.True(result);
            _mockClient.Verify(c => c.ChangeDirectory("/ruta/directorio"), Times.Once);
        }

        [Fact]
        public async Task LeerLineasArchivoSftp_ReturnsLines()
        {
            // Arrange
            var lines = new[] { "line1", "line2" };
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            _mockClient.Setup(c => c.ReadAllLines("/ruta/archivo.txt")).Returns(lines);

            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Act
            var result = await service.LeerLineasArchivoSftp("/ruta/archivo.txt");

            // Assert
            Assert.Equal(lines, result);
        }

        [Fact]
        public async Task SubirArchivoSftp_CallsUploadFile()
        {
            // Arrange
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

            // Act
            await service.SubirArchivoSftp(stream, "/ruta/remota.txt");

            // Assert
            _mockClient.Verify(c => c.UploadFile(stream, "/ruta/remota.txt"), Times.Once);
        }

        [Fact]
        public async Task DescargarArchivoSftp_CallsDownloadFile()
        {
            // Arrange
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Usamos un MemoryStream en lugar de un archivo real
            var memStream = new MemoryStream();

            // Capturamos el stream que le pasa el servicio
            Stream? capturedStream = null;
            _mockClient.Setup(c => c.DownloadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, Stream>((ruta, stream) => capturedStream = stream);

            var tempFile = Path.GetTempFileName();

            // Act
            await service.DescargarArchivoSftp("/ruta/remota.txt", tempFile);

            // Assert
            _mockClient.Verify(c => c.DownloadFile("/ruta/remota.txt", It.IsAny<Stream>()), Times.Once);
            Assert.NotNull(capturedStream); // Se pasó un stream al cliente

            // Limpiamos
            File.Delete(tempFile);
        }

        [Fact]
        public async Task Desconectar_ShouldCallDisconnect()
        {
            // Arrange
            _mockClient.Setup(c => c.IsConnected).Returns(true);
            var service = CreateService();
            await service.Conectar("host", 22, "user", "pass", "", false);

            // Act
            await service.Desconectar();

            // Assert
            _mockClient.Verify(c => c.Disconnect(), Times.Once);
        }
    }
}
