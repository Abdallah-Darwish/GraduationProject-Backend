using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.ExamSubQuestions;
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
        //todo: check me
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
            dst.Questions = src.Questions.Select(q => new ExamQuestionDto
            {
                Id = q.Id,
                Title = q.Question.Title,
                Content = q.Question.Content,
                QuestionId = q.QuestionId,
                ExamSubQuestions = q.ExamSubQuestions.Select(sq => new ExamSubQuestionDto
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
