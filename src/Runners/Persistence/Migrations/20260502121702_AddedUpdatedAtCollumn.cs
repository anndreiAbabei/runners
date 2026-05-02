using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runners.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUpdatedAtCollumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "RunnerItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RunnerItems");
        }
    }
}
