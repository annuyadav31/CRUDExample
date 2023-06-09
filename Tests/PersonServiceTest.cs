﻿using AutoFixture;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts.ModelDTO;

namespace Tests
{
    public class PersonServiceTest
    {
        #region "Readonly fields"
        //private readonly variable for Service Interface
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        #endregion

        #region "Constructor"
        public PersonServiceTest(ITestOutputHelper testOutputHelper) 
        {
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            var personsInitialData = new List<Person>() { };
            var countriesInitialData = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;

            //Setup
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            //Initializing the readOnly Fields
            _countriesService = new CountriesService(null);
            _personService = new PersonService(_personsRepository);
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();
        }
        #endregion

        #region "Common private method"
        private async Task<List<PersonResponse>> AddFewPersonsToList()
        {

            //CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "UK" };
            //CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "India" };

            CountryAddRequest countryAddRequest1 = _fixture.Build<CountryAddRequest>().Create();
            CountryAddRequest countryAddRequest2 = _fixture.Build<CountryAddRequest>().Create();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>
            {   _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample@example.com").With(temp=>temp.CountryId,countryResponse1.CountryId).Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample2@example.com").With(temp=>temp.CountryId,countryResponse1.CountryId).Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample3@example.com").With(temp=>temp.CountryId,countryResponse2.CountryId).Create()
            };

            List<PersonResponse> expectedPersonResponses = new List<PersonResponse>();

            //Act
            foreach (PersonAddRequest personAddRequest in personAddRequests)
            {
                expectedPersonResponses.Add(await _personService.AddPerson(personAddRequest));
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
        public async Task AddPerson_PersonAddRequestIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = async () => { await _personService.AddPerson(personAddRequest); };

            //FluentAssertion
            await action.Should().ThrowAsync<ArgumentNullException>();

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async()=>
            //{
                //Act
                //await _personService.AddPerson(personAddRequest);
            //});
        }

        //when PersonName in PersonAddRequest Object is null throws ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null};

            //Arrange using autoFixture
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "Sample@example.com").With(temp=>temp.PersonName, null as string).Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
               await _personService.AddPerson(personAddRequest);
            });
        }

        //when PersonAddRequest Object is correct return PersonResponse
        [Fact]
        public async Task AddPerson_PersonAddRequestIsCorrect_ToBeSuccessful()
        {

            //Arrange using autoFixture
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"Sample@example.com").Create();

            PersonResponse personResponse_expected = personAddRequest.ToPerson().ToPersonResponse();

            _personRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(personAddRequest.ToPerson());

            //Act
            PersonResponse personResponse_from_add =await _personService.AddPerson(personAddRequest);
            personResponse_expected.PersonId = personResponse_from_add.PersonId;

            //FluentAssertion
            personResponse_from_add.PersonId.Should().NotBe(Guid.Empty);
            personResponse_from_add.Should().Be(personResponse_expected);

            //Writing actual and expected Output
            _testOutputHelper.WriteLine("Actual:");
            _testOutputHelper.WriteLine(personResponse_from_add.ToString());

            _testOutputHelper.WriteLine("Expected to be in personResponses:");
            _testOutputHelper.WriteLine(personResponse_expected.ToString());


        }
        #endregion

        #region "GetPersonListTests"

        //When PersonList is empty
        [Fact]
        public async Task GetAllPersonsList_EmptyList()
        {
            //Act
            List<PersonResponse> personResponses =await _personService.GetAllPersonsList();

            //Assert
            //Assert.Empty(personResponses);

            //FluentAssertion
            personResponses.Should().BeEmpty();
        }

        //When we have added few persons to the list
        [Fact]
        public async Task GetAllPersonsList_AddFewPersonsList()
        {

            //Arrange
            //CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };

            //Arrange With AutoFixture
            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>().Create();

            CountryResponse countryResponse =await _countriesService.AddCountry(countryAddRequest);

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>
            {
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample@example.com").With(temp=>temp.CountryId,countryResponse.CountryId).Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample2@example.com").With(temp=>temp.CountryId,countryResponse.CountryId).Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"sample3@example.com").With(temp=>temp.CountryId,countryResponse.CountryId).Create()
            };

            List<PersonResponse> expectedPersonResponses = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest personAddRequest in personAddRequests)
            {
               expectedPersonResponses.Add(await _personService.AddPerson(personAddRequest));
            }

            //print expectedOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponseList =await _personService.GetAllPersonsList();

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach(PersonResponse personResponse in actualPersonResponseList)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            //foreach(PersonResponse expectedResponse in expectedPersonResponses)
            //{
            //    Assert.Contains(expectedResponse, actualPersonResponseList);
            //}

            //FluentAssertion
            actualPersonResponseList.Should().BeEquivalentTo(expectedPersonResponses);
        }
        #endregion

        #region "GetPersonByIdTests"
            
        //When Id is null, throw argumentException 

        [Fact]
        public async Task GetPersonById_IdIsNull()
        {
            //Arrange
            Guid? personId = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
               await _personService.GetPersonById(personId);
            });
        }

        //When personIdNotFound
        [Fact]
        public async Task GetPersonById_IdNotFound()
        {
            //Arrange
            Guid personId = Guid.NewGuid();

            //Act
            PersonResponse? personResponse =await _personService.GetPersonById(personId);

            //Assert
            //Assert.Null(personResponse);

            //Fluent Assertion
            personResponse.Should().BeNull();
        }

        //When personIdFound
        [Fact]
        public async Task GetPersonById_IdFound()
        {

            //Arrange
            //CountryAddRequest countryAddRequest = new CountryAddRequest() {CountryName="UK"};

            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse =await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@example.com").With(temp => temp.CountryId, countryResponse.CountryId).Create();

            PersonResponse expectedPersonResponse =await _personService.AddPerson(personAddRequest);

            //Act
            PersonResponse? actualPersonResponse =await _personService.GetPersonById(expectedPersonResponse.PersonId);

            //Assert
            Assert.Equal(expectedPersonResponse, actualPersonResponse);

        }
        #endregion

        #region "GetFilteredListTests"

        //When search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredList_EmptySearchText()
        {

            //Arrange
            List<PersonResponse> expectedPersonResponses = await AddFewPersonsToList();

            //print expectedOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponseList =await _personService.GetFilteredList(nameof(Person.PersonName),"");

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
        public async Task GetFilteredList_SearchByPersonName()
        {

            //Arrange
            List<PersonResponse> personResponsesFromAdd = await AddFewPersonsToList();
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

            List<PersonResponse> actualPersonResponseList =await _personService.GetFilteredList(nameof(Person.PersonName), searchString);

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
        public async Task GetSortedPersonsList_SortByPersonNameDesc()
        {

            //Arrange
            List<PersonResponse> personResponses_from_add = await AddFewPersonsToList();
            
            //ExpectedSortedList by Desc order
            List<PersonResponse> expectedPersonListAfterSort = personResponses_from_add.OrderByDescending(temp=>temp.PersonName).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse personResponse in expectedPersonListAfterSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> allPersons =await _personService.GetAllPersonsList();
            List<PersonResponse> actualPersonListAfterSort =await _personService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.DESC);

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

        //When we sort based on personName in ASC , it should return persons List in descending on personName
        [Fact]
        public async Task GetSortedPersonsList_SortByPersonNameASC()
        {

            //Arrange
            List<PersonResponse> personResponses_from_add = await AddFewPersonsToList();

            //ExpectedSortedList by Desc order
            List<PersonResponse> expectedPersonListAfterSort = personResponses_from_add.OrderBy(temp => temp.PersonName).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonListAfterSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> allPersons =await _personService.GetAllPersonsList();
            List<PersonResponse> actualPersonListAfterSort =await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);

            //print actualOutput Result using testOutputHelper
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonListAfterSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            for (int i = 0; i < expectedPersonListAfterSort.Count; i++)
            {
                Assert.Equal(expectedPersonListAfterSort[i], actualPersonListAfterSort[i]);
            }
        }

        #endregion

        #region "UpdatePersonTests"
        //when we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonUpdateRequestIsNull()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        //When we supply correct personDetails, it should update the model
        [Fact]
        public async Task UpdatePerson_PersonUpdateRequestIsCorrect()
        {
            //Arrange 

            //Adding Country
            //CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };

            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse =await _countriesService.AddCountry(countryAddRequest);

            //Adding Person
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@example.com").With(temp=>temp.CountryId,countryResponse.CountryId).Create();

            PersonResponse personResponse =await _personService.AddPerson(personAddRequest);

            //Update Person Model
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>().With(temp => temp.Email, "sample@example.com").With(temp => temp.CountryId, countryResponse.CountryId).With(temp=>temp.PersonId,personResponse.PersonId).Create();

            //Act
            PersonResponse personResponseAfterUpdate =await _personService.UpdatePerson(personUpdateRequest);
            PersonResponse personResponseFromId =await _personService.GetPersonById(personResponse.PersonId);

            //Assert
            Assert.Equal(personResponseAfterUpdate, personResponseFromId);

        }
        #endregion

        #region "DeletePersonTests"

        //if you supply a invalid personId, it should return false
        [Fact]
        public async Task DeletePerson_personIdIsInvalid()
        {
            //Arrange
            Guid personIdToDelete = Guid.NewGuid();

            //Act
            bool isDeleted =await _personService.DeletePerson(personIdToDelete);

            //Assert
            Assert.False(isDeleted);
        }

        //if you supply valid personId, the isDeleted should be true
        [Fact]
        public async Task DeletePerson_personIdIsValid()
        {
            //Arrange
            List<PersonResponse> personResponsesFromAdd = await AddFewPersonsToList();
            Guid personIdToDelete = personResponsesFromAdd.First().PersonId;

            //Act
            bool isDeleted =await _personService.DeletePerson(personIdToDelete);

            //Assert
            //Assert.True(isDeleted);

            //Fluent Assertion
            isDeleted.Should().BeTrue();
        }


        #endregion

    }
}
