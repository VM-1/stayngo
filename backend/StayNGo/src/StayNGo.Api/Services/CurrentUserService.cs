using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, StayNGoDbContext db) : ICurrentUserService
{
    private const string ClerkIdClaim = "sub";
    private const string EmailClaim = "email";
    private const string DisplayNameClaim = "name";

    private User? _cachedUser;

    public async Task<User> GetOrProvisionAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedUser is not null)
        {
            return _cachedUser;
        }

        var claims = httpContextAccessor.HttpContext?.User;

        if (claims?.Identity?.IsAuthenticated != true)
        {
            throw new AuthenticationException();
        }

        var clerkId = claims.FindFirst(ClerkIdClaim)?.Value ?? ThrowMissingClaim(ClerkIdClaim);

        var user = await db.Users.SingleOrDefaultAsync(x => x.ClerkId == clerkId, cancellationToken);

        if (user is not null)
        {
            return _cachedUser = user;
        }

        var email = claims.FindFirst(EmailClaim)?.Value ?? ThrowMissingClaim(EmailClaim);

        var displayName = claims.FindFirst(DisplayNameClaim)?.Value ?? ThrowMissingClaim(DisplayNameClaim);

        user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            ClerkId = clerkId,
        };

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex, nameof(User.ClerkId)))
        {
            db.Entry(user).State = EntityState.Detached;
            user = await db.Users.SingleOrDefaultAsync(x => x.ClerkId == clerkId, cancellationToken);
        }

        return _cachedUser = user!;
    }

    public async Task<User> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedUser is not null)
        {
            return _cachedUser;
        }

        var claims = httpContextAccessor.HttpContext?.User;

        if (claims?.Identity?.IsAuthenticated != true)
        {
            throw new AuthenticationException();
        }

        var clerkId = claims.FindFirst(ClerkIdClaim)?.Value ?? ThrowMissingClaim(ClerkIdClaim);

        var user = await db.Users.SingleOrDefaultAsync(x => x.ClerkId == clerkId, cancellationToken);

        if (user is null)
        {
            throw new BadHttpRequestException($"User with clerkId {clerkId} not found");
        }

        return _cachedUser = user;
    }

    private static string ThrowMissingClaim(string claimName)
    {
        throw new BadHttpRequestException($"JWT missing {claimName}' claim");
    }

    private static bool IsUniqueViolation(DbUpdateException ex, string column)
        => ex.InnerException is PostgresException { SqlState: "23505" } pg // unique_violation
           && pg.ConstraintName?.Contains(column, StringComparison.CurrentCultureIgnoreCase) == true;
}