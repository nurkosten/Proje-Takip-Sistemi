using FluentValidation;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Validators.DepartmentValidators
{
    public class DepartmentListValidator : AbstractValidator<DepartmentListDto>
    {
        public DepartmentListValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Bölüm adı boş olamaz.")
                .NotNull().WithMessage("Bölüm adı boş olamaz.")
                .MinimumLength(2).WithMessage("Bölüm adı en az 2 karakter olmalıdır.")
                .MaximumLength(150).WithMessage("Bölüm adı en fazla 150 karakter olabilir.")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Bölüm adı sadece harf ve boşluk içerebilir.");

            RuleFor(x => x.FacultyName)
                .NotEmpty().WithMessage("Fakülte adı boş olamaz.")
                .NotNull().WithMessage("Fakülte adı boş olamaz.")
                .MinimumLength(2).WithMessage("Fakülte adı en az 2 karakter olmalıdır.")
                .MaximumLength(150).WithMessage("Fakülte adı en fazla 150 karakter olabilir.");
        }
    }
}
