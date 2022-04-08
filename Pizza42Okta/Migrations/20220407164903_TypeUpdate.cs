using Microsoft.EntityFrameworkCore.Migrations;

namespace Pizza42Okta.Migrations
{
    public partial class TypeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PizzaOrderTypeId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TypeId",
                table: "Orders",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Types_TypeId",
                table: "Orders",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Types_TypeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TypeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PizzaOrderTypeId",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
