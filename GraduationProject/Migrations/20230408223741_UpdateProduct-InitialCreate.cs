using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    public partial class UpdateProductInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProdutUpdates",
                columns: table => new
                {
                    CurrentProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductUpdateId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutUpdates", x => new { x.CurrentProductId, x.ProductUpdateId });
                    table.ForeignKey(
                        name: "FK_ProdutUpdates_Products_CurrentProductId",
                        column: x => x.CurrentProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProdutUpdates_Products_ProductUpdateId",
                        column: x => x.ProductUpdateId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutUpdates_ProductUpdateId",
                table: "ProdutUpdates",
                column: "ProductUpdateId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutUpdates");
        }
    }
}
