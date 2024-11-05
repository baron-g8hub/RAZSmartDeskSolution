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
using Microsoft.AspNetCore.RateLimiting;
using System.Linq.Expressions;

namespace RAZSmartDesk.WebUI.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
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


        [HttpGet]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<ActionResult<User>> Get()
        {
            var appUserId = GetTokenClaims();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUserEntity = await _usersRepository.FindByUserIdAsync(int.Parse(appUserId));
                if (appUserEntity == null)
                {
                    return NotFound("Application User not found. Return to login");
                }
                var entity = await _usersRepository.GetAppUsersByCompanyIdAsync(appUserEntity.UserCompanyId, appUserEntity.UserTypeId);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpGet("{id}")]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var appUserId = GetTokenClaims();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUserEntity = await _usersRepository.FindByUserIdAsync(int.Parse(appUserId));
                if (appUserEntity == null)
                {
                    return NotFound("Application User not found. Return to login.");
                }
                var entity = await _usersRepository.FindByUserIdCompanyIdAsync(id, appUserEntity.UserCompanyId);
                if (entity == null)
                {
                    return NotFound("User not found.");
                }
                switch (appUserEntity.UserTypeId)
                {
                    case 1:
                        return Ok(entity);
                        break;
                    case 2:
                        if (entity.UserTypeId == appUserEntity.UserTypeId)
                        {
                            return Ok(entity);
                        }
                        break;
                    default:
                        break;
                }
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Add([FromBody] User entity)
        {
            try
            {
                var appUserId = GetTokenClaims();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUserEntity = await _usersRepository.FindByUserIdAsync(int.Parse(appUserId));
                if (appUserEntity == null)
                {
                    return NotFound("Application User not found. Return to login.");
                }

                if (appUserEntity.UserTypeId == 1)
                {
                    var list = await _usersRepository.GetAppUsersByCompanyIdAsync(appUserEntity.UserCompanyId, 1);
                    if (list.Count() > 0)
                    {
                        var isUserExist = list.Any(x => x.Username == entity.Username);
                        if (isUserExist)
                        {
                            return BadRequest(entity.Username +  " username already exist.");
                        }
                    }

                    entity.UserCompanyId = appUserEntity.UserCompanyId;
                    entity.CreatedBy = appUserEntity.Username;
                    entity.UpdatedBy = appUserEntity.Username;
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.UpdatedDate = entity.UpdatedDate;
                    var result = await _usersRepository.AddAsync(entity);
                    if (result != null)
                    {
                        var response = entity.Username + " user created successfully.";
                        return Ok(response);
                    }
                    return BadRequest(result);
                }
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Update([FromBody] User entity)
        {
            try
            {
                var appUserId = GetTokenClaims();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUserEntity = await _usersRepository.FindByUserIdAsync(int.Parse(appUserId));
                if (appUserEntity == null)
                {
                    return NotFound("Application User not found. Return to login.");
                }

                if (appUserEntity.UserTypeId == 1)
                {
                    entity.UpdatedBy = appUserEntity.Username;
                    entity.UpdatedDate = DateTime.UtcNow;
                    var result = await _usersRepository.UpdateAsync(entity);
                    if (result != null)
                    {
                        var response = entity.Username + " user updated successfully.";
                        return Ok(response);
                    }
                    return BadRequest(result);
                }
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(400, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                var appUserId = GetTokenClaims();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUserEntity = await _usersRepository.FindByUserIdAsync(int.Parse(appUserId));
                if (appUserEntity == null)
                {
                    return NotFound("Application User not found. Return to login.");
                }
                var entity = await _usersRepository.FindByUserIdCompanyIdAsync(id, appUserEntity.UserCompanyId);
                if (entity == null)
                {
                    return NotFound("User not found.");
                }
                switch (appUserEntity.UserTypeId)
                {
                    case 1:
                        if (entity.UserTypeId == appUserEntity.UserTypeId && entity.UserCompanyId == appUserEntity.UserCompanyId)
                        {
                            var result = await _usersRepository.RemoveAsync(entity);
                            if (result != null)
                            {
                                var response = entity.Username + " user deleted successfully.";
                                return Ok(response);
                            }
                            return BadRequest(result);
                        }
                        break;
                    case 2:
                        NotFound("Flat Users not allowed to modify other users.");
                        break;
                    default:
                        break;
                }

                return NotFound("User not found.");
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

        private string GetTokenClaims()
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            //var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var appUserId = tokenS.Claims.First(claim => claim.Type == "Username").Value;

            return appUserId;
        }
        #endregion
    }
}
