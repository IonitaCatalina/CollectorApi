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
            var patterns = db.Patterns.Include("AnswerSheet").Where(x => x.TeacherId == id);

            foreach (var pattern in patterns)
            {
                foreach (var answersheet in pattern.AnswerSheet)
                {
                    answersheet.Pattern = null;
                }    
            }
            return patterns;
        }

        [Route("~/api/patterns/{id:int}/answerBlocks")]
        public Pattern GetPattern(int id)
        {
            return db.Patterns.Include("AnswerBlocks").FirstOrDefault(x => x.Id == id);
        }

        [Route("addPattern")]
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

        [Route("~/api/deletePattern/{id:int}")]
        [HttpPost]
        public IHttpActionResult DeletePattern(int id)
        {
            if (ModelState.IsValid)
            {
                db.Patterns.Remove(db.Patterns.FirstOrDefault(x => x.Id == id));
                db.SaveChanges();
                return Ok();
            }

            return BadRequest();
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

        //get answer sheet based on pattern id and is not of student
        [Route("~/api/pattern/{id:int}/answerSheet")]
        public IEnumerable<PatternAnswerSheet> GetAnswerSheet(int id)
        {
            return db.AnswerSheets.Where(x => x.PatternId == id && x.StudentId == null);           
        }
    }
}
