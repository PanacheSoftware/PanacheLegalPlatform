using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Task.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskGroupHeader",
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
                    ParentTaskGroupId = table.Column<Guid>(nullable: true),
                    MainUserId = table.Column<Guid>(nullable: false),
                    TeamHeaderId = table.Column<Guid>(nullable: false),
                    ClientHeaderId = table.Column<Guid>(nullable: false),
                    SequenceNumber = table.Column<int>(nullable: false),
                    CompletionDate = table.Column<DateTime>(nullable: false),
                    OriginalCompletionDate = table.Column<DateTime>(nullable: false),
                    CompletedOnDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    OriginalStartDate = table.Column<DateTime>(nullable: false),
                    Completed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskGroupHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskGroupHeader_TaskGroupHeader_ParentTaskGroupId",
                        column: x => x.ParentTaskGroupId,
                        principalTable: "TaskGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskGroupDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    TaskGroupHeaderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskGroupDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskGroupDetail_TaskGroupHeader_TaskGroupHeaderId",
                        column: x => x.TaskGroupHeaderId,
                        principalTable: "TaskGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    TaskGroupHeaderId = table.Column<Guid>(nullable: false),
                    MainUserId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    CompletionDate = table.Column<DateTime>(nullable: false),
                    OriginalCompletionDate = table.Column<DateTime>(nullable: false),
                    CompletedOnDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    OriginalStartDate = table.Column<DateTime>(nullable: false),
                    TaskType = table.Column<string>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    SequenceNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskHeader_TaskGroupHeader_TaskGroupHeaderId",
                        column: x => x.TaskGroupHeaderId,
                        principalTable: "TaskGroupHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    TaskHeaderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDetail_TaskHeader_TaskHeaderId",
                        column: x => x.TaskHeaderId,
                        principalTable: "TaskHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskDetail_TaskHeaderId",
                table: "TaskDetail",
                column: "TaskHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskGroupDetail_TaskGroupHeaderId",
                table: "TaskGroupDetail",
                column: "TaskGroupHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskGroupHeader_ParentTaskGroupId",
                table: "TaskGroupHeader",
                column: "ParentTaskGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHeader_TaskGroupHeaderId",
                table: "TaskHeader",
                column: "TaskGroupHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskDetail");

            migrationBuilder.DropTable(
                name: "TaskGroupDetail");

            migrationBuilder.DropTable(
                name: "TaskHeader");

            migrationBuilder.DropTable(
                name: "TaskGroupHeader");
        }
    }
}
