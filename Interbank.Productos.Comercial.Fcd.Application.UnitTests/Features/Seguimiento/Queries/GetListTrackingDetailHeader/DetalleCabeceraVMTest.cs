using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class DetalleCabeceraVMTest
    {
        [Fact]
        public void DetalleCabeceraVM_Properties_ShouldSetAndGetCorrectValues()
        {
            // Arrange
            var model = new DetalleCabeceraVM();

            // Valores de prueba
            decimal expectedIdSeguimiento = 10.5m;
            decimal expectedIdDetalle = 20.0m;
            string expectedFecha = "2026-02-12";
            string expectedArchivo = "documento.pdf";
            string expectedPlanilla = "PL-001";
            string expectedMetodo = "CargaMasiva";
            string expectedCapa = "Infraestructura";
            string expectedDetalleObs = "Error de conexión";
            decimal expectedIdEstacion = 5m;

            // Act
            model.IdSeguimiento = expectedIdSeguimiento;
            model.IdDetalle = expectedIdDetalle;
            model.FechaRegistro = expectedFecha;
            model.NombreArchivo = expectedArchivo;
            model.NroPlanilla = expectedPlanilla;
            model.NombreMetodo = expectedMetodo;
            model.CapaObservacion = expectedCapa;
            model.DetalleObservacion = expectedDetalleObs;
            model.IdEstacion = expectedIdEstacion;

            // Assert
            model.IdSeguimiento.Should().Be(expectedIdSeguimiento);
            model.IdDetalle.Should().Be(expectedIdDetalle);
            model.FechaRegistro.Should().Be(expectedFecha);
            model.NombreArchivo.Should().Be(expectedArchivo);
            model.NroPlanilla.Should().Be(expectedPlanilla);
            model.NombreMetodo.Should().Be(expectedMetodo);
            model.CapaObservacion.Should().Be(expectedCapa);
            model.DetalleObservacion.Should().Be(expectedDetalleObs);
            model.IdEstacion.Should().Be(expectedIdEstacion);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Valor Especial con Ñ y @")]
        public void DetalleCabeceraVM_StringProperties_ShouldHandleAllStringInputs(string testValue)
        {
            // Arrange
            var model = new DetalleCabeceraVM();

            // Act
            model.FechaRegistro = testValue;
            model.NombreArchivo = testValue;
            model.NroPlanilla = testValue;
            model.NombreMetodo = testValue;
            model.CapaObservacion = testValue;
            model.DetalleObservacion = testValue;

            // Assert
            model.FechaRegistro.Should().Be(testValue);
            model.NombreArchivo.Should().Be(testValue);
            model.NroPlanilla.Should().Be(testValue);
            model.NombreMetodo.Should().Be(testValue);
            model.CapaObservacion.Should().Be(testValue);
            model.DetalleObservacion.Should().Be(testValue);
        }

        [Fact]
        public void DetalleCabeceraVM_NumericProperties_ShouldHandleEdgeValues()
        {
            // Arrange
            var model = new DetalleCabeceraVM();
            decimal minVal = decimal.MinValue;
            decimal maxVal = decimal.MaxValue;

            // Act & Assert para MinValue
            model.IdSeguimiento = minVal;
            model.IdDetalle = minVal;
            model.IdEstacion = minVal;

            model.IdSeguimiento.Should().Be(minVal);
            model.IdDetalle.Should().Be(minVal);
            model.IdEstacion.Should().Be(minVal);

            // Act & Assert para MaxValue
            model.IdSeguimiento = maxVal;
            model.IdDetalle = maxVal;
            model.IdEstacion = maxVal;

            model.IdSeguimiento.Should().Be(maxVal);
            model.IdDetalle.Should().Be(maxVal);
            model.IdEstacion.Should().Be(maxVal);
        }
    }
}
