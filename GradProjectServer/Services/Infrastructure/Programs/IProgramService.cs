using GradProjectServer.DTO.Programs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure.Programs
{
    //todo: implement me
    public interface IProgramService
    {
        Task<bool> VerifyChecker(Stream checker);
        Task<string> SaveChecker(Stream checker, string? nameHint = null);
        Task<Stream> GetChecker(string name);
        Task<Stream> GetChecker(Program program);
        //A little bit dirty because I am mixing domains models, but at this point who even cares
        Task<Program> SaveProgram(CreateProgramDto pro, string? nameHint = null);
    }
}
