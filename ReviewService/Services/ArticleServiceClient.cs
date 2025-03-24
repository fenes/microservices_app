using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReviewService.Services
{
    public class ArticleServiceClient : IArticleServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _articleServiceUrl;

        public ArticleServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _articleServiceUrl = configuration.GetSection("ProjectConfig").GetValue<string>("ArticleServiceUrl");
        }

        public async Task<bool> ArticleExists(int articleId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_articleServiceUrl}/{articleId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 