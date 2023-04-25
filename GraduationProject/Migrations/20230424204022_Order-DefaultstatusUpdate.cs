using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    public partial class OrderDefaultstatusUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "varchar(15)",
                nullable: false,
                defaultValue: "Confirmed",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldDefaultValue: "Ordered");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "varchar(15)",
                nullable: false,
                defaultValue: "Ordered",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldDefaultValue: "Confirmed");
        }
    }
}
