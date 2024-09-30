using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using lastSETIONDEPI.Models.Data;
using lastSETIONDEPI.ViewModel;
namespace lastSETIONDEPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<SchoolUser> _userManager;
        private readonly SignInManager<SchoolUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<SchoolUser> userManager, SignInManager<SchoolUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("registerteacher")]
        public async Task<IActionResult> registerteacher([FromBody] RegisterModel model)
        {
            var user = new SchoolUser
            {
                UserName = model.Email,
                Email = model.Email,
                SchoolName = model.SchoolName,
                PerformanceRate = model.PerformanceRate
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Teacher");

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Teacher")
            };
                await _userManager.AddClaimsAsync(user, claims);

                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("registerstudent")]
        public async Task<IActionResult> registerstudent([FromBody] RegisterModel model)
        {
            var user = new SchoolUser
            {
                UserName = model.Email,
                Email = model.Email,
                SchoolName = model.SchoolName,
                PerformanceRate = model.PerformanceRate
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Student")
            };
                await _userManager.AddClaimsAsync(user, claims);

                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();  
            }

            var claims = await _userManager.GetClaimsAsync(user);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            var keyString = _configuration.GetValue<string>("SecretKey");
            var keyBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyBytes);

            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var jwt = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredential,
                expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("TokenDuration")),
                notBefore: DateTime.Now
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new
            {
                Token = tokenString,
                Expire = DateTime.Now.AddMinutes(_configuration.GetValue<int>("TokenDuration"))
            });
        }

    }

}
