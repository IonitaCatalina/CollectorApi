using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }

        public virtual IEnumerable<Pattern> Patterns { get; set; }
        public virtual IEnumerable<Photo> Tests { get; set; }
    }
}