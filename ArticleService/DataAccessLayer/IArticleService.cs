using System.Collections.Generic;
using System.Threading.Tasks;
using ArticleService.Models;

namespace ArticleService.DataAccessLayer;

public interface IArticleService
{
  public Task<Article> GetArticle(int id);
  public Task<List<Article>> GetArticles();
  public Task<Article> AddArticle(Article article);
  public Task UpdateArticle(Article article);
  public Task DeleteArticle(int id);
  Task<List<Article>> GetArticles(string? title, int? starCount);
}