using System.Threading.Tasks;

namespace ReviewService.Services
{
    public interface IArticleServiceClient
    {
        Task<bool> ArticleExists(int articleId);
    }
} 