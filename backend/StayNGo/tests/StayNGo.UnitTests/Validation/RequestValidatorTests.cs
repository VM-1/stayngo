using FluentAssertions;
using StayNGo.Api.Features.Bookings.Create;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings.Update;

namespace StayNGo.UnitTests.Validation;

public class RequestValidatorTests
{
    private static readonly DateOnly Future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);

    // ---- CreateBookingRequest ----

    [Fact]
    public void CreateBooking_ValidRange_Passes()
    {
        var result = new CreateBookingRequestValidator()
            .Validate(new CreateBookingRequest(Guid.NewGuid(), Future, Future.AddDays(3)));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateBooking_CheckOutNotAfterCheckIn_FailsOnCheckOut()
    {
        var result = new CreateBookingRequestValidator()
            .Validate(new CreateBookingRequest(Guid.NewGuid(), Future, Future));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBookingRequest.CheckOut));
    }

    [Fact]
    public void CreateBooking_PastCheckIn_Fails()
    {
        var past = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        var result = new CreateBookingRequestValidator()
            .Validate(new CreateBookingRequest(Guid.NewGuid(), past, Future));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateBooking_EmptyListingId_Fails()
    {
        var result = new CreateBookingRequestValidator()
            .Validate(new CreateBookingRequest(Guid.Empty, Future, Future.AddDays(3)));

        result.IsValid.Should().BeFalse();
    }

    // ---- UpsertListingRequest (draft — lenient/format-only) ----

    [Fact]
    public void Upsert_EmptyDraft_Passes()
    {
        var result = new UpsertListingRequestValidator().Validate(new UpsertListingRequest());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Upsert_ZeroCapacity_Fails()
    {
        var result = new UpsertListingRequestValidator().Validate(new UpsertListingRequest { Capacity = 0 });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Upsert_BadCurrencyLength_Fails()
    {
        var result = new UpsertListingRequestValidator()
            .Validate(new UpsertListingRequest { Price = new MoneyContract(10000, "US") });

        result.IsValid.Should().BeFalse();
    }

    // ---- UpdatePublishedListingRequest ----

    [Fact]
    public void UpdatePublished_MissingFields_Fails()
    {
        var result = new UpdatePublishedListingRequestValidator().Validate(new UpdatePublishedListingRequest
        {
            Description = "",
            ImageUrls = [],
            MainImageUrl = "",
            Price = new MoneyContract(0, "USD"),
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdatePublished_Valid_Passes()
    {
        var result = new UpdatePublishedListingRequestValidator().Validate(new UpdatePublishedListingRequest
        {
            Description = "A bright loft.",
            ImageUrls = ["https://img/main.jpg"],
            MainImageUrl = "https://img/main.jpg",
            Price = new MoneyContract(12000, "EUR"),
        });

        result.IsValid.Should().BeTrue();
    }
}
