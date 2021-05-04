using System;
using System.IO;
using System.Threading.Tasks;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services.FilesManagers
{
    public class ProgrammingSubQuestionAnswerFileManager
    {
        /*
          * ProgrammingSubQuestionAnswer, needs extension:
            - Save Answer
            - Get Answer
         */
        public static string SaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "ProgrammingSubQuestionsAnswers");
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        public static string GetAnswerPath(int answerId, string extension) =>
            Path.Combine(SaveDirectory, $"{answerId}.{extension}");

        public async Task SaveAnswer(int answerId, string extension, Stream content)
        {
            var answerPath = GetAnswerPath(answerId, extension);
            await using var answerFileStream =
                new FileStream(answerPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            await content.CopyToAsync(answerFileStream).ConfigureAwait(false);
            content.SetLength(content.Position);
        }

        public Task SaveAnswer(ProgrammingSubQuestionAnswer answer, Stream content) =>
            SaveAnswer(answer.Id, answer.FileExtension, content);

        public Stream GetAnswer(int answerId, string extension)
        {
            var answerPath = GetAnswerPath(answerId, extension);
            return new FileStream(answerPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetAnswer(ProgrammingSubQuestionAnswer answer) => GetAnswer(answer.Id, answer.FileExtension);

        public void DeleteAnswer(int answerId, string extension)
        {
            var answerPath = GetAnswerPath(answerId, extension);
            if (File.Exists(answerPath))
            {
                File.Delete(answerPath);
            }
        }

        public void DeleteAnswer(ProgrammingSubQuestionAnswer answer) => DeleteAnswer(answer.Id, answer.FileExtension);
    }
}