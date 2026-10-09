using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Planilla.Commands.ValidarPlanilla
{
    public class ValidatePlanillaResponseTest
    {
        [Fact]
        public void ValidatePlanillaResponse_Should_StoreValuesCorrectly()
        {
            // Arrange
            var response = new ValidatePlanillaResponse();
            var expectedDocFisico = "F001-000123";
            var expectedTipoDoc = "FACTURA";
            var expectedDocIdentidad = "20100012345";

            // Act
            response.NumeroDocumentoFisico = expectedDocFisico;
            response.TipoDocumentoCobranza = expectedTipoDoc;
            response.NumeroDocumentoIdentidad = expectedDocIdentidad;

            // Assert
            response.NumeroDocumentoFisico.Should().Be(expectedDocFisico);
            response.TipoDocumentoCobranza.Should().Be(expectedTipoDoc);
            response.NumeroDocumentoIdentidad.Should().Be(expectedDocIdentidad);
        }

        [Fact]
        public void ValidatePlanillaResponse_Should_AllowNulls()
        {
            // Arrange & Act
            var response = new ValidatePlanillaResponse
            {
                NumeroDocumentoFisico = null,
                TipoDocumentoCobranza = null,
                NumeroDocumentoIdentidad = null
            };

            // Assert
            response.NumeroDocumentoFisico.Should().BeNull();
            response.TipoDocumentoCobranza.Should().BeNull();
            response.NumeroDocumentoIdentidad.Should().BeNull();
        }

        [Theory]
        [InlineData("DOC001", "BOL", "45678901")]
        [InlineData("", "", "")]
        public void ValidatePlanillaResponse_DataConsistency_Test(string docFisico, string tipo, string identidad)
        {
            // Arrange & Act
            var response = new ValidatePlanillaResponse
            {
                NumeroDocumentoFisico = docFisico,
                TipoDocumentoCobranza = tipo,
                NumeroDocumentoIdentidad = identidad
            };

            // Assert
            response.NumeroDocumentoFisico.Should().Be(docFisico);
            response.TipoDocumentoCobranza.Should().Be(tipo);
            response.NumeroDocumentoIdentidad.Should().Be(identidad);
        }
    }
}
