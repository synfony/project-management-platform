using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Projects");
}
