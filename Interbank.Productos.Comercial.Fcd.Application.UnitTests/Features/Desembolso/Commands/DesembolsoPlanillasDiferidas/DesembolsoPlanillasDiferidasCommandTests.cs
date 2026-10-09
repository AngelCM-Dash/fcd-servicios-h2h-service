using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Desembolso.Commands.DesembolsoPlanillasDiferidas
{
    public class DesembolsoPlanillasDiferidasCommandTests
    {
        [Fact]
        public void PuedeCrearInstancia()
        {
            // Arrange & Act
            var command = new DesembolsoPlanillasDiferidasCommand
            {
                numeroPlanilla = "123",
                numeroInstruccion = "456",
                numeroLinea = "789"
            };

            // Assert
            Assert.NotNull(command);
            Assert.Equal("123", command.numeroPlanilla);
            Assert.Equal("456", command.numeroInstruccion);
            Assert.Equal("789", command.numeroLinea);
        }
    }
}
