using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public virtual IEnumerable<Pattern> Patterns { get; set; }
        public virtual IEnumerable<Photo> Tests { get; set; }
    }
}