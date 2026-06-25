using StayNGo.Domain.Entities;

namespace StayNGo.Api.Features.Common;

public record UserShortContract
{
    public required Guid Id { get; init; }
    public required string Email { get; init; } = null!;
    public required string DisplayName { get; init; } = null!;

    public static UserShortContract FromDomain(User user)
    {
        return new UserShortContract
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName
        };
    }
}