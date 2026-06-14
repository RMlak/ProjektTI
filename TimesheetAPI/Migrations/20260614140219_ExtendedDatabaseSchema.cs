using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimesheetAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoursWorked",
                table: "TimesheetEntries",
                newName: "ProjectTaskId");

            migrationBuilder.AddColumn<double>(
                name: "Hours",
                table: "TimesheetEntries",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TimesheetEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetEntries_ProjectTaskId",
                table: "TimesheetEntries",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_ProjectTasks_ProjectTaskId",
                table: "TimesheetEntries",
                column: "ProjectTaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_ProjectTasks_ProjectTaskId",
                table: "TimesheetEntries");

            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_TimesheetEntries_ProjectTaskId",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TimesheetEntries");

            migrationBuilder.RenameColumn(
                name: "ProjectTaskId",
                table: "TimesheetEntries",
                newName: "HoursWorked");
        }
    }
}
