using GradProjectServer.Services.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Db
{
    public static class DbContextFac
    {
        //todo: fill me
        public static IDbContextFactory<AppDbContext> Factory { get; }
    }
}
