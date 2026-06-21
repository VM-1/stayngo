using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace StayNGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceBookingDuringWithCheckInCheckOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"ALTER TABLE bookings DROP CONSTRAINT IF EXISTS bookings_no_overlap_confirmed");
            
            migrationBuilder.DropIndex(
                name: "ix_bookings_listing_id",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "during",
                table: "bookings");

            migrationBuilder.AddColumn<DateOnly>(
                name: "check_in",
                table: "bookings",
                type: "date",
                nullable: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "check_out",
                table: "bookings",
                type: "date",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_listing_id_check_in_check_out",
                table: "bookings",
                columns: ["listing_id", "check_in", "check_out"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_bookings_listing_id_check_in_check_out",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "check_in",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "check_out",
                table: "bookings");

            migrationBuilder.AddColumn<NpgsqlRange<DateOnly>>(
                name: "during",
                table: "bookings",
                type: "daterange",
                nullable: false,
                defaultValue: new NpgsqlTypes.NpgsqlRange<System.DateOnly>(new DateOnly(1, 1, 1), false, new DateOnly(1, 1, 1), false));

            migrationBuilder.CreateIndex(
                name: "ix_bookings_listing_id",
                table: "bookings",
                column: "listing_id");
        }
    }
}
