using ServiceContracts.Enums;
using System.Net.Sockets;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _people;
        private readonly ICountriesService _countriesService;

        public PersonService(bool isInitialize = true)
        {
            _people = new List<Person>();
            _countriesService = new CountriesService();

            if(isInitialize )
            {
                _people.AddRange(new List<Person>() {
                new Person() {PersonId =  Guid.Parse("F45D9C32-502A-4E2F-86CF-71D3A1D818D8"),PersonName= "Idaline",Address="PO Box 62822",
                    ReceiveNewsLetters=true,Email="ipadkin0@webnode.com",Gender="Male",DateOfBirth=DateTime.Parse("15/06/2002"),
                    CountryId= Guid.Parse("EE06A028-565B-4510-A072-76772D17213D")
                },
                new Person() { PersonId = Guid.Parse("4A815927-21BB-4C6B-9A96-BB0DFC04B913"),PersonName = "Zorine",Address = "PO Box 56296",
                    ReceiveNewsLetters = false,Email = "zdenison1@gnu.org",Gender = "Female",DateOfBirth = DateTime.Parse("02/12/2003"),
                    CountryId = Guid.Parse("A923BCEA-E339-4DA9-A13B-0FF1EBDE8E72")
                },
                new Person() { PersonId = Guid.Parse("778C7498-C1B8-4D94-90CF-6351BFA055B6"),PersonName = "Shermy",Address = "1st Floor",
                    ReceiveNewsLetters = true,Email = "syoslowitz2@mashable.com",Gender = "Male",DateOfBirth = DateTime.Parse("03/07/1995"),
                    CountryId = Guid.Parse("B00278A7-1F91-4042-B2A6-AA09F7AAD8DE")
                },

                new Person() {PersonId =  Guid.Parse("FBD09B53-D314-46BB-ABB8-2A5C5D183C97"),PersonName= "Marion",Address= "Suite 1",
                    ReceiveNewsLetters=true,Email= "mbridgelandh@apache.org",Gender="Female",DateOfBirth=DateTime.Parse("07/11/2009"),
                    CountryId= Guid.Parse("EE06A028-565B-4510-A072-76772D17213D")
                },
                new Person() { PersonId = Guid.Parse("93E59D48-7AC1-48C0-8FD0-D97A81DB0875"),PersonName = "Milena",Address = "Suite 90",
                    ReceiveNewsLetters = false,Email = "mmccleani@reverbnation.com",Gender = "Female",DateOfBirth = DateTime.Parse("09/03/1998"),
                    CountryId = Guid.Parse("A923BCEA-E339-4DA9-A13B-0FF1EBDE8E72")
                },
                new Person() { PersonId = Guid.Parse("98CACABD-D488-4F98-A664-664D8CBE0B2B"),PersonName = "Vincent",Address = "Apt 694",
                    ReceiveNewsLetters = true,Email = "vdendlej@technorati.com",Gender = "Male",DateOfBirth = DateTime.Parse("09/01/1996"),
                    CountryId = Guid.Parse("EE06A028-565B-4510-A072-76772D17213D")
                }});
            }
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
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp=>
                    (temp.DateOfBirth != null)?temp.DateOfBirth.Value.ToString("dd MMMM YYYY").Contains(searchString,StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(PersonResponse.ReceiveNewsLetters):
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
