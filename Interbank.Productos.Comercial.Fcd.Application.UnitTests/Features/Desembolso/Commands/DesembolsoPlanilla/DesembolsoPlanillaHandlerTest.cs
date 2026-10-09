using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.DesembolsoPlanilla
{
    public class DesembolsoPlanillaHandlerTest
    {
        private readonly Mock<ILogger<DesembolsoPlanillaHandler>> _loggerMock;
        private readonly Mock<IDesembolsoService> _desembolsoServiceMock;

        private readonly DesembolsoPlanillaHandler _handler;

        public DesembolsoPlanillaHandlerTest()
        {
            _loggerMock = new Mock<ILogger<DesembolsoPlanillaHandler>>();
            _desembolsoServiceMock = new Mock<IDesembolsoService>();

            _handler = new DesembolsoPlanillaHandler(
                _loggerMock.Object,
                _desembolsoServiceMock.Object);
        }

        #region Handle - Sin Reserva

        [Fact]
        public async Task Handle_ShouldCallServiceDirectly_WhenCodigoReservaIsNull()
        {
            var command = new DesembolsoPlanillaCommand
            {
                NumeroPlanilla = null,
                CodigoReserva = null
            };

            var expectedResponse = new DesembolsoResponse();

            _desembolsoServiceMock
                .Setup(x => x.EncolamientoDesembolso(It.IsAny<DesembolsoRequest>()))
                .ReturnsAsync(expectedResponse);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedResponse);
        }

        #endregion

    }
}
