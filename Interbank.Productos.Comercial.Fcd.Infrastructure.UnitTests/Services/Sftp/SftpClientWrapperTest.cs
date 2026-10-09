using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Sftp
{
    public class SftpClientWrapperTest
    {
        [Fact]
        public void Connect_CallsClientConnect()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            // Act
            wrapper.Connect();

            // Assert
            mockClient.Verify(c => c.Connect(), Times.Once);
        }

        [Fact]
        public void UploadFile_CallsClientUploadFile()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            using var stream = new MemoryStream();

            // Act
            wrapper.UploadFile(stream, "/remote/path.txt");

            // Assert
            mockClient.Verify(c => c.UploadFile(stream, "/remote/path.txt"), Times.Once);
        }

        [Fact]
        public void Dispose_WhenClientConnected_DisconnectsAndDisposesClient()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            mockClient.Setup(c => c.IsConnected).Returns(true);

            var wrapper = new SftpClientWrapper(mockClient.Object);

            // Act
            wrapper.Dispose();

            // Assert
            mockClient.Verify(c => c.Disconnect(), Times.Once);
            mockClient.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_WhenClientNotConnected_DisposesClientWithoutDisconnect()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            mockClient.Setup(c => c.IsConnected).Returns(false);

            var wrapper = new SftpClientWrapper(mockClient.Object);

            // Act
            wrapper.Dispose();

            // Assert
            mockClient.Verify(c => c.Disconnect(), Times.Never);
            mockClient.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_CalledTwice_OnlyDisposesOnce()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            // Act
            wrapper.Dispose();
            wrapper.Dispose(); // segunda llamada

            // Assert
            mockClient.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Disconnect_CallsClientDisconnect()
        {
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            wrapper.Disconnect();

            mockClient.Verify(c => c.Disconnect(), Times.Once);
        }

        [Fact]
        public void ChangeDirectory_CallsClientChangeDirectory()
        {
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            wrapper.ChangeDirectory("/remote/path");

            mockClient.Verify(c => c.ChangeDirectory("/remote/path"), Times.Once);
        }

        [Fact]
        public void Exists_CallsClientExists()
        {
            var mockClient = new Mock<ISftpClient>();
            mockClient.Setup(c => c.Exists("/remote/file")).Returns(true);

            var wrapper = new SftpClientWrapper(mockClient.Object);

            var result = wrapper.Exists("/remote/file");

            Assert.True(result);
            mockClient.Verify(c => c.Exists("/remote/file"), Times.Once);
        }

        [Fact]
        public void ReadAllLines_CallsClientReadAllLines()
        {
            var mockClient = new Mock<ISftpClient>();
            mockClient.Setup(c => c.ReadAllLines("/remote/file")).Returns(new[] { "line1", "line2" });

            var wrapper = new SftpClientWrapper(mockClient.Object);

            var lines = wrapper.ReadAllLines("/remote/file");

            Assert.Equal(2, lines.Length);
            mockClient.Verify(c => c.ReadAllLines("/remote/file"), Times.Once);
        }

        [Fact]
        public void DownloadFile_CallsClientDownloadFile()
        {
            var mockClient = new Mock<ISftpClient>();
            var wrapper = new SftpClientWrapper(mockClient.Object);

            using var stream = new MemoryStream();

            wrapper.DownloadFile("/remote/file", stream);

            mockClient.Verify(c => c.DownloadFile("/remote/file", stream), Times.Once);
        }

        [Fact]
        public void IsConnected_ReturnsClientIsConnectedValue()
        {
            // Arrange
            var mockClient = new Mock<ISftpClient>();
            mockClient.Setup(c => c.IsConnected).Returns(true);
            var wrapper = new SftpClientWrapper(mockClient.Object);

            // Act
            var result = wrapper.IsConnected;

            // Assert
            Assert.True(result);
            mockClient.Verify(c => c.IsConnected, Times.Once);
        }

        [Fact]
        public void Constructor_NullClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new SftpClientWrapper(null!));

            Assert.Equal("client", exception.ParamName);
        }
    }
}
