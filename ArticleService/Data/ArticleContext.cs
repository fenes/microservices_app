using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Data
{
    public class ArticleContext : DbContext
    {
        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options) { }
        public DbSet<Article> Articles { get; set; }
    }
}
