using Insurance.Infrastructure.EF;
using Insurance.Shared.Entities;
using System;

namespace Insurance.Tests.Helpers
{
    internal class TestDataInitializer
    {
        private readonly InsuranceContext _context;
        public TestDataInitializer(InsuranceContext context)
        {
            _context = context;

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public void SeedDatabase()
        {
            _context.CostRangeRules.Add(new CostRangeRule
            {
                Min = 0,
                Max = 500,
                Value = 0,
                IgnoreMax = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.CostRangeRules.Add(new CostRangeRule
            {
                Min = 500,
                Max = 2000,
                Value = 1000,
                IgnoreMax = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.CostRangeRules.Add(new CostRangeRule
            {
                Min = 0,
                Max = 500,
                Value = 2000,
                IgnoreMax = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.InsuranceExtraCosts.Add(new InsuranceExtraCost
            {
                ProductName = "Laptops",
                ExtraCost = 500,
                ApplyCostRangeRule = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.InsuranceExtraCosts.Add(new InsuranceExtraCost
            {
                ProductName = "Smartphones",
                ExtraCost = 500,
                ApplyCostRangeRule = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.InsuranceExtraCosts.Add(new InsuranceExtraCost
            {
                ProductName = "Digital cameras",
                ExtraCost = 500,
                ApplyCostRangeRule = false,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });


            _context.ProductTypeSurchargeCosts.Add(new ProductTypeSurchargeCost
            {
                ProductTypeId = 124,
                Rate = 1000,
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CreatedByUserId = "System"
            });

            _context.SaveChanges();
        }
    }
}
