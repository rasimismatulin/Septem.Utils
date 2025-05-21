using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Septem.Notifications.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    target_uid = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    type = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_to_send_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<byte>(type: "smallint", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    data = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    default_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    group_key = table.Column<Guid>(type: "uuid", nullable: true),
                    cancellation_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fcm_configuration = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<byte>(type: "smallint", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    token_type = table.Column<byte>(type: "smallint", nullable: true),
                    notification_token_uid = table.Column<Guid>(type: "uuid", nullable: true),
                    notification_uid = table.Column<Guid>(type: "uuid", nullable: false),
                    service_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    exception_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_view = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_messages_notification_tokens_notification_toke",
                        column: x => x.notification_token_uid,
                        principalTable: "notification_tokens",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_notification_messages_notifications_notification_uid",
                        column: x => x.notification_uid,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_receivers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_uid = table.Column<Guid>(type: "uuid", nullable: true),
                    token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    receiver_type = table.Column<byte>(type: "smallint", nullable: false),
                    parameters = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    notification_uid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_receivers", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_receivers_notifications_notification_uid",
                        column: x => x.notification_uid,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notification_messages_notification_token_uid",
                table: "notification_messages",
                column: "notification_token_uid");

            migrationBuilder.CreateIndex(
                name: "ix_notification_messages_notification_uid",
                table: "notification_messages",
                column: "notification_uid");

            migrationBuilder.CreateIndex(
                name: "ix_notification_receivers_notification_uid",
                table: "notification_receivers",
                column: "notification_uid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_messages");

            migrationBuilder.DropTable(
                name: "notification_receivers");

            migrationBuilder.DropTable(
                name: "notification_tokens");

            migrationBuilder.DropTable(
                name: "notifications");
        }
    }
}
