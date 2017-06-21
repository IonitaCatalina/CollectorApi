using CollectorsApi.Models;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    [RoutePrefix("api/photos")]
    public class PhotosController : ApiController
    {
        //connect to db
        public PatternsContext db = new PatternsContext();

        [Route("~/api/user/{id:guid}/photos")]
        public IEnumerable<Photo> Get(string id)
        {

            var test = Main.GetTestScore(Image.FromFile(HttpContext.Current.Server.MapPath("~/omrtemp/testImage.jpg")));

            var photos = db.Photos.Include("Student").Where(x => x.StudentId == id) as IEnumerable<Photo>;

            foreach (var photo in photos)
            {
                photo.Pattern = db.Patterns.FirstOrDefault(x => x.Id == photo.PatternId);
                photo.Pattern.TestPhotos = null;
                photo.Pattern.Image = null;
                photo.Image = null;
            }

            return photos;
        }

        [Route("~/api/user/{id:int}/photo")]
        public Photo GetPhoto(int id)
        {
            return db.Photos.FirstOrDefault(x => x.Id == id);

        }

        public Photo Get(int id)
        {
            return db.Photos.FirstOrDefault(x => x.Id == id);               
        }

        public IHttpActionResult Post([FromBody]Photo photo)
        {
            if (ModelState.IsValid)
            {
                photo.Grade = 0; // add grade
                //photo.StudentId = "70eb4029-587a-4069-a68a-15b367032a15";
                //photo.PatternId = 2;
                db.Photos.Add(photo);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }
    }
}