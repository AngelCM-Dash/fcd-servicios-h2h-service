using Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Clientes.Queries.GetSuppliersList
{
    public class GetSuppliersListQueryTest
    {
        [Theory]
        [InlineData("123", "31", "0000000123", "31")]
        [InlineData("9876543210", null, "9876543210", null)]
        [InlineData("ABC123", "34", "ABC123", "34")]
        public void Constructor_AsignaPropiedades_Correctamente(string inputCodigoUnico, string? inputCodigoProducto, string expectedCodigoUnico, string? expectedCodigoProducto)
        {
            // Act
            var query = new GetSuppliersListQuery(inputCodigoUnico, inputCodigoProducto);

            // Assert
            Assert.Equal(expectedCodigoUnico, query.CodigoUnicoAceptante);
            Assert.Equal(expectedCodigoProducto, query.CodigoProducto);
        }

        [Fact]
        public void Constructor_NumeroCortoSeRellenaConCeros()
        {
            // Arrange
            string codigoCorto = "4567";

            // Act
            var query = new GetSuppliersListQuery(codigoCorto, null);

            // Assert: se rellena a 10 dígitos
            Assert.Equal("0000004567", query.CodigoUnicoAceptante);
            Assert.Null(query.CodigoProducto);
        }

        [Fact]
        public void Constructor_CodigoNoNumericoNoSeRellena()
        {
            // Arrange
            string codigoNoNumerico = "XYZ12";

            // Act
            var query = new GetSuppliersListQuery(codigoNoNumerico, "31");

            // Assert: no se modifica el valor
            Assert.Equal("XYZ12", query.CodigoUnicoAceptante);
            Assert.Equal("31", query.CodigoProducto);
        }
    }
}
