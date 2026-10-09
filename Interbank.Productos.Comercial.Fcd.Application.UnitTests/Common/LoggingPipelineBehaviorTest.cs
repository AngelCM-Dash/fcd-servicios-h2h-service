using Interbank.Productos.Comercial.Fcd.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Common
{
    public class LoggingPipelineBehaviorTest
    {
        [Fact]
        public async Task Handle_ShouldLogInformationAndReturnResponse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingPipelineBehavior<DummyRequest, DummyResponse>>>();

            var behavior = new LoggingPipelineBehavior<DummyRequest, DummyResponse>(loggerMock.Object);

            var request = new DummyRequest();
            var expectedResponse = new DummyResponse();

            // Simula el siguiente handler en el pipeline
            RequestHandlerDelegate<DummyResponse> next = (ct) => Task.FromResult(expectedResponse);

            // Act
            var response = await behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, response);

            // Verificar log de "Handling"
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (Convert.ToString(v) ?? string.Empty).Contains("Handling DummyRequest")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);


            // Verificar log de "CleanArchitecture Request"
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (Convert.ToString(v) ?? string.Empty).Contains("CleanArchitecture Request: DummyRequest")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }

    // Clases de prueba
    public class DummyRequest { }
    public class DummyResponse { }
}


