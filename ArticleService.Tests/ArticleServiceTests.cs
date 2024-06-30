using System;
using System.Threading.Tasks;
using ArticleService.Controllers;
using ArticleService.DataAccessLayer;
using ArticleService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ArticleService.Tests
{
  public class ArticleServiceTests
  {
    private readonly ArticlesController _controller;
    private readonly Mock<IArticleService> _mockArticleService;

    public ArticleServiceTests()
    {
      _mockArticleService = new Mock<IArticleService>();
      _controller = new ArticlesController(null, _mockArticleService.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsOkResult_WithArticle()
    {
      // Arrange
      var article = new Article
      {
        Id = 1, Title = "Test Article", Author = "Author", ArticleContent = "Content", PublishDate = DateTime.Now,
        StarCount = 5
      };
      _mockArticleService.Setup(service => service.GetArticle(1)).ReturnsAsync(article);

      // Act
      var result = await _controller.GetArticle(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var returnedArticle = Assert.IsType<Article>(okResult.Value);
      Assert.Equal(1, returnedArticle.Id);
    }

    [Fact]
    public async Task CreateArticle_ReturnsCreatedAtActionResult()
    {
      // Arrange
      var article = new Article
      {
        Title = "New Article", Author = "Author", ArticleContent = "Content", PublishDate = DateTime.Now, StarCount = 4
      };
      _mockArticleService.Setup(service => service.AddArticle(article)).ReturnsAsync(article);

      // Act
      var result = await _controller.AddArticle(article);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result);
      var returnedArticle = Assert.IsType<Article>(createdResult.Value);
      Assert.Equal(article.Title, returnedArticle.Title);
    }

    [Fact]
    public async Task UpdateArticle_ReturnsNoContentResult()
    {
      // Arrange
      var article = new Article
      {
        Id = 1, Title = "Updated Article", Author = "Author", ArticleContent = "Updated Content",
        PublishDate = DateTime.Now, StarCount = 5
      };
      _mockArticleService.Setup(service => service.UpdateArticle(article)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateArticle(1, article);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteArticle_ReturnsNoContentResult()
    {
      // Arrange
      _mockArticleService.Setup(service => service.DeleteArticle(1)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteArticle(1);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }
  }
}