using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEthnicityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ethnicity",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "EthnicityId",
                table: "Employees",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Ethnicities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ethnicities", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EthnicityId",
                table: "Employees",
                column: "EthnicityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Ethnicities_EthnicityId",
                table: "Employees",
                column: "EthnicityId",
                principalTable: "Ethnicities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Ethnicities_EthnicityId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Ethnicities");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EthnicityId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EthnicityId",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "Ethnicity",
                table: "Employees",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
