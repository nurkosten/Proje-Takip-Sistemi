using AutoMapper;
using FluentValidation;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
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

        public ProjectService(
            IProjectRepository projectRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IValidator<ProjectCreateDto> validator,
            IProjectStudentRepository projectStudentRepository)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _validator = validator;
            _projectStudentRepository = projectStudentRepository;
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
            // Validation
            var validationResult = await _validator.ValidateAsync(projectCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Category kontrolü
            var category = await _categoryRepository.GetAsync(c => c.Id == projectCreateDto.CategoryId && !c.IsDeleted);
            if (category == null)
            {
                throw new ArgumentException("Geçersiz kategori seçildi.");
            }

            // Entity mapping
            var project = _mapper.Map<Project>(projectCreateDto);
            await _projectRepository.AddAsync(project);
            return true;
        }

        public async Task<bool> UpdateProjectAsync(Guid id, ProjectCreateDto projectCreateDto)
        {
            var existingProject = await _projectRepository.GetAsync(p => p.Id == id && !p.IsDeleted);
            if (existingProject == null)
            {
                throw new ArgumentException("Proje bulunamadı.");
            }

            // Validation
            var validationResult = await _validator.ValidateAsync(projectCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Category kontrolü
            var category = await _categoryRepository.GetAsync(c => c.Id == projectCreateDto.CategoryId && !c.IsDeleted);
            if (category == null)
            {
                throw new ArgumentException("Geçersiz kategori seçildi.");
            }

            // Update mapping
            existingProject.ProjectTitle = projectCreateDto.ProjectTitle;
            existingProject.Description = projectCreateDto.Description;
            existingProject.CategoryId = projectCreateDto.CategoryId;
            existingProject.DifficultyLevel = projectCreateDto.DifficultyLevel.Value;
            existingProject.StartDate = projectCreateDto.StartDate;
            existingProject.EndDate = projectCreateDto.EndDate;
            existingProject.ProjectLink = projectCreateDto.ProjectLink;
            existingProject.AppUserId = projectCreateDto.AppUserId;

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
               if(deletedProject != null)
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
    }
}
