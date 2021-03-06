using System;
using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.Exams.Entities;
using System.Linq;

namespace GradProjectServer.Mapping.Exams
{
    public class ExamToExamDtoConverter : ITypeConverter<Exam, ExamDto>
    {
        //todo: check me
        public ExamDto Convert(Exam src, ExamDto dst, ResolutionContext ctx)
        {
            dst = new()
            {
                Course = ctx.Mapper.Map<CourseDto>(src.Course),
                Duration = (int) src.Duration.TotalMilliseconds,
                Id = src.Id,
                IsApproved = src.IsApproved,
                Name = src.Name,
                Semester = src.Semester,
                Volunteer = ctx.Mapper.Map<UserMetadataDto>(src.Volunteer),
                Year = src.Year,
                Questions = src.Questions?.Select(q => new ExamQuestionDto
                    {
                        Id = q.Id,
                        Title = q.Question.Title,
                        Content = q.Question.Content,
                        QuestionId = q.QuestionId,
                        ExamSubQuestions = q.ExamSubQuestions?.Select(sq => new ExamSubQuestionDto
                            {
                                Id = sq.Id,
                                SubQuestion = ctx.Mapper.Map<SubQuestionMetadataDto>(sq.SubQuestion),
                                Weight = sq.Weight,
                                Order = sq.Order
                            })
                            .OrderBy(sq => sq.Order)
                            .ThenBy(sq => sq.Id)
                            .ToArray()
                    })
                    .OrderBy(q => q.Order)
                    .ThenBy(q => q.Id)
                    .ToArray()
            };
            return dst;
        }
    }
}