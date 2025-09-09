using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemAndAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetValue",
                table: "KpiAssignment");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "KpiTemplate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "KpiTemplate",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "KpiTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "KpiTemplate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "KpiTemplate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "KpiTemplate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "KpiTemplate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "KpiTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TargetValue",
                table: "KpiItem",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "KpiTemplate");

            migrationBuilder.DropColumn(
                name: "TargetValue",
                table: "KpiItem");

            migrationBuilder.AddColumn<float>(
                name: "TargetValue",
                table: "KpiAssignment",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
