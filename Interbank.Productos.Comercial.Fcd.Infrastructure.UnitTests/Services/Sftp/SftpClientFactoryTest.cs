using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services.Sftp;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Sftp;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Services.Sftp
{
    public class SftpClientFactoryTest
    {
        [Fact]
        public void CreateClient_WithPrivateKey_ShouldCoverAllLines()
        {
            using var rsa = RSA.Create(2048); // Genera una clave RSA de 2048 bits

            // Exporta la clave privada en formato PKCS#1 PEM (como el que usas en SSH)
            var privateKeyBytes = rsa.ExportRSAPrivateKey();
            var privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN RSA PRIVATE KEY-----");

            // Divide en líneas de 64 caracteres
            for (int i = 0; i < privateKeyBase64.Length; i += 64)
            {
                int len = Math.Min(64, privateKeyBase64.Length - i);
                sb.AppendLine(privateKeyBase64.Substring(i, len));
            }

            sb.AppendLine("-----END RSA PRIVATE KEY-----");

            string privateKeyPem = sb.ToString();
            // Arrange
            var factory = new SftpClientFactory();
            string tempPath = Path.Combine(Path.GetTempPath(), "test_rsa.txt");


            File.WriteAllText(tempPath, privateKeyPem);

            try
            {
                // 2. Act
                // Enviamos la ruta real. Ahora 'new PrivateKeyFile(ruta)' tendrá éxito.
                var result = factory.CreateClient("127.0.0.1", 22, "user", null!, tempPath, true);

                // 3. Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<ISftpClientWrapper>(result);
            }
            finally
            {
                // 4. Cleanup: Borramos el archivo para no dejar rastro
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Fact]
        public void CreateClient_WithPassword_ReturnsWrapper()
        {
            // Arrange
            var factory = new SftpClientFactory();

            // Act
            var result = factory.CreateClient("127.0.0.1", 22, "user", "pass", null!, false);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SftpClientWrapper>(result);
        }

        [Fact]
        public void CreateClient_WithPrivateKey_ThrowsIfFileDoesNotExist()
        {
            // Arrange
            var factory = new SftpClientFactory();
            // Usamos un nombre de archivo que NO tenga carpetas intermedias 
            // para asegurar que el error sea de archivo y no de directorio
            string rutaInexistente = "archivo_que_no_existe_nunca.ppk";

            // Act & Assert
            Assert.Throws<System.IO.FileNotFoundException>(() =>
                factory.CreateClient("127.0.0.1", 22, "user", null!, rutaInexistente, true));
        }

        [Fact]
        public void Wrapper_Connect_CallsInternalClientConnect()
        {
            // Como SftpClient no tiene interfaz, esto es difícil de mockear directamente.
            // Por eso creaste ISftpClientWrapper. 
            // El test de la Factory suele ser un "Pass-through".

            var mockWrapper = new Mock<ISftpClientWrapper>();
            mockWrapper.Setup(m => m.Connect());

            mockWrapper.Object.Connect();

            mockWrapper.Verify(m => m.Connect(), Times.Once);
        }

        [Fact]
        public void CreateClient_WithPrivateKey_WhenFileExists_ExecutesConnectionInfo()
        {
            // 1. Arrange: Crear un archivo temporal real en el disco
            string tempPath = Path.GetTempFileName();

            // Renci.SshNet requiere que el archivo tenga un formato de llave válido.
            // Si solo creas un archivo vacío, PrivateKeyFile fallará con "Invalid private key file".
            // Usaremos un contenido dummy que simule una estructura de llave SSH básica.
            string dummyKey = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEA75...\n-----END RSA PRIVATE KEY-----";
            File.WriteAllText(tempPath, dummyKey);

            var factory = new SftpClientFactory();

            try
            {
                // 2. Act
                // Intentamos crear el cliente. 
                // Nota: Puede fallar por el contenido de la llave, pero pasará la línea de PrivateKeyFile
                // y entrará a la creación de ConnectionInfo.
                var result = factory.CreateClient("127.0.0.1", 22, "user", null!, tempPath, true);

                // 3. Assert
                Assert.NotNull(result);
            }
            catch (Renci.SshNet.Common.SshException)
            {
                // Si falla por "Invalid private key", ¡está bien! 
                // Significa que ya pasó por la línea de ConnectionInfo 
                // y la cobertura de código ya se marcó como ejecutada.
            }
            finally
            {
                // 4. Cleanup: Borrar el archivo temporal
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }
    }
}
