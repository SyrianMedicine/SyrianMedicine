using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class EditReserves004 : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
