using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Task.Persistance.Data.Migrations
{
    public partial class ShortName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "TemplateItemHeader",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "TaskHeader",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "TemplateItemHeader");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "TaskHeader");
        }
    }
}
