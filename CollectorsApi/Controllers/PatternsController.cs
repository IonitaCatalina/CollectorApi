using CollectorsApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    public class PatternsController : ApiController
    {
        //connect to db
        public PatternsContext db = new PatternsContext();

        public IEnumerable<Pattern> Get()
        {
            return db.Patterns as IEnumerable<Pattern>;
        }

        public Pattern Get(int id)
        {
            return db.Patterns.FirstOrDefault(x=>x.Id == id);
        }

        public IHttpActionResult Post([FromBody]Pattern pattern)
        {
         
            if (ModelState.IsValid && !db.Patterns.Any(x => x.Id == pattern.Id))
            {
                db.Patterns.Add(pattern);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }       
    }
}
