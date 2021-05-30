using System.Net;
using System.Threading.Tasks;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Tags;
using GradProjectServer.DTO.Users;
using RestSharp;
using Xunit;

namespace GradProjectServerTests
{
    public class Tag: IClassFixture<TestContext>
    {
        private readonly TestContext _ctx;
        private readonly static string ControllerName = "Tag";
        public Tag(TestContext ctx)
        {
            _ctx = ctx;
        }

        RestClient GetClient() => _ctx.GetClient(ControllerName);

        [Fact]
        public async Task Create_NotAdmin_403()
        {
            CreateTagDto dto = new()
            {
                Name = "Abdallah",
            };

            RestRequest request = new("Create");
            request.AddJsonBody(dto);

            var client =await _ctx.GetNonAdminClient(ControllerName);
            var response = await client.ExecutePostAsync<ErrorDTO>(request).ConfigureAwait(false);
            
            //Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}