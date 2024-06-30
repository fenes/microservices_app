using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewService.Models;

namespace ReviewService.DataAccessLayer
{
  public interface IReviewService
  {
    Task<Review> GetReview(int id);
    Task<List<Review>> GetReviews();
    Task<Review> AddReview(Review review);
    Task UpdateReview(Review review);
    Task DeleteReview(int id);
    Task<List<Review>> GetReviews(string reviewer, int reviewCount, int articleId);
  }
}