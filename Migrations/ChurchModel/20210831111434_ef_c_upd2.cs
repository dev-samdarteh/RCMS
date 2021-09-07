using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RhemaCMS.Migrations.ChurchModel
{
    public partial class ef_c_upd2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppGlobalOwnerId = table.Column<int>(nullable: true),
                    ChurchBodyId = table.Column<int>(nullable: true),
                    OwnedByChurchBodyId = table.Column<int>(nullable: true),
                    ChurchMemberId = table.Column<int>(nullable: true),
                    ChurchYear = table.Column<string>(maxLength: 4, nullable: true),
                    ChurchPeriodId = table.Column<int>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: true),
                    RegCode = table.Column<string>(maxLength: 10, nullable: true),
                    SharingStatus = table.Column<string>(maxLength: 1, nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    LastMod = table.Column<DateTime>(nullable: true),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    LastModByUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRegistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberRegistration_AppGlobalOwner_AppGlobalOwnerId",
                        column: x => x.AppGlobalOwnerId,
                        principalTable: "AppGlobalOwner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberRegistration_ChurchBody_ChurchBodyId",
                        column: x => x.ChurchBodyId,
                        principalTable: "ChurchBody",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberRegistration_ChurchMember_ChurchMemberId",
                        column: x => x.ChurchMemberId,
                        principalTable: "ChurchMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberRegistration_ChurchPeriod_ChurchPeriodId",
                        column: x => x.ChurchPeriodId,
                        principalTable: "ChurchPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberRegistration_ChurchBody_OwnedByChurchBodyId",
                        column: x => x.OwnedByChurchBodyId,
                        principalTable: "ChurchBody",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_AppGlobalOwnerId",
                table: "MemberRegistration",
                column: "AppGlobalOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_ChurchBodyId",
                table: "MemberRegistration",
                column: "ChurchBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_ChurchMemberId",
                table: "MemberRegistration",
                column: "ChurchMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_ChurchPeriodId",
                table: "MemberRegistration",
                column: "ChurchPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_OwnedByChurchBodyId",
                table: "MemberRegistration",
                column: "OwnedByChurchBodyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberRegistration");
        }
    }
}
