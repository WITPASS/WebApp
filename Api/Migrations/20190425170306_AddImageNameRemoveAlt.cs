using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AddImageNameRemoveAlt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Alt",
                table: "Images",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Images",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Images",
                newName: "Alt");
        }
    }
}
