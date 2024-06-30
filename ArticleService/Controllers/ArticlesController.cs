using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using StackExchange.Redis;

namespace ArticleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleContext _context;
        private readonly IDatabase _cache;

        public ArticlesController(ArticleContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _cache = redis.GetDatabase();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var cacheKey = $"article-{id}";
            var cachedArticle = await _cache.StringGetAsync(cacheKey);

            if (!cachedArticle.IsNullOrEmpty)
            {
                return Ok(System.Text.Json.JsonSerializer.Deserialize<Article>(cachedArticle));
            }

            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(article));
            return article;
        }

        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        // Diğer CRUD operasyonları buraya eklenebilir
    }
}
