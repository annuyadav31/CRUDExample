namespace ServiceContracts.ModelDTO
{
    /// <summary>
    /// DTO Class that is used to add country details
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }

        public Country ToCountry()
        {
            return new Country() { CountryName = CountryName };
        }
    }


}
