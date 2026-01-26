using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureGym.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IdentityRestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_members_AspNetUsers_user_id",
                table: "members");

            migrationBuilder.DropIndex(
                name: "IX_members_user_id",
                table: "members");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "members");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "members",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MemberId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_user_id",
                table: "members",
                column: "user_id",
                unique: true,
                filter: "user_id IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_members_AspNetUsers_user_id",
                table: "members",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
