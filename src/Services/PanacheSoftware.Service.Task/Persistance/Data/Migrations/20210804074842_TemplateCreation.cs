using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Task.Persistance.Data.Migrations
{
    public partial class TemplateCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.CreateTable(
                name: "TemplateHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ShortName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    LongName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TemplateHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateDetail_TemplateHeader_TemplateHeaderId",
                        column: x => x.TemplateHeaderId,
                        principalTable: "TemplateHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateGroupHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ShortName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    LongName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    TemplateHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateGroupHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateGroupHeader_TemplateHeader_TemplateHeaderId",
                        column: x => x.TemplateHeaderId,
                        principalTable: "TemplateHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateGroupDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TemplateGroupHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    DaysOffset = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateGroupDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateGroupDetail_TemplateGroupHeader_TemplateGroupHeaderId",
                        column: x => x.TemplateGroupHeaderId,
                        principalTable: "TemplateGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateItemHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TemplateGroupHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Title = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TaskType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateItemHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateItemHeader_TemplateGroupHeader_TemplateGroupHeaderId",
                        column: x => x.TemplateGroupHeaderId,
                        principalTable: "TemplateGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateItemDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TemplateItemHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    DaysOffset = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateItemDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateItemDetail_TemplateItemHeader_TemplateItemHeaderId",
                        column: x => x.TemplateItemHeaderId,
                        principalTable: "TemplateItemHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateDetail_TemplateHeaderId",
                table: "TemplateDetail",
                column: "TemplateHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateGroupDetail_TemplateGroupHeaderId",
                table: "TemplateGroupDetail",
                column: "TemplateGroupHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateGroupHeader_TemplateHeaderId",
                table: "TemplateGroupHeader",
                column: "TemplateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateItemDetail_TemplateItemHeaderId",
                table: "TemplateItemDetail",
                column: "TemplateItemHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateItemHeader_TemplateGroupHeaderId",
                table: "TemplateItemHeader",
                column: "TemplateGroupHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateDetail");

            migrationBuilder.DropTable(
                name: "TemplateGroupDetail");

            migrationBuilder.DropTable(
                name: "TemplateItemDetail");

            migrationBuilder.DropTable(
                name: "TemplateItemHeader");

            migrationBuilder.DropTable(
                name: "TemplateGroupHeader");

            migrationBuilder.DropTable(
                name: "TemplateHeader");
        }
    }
}
