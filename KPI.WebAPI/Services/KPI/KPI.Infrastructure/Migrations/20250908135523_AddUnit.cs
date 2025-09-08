using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KpiAssignmentId",
                table: "ApprovalLogs",
                newName: "TargetId");

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "ApprovalLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "ApprovalLogs");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "ApprovalLogs",
                newName: "KpiAssignmentId");
        }
    }
}
