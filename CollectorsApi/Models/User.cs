using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace CollectorsApi.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }

        public virtual IEnumerable<Grade> Grades { get; set; }
        public virtual IEnumerable<Pattern> Patterns { get; set; }
        public virtual IEnumerable<Photo> Tests { get; set; }
    }
}