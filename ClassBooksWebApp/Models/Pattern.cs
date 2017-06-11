using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class Pattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        public virtual IEnumerable<Photo> TestPhotos { get; set; }
    }
}