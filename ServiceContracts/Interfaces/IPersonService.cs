using ServiceContracts.ModelDTO;

namespace ServiceContracts.Interfaces
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
    }
}
