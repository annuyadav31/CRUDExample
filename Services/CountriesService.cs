namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private readonly
        private readonly PersonsDbContext _db;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {

            _db = personsDbContext;   
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
            if (_db.Countries.Where(x=>x.CountryName == countryAddRequest.CountryName).Count()>0)
            {
                throw new ArgumentException("CountryName already exists");
            }

            //Convert "countryAddRequest" from "CountryAddRequest" to "Country"
            Country country = countryAddRequest.ToCountry();

            //Generate a new CountryID
            country.CountryId = Guid.NewGuid();

            //Then add it into database

            _db.Countries.Add(country);
            _db.SaveChanges();

            //Return CountryResponse object with generated CountryID
            return country.ToCountryResponse();

        }

        public CountryResponse GetCountryById(Guid? countryId)
        {
            if(countryId == null)
            {
                throw new ArgumentException(nameof(countryId));
            }

            Country? countryDetails =  _db.Countries.Where(x => x.CountryId == countryId).FirstOrDefault();

            if (countryDetails == null) { return null; }

            return countryDetails.ToCountryResponse();
        }

        public List<CountryResponse> GetCountryList()
        {
           return _db.Countries.Select(country=>country.ToCountryResponse()).ToList();
        }
    }
}