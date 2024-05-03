using AngularAuthApi.context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AngularAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public static IWebHostEnvironment _environment;
        public UserController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _environment = environment;
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
        [HttpPost("PostSingleFile")]
        public async Task PostFileAsync(IFormFile fileData)
        {
            try
            {
                var fileDetails = new FileDetails()
                {
                    ID = 0,
                    FileName = fileData.FileName,
                    FileType = fileData.GetType().ToString(),
                };
                using (var stream = new MemoryStream())
                {
                    fileData.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }
                var result = _appDbContext.FileDetails.Add(fileDetails);
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("FileUpload"), DisableRequestSizeLimit]
        public async Task<string> UploadProfilePicture([FromForm(Name = "uploadedFile")] IFormFile file)
        {

            var folderName = Path.Combine("Resources", "ProfilePics");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var uniqueFileName = $"file_profilepic.png";
            var dbPath = Path.Combine(folderName, uniqueFileName);

            using (var fileStream = new FileStream(Path.Combine(filePath, uniqueFileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return dbPath;
        }



        [HttpPost("file")]
        public Task<FileUploadAPI> Post([FromForm] FileUploadAPI objFile)
        {
            FileUploadAPI obj = new FileUploadAPI();
            try
            {
                obj.ImgID = objFile.ImgID;
                obj.ImgName = "\\Upload\\" + objFile.files.FileName;
                string uniqueFileName = UploadedFile(objFile);
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream filestream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(filestream);
                        filestream.Flush();
                        //  return "\\Upload\\" + objFile.files.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Task.FromResult(obj);
        }

        private string UploadedFile(FileUploadAPI model)
        {
            string uniqueFileName = null;

            if (model.files != null)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.files.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.files.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return uniqueFileName;
        }
    }
}
