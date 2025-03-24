using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticleService.DataAccessLayer;
using ArticleService.Models;
using ArticleService.Models.DTOs;
using ArticleService.Models.Common;
using ArticleService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ICacheService<ArticleDto> _cacheService;

        public ArticlesController(IArticleService articleService, ICacheService<ArticleDto> cacheService)
        {
            _articleService = articleService;
            _cacheService = cacheService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticle(int id)
        {
            var cacheKey = $"article-{id}";
            var article = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var result = await _articleService.GetArticle(id);
                return result == null ? null : MapToDto(result);
            });

            if (article == null)
            {
                return NotFound(new ApiError("Article not found", "NOT_FOUND"));
            }

            return article;
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto createArticleDto)
        {
            var article = MapToEntity(createArticleDto);
            var result = await _articleService.AddArticle(article);
            var articleDto = MapToDto(result);
            
            return CreatedAtAction(nameof(GetArticle), new { id = articleDto.Id }, articleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, UpdateArticleDto updateArticleDto)
        {
            var article = MapToEntity(updateArticleDto);
            article.Id = id;
            
            try
            {
                await _articleService.UpdateArticle(article);
                await _cacheService.RemoveAsync($"article-{id}");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiError("Article not found", "NOT_FOUND"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                await _articleService.DeleteArticle(id);
                await _cacheService.RemoveAsync($"article-{id}");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiError("Article not found", "NOT_FOUND"));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<ArticleDto>>> SearchArticles(
            string? title,
            int? starCount,
            int page = 1,
            int pageSize = 10)
        {
            var articles = await _articleService.GetArticles(title, starCount);
            var totalCount = articles.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PaginatedResponse<ArticleDto>
            {
                Items = pagedArticles,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }

        private static ArticleDto MapToDto(Article article)
        {
            return new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Author = article.Author,
                ArticleContent = article.ArticleContent,
                PublishDate = article.PublishDate,
                StarCount = article.StarCount
            };
        }

        private static Article MapToEntity(CreateArticleDto dto)
        {
            return new Article
            {
                Title = dto.Title,
                Author = dto.Author,
                ArticleContent = dto.ArticleContent,
                PublishDate = dto.PublishDate,
                StarCount = dto.StarCount
            };
        }

        private static Article MapToEntity(UpdateArticleDto dto)
        {
            return new Article
            {
                Title = dto.Title,
                Author = dto.Author,
                ArticleContent = dto.ArticleContent,
                PublishDate = dto.PublishDate,
                StarCount = dto.StarCount
            };
        }
    }
}