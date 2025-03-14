namespace CorpBite.Web.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}