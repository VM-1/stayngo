using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayNGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameListingTimeZoneIdToTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time_zone_id",
                table: "listings",
                newName: "time_zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time_zone",
                table: "listings",
                newName: "time_zone_id");
        }
    }
}
