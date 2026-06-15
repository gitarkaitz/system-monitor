using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemMonitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Threshold = table.Column<double>(type: "REAL", nullable: false),
                    MetricType = table.Column<int>(type: "INTEGER", nullable: false),
                    Condition = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetricSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CpuUsagePercent = table.Column<double>(type: "REAL", nullable: false),
                    MemoryUsedBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    MemoryTotalBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    NetworkBytesReceived = table.Column<long>(type: "INTEGER", nullable: false),
                    NetworkBytesSent = table.Column<long>(type: "INTEGER", nullable: false),
                    DiskBytesRead = table.Column<long>(type: "INTEGER", nullable: false),
                    DiskBytesWritten = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CpuUsagePercent = table.Column<double>(type: "REAL", nullable: false),
                    MemoryBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlertId = table.Column<int>(type: "INTEGER", nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TriggeredValue = table.Column<double>(type: "REAL", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertEvents_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertEvents_AlertId",
                table: "AlertEvents",
                column: "AlertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertEvents");

            migrationBuilder.DropTable(
                name: "MetricSnapshots");

            migrationBuilder.DropTable(
                name: "ProcessSnapshots");

            migrationBuilder.DropTable(
                name: "Alerts");
        }
    }
}
