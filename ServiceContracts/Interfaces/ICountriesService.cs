using ServiceContracts.ModelDTO;

namespace ServiceContracts.Interfaces
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it.</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Gets the all countries
        /// </summary>
        /// <returns></returns>
        List<CountryResponse> GetCountryList();
    }
}
