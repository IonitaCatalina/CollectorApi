using System.ComponentModel.DataAnnotations;

namespace CollectorsApi.Models
{
    public class PatternAnswerSheet
    {
        [Key]
        public int Id { get; set; }
        public int QuestionNumber { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string AnswerString { get; set; }

        public int PatternId { get; set; }
        public Pattern Pattern { get; set; }

        public string StudentId { get; set; }
        public User Student { get; set; }
    }
}