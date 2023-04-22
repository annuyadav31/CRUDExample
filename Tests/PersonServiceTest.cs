namespace Tests
{
    public class PersonServiceTest
    {
        #region "Readonly fields"
        //private readonly variable for Service Interface
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        #endregion

        #region "Constructor"
        public PersonServiceTest(ITestOutputHelper testOutputHelper) 
        {
            _personService = new PersonService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }
        #endregion

        #region "Common private method"
        private List<PersonResponse> AddFewPersonsToList()
        {
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "UK" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>
            {   new PersonAddRequest(){   PersonName = "Mary",
                Email = "Test1@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test1Address",
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true },

                new PersonAddRequest() {  PersonName = "Rehman",
                Email = "Test2@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test2Address",
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false},

                new PersonAddRequest() {  PersonName = "Bhupesh",
                Email = "Test2@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test2Address",
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false},
            };

            List<PersonResponse> expectedPersonResponses = new List<PersonResponse>();

            //Act
            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                expectedPersonResponses.Add(_personService.AddPerson(personAddRequest));
            }

            //print input
            _testOutputHelper.WriteLine("Input:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            return expectedPersonResponses;
        }
        #endregion

        #region "AddPersonTests"


        //when PersonAddRequest Object is null throws ArgumentNullException
        [Fact]
        public void AddPerson_PersonAddRequestIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(()=>
            {
                //Act
                _personService.AddPerson(personAddRequest);
            });
        }

        //when PersonName in PersonAddRequest Object is null throws ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null};

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.AddPerson(personAddRequest);
            });
        }

        //when PersonAddRequest Object is correct return PersonResponse
        [Fact]
        public void AddPerson_PersonAddRequestIsCorrect()
        {
            //Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "Test@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "TestAddress",
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            //Act
            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);
            List<PersonResponse> personResponses = _personService.GetAllPersonsList();

            //Assert
            Assert.True(personResponse_from_add.PersonId != Guid.Empty);
            Assert.Contains(personResponse_from_add, personResponses);
        }
        #endregion

        #region "GetPersonListTests"

        //When PersonList is empty
        [Fact]
        public void GetAllPersonsList_EmptyList()
        {
            //Act
            List<PersonResponse> personResponses = _personService.GetAllPersonsList();

            //Assert
            Assert.Empty(personResponses);
        }

        //When we have added few persons to the list
        [Fact]
        public void GetAllPersonsList_AddFewPersonsList()
        {

            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>
            { new PersonAddRequest(){   PersonName = "Test1",
                Email = "Test1@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test1Address",
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true },
                new PersonAddRequest() {  PersonName = "Test2",
                Email = "Test2@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test2Address",
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false}
            };

            List<PersonResponse> expectedPersonResponses = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest personAddRequest in personAddRequests)
            {
               expectedPersonResponses.Add(_personService.AddPerson(personAddRequest));
            }

            //print expectedOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponseList = _personService.GetAllPersonsList();

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach(PersonResponse personResponse in actualPersonResponseList)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach(PersonResponse expectedResponse in expectedPersonResponses)
            {
                Assert.Contains(expectedResponse, actualPersonResponseList);
            }
        }
        #endregion

        #region "GetPersonByIdTests"

        //When Id is null, throw argumentException 

        [Fact]
        public void GetPersonById_IdIsNull()
        {
            //Arrange
            Guid? personId = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.GetPersonById(personId);
            });
        }

        //When personIdNotFound
        [Fact]
        public void GetPersonById_IdNotFound()
        {
            //Arrange
            Guid personId = Guid.NewGuid();

            //Act
            PersonResponse? personResponse = _personService.GetPersonById(personId);

            //Assert
            Assert.Null(personResponse);
        }

        //When personIdFound
        [Fact]
        public void GetPersonById_IdFound()
        {

            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() {CountryName="UK"};
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "Test@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "TestAddress",
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse expectedPersonResponse = _personService.AddPerson(personAddRequest);

            //Act
            PersonResponse? actualPersonResponse = _personService.GetPersonById(expectedPersonResponse.PersonId);

            //Assert
            Assert.Equal(expectedPersonResponse, actualPersonResponse);

        }
        #endregion

        #region "GetFilteredListTests"

        //When search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public void GetFilteredList_EmptySearchText()
        {

            //Arrange
            List<PersonResponse> expectedPersonResponses = AddFewPersonsToList();

            //print expectedOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponseList = _personService.GetFilteredList(nameof(Person.PersonName),"");

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonResponseList)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (PersonResponse expectedResponse in expectedPersonResponses)
            {
                Assert.Contains(expectedResponse, actualPersonResponseList);
            }
        }

        //First we will add few persons and then we will search based on person name with some search string. It should return matching persons.
        [Fact]
        public void GetFilteredList_SearchByPersonName()
        {

            //Arrange
            List<PersonResponse> personResponsesFromAdd = AddFewPersonsToList();
            List<PersonResponse> expectedPersonReponseAfterSearch = new List<PersonResponse>();
            string? searchString = "ma";

            //ExpectedList
            foreach (PersonResponse personResponse in personResponsesFromAdd)
            {
                if (personResponse.PersonName != null)
                {
                    if (personResponse.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    {
                        expectedPersonReponseAfterSearch.Add(personResponse);
                    }
                }
            }

            //print expectedOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonReponseAfterSearch)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponseList = _personService.GetFilteredList(nameof(Person.PersonName), searchString);

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonResponseList)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            for (int i = 0; i < expectedPersonReponseAfterSearch.Count; i++)
            {
                Assert.Equal(expectedPersonReponseAfterSearch[i], actualPersonResponseList[i]);
            }
        }

        #endregion

        #region "GetSortedPersonsListTests"
        //When we sort based on personName in DESC , it should return persons List in descending on personName
        [Fact]
        public void GetSortedPersonsList_SearchByPersonNameDesc()
        {

            //Arrange
            List<PersonResponse> personResponses_from_add = AddFewPersonsToList();
            
            //ExpectedSortedList by Desc order
            List<PersonResponse> expectedPersonListAfterSort = personResponses_from_add.OrderByDescending(temp=>temp.PersonName).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse personResponse in expectedPersonListAfterSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> allPersons = _personService.GetAllPersonsList();
            List<PersonResponse> actualPersonListAfterSort = _personService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.DESC);

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonListAfterSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            for(int i=0; i<expectedPersonListAfterSort.Count;i++)
            {
                Assert.Equal(expectedPersonListAfterSort[i], actualPersonListAfterSort[i]);
            }
        }

        #endregion
    }
}
