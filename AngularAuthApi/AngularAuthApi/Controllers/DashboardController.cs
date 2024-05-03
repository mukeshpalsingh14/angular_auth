using AngularAuthApi.context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public DashboardController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _appDbContext.Users.ToListAsync();
            //var UsersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(users);

        }

        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetUsers(int Id)
        {
            var user = await _appDbContext.Users.FindAsync(Id);
            return user;
        }
    }
}
