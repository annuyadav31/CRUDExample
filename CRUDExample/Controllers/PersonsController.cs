using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;
using ServiceContracts.Interfaces;
using ServiceContracts.ModelDTO;
using Services;

namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {
        //private readOnly field
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        [Route("/")]
        [Route("persons/index")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            #region "Searching"
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.DateOfBirth),"Date of Birth" },
                { nameof(PersonResponse.Gender),"Gender" },
                { nameof(PersonResponse.Country),"Country Name" },
                { nameof(PersonResponse.Address),"Address" },
            };

            List<PersonResponse> list = _personService.GetFilteredList(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            #endregion

            #region "Sorting"
            List<PersonResponse> list2=_personService.GetSortedPersons(list,sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder;
            #endregion

            return View(list2); //Views/Persons/Index.html
        }
    }
}
