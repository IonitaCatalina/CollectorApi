using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectorsApi.Models
{
    public class ClassBook
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; }

        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        public virtual ICollection<User> Students { get; set; }
    }
}