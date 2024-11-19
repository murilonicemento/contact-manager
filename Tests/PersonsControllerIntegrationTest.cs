using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

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
        
        string responseBody = await response.Content.ReadAsStringAsync();
        HtmlDocument html = new HtmlDocument();
        
        html.LoadHtml(responseBody);

        HtmlNode document = html.DocumentNode;

        document.QuerySelectorAll("table.persons").Should().NotBeNull();
    }

    #endregion
}