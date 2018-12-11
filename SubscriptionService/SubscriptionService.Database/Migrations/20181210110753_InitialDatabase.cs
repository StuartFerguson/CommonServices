using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionService.Database.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndPoints",
                columns: table => new
                {
                    EndPointId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndPoints", x => x.EndPointId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionStream",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsNetCoreDomainStream = table.Column<bool>(nullable: false),
                    StreamName = table.Column<string>(nullable: true),
                    SubscriptionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionStream", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatchUpSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    EndPointId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    StreamName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatchUpSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatchUpSubscriptions_EndPoints_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "EndPoints",
                        principalColumn: "EndPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BufferSize = table.Column<int>(nullable: true),
                    EndPointId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StreamPosition = table.Column<int>(nullable: true),
                    SubscriptionStreamId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionGroups_EndPoints_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "EndPoints",
                        principalColumn: "EndPointId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionGroups_SubscriptionStream_SubscriptionStreamId",
                        column: x => x.SubscriptionStreamId,
                        principalTable: "SubscriptionStream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatchUpSubscriptions_EndPointId",
                table: "CatchUpSubscriptions",
                column: "EndPointId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionGroups_EndPointId",
                table: "SubscriptionGroups",
                column: "EndPointId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionGroups_SubscriptionStreamId",
                table: "SubscriptionGroups",
                column: "SubscriptionStreamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatchUpSubscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionGroups");

            migrationBuilder.DropTable(
                name: "EndPoints");

            migrationBuilder.DropTable(
                name: "SubscriptionStream");
        }
    }
}
