using System.Net;
using System.Text;
using System.Threading.Tasks;
using GradProjectServer.DTO.Users;
using RestSharp;

namespace GradProjectServerTests
{
    public class TestContext
    {
        private static readonly string ServerAddress = "http://localhost:1234/";
        public RestClient GetClient(string urlSuffix = "") => new($"{ServerAddress}{urlSuffix}");

        private async Task<RestClient> GetClient(string email, string pass, string urlSuffix)
        {
            var client = GetClient();
            LoginDto login = new()
            {
                Email = email,
                Password = pass
            };
            client.CookieContainer = new CookieContainer();
            RestRequest loginRequest = new($"{ServerAddress}User/Login");
            loginRequest.AddJsonBody(login);
            var x = await client.ExecutePostAsync(loginRequest).ConfigureAwait(false);
            client.BaseHost = $"{ServerAddress}";
            client.BaseUrl = new ($"{ServerAddress}{urlSuffix}");
            return client;
        }

        public Task<RestClient> GetNonAdminClient(string urlSuffix = "") =>
            GetClient("z@z.com", "z123456789z", urlSuffix);
        public Task<RestClient> GetAdminClient(string urlSuffix = "") =>
            GetClient("a@a.com", "a123456789a", urlSuffix);
    }
}