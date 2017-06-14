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
        private readonly string _serviceUrl = "http://localhost:54098/";

        // GET: ClassBook
        public ActionResult Index()
        {
            return View();
        }

        //get all catalogues of students owned by teacher
        public async Task<IEnumerable<ClassBook>> GetClassBooks(string id)
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format(_serviceUrl + "{0}/{1}", "/api/classBooks", id)));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<ClassBook>>(content);
                return items;
            }

            return null;
        }

        public async Task<HttpResponseMessage> CreateClassBook(ClassBook classBook)
        {
            var content = new StringContent(JsonConvert.SerializeObject(classBook), Encoding.UTF8, "application/json");
            var result = await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}/{1}", "/api/classBooks/create", content)), null);
            return result;
        }
    }
}
