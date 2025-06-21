using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPassionProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierNamesToProductDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierNames",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierNames",
                table: "Products");
        }
    }
}
