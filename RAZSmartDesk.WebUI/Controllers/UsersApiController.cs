using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;
using RAZSmartDesk.WebUI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Authentication;

namespace RAZSmartDesk.WebUI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAppUserRepository _usersRepository;
        private readonly ICompanyRepository _companyRepository;

        public static Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();

        public UsersApiController(IConfiguration configuration, IAppUserRepository repository, ICompanyRepository companyRepository)
        {
            _configuration = configuration;
            _usersRepository = repository;
            _companyRepository = companyRepository;
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            //var handler = new JwtSecurityTokenHandler();
            //string authHeader = Request.Headers["Authorization"];
            //authHeader = authHeader.Replace("Bearer ", "");
            ////var jsonToken = handler.ReadToken(authHeader);
            //var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            //var userId = tokenS.Claims.First(claim => claim.Type == "Username").Value;
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entity = await _usersRepository.FindByUserIdAsync((id));
                if (entity == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(string userId)
        {
            try
            {
                //var handler = new JwtSecurityTokenHandler();
                //string authHeader = Request.Headers["Authorization"];
                //authHeader = authHeader.Replace("Bearer ", "");
                //var jsonToken = handler.ReadToken(authHeader);
                //var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                //var userId = tokenS.Claims.First(claim => claim.Type == "Username").Value;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _usersRepository.FindByUserIdAsync(int.Parse(userId));
                if (user == null)
                {
                    return NotFound("Users not found.");
                }
                var entity = await _usersRepository.GetAppUsersByCompanyIdAsync(user.UserCompanyId, user.UserTypeId);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] User model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (ModelState.IsValid)
                {
                    // 1. Check if CompanyId is valid.
                    // var employee = await _companyRepository.FindAsync(model.CreatedBy);


                    // 2. Check if Creator user role is valid.



                    await _usersRepository.AddAsync(model);
                    return Ok();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] User entity)
        {
            try
            {
                //var handler = new JwtSecurityTokenHandler();
                //string authHeader = Request.Headers["Authorization"];
                //authHeader = authHeader.Replace("Bearer ", "");
                ////var jsonToken = handler.ReadToken(authHeader);
                //var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                //var userId = tokenS.Claims.First(claim => claim.Type == "Username").Value;

                if (!ModelState.IsValid || (entity.UserTypeName == "" || entity.Username == "string"))
                {
                    return BadRequest(ModelState);
                }
                var result = await _usersRepository.AddAsync(entity);
                if (result != null)
                {
                    //  result = entity.Username + " account created successfully.";
                    //return CreatedAtAction("GetUers", new { userId = entity.UserId });
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] User entity)
        {
            try
            {
                //var handler = new JwtSecurityTokenHandler();
                //string authHeader = Request.Headers["Authorization"];
                //authHeader = authHeader.Replace("Bearer ", "");
                ////var jsonToken = handler.ReadToken(authHeader);
                //var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                //var userId = tokenS.Claims.First(claim => claim.Type == "Username").Value;
             
                var result = await _usersRepository.UpdateAsync(entity);
                if (result != null)
                {
                    //  result = entity.Username + " account created successfully.";
                    //return CreatedAtAction("GetUers", new { userId = entity.UserId });
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(400, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                var entity = new User();
                entity = await _usersRepository.FindByUserIdAsync(id);
                if (entity != null)
                {
                    var result = await _usersRepository.RemoveAsync(entity);
                    if (result != null)
                    {
                        var response = "User deleted successfully.";
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.StatusCode(400, ex.Message);
            }
        }


        #region Authenticate User

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            // Check user credentials (in a real application, you'd authenticate against a database)
            var appUser = await _usersRepository.FindByUsernamePasswordAsync(model.Username, model.Password);
            if (appUser != null)
            {
                if (model.Username == appUser.Username || model.Password == appUser.Password)
                {
                    var token = GenerateAccessToken(appUser.UserId.ToString());
                    // Generate refresh token
                    var refreshToken = Guid.NewGuid().ToString();

                    // Store the refresh token (in-memory for simplicity)
                    RefreshTokens[refreshToken] = model.Username;

                    //var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.ToString());
                    //string userApp = jwt.Claims.First(c => c.Type == "Name").Value;

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

        private JwtSecurityToken GenerateAccessToken(string userId)
        {
            //var permClaims = new List<Claim>();
            //permClaims.Add(new Claim(ClaimTypes.Name, userId));
            //permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //permClaims.Add(new Claim("User", userId));

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Username", userId)
            };

            // Create a JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5), // Token expiration time
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }



        #endregion
    }
}
