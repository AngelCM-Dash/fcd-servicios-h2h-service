using Interbank.Productos.Comercial.Fcd.Application.Features.Afiliacion.Commands.ProveedorAfiliacion;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Afiliacion.Commands.ProveedorAfiliacion
{
    public class ClienteProveedorAfiliacionTest
    {
        [Fact]
        public void ClienteProveedorAfiliacion_PropertyAssignment_ShouldWorkCorrectly()
        {
            // Arrange
            var model = new ClienteProveedorAfiliacion
            {
                // Act
                CodigoUnico = "CU123",
                CodigoCliente = 1001,
                Codigotipodocumento = 1,
                NumeroDocumento = "20123456789",
                Segmento = "Corporativo",
                CodigoEjecutivo = "E001",
                NombreEjecutivo = "Juan Perez",
                RazonSocial = "Empresa Test S.A.C.",
                CodigoTipoCliente = 2,
                banca = "Banca Empresa",
                RatingEmpresa = 4.5m,
                IdCiiu = "6201",
                CodigoTienda = "T05",
                NombreTienda = "Tienda Central",
                ClasificacionSbs = "Normal",
                ClasificacionFeve = "A",
                CodigoGrupo = "G10",
                NombreGrupo = "Grupo Económico 1",
                IdDireccion = "DIR-001",
                IdDistrito = "150101",
                IdProvincia = "1501",
                IdDepartamento = "15"
            };

            // Assert
            Assert.Equal("CU123", model.CodigoUnico);
            Assert.Equal(1001, model.CodigoCliente);
            Assert.Equal(1, model.Codigotipodocumento);
            Assert.Equal("20123456789", model.NumeroDocumento);
            Assert.Equal("Corporativo", model.Segmento);
            Assert.Equal("Juan Perez", model.NombreEjecutivo);
            Assert.Equal(4.5m, model.RatingEmpresa);
            Assert.Equal("150101", model.IdDistrito);
        }

        [Fact]
        public void ClienteProveedorAfiliacion_Defaults_ShouldBeNullOrZero()
        {
            // Arrange & Act
            var model = new ClienteProveedorAfiliacion();

            // Assert
            Assert.Null(model.CodigoUnico);
            Assert.Null(model.RazonSocial);
            Assert.Equal(0, model.CodigoCliente);
            Assert.Equal(0m, model.RatingEmpresa);
            Assert.Null(model.CodigoTipoCliente);
        }
    }
}
