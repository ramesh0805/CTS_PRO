using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MQTTR1.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeTelemetryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Telemetries_Timestamp",
                table: "Telemetries",
                newName: "IX_Telemetry_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Telemetries_DeviceId_Metric_Timestamp",
                table: "Telemetries",
                newName: "IX_Telemetry_DeviceId_Metric_Timestamp");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "Telemetries",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Metric",
                table: "Telemetries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Telemetry_DeviceId",
                table: "Telemetries",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Telemetry_DeviceId",
                table: "Telemetries");

            migrationBuilder.RenameIndex(
                name: "IX_Telemetry_Timestamp",
                table: "Telemetries",
                newName: "IX_Telemetries_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Telemetry_DeviceId_Metric_Timestamp",
                table: "Telemetries",
                newName: "IX_Telemetries_DeviceId_Metric_Timestamp");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "Telemetries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(18)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Metric",
                table: "Telemetries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
