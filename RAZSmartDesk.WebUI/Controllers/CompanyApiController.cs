using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.WebUI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyRepository _repository;
        public CompanyApiController(ICompanyRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> Get()
        {
            var list = await _repository.GetAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Company>> Get(int? id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entity = await _repository.FindAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return this.StatusCode(400, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Company model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (ModelState.IsValid)
                {
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


        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
