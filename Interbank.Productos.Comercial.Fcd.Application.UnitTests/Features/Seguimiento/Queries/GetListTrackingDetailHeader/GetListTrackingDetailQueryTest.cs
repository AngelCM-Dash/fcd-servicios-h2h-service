using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Seguimiento.Queries.GetListTrackingDetailHeader
{
    public class GetListTrackingDetailQueryTest
    {
        [Theory]
        [InlineData("2026-01-01", "2026-01-31", "reporte.csv", "PL-001")]
        [InlineData(null, null, null, null)]
        [InlineData("", "", "", "")]
        [InlineData("  ", "  ", "  ", "  ")]
        public void GetListTrackingDetailQuery_Constructor_ShouldAssignPropertiesCorrectly(
            string? fechaDesde,
            string? fechaHasta,
            string? nombreArchivo,
            string? numeroPlanilla)
        {
            // Act
            var query = new GetListTrackingDetailQuery(fechaDesde, fechaHasta, nombreArchivo, numeroPlanilla);

            // Assert
            query.fechaDesde.Should().Be(fechaDesde);
            query.fechaHasta.Should().Be(fechaHasta);
            query.nombreArchivo.Should().Be(nombreArchivo);
            query.numeroPlanilla.Should().Be(numeroPlanilla);
        }

        [Fact]
        public void GetListTrackingDetailQuery_ShouldAllowMutation_ThroughSetters()
        {
            // Arrange
            var query = new GetListTrackingDetailQuery("original", "original", "original", "original");
            string newValue = "updated";

            // Act
            query.fechaDesde = newValue;
            query.fechaHasta = newValue;
            query.nombreArchivo = newValue;
            query.numeroPlanilla = newValue;

            // Assert
            query.fechaDesde.Should().Be(newValue);
            query.fechaHasta.Should().Be(newValue);
            query.nombreArchivo.Should().Be(newValue);
            query.numeroPlanilla.Should().Be(newValue);
        }
    }
}
