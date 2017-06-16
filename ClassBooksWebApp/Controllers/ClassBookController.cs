using ClassBooksWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ClassBooksWebApp.Controllers
{
    public class ClassBookController : Controller
    {
        readonly HttpClient _apiClient = new HttpClient();
        private readonly string _serviceUrl = "http://localhost:54098";

        // GET: ClassBook
        public ActionResult Index()
        {
            if (Session["UserId"] == null) return View("~/Views/Login/Login.cshtml");
            return View();
        }

        //get all catalogues of students owned by teacher
        [HttpGet]
        public async Task<JsonResult> GetClassBooks()
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format(_serviceUrl + "{0}/{1}", "/api/classBooks", Session["UserId"].ToString())));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<ClassBook>>(content);
                return Json(items, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public async Task<HttpResponseMessage> CreateClassBook(ClassBook classBook)
        {
            classBook.TeacherId = Session["UserId"].ToString();

            var content = new StringContent(JsonConvert.SerializeObject(classBook), Encoding.UTF8, "application/json");
            var result = await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}", "/api/classBooks/create")), content);
            return result;
        }

        public async Task<HttpResponseMessage> AddStudentToClassBook(string id)
        {
            var content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
            var result = await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}", "/api/classBooks/addStudent")), content);

            return result;
        }
        
    }
}
