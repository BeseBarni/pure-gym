using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureGym.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpMembershipAndOrders : Migration
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
                name: "member_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MembershipTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_member_orders_members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_member_orders_membership_types_MembershipTypeId",
                        column: x => x.MembershipTypeId,
                        principalTable: "membership_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_member_orders_memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "memberships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_MemberId",
                table: "member_orders",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_MembershipId",
                table: "member_orders",
                column: "MembershipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_MembershipTypeId",
                table: "member_orders",
                column: "MembershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_OrderedAtUtc",
                table: "member_orders",
                column: "OrderedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_orders");

            migrationBuilder.DropColumn(
                name: "PendingSinceUtc",
                table: "memberships");
        }
    }
}
