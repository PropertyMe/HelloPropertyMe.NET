using Microsoft.AspNetCore.Mvc;

namespace PropertyMeOAuthDemo.Controllers;

public class ApiExamplesController: Controller
{
    public IActionResult Index()
    {
        return View();
    }
}