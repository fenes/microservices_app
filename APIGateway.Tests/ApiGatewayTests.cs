using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests
{
    public class ApiGatewayTests
    {
        private readonly HttpClient _client;

        public ApiGatewayTests()
        {
            _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:5000") };
        }

        [Fact]
        public async Task GetArticles_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/articles");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetReviews_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/reviews");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
