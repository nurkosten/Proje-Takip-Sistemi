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
                .NotEmpty().WithMessage("Proje başlığı boş olamaz.")
                .NotNull().WithMessage("Proje başlığı boş olamaz.")
                .MinimumLength(3).WithMessage("Proje başlığı en az 3 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Proje başlığı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Proje açıklaması boş olamaz.")
                .NotNull().WithMessage("Proje açıklaması boş olamaz.")
                .MinimumLength(10).WithMessage("Proje açıklaması en az 10 karakter olmalıdır.")
                .MaximumLength(2000).WithMessage("Proje açıklaması en fazla 2000 karakter olabilir.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori seçimi zorunludur.")
                .NotEqual(Guid.Empty).WithMessage("Geçerli bir kategori seçmelisiniz.");

            RuleFor(x => x.DifficultyLevel)
                .IsInEnum().WithMessage("Geçersiz zorluk seviyesi seçildi.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.")
                .GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.")
                .GreaterThanOrEqualTo(new DateTime(2026, 1, 1)).WithMessage("Bitiş tarihi 2026 yılından önce olamaz.");

            RuleFor(x => x.ProjectLink)
                .NotEmpty().WithMessage("Proje linki boş olamaz.")
                .NotNull().WithMessage("Proje linki boş olamaz.")
                .MaximumLength(500).WithMessage("Proje linki en fazla 500 karakter olabilir.")
                .Must(BeValidUrl).WithMessage("Geçerli bir URL formatı giriniz.");
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
