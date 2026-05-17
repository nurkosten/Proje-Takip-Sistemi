using AutoMapper;
using FluentValidation;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Repository.Abstract;
using ProjeHavuzu.Data.Validators.ProjectValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ProjectCreateDto> _validator;
        private readonly IProjectStudentRepository _projectStudentRepository;
        private readonly IProjectPhaseRepository _projectPhaseRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IValidator<ProjectCreateDto> validator,
            IProjectStudentRepository projectStudentRepository,
            IProjectPhaseRepository projectPhaseRepository)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _validator = validator;
            _projectStudentRepository = projectStudentRepository;
            _projectPhaseRepository = projectPhaseRepository;
        }

        public async Task<List<ProjectListDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllProjectsByCategoryAsync();
            return projects;
        }

        public async Task<ProjectListDto> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == id && !p.IsDeleted);
            if (project == null)
                return null;

            var projectList = await _projectRepository.GetAllProjectsByCategoryAsync();
            return projectList.FirstOrDefault(p => p.Id == id);
        }

        public async Task<ProjectCreateDto> GetProjectCreateDtoAsync()
        {
            var categories = await _categoryRepository.ListAsync(c => !c.IsDeleted && c.IsActive);
            return new ProjectCreateDto
            {
                Categories = categories
            };
        }

        public async Task<bool> CreateProjectAsync(ProjectCreateDto projectCreateDto)
        {
            // 1. Kategori Belirleme
            Guid finalCategoryId = await GetOrCreateCategoryIdAsync(projectCreateDto.CustomCategory, projectCreateDto.CategoryId);

            projectCreateDto.CategoryId = finalCategoryId;

            // Validation
            var validationResult = await _validator.ValidateAsync(projectCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Kategori kontrolü (Zaten yukarıda hallettik, ID geçerli)

            // Entity mapping
            var project = _mapper.Map<Project>(projectCreateDto);

            // Yeni Alanların Elle Eşlenmesi (AutoMapper güncellenene kadar)
            project.InitialCode = projectCreateDto.InitialCode;
            project.ProjectArea = projectCreateDto.ProjectArea;
            project.ConsultantId = projectCreateDto.ConsultantId;
            project.ApprovalStatus = ProjectApprovalStatus.Pending;
            project.RejectionReason = null;
            project.ApprovedAt = null;
            project.RejectedAt = null;

            // Fazların Eklenmesi
            if (projectCreateDto.PhaseNames != null && projectCreateDto.PhaseNames.Any())
            {
                project.Phases = new List<ProjectPhase>();
                for (int i = 0; i < projectCreateDto.PhaseNames.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(projectCreateDto.PhaseNames[i]))
                    {
                        var description = "";
                        if (projectCreateDto.PhaseDescriptions != null && i < projectCreateDto.PhaseDescriptions.Count)
                        {
                            description = projectCreateDto.PhaseDescriptions[i] ?? "";
                        }

                        project.Phases.Add(new ProjectPhase
                        {
                            PhaseName = projectCreateDto.PhaseNames[i],
                            Description = description,
                            Order = i + 1
                        });
                    }
                }
            }

            // Dillerin Eklenmesi (Virgülle ayrılmış string'den)
            if (!string.IsNullOrEmpty(projectCreateDto.SelectedLanguagesJson))
            {
                project.Languages = new List<ProjectLanguage>();
                var langs = projectCreateDto.SelectedLanguagesJson.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var lang in langs)
                {
                    project.Languages.Add(new ProjectLanguage
                    {
                        LanguageName = lang.Trim()
                    });
                }
            }

            await _projectRepository.AddAsync(project);

            if (projectCreateDto.AppUserId.HasValue && projectCreateDto.AppUserId != Guid.Empty)
            {
                var existingAssignment = await _projectStudentRepository.GetProjectStudentAsync(project.Id, projectCreateDto.AppUserId.Value);
                if (existingAssignment == null)
                {
                    await _projectStudentRepository.AddAsync(new ProjectStudent
                    {
                        ProjectId = project.Id,
                        StudentId = projectCreateDto.AppUserId.Value
                    });
                }
            }

            return true;
        }

        public async Task<bool> UpdateProjectAsync(Guid id, ProjectCreateDto projectCreateDto)
        {
            var existingProject = await _projectRepository.GetAsync(p => p.Id == id && !p.IsDeleted);
            if (existingProject == null)
            {
                throw new ArgumentException("Proje bulunamadı.");
            }

            // 1. Kategori Belirleme
            Guid finalCategoryId = await GetOrCreateCategoryIdAsync(projectCreateDto.CustomCategory, projectCreateDto.CategoryId);

            projectCreateDto.CategoryId = finalCategoryId;

            // Validation
            var validationResult = await _validator.ValidateAsync(projectCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Update mapping
            existingProject.ProjectTitle = projectCreateDto.ProjectTitle ?? "";
            existingProject.Description = projectCreateDto.Description ?? "";
            existingProject.CategoryId = projectCreateDto.CategoryId ?? Guid.Empty;
            existingProject.DifficultyLevel = projectCreateDto.DifficultyLevel.GetValueOrDefault(Data.Entites.Enums.DifficultStatus.Kolay);
            existingProject.StartDate = projectCreateDto.StartDate.GetValueOrDefault(DateTime.Now);
            existingProject.EndDate = projectCreateDto.EndDate.GetValueOrDefault(DateTime.Now);
            existingProject.ProjectLink = projectCreateDto.ProjectLink;
            existingProject.AppUserId = projectCreateDto.AppUserId;
            existingProject.ConsultantId = projectCreateDto.ConsultantId;

            _projectRepository.Update(existingProject);
            return true;
        }

        // ARTIK BU METOD "ÇÖP KUTUSUNA TAŞI" (SOFT DELETE) İŞLEMİ YAPAR
        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            // Silinmiş olmayan projeyi bul (Geri Dönüşüm kutusunda değilse)
            var project = await _projectRepository.GetAsync(p => p.Id == id && !p.IsDeleted);

            // Eğer "SİLİNMİŞ" olanı bulup hard delete yapacaksak, onu aşağıda HardDelete'de yapacağız.
            // Ama kullanıcı "Sil" dediğinde önce soft delete mi istiyor? Evet "Çöp Kutusuna Gönder".

            if (project == null)
            {
                // Belki zaten silinmiştir? Kontrol edelim.
                var deletedProject = await _projectRepository.GetAsync(p => p.Id == id && p.IsDeleted);
                if (deletedProject != null)
                {
                    throw new ArgumentException("Proje zaten çöp kutusunda.");
                }
                throw new ArgumentException("Proje bulunamadı.");
            }

            project.IsDeleted = true;
            project.IsActive = false;
            _projectRepository.Update(project);
            return true;
        }

        public async Task<bool> SoftDeleteProjectAsync(Guid id)
        {
            // Bu metod artık DeleteProjectAsync ile aynı işi yapıyor.
            return await DeleteProjectAsync(id);
        }

        #region Recycle Bin Actions

        public async Task<List<ProjectListDto>> GetDeletedProjectsAsync()
        {
            return await _projectRepository.GetDeletedProjectsAsync();
        }

        public async Task<bool> RestoreProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == id && p.IsDeleted);
            if (project == null)
            {
                throw new ArgumentException("Geri yüklenecek proje bulunamadı.");
            }

            project.IsDeleted = false;
            project.IsActive = true;
            _projectRepository.Update(project);
            return true;
        }

        // BU METOD "KALICI OLARAK SİL" İŞLEMİDİR (HARD DELETE)
        public async Task<bool> HardDeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == id); // Silinmiş veya silinmemiş fark etmez
            if (project == null)
            {
                throw new ArgumentException("Proje bulunamadı.");
            }

            // CASCADE DELETE: Önce ilişkili öğrenci atamalarını sil
            var projectStudents = await _projectStudentRepository.ListAsync(ps => ps.ProjectId == id);
            if (projectStudents != null)
            {
                foreach (var ps in projectStudents)
                {
                    _projectStudentRepository.Remove(ps);
                }
            }

            _projectRepository.Remove(project);
            return true;
        }
        #endregion

        #region Phase Management

        public async Task<ProjectDetailDto> GetProjectDetailAsync(Guid id)
        {
            return await _projectRepository.GetProjectDetailWithPhasesAsync(id);
        }

        public async Task<List<ProjectPhaseDto>> GetProjectPhasesAsync(Guid projectId)
        {
            var phases = await _projectPhaseRepository.GetPhasesByProjectIdAsync(projectId);
            return phases.Select(p => new ProjectPhaseDto
            {
                Id = p.Id,
                ProjectId = p.ProjectId,
                PhaseName = p.PhaseName,
                Description = p.Description,
                Order = p.Order,
                IsCompleted = p.IsCompleted,
                CompletedDate = p.CompletedDate
            }).ToList();
        }

        public async Task<bool> TogglePhaseCompletionAsync(Guid phaseId)
        {
            var phase = await _projectPhaseRepository.GetPhaseByIdAsync(phaseId);
            if (phase == null)
            {
                throw new ArgumentException("Aşama bulunamadı.");
            }

            phase.IsCompleted = !phase.IsCompleted;
            phase.CompletedDate = phase.IsCompleted ? DateTime.Now : null;

            _projectPhaseRepository.Update(phase);
            return true;
        }

        #endregion

        #region Server-side DataTables

        public async Task<DataTableResponse<ProjectListDto>> GetProjectsServerSideAsync(DataTableRequest request)
        {
            return await _projectRepository.GetProjectsServerSideAsync(request);
        }

        public async Task<List<ProjectListDto>> GetProjectsByConsultantIdAsync(Guid consultantId)
        {
            return await _projectRepository.GetProjectsByConsultantIdAsync(consultantId);
        }

        public async Task ApproveProjectAsync(Guid projectId, Guid advisorId, bool isAdmin = false)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == projectId && !p.IsDeleted);
            if (project == null)
                throw new ArgumentException("Proje bulunamadı.");

            if (!isAdmin && project.ConsultantId != advisorId)
                throw new UnauthorizedAccessException("Bu projeyi onaylama yetkiniz yok.");

            if (project.ApprovalStatus != ProjectApprovalStatus.Pending)
                throw new InvalidOperationException("Bu proje zaten işleme alınmış.");

            project.ApprovalStatus = ProjectApprovalStatus.Approved;
            project.ApprovedAt = DateTime.UtcNow;
            project.RejectedAt = null;
            project.RejectionReason = null;

            _projectRepository.Update(project);
        }

        public async Task RejectProjectAsync(Guid projectId, Guid advisorId, string rejectionReason, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new ArgumentException("Red açıklaması zorunludur.");

            var project = await _projectRepository.GetAsync(p => p.Id == projectId && !p.IsDeleted);
            if (project == null)
                throw new ArgumentException("Proje bulunamadı.");

            if (!isAdmin && project.ConsultantId != advisorId)
                throw new UnauthorizedAccessException("Bu projeyi reddetme yetkiniz yok.");

            if (project.ApprovalStatus != ProjectApprovalStatus.Pending)
                throw new InvalidOperationException("Bu proje zaten işleme alınmış.");

            project.ApprovalStatus = ProjectApprovalStatus.Rejected;
            project.RejectedAt = DateTime.UtcNow;
            project.RejectionReason = rejectionReason.Trim();
            project.ApprovedAt = null;

            _projectRepository.Update(project);
        }

        #endregion

        #region Private Methods

        private async Task<Guid> GetOrCreateCategoryIdAsync(string? customCategory, Guid? existingCategoryId)
        {
            if (!string.IsNullOrWhiteSpace(customCategory))
            {
                var existingCategory = await _categoryRepository.GetAsync(c => c.CategoryName.ToLower() == customCategory.ToLower());
                if (existingCategory != null) return existingCategory.Id;

                var newCategory = new Category
                {
                    Id = Guid.NewGuid(),
                    CategoryName = customCategory,
                    IsActive = true,
                    IsDeleted = false
                };
                await _categoryRepository.AddAsync(newCategory);
                return newCategory.Id;
            }

            if (existingCategoryId.HasValue && existingCategoryId.Value != Guid.Empty)
            {
                return existingCategoryId.Value;
            }

            var generalCat = await _categoryRepository.GetAsync(c => c.CategoryName == "Genel");
            if (generalCat != null) return generalCat.Id;

            var defaultCategory = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Genel",
                IsActive = true,
                IsDeleted = false
            };
            await _categoryRepository.AddAsync(defaultCategory);
            return defaultCategory.Id;
        }

        #endregion
    }
}
