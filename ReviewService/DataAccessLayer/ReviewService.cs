using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Models;

namespace ReviewService.DataAccessLayer
{
  public class ReviewService : IReviewService
  {
    private readonly ReviewContext _context;

    public ReviewService(ReviewContext context)
    {
      _context = context;
    }

    public async Task<Review> GetReview(int id)
    {
      return await _context.Reviews.FindAsync(id);
    }

    public async Task<List<Review>> GetReviews()
    {
      return await _context.Reviews.ToListAsync();
    }

    public async Task<Review> AddReview(Review review)
    {
      _context.Reviews.Add(review);
      await _context.SaveChangesAsync();
      return review;
    }

    public async Task UpdateReview(Review review)
    {
      _context.Entry(review).State = EntityState.Modified;
      await _context.SaveChangesAsync();
    }

    public async Task DeleteReview(int id)
    {
      var review = await _context.Reviews.FindAsync(id);
      if (review == null)
      {
        throw new KeyNotFoundException("Review not found");
      }

      _context.Reviews.Remove(review);
      await _context.SaveChangesAsync();
    }

    public Task<List<Review>> GetReviews(string reviewer, int reviewCount, int articleId)
    {
      return _context.Reviews.Where(r => r.Reviewer == reviewer && r.ArticleId == articleId)
        .Take(reviewCount)
        .ToListAsync();
    }
  }
}