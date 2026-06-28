using FluentValidation;

namespace StayNGo.Api.Features.Listings.Update;

// Editing a published listing: these fields must stay valid (it's already live).
public class UpdatePublishedListingRequestValidator : AbstractValidator<UpdatePublishedListingRequest>
{
    public UpdatePublishedListingRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.MainImageUrl).NotEmpty();
        RuleFor(x => x.ImageUrls).NotEmpty();
        RuleFor(x => x.Price).NotNull();
        RuleFor(x => x.Price.AmountCents).GreaterThan(0).When(x => x.Price is not null);
        RuleFor(x => x.Price.Currency).Length(3).When(x => x.Price is not null);
    }
}
