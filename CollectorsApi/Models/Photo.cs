﻿namespace CollectorsApi.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        //1 creator
        public string TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        //1 student
        public string StudentId { get; set; }
        public virtual User Student { get; set; }

        //1 pattern
        public string PatternId { get; set; }
        public virtual Pattern Pattern { get; set; }

        //1 grade
        public int GradeId { get; set; }
        public virtual Grade Grade { get; set; }
    }
}