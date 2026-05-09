using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpensesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Expenses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaidBy",
                table: "Expenses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaidBy",
                table: "Expenses");
        }
    }
}
