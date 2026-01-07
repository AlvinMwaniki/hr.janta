using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Employees",
                type: "longblob",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Employees");
        }
    }
}
