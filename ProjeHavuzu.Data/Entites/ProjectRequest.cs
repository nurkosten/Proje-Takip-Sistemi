using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;

namespace ProjeHavuzu.Data.Entites
{
    public class ProjectRequest : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid StudentId { get; set; }
        public string? Message { get; set; } // Öğrencinin isteğe eklediği mesaj
        public ProjectRequestStatus RequestStatus { get; set; } = ProjectRequestStatus.Pending;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ResponseDate { get; set; }
        public string? ResponseMessage { get; set; } // Danışmanın yanıt mesajı

        // Navigation properties
        public virtual Project? Project { get; set; }
        public virtual AppUser? Student { get; set; }
    }

    public enum ProjectRequestStatus
    {
        Pending = 0,    // Beklemede
        Approved = 1,   // Onaylandı
        Rejected = 2    // Reddedildi
    }
}
