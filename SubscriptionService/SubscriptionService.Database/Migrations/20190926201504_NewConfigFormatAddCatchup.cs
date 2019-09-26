using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionService.Database.Migrations
{
    public partial class NewConfigFormatAddCatchup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StreamPosition",
                table: "SubscriptionConfigurations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "CatchupSubscriptionConfigurations",
                columns: table => new
                {
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    EndPointUri = table.Column<string>(nullable: true),
                    EventStoreServerId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    StreamName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatchupSubscriptionConfigurations", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_CatchupSubscriptionConfigurations_EventStoreServers_EventSto~",
                        column: x => x.EventStoreServerId,
                        principalTable: "EventStoreServers",
                        principalColumn: "EventStoreServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatchupSubscriptionConfigurations_EventStoreServerId",
                table: "CatchupSubscriptionConfigurations",
                column: "EventStoreServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatchupSubscriptionConfigurations");

            migrationBuilder.AlterColumn<int>(
                name: "StreamPosition",
                table: "SubscriptionConfigurations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
