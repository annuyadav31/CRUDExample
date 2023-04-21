using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;
using Services;

namespace Tests
{
    public class CountriesServiceTest
    {
        //private readonly class to hold ICountriesService
        private readonly ICountriesService _countriesService;

        //constructor to create CountriesService Object
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region "AddCountryTest"
        //when CountryAddRequest is null, it should return ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            //Act
            _countriesService.AddCountry(countryAddRequest)
            );
        }

        //when CountryName is null, it should return Argument Exception
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            //Act
            _countriesService.AddCountry(countryAddRequest)
            );

        }

        //when CountryName is duplicate, it should return Argument Exception
        [Fact]
        public void AddCountry_CountryNameIsDuplicate()
        {
            //Arrange
            CountryAddRequest? countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };

            //Assert
            Assert.Throws<ArgumentException>(()=>
            {
                //Act
                _countriesService.AddCountry(countryAddRequest1);
                _countriesService.AddCountry(countryAddRequest2);
            });
        }

        //when supply proper CountryName, it should insert to the existing list of the countries
        [Fact]
        public void AddCountry_CountryNameIsCorrect()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            //Act
            CountryResponse response =_countriesService.AddCountry(countryAddRequest);
            List<CountryResponse> countries = _countriesService.GetCountryList(); 

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response,countries);

        }
        #endregion

        #region "GetAllCountriesTest"

        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_response = _countriesService.GetCountryList();

            //Assert
            Assert.Empty(actual_response);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
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
               expectedCountriesList.Add(_countriesService.AddCountry(countryAddRequest));
            }
            
            List<CountryResponse> actualCountriesList=_countriesService.GetCountryList();

            //Assert
            foreach(CountryResponse expected_country in  expectedCountriesList)
            {
                Assert.Contains(expected_country, actualCountriesList);
            }
        }
        #endregion
    }
}