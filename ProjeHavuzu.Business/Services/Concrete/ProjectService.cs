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

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ProjectCreateDto> _validator;

        public ProjectService(
            IProjectRepository projectRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IValidator<ProjectCreateDto> validator)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _validator = validator;
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

            // Business rules
            if (project.EndTime <= 0)
            {
                throw new ArgumentException("Proje tamamlanma süresi 0'dan büyük olmalıdır.");
            }

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
            existingProject.DifficultyLevel = projectCreateDto.DifficultyLevel;
            existingProject.EndTime = projectCreateDto.EndTime;
            existingProject.ProjectLink = projectCreateDto.ProjectLink;
            existingProject.AppUserId = projectCreateDto.AppUserId;

            if (existingProject.EndTime <= 0)
            {
                throw new ArgumentException("Proje tamamlanma süresi 0'dan büyük olmalıdır.");
            }

            _projectRepository.Update(existingProject);
            return true;
        }

        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == id);
            if (project == null)
            {
                throw new ArgumentException("Proje bulunamadı.");
            }

            _projectRepository.Remove(project);
            return true;
        }

        public async Task<bool> SoftDeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == id && !p.IsDeleted);
            if (project == null)
            {
                throw new ArgumentException("Proje bulunamadı.");
            }

            project.IsDeleted = true;
            project.IsActive = false;
            _projectRepository.Update(project);
            return true;
        }
    }
}
