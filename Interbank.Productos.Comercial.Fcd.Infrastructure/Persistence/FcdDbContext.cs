using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Persistence
{
    public class FcdDbContext : DbContext
    {
        public FcdDbContext(DbContextOptions<FcdDbContext> options) : base(options)
        {
        }
        public DbSet<EFClienteAfiliacion>? Afiliaciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EFClienteAfiliacionConfiguration());
        }
    }
}
