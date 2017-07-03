using CollectorsApi.Helpers;
using CollectorsApi.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            return db.Patterns.Include("AnswerSheet").FirstOrDefault(x=>x.Id == id);
        }
        
        [Route("published")]
        public IEnumerable<Pattern> Get()
        {
            var patterns = db.Patterns.Include("AnswerSheet");
            foreach (var pattern in patterns)
            {
                foreach (var answersheet in pattern.AnswerSheet)
                {
                    answersheet.Pattern = null;
                }
            }
            return patterns;
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
                    pattern.Image = null;
                }    
            }
            return patterns;
        }

        [Route("~/api/patterns/publish/{id:int}")]
        public IHttpActionResult Publish(int id)
        {
           var pattern = db.Patterns.FirstOrDefault(x => x.Id == id);

            if (pattern != null)
            {
                pattern.Published = true;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [Route("~/api/patterns/{id:int}/answerBlocks")]
        public Pattern GetPattern(int id)
        {
            return db.Patterns.Include("AnswerBlocks").FirstOrDefault(x => x.Id == id);
        }

        [Route("~/api/patterns/pattern/{id:int}")]
        public Pattern GetPatternImage(int id)
        {
            return db.Patterns.FirstOrDefault(x => x.Id == id);
        }

        [Route("addPattern")]
        public IHttpActionResult Post([FromBody]Pattern pattern)
        {
         
            if (ModelState.IsValid)
            {
                var image = PatternGeneratorHelper.AddWrapPoints();
                pattern.Image = image.ToByteArray(ImageFormat.Jpeg);

                pattern.Width = image.Width;
                pattern.Height = image.Height;
                pattern.MaxSizeRatio = 0.001596;
                pattern.MinSizeRatio = 0.000304;
                
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
        public IHttpActionResult AddAnswerSheet([FromBody]List<PatternAnswerSheet> sheets)
        {
            var patternId = sheets[0].PatternId;
            var pattern = db.Patterns.Include("AnswerBlocks").FirstOrDefault(x => x.Id == patternId);

            var answerBlock = new AnswerBlock() {
                PatternId = patternId,
                AnswerOptionsNumber = 4
                };

            if (pattern != null)
            {
                var i = 1;
                if (pattern.AnswerBlocks.Count() != 0)
                {
                    i = pattern.AnswerBlocks.Last().FirstQuestionIndex + pattern.AnswerBlocks.Last().Rows;

                    answerBlock.FirstQuestionIndex = i;
                    answerBlock.Rows = sheets.Count();
                    answerBlock.CoordinateX = pattern.AnswerBlocks.Last().CoordinateX + 1000;
                    answerBlock.CoordinateY = 500;

                    var tempPattern = new Pattern {
                        Image = pattern.Image,
                        AnswerBlocks = new List<AnswerBlock>()
                    };

                    tempPattern.AnswerBlocks.Add(answerBlock);

                    var result = PatternGeneratorHelper.AddAnswerBlock(tempPattern);

                    pattern.Image = result.Image;
                    pattern.AnswerBlocks.Add(result.AnswerBlocks.First());

                    foreach (var sheet in sheets)
                    {
                        sheet.QuestionNumber = i;
                        i++;
                    }
                }
                else
                {
                    answerBlock.FirstQuestionIndex = i;
                    answerBlock.Rows = sheets.Count();
                    answerBlock.CoordinateX = 500;
                    answerBlock.CoordinateY = 500;

                    var tempPattern = new Pattern
                    {
                        Image = pattern.Image,
                        AnswerBlocks = new List<AnswerBlock>()
                    };

                    tempPattern.AnswerBlocks.Add(answerBlock);

                    var result = PatternGeneratorHelper.AddAnswerBlock(tempPattern);

                    pattern.Image = result.Image;
                    pattern.AnswerBlocks.Add(result.AnswerBlocks.First());

                    foreach (var sheet in sheets)
                    {
                        sheet.QuestionNumber = i;
                        i++;
                    }
                }

                db.AnswerBlocks.Add(answerBlock);
                db.AnswerSheets.AddRange(sheets);
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
