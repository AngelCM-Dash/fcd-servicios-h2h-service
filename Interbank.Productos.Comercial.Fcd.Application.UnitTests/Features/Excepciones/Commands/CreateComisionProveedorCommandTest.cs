using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Commands;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Excepciones.Commands
{
    public class CreateComisionProveedorCommandTest
    {
        [Fact]
        public void Constructor_Should_Set_NombreArchivo_Property()
        {
            // Arrange
            string expectedNombreArchivo = "archivo_prueba.txt";

            // Act
            var command = new CreateComisionProveedorCommand(expectedNombreArchivo);

            // Assert
            Assert.Equal(expectedNombreArchivo, command.nombreArchivo);
        }
    }
}
