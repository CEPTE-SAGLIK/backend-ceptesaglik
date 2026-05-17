using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAudienceBirthDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AudienceBirthDate",
                table: "Reminders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AudienceBirthDate",
                table: "Medicines",
                type: "datetime2",
                nullable: true);

            // Eski kayıtlar için en iyi tahmin: hesabın ilk Person'ı (kayıt sahibi).
            // Böylece geçmiş kayıtlar da yaşa göre dinamik gruplanabilir.
            migrationBuilder.Sql(@"
                UPDATE Reminders
                SET AudienceBirthDate = (
                    SELECT TOP 1 p.BirthDate FROM Persons p
                    WHERE p.UserId = Reminders.UserId
                    ORDER BY p.CreatedAt
                )
                WHERE AudienceBirthDate IS NULL;");

            migrationBuilder.Sql(@"
                UPDATE Medicines
                SET AudienceBirthDate = (
                    SELECT TOP 1 p.BirthDate FROM Persons p
                    WHERE p.UserId = Medicines.UserId
                    ORDER BY p.CreatedAt
                )
                WHERE AudienceBirthDate IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceBirthDate",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "AudienceBirthDate",
                table: "Medicines");
        }
    }
}
