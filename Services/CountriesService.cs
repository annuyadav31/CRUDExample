using Microsoft.EntityFrameworkCore;

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
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            if (await _db.Countries.CountAsync(temp=>temp.CountryName == countryAddRequest.CountryName)>0)
            {
                throw new ArgumentException("CountryName already exists");
            }

            //Convert "countryAddRequest" from "CountryAddRequest" to "Country"
            Country country = countryAddRequest.ToCountry();

            //Generate a new CountryID
            country.CountryId = Guid.NewGuid();

            //Then add it into database

            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            //Return CountryResponse object with generated CountryID
            return country.ToCountryResponse();

        }

        public async Task<CountryResponse> GetCountryById(Guid? countryId)
        {
            if(countryId == null)
            {
                throw new ArgumentException(nameof(countryId));
            }

            Country? countryDetails = await _db.Countries.Where(x => x.CountryId == countryId).FirstOrDefaultAsync();

            if (countryDetails == null) { return null; }

            return countryDetails.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetCountryList()
        {
           return await _db.Countries.Select(country=>country.ToCountryResponse()).ToListAsync();
        }
    }
}