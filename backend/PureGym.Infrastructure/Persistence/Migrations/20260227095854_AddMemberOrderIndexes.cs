using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureGym.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberOrderIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_member_orders_MemberId",
                table: "member_orders");

            migrationBuilder.DropIndex(
                name: "IX_member_orders_MembershipId",
                table: "member_orders");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_member_orders_MemberId",
                table: "member_orders");

            migrationBuilder.DropIndex(
                name: "IX_member_orders_MembershipId",
                table: "member_orders");

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_MemberId",
                table: "member_orders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_member_orders_MembershipId",
                table: "member_orders",
                column: "MembershipId");
        }
    }
}
