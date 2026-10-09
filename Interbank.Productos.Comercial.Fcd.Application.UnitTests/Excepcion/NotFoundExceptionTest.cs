using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Excepcion
{
    public class NotFoundExceptionTest
    {
        [Fact]
        public void Constructor_Default_ShouldSetEmptyMessage()
        {
            // Act
            var exception = new NotFoundException();

            // Assert
            exception.Message.Should().Contain("Exception of type"); // Mensaje base de .NET
        }

        [Fact]
        public void Constructor_WithMessage_ShouldSetMessageCorrectly()
        {
            // Arrange
            var message = "Recurso no encontrado";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            exception.Message.Should().Be(message);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_ShouldSetBothCorrectly()
        {
            // Arrange
            var message = "Error de búsqueda";
            var inner = new Exception("Causa original");

            // Act
            var exception = new NotFoundException(message, inner);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().Be(inner);
        }

        [Fact]
        public void Constructor_WithNameAndKey_ShouldFormatMessageCorrectly()
        {
            // Arrange
            var name = "Usuario";
            var key = 12345;

            // Act
            var exception = new NotFoundException(name, key);

            // Assert
            // Verifica el formato: $"{name} ({key})"
            exception.Message.Should().Be("Usuario (12345)");
        }

        [Fact]
        public void Constructor_WithNameKeyAndMessage_ShouldFormatDetailedMessage()
        {
            // Arrange
            var name = "Planilla";
            var key = "PL-999";
            var detail = "No tiene permisos de acceso";

            // Act
            var exception = new NotFoundException(name, key, detail);

            // Assert
            // Verifica el formato: $"Entity '{name}' ('{key}'): {message}"
            exception.Message.Should().Be("Entity 'Planilla' ('PL-999'): No tiene permisos de acceso");
        }
    }
}
