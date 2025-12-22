using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalBankingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDetails_PaymentData_PaymentDataId",
                table: "BankDetails");

            migrationBuilder.DropIndex(
                name: "IX_BankDetails_PaymentDataId",
                table: "BankDetails");

            migrationBuilder.DropColumn(
                name: "PaymentDataId",
                table: "BankDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "BankDetailId",
                table: "PaymentData",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentData_BankDetailId",
                table: "PaymentData",
                column: "BankDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentData_BankDetails_BankDetailId",
                table: "PaymentData",
                column: "BankDetailId",
                principalTable: "BankDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentData_BankDetails_BankDetailId",
                table: "PaymentData");

            migrationBuilder.DropIndex(
                name: "IX_PaymentData_BankDetailId",
                table: "PaymentData");

            migrationBuilder.DropColumn(
                name: "BankDetailId",
                table: "PaymentData");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentDataId",
                table: "BankDetails",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_PaymentDataId",
                table: "BankDetails",
                column: "PaymentDataId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BankDetails_PaymentData_PaymentDataId",
                table: "BankDetails",
                column: "PaymentDataId",
                principalTable: "PaymentData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
