using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class addIsEmailConfirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComment_posts_PostId",
                table: "PostComment");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_Aspnetuser_UserId",
                table: "posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_posts",
                table: "posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostComment",
                table: "PostComment");

            migrationBuilder.RenameTable(
                name: "posts",
                newName: "PostTable");

            migrationBuilder.RenameTable(
                name: "PostComment",
                newName: "comments");

            migrationBuilder.RenameIndex(
                name: "IX_posts_UserId",
                table: "PostTable",
                newName: "IX_PostTable_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComment_PostId",
                table: "comments",
                newName: "IX_comments_PostId");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTable",
                table: "PostTable",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "CommentId");

            migrationBuilder.CreateTable(
                name: "places",
                columns: table => new
                {
                    PlaceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PlaceName = table.Column<string>(type: "longtext", nullable: false),
                    PostalCode = table.Column<int>(type: "int", nullable: false),
                    TownName = table.Column<string>(type: "longtext", nullable: false),
                    StreetName = table.Column<string>(type: "longtext", nullable: false),
                    StoryLevel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    Rating = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_places", x => x.PlaceId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_PostTable_PostId",
                table: "comments",
                column: "PostId",
                principalTable: "PostTable",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTable_Aspnetuser_UserId",
                table: "PostTable",
                column: "UserId",
                principalTable: "Aspnetuser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_PostTable_PostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTable_Aspnetuser_UserId",
                table: "PostTable");

            migrationBuilder.DropTable(
                name: "places");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTable",
                table: "PostTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "PostTable",
                newName: "posts");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "PostComment");

            migrationBuilder.RenameIndex(
                name: "IX_PostTable_UserId",
                table: "posts",
                newName: "IX_posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_PostId",
                table: "PostComment",
                newName: "IX_PostComment_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_posts",
                table: "posts",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostComment",
                table: "PostComment",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComment_posts_PostId",
                table: "PostComment",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_Aspnetuser_UserId",
                table: "posts",
                column: "UserId",
                principalTable: "Aspnetuser",
                principalColumn: "Id");
        }
    }
}
