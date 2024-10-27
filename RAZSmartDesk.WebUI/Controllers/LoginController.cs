using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;
using RAZSmartDesk.WebUI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using static Dapper.SqlMapper;

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
                var vm = new ApplicationUserModel();
                var entity = new User();

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
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/login";
                    }
                    HttpResponseMessage result = await httpClient.PostAsync(url , byteContent);
                    if (result.IsSuccessStatusCode)
                    {
                        response = result.StatusCode.ToString();
                        string apiResponse = await result.Content.ReadAsStringAsync();

                       

                        entity = JsonConvert.DeserializeObject<User>(apiResponse);
                        vm.ApplicationUserId = entity.UserId;
                        vm.ApplicationUsername = entity.Username;
                        vm.ApplicationUserPassword = entity.Password;
                        vm.ApplicationUserTypeId = entity.UserTypeId;
                        vm.ApplicationUserTypeName = entity.UserTypeName;
                        //vm.ApplicationUserToken = entity.
                        //return CreatedAtAction(nameof(Users(apiResponse)), new { accessToken = apiResponse });

                        return RedirectToAction("Users", "Login", new { accessToken = apiResponse });
                        //return RedirectToAction(nameof(Users));
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
                return RedirectToAction(nameof(Users));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Users(string accessToken)
        {
            try
            {
                var vm = new UsersViewModel();
                var list = new List<User>();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    //httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    //var content = new StringContent(request_json, Encoding.UTF8, "application/json");

                    //var authenticationBytes = Encoding.ASCII.GetBytes(accessToken);
                    //httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                    //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                    //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers";
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers";
                    }
                    using (var response = await httpClient.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            vm.EntityList = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                        }
                        else
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
