using Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList;

namespace Interbank.Productos.Comercial.Fcd.Application.UnitTests.Features.Clientes.Queries.GetSuppliersList
{
    public class ClienteAfiliacionVMTest
    {
        [Fact]
        public void TodasPropiedades_SeAsignanYRecuperanCorrectamente()
        {
            // Arrange
            var fechaAhora = DateTime.Now;
            var fechaDesafiliacion = fechaAhora.AddDays(30);

            var cliente = new ClienteAfiliacionVM
            {
                docDuplicado = 1,
                tipoMaxLote = 2,
                tipoMaxProv = 3,
                montoMaxLote = 1000.50m,
                montoMaxProv = 500.25m,
                nombreContacto1 = "Juan Perez",
                emailContacto1 = "juan@example.com",
                cargoContacto1 = "Gerente",
                telefono1 = "999111222",
                telefono2 = "988333444",
                nombreContacto2 = "Ana Gomez",
                emailContacto2 = "ana@example.com",
                nombreContacto3 = "Luis Torres",
                emailContacto3 = "luis@example.com",
                tipoComision = 1,
                montoComision = 150.75m,
                ampliacionPago = 7,
                codigoUnicoAceptante = "UNICO001",
                razonSocialAceptante = "Empresa S.A.",
                numeroLineaAceptante = "LINEA001",
                montoMinSolesAceptante = 100m,
                montoMinDolarAceptante = 50m,
                codigoProducto = 31,
                codigoEstadoAfiliacion = 1,
                tipoAfiliacion = 2,
                fechaRegistro = fechaAhora,
                codigoUsuarioRegistro = "USR001",
                estadoProveedor = 1,
                codigoUnico = "UNICO123",
                razonSocial = "Mi Empresa",
                numeroLinea = "LINEA123",
                codigoTipoDocumento = 6,
                numeroDocumento = "12345678",
                documentoAuxiliarCliente = "AUX123",
                validaCuentaSoles = 1,
                validaCuentaDolares = 1,
                desembolsoAutoSoles = 0,
                desembolsoAutoDolar = 1,
                fechaIngreso = fechaAhora,
                fechaAfiliacion = fechaAhora,
                fechaDesafiliacion = fechaDesafiliacion,
                tipoMonedaSoles = 1,
                tipoMonedaDolar = 2,
                numeroCuentaSoles = "S123456789",
                numeroCuentaDolar = "D987654321",
                tasaSoles = 0.05m,
                tasaDolar = 0.07m,
                portes = 10,
                numeroLineaCliente = "CL001",
                numeroLineaProveedor = "PR001",
                codigoCliente = 1001,
                codigoProveedor = 2001,
                codigoAfiliacion = 3001,
                fechaUltimaActualizacion = fechaAhora,
                tasaClienteSoles = 0.06m,
                tasaClienteDolar = 0.08m
            };

            // Act & Assert: revisamos algunas propiedades como ejemplo
            Assert.Equal(1, cliente.docDuplicado);
            Assert.Equal("Juan Perez", cliente.nombreContacto1);
            Assert.Equal("UNICO001", cliente.codigoUnicoAceptante);
            Assert.Equal(fechaDesafiliacion, cliente.fechaDesafiliacion);
            Assert.Equal(0.05m, cliente.tasaSoles);
            Assert.Equal("S123456789", cliente.numeroCuentaSoles);
            Assert.Equal(2001, cliente.codigoProveedor);
            Assert.Equal(fechaAhora, cliente.fechaUltimaActualizacion);
        }
    }


}
