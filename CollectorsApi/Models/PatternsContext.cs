using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace CollectorsApi.Models
{
    public class PatternsContext : IdentityDbContext<User>
    {
        public PatternsContext() : base("name=PatternsContext")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<ClassBook> ClassBooks { get; set; }

        public static PatternsContext Create()
        {
            return new PatternsContext();
        }
    }
}