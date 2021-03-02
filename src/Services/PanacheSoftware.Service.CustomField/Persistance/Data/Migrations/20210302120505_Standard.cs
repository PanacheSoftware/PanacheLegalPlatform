using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.CustomField.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomFieldGroupHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ShortName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    LongName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldGroupHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CustomFieldType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    GDPR = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    History = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldGroupDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomFieldGroupHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomFieldHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldGroupDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldGroupDetail_CustomFieldGroupHeader_CustomFieldGro~",
                        column: x => x.CustomFieldGroupHeaderId,
                        principalTable: "CustomFieldGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomFieldHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldDetail_CustomFieldHeader_CustomFieldHeaderId",
                        column: x => x.CustomFieldHeaderId,
                        principalTable: "CustomFieldHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomFieldHeaderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LinkId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LinkType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    FieldValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldValue_CustomFieldHeader_CustomFieldHeaderId",
                        column: x => x.CustomFieldHeaderId,
                        principalTable: "CustomFieldHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomFieldValueHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomFieldValueId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LinkId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LinkType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    FieldValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    OriginalCreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false),
                    LastUpdateBy = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldValueHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldValueHistory_CustomFieldValue_CustomFieldValueId",
                        column: x => x.CustomFieldValueId,
                        principalTable: "CustomFieldValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldDetail_CustomFieldHeaderId",
                table: "CustomFieldDetail",
                column: "CustomFieldHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldGroupDetail_CustomFieldGroupHeaderId",
                table: "CustomFieldGroupDetail",
                column: "CustomFieldGroupHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldValue_CustomFieldHeaderId",
                table: "CustomFieldValue",
                column: "CustomFieldHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldValueHistory_CustomFieldValueId",
                table: "CustomFieldValueHistory",
                column: "CustomFieldValueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomFieldDetail");

            migrationBuilder.DropTable(
                name: "CustomFieldGroupDetail");

            migrationBuilder.DropTable(
                name: "CustomFieldTag");

            migrationBuilder.DropTable(
                name: "CustomFieldValueHistory");

            migrationBuilder.DropTable(
                name: "CustomFieldGroupHeader");

            migrationBuilder.DropTable(
                name: "CustomFieldValue");

            migrationBuilder.DropTable(
                name: "CustomFieldHeader");
        }
    }
}
