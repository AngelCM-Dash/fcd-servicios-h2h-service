using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.UnitTests.Repositories
{
    public class RepositoryBaseTest
    {
        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion
            {
                CodigoAfiliacion = 1,
                RazonSocial = "Empresa Test",
                MontoMaxLote = 5000
            };

            // Act
            await repo.AddAsync(entity);
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("Empresa Test", all[0].RazonSocial);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Entity()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion { CodigoAfiliacion = 1, RazonSocial = "Empresa A" };
            await repo.AddAsync(entity);

            // Act
            entity.RazonSocial = "Empresa B";
            await repo.UpdateAsync(entity);
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("Empresa B", all[0].RazonSocial);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion { CodigoAfiliacion = 1, RazonSocial = "Empresa Test" };
            await repo.AddAsync(entity);

            // Act
            await repo.DeleteAsync(entity);
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Empty(all);
        }

        [Fact]
        public async Task GetAsync_Should_Filter_Entities()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            await repo.AddAsync(new EFClienteAfiliacion { CodigoAfiliacion = 1, RazonSocial = "A" });
            await repo.AddAsync(new EFClienteAfiliacion { CodigoAfiliacion = 2, RazonSocial = "B" });

            // Act
            var filtered = await repo.GetAsync(x => x.RazonSocial == "B");

            // Assert
            Assert.Single(filtered);
            Assert.Equal(2, filtered[0].CodigoAfiliacion);
        }

        [Fact]
        public async Task AddEntity_Should_Add_Entity_But_Not_Save()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion
            {
                CodigoAfiliacion = 1,
                RazonSocial = "Empresa Test"
            };

            // Act
            repo.AddEntity(entity); // agrega al contexto pero no guarda
            await context.SaveChangesAsync(); // necesario
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("Empresa Test", all[0].RazonSocial);
        }

        [Fact]
        public async Task UpdateEntity_Should_Modify_Entity_But_Not_Save()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion
            {
                CodigoAfiliacion = 1,
                RazonSocial = "Empresa A"
            };

            await repo.AddAsync(entity); // guardamos inicialmente

            // Act
            entity.RazonSocial = "Empresa B";
            repo.UpdateEntity(entity); // marca como modificado
            await context.SaveChangesAsync(); // necesario para aplicar cambios
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("Empresa B", all[0].RazonSocial);
        }

        [Fact]
        public async Task DeleteEntity_Should_Remove_Entity_But_Not_Save()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new RepositoryBase<EFClienteAfiliacion>(context);

            var entity = new EFClienteAfiliacion
            {
                CodigoAfiliacion = 1,
                RazonSocial = "Empresa Test"
            };

            await repo.AddAsync(entity); // guardamos inicialmente

            // Act
            repo.DeleteEntity(entity); // elimina del contexto
            await context.SaveChangesAsync(); // necesario para persistir
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Empty(all);
        }

        public static class TestDbContextFactory
        {
            public static FcdDbContext Create()
            {
                var options = new DbContextOptionsBuilder<FcdDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Cada test tiene DB aislada
                    .Options;

                var context = new FcdDbContext(options);
                context.Database.EnsureCreated();
                return context;
            }
        }
    }
}
