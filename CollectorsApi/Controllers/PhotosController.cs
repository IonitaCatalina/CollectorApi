using CollectorsApi.Helpers;
using CollectorsApi.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            var photo = db.Photos.FirstOrDefault(x => x.Id == id);

            using (var ms = new MemoryStream(photo.Image))
            {
                var bmp = new Bitmap(ms);

                var scaled = new Bitmap(bmp, new Size(bmp.Width/2, bmp.Height/2));
                photo.Image = PreprocessingHelper.ToByteArray(scaled, ImageFormat.Jpeg);

                return photo;
            }

        }

        public Photo Get(int id)
        {
            return db.Photos.FirstOrDefault(x => x.Id == id);               
        }

        public IHttpActionResult Post([FromBody]Photo photo)
        {
            var inst = new PatternsController();
            var pattern = inst.GetPattern(photo.PatternId);

            if (ModelState.IsValid)
            {
                var answers = db.AnswerSheets.Where(x => x.PatternId == photo.PatternId && string.IsNullOrEmpty(x.StudentId)).ToList();

                photo.Grade = Main.GetTestScore(pattern, photo, answers);
                db.Photos.Add(photo);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }
    }
}