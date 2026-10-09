using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Documento.Commands.ValidarDocumento
{
    public class ValidateDocumentValidatorTest
    {
        private readonly ValidateDocumentValidator _validator = new();

        [Fact]
        public void Should_fail_when_filtro_is_invalid()
        {
            var cmd = new ValidateDocumentCommand("1234567890", 31, "file.txt", 3);

            var result = _validator.Validate(cmd);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Filtro");
        }

        [Fact]
        public void Filtro_1_requires_all_fields_valid()
        {
            var cmd = new ValidateDocumentCommand("1234567890", 31, "file.txt", 1);

            var result = _validator.Validate(cmd);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_fail_when_codigo_unico_length_is_invalid()
        {
            var cmd = new ValidateDocumentCommand("123", 31, "file.txt", 1);

            var result = _validator.Validate(cmd);

            Assert.Contains(result.Errors, e => e.PropertyName == "CodigoUnico");
        }

        [Fact]
        public void Should_fail_when_codigo_producto_is_not_allowed()
        {
            var cmd = new ValidateDocumentCommand("1234567890", 99, "file.txt", 1);

            var result = _validator.Validate(cmd);

            Assert.Contains(result.Errors, e => e.PropertyName == "CodigoProducto");
        }

        [Fact]
        public void NombreArchivo_is_not_validated_when_filtro_is_2()
        {
            var cmd = new ValidateDocumentCommand("1234567890", 31, null, 2);

            var result = _validator.Validate(cmd);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CodigoProducto_is_validated_when_filtro_is_2()
        {
            var cmd = new ValidateDocumentCommand(
                "1234567890",
                31,
                null,
                2
            );

            var result = _validator.Validate(cmd);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_fail_when_nombre_archivo_extension_is_invalid()
        {
            var cmd = new ValidateDocumentCommand("1234567890", 31, "file.pdf", 1);

            var result = _validator.Validate(cmd);

            Assert.Contains(result.Errors, e => e.PropertyName == "NombreArchivo");
        }

    }
}
