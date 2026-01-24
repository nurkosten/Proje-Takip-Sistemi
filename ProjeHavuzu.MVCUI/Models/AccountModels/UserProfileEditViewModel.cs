using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjeHavuzu.MVCUI.Models.AccountModels
{
    public class UserProfileEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Öğrenci Numarası")]
        public string? StudentNumber { get; set; }

        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Fakülte")]
        public Guid? FacultyId { get; set; }

        [Display(Name = "Bölüm")]
        public Guid? DepartmentId { get; set; }

        // Dropdown listeleri
        public List<FacultyListDto>? Faculties { get; set; }
        public List<DepartmentListDto>? Departments { get; set; }
    }
}
