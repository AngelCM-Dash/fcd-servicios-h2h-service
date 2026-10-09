using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class ClienteAfiliacionRepositoryTest
    {
        private static FcdDbContext CrearContexto(string dbName)
        {
            var options = new DbContextOptionsBuilder<FcdDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new FcdDbContext(options);
        }

        private static void SeedData(FcdDbContext context)
        {
            context.Afiliaciones?.AddRange(
                new EFClienteAfiliacion
                {
                    CodigoUnicoAceptante = "CU1",
                    CodigoUnico = "CU1",
                    CodigoProducto = 1
                },
                new EFClienteAfiliacion
                {
                    CodigoUnicoAceptante = "CU1",
                    CodigoUnico = "CU1",
                    CodigoProducto = 2
                },
                new EFClienteAfiliacion
                {
                    CodigoUnicoAceptante = "CU2",
                    CodigoUnico = "CU2",
                    CodigoProducto = 1
                }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetSuppliersByAcceptor_WhenCodProductoHasValue_ReturnsFilteredByProducto()
        {
            var context = CrearContexto(nameof(GetSuppliersByAcceptor_WhenCodProductoHasValue_ReturnsFilteredByProducto));
            SeedData(context);

            var repo = new ClienteAfiliacionRepository(context);

            var result = await repo.GetSuppliersByAcceptor("CU1", 1);

            Assert.Single(result);
            Assert.All(result, r => Assert.Equal(1, r.CodigoProducto));
        }

        [Fact]
        public async Task GetSuppliersByAcceptor_WhenCodProductoIsNull_ReturnsAllForAcceptor()
        {
            var context = CrearContexto(nameof(GetSuppliersByAcceptor_WhenCodProductoIsNull_ReturnsAllForAcceptor));
            SeedData(context);

            var repo = new ClienteAfiliacionRepository(context);

            var result = await repo.GetSuppliersByAcceptor("CU1", null);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSuppliersByAcceptor_WhenCodProductoIs99_FiltersByCodigoUnico()
        {
            var context = CrearContexto(nameof(GetSuppliersByAcceptor_WhenCodProductoIs99_FiltersByCodigoUnico));
            SeedData(context);

            var repo = new ClienteAfiliacionRepository(context);

            var result = await repo.GetSuppliersByAcceptor("CU1", 99);

            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal("CU1", r.CodigoUnico));
        }
    }

}






