using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;

namespace StayNGo.IntegrationTests.FakeServices;

public class FakeCurrentUserService(User user) : ICurrentUserService
{
    public User CurrentUser { get; set; } = user;
    public Task<User> GetOrProvisionAsync(CancellationToken cancellationToken = default) => Task.FromResult(CurrentUser);

    public Task<User> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(CurrentUser);
}