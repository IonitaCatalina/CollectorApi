using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClassBooksWebApp.Models;
using Newtonsoft.Json;
using System.Web.Configuration;

namespace ClassBooksWebApp.Controllers
{
    public class LoginController : Controller
    {
        HttpClient apiClient = new HttpClient();
        private string ServiceUrl = WebConfigurationManager.AppSettings["apiUrl"];

        public ActionResult Login()
        {
            return View();
        }

        public async Task<ActionResult> LoginUser(User user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await apiClient.PostAsync(new Uri(string.Format(ServiceUrl + "{0}", "/api/users/GetUserByEmail")), content);

            if (response.IsSuccessStatusCode)
            {
                Session["UserId"] = response.Content.ReadAsStringAsync().Result;
                return Content(Session["UserId"].ToString());
            }

            return View("Login", "Login");
        }

        public async Task<User> GetUserById(string id)
        {
            var response = await apiClient.GetAsync(new Uri(string.Format(ServiceUrl + "{0}", string.Format("/api/users/{0}", id))));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                return user;
            }

            return null;
        }

        public async Task<ActionResult> RegisterUser(User user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await apiClient.PostAsync(new Uri(string.Format(ServiceUrl + "{0}", "/api/users/create")), content);

            if (response.IsSuccessStatusCode)
            {
                return Content("User created succesfully!");
            }

            return Content("Error on creating user.");
        }

        public async Task<JsonResult> GetUsersBasedOnRole(string Id)
        {
            var response = await apiClient.GetAsync(new Uri(string.Format(ServiceUrl + "{0}", string.Format("/api/users/{0}", Id))));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<User>>(content);
                return Json(items, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult GetSessionId()
        {
            if (!string.IsNullOrEmpty(Session["UserId"].ToString())) return Content(Session["UserId"].ToString());
            return HttpNotFound();
        }
    }
}