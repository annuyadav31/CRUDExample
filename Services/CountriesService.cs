using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private readonly
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesService(ICountriesRepository countriesRepository)
        {

            _countriesRepository = countriesRepository;  
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
            if (await _countriesRepository.GetCountryByName(countryAddRequest.CountryName)!= null)
            {
                throw new ArgumentException("CountryName already exists");
            }

            //Convert "countryAddRequest" from "CountryAddRequest" to "Country"
            Country country = countryAddRequest.ToCountry();

            //Generate a new CountryID
            country.CountryId = Guid.NewGuid();

            //Then add it into database

            await _countriesRepository.AddCountry(country);

            //Return CountryResponse object with generated CountryID
            return country.ToCountryResponse();

        }

        public async Task<CountryResponse> GetCountryById(Guid? countryId)
        {
            if(countryId == null)
            {
                throw new ArgumentException(nameof(countryId));
            }

            Country? countryDetails = await _countriesRepository.GetCountryById(countryId);

            if (countryDetails == null) { return null; }

            return countryDetails.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetCountryList()
        {
           return (await _countriesRepository.GetAllCountries()).Select(country=>country.ToCountryResponse()).ToList();
        }
    }
}