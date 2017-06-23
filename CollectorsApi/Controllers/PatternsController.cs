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

        [Route("~/api/patterns/{id:int}/answerBlocks")]
        public Pattern GetPattern(int id)
        {
            return db.Patterns.Include("AnswerBlocks").FirstOrDefault(x => x.Id == id);
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

        [Route("AddBlock")]
        public IHttpActionResult AddAnswerBlock([FromBody]Pattern p)
        {
            var pattern = db.Patterns.Include("AnswerBlocks").FirstOrDefault(x => x.Id == p.Id);

            if (pattern != null)
            {
                foreach (var block in p.AnswerBlocks)
                {
                    if (!pattern.AnswerBlocks.Contains(block))
                    {
                        pattern.AnswerBlocks.Add(block);
                        db.SaveChanges();
                        ;
                    }
                }

                return Ok();
            }

            return BadRequest("User already exists in the class book");
        }

        [Route("AddSheet")]
        public IHttpActionResult AddAnswerSheet([FromBody]PatternAnswerSheet aSheet)
        {
            var pattern = db.Patterns.FirstOrDefault(x => x.Id == aSheet.PatternId);
            if (pattern != null)
            {
                db.AnswerSheets.Add(aSheet);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}
