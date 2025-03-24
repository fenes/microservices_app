namespace ReviewService.Models.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }
    }

    public class CreateReviewDto
    {
        public int ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }
    }

    public class UpdateReviewDto
    {
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }
    }
} 