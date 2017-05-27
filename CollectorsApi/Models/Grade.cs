namespace CollectorsApi.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public float Score { get; set; }

        // 1 student
        public int UserId { get; set; }
        public virtual User Student { get; set; }
    }
}