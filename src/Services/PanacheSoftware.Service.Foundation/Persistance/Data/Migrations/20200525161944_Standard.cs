using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Foundation.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    LanguageCodeId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    TextCode = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettingHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: false),
                    SettingType = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    LanguageHeaderId = table.Column<Guid>(nullable: false),
                    LanguageCodeId = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageItem_LanguageHeader_LanguageHeaderId",
                        column: x => x.LanguageHeaderId,
                        principalTable: "LanguageHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    SettingHeaderId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSetting_SettingHeader_SettingHeaderId",
                        column: x => x.SettingHeaderId,
                        principalTable: "SettingHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageItem_LanguageHeaderId",
                table: "LanguageItem",
                column: "LanguageHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSetting_SettingHeaderId",
                table: "UserSetting",
                column: "SettingHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageCode");

            migrationBuilder.DropTable(
                name: "LanguageItem");

            migrationBuilder.DropTable(
                name: "UserSetting");

            migrationBuilder.DropTable(
                name: "LanguageHeader");

            migrationBuilder.DropTable(
                name: "SettingHeader");
        }
    }
}
