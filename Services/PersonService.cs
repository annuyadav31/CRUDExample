using ServiceContracts.Enums;

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

            //Model Validations
            ValidationHelper.ModelValidation(personAddRequest);

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

        public PersonResponse GetPersonById(Guid? personId)
        {
            //If personId is null, throw ArgumentException
            if(personId == null)
            {
                throw new ArgumentException(nameof(personId));
            }

            Person? personResponse = _people.Where(temp=>temp.PersonId == personId).FirstOrDefault();

            //If personId is not null but personId doesn't exist
            if (personResponse == null) { return null; }

            return personResponse.ToPersonResponse();
            
        }

        public List<PersonResponse> GetFilteredList(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersonsList();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
            { return matchingPersons; }

            switch(searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(temp=>
                    (temp.DateOfBirth != null)?temp.DateOfBirth.Value.ToString("dd MMMM YYYY").Contains(searchString,StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(Person.ReceiveNewsLetters):
                    matchingPersons = allPersons.Where(temp =>
                    temp.ReceiveNewsLetters.ToString() == searchString).ToList();
                    break;
                default: matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            throw new NotImplementedException();
        }
    }
}
