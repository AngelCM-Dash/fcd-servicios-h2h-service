using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Excepcion
{
    public class CustomExceptionTest
    {
        [Fact]
        public void Constructor_WithMessageOnly_ShouldSetMessageAndDefaults()
        {
            // Arrange
            var message = "Error inesperado en el sistema";

            // Act
            var exception = new CustomException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.CodigoError.Should().Be(0); // Valor por defecto de int
            exception.StatusCode.Should().Be(0);  // Valor por defecto de int
        }

        [Fact]
        public void Constructor_WithCodigoErrorAndMessage_ShouldSetProperties()
        {
            // Arrange
            var codigoError = 505;
            var message = "Error de validación de negocio";

            // Act
            var exception = new CustomException(codigoError, message);

            // Assert
            exception.CodigoError.Should().Be(codigoError);
            exception.Message.Should().Be(message);
            exception.StatusCode.Should().Be(0);
        }

        [Fact]
        public void Constructor_WithAllParameters_ShouldSetAllProperties()
        {
            // Arrange
            var codigoError = 101;
            var message = "No autorizado";
            var statusCode = 401;

            // Act
            var exception = new CustomException(codigoError, message, statusCode);

            // Assert
            exception.CodigoError.Should().Be(codigoError);
            exception.Message.Should().Be(message);
            exception.StatusCode.Should().Be(statusCode);
        }
    }
}
