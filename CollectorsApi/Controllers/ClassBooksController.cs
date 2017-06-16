using CollectorsApi.Models;
using CollectorsApi.Models.BindingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    [RoutePrefix("api/ClassBooks")]
    public class ClassBooksController : ApiController
    {
        public PatternsContext db = new PatternsContext();

        //get based on created by
        public IEnumerable<ClassBook> Get(string id)
        {
            return db.ClassBooks.Include("Students").Where(x=>x.TeacherId == id.Replace("\"", "")).AsEnumerable();
        }

        //create
        [HttpPost]
        public IHttpActionResult Create([FromBody]ClassBook classBook)
        {
            if (ModelState.IsValid)
            {
                classBook.Id = Guid.NewGuid().ToString();
                var teacher = db.Users.FirstOrDefault(x => x.Id == classBook.TeacherId.Replace("\"", ""));
                db.ClassBooks.Add(classBook);
                classBook.Teacher = teacher;

                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The catalogue already exists in the database.");
        }

        //add student to classbook
        [Route("addStudent")]
        public IHttpActionResult AddStudentToClassBook([FromBody]ClassBookBindingModel binding)
        {
            var student = db.Users.FirstOrDefault(x => x.Id == binding.StudentId);
            var classBook = db.ClassBooks.Include("Students").FirstOrDefault(x => x.Id == binding.ClassBookId);

            if (student != null && classBook != null)
            {
                classBook.Students.Add(student);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest();
        }
    }
}