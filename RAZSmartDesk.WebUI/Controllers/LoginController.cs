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
using Microsoft.AspNetCore.Identity;
using RAZSmartDesk.Models;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

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
                var user = await _repository.FindByUsernamePasswordAsync(model.Username, model.Password);
                var entity = new User();
                if (user != null)
                {
                    entity = user;
                    // AuthApi call to request login
                    var vm = new ApplicationUserModel();

                    var tokenModel = new TokenModel();

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
                        HttpResponseMessage result = await httpClient.PostAsync(url, byteContent);
                        if (result.IsSuccessStatusCode)
                        {
                            response = result.StatusCode.ToString();
                            string apiResponse = await result.Content.ReadAsStringAsync();
                            if (tokenModel != null)
                            {
                                tokenModel = JsonConvert.DeserializeObject<TokenModel>(apiResponse);
                                HttpContext.Session.SetString("JWToken", tokenModel.AccessToken);
                            }
                            return RedirectToAction("Users", "Login", new { id = entity.UserCompanyId, userTypeId = entity.UserTypeId });
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
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    // Redirect to AppUsers Page then call AppUserApi for content request
                    // Pass the AppUserContext object
                    return RedirectToAction(nameof(Users));
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Users(string id, string userTypeId)
        {
            try
            {
                var jwt = HttpContext.Session.GetString("JWToken");
                string accessToken = string.Empty;
                if (!string.IsNullOrEmpty(jwt))
                {
                    accessToken = jwt;
                }

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

                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers/" + id + "/" + userTypeId;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers/" + id + "/" + userTypeId;
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


        //public async Task<IActionResult> Logout()
        //{
        //    try
        //    {
        //        HttpContext.Session.Clear();
        //        await HttpContext.SignOutAsync("ExampleSession");
        //        return RedirectToAction("Index", "Login");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
