using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo bảng ViolationCategory
            migrationBuilder.CreateTable(
                name: "ViolationCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CalculationFormula = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationCategory", x => x.Id);
                });

            // Tạo bảng KpiViolationLevel
            migrationBuilder.CreateTable(
                name: "KpiViolationLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    MaxDeduction = table.Column<float>(type: "real", nullable: false),
                    ViolationCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiViolationLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KpiViolationLevel_ViolationCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ViolationCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Tạo index cho KpiViolationLevel
            migrationBuilder.CreateIndex(
                name: "IX_KpiViolationLevel_CategoryId",
                table: "KpiViolationLevel",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xoá bảng con trước
            migrationBuilder.DropTable(
                name: "KpiViolationLevel");

            // Xoá bảng cha
            migrationBuilder.DropTable(
                name: "ViolationCategory");
        }
    }
}
