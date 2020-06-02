using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Foundation.Persistance.Data.Migrations
{
    public partial class TenantSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    SettingHeaderId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSetting_SettingHeader_SettingHeaderId",
                        column: x => x.SettingHeaderId,
                        principalTable: "SettingHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantSetting_SettingHeaderId",
                table: "TenantSetting",
                column: "SettingHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantSetting");
        }
    }
}
