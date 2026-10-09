using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class AfiliacionResponseTest
    {
        [Fact]
        public void AfiliacionResponse_ShouldAllowNulls()
        {
            // Arrange & Act
            var response = new AfiliacionResponse
            {
                CodigoRespuesta = null,
                CodigoAfiliacion = null
            };

            // Assert
            Assert.Null(response.CodigoRespuesta);
            Assert.Null(response.NombreContacto);
            Assert.Null(response.CodigoAfiliacion);
        }

        [Fact]
        public void AfiliacionResponse_ShouldBeCorrect()
        {
            var response = new AfiliacionResponse { CodigoAfiliacion = 1 };

            response.CodigoAfiliacion.Should().Be(1);
            response.NombreContacto.Should().BeNull();
        }
    }
}
