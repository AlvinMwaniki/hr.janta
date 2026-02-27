using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAtsFieldsToRequisitionAndListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredEducationLevel",
                table: "JobRequisition",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RequiredExperienceYears",
                table: "JobRequisition",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredSkills",
                table: "JobRequisition",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RequiredEducationLevel",
                table: "JobListings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RequiredExperienceYears",
                table: "JobListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredSkills",
                table: "JobListings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredEducationLevel",
                table: "JobRequisition");

            migrationBuilder.DropColumn(
                name: "RequiredExperienceYears",
                table: "JobRequisition");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "JobRequisition");

            migrationBuilder.DropColumn(
                name: "RequiredEducationLevel",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "RequiredExperienceYears",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "JobListings");
        }
    }
}
