using Microsoft.EntityFrameworkCore.Migrations;

namespace RhemaCMS.Migrations
{
    public partial class ef_m_upd6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileRole_UserProfile_UserProfileId",
                table: "UserProfileRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileRole_UserRole_UserRoleId",
                table: "UserProfileRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRolePermission_UserPermission_UserPermissionId",
                table: "UserRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRolePermission_UserRole_UserRoleId",
                table: "UserRolePermission");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "UserRolePermission",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserPermissionId",
                table: "UserRolePermission",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "UserProfileRole",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "UserProfileRole",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileRole_UserProfile_UserProfileId",
                table: "UserProfileRole",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileRole_UserRole_UserRoleId",
                table: "UserProfileRole",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRolePermission_UserPermission_UserPermissionId",
                table: "UserRolePermission",
                column: "UserPermissionId",
                principalTable: "UserPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRolePermission_UserRole_UserRoleId",
                table: "UserRolePermission",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileRole_UserProfile_UserProfileId",
                table: "UserProfileRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileRole_UserRole_UserRoleId",
                table: "UserProfileRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRolePermission_UserPermission_UserPermissionId",
                table: "UserRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRolePermission_UserRole_UserRoleId",
                table: "UserRolePermission");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "UserRolePermission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserPermissionId",
                table: "UserRolePermission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "UserProfileRole",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserProfileId",
                table: "UserProfileRole",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileRole_UserProfile_UserProfileId",
                table: "UserProfileRole",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileRole_UserRole_UserRoleId",
                table: "UserProfileRole",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRolePermission_UserPermission_UserPermissionId",
                table: "UserRolePermission",
                column: "UserPermissionId",
                principalTable: "UserPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRolePermission_UserRole_UserRoleId",
                table: "UserRolePermission",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
