using System;
using System.Net.Http;
using System.Threading.Tasks;
using DockerCommon;
using Microsoft.Extensions.Options;
using RestSharp;

namespace GradProjectServer.Services.CheckersManagers
{
    public class DockerBroker
    {
        private readonly RestClient _restClient;

        public DockerBroker(IOptions<AppOptions> options)
        {
            _restClient =
                new RestClient($"http://{options.Value.DockerBrokerAddress}:{options.Value.DockerBrokerPort}/Docker/");
        }

        public async Task<JobResult> Build(string relativeArchivePath, string relativeSavePath)
        {
            var request = new RestRequest("Build", Method.POST);
            request.AddJsonBody(new BuildJobDto
            {
                RelativeArchivePath = relativeArchivePath,
                RelativeSavePath = relativeSavePath
            });
            return await _restClient.PostAsync<JobResult>(request).ConfigureAwait(false);
        }

        public async Task<JobResult> Check(string relativeSubmissionDirectory, string relativeCheckerDirectory,
            string relativeResultDirectory)
        {
            var request = new RestRequest("Check", Method.POST);
            request.AddJsonBody(new CheckJobDto
            {
                RelativeSubmissionDirectory = relativeSubmissionDirectory,
                RelativeCheckerDirectory = relativeCheckerDirectory,
                RelativeResultDirectory = relativeResultDirectory
            });
            return await _restClient.PostAsync<JobResult>(request).ConfigureAwait(false);
        }
    }
}