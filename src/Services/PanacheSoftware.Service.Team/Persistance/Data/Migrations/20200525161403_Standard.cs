using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Team.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    ShortName = table.Column<string>(nullable: false),
                    LongName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    ParentTeamId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamHeader_TeamHeader_ParentTeamId",
                        column: x => x.ParentTeamId,
                        principalTable: "TeamHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    TeamHeaderId = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamDetail_TeamHeader_TeamHeaderId",
                        column: x => x.TeamHeaderId,
                        principalTable: "TeamHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTeam",
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
                    TeamHeaderId = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTeam_TeamHeader_TeamHeaderId",
                        column: x => x.TeamHeaderId,
                        principalTable: "TeamHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamDetail_TeamHeaderId",
                table: "TeamDetail",
                column: "TeamHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamHeader_ParentTeamId",
                table: "TeamHeader",
                column: "ParentTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeam_TeamHeaderId",
                table: "UserTeam",
                column: "TeamHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamDetail");

            migrationBuilder.DropTable(
                name: "UserTeam");

            migrationBuilder.DropTable(
                name: "TeamHeader");
        }
    }
}
