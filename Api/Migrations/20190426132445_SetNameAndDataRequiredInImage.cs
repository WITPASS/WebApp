using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class SetNameAndDataRequiredInImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Images",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "Images",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Images",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "Images",
                nullable: true,
                oldClrType: typeof(byte[]));
        }
    }
}
