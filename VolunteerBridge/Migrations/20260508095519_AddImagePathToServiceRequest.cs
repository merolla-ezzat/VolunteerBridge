using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerBridge.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ServiceRequests");
        }
    }
}
