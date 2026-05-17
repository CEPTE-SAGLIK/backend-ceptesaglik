using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonIsAccountOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountOwner",
                table: "Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Mevcut veriler için backfill: her hesabın en eski (ilk oluşturulan)
            // Person'ı kayıt sahibi profili kabul edilir.
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY UserId
                               ORDER BY CreatedAt ASC, Id ASC
                           ) AS rn
                    FROM Persons
                )
                UPDATE Persons
                SET IsAccountOwner = 1
                WHERE Id IN (SELECT Id FROM ranked WHERE rn = 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountOwner",
                table: "Persons");
        }
    }
}
