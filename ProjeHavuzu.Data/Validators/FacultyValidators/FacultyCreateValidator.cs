using FluentValidation;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Validators.FacultyValidators
{
    public class FacultyCreateValidator : AbstractValidator<FacultyCreateDto>
    {
        public FacultyCreateValidator()
        {
            RuleFor(x => x.FacultyName)
                .NotEmpty().WithMessage("Fakülte adı boş olamaz.")
                .NotNull().WithMessage("Fakülte adı boş olamaz.")
                .MinimumLength(2).WithMessage("Fakülte adı en az 2 karakter olmalıdır.")
                .MaximumLength(150).WithMessage("Fakülte adı en fazla 150 karakter olabilir.")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Fakülte adı sadece harf ve boşluk içerebilir.");
        }
    }
}
