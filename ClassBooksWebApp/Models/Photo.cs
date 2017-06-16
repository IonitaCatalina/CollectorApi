namespace ClassBooksWebApp.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        //1 creator
        public string TeacherId { get; set; }
        public User Teacher { get; set; }

        //1 student
        public string StudentId { get; set; }
        public User Student { get; set; }

        //1 pattern
        public int PatternId { get; set; }
        public Pattern Pattern { get; set; }

        public int Grade { get; set; }
    }
}