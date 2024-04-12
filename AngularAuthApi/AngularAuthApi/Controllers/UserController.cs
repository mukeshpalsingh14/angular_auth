using AngularAuthApi.context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public UserController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userobj)
        {
            if (userobj == null)
                return BadRequest();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == userobj.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }
            return Ok(new
            {
                Message = "login Success"
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {

            try
            {
                if (userObj == null)
                    return BadRequest();
                //User exist check
                if (await checkUserExist(userObj.Email))
                    return BadRequest(new { Message = "User Already exist." });

                if (string.IsNullOrEmpty(userObj.Email)) return NotFound(new { Message = "Mail have required" });
                await _appDbContext.Users.AddAsync(userObj);
                await _appDbContext.SaveChangesAsync();
                return Ok(new
                {
                    Message = "User Registered"
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet("login")]
        public async Task<IActionResult> Authenticate(string userName)
        {
            if (userName == null)
                return BadRequest();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == userName);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }
            return Ok(new
            {
                Message = "login Success"
            });
        }
        [HttpGet("getUser")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                var user = await _appDbContext.Users.ToListAsync();
                if (user == null)
                {
                    return NotFound(new { Message = "User Not Found!" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private async Task<bool> checkUserExist(string username)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Email == username);
        }
    }
}
