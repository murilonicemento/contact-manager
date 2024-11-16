using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly ICountriesRepository _countriesRepository;
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly Fixture _fixture;

    public CountriesServiceTest()
    {
        // MOCK DB CONTEXT
        // List<Country> countriesInitialData = new();
        // DbContextMock<ApplicationDbContext> dbContextMock =
        //     new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        // ApplicationDbContext dbContext = dbContextMock.Object;
        //
        // dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        // MOCK REPOSITORY
        _fixture = new Fixture();
        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countriesRepository = _countriesRepositoryMock.Object;
        _countriesService = new CountriesService(_countriesRepository);
    }

    #region AddCountry

    // When CountryAddRequests is null, it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry_ToBeArgumentNullException()
    {
        CountryAddRequest? request = null;

        Country country = _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>).Create();

        _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

        Func<Task> action = async () => { await _countriesService.AddCountry(request); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
    {
        CountryAddRequest countryAddRequest =
            _fixture.Build<CountryAddRequest>().With(temp => temp.Name, null as string).Create();
        Country country =
            _fixture.Build<Country>().With(temp => temp.Person, new List<Person>()).Create();

        _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

        Func<Task> action = async () => { await _countriesService.AddCountry(countryAddRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsDuplicate_ToBeArgumentException()
    {
        CountryAddRequest firstCountryRequest = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.Name, "Test name").Create();
        CountryAddRequest secondCountryRequest = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.Name, "Test name").Create();

        Country firstCountry = firstCountryRequest.ToCountry();
        Country secondCountry = secondCountryRequest.ToCountry();

        _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(firstCountry);

        //Return null when GetCountryByCountryName is called
        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(null as Country);

        CountryResponse firstCountryFromAddCountry = await _countriesService.AddCountry(firstCountryRequest);

        Func<Task> action = async () =>
        {
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(firstCountry);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(firstCountry);

            await _countriesService.AddCountry(secondCountryRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When you supply proper country name, it should insert (add) the country to existing list of countries
    [Fact]
    public async Task AddCountry_FullCountry_ToBeSuccessful()
    {
        CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();
        Country country = countryRequest.ToCountry();
        CountryResponse countryResponse = country.ToCountryResponse();

        _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(null as Country);


        CountryResponse countryFromAddCountry = await _countriesService.AddCountry(countryRequest);

        country.Id = countryFromAddCountry.Id;
        countryResponse.Id = countryFromAddCountry.Id;

        countryFromAddCountry.Id.Should().NotBe(Guid.Empty);
        countryFromAddCountry.Should().BeEquivalentTo(countryResponse);
    }

    #endregion

    #region GetAllCountries

    // The list of countries should be empty by default (before adding any countries)
    [Fact]
    public async Task GetAllCountries_ToBeEmptyList()
    {
        List<Country> countryEmptyList = new List<Country>();

        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryEmptyList);

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        actualCountryResponseList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCountries_ShouldHaveFewCountries()
    {
        List<Country> countryList = new List<Country>
        {
            _fixture.Build<Country>()
                .With(temp => temp.Person, null as List<Person>).Create(),
            _fixture.Build<Country>()
                .With(temp => temp.Person, null as List<Person>).Create()
        };
        List<CountryResponse> countryResponseList = countryList.Select(temp => temp.ToCountryResponse()).ToList();

        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryList);

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        actualCountryResponseList.Should().BeEquivalentTo(countryResponseList);
    }

    #endregion

    #region GetCountryByCountryId

    // If we supply null as CountryId, it should return null as CountryResponse
    [Fact]
    public async Task GetCountryByCountryId_NullCountryId_ToBeNull()
    {
        Guid? id = null;

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(null as Country);

        CountryResponse? countryResponseFromGetMethod = await _countriesService.GetCountryByCountryId(id);

        countryResponseFromGetMethod.Should().BeNull();
    }

    // If we supply a valid CountryId, it should return the matching country details as CountryResponse object
    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
    {
        Country country = _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>)
            .Create();
        CountryResponse countryResponse = country.ToCountryResponse();

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(country);

        CountryResponse? countryResponseFromGet = await _countriesService.GetCountryByCountryId(country.Id);

        countryResponseFromGet.Should().Be(countryResponse);
    }

    #endregion
}