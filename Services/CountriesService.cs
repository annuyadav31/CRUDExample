using Entities;
using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;
using System.Runtime.CompilerServices;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private readonly
        private readonly List<Country> _countries;

        //constructor
        public CountriesService()
        {
            _countries = new List<Country>();   
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


        public List<CountryResponse> GetCountryList()
        {
           return _countries.Select(country=>country.ToCountryResponse()).ToList();
        }
    }
}