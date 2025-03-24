using System;
using System.Threading.Tasks;
using ReviewService.Controllers;
using ReviewService.DataAccessLayer;
using ReviewService.Models;
using ReviewService.Models.DTOs;
using ReviewService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ReviewService.Tests
{
  public class ReviewServiceTests
  {
    private readonly ReviewsController _controller;
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly Mock<ICacheService<ReviewDto>> _mockCacheService;
    private readonly Mock<IArticleServiceClient> _mockArticleServiceClient;

    public ReviewServiceTests()
    {
      _mockReviewService = new Mock<IReviewService>();
      _mockCacheService = new Mock<ICacheService<ReviewDto>>();
      _mockArticleServiceClient = new Mock<IArticleServiceClient>();
      _controller = new ReviewsController(
        _mockReviewService.Object,
        _mockCacheService.Object,
        _mockArticleServiceClient.Object
      );
    }

    [Fact]
    public async Task GetReview_ReturnsOkResult_WithReview()
    {
      // Arrange
      var review = new Review
      {
        Id = 1,
        Reviewer = "Reviewer",
        ArticleId = 1,
        ReviewContent = "Content"
      };
      var reviewDto = new ReviewDto
      {
        Id = 1,
        Reviewer = "Reviewer",
        ArticleId = 1,
        ReviewContent = "Content"
      };

      _mockReviewService.Setup(service => service.GetReview(1)).ReturnsAsync(review);
      _mockCacheService.Setup(service => service.GetOrSetAsync(
        It.IsAny<string>(),
        It.IsAny<Func<Task<ReviewDto>>>(),
        It.IsAny<TimeSpan?>()
      )).ReturnsAsync(reviewDto);

      // Act
      var result = await _controller.GetReview(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var returned = Assert.IsType<ReviewDto>(okResult.Value);
      Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task CreateReview_ReturnsCreatedAtActionResult()
    {
      // Arrange
      var createReviewDto = new CreateReviewDto
      {
        Reviewer = "Reviewer",
        ArticleId = 1,
        ReviewContent = "Content"
      };

      var review = new Review
      {
        Id = 1,
        Reviewer = createReviewDto.Reviewer,
        ArticleId = createReviewDto.ArticleId,
        ReviewContent = createReviewDto.ReviewContent
      };

      _mockArticleServiceClient.Setup(client => client.ArticleExists(1)).ReturnsAsync(true);
      _mockReviewService.Setup(service => service.AddReview(It.IsAny<Review>())).ReturnsAsync(review);

      // Act
      var result = await _controller.CreateReview(createReviewDto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result);
      var returned = Assert.IsType<ReviewDto>(createdResult.Value);
      Assert.Equal(createReviewDto.Reviewer, returned.Reviewer);
    }

    [Fact]
    public async Task UpdateReview_ReturnsNoContentResult()
    {
      // Arrange
      var updateReviewDto = new UpdateReviewDto
      {
        Reviewer = "Updated Reviewer",
        ReviewContent = "Updated Content"
      };

      _mockReviewService.Setup(service => service.UpdateReview(It.IsAny<Review>())).Returns(Task.CompletedTask);
      _mockCacheService.Setup(service => service.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateReview(1, updateReviewDto);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNoContentResult()
    {
      // Arrange
      _mockReviewService.Setup(service => service.DeleteReview(1)).Returns(Task.CompletedTask);
      _mockCacheService.Setup(service => service.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteReview(1);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }
  }
}