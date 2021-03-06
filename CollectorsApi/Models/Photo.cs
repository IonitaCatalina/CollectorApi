﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectorsApi.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }

        //1 creator
        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        //1 student
        public string StudentId { get; set; }
        public virtual User Student { get; set; }

        //1 pattern
        public int PatternId { get; set; }
        public virtual Pattern Pattern { get; set; }

        public int Grade { get; set; }
    }
}