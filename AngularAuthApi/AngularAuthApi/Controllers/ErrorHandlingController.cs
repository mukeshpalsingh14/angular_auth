using AngularAuthApi.context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularAuthApi.Controllers
{
    public class ErrorHandlingController : Controller
    {
        private readonly AppDbContext _dataContext;
        public ErrorHandlingController(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> getSecret()
        {
            return "secret test";
        }
        [HttpGet("not-found")]
        public ActionResult<User> getNotFound()
        {
            var thing = _dataContext.Users.Find(-1);
            if (thing == null) return NotFound();
            return thing;
        }
        [HttpGet("server-error")]
        public ActionResult<string> getServerError()
        {
            var thing = _dataContext.Users.Find(-1);
            var thingToRetun = thing.ToString();
            return thingToRetun;

        }
        [HttpGet("bad-request")]
        public ActionResult<string> getBadRequest()
        {
            return BadRequest("This was not a good request");

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
