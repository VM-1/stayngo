using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayNGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_bookings_guest_user_id",
                table: "bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "idempotency_key",
                table: "bookings",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_guest_user_id_idempotency_key",
                table: "bookings",
                columns: ["guest_user_id", "idempotency_key"],
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_bookings_guest_user_id_idempotency_key",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "idempotency_key",
                table: "bookings");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_guest_user_id",
                table: "bookings",
                column: "guest_user_id");
        }
    }
}
