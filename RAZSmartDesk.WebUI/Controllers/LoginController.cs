using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;
using RAZSmartDesk.WebUI.Models;
using System.Net.Http.Headers;

namespace RAZSmartDesk.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAppUserRepository _repository;

        public static Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();

        public LoginController(IAppUserRepository repository)
        {
            _repository = repository;
        }


        public IActionResult Index()
        {
            // Check for existing token then assign to values to AppUserContext
            var vm = new LoginModel();



            // var appUser = _repository.FindByUsernamePasswordAsync(model.Username, model.Password);
            vm.Password = "";
            vm.Username = "";

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {
            try
            {
                var user = _repository.FindByUsernamePasswordAsync(model.Username, model.Password);
                // AuthApi call to request login
                var vm = new LoginModel();
                var userContext = new User();

                var myContent = JsonConvert.SerializeObject(model);
                var response = string.Empty;
                using (var httpClient = new HttpClient())
                {
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var url = "http://" + HttpContext.Request.Host.Value;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/AuthApi/login";
                    }
                    HttpResponseMessage result = await httpClient.PostAsync(url , byteContent);
                    if (result.IsSuccessStatusCode)
                    {
                        response = result.StatusCode.ToString();
                    }
                    else
                    {
                        string apiResponse = await result.Content.ReadAsStringAsync();
                        // Parse response to AppUserContext

                        //if (apiResponse.ToLower().Contains("insufficient"))
                        //{
                        //    ModelState.ClearValidationState("Amount");
                        //    ModelState.AddModelError("Amount", apiResponse);
                        //}
                        //else if (apiResponse.ToLower().Contains("source"))
                        //{
                        //    ModelState.ClearValidationState("SourceAccount");
                        //    ModelState.AddModelError("SourceAccount", apiResponse);
                        //}
                        //else if (apiResponse.ToLower().Contains("destination"))
                        //{
                        //    ModelState.ClearValidationState("DestinationAccount");
                        //    ModelState.AddModelError("DestinationAccount", apiResponse);
                        //}
                        return View(vm);
                    }
                } 
                // Redirect to AppUsers Page then call AppUserApi for content request
                // Pass the AppUserContext object
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View();
        }
    }
}
