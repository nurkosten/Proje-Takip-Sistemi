using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using System.ComponentModel.DataAnnotations;

namespace ProjeHavuzu.MVCUI.Models
{
    public class AdminAddAcademicianViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "Kurumsal E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sicil numarası zorunludur.")]
        [Display(Name = "Sicil Numarası")]
        public string StaffNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Akademik unvan zorunludur.")]
        [Display(Name = "Akademik Unvan")]
        public string AcademicTitle { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Fakülte")]
        public Guid? FacultyId { get; set; }

        [Display(Name = "Bölüm")]
        public Guid? DepartmentId { get; set; }

        [Required(ErrorMessage = "Geçici şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Geçici Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<FacultyListDto> Faculties { get; set; } = new();
        public List<DepartmentListDto> Departments { get; set; } = new();

        public static List<string> AcademicTitleOptions { get; } = new()
        {
            "Araştırma Görevlisi",
            "Öğretim Görevlisi",
            "Dr. Öğretim Üyesi",
            "Doçent",
            "Profesör"
        };
    }
}
