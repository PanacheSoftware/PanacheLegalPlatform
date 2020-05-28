using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PanacheSoftware.Service.Client.Persistance.Data.Migrations
{
    public partial class Standard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientHeader",
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
                    LongName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientContact",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    ClientHeaderId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    MainContact = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientContact_ClientHeader_ClientHeaderId",
                        column: x => x.ClientHeaderId,
                        principalTable: "ClientHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    ClientHeaderId = table.Column<Guid>(nullable: false),
                    url = table.Column<string>(nullable: true),
                    Base64Image = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDetail_ClientHeader_ClientHeaderId",
                        column: x => x.ClientHeaderId,
                        principalTable: "ClientHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastUpdateBy = table.Column<Guid>(nullable: false),
                    AddressType = table.Column<string>(nullable: false),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    AddressLine3 = table.Column<string>(nullable: true),
                    AddressLine4 = table.Column<string>(nullable: true),
                    AddressLine5 = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Email1 = table.Column<string>(nullable: true),
                    Email2 = table.Column<string>(nullable: true),
                    Email3 = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    Phone3 = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    ClientContactId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAddress_ClientContact_ClientContactId",
                        column: x => x.ClientContactId,
                        principalTable: "ClientContact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientAddress_ClientContactId",
                table: "ClientAddress",
                column: "ClientContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContact_ClientHeaderId",
                table: "ClientContact",
                column: "ClientHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetail_ClientHeaderId",
                table: "ClientDetail",
                column: "ClientHeaderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAddress");

            migrationBuilder.DropTable(
                name: "ClientDetail");

            migrationBuilder.DropTable(
                name: "ClientContact");

            migrationBuilder.DropTable(
                name: "ClientHeader");
        }
    }
}
