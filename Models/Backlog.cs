namespace final_qualifying_work.Models
{
    public class Backlog
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public required string Description { get; set; }
    }
}
