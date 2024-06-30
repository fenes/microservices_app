using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.DataAccessLayer;

public class ArticleService : IArticleService
{
  private readonly ArticleContext _context;

  public ArticleService(ArticleContext context)
  {
    _context = context;
  }

  public async Task<Article> GetArticle(int id)
  {
    return await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
  }

  public async Task<List<Article>> GetArticles()
  {
    return await _context.Articles.ToListAsync();
  }

  public async Task<Article> AddArticle(Article article)
  {
    _context.Articles.Add(article);
    await _context.SaveChangesAsync();
    return article;
  }

  public async Task UpdateArticle(Article article)
  {
    _context.Entry(article).State = EntityState.Modified;
    await _context.SaveChangesAsync();
  }

  public async Task DeleteArticle(int id)
  {
    var review = await _context.Articles.FindAsync(id);
    if (review == null)
    {
      throw new Exception("Article not found");
    }

    _context.Articles.Remove(review);
    await _context.SaveChangesAsync();
  }

  public Task<List<Article>> GetArticles(string title, int StarCount)
  {
    return _context.Articles.Where(a => a.Title == title && a.StarCount == StarCount)
      .ToListAsync();
  }
}