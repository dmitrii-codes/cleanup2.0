using Microsoft.AspNetCore.Mvc;

namespace Cleanup
{
    public class ErrorController : Controller
    {
        public IActionResult Default()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}