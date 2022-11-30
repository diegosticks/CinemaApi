using AuthenticationPlugin;
using CinemaApi.Data;
using CinemaApi.Models;
using CinemaApi.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public UsersController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if(_db.Users.FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "User Already Exist");
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                return BadRequest(user);
            }

            if (user.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            User model = new()
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "Users"
                
            };
            _db.Users.Add(model);
            _db.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public IActionResult Login([FromBody]UserDto userDto)
        {
            var userEmail = _db.Users.FirstOrDefault(u => u.Email.ToLower() == userDto.Email.ToLower());
            if (userEmail == null)
            {
                return NotFound("User does not exist");
            }

            if (!SecurePasswordHasherHelper.Verify(userDto.Password, userEmail.Password))
            {
                return Unauthorized("Wrong Password");
            }

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
               new Claim(ClaimTypes.Email, userDto.Email),
               new Claim(ClaimTypes.Role, userEmail.Role)
             };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_Id = userEmail.Id
            });
        }
    }
}
