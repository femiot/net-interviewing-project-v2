using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Infrastructure.MigrationData
{
    public partial class InitialCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostRangeRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Min = table.Column<float>(type: "REAL", nullable: false),
                    Max = table.Column<float>(type: "REAL", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false),
                    IgnoreMax = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    DateModified = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdatedByUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostRangeRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceExtraCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    ExtraCost = table.Column<float>(type: "REAL", nullable: false),
                    ApplyCostRangeRule = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    DateModified = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdatedByUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceExtraCosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypeSurchargeCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rate = table.Column<float>(type: "REAL", nullable: false),
                    DateCreated = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    DateModified = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdatedByUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeSurchargeCosts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostRangeRules");

            migrationBuilder.DropTable(
                name: "InsuranceExtraCosts");

            migrationBuilder.DropTable(
                name: "ProductTypeSurchargeCosts");
        }
    }
}
