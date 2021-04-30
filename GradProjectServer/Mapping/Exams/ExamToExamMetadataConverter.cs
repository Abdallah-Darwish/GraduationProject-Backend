using System;
using System.Linq;
using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping.Exams
{
    public class ExamToExamMetadataConverter : ITypeConverter<Exam, ExamMetadataDto>
    {
        public ExamMetadataDto Convert(Exam src, ExamMetadataDto dst, ResolutionContext ctx)
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
                Year = src.Year
            };
            return dst;
        }
    }
}