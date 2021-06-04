using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Xunit;

namespace GradProjectServerTests
{
    public class ExamAttempt : IClassFixture<TestContext>
    {
        private readonly TestContext _ctx;
        private readonly static string ControllerName = "ExamAttempt";

        public ExamAttempt(TestContext ctx)
        {
            _ctx = ctx;
        }

        [Fact]
        public async Task GetActive_NoActive_NoContent()
        {
            var client = await _ctx.GetNonAdminClient(ControllerName).ConfigureAwait(false);
            RestRequest finishRequest = new("FinishCurrent", Method.POST);
            var finishResponse = await client.ExecuteAsync(finishRequest).ConfigureAwait(false);


            RestRequest getActiveRequest = new("GetActive", Method.GET);
            var getActiveResponse = await client.ExecuteAsync(getActiveRequest).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.NoContent, getActiveResponse.StatusCode);
        }

        [Fact]
        public async Task Create_ExistingExam_Created()
        {
            var client = await _ctx.GetNonAdminClient(ControllerName).ConfigureAwait(false);
            RestRequest finishRequest = new("FinishCurrent", Method.POST);
            var finishResponse = await client.ExecuteAsync(finishRequest).ConfigureAwait(false);


            RestRequest createRequest = new("Create", Method.GET);
            createRequest.AddQueryParameter("examId", "1");
            var getActiveResponse = await client.ExecuteAsync(createRequest).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.Created, getActiveResponse.StatusCode);
        }
    }
}