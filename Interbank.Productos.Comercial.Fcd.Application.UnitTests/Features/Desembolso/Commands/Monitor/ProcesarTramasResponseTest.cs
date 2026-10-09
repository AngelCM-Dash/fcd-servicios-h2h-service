using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasResponseTest
    {
        [Fact]
        public void PuedeCrearInstanciaYAsignarPropiedades()
        {
            // Arrange & Act
            var response = new ProcesarTramasResponse
            {
                CodigoRespuesta = "200",
                MensajeRespuesta = "Proceso exitoso"
            };

            // Assert
            Assert.NotNull(response);
            Assert.Equal("200", response.CodigoRespuesta);
            Assert.Equal("Proceso exitoso", response.MensajeRespuesta);
        }
    }
}
