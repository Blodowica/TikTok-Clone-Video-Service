using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikTok_Clone_Video_Service.Migrations
{
    /// <inheritdoc />
    public partial class updatedVideoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Videos");
        }
    }
}
