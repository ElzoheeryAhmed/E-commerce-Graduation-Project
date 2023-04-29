using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    public partial class IssueResponseNStatusUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

                migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true);
           
           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "Response",
                table: "Issues");

            
        }
    }
}
