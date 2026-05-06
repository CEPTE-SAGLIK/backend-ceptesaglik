using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonEntityForProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Persons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "Persons",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Persons",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_UserId",
                table: "Persons",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Users_UserId",
                table: "Persons",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Users_UserId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_UserId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Persons");
        }
    }
}
