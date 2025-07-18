using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIQuizGeneratorApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedQuizCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizCount",
                table: "AspNetUsers");
        }
    }
}
