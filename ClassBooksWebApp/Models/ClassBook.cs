using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class ClassBook
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string TeacherId { get; set; }
        public User Teacher { get; set; }

        public ICollection<User> Students { get; set; }
    }
}