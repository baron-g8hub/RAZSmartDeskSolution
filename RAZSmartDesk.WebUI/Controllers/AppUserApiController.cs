using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.WebUI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AppUserApiController : ControllerBase
    {
        private readonly IAppUserRepository _repository;
        private readonly ICompanyRepository _companyRepository;

        public AppUserApiController(IAppUserRepository repository, ICompanyRepository companyRepository)
        {
            _repository = repository;
            _companyRepository = companyRepository;
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entity = await _repository.FindByAppUserIdAsync(id);
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


        [HttpGet("{companyId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(int companyId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entity = await _repository.GetAppUsersByCompanyIdAsync(companyId);
                if (entity == null)
                {
                    return NotFound("Users not found.");
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
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



                    await _repository.AddAsync(model);
                    return Ok();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
