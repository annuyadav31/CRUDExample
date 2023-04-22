using Entities;
using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _people;
        private readonly ICountriesService _countriesService;

        public PersonService()
        {
            _people = new List<Person>();
            _countriesService = new CountriesService();
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryById(person.CountryId)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if "personAddRequest" is not null
            if(personAddRequest == null)
            { throw new ArgumentNullException(nameof(personAddRequest));}

            //Check if "personName" is not null
            if(personAddRequest.PersonName == null)
            { throw new ArgumentException(nameof(personAddRequest.PersonName));}

            //Convert personAddRequest to person type obect
            Person person = personAddRequest.ToPerson();

            //Generate new personId
            person.PersonId = Guid.NewGuid();

            //Then add it to List<Person>
            _people.Add(person);
            
            //Return PersonResponse Object
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersonsList()
        {
            return _people.Select(temp=>temp.ToPersonResponse()).ToList();
        }

    }
}
