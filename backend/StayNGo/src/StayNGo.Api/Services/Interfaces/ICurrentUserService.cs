using StayNGo.Domain.Entities;

namespace StayNGo.Api.Services.Interfaces;

public interface ICurrentUserService
{
    Task<User> GetOrProvisionAsync(CancellationToken cancellationToken = default);
    Task<User> GetAsync(CancellationToken cancellationToken = default);
}