using FluentValidation;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Validators.ProjectValidators
{
    public class ProjectCreateValidator : AbstractValidator<ProjectCreateDto>
    {
        public ProjectCreateValidator()
        {

            RuleFor(x => x.ProjectTitle)
                .NotEmpty().WithMessage("Project title is required.")
                .MaximumLength(100).WithMessage("Project title must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");
            RuleFor(x => x.DifficultyLevel)
                .IsInEnum().WithMessage("Invalid difficulty level.");
            RuleFor(x => x.EndTime)
                .GreaterThan(0).WithMessage("End time must be greater than zero.");
            RuleFor(x => x.ProjetLink)
                .NotEmpty().WithMessage("Project link is required.")
                .MaximumLength(250).WithMessage("Project link must not exceed 200 characters.");
        }
    }
}
