namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private readonly
        private readonly List<Country> _countries;

        //constructor
        public CountriesService(bool isInitialize = true)
        {

            _countries = new List<Country>();   
            if(isInitialize )
            {
                _countries.AddRange( new List<Country>() {
                new Country() { CountryId = Guid.Parse("EE06A028-565B-4510-A072-76772D17213D"),CountryName="UK"},
                new Country() { CountryId = Guid.Parse("A923BCEA-E339-4DA9-A13B-0FF1EBDE8E72"), CountryName = "INDIA" },
                new Country() { CountryId = Guid.Parse("B00278A7-1F91-4042-B2A6-AA09F7AAD8DE"), CountryName = "JAPAN" },
                new Country() { CountryId = Guid.Parse("19D15CFB-2C97-481C-9437-D91A3148FE71"), CountryName = "US" },
                new Country() { CountryId = Guid.Parse("1B2B2F83-D565-4BA3-8F84-017912592673"), CountryName = "CHINA" },
                });               
            }
        }

        /// <summary>
        /// Represents the business logic for adding country
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns>Country Response after adding country</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {

            //Check if "countryAddRequest" is not null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validate all properties of countryAddRequest
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //if countryName is duplicate
            if (_countries.Where(x=>x.CountryName == countryAddRequest.CountryName).Count()>0)
            {
                throw new ArgumentException("CountryName already exists");
            }

            //Convert "countryAddRequest" from "CountryAddRequest" to "Country"
            Country country = countryAddRequest.ToCountry();

            //Generate a new CountryID
            country.CountryId = Guid.NewGuid();

            //Then add it into List<Country>

            _countries.Add(country);

            //Return CountryResponse object with generated CountryID
            return country.ToCountryResponse();

        }

        public CountryResponse GetCountryById(Guid? countryId)
        {
            if(countryId == null)
            {
                throw new ArgumentException(nameof(countryId));
            }

            Country? countryDetails =  _countries.Where(x => x.CountryId == countryId).FirstOrDefault();

            if (countryDetails == null) { return null; }

            return countryDetails.ToCountryResponse();
        }

        public List<CountryResponse> GetCountryList()
        {
           return _countries.Select(country=>country.ToCountryResponse()).ToList();
        }
    }
}