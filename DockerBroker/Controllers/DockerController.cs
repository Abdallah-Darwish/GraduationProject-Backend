using System.Threading.Tasks;
using DockerBroker.Services;
using Microsoft.AspNetCore.Mvc;
using DockerCommon;

namespace DockerBroker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DockerController : ControllerBase
    {
        private readonly JobDistributorService _distributor;

        public DockerController(JobDistributorService jobDistributorService)
        {
            _distributor = jobDistributorService;
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("Alive");
        }
        [HttpPost("Build")]
        public async Task<IActionResult> Build(BuildJobDto info)
        {
            var res = await _distributor.EnqueueBuildJob(info.RelativeArchivePath, info.RelativeSavePath).ConfigureAwait(false);
            return Ok(res);
        }

        [HttpPost("Check")]
        public async Task<IActionResult> Check(CheckJobDto info)
        {
            var res = await _distributor
                .EnqueueCheckJob(info.RelativeCheckerDirectory, info.RelativeResultDirectory, info.RelativeSubmissionDirectory)
                .ConfigureAwait(false);
            return Ok(res);
        }
    }
}