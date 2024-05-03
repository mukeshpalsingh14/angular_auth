using Microsoft.AspNetCore.Mvc;

namespace AngularAuthApi.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
