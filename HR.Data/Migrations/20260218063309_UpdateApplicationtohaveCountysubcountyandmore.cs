using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationtohaveCountysubcountyandmore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "JobApplications");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "JobApplications",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CountyId",
                table: "JobApplications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Estate",
                table: "JobApplications",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "POBox",
                table: "JobApplications",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "SubCountyId",
                table: "JobApplications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CountryId",
                table: "JobApplications",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CountyId",
                table: "JobApplications",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_SubCountyId",
                table: "JobApplications",
                column: "SubCountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Counties_CountyId",
                table: "JobApplications",
                column: "CountyId",
                principalTable: "Counties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Countries_CountryId",
                table: "JobApplications",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_SubCounties_SubCountyId",
                table: "JobApplications",
                column: "SubCountyId",
                principalTable: "SubCounties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Counties_CountyId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Countries_CountryId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_SubCounties_SubCountyId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_CountryId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_CountyId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_SubCountyId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "Estate",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "POBox",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "SubCountyId",
                table: "JobApplications");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "JobApplications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
