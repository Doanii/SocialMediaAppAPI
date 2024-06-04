using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class FollowingCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowingCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowingCount",
                table: "Users");
        }
    }
}
