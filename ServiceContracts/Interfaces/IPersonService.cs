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
    }
}
