using System;
using System.Net;
using System.Threading.Tasks;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Courses;
using RestSharp;
using Xunit;

namespace GradProjectServerTests
{
    public class Course : IClassFixture<TestContext>
    {
        private readonly TestContext _ctx;
        private readonly static string ControllerName = "Course";

        public Course(TestContext ctx)
        {
            _ctx = ctx;
        }

        RestClient GetClient() => _ctx.GetClient(ControllerName);

        [Fact]
        public async Task Update_NonAdmin_403()
        {
            UpdateCourseDto dto = new()
            {
                Id = 1,
                Name = "Abdallah",
            };

            RestRequest request = new("Update", Method.PATCH);
            request.AddJsonBody(dto);

            var client =await _ctx.GetNonAdminClient(ControllerName);
            var response = await client.ExecuteAsync<ErrorDTO>(request).ConfigureAwait(false);
            
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }
        
        [Fact]
        public async Task Update_NegativeCreditHours_422AndCreditHoursError()
        {
            UpdateCourseDto dto = new()
            {
                Id = 1,
                CreditHours = -1
            };

            RestRequest request = new("Update", Method.PATCH);
            request.AddJsonBody(dto);

            var client =await _ctx.GetAdminClient(ControllerName);
            var response = await client.ExecuteAsync<ErrorDTO>(request).ConfigureAwait(false);
            
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(UpdateCourseDto.CreditHours), StringComparison.OrdinalIgnoreCase));
        }
        [Fact]
        public async Task Update_NonExistingCourse_422AndIdError()
        {
            UpdateCourseDto dto = new()
            {
                Id = 1,
                CreditHours = -1
            };

            RestRequest request = new("Update", Method.PATCH);
            request.AddJsonBody(dto);

            var client =await _ctx.GetAdminClient(ControllerName);
            var response = await client.ExecuteAsync<ErrorDTO>(request).ConfigureAwait(false);
            
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(UpdateCourseDto.CreditHours), StringComparison.OrdinalIgnoreCase));
        }
    }
}