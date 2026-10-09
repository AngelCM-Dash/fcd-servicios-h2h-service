using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.ActualizarPlanilla
{
    public class ActualizarPlanillaHandlerTest
    {
        private readonly Mock<ILogger<ActualizarPlanillaHandler>> _loggerMock;
        private readonly Mock<ActualizarPlanillaDependencies> _depsMock;
        private readonly Mock<ITrazaService> _trazaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ActualizarPlanillaHandler _handler;

        public ActualizarPlanillaHandlerTest()
        {
            _loggerMock = new Mock<ILogger<ActualizarPlanillaHandler>>();
            _depsMock = new Mock<ActualizarPlanillaDependencies>();
            _trazaMock = new Mock<ITrazaService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new ActualizarPlanillaHandler(
                _loggerMock.Object,
                _depsMock.Object,
                _trazaMock.Object,
                _mapperMock.Object
            );
        }
    }
}
