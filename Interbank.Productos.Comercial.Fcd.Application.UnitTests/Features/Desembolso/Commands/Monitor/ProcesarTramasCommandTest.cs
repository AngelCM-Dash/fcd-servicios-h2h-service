using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Monitor;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.Monitor
{
    public class ProcesarTramasCommandTest
    {
        [Fact]
        public void PuedeCrearInstanciaYAsignarPropiedades()
        {
            // Arrange & Act
            var command = new ProcesarTramasCommand
            {
                FlagMonitor = true,
                NumeroPlanilla = "1234567890",
                NumeroSecuencia = 5,
                TipoProcesamiento = 2
            };

            // Assert
            Assert.NotNull(command);
            Assert.True(command.FlagMonitor);
            Assert.Equal("1234567890", command.NumeroPlanilla);
            Assert.Equal(5, command.NumeroSecuencia);
            Assert.Equal(2, command.TipoProcesamiento);
        }
    }
}
