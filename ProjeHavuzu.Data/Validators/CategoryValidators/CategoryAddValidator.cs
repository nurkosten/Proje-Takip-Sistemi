using FluentValidation;
using ProjeHavuzu.Data.DTOs.CategoryDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Validators.CategoryValidators
{
    public class CategoryAddValidator : AbstractValidator<CategoryAddDto>
    {
        public CategoryAddValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .NotNull().WithMessage("Kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Kategori adı sadece harf ve boşluk içerebilir.");
        }
    }
}
