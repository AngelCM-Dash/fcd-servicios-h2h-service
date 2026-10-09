using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Excepciones.Queries
{
    public class GetCalculoInteresComisionQueryTests
    {
        [Fact]
        public void Constructor_Should_Set_CodigoSecuencia_Property()
        {
            // Arrange
            int expectedCodigoSecuencia = 123;

            // Act
            var query = new GetCalculoInteresComisionQuery(expectedCodigoSecuencia);

            // Assert
            Assert.Equal(expectedCodigoSecuencia, query.codigoSecuencia);
        }
    }
}
