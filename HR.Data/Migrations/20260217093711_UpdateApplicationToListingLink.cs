using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationToListingLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobRequisition_JobRequisitionId",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "JobRequisitionId",
                table: "JobApplications",
                newName: "JobListingId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_JobRequisitionId",
                table: "JobApplications",
                newName: "IX_JobApplications_JobListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobListings_JobListingId",
                table: "JobApplications",
                column: "JobListingId",
                principalTable: "JobListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobListings_JobListingId",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "JobListingId",
                table: "JobApplications",
                newName: "JobRequisitionId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_JobListingId",
                table: "JobApplications",
                newName: "IX_JobApplications_JobRequisitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobRequisition_JobRequisitionId",
                table: "JobApplications",
                column: "JobRequisitionId",
                principalTable: "JobRequisition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
