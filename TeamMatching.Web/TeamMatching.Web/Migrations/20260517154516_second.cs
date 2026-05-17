using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamMatching.Web.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TeamPosts_AuthorId",
                table: "TeamPosts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPosts_Users_AuthorId",
                table: "TeamPosts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamPosts_Users_AuthorId",
                table: "TeamPosts");

            migrationBuilder.DropIndex(
                name: "IX_TeamPosts_AuthorId",
                table: "TeamPosts");
        }
    }
}
