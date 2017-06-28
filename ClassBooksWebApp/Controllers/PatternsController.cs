using ClassBooksWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ClassBooksWebApp.Controllers
{
    public class PatternsController : Controller
    {
        readonly HttpClient _apiClient = new HttpClient();
        private readonly string _serviceUrl = WebConfigurationManager.AppSettings["apiUrl"];

        // GET: Patterns
        public ActionResult Index()
        {
            if (Session["UserId"] == null) return View("~/Views/Login/Login.cshtml");
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetPatterns()
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format(_serviceUrl + "/api/{0}/patterns", new Guid(Session["UserId"].ToString().Replace("\"", "")))));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Pattern>>(content);

                foreach (var item in items)
                {
                    item.Image = null;
                }
                return Json(items, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public async Task<JsonResult> GetPatternImage(int id)
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format(_serviceUrl + "/api/patterns/pattern/{0}", id)));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<Pattern>(content);

                using (var ms = new MemoryStream(item.Image))
                {
                    var bmp = new Bitmap(ms);

                    var scaled = new Bitmap(bmp, new Size(578, 839));
                    item.Image = ToByteArray(scaled, ImageFormat.Jpeg);

                    return Json(item, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        // get answer sheet by pattern id
        [HttpGet]
        public async Task<JsonResult> GetAnswerSheet(int id)
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format(_serviceUrl + "/api/pattern/{0}/answerSheet", id)));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<AnswerSheet>>(content);
                return Json(items, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public async Task<HttpStatusCodeResult> AddAnswerSheet(List<AnswerSheet> answerSheet)
        {
            var content = new StringContent(JsonConvert.SerializeObject(answerSheet), Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}", "/api/patterns/AddSheet")), content);

            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }



        public async Task<HttpStatusCodeResult> AddPattern(Pattern pattern)
        {
            pattern.TeacherId = Session["UserId"].ToString().Replace("\"", "");

            var content = new StringContent(JsonConvert.SerializeObject(pattern), Encoding.UTF8, "application/json");
            var result = await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}", "/api/patterns/addPattern")), content);

            return new HttpStatusCodeResult(HttpStatusCode.Created);

        }

        public async Task<HttpStatusCodeResult> RemovePattern(int id)
        {
            await _apiClient.PostAsync(new Uri(string.Format(_serviceUrl + "{0}/{1}", "/api/DeletePattern", id)), null);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public byte[] ToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}