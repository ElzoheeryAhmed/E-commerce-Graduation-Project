using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    public partial class ProductUpdate_InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductUpdates",
                columns: table => new
                {
                    CurrentProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedProductId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUpdates", x => new { x.CurrentProductId, x.UpdatedProductId });
                    table.ForeignKey(
                        name: "FK_ProductUpdates_Products_CurrentProductId",
                        column: x => x.CurrentProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductUpdates_Products_UpdatedProductId",
                        column: x => x.UpdatedProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductUpdates_UpdatedProductId",
                table: "ProductUpdates",
                column: "UpdatedProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUpdates");
        }
    }
}
