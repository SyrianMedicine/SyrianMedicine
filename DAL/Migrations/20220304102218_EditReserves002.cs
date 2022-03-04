using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class EditReserves002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ReserveNurses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReserveNurses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ReserveHospitals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReserveHospitals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ReserveDoctors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReserveDoctors",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ReserveNurses");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReserveNurses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ReserveHospitals");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReserveHospitals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ReserveDoctors");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReserveDoctors");
        }
    }
}
