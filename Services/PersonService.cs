using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;

namespace Services
{
    public class PersonService : IPersonService
    {
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            throw new NotImplementedException();
        }

        public List<PersonResponse> GetAllPersonsList()
        {
            throw new NotImplementedException();
        }
    }
}
