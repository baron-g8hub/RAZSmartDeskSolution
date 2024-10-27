using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RAZSmartDesk.WebUI.Models;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.WebUI.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAppUserRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthApiController(IConfiguration configuration, IAppUserRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public static Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();


        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {

            // Check user credentials (in a real application, you'd authenticate against a database)
            var appUser = await _repository.FindByUsernamePasswordAsync(model.Username, model.Password);
            if (appUser != null)
            {
                if (model.Username == appUser.Username || model.Password == appUser.Password)
                {
                    var token = GenerateAccessToken(model.Username);
                    // Generate refresh token
                    var refreshToken = Guid.NewGuid().ToString();

                    // Store the refresh token (in-memory for simplicity)
                    RefreshTokens[refreshToken] = model.Username;

                    return Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), RefreshToken = refreshToken });
                }
            }

          

            //if (model is { Username: "demo", Password: "password" })
            //{
            //    var token = GenerateAccessToken(model.Username);
            //    // Generate refresh token
            //    var refreshToken = Guid.NewGuid().ToString();

            //    // Store the refresh token (in-memory for simplicity)
            //    RefreshTokens[refreshToken] = model.Username;

            //    return Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), RefreshToken = refreshToken });
            //}
            return Unauthorized("Invalid credentials");
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            if (RefreshTokens.TryGetValue(request.RefreshToken, out var userId))
            {
                // Generate a new access token
                var token = GenerateAccessToken(userId);

                // Return the new access token to the client
                return Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest("Invalid refresh token");
        }

        private JwtSecurityToken GenerateAccessToken(string userName)
        {
            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                // Add additional claims as needed (e.g., roles, etc.)
            };

            // Create a JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1), // Token expiration time
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

    }
}
