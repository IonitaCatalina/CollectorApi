using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class ClassBook
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<User> Students { get; set; }
    }
}