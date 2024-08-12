using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartiesAPI.Migrations
{
    public partial class FixOrganizerDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Events_Users_OrganizerId",
            //    table: "Events");
            // BUG WITH DROPFOREIGNKEY METHOD, USES 'DROP CONSTRAINT' INSTEAD OF MYSQL'S 'DROP FOREIGN KEY'
            migrationBuilder.Sql("ALTER TABLE `Events` DROP FOREIGN KEY `FK_Events_Users_OrganizerId`;");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_OrganizerId",
                table: "Events",
                column: "OrganizerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_OrganizerId",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_OrganizerId",
                table: "Events",
                column: "OrganizerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
