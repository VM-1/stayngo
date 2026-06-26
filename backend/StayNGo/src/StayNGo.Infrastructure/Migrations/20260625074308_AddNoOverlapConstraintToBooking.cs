using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayNGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNoOverlapConstraintToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
              ALTER TABLE bookings ADD COLUMN during daterange
                GENERATED ALWAYS AS (daterange(check_in, check_out, '[)')) STORED;
              ALTER TABLE bookings ADD CONSTRAINT no_overlap_confirmed
                EXCLUDE USING gist (listing_id WITH =, during WITH &&) WHERE (status = 2);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             ALTER TABLE bookings DROP CONSTRAINT IF EXISTS no_overlap_confirmed;
             ALTER TABLE bookings DROP COLUMN IF EXISTS during;");
        }
    }
}
