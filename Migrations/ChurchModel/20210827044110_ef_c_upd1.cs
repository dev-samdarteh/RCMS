using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RhemaCMS.Migrations.ChurchModel
{
    public partial class ef_c_upd1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventStatus",
                table: "ChurchCalendarEvent",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChurchMemTypeId",
                table: "ChurchAttendAttendee",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPersFirstimer",
                table: "ChurchAttendAttendee",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChurchCalendarEventDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppGlobalOwnerId = table.Column<int>(nullable: true),
                    ChurchBodyId = table.Column<int>(nullable: true),
                    EventDescription = table.Column<string>(maxLength: 100, nullable: true),
                    EventSessionCode = table.Column<string>(maxLength: 1, nullable: true),
                    CustomSessionDesc = table.Column<string>(maxLength: 50, nullable: true),
                    ChurchCalendarEventId = table.Column<int>(nullable: true),
                    IsChurchServiceEvent = table.Column<bool>(nullable: false),
                    ChurchBodyServiceId = table.Column<int>(nullable: true),
                    ChurchlifeActivityId = table.Column<int>(nullable: true),
                    Venue = table.Column<string>(maxLength: 100, nullable: true),
                    IsFullDay = table.Column<bool>(nullable: false),
                    EventFrom = table.Column<DateTime>(nullable: true),
                    EventTo = table.Column<DateTime>(nullable: true),
                    IsEventActive = table.Column<bool>(nullable: false),
                    IsCurrentEvent = table.Column<bool>(nullable: false),
                    EventStatus = table.Column<string>(maxLength: 1, nullable: true),
                    SharingStatus = table.Column<string>(maxLength: 1, nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(maxLength: 300, nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    LastMod = table.Column<DateTime>(nullable: true),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    LastModByUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChurchCalendarEventDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChurchCalendarEventDetail_AppGlobalOwner_AppGlobalOwnerId",
                        column: x => x.AppGlobalOwnerId,
                        principalTable: "AppGlobalOwner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChurchCalendarEventDetail_ChurchBody_ChurchBodyId",
                        column: x => x.ChurchBodyId,
                        principalTable: "ChurchBody",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChurchCalendarEventDetail_ChurchBodyService_ChurchBodyServiceId",
                        column: x => x.ChurchBodyServiceId,
                        principalTable: "ChurchBodyService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChurchCalendarEventDetail_ChurchCalendarEvent_ChurchCalendarEventId",
                        column: x => x.ChurchCalendarEventId,
                        principalTable: "ChurchCalendarEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChurchCalendarEventDetail_AppUtilityNVP_ChurchlifeActivityId",
                        column: x => x.ChurchlifeActivityId,
                        principalTable: "AppUtilityNVP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChurchCalendarEventDetail_AppGlobalOwnerId",
                table: "ChurchCalendarEventDetail",
                column: "AppGlobalOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchCalendarEventDetail_ChurchBodyId",
                table: "ChurchCalendarEventDetail",
                column: "ChurchBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchCalendarEventDetail_ChurchBodyServiceId",
                table: "ChurchCalendarEventDetail",
                column: "ChurchBodyServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchCalendarEventDetail_ChurchCalendarEventId",
                table: "ChurchCalendarEventDetail",
                column: "ChurchCalendarEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchCalendarEventDetail_ChurchlifeActivityId",
                table: "ChurchCalendarEventDetail",
                column: "ChurchlifeActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChurchCalendarEventDetail");

            migrationBuilder.DropColumn(
                name: "EventStatus",
                table: "ChurchCalendarEvent");

            migrationBuilder.DropColumn(
                name: "ChurchMemTypeId",
                table: "ChurchAttendAttendee");

            migrationBuilder.DropColumn(
                name: "isPersFirstimer",
                table: "ChurchAttendAttendee");
        }
    }
}
