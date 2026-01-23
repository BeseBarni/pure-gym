using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureGym.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "membership_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    price_per_month = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    duration_in_days = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "memberships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memberships", x => x.id);
                    table.ForeignKey(
                        name: "FK_memberships_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_memberships_membership_types_membership_type_id",
                        column: x => x.membership_type_id,
                        principalTable: "membership_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gym_access_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: true),
                    accessed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gym_access_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_gym_access_logs_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gym_access_logs_memberships_membership_id",
                        column: x => x.membership_id,
                        principalTable: "memberships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_accessed_at_utc",
                table: "gym_access_logs",
                column: "accessed_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_member_id",
                table: "gym_access_logs",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_member_id_accessed_at_utc",
                table: "gym_access_logs",
                columns: new[] { "member_id", "accessed_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_membership_id",
                table: "gym_access_logs",
                column: "membership_id");

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_membership_id_accessed_at_utc",
                table: "gym_access_logs",
                columns: new[] { "membership_id", "accessed_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_gym_access_logs_result",
                table: "gym_access_logs",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "IX_members_email",
                table: "members",
                column: "email",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "IX_members_is_deleted",
                table: "members",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_members_user_id",
                table: "members",
                column: "user_id",
                filter: "user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_membership_types_is_active",
                table: "membership_types",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_membership_types_is_deleted",
                table: "membership_types",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_membership_types_name",
                table: "membership_types",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_end_date_utc",
                table: "memberships",
                column: "end_date_utc");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_is_deleted",
                table: "memberships",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_member_id",
                table: "memberships",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_membership_type_id",
                table: "memberships",
                column: "membership_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_status",
                table: "memberships",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gym_access_logs");

            migrationBuilder.DropTable(
                name: "memberships");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "membership_types");
        }
    }
}
