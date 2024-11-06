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
using RAZSmartDesk.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RAZSmartDesk.WebUI.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {

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
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

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
                        return RedirectToAction(nameof(Users));
                    }
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
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "Username").Value;

                var vm = new UsersViewModel();
                var list = new List<User>();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/Get/" + id;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/Get/" + id;
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
                // *** start API Call to get all Users in the Company of the Application User
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/Get";
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/Get";
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
                // *** end API Call to get all Users in the Company of the Application User
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
            var appUserEntity = new User();
            var jwt = HttpContext.Session.GetString("JWToken");
            string accessToken = string.Empty;
            if (!string.IsNullOrEmpty(jwt))
            {
                accessToken = jwt;
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
            var appUserId = tokenS.Claims.First(claim => claim.Type == "Username").Value;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/" + appUserId;
                if (Request.Host.Host == "localhost")
                {
                    url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/" + appUserId;
                }
                using (var response = await httpClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        appUserEntity = JsonConvert.DeserializeObject<User>(apiResponse);
                        vm.ApplicationUserId = appUserEntity.UserId;
                        vm.CompanyId = appUserEntity.UserCompanyId;
                        vm.CompanyName = appUserEntity.CompanyName;
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            if (id == null)
            {
                vm.CreatedBy = appUserEntity.Username;
                vm.UpdatedBy = appUserEntity.Username;
                vm.UserViewModelId = 0;
            }
            else
            {
                var userViewModelEntity = new User();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/" + id;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/GetUser/" + id;
                    }
                    using (var response = await httpClient.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            userViewModelEntity = JsonConvert.DeserializeObject<User>(apiResponse);
                            vm.CompanyId = userViewModelEntity.UserCompanyId;
                            vm.CompanyName = userViewModelEntity.CompanyName;
                            vm.CreatedBy = userViewModelEntity.Username;
                            vm.UpdatedBy = userViewModelEntity.Username;
                            vm.UserViewModelId = userViewModelEntity.UserId;
                            vm.UserTypeId = userViewModelEntity.UserTypeId;
                            foreach (var item in vm.SelectListUserTypes)
                            {
                                item.Selected = false;
                                if (item.Value == userViewModelEntity.UserTypeId.ToString())
                                {
                                    item.Selected = true;
                                }
                            }
                            // Username,Password,IsActive,UserType
                            vm.Username = userViewModelEntity.Username;
                            vm.Password = userViewModelEntity.Password;
                            vm.IsActive = userViewModelEntity.IsActive;
                            vm.UserTypeName = userViewModelEntity.UserTypeName;
                            vm.Entity.UserId = userViewModelEntity.UserId;
                        }
                        else
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
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

                    var jwt = HttpContext.Session.GetString("JWToken");
                    string accessToken = string.Empty;
                    if (!string.IsNullOrEmpty(jwt))
                    {
                        accessToken = jwt;
                    }
                    var response = string.Empty;
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var url = "http://" + HttpContext.Request.Host.Value;
                        if (Request.Host.Host == "localhost")
                        {
                            url = "https://" + HttpContext.Request.Host.Value;
                        }
                        if (model.Entity.UserId != 0)
                        {
                            entity.UserTypeName = model.Entity.UserTypeName;
                            //entity.UserId = model.ApplicationUserId;
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
                            //if (apiResponse.ToLower().Contains("duplicate"))
                            //{
                            //    ModelState.ClearValidationState("AccountName");
                            //    ModelState.AddModelError("AccountName", apiResponse);
                            //}
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



        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var entity = new User();
                var jwt = HttpContext.Session.GetString("JWToken");
                string accessToken = string.Empty;
                if (!string.IsNullOrEmpty(jwt))
                {
                    accessToken = jwt;
                }
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var url = "http://" + HttpContext.Request.Host.Value + "/UsersApi/DeleteById/" + id;
                    if (Request.Host.Host == "localhost")
                    {
                        url = "https://" + HttpContext.Request.Host.Value + "/UsersApi/DeleteById/" + id;
                    }
                    using (var response = await httpClient.DeleteAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            entity = JsonConvert.DeserializeObject<User>(apiResponse);
                        }
                        else
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return RedirectToAction(nameof(Users));
        }



        //public async Task<IActionResult> Logout()
        //{
        //    try
        //    {
        //        HttpContext.Session.Clear();
        //        await HttpContext.SignOutAsync("JWToken");
        //        return RedirectToAction("Index", "Login");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
