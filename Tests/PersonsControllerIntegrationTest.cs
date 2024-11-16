using FluentAssertions;

namespace Tests;

public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Index

    [Fact]
    public async Task Index_ToReturnView()
    {
        HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

        response.Should().BeSuccessful();
    }

    #endregion
}