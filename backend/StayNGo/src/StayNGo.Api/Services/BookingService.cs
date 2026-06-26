using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Extensions;
using StayNGo.Api.Features.Bookings;
using StayNGo.Api.Features.Bookings.Create;
using StayNGo.Api.Features.Bookings.Reservations;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.Api.Services;

public class BookingService(StayNGoDbContext db, ICurrentUserService currentUserService) : IBookingService
{
    public async Task<PageResult<BookingContract>> GetMyTrips(GetBookingFilter filter)
    {
        var user = await currentUserService.GetAsync();
        var query = db.Bookings
            .Where(x => x.GuestUserId == user.Id)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        var totalCount = await query.CountAsync();
        var bookings = await query.ApplyPagination(filter).ToListAsync();

        return new PageResult<BookingContract>(BookingContract.FromDomain(bookings), filter, totalCount);
    }

    public async Task<PageResult<ReservationContract>> GetReservations(GetBookingFilter filter)
    {
        var user = await currentUserService.GetAsync();
        var query = db.Bookings
            .Include(x => x.Guest)
            .Where(x => x.Listing.OwnerUserId == user.Id)
            .OrderByDescending(x => x.Status == BookingStatus.Pending)
            .ThenByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        var totalCount = await query.CountAsync();

        var bookings = await query.ApplyPagination(filter).ToListAsync();
        return new PageResult<ReservationContract>(ReservationContract.FromDomain(bookings), filter, totalCount);
    }

    public async Task<BookingContract> CreateAsync(CreateBookingRequest request, Guid idempotencyKey)
    {
        var user = await currentUserService.GetAsync();
        var listing = await db.Listings.FirstOrDefaultAsync(x => x.Id == request.ListingId &&
                                                                 x.Status == ListingStatus.Published &&
                                                                 x.OwnerUserId != user.Id);

        if (listing is null)
        {
            throw new RecordNotFoundException(nameof(listing), request.ListingId);
        }

        var booking = Booking.CreateBooking(user.Id, request.CheckIn, request.CheckOut, listing, idempotencyKey);

        db.Bookings.Add(booking);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when ((ex.InnerException as PostgresException)?.SqlState == "23505")
        {
            // Same (guest, idempotency key) already created a booking — replay it instead of creating a duplicate.
            db.ChangeTracker.Clear();
            var existing = await db.Bookings.SingleAsync(
                x => x.GuestUserId == user.Id && x.IdempotencyKey == idempotencyKey);
            return BookingContract.FromDomain(existing);
        }

        return BookingContract.FromDomain(booking);
    }

    public async Task<BookingContract> ConfirmReservation(Guid bookingId)
    {
        var booking = await GetOwnedReservationAsync(bookingId);
        booking.Confirm();
        await db.SaveChangesAsync();
        return BookingContract.FromDomain(booking);
    }

    public async Task<BookingContract> RejectReservation(Guid bookingId)
    {
        var booking = await GetOwnedReservationAsync(bookingId);
        booking.Reject();
        await db.SaveChangesAsync();
        return BookingContract.FromDomain(booking);
    }

    private async Task<Booking> GetOwnedReservationAsync(Guid bookingId)
    {
        var user = await currentUserService.GetAsync();
        var booking =
            await db.Bookings.SingleOrDefaultAsync(x => x.Id == bookingId && x.Listing.OwnerUserId == user.Id);
        return booking ?? throw new RecordNotFoundException(nameof(Booking), bookingId);
    }
}