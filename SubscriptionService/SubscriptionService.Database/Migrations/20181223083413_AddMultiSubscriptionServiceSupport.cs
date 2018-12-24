using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionService.Database.Migrations
{
    public partial class AddMultiSubscriptionServiceSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionServices",
                columns: table => new
                {
                    SubscriptionServiceId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionServices", x => x.SubscriptionServiceId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionServiceGroups",
                columns: table => new
                {
                    SubscriptionServiceGroupId = table.Column<Guid>(nullable: false),
                    SubscriptionGroupId = table.Column<Guid>(nullable: false),
                    SubscriptionServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionServiceGroups", x => x.SubscriptionServiceGroupId);
                    table.ForeignKey(
                        name: "FK_SubscriptionServiceGroups_SubscriptionGroups_SubscriptionGro~",
                        column: x => x.SubscriptionGroupId,
                        principalTable: "SubscriptionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionServiceGroups_SubscriptionServices_SubscriptionS~",
                        column: x => x.SubscriptionServiceId,
                        principalTable: "SubscriptionServices",
                        principalColumn: "SubscriptionServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionServiceGroups_SubscriptionGroupId",
                table: "SubscriptionServiceGroups",
                column: "SubscriptionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionServiceGroups_SubscriptionServiceId",
                table: "SubscriptionServiceGroups",
                column: "SubscriptionServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionServiceGroups");

            migrationBuilder.DropTable(
                name: "SubscriptionServices");
        }
    }
}
