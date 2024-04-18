using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCV.Migrations
{
    /// <inheritdoc />
    public partial class Vu2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "CV");

            migrationBuilder.AddColumn<int>(
                name: "Hide",
                table: "CV",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hide",
                table: "CV");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CV",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
