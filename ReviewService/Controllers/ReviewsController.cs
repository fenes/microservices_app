using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Models;
using StackExchange.Redis;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReviewService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewContext _context;
        private readonly IDatabase _cache;
        private readonly HttpClient _httpClient;

        public ReviewsController(ReviewContext context, IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _cache = redis.GetDatabase();
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            var response = await _httpClient.GetAsync($"http://article-service/api/articles/{review.ArticleId}");
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Invalid Article ID");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var cacheKey = $"review-{id}";
            var cachedReview = await _cache.StringGetAsync(cacheKey);

            if (!cachedReview.IsNullOrEmpty)
            {
                return Ok(System.Text.Json.JsonSerializer.Deserialize<Review>(cachedReview));
            }

            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(review));
            return review;
        }

        // Diğer CRUD operasyonları buraya eklenebilir
    }
}
