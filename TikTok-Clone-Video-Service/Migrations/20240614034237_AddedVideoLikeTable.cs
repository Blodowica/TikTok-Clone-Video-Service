using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikTok_Clone_Video_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddedVideoLikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikedusersID",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "UserLikedVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoID = table.Column<int>(type: "int", nullable: false),
                    authID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLikedVideos_Videos_VideoID",
                        column: x => x.VideoID,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedVideos_VideoID",
                table: "UserLikedVideos",
                column: "VideoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLikedVideos");

            migrationBuilder.AddColumn<string>(
                name: "LikedusersID",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
