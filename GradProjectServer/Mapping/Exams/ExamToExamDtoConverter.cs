using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping.Exams
{
    public class ExamToExamDtoConverter : ITypeConverter<Exam, ExamDto>
    {
        public ExamDto Convert(Exam src, ExamDto dst, ResolutionContext ctx)
        {
            dst.Course = ctx.Mapper.Map<CourseDto>(src.Course);
            dst.Duration = src.Duration;
            dst.Id = src.Id;
            dst.IsApproved = src.IsApproved;
            dst.Name = src.Name;
            dst.Semester = src.Semester;
            dst.Volunteer = ctx.Mapper.Map<UserMetadataDto>(src.Volunteer);
            dst.Year = src.Year;
            dst.Questions = src.SubQuestions.GroupBy(sq => sq.SubQuestion.Question).Select(g => new ExamQuestionDto
            {
                Id = g.Key.Id,
                Title = g.Key.Title,
                Content = g.Key.Content,
                SubQuestions = g.Select(sq => new ExamSubQuestionDto
                {
                    SubQuestion = ctx.Mapper.Map<SubQuestionMetadataDto>(sq.SubQuestion),
                    Weight = sq.Weight
                }).ToArray()
            })
                .ToArray();
            return dst;
        }
    }
}
