using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client=factory.CreateClient();
        }

        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange
            //Act
            HttpResponseMessage response=await _client.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful();
        }


    }
}
