using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionService.Database.Migrations
{
    public partial class RemoveIsNetCoreStream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNetCoreDomainStream",
                table: "SubscriptionStream");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNetCoreDomainStream",
                table: "SubscriptionStream",
                nullable: false,
                defaultValue: false);
        }
    }
}
