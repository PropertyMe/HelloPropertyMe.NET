using Microsoft.AspNetCore.Mvc;

namespace PM.Api.Host.Controllers
{
    public class APIExamplesController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
