using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikTok_Clone_Video_Service.Migrations
{
    /// <inheritdoc />
    public partial class updatedVideLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LikedusersID",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikedusersID",
                table: "Videos");
        }
    }
}
