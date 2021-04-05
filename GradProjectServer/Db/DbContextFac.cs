using GradProjectServer.Services.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Db
{
    public static class DbContextFac
    {
        //todo: fill me
        public static IDbContextFactory<AppDbContext> Factory { get; }
    }
}
