﻿namespace ServiceContracts.Interfaces
{
    /// <summary>
    /// Reprsents business logic for manipulating person entity
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Adds a new person to the existing list of person
        /// </summary>
        /// <param name="personAddRequest">object to add</param>
        /// <returns>Returns generated PersonResponse</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Gets list of all existing persons
        /// </summary>
        /// <returns>Returns List of personResponse Type</returns>
        List<PersonResponse> GetAllPersonsList();

        /// <summary>
        /// Gets the person details by personId
        /// </summary>
        /// <param name="personId">PersonId is used to retrieve the person details</param>
        /// <returns>Returns the person details as personResponse</returns>
        PersonResponse GetPersonById(Guid? personId);

        /// <summary>
        /// Gets the filtered list that matches with the search field and search string
        /// </summary>
        /// <param name="searchBy">Search Field To Search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns the matching records in PersonResponse list.</returns>
        List<PersonResponse> GetFilteredList(string searchBy, string? searchString);
    }
}
