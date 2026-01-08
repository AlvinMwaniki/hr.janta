using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisabilityTableDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Users_UserId1",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_UserId1",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserPermissions");

            migrationBuilder.UpdateData(
                table: "PaymentData",
                keyColumn: "KRA_PIN",
                keyValue: null,
                column: "KRA_PIN",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "KRA_PIN",
                table: "PaymentData",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DisabilityDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DisabilityNature = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NCPWD_Number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KRA_ExemptionNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CertificateFileName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CertificateFilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabilityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisabilityDetails_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DisabilityDetails_EmployeeId",
                table: "DisabilityDetails",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisabilityDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserPermissions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "KRA_PIN",
                table: "PaymentData",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId1",
                table: "UserPermissions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Users_UserId1",
                table: "UserPermissions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
