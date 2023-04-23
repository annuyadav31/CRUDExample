using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
           List<PersonResponse> list= _personService.GetAllPersonsList();
            return View(list); //Views/Persons/Index.html
        }
    }
}
