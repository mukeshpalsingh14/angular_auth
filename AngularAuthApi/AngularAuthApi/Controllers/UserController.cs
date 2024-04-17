using AngularAuthApi.context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            user.Token = CreateJWT(user);
            return Ok(new
            {
                token= user.Token,
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
        [Authorize]
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
        private string CreateJWT(User user)
        {
            try
            {
                var JwtToken = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("mukeshmehay apne yaar dost ha kine ka badi key chaiadyi ha");
                var identity = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Name,$"{user.FirstName}:{user.LastName}"),
                });
                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials,
                };
                var token = JwtToken.CreateToken(tokenDescriptor);
                return JwtToken.WriteToken(token);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
