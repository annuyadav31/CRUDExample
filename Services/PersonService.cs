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
            if(string.IsNullOrEmpty(sortBy)) { return allPersons; }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName),SortOrderOptions.ASC) => allPersons.OrderBy(temp=>temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
                _=>allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            //Model Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //find the person in the existing table
            Person? personDetails =_people.Where(x => x.PersonId == personUpdateRequest.PersonId).FirstOrDefault();

            //if personDetails are null
            if(personDetails == null)
            {
                throw new ArgumentException("personId doesn't exist");
            }

            //update all details
            personDetails.PersonName = personUpdateRequest.PersonName;
            personDetails.Address = personUpdateRequest.Address;
            personDetails.Gender = personUpdateRequest.Gender.ToString();
            personDetails.CountryId = personUpdateRequest.CountryId;
            personDetails.DateOfBirth = personUpdateRequest.DateOfBirth;
            personDetails.Email = personUpdateRequest.Email;
            personDetails.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            return personDetails.ToPersonResponse();

        }

        public bool DeletePerson(Guid? personId)
        {
            if(personId == null)
            {
                return false;
            }

            Person? personDetails = _people.Where(x=>x.PersonId == personId).FirstOrDefault();
            if( personDetails == null) { return false; }

            _people.RemoveAll(x=>x.PersonId == personId);

            return true;

        }
    }
}
