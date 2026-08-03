using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerceAPI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_ProductId",
                table: "CartItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId_ProductId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");
        }
    }
}
