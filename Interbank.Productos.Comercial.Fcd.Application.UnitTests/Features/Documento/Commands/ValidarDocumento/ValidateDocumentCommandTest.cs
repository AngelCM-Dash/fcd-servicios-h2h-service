using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentCommandTest
    {
        [Fact]
        public void ValidateDocumentCommand_Constructor_Assigns_Properties()
        {
            var command = new ValidateDocumentCommand(
                "ABC123",
                1,
                "archivo.pdf",
                0
            );

            Assert.Equal("ABC123", command.CodigoUnico);
            Assert.Equal(1, command.CodigoProducto);
            Assert.Equal("archivo.pdf", command.NombreArchivo);
            Assert.Equal(0, command.Filtro);
        }
    }
}
