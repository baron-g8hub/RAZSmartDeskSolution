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
    public class UserController : Controller
    {
        private readonly IAppUserRepository _repository;

        public static Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();

        public UserController(IAppUserRepository repository)
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
                            //return RedirectToAction("Users", "User");
                            return RedirectToAction(nameof(Users));
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
        public async Task<IActionResult> Users()
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
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/";
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/";
                    }
                    using (var response = await httpClient.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            vm.Entity = JsonConvert.DeserializeObject<User>(apiResponse);
                            vm.CompanyId = vm.Entity.UserCompanyId;
                            vm.CompanyName = vm.Entity.CompanyName;
                            vm.UserTypeName = vm.UserTypeName;
                            vm.ApplicationUserId = vm.Entity.UserId;
                        }
                        else
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers/" + vm.ApplicationUserId;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUsers/" + vm.ApplicationUserId;
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


        public async Task<ActionResult> Create()
        {
            var id = RouteData.Values["id"];
            var vm = new UsersViewModel();
            ViewBag.accountTypes = vm.LoadUserTypes();
            var jwt = HttpContext.Session.GetString("JWToken");
            string accessToken = string.Empty;
            if (!string.IsNullOrEmpty(jwt))
            {
                accessToken = jwt;
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/";
                if (Request.Host.Host == "localhost")
                {
                    url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/";
                }
                using (var response = await httpClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        vm.Entity = JsonConvert.DeserializeObject<User>(apiResponse);
                        vm.CompanyId = vm.Entity.UserCompanyId;
                        vm.CompanyName = vm.Entity.CompanyName;
                        vm.UserTypeName = vm.Entity.CompanyName;
                        vm.ApplicationUserId = vm.Entity.UserId;
                        vm.CreatedBy = vm.Entity.Username;
                        vm.Entity.UserId = 0;
                        return View(vm);
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UsersViewModel model)
        {
            try
            {
                try
                {
                    var entity = new User();
                    entity.UserCompanyId = model.ApplicationUserId;
                    entity.Username = model.Username;
                    entity.Password = model.Password;
                    entity.CreatedBy = model.CreatedBy;
                    entity.CreatedDate = model.CreatedDate;
                    entity.UpdatedBy = model.UpdatedBy;
                    entity.UpdatedDate = model.UpdatedDate;
                    entity.IsActive = model.IsActive;
                    entity.UserTypeId = model.UserTypeId;
                    var response = string.Empty;
                    using (var httpClient = new HttpClient())
                    {
                        var url = "http://" + HttpContext.Request.Host.Value;
                        if (Request.Host.Host == "localhost")
                        {
                            url = "https://" + HttpContext.Request.Host.Value;
                        }
                        if (entity.UserId != 0)
                        {
                            entity.UserId = model.ApplicationUserId;
                            entity.UserCompanyId = model.CompanyId;
                            url += "/UsersApi/Update";
                        }
                        else
                        {
                            entity.UpdatedBy = model.CreatedBy;
                            entity.UpdatedDate = DateTime.Now;
                            url += "/UsersApi/Add";
                        }
                        var myContent = JsonConvert.SerializeObject(entity);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        HttpResponseMessage result = await httpClient.PostAsync(url, byteContent);
                        if (result.IsSuccessStatusCode)
                        {
                            response = result.StatusCode.ToString();
                            return RedirectToAction("Users", "User", new { id = entity.UserId });
                            // return RedirectToAction(nameof(Users));
                        }
                        else
                        {
                            string apiResponse = await result.Content.ReadAsStringAsync();
                            if (apiResponse.ToLower().Contains("duplicate"))
                            {
                                ModelState.ClearValidationState("AccountName");
                                ModelState.AddModelError("AccountName", apiResponse);
                            }
                            //ViewBag.accountTypes = model.LoadAccountTypes();
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

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
