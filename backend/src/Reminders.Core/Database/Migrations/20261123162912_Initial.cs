using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminders.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invite_token_hash = table.Column<string>(type: "text", nullable: true),
                    invite_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.CheckConstraint("ck_teams_invite_fields_together", "(invite_token_hash IS NULL) = (invite_expires_at IS NULL)");
                });

            migrationBuilder.CreateTable(
                name: "telegram_bot_state",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    updates_offset = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bot_state", x => x.id);
                    table.CheckConstraint("ck_telegram_bot_state_singleton", "id = 1");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deactivated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.CheckConstraint("ck_users_email_normalized", "email = lower(btrim(email))");
                });

            migrationBuilder.CreateTable(
                name: "channel_bindings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    code_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_bindings", x => x.id);
                    table.CheckConstraint("ck_channel_bindings_expires_at", "expires_at > created_at");
                    table.CheckConstraint("ck_channel_bindings_used_at", "used_at IS NULL OR (used_at >= created_at AND used_at < expires_at)");
                    table.ForeignKey(
                        name: "fk_channel_bindings_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "channel_targets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    broken_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_targets", x => x.id);
                    table.UniqueConstraint("ak_channel_targets_team_id_id", x => new { x.team_id, x.id });
                    table.ForeignKey(
                        name: "fk_channel_targets_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reminders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    start_local = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    time_zone_id = table.Column<string>(type: "text", nullable: false),
                    repeat_unit = table.Column<int>(type: "integer", nullable: true),
                    repeat_every = table.Column<int>(type: "integer", nullable: true),
                    next_occurrence_number = table.Column<int>(type: "integer", nullable: false),
                    next_fire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reminders", x => x.id);
                    table.CheckConstraint("ck_reminders_repeat_settings", "(repeat_unit IS NULL AND repeat_every IS NULL)\r\nOR\r\n(repeat_unit IS NOT NULL AND repeat_every IS NOT NULL AND repeat_every > 0)");
                    table.ForeignKey(
                        name: "fk_reminders_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reminders_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_members", x => new { x.team_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_team_members_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "deliveries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reminder_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    next_attempt_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lease_token = table.Column<Guid>(type: "uuid", nullable: true),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deliveries", x => x.id);
                    table.CheckConstraint("ck_deliveries_attempts_non_negative", "attempts >= 0");
                    table.CheckConstraint("ck_deliveries_lease", "(status = 1 AND locked_until IS NOT NULL AND lease_token IS NOT NULL)\r\nOR\r\n(status <> 1 AND locked_until IS NULL AND lease_token IS NULL)");
                    table.ForeignKey(
                        name: "fk_deliveries_channel_targets_team_id_target_id",
                        columns: x => new { x.team_id, x.target_id },
                        principalTable: "channel_targets",
                        principalColumns: new[] { "team_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_deliveries_reminders_reminder_id",
                        column: x => x.reminder_id,
                        principalTable: "reminders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_channel_bindings_code_hash",
                table: "channel_bindings",
                column: "code_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_channel_bindings_team_id",
                table: "channel_bindings",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_channel_targets_type_address",
                table: "channel_targets",
                columns: new[] { "type", "address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_next_attempt_at",
                table: "deliveries",
                column: "next_attempt_at");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_reminder_id",
                table: "deliveries",
                column: "reminder_id");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_team_id_target_id",
                table: "deliveries",
                columns: new[] { "team_id", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reminders_author_id",
                table: "reminders",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_reminders_next_fire_at",
                table: "reminders",
                column: "next_fire_at",
                filter: "next_fire_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_reminders_team_id",
                table: "reminders",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_user_id",
                table: "team_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channel_bindings");

            migrationBuilder.DropTable(
                name: "deliveries");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "telegram_bot_state");

            migrationBuilder.DropTable(
                name: "channel_targets");

            migrationBuilder.DropTable(
                name: "reminders");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
