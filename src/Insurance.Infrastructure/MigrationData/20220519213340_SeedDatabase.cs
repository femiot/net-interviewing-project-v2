using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Infrastructure.MigrationData
{
    public partial class SeedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Insert INTO CostRangeRules (Min, Max, Value, IgnoreMax, DateCreated, CreatedByUserId) VALUES (0, 500, 0, 0, '2022-05-19', 'System')");
            migrationBuilder.Sql("Insert INTO CostRangeRules (Min, Max, Value, IgnoreMax, DateCreated, CreatedByUserId) VALUES (500, 2000, 1000, 0, '2022-05-19', 'System')");
            migrationBuilder.Sql("Insert INTO CostRangeRules (Min, Max, Value, IgnoreMax, DateCreated, CreatedByUserId) VALUES (2000, 2000, 2000, 1, '2022-05-19', 'System')");

            migrationBuilder.Sql("Insert INTO InsuranceExtraCosts (ProductName, ExtraCost, ApplyCostRangeRule, DateCreated, CreatedByUserId) VALUES ('Laptops', 500, 1, '2022-05-19', 'System')");
            migrationBuilder.Sql("Insert INTO InsuranceExtraCosts (ProductName, ExtraCost, ApplyCostRangeRule, DateCreated, CreatedByUserId) VALUES ('Smartphones', 500, 1, '2022-05-19', 'System')");
            migrationBuilder.Sql("Insert INTO InsuranceExtraCosts (ProductName, ExtraCost, ApplyCostRangeRule, DateCreated, CreatedByUserId) VALUES ('Digital cameras', 500, 1, '2022-05-19', 'System')");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete CostRangeRules Where CreatedByUserId = 'System'");
            migrationBuilder.Sql("Delete InsuranceExtraCosts Where CreatedByUserId = 'System'");
        }
    }
}
