using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace Scratch
{
    class Program
    {
        static void Run()
        {
            using var push = new PushSocket("@tcp://*:1235");
            using var pull = new PullSocket(">tcp://*:1235");
            NetMQMessage msg = new();
            msg.Append("Hello");
            msg.Append("There");
            push.SendMultipartMessage(msg);
            var rec = pull.ReceiveMultipartMessage();
            Console.WriteLine(rec[0].ConvertToString());
        }
        static async Task Main(string[] args)
        {
            HttpClient client = new();
            dynamic x = new ExpandoObject();
            x.RelativeArchivePath = @"ProgrammingSubQuestions/Checkers/928.zip";
            x.RelativeSavePath = @"CheckerBin/";
            JsonContent content = JsonContent.Create(x);
            var res = await client.PostAsync("http://localhost:1237/Docker/Build", content).ConfigureAwait(false);
            Console.WriteLine(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            Console.ReadLine();
        }
    }
}