﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Exams
{
    public class CreateExamSubQuestionDtoValidator : AbstractValidator<CreateExamSubQuestionDto>
    {
        public CreateExamSubQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Weight)
                .GreaterThan(0.0f);
            RuleFor(d => d.QuestionId)
                .MustAsync(async (id, _) =>
                {
                    var subQuestion = await dbContext.SubQuestions.FindAsync(id).ConfigureAwait(false);
                    return subQuestion != null && subQuestion.Question.IsApproved;
                })
                .WithMessage("SubQuestion(Id: {PropertyValue}) doesn't exist or isn't approved yet.");
        }
    }
}