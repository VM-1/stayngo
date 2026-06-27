using System.Net;
using FluentAssertions;

namespace StayNGo.IntegrationTests.Listings;

public class ListingsBrowseEndpointTests(IntegrationTestFactory factory) : BaseIntegrationTests(factory)
{
    // Guards the [AsParameters] binding regression: pagination params are optional,
    // so a bare GET /listings must return 200, not 400 "Required parameter int Page".
    [Fact]
    public async Task GetListings_WithoutPaginationQueryParams_Returns200()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/listings");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
