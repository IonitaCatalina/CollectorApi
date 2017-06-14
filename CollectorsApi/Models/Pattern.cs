using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectorsApi.Models
{
    public class Pattern
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        

        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        public virtual ICollection<Photo> TestPhotos { get; set; }
        public virtual ICollection<AnswerBlock> AnswerBlocks { get; set; }
    }
}