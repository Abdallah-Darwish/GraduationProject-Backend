using System;
using System.Net;
using System.Threading.Tasks;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Users;
using RestSharp;
using Xunit;

namespace GradProjectServerTests
{
    public class User : IClassFixture<TestContext>
    {
        private readonly TestContext _ctx;
        private readonly static string ControllerName = "User";

        public User(TestContext ctx)
        {
            _ctx = ctx;
        }

        RestClient GetClient() => _ctx.GetClient(ControllerName);

        [Fact]
        public async Task Signup_InvalidEmail_422AndInvalidEmailError()
        {
            SignUpDto dto = new()
            {
                Email = "mail",
                Name = "Abdallah",
                Password = ".123456789a",
                StudyPlanId = 1
            };

            RestRequest request = new("Create");
            request.AddJsonBody(dto);

            var client = GetClient();
            var response = await client.ExecutePostAsync<ErrorDTO>(request).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(SignUpDto.Email), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Signup_UsedEmail_422AndUsedEmailError()
        {
            SignUpDto dto = new()
            {
                Email = "a@a.com",
                Name = "Abdallah",
                Password = ".123456789a",
                StudyPlanId = 1
            };

            RestRequest request = new("Create");
            request.AddJsonBody(dto);

            var client = GetClient();
            var response = await client.ExecutePostAsync<ErrorDTO>(request).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(SignUpDto.Email), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Signup_InvalidImage_422AndImageError()
        {
            SignUpDto dto = new()
            {
                Email = "zzz@zzz.com",
                Name = "Abdallah",
                Password = ".123456789a",
                StudyPlanId = 1,
                ProfilePictureJpgBase64 = "12uu"
            };

            RestRequest request = new("Create");
            request.AddJsonBody(dto);

            var client = GetClient();
            var response = await client.ExecutePostAsync<ErrorDTO>(request).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(SignUpDto.ProfilePictureJpgBase64), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Update_AnotherUserByNonAdmin_422AndIdError()
        {
            UpdateUserDto dto = new()
            {
                Id = 1,
                Name = "Abdallah",
            };

            RestRequest request = new("Update");
            request.AddJsonBody(dto);

            var client = await _ctx.GetNonAdminClient(ControllerName).ConfigureAwait(false);
            var response = await client.ExecuteAsync<ErrorDTO>(request, Method.PATCH).ConfigureAwait(false);

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(response.Data.Data.Keys,
                p => p.Equals(nameof(UpdateUserDto.Id), StringComparison.OrdinalIgnoreCase));
        }
    }
}