using FluentAssertions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Excepciones.Queries
{
    public class ComisionProveedorVMTest
    {
        [Fact]
        public void ComisionProveedorVM_Should_SetAndGetPropertiesCorrectly()
        {
            // Arrange
            var viewModel = new ComisionProveedorVM();
            var expectedCodigo = "PROV-2024-001";
            var expectedDescuento = 150.50m;
            var expectedPortes = 25.00m;
            var expectedTotal = 175.50m;

            // Act
            viewModel.codigoProveedor = expectedCodigo;
            viewModel.proveedorImporteDescuento = expectedDescuento;
            viewModel.proveedorImportePortes = expectedPortes;
            viewModel.total = expectedTotal;

            // Assert
            viewModel.codigoProveedor.Should().Be(expectedCodigo);
            viewModel.proveedorImporteDescuento.Should().Be(expectedDescuento);
            viewModel.proveedorImportePortes.Should().Be(expectedPortes);
            viewModel.total.Should().Be(expectedTotal);
        }

        [Fact]
        public void ComisionProveedorVM_Should_HandleNullCodigoProveedor()
        {
            // Arrange & Act
            var viewModel = new ComisionProveedorVM
            {
                codigoProveedor = null,
                total = 100.00m
            };

            // Assert
            viewModel.codigoProveedor.Should().BeNull();
            viewModel.total.Should().Be(100.00m);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(10.5, 5.25, 15.75)]
        [InlineData(-100, 50, -50)] // Casos de valores negativos si la lógica lo permite
        public void ComisionProveedorVM_NumericConsistency_ShouldMatch(decimal desc, decimal portes, decimal total)
        {
            // Arrange & Act
            var viewModel = new ComisionProveedorVM
            {
                proveedorImporteDescuento = desc,
                proveedorImportePortes = portes,
                total = total
            };

            // Assert
            viewModel.proveedorImporteDescuento.Should().Be(desc);
            viewModel.proveedorImportePortes.Should().Be(portes);
            viewModel.total.Should().Be(total);
        }
    }
}
