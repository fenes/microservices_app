using System;
using System.Threading.Tasks;
using ArticleService.Controllers;
using ArticleService.DataAccessLayer;
using ArticleService.Models;
using ArticleService.Models.DTOs;
using ArticleService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ArticleService.Tests
{
  public class ArticleServiceTests
  {
    private readonly ArticlesController _controller;
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly Mock<ICacheService<ArticleDto>> _mockCacheService;

    public ArticleServiceTests()
    {
      _mockArticleService = new Mock<IArticleService>();
      _mockCacheService = new Mock<ICacheService<ArticleDto>>();
      _controller = new ArticlesController(_mockArticleService.Object, _mockCacheService.Object);
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
      var articleDto = new ArticleDto
      {
        Id = 1,
        Title = "Test Article",
        Author = "Author",
        ArticleContent = "Content",
        PublishDate = article.PublishDate,
        StarCount = 5
      };

      _mockArticleService.Setup(service => service.GetArticle(1)).ReturnsAsync(article);
      _mockCacheService.Setup(service => service.GetOrSetAsync(
        It.IsAny<string>(),
        It.IsAny<Func<Task<ArticleDto>>>(),
        It.IsAny<TimeSpan?>()
      )).ReturnsAsync(articleDto);

      // Act
      var result = await _controller.GetArticle(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var returnedArticle = Assert.IsType<ArticleDto>(okResult.Value);
      Assert.Equal(1, returnedArticle.Id);
    }

    [Fact]
    public async Task CreateArticle_ReturnsCreatedAtActionResult()
    {
      // Arrange
      var createArticleDto = new CreateArticleDto
      {
        Title = "New Article",
        Author = "Author",
        ArticleContent = "Content",
        PublishDate = DateTime.Now,
        StarCount = 4
      };

      var article = new Article
      {
        Id = 1,
        Title = createArticleDto.Title,
        Author = createArticleDto.Author,
        ArticleContent = createArticleDto.ArticleContent,
        PublishDate = createArticleDto.PublishDate,
        StarCount = createArticleDto.StarCount
      };

      _mockArticleService.Setup(service => service.AddArticle(It.IsAny<Article>())).ReturnsAsync(article);

      // Act
      var result = await _controller.CreateArticle(createArticleDto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result);
      var returnedArticle = Assert.IsType<ArticleDto>(createdResult.Value);
      Assert.Equal(createArticleDto.Title, returnedArticle.Title);
    }

    [Fact]
    public async Task UpdateArticle_ReturnsNoContentResult()
    {
      // Arrange
      var updateArticleDto = new UpdateArticleDto
      {
        Title = "Updated Article",
        Author = "Author",
        ArticleContent = "Updated Content",
        PublishDate = DateTime.Now,
        StarCount = 5
      };

      _mockArticleService.Setup(service => service.UpdateArticle(It.IsAny<Article>())).Returns(Task.CompletedTask);
      _mockCacheService.Setup(service => service.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateArticle(1, updateArticleDto);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteArticle_ReturnsNoContentResult()
    {
      // Arrange
      _mockArticleService.Setup(service => service.DeleteArticle(1)).Returns(Task.CompletedTask);
      _mockCacheService.Setup(service => service.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteArticle(1);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }
  }
}