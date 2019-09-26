using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionService.Database.Migrations
{
    public partial class NewConfigFormat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventStoreServers",
                columns: table => new
                {
                    EventStoreServerId = table.Column<Guid>(nullable: false),
                    ConnectionString = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStoreServers", x => x.EventStoreServerId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionConfigurations",
                columns: table => new
                {
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    EventStoreServerId = table.Column<Guid>(nullable: false),
                    StreamName = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    EndPointUri = table.Column<string>(nullable: true),
                    StreamPosition = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionConfigurations", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_SubscriptionConfigurations_EventStoreServers_EventStoreServe~",
                        column: x => x.EventStoreServerId,
                        principalTable: "EventStoreServers",
                        principalColumn: "EventStoreServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionConfigurations_EventStoreServerId",
                table: "SubscriptionConfigurations",
                column: "EventStoreServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionConfigurations");

            migrationBuilder.DropTable(
                name: "EventStoreServers");
        }
    }
}
