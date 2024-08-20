using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    public partial class AddDescriptionToSuggestedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "suggesteddate",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "suggesteddate");
        }
    }
}
