using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Models.Response;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Models.Response
{
    public class CargaArchivoResponseTest
    {
        [Fact]
        public void CargaArchivoResponse_Properties_ShouldSetAndRetrieveCorrectValues()
        {
            // Arrange
            var response = new CargaArchivoResponse();

            var expectedNombre = "nomina_enero.txt";
            var expectedRuta = "/sftp/cargas/procesados/";
            var expectedRegistros = "150";
            var expectedResultado = "Procesado con éxito";
            var expectedCodigo = "00";

            // Act
            response.ArchivoTxtNombre = expectedNombre;
            response.ArchivoTxtRuta = expectedRuta;
            response.ArchivoTxtRegistros = expectedRegistros;
            response.Resultado = expectedResultado;
            response.CodigoRespuesta = expectedCodigo;

            // Assert (FluentAssertions)
            response.ArchivoTxtNombre.Should().Be(expectedNombre);
            response.ArchivoTxtRuta.Should().Be(expectedRuta);
            response.ArchivoTxtRegistros.Should().Be(expectedRegistros);
            response.Resultado.Should().Be(expectedResultado);
            response.CodigoRespuesta.Should().Be(expectedCodigo);
        }

        [Fact]
        public void CargaArchivoResponse_ShouldAllowNullValues()
        {
            // Arrange & Act
            var response = new CargaArchivoResponse
            {
                ArchivoTxtNombre = null,
                ArchivoTxtRuta = null,
                ArchivoTxtRegistros = null,
                Resultado = null,
                CodigoRespuesta = null
            };

            // Assert
            response.ArchivoTxtNombre.Should().BeNull();
            response.ArchivoTxtRuta.Should().BeNull();
            response.ArchivoTxtRegistros.Should().BeNull();
            response.Resultado.Should().BeNull();
            response.CodigoRespuesta.Should().BeNull();
        }

        [Theory]
        [InlineData("", "", "0")]
        [InlineData("file.txt", "/tmp/", "10")]
        public void CargaArchivoResponse_MultipleScenarios_ShouldWorkCorrectly(string nombre, string ruta, string registros)
        {
            // Arrange & Act
            var response = new CargaArchivoResponse
            {
                ArchivoTxtNombre = nombre,
                ArchivoTxtRuta = ruta,
                ArchivoTxtRegistros = registros
            };

            // Assert
            response.ArchivoTxtNombre.Should().Be(nombre);
            response.ArchivoTxtRuta.Should().Be(ruta);
            response.ArchivoTxtRegistros.Should().Be(registros);
        }
    }
}
