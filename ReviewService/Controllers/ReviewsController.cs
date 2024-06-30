using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReviewService.DataAccessLayer;
using ReviewService.Models;
using StackExchange.Redis;

namespace ReviewService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ReviewsController : ControllerBase
  {
    private readonly IDatabase _cache;
    private readonly HttpClient _httpClient;
    private readonly IReviewService _reviewService;
    private readonly string articleServiceUrl;
    private readonly IConfiguration _config;


    public ReviewsController(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory,
      IReviewService reviewService, IConfiguration config)
    {
      _reviewService = reviewService;
      _config = config;
      _cache = redis.GetDatabase();
      _httpClient = httpClientFactory.CreateClient();
      //get appsettings ProjectConfig:ArticleServiceUrl
      articleServiceUrl = _config.GetSection("ProjectConfig").GetValue<string>("ArticleServiceUrl");
    }

    [HttpPost]
    public async Task<ActionResult<Review>> AddReview(Review review)
    {
      try
      {
        var response = await _httpClient.GetAsync($"{articleServiceUrl}/{review.ArticleId}");
        if (!response.IsSuccessStatusCode)
        {
          return BadRequest("Invalid Article ID");
        }
      }
      catch (Exception e)
      {
        return BadRequest("Article Service is not available");
      }

      await _reviewService.AddReview(review);
      return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
      var cacheKey = $"review-{id}";
      var cachedReview = await _cache.StringGetAsync(cacheKey);

      if (!cachedReview.IsNullOrEmpty)
      {
        return Ok(JsonSerializer.Deserialize<Review>(cachedReview));
      }

      var review = await _reviewService.GetReview(id);

      if (review == null)
      {
        return NotFound();
      }

      await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(review));
      return review;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, Review review)
    {
      if (id != review.Id)
      {
        return BadRequest();
      }

      await _reviewService.UpdateReview(review);
      await _cache.KeyDeleteAsync($"review-{id}");
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
      await _reviewService.DeleteReview(id);
      await _cache.KeyDeleteAsync($"review-{id}");
      return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<Review>> GetReviews(string? reviewer, int? reviewCount, int? articleId)
    {
      return Ok(await _reviewService.GetReviews(reviewer, reviewCount, articleId));
    }
  }
}