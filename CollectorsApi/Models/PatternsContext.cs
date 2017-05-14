using System.Data.Entity;

namespace CollectorsApi.Models
{
    public class PatternsContext : DbContext
    {
        public PatternsContext() : base("name=PatternsContext")
        {

        }

        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}