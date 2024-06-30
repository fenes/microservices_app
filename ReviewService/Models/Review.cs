namespace ReviewService.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }
    }
}
