using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureGym.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingToMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PendingSinceUtc",
                table: "memberships",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberOrders_members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberOrders_memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "memberships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberOrders_MemberId",
                table: "MemberOrders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberOrders_MembershipId",
                table: "MemberOrders",
                column: "MembershipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberOrders");

            migrationBuilder.DropColumn(
                name: "PendingSinceUtc",
                table: "memberships");
        }
    }
}
