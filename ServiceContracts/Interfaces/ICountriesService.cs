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
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Gets the all countries
        /// </summary>
        /// <returns>Returns all the countries present in the list with Guid id</returns>
        Task<List<CountryResponse>> GetCountryList();

        /// <summary>
        /// Gets the details of a particular country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>Returns the details of a particular id</returns>
        Task<CountryResponse> GetCountryById(Guid? countryId);
    }
}
