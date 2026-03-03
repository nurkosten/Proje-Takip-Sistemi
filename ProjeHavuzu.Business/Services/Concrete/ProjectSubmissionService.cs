
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Repository.Abstract;
using ProjeHavuzu.Data.Repository.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class ProjectSubmissionService : IProjectSubmissionService
    {
        private readonly IProjectSubmissionRepository _submissionRepository;
        private readonly string _uploadPath;

        // Dependency: Eğer IProjectSubmissionRepository servisi kayıtlı değilse program.cs eklemeliyiz
        public ProjectSubmissionService(IProjectSubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;

            // Güvenli Klasör: Uygulamanın root'unda wwwroot dışında
            // Bu klasöre web erişimi yok
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Projects");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<bool> UploadSubmissionAsync(Guid studentId, Stream fileStream, string fileName)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Dosya içeriği boş.");

            if (fileStream.Length > 100 * 1024 * 1024) // 100MB
                throw new ArgumentException("Dosya boyutu 100MB'ı aşamaz.");

            // Uzantı kontrolü
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (extension != ".zip")
                throw new ArgumentException("Sadece .zip uzantılı dosyalar yüklenebilir.");

            // Güvenli dosya adı: GUID + .zip
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, storedFileName);

            // Stream kopyalama (Performanslı)
            using (var targetStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(targetStream);
            }

            // Veritabanına kaydet
            var submission = new ProjectSubmission
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                FileName = fileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                FileSize = new FileInfo(filePath).Length,
                UploadDate = DateTime.Now,
                SubmissionStatus = SubmissionStatus.Pending
            };

            await _submissionRepository.AddAsync(submission);
            return true;
        }

        public async Task<List<ProjectSubmission>> GetSubmissionsByStudentIdAsync(Guid studentId)
        {
            var all = await _submissionRepository.ListAsync(x => x.StudentId == studentId);
            return all.OrderByDescending(x => x.UploadDate).ToList();
        }

        public async Task<List<ProjectSubmission>> GetAllSubmissionsAsync()
        {
            var all = await _submissionRepository.GetAllAsync();
            return all.OrderByDescending(x => x.UploadDate).ToList();
        }

        public async Task<ProjectSubmission> GetSubmissionByIdAsync(Guid id)
        {
            return await _submissionRepository.GetAsync(x => x.Id == id);
        }

        public async Task<bool> ApproveSubmissionAsync(Guid id)
        {
            var submission = await _submissionRepository.GetAsync(x => x.Id == id);
            if (submission == null) return false;

            submission.SubmissionStatus = SubmissionStatus.Approved;
            _submissionRepository.Update(submission);
            return true;
        }

        public async Task<bool> RejectSubmissionAsync(Guid id)
        {
            var submission = await _submissionRepository.GetAsync(x => x.Id == id);
            if (submission == null) return false;

            submission.SubmissionStatus = SubmissionStatus.Rejected;
            _submissionRepository.Update(submission);
            return true;
        }

        public async Task<bool> DeleteSubmissionAsync(Guid id)
        {
            var submission = await _submissionRepository.GetAsync(x => x.Id == id);
            if (submission == null) return false;

            // Dosyayı sistemden sil
            if (File.Exists(submission.FilePath))
            {
                File.Delete(submission.FilePath);
            }

            _submissionRepository.Remove(submission);
            return true;
        }

        public async Task<(Stream fileStream, string contentType, string fileName)> GetFileDownloadAsync(Guid id)
        {
            var submission = await _submissionRepository.GetAsync(x => x.Id == id);
            if (submission == null) throw new FileNotFoundException("Dosya kaydı bulunamadı.");

            if (!File.Exists(submission.FilePath))
                throw new FileNotFoundException("Dosya sunucuda bulunamadı.");

            var stream = new FileStream(submission.FilePath, FileMode.Open, FileAccess.Read);
            return (stream, "application/zip", submission.FileName);
        }
    }
}
