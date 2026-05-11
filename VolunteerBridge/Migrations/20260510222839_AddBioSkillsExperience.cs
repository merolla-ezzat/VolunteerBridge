using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerBridge.Migrations
{
    /// <inheritdoc />
    public partial class AddBioSkillsExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Users");
        }
    }
}
