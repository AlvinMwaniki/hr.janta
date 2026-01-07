using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationDetailsToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_County_CountyId",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_County",
                table: "County");

            migrationBuilder.RenameTable(
                name: "County",
                newName: "Counties");

            migrationBuilder.AlterColumn<string>(
                name: "NationalID",
                table: "Employees",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Estate",
                table: "Employees",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "POBox",
                table: "Employees",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SubCounty",
                table: "Employees",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Counties",
                table: "Counties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Counties_CountyId",
                table: "Employees",
                column: "CountyId",
                principalTable: "Counties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Counties_CountyId",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Counties",
                table: "Counties");

            migrationBuilder.DropColumn(
                name: "Estate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "POBox",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SubCounty",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Counties",
                newName: "County");

            migrationBuilder.AlterColumn<string>(
                name: "NationalID",
                table: "Employees",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_County",
                table: "County",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_County_CountyId",
                table: "Employees",
                column: "CountyId",
                principalTable: "County",
                principalColumn: "Id");
        }
    }
}
