using System;
using System.Threading.Tasks;
using ReviewService.Controllers;
using ReviewService.DataAccessLayer;
using ReviewService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ReviewService.Tests
{
  public class ReviewServiceTests
  {
    private readonly ReviewsController _controller;
    private readonly Mock<IReviewService> _mockReviewService;

    public ReviewServiceTests()
    {
      _mockReviewService = new Mock<IReviewService>();
      _controller = new ReviewsController(null, null, _mockReviewService.Object, null);
    }

    [Fact]
    public async Task GetReview_ReturnsOkResult_With()
    {
      var review = new Review()
        { Id = 1, Reviewer = "Reviewer", ArticleId = 1, ReviewContent = "Content" };
      _mockReviewService.Setup(service => service.GetReview(1)).ReturnsAsync(review);

      // Act
      var result = await _controller.GetReview(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var returned = Assert.IsType<Review>(okResult.Value);
      Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task CreateReview_ReturnsCreatedAtActionResult()
    {
      // Arrange
      var review = new Review { Reviewer = "Reviewer", ArticleId = 1, ReviewContent = "Content" };
      _mockReviewService.Setup(service => service.AddReview(review)).ReturnsAsync(review);

      // Act
      var result = await _controller.AddReview(review);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result);
      var returned = Assert.IsType<Review>(createdResult.Value);
      Assert.Equal(review.Reviewer, returned.Reviewer);
    }

    [Fact]
    public async Task UpdateReview_ReturnsNoContentResult()
    {
      // Arrange
      var review = new Review() { Id = 1, Reviewer = "Reviewer", ArticleId = 1, ReviewContent = "Content" };
      _mockReviewService.Setup(service => service.UpdateReview(review)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateReview(1, review);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNoContentResult()
    {
      // Arrange
      _mockReviewService.Setup(service => service.DeleteReview(1)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteReview(1);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }
  }
}