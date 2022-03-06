using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals");

            migrationBuilder.DropIndex(
                name: "IX_ReserveHospitals_HospitalId",
                table: "ReserveHospitals");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "ReserveHospitals");

            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userid = table.Column<string>(type: "TEXT", nullable: true),
                    ConnectionID = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    ConnecteDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnections_AspNetUsers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_ConnectionID",
                table: "UserConnections",
                column: "ConnectionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_userid",
                table: "UserConnections",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConnections");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "ReserveHospitals",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReserveHospitals_HospitalId",
                table: "ReserveHospitals",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }
    }
}
