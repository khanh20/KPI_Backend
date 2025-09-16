using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "KpiViolation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "KpiViolation");
        }
    }
}
