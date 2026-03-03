
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeHavuzu.Data.Entites
{
    // BaseEntity'den miras almalı
    public class ProjectSubmission : BaseEntity
    {
        // BaseEntity'de Id zaten var, kaldırdık.

        /* [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); */

        // Veritabanı tasarımında StudentId'nin AppUser ile ilişkisi
        [Required]
        public Guid StudentId { get; set; }

        // Orijinal dosya adı (Kullanıcının yüklediği isim)
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        // Sunucuda saklanan benzersiz isim (GUID)
        [Required]
        [StringLength(255)]
        public string StoredFileName { get; set; }

        // Fiziksel dosya yolu (wwwroot dışında)
        [Required]
        public string FilePath { get; set; }

        // Dosya boyutu (bytes)
        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        // BaseEntity'de Status var ama DataStatus enum. Biz SubmissionStatus istiyoruz.
        // BaseEntity.Status özelliğini kullanmak yerine yeni bir status alanı tanımlayalım veya mapeleyelim.
        // Ancak BaseEntity zorunlu kılındığı için DataStatus Status {get;set;} gelecek.
        // Biz iş mantığında SubmissionStatus kullanacağız.

        public SubmissionStatus SubmissionStatus { get; set; } = SubmissionStatus.Pending;

        // Navigation Property (Opsiyonel - Eğer AppUser entities klasöründe ise)
        // [ForeignKey("StudentId")]
        // public virtual Identity.AppUser Student { get; set; }
    }
}
