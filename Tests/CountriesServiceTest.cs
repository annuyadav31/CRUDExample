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
        public CountriesServiceTest(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        #endregion

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

        #region "GetCountryByIdTest"
        [Fact]
        public void GetCountryById_CountryIdIsNull()
        {
            //Arrange
            Guid? countryId = null;

            //Assert
            Assert.Throws<ArgumentException>(()=>
            {
                //Act
                _countriesService.GetCountryById(countryId);
            });
        }

        [Fact]
        public void GetCountryById_CountryIdNotFound()
        {
            //Arrange
            Guid countryId = Guid.NewGuid();
            CountryResponse? expectedCountryResponse = null;

            //Act
            CountryResponse actualCountryResponse = _countriesService.GetCountryById(countryId);

            //Assert
            Assert.Equal(expectedCountryResponse, actualCountryResponse);
        }

        [Fact]
        public void GetCountryById_CountryIdFound()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse_from_add = _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse countryResponse_from_Method = _countriesService.GetCountryById(countryResponse_from_add.CountryId);

            //Assert
            Assert.Equal(countryResponse_from_add, countryResponse_from_Method);

        }

        #endregion
    }
}