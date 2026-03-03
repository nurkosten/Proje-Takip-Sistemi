
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IProjectSubmissionService
    {
        // Dosya yükleme (Steam ile)
        Task<bool> UploadSubmissionAsync(Guid studentId, Stream fileStream, string fileName);

        // Submissions Listesi (Server-side Pagination için DTO'lar kullanılmalı ama şimdilik Entity listesi veya basit bir DTO)
        Task<List<ProjectSubmission>> GetSubmissionsByStudentIdAsync(Guid studentId);

        // Admin Liste
        Task<List<ProjectSubmission>> GetAllSubmissionsAsync();

        // Dosyayı bulma (İndirmek için)
        Task<ProjectSubmission> GetSubmissionByIdAsync(Guid id);

        // Onay/Red
        Task<bool> ApproveSubmissionAsync(Guid id);
        Task<bool> RejectSubmissionAsync(Guid id);
        Task<bool> DeleteSubmissionAsync(Guid id);

        // Dosya indirme (FilePath döner veya Stream)
        Task<(Stream fileStream, string contentType, string fileName)> GetFileDownloadAsync(Guid id);
    }
}
