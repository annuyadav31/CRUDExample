using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;
using Services;
using ServiceContracts.Enums;

namespace Tests
{
    public class PersonServiceTest
    {
        //private readonly variable for Person Service Interface
        private readonly IPersonService _personService;

        public PersonServiceTest() 
        {
            _personService = new PersonService();
        }

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
            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>
            { new PersonAddRequest(){   PersonName = "Test1",
                Email = "Test1@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test1Address",
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true },
                new PersonAddRequest() {  PersonName = "Test2",
                Email = "Test2@example.com",
                DateOfBirth = DateTime.Parse("2000-10-10"),
                Address = "Test2Address",
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false}
            };

            List<PersonResponse> expectedPersonResponses = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest personAddRequest in personAddRequests)
            {
               expectedPersonResponses.Add(_personService.AddPerson(personAddRequest));
            }

            List<PersonResponse> actualPersonResponseList = _personService.GetAllPersonsList();

            //Assert
            foreach(PersonResponse expectedResponse in expectedPersonResponses)
            {
                Assert.Contains(expectedResponse, actualPersonResponseList);
            }
        }
        #endregion

    }
}
