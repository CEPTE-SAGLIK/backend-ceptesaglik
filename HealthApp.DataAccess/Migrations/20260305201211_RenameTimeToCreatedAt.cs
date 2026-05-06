using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameTimeToCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Persons",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Children",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Persons",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Children",
                newName: "Time");
        }
    }
}
