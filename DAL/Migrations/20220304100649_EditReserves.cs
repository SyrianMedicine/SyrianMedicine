using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class EditReserves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals");

            migrationBuilder.AddColumn<int>(
                name: "ReserveState",
                table: "ReserveNurses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HospitalId",
                table: "ReserveHospitals",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ReserveState",
                table: "ReserveHospitals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReserveState",
                table: "ReserveDoctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals");

            migrationBuilder.DropColumn(
                name: "ReserveState",
                table: "ReserveNurses");

            migrationBuilder.DropColumn(
                name: "ReserveState",
                table: "ReserveHospitals");

            migrationBuilder.DropColumn(
                name: "ReserveState",
                table: "ReserveDoctors");

            migrationBuilder.AlterColumn<int>(
                name: "HospitalId",
                table: "ReserveHospitals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveHospitals_Hospitals_HospitalId",
                table: "ReserveHospitals",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
