using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Septem.Notifications.Core.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeToSendUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Data = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    GroupKey = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FcmConfiguration = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTokens",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TargetUid = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTokens", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "NotificationReceivers",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ReceiverType = table.Column<byte>(type: "smallint", nullable: false),
                    Parameters = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NotificationUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationReceivers", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_NotificationReceivers_Notifications_NotificationUid",
                        column: x => x.NotificationUid,
                        principalTable: "Notifications",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessages",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TokenType = table.Column<byte>(type: "smallint", nullable: true),
                    NotificationTokenUid = table.Column<Guid>(type: "uuid", nullable: true),
                    NotificationUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExceptionMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsView = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessages", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_Notifications_NotificationUid",
                        column: x => x.NotificationUid,
                        principalTable: "Notifications",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_NotificationTokens_NotificationTokenUid",
                        column: x => x.NotificationTokenUid,
                        principalTable: "NotificationTokens",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_NotificationTokenUid",
                table: "NotificationMessages",
                column: "NotificationTokenUid");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_NotificationUid",
                table: "NotificationMessages",
                column: "NotificationUid");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_NotificationUid",
                table: "NotificationReceivers",
                column: "NotificationUid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationMessages");

            migrationBuilder.DropTable(
                name: "NotificationReceivers");

            migrationBuilder.DropTable(
                name: "NotificationTokens");

            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
