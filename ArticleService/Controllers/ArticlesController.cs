using System.Text.Json;
using System.Threading.Tasks;
using ArticleService.DataAccessLayer;
using ArticleService.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ArticleService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ArticlesController : ControllerBase
  {
    private readonly IArticleService _articleService;
    private readonly IDatabase _cache;

    public ArticlesController(IConnectionMultiplexer redis, IArticleService articleService)
    {
      _articleService = articleService;
      _cache = redis.GetDatabase();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetArticle(int id)
    {
      var cacheKey = $"article-{id}";
      var cachedArticle = await _cache.StringGetAsync(cacheKey);

      if (!cachedArticle.IsNullOrEmpty)
      {
        return Ok(JsonSerializer.Deserialize<Article>(cachedArticle));
      }

      var article = await _articleService.GetArticle(id);

      if (article == null)
      {
        return NotFound();
      }

      await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(article));
      return article;
    }

    [HttpPost]
    public async Task<ActionResult<Article>> AddArticle(Article article)
    {
      await _articleService.AddArticle(article);
      return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, Article article)
    {
      if (id != article.Id)
      {
        return BadRequest();
      }

      await _articleService.UpdateArticle(article);
      await _cache.KeyDeleteAsync($"article-{id}");
      return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
      await _articleService.DeleteArticle(id);
      await _cache.KeyDeleteAsync($"article-{id}");
      return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<Article>> GetArticles(string? title, int? starCount)
    {
      return Ok(await _articleService.GetArticles(title, starCount));
    }
  }
}