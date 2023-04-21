﻿using Entities;

namespace ServiceContracts.ModelDTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService Methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }

        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if(obj.GetType() != typeof(CountryResponse))
            {  return false; }
            CountryResponse country_to_compare = (CountryResponse)obj;
            return CountryId == country_to_compare.CountryId && CountryName == country_to_compare.CountryName;
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
