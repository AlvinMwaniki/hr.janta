using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobListingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    JobRequisitionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExternalTitle = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExternalDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListings_JobRequisition_JobRequisitionId",
                        column: x => x.JobRequisitionId,
                        principalTable: "JobRequisition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionApprovals_ActionByUserId",
                table: "RequisitionApprovals",
                column: "ActionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_JobRequisitionId",
                table: "JobListings",
                column: "JobRequisitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequisitionApprovals_Users_ActionByUserId",
                table: "RequisitionApprovals",
                column: "ActionByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequisitionApprovals_Users_ActionByUserId",
                table: "RequisitionApprovals");

            migrationBuilder.DropTable(
                name: "JobListings");

            migrationBuilder.DropIndex(
                name: "IX_RequisitionApprovals_ActionByUserId",
                table: "RequisitionApprovals");
        }
    }
}
