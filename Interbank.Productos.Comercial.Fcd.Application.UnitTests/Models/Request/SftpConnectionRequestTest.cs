using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Models.Request;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Models.Request
{
    public class SftpConnectionRequestTest
    {
        [Fact]
        public void SftpConnectionRequest_Should_SetAndGetAllPropertiesCorrectly()
        {
            // Arrange
            var request = new SftpConnectionRequest();

            var expectedHost = "10.20.30.40";
            var expectedPort = 22;
            var expectedUsername = "sftp_user";
            var expectedPassword = "secure_password_123";
            var expectedRemotePath = "/upload/claims/";
            var expectedFileName = "data_2024.csv";
            var expectedProjectDir = "C:\\Projects\\App";
            var expectedKeyPath = "C:\\Keys\\private.ppk";
            var expectedFlag = 1;
            var expectedCodigoCab = 987654321.50m;
            var expectedMetodo = "POST";

            // Act
            request.Host = expectedHost;
            request.Port = expectedPort;
            request.Username = expectedUsername;
            request.Password = expectedPassword;
            request.RemotePath = expectedRemotePath;
            request.FileName = expectedFileName;
            request.ProjectDirectory = expectedProjectDir;
            request.PrivateKeyLocalFilePath = expectedKeyPath;
            request.FlagAccesoPPk = expectedFlag;
            request.CodigoCab = expectedCodigoCab;
            request.Metodo = expectedMetodo;

            // Assert
            request.Host.Should().Be(expectedHost);
            request.Port.Should().Be(expectedPort);
            request.Username.Should().Be(expectedUsername);
            request.Password.Should().Be(expectedPassword);
            request.RemotePath.Should().Be(expectedRemotePath);
            request.FileName.Should().Be(expectedFileName);
            request.ProjectDirectory.Should().Be(expectedProjectDir);
            request.PrivateKeyLocalFilePath.Should().Be(expectedKeyPath);
            request.FlagAccesoPPk.Should().Be(expectedFlag);
            request.CodigoCab.Should().Be(expectedCodigoCab);
            request.Metodo.Should().Be(expectedMetodo);
        }

        [Fact]
        public void SftpConnectionRequest_Should_InitializeNumericTypesToZero()
        {
            // Act
            var request = new SftpConnectionRequest();

            // Assert (Validación de tipos por valor)
            request.Port.Should().Be(0);
            request.FlagAccesoPPk.Should().Be(0);
            request.CodigoCab.Should().Be(0m);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        public void SftpConnectionRequest_Should_HandleEmptyOrNullStrings(string? host, string? user)
        {
            // Arrange & Act
            var request = new SftpConnectionRequest
            {
                Host = host,
                Username = user
            };

            // Assert
            request.Host.Should().Be(host);
            request.Username.Should().Be(user);
        }
    }
}
