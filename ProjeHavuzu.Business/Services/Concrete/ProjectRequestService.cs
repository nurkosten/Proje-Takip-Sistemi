using Microsoft.AspNetCore.Identity;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class ProjectRequestService : IProjectRequestService
    {
        private readonly IProjectRequestRepository _projectRequestRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectStudentRepository _projectStudentRepository;
        private readonly UserManager<AppUser> _userManager;

        public ProjectRequestService(
            IProjectRequestRepository projectRequestRepository,
            IProjectRepository projectRepository,
            IProjectStudentRepository projectStudentRepository,
            UserManager<AppUser> userManager)
        {
            _projectRequestRepository = projectRequestRepository;
            _projectRepository = projectRepository;
            _projectStudentRepository = projectStudentRepository;
            _userManager = userManager;
        }

        public async Task<bool> SendRequestAsync(Guid projectId, Guid studentId, string? message)
        {
            // Mevcut istek kontrolü
            var existingRequest = await _projectRequestRepository.GetExistingRequestAsync(projectId, studentId);
            if (existingRequest != null)
            {
                // Eğer beklemede bir istek varsa, yenisini gönderme
                if (existingRequest.RequestStatus == ProjectRequestStatus.Pending)
                    return false;
            }

            // Yeni istek oluştur
            var request = new ProjectRequest
            {
                ProjectId = projectId,
                StudentId = studentId,
                Message = message,
                RequestStatus = ProjectRequestStatus.Pending,
                RequestDate = DateTime.Now
            };

            await _projectRequestRepository.AddAsync(request);
            return true;
        }

        public async Task<bool> ApproveRequestAsync(Guid requestId, string? responseMessage)
        {
            var request = await _projectRequestRepository.GetAsync(r => r.Id == requestId && !r.IsDeleted);
            if (request == null)
                return false;

            request.RequestStatus = ProjectRequestStatus.Approved;
            request.ResponseDate = DateTime.Now;
            request.ResponseMessage = responseMessage;
            _projectRequestRepository.Update(request);

            // Öğrenciyi projeye ata
            var projectStudent = new ProjectStudent
            {
                ProjectId = request.ProjectId,
                StudentId = request.StudentId
            };
            await _projectStudentRepository.AddAsync(projectStudent);

            return true;
        }

        public async Task<bool> RejectRequestAsync(Guid requestId, string? responseMessage)
        {
            var request = await _projectRequestRepository.GetAsync(r => r.Id == requestId && !r.IsDeleted);
            if (request == null)
                return false;

            request.RequestStatus = ProjectRequestStatus.Rejected;
            request.ResponseDate = DateTime.Now;
            request.ResponseMessage = responseMessage;
            _projectRequestRepository.Update(request);

            return true;
        }

        public async Task<List<ProjectRequest>> GetRequestsByStudentIdAsync(Guid studentId)
        {
            return await _projectRequestRepository.GetRequestsByStudentIdAsync(studentId);
        }

        public async Task<List<ProjectRequest>> GetPendingRequestsForConsultantAsync(Guid consultantId)
        {
            return await _projectRequestRepository.GetPendingRequestsForConsultantAsync(consultantId);
        }

        public async Task<List<ProjectRequest>> GetAllPendingRequestsAsync()
        {
            return await _projectRequestRepository.GetAllPendingRequestsAsync();
        }

        public async Task<bool> HasPendingRequestAsync(Guid projectId, Guid studentId)
        {
            var existingRequest = await _projectRequestRepository.GetExistingRequestAsync(projectId, studentId);
            return existingRequest != null && existingRequest.RequestStatus == ProjectRequestStatus.Pending;
        }

        public async Task<ProjectRequest?> GetRequestByIdAsync(Guid requestId)
        {
            return await _projectRequestRepository.GetAsync(r => r.Id == requestId && !r.IsDeleted);
        }
    }
}
