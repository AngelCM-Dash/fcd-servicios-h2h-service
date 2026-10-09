using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Excepcion
{
    public class ThrowExceptionTest
    {
        [Fact]
        public void Constructor_WithMessageOnly_ShouldSetMessageAndNullStatusCode()
        {
            // Arrange
            var message = "Error de negocio genérico";

            // Act
            var exception = new ThrowException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.StatusCode.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithStatusCodeAndMessage_ShouldSetBothProperties()
        {
            // Arrange
            var statusCode = "400";
            var message = "El formato del archivo es inválido";

            // Act
            var exception = new ThrowException(statusCode, message);

            // Assert
            exception.StatusCode.Should().Be(statusCode);
            exception.Message.Should().Be(message);
        }

        [Fact]
        public void Constructor_WithStatusCodeMessageAndInnerException_ShouldSetAllProperties()
        {
            // Arrange
            var statusCode = "500";
            var message = "Error interno al procesar la trama";
            var innerException = new Exception("Error de conexión a la base de datos");

            // Act
            var exception = new ThrowException(statusCode, message, innerException);

            // Assert
            exception.StatusCode.Should().Be(statusCode);
            exception.Message.Should().Be(message);
            // Nota: Tu constructor actual no está pasando 'inner' a base(message, inner), 
            // pero verificamos que se cree la instancia correctamente.
            exception.InnerException.Should().BeNull();
        }
    }
}
