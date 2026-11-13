using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpsPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentSlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ResponseTargetMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionTargetMinutes = table.Column<int>(type: "int", nullable: false),
                    ResponseDueAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResolutionDueAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FirstResponseAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResolvedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResponseBreachedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResolutionBreachedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentSlas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentSlas_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ResponseTargetMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionTargetMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentSlas_IncidentId",
                table: "IncidentSlas",
                column: "IncidentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentSlas_ResolutionDueAtUtc",
                table: "IncidentSlas",
                column: "ResolutionDueAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentSlas_ResponseDueAtUtc",
                table: "IncidentSlas",
                column: "ResponseDueAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicies_Priority",
                table: "SlaPolicies",
                column: "Priority",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentSlas");

            migrationBuilder.DropTable(
                name: "SlaPolicies");
        }
    }
}
