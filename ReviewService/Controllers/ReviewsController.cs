using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReviewService.DataAccessLayer;
using ReviewService.Models;
using ReviewService.Models.DTOs;
using ReviewService.Models.Common;
using ReviewService.Services;

namespace ReviewService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICacheService<ReviewDto> _cacheService;
        private readonly IArticleServiceClient _articleServiceClient;

        public ReviewsController(
            IReviewService reviewService,
            ICacheService<ReviewDto> cacheService,
            IArticleServiceClient articleServiceClient)
        {
            _reviewService = reviewService;
            _cacheService = cacheService;
            _articleServiceClient = articleServiceClient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var cacheKey = $"review-{id}";
            var review = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var result = await _reviewService.GetReview(id);
                return result == null ? null : MapToDto(result);
            });

            if (review == null)
            {
                return NotFound(new ApiError("Review not found", "NOT_FOUND"));
            }

            return review;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            var articleExists = await _articleServiceClient.ArticleExists(createReviewDto.ArticleId);
            if (!articleExists)
            {
                return BadRequest(new ApiError("Invalid Article ID", "INVALID_ARTICLE"));
            }

            var review = MapToEntity(createReviewDto);
            var result = await _reviewService.AddReview(review);
            var reviewDto = MapToDto(result);
            
            return CreatedAtAction(nameof(GetReview), new { id = reviewDto.Id }, reviewDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto updateReviewDto)
        {
            var review = MapToEntity(updateReviewDto);
            review.Id = id;
            
            try
            {
                await _reviewService.UpdateReview(review);
                await _cacheService.RemoveAsync($"review-{id}");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiError("Review not found", "NOT_FOUND"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReview(id);
                await _cacheService.RemoveAsync($"review-{id}");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiError("Review not found", "NOT_FOUND"));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<ReviewDto>>> SearchReviews(
            string? reviewer,
            int? reviewCount,
            int? articleId,
            int page = 1,
            int pageSize = 10)
        {
            var reviews = await _reviewService.GetReviews(reviewer, reviewCount, articleId);
            var totalCount = reviews.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var pagedReviews = reviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PaginatedResponse<ReviewDto>
            {
                Items = pagedReviews,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }

        private static ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                ArticleId = review.ArticleId,
                Reviewer = review.Reviewer,
                ReviewContent = review.ReviewContent
            };
        }

        private static Review MapToEntity(CreateReviewDto dto)
        {
            return new Review
            {
                ArticleId = dto.ArticleId,
                Reviewer = dto.Reviewer,
                ReviewContent = dto.ReviewContent
            };
        }

        private static Review MapToEntity(UpdateReviewDto dto)
        {
            return new Review
            {
                Reviewer = dto.Reviewer,
                ReviewContent = dto.ReviewContent
            };
        }
    }
}