using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class CountriesServiceTest
    {
        #region "readonly fields"
        //private readonly class to hold Services
        private readonly ICountriesService _countriesService;
        #endregion

        #region "Constructor"
        //constructor to create CountriesService Object
        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(dbContext);
        }
        #endregion

        #region "AddCountryTest"
        //when CountryAddRequest is null, it should return ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            //Act
            await _countriesService.AddCountry(countryAddRequest)
            );
        }

        //when CountryName is null, it should return Argument Exception
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            //Act
            await _countriesService.AddCountry(countryAddRequest)
            );

        }

        //when CountryName is duplicate, it should return Argument Exception
        [Fact]
        public async Task AddCountry_CountryNameIsDuplicate()
        {
            //Arrange
            CountryAddRequest? countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async()=>
            {
                //Act
               await _countriesService.AddCountry(countryAddRequest1);
               await _countriesService.AddCountry(countryAddRequest2);
            });
        }

        //when supply proper CountryName, it should insert to the existing list of the countries
        [Fact]
        public async Task AddCountry_CountryNameIsCorrect()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            //Act
            CountryResponse response =await _countriesService.AddCountry(countryAddRequest);
            List<CountryResponse> countries =await _countriesService.GetCountryList(); 

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response,countries);

        }
        #endregion

        #region "GetAllCountriesTest"

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_response =await _countriesService.GetCountryList();

            //Assert
            Assert.Empty(actual_response);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                new CountryAddRequest() {CountryName = "USA"},
                new CountryAddRequest() {CountryName = "JAPAN"},
                new CountryAddRequest() {CountryName = "INDIA"}
            };

            List<CountryResponse> expectedCountriesList = new List<CountryResponse>();

            //Act
            foreach(var countryAddRequest in countryAddRequests)
            {
               expectedCountriesList.Add(await _countriesService.AddCountry(countryAddRequest));
            }
            
            List<CountryResponse> actualCountriesList=await _countriesService.GetCountryList();

            //Assert
            foreach(CountryResponse expected_country in  expectedCountriesList)
            {
                Assert.Contains(expected_country, actualCountriesList);
            }
        }
        #endregion

        #region "GetCountryByIdTest"
        [Fact]
        public async Task GetCountryById_CountryIdIsNull()
        {
            //Arrange
            Guid? countryId = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async()=>
            {
                //Act
                await _countriesService.GetCountryById(countryId);
            });
        }

        [Fact]
        public async Task GetCountryById_CountryIdNotFound()
        {
            //Arrange
            Guid countryId = Guid.NewGuid();
            CountryResponse? expectedCountryResponse = null;

            //Act
            CountryResponse actualCountryResponse =await _countriesService.GetCountryById(countryId);

            //Assert
            Assert.Equal(expectedCountryResponse, actualCountryResponse);
        }

        [Fact]
        public async Task GetCountryById_CountryIdFound()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse_from_add =await _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse countryResponse_from_Method =await _countriesService.GetCountryById(countryResponse_from_add.CountryId);

            //Assert
            Assert.Equal(countryResponse_from_add, countryResponse_from_Method);

        }

        #endregion
    }
}