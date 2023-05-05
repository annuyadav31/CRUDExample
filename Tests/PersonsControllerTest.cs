using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests
{
    public class PersonsControllerTest
    {

        #region "Private ReadOnly Fields"
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;
        #endregion

        #region "Constructor"
        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personServiceMock = new Mock<IPersonService>();

            _countriesService=_countriesServiceMock.Object;
            _personService = _personServiceMock.Object;
        }
        #endregion

        #region "Index"
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personService, _countriesService);

            _personServiceMock.Setup(temp => temp.GetFilteredList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons_response_list);
            _personServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult=Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }
        #endregion
    }
}
