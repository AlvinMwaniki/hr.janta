using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CountyId",
                table: "Employees",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "County",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_County", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CountyId",
                table: "Employees",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_County_CountyId",
                table: "Employees",
                column: "CountyId",
                principalTable: "County",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_County_CountyId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "County");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CountyId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "Employees");
        }
    }
}
