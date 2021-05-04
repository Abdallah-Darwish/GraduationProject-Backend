using AutoMapper;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DbManager _dbSeeder;

        public TestController(AppDbContext dbContext, IMapper mapper, DbManager dbSeeder)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dbSeeder = dbSeeder;
        }

        [HttpGet("Seed")]
        public async Task<IActionResult> Seed()
        {
            await _dbSeeder.Seed().ConfigureAwait(false);
            return Ok("Seeded succfully.");
        }

        [HttpGet("RecreateDb")]
        public async Task<IActionResult> RecreateDb()
        {
            await _dbSeeder.RecreateDb().ConfigureAwait(false);
            return Ok("Recerated succfully.");
        }
    }
}