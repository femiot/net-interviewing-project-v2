using Insurance.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.EF
{
    public class InsuranceContext : DbContext
    {
        public InsuranceContext(DbContextOptions<InsuranceContext> options)
        : base(options)
        {  }

        public DbSet<CostRangeRule> CostRangeRules { get; set; } = null!;
        public DbSet<InsuranceExtraCost> InsuranceExtraCosts { get; set; } = null!;
        public DbSet<ProductTypeSurchargeCost> ProductTypeSurchargeCosts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CostRangeRule>().ToTable("CostRangeRules");
            modelBuilder.Entity<InsuranceExtraCost>().ToTable("InsuranceExtraCosts");
            modelBuilder.Entity<ProductTypeSurchargeCost>().ToTable("ProductTypeSurchargeCosts");
        }
    }
}
