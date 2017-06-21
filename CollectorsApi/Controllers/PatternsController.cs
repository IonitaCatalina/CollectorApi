using CollectorsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    [RoutePrefix("api/patterns")]
    public class PatternsController : ApiController
    {
        //connect to db
        public PatternsContext db = new PatternsContext();

        public Pattern Get(int id)
        {
            return db.Patterns.FirstOrDefault(x=>x.Id == id);
        }

        [Route("~/api/{id:guid}/patterns")]
        public IEnumerable<Pattern> GetPatternsByUser(string id)
        {
            return db.Patterns.Where(x => x.TeacherId == id.ToString());
        }

        public IHttpActionResult Post([FromBody]Pattern pattern)
        {
         
            if (ModelState.IsValid)
            {
                db.Patterns.Add(pattern);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }

        public IHttpActionResult AddAnswerBlock([FromBody]Pattern pattern)
        {
            if (ModelState.IsValid)
            {

                db.Patterns.Add(pattern);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("The images already exists in the database.");
        }
    }
}
