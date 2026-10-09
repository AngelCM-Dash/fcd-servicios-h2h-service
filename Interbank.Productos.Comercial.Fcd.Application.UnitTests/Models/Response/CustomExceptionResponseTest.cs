using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Models.Response;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Models.Response
{
    public class CustomExceptionResponseTest
    {
        [Fact]
        public void CustomExceptionResponse_Should_SetAndGetPropertiesCorrectly()
        {
            // Arrange
            var response = new CustomExceptionResponse();
            var expectedStatus = 404;
            var expectedDetail = "El recurso solicitado no fue encontrado en el servidor.";

            // Act
            response.Status = expectedStatus;
            response.Detail = expectedDetail;

            // Assert
            response.Status.Should().Be(expectedStatus);
            response.Detail.Should().Be(expectedDetail);
        }

        [Fact]
        public void CustomExceptionResponse_Should_HandleNullDetail()
        {
            // Arrange & Act
            var response = new CustomExceptionResponse
            {
                Status = 500,
                Detail = null // Caso de prueba para nulabilidad
            };

            // Assert
            response.Status.Should().Be(500);
            response.Detail.Should().BeNull();
        }

        [Theory]
        [InlineData(400, "Solicitud incorrecta")]
        [InlineData(401, "No autorizado")]
        [InlineData(200, "")] // Caso con string vacío
        public void CustomExceptionResponse_MultipleScenarios_ShouldRetainValues(int status, string detail)
        {
            // Arrange & Act
            var response = new CustomExceptionResponse
            {
                Status = status,
                Detail = detail
            };

            // Assert
            response.Status.Should().Be(status);
            response.Detail.Should().Be(detail);
        }
    }
}
