﻿using ClassBooksWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ClassBooksWebApp.Controllers
{
    public class StudentsController : Controller
    {
        readonly HttpClient _apiClient = new HttpClient();
        private readonly string _serviceUrl = WebConfigurationManager.AppSettings["apiUrl"];

        public ActionResult Index()
        { 
            if (Session["UserId"] == null) return View("~/Views/Login/Login.cshtml");
            
            return View();
        }

        public async Task<JsonResult> GetUserPhotos(User student)
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format("{0}/api/user/{1}/photos",_serviceUrl, student.Id)));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<List<Photo>>(content);

                return Json(deserializedContent, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public async Task<JsonResult> GetPhoto(int id)
        {
            var response = await _apiClient.GetAsync(new Uri(string.Format("{0}/api/user/{1}/photo", _serviceUrl, id)));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<Photo>(content);

                using (Image image = Image.FromStream(new MemoryStream(deserializedContent.Image)))
                {
                    image.Save(@"C:\Users\Chupachups\Desktop\output.jpg", ImageFormat.Jpeg);
                }

                return Json(deserializedContent, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}