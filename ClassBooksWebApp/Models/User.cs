using System.Collections.Generic;

namespace ClassBooksWebApp.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public List<string> Roles { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICollection<Pattern> Patterns { get; set; }
        public ICollection<Photo> Tests { get; set; }
    }
}