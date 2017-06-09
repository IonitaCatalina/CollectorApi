namespace CollectorsApi.Models
{
    public class CorrectAnswers
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int QuestionId { get; set; }
        public int Answer { get; set; }

        public int PatternId { get; set; }
        public virtual Pattern Pattern { get; set; }
    }
}