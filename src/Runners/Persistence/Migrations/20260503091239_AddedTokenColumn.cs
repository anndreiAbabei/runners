using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runners.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "RunnerItems",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "RunnerItems");
        }
    }
}
