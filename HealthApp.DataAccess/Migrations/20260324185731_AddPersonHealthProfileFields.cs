using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonHealthProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ChronicDiseases",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ChronicDiseases",
                table: "Persons");
        }
    }
}
