using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DockerCommon;
using NetMQ;
using NetMQ.Sockets;
using RestSharp;
using static System.Console;
namespace Scratch
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var client = new RestClient($"http://localhost:1237/Docker/");
            var request = new RestRequest("Build", Method.POST);
            request.AddJsonBody(new BuildJobDto
            {
                RelativeArchivePath = @"ProgrammingSubQuestions/CheckersSources/1222.zip",
                RelativeSavePath = "Temp"
            });
            var response= await client.ExecuteAsync<JobResult>(request).ConfigureAwait(false);
            response.ToString();
        }
    }
}