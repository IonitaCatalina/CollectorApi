using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace CollectorsApi.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Pattern> Patterns { get; set; }
        public virtual ICollection<Photo> Tests { get; set; }
        public virtual ICollection<ClassBook> ClassBooks { get; set; }
    }
}