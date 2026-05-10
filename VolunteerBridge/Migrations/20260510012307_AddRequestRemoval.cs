using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerBridge.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestRemoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminRemovalReason",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemovedByAdmin",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemovalAcknowledged",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRemovalReason",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsRemovedByAdmin",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RemovalAcknowledged",
                table: "ServiceRequests");
        }
    }
}
