namespace ClassBooksWebApp.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        //1 creator
        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        //1 student
        public string StudentId { get; set; }
        public virtual User Student { get; set; }

        //1 pattern
        public int PatternId { get; set; }
        public virtual Pattern Pattern { get; set; }

        public int Grade { get; set; }
    }
}