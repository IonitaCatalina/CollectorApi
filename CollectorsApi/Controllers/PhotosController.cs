using CollectorsApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    public class PhotosController : ApiController
    {
        //connect to db
        public PatternsContext db = new PatternsContext();

        public IEnumerable<Photo> Get()
        {
            return db.Photos as IEnumerable<Photo>;
        }

        public Photo Get(int id)
        {
            return db.Photos.FirstOrDefault(x => x.Id == id);               
        }

        public IHttpActionResult Post([FromBody]Photo photo)
        {
            if (ModelState.IsValid && !db.Photos.Any(x => x.Id == photo.Id))
            {
                db.Photos.Add(photo);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }
    }
}