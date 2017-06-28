namespace ClassBooksWebApp.Models
{
    public class AnswerBlock
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int AnswerOptionsNumber { get; set; }
        public int Rows { get; set; }
        public int FirstQuestionIndex { get; set; }

        public int PatternId { get; set; }
    }
}