using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;

namespace RAZSmartDesk.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyRepository _repository;
        public CompanyApiController(ICompanyRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAccounts()
        {
            var list = await _repository.GetAsync();
            return Ok(list);
        }


    }
}
