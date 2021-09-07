using Microsoft.EntityFrameworkCore.Migrations;

namespace RhemaCMS.Migrations.ChurchModel
{
    public partial class ef_c_upd3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChurchAttendAttendee_ChurchCalendarEvent_ChurchEventId",
                table: "ChurchAttendAttendee");

            migrationBuilder.DropIndex(
                name: "IX_ChurchAttendAttendee_ChurchEventId",
                table: "ChurchAttendAttendee");

            migrationBuilder.DropColumn(
                name: "ChurchEventId",
                table: "ChurchAttendAttendee");

            migrationBuilder.AddColumn<int>(
                name: "ChurchEventDetailId",
                table: "ChurchAttendAttendee",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChurchAttendAttendee_ChurchEventDetailId",
                table: "ChurchAttendAttendee",
                column: "ChurchEventDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChurchAttendAttendee_ChurchCalendarEventDetail_ChurchEventDetailId",
                table: "ChurchAttendAttendee",
                column: "ChurchEventDetailId",
                principalTable: "ChurchCalendarEventDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChurchAttendAttendee_ChurchCalendarEventDetail_ChurchEventDetailId",
                table: "ChurchAttendAttendee");

            migrationBuilder.DropIndex(
                name: "IX_ChurchAttendAttendee_ChurchEventDetailId",
                table: "ChurchAttendAttendee");

            migrationBuilder.DropColumn(
                name: "ChurchEventDetailId",
                table: "ChurchAttendAttendee");

            migrationBuilder.AddColumn<int>(
                name: "ChurchEventId",
                table: "ChurchAttendAttendee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChurchAttendAttendee_ChurchEventId",
                table: "ChurchAttendAttendee",
                column: "ChurchEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChurchAttendAttendee_ChurchCalendarEvent_ChurchEventId",
                table: "ChurchAttendAttendee",
                column: "ChurchEventId",
                principalTable: "ChurchCalendarEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
