using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpsPilot.Infrastructure.Persistence.Migrations;

public partial class CreateMaintenanceRecordsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MaintenanceRecords",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                ScheduledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CompletionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_MaintenanceRecords_Assets_AssetId",
                    column: x => x.AssetId,
                    principalTable: "Assets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_MaintenanceRecords_AssetId",
            table: "MaintenanceRecords",
            column: "AssetId");

        migrationBuilder.CreateIndex(
            name: "IX_MaintenanceRecords_ScheduledAtUtc",
            table: "MaintenanceRecords",
            column: "ScheduledAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_MaintenanceRecords_Status",
            table: "MaintenanceRecords",
            column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "MaintenanceRecords");
    }
}
