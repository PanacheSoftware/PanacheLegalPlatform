using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.File.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    FileHeaderId = table.Column<Guid>(nullable: false),
                    FileTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    FileExtension = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDetail_FileHeader_FileHeaderId",
                        column: x => x.FileHeaderId,
                        principalTable: "FileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    LinkId = table.Column<Guid>(nullable: false),
                    LinkType = table.Column<string>(nullable: false),
                    FileHeaderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileLink_FileHeader_FileHeaderId",
                        column: x => x.FileHeaderId,
                        principalTable: "FileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    FileHeaderId = table.Column<Guid>(nullable: false),
                    URI = table.Column<string>(nullable: true),
                    Content = table.Column<byte[]>(nullable: true),
                    UntrustedName = table.Column<string>(nullable: true),
                    TrustedName = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    VersionNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileVersion_FileHeader_FileHeaderId",
                        column: x => x.FileHeaderId,
                        principalTable: "FileHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileHeaderId",
                table: "FileDetail",
                column: "FileHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileLink_FileHeaderId",
                table: "FileLink",
                column: "FileHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersion_FileHeaderId",
                table: "FileVersion",
                column: "FileHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDetail");

            migrationBuilder.DropTable(
                name: "FileLink");

            migrationBuilder.DropTable(
                name: "FileVersion");

            migrationBuilder.DropTable(
                name: "FileHeader");
        }
    }
}
