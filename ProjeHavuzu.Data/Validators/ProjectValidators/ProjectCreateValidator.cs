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
                .NotEmpty().WithMessage("Proje başlığı alanı zorunludur.")
                .NotNull().WithMessage("Proje başlığı alanı zorunludur.")
                .MinimumLength(3).WithMessage("Proje başlığı en az 3 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Proje başlığı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Proje açıklaması alanı zorunludur.")
                .NotNull().WithMessage("Proje açıklaması alanı zorunludur.")
                .MinimumLength(10).WithMessage("Proje açıklaması en az 10 karakter olmalıdır.")
                .MaximumLength(2000).WithMessage("Proje açıklaması en fazla 2000 karakter olabilir.");

            // Kategori veya Özel Kategori'den en az biri dolu olmalı (Gerçi serviste Genel'e düşüyor ama formda zorunlu yapalım)
            RuleFor(x => x.CustomCategory)
                .NotEmpty().When(x => !x.CategoryId.HasValue || x.CategoryId == Guid.Empty)
                .WithMessage("Lütfen bir kategori adı giriniz.");

            RuleFor(x => x.ConsultantId)
                .NotEmpty().WithMessage("Lütfen bir danışman seçiniz.")
                .NotEqual(Guid.Empty).WithMessage("Lütfen listeden geçerli bir danışman seçiniz.");

            RuleFor(x => x.DifficultyLevel)
                .NotNull().WithMessage("Lütfen bir zorluk seviyesi seçiniz.")
                .IsInEnum().WithMessage("Geçersiz zorluk seviyesi.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Bitiş tarihi zorunludur.")
                .GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi, başlangıç tarihinden sonraki bir tarih olmalıdır.")
                .GreaterThanOrEqualTo(new DateTime(2025, 1, 1)).WithMessage("Bitiş tarihi geçerli bir yıl olmalıdır.");

            RuleFor(x => x.ProjectLink)
                .MaximumLength(500).WithMessage("Proje linki en fazla 500 karakter olabilir.")
                .Must(url => string.IsNullOrEmpty(url) || BeValidUrl(url)).WithMessage("Geçerli bir web adresi (http/https) giriniz.");

            RuleFor(x => x.PhaseNames)
                .Must(x => x != null && x.Count > 0 && x.Any(n => !string.IsNullOrWhiteSpace(n)))
                .WithMessage("Proje yol haritası (fazlar) boş olamaz. En az bir aşama eklemelisiniz.");
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
