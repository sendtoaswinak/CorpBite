namespace CorpBite.Web.Models
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}