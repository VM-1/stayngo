using FluentValidation;

namespace StayNGo.Api.Features.Listings.Update;

// A draft may be saved incomplete, so this validates *format where present*, not completeness.
// Completeness for publishing is enforced by the domain (Listing publish gate).
public class UpsertListingRequestValidator : AbstractValidator<UpsertListingRequest>
{
    public UpsertListingRequestValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Capacity.HasValue);

        When(x => x.Price is not null, () =>
        {
            RuleFor(x => x.Price!.AmountCents).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Price!.Currency).Length(3);
        });
    }
}
