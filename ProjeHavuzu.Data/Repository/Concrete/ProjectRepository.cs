using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationContext _context;

        public ProjectRepository(ApplicationContext context, DbSet<Project> dbSet = null, ICurrentUserService currentUser = null) : base(context, dbSet)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ProjectListDto>> GetAllProjectsByCategoryAsync()
        {
            var result =
                from p in _context.Projects
                where !p.IsDeleted
                join c in _context.Categories
                    on p.CategoryId equals c.Id
                where !c.IsDeleted
                join u in _context.Users
                    on p.CreatedBy equals u.Id into userGroup
                from u in userGroup.DefaultIfEmpty()
                orderby p.CreatedDate descending
                select new ProjectListDto
                {
                    Id = p.Id,
                    ProjectTitle = p.ProjectTitle,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    DifficultyLevel = p.DifficultyLevel,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink,
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    CreatedByFullName = u != null
                        ? u.FirstName + " " + u.LastName
                        : "Unknown",
                    TotalPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted) : 0,
                    CompletedPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted && ph.IsCompleted) : 0
                };
            return await result.ToListAsync();
        }

        public async Task<List<ProjectListDto>> GetDeletedProjectsAsync()
        {
            var result =
                from p in _context.Projects
                where p.IsDeleted
                join c in _context.Categories
                    on p.CategoryId equals c.Id
                join u in _context.Users
                    on p.CreatedBy equals u.Id into userGroup
                from u in userGroup.DefaultIfEmpty()
                orderby p.CreatedDate descending
                select new ProjectListDto
                {
                    Id = p.Id,
                    ProjectTitle = p.ProjectTitle,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    IsDeleted = p.IsDeleted,
                    DifficultyLevel = p.DifficultyLevel,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink,
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    CreatedByFullName = u != null ? u.FirstName + " " + u.LastName : "Unknown"
                };
            return await result.ToListAsync();
        }

        /// <summary>
        /// Server-side DataTables: Sadece istenen sayfa kadar veri çekilir.
        /// AsNoTracking + IQueryable + SQL seviyesinde filtreleme/sıralama/sayfalama.
        /// </summary>
        public async Task<DataTableResponse<ProjectListDto>> GetProjectsServerSideAsync(DataTableRequest request)
        {
            // 1. Base IQueryable - AsNoTracking ile tracking kapalı
            var baseQuery =
                from p in _context.Projects.AsNoTracking()
                where !p.IsDeleted
                join c in _context.Categories.AsNoTracking()
                    on p.CategoryId equals c.Id
                where !c.IsDeleted
                join u in _context.Users.AsNoTracking()
                    on p.CreatedBy equals u.Id into userGroup
                from u in userGroup.DefaultIfEmpty()
                select new
                {
                    p.Id,
                    p.ProjectTitle,
                    p.Description,
                    p.CreatedDate,
                    p.DifficultyLevel,
                    p.StartDate,
                    p.EndDate,
                    p.ProjectLink,
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    CreatedByFullName = u != null ? u.FirstName + " " + u.LastName : "Unknown",
                    TotalPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted) : 0,
                    CompletedPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted && ph.IsCompleted) : 0
                };

            // 2. Toplam kayıt sayısı (filtrelenmemiş)
            var totalRecords = await baseQuery.CountAsync();

            // 3. Arama filtresi
            var searchValue = request.Search?.Value?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchValue))
            {
                baseQuery = baseQuery.Where(p =>
                    p.ProjectTitle.ToLower().Contains(searchValue) ||
                    p.Description.ToLower().Contains(searchValue) ||
                    p.CategoryName.ToLower().Contains(searchValue) ||
                    p.CreatedByFullName.ToLower().Contains(searchValue)
                );
            }

            // 4. Filtrelenmiş kayıt sayısı
            var filteredRecords = await baseQuery.CountAsync();

            // 5. Sıralama
            if (request.Order != null && request.Order.Any() && request.Columns != null)
            {
                var orderColumn = request.Order[0];
                var columnName = request.Columns.Count > orderColumn.Column
                    ? request.Columns[orderColumn.Column].Data
                    : "createdDate";
                var isDesc = orderColumn.Dir == "desc";

                baseQuery = columnName?.ToLower() switch
                {
                    "projecttitle" => isDesc ? baseQuery.OrderByDescending(p => p.ProjectTitle) : baseQuery.OrderBy(p => p.ProjectTitle),
                    "categoryname" => isDesc ? baseQuery.OrderByDescending(p => p.CategoryName) : baseQuery.OrderBy(p => p.CategoryName),
                    "difficultylevel" => isDesc ? baseQuery.OrderByDescending(p => p.DifficultyLevel) : baseQuery.OrderBy(p => p.DifficultyLevel),
                    "enddate" => isDesc ? baseQuery.OrderByDescending(p => p.EndDate) : baseQuery.OrderBy(p => p.EndDate),
                    "createdbyfullname" => isDesc ? baseQuery.OrderByDescending(p => p.CreatedByFullName) : baseQuery.OrderBy(p => p.CreatedByFullName),
                    _ => baseQuery.OrderByDescending(p => p.CreatedDate)
                };
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(p => p.CreatedDate);
            }

            // 6. Sayfalama ve Projection
            var data = await baseQuery
                .Skip(request.Start)
                .Take(request.Length)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    ProjectTitle = p.ProjectTitle,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    DifficultyLevel = p.DifficultyLevel,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink,
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName,
                    CreatedByFullName = p.CreatedByFullName,
                    TotalPhasesCount = p.TotalPhasesCount,
                    CompletedPhasesCount = p.CompletedPhasesCount
                })
                .ToListAsync();

            return new DataTableResponse<ProjectListDto>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        public async Task<ProjectDetailDto> GetProjectDetailWithPhasesAsync(Guid projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Phases.Where(ph => !ph.IsDeleted).OrderBy(ph => ph.Order))
                .Include(p => p.Languages)
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                return null!;

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == project.CategoryId);
            var createdByUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == project.CreatedBy);
            var consultant = project.ConsultantId.HasValue
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == project.ConsultantId.Value)
                : null;

            return new ProjectDetailDto
            {
                Id = project.Id,
                ProjectTitle = project.ProjectTitle,
                Description = project.Description,
                DifficultyLevel = project.DifficultyLevel,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectLink = project.ProjectLink,
                CategoryId = project.CategoryId,
                CategoryName = category?.CategoryName ?? "Bilinmiyor",
                CreatedDate = project.CreatedDate,
                CreatedByFullName = createdByUser != null ? $"{createdByUser.FirstName} {createdByUser.LastName}" : "Bilinmiyor",
                ConsultantFullName = consultant != null ? $"{consultant.FirstName} {consultant.LastName}" : null,
                ProjectArea = project.ProjectArea,
                InitialCode = project.InitialCode,
                IsActive = project.IsActive,
                IsDeleted = project.IsDeleted,
                Phases = project.Phases?.Select(ph => new ProjectPhaseDto
                {
                    Id = ph.Id,
                    ProjectId = ph.ProjectId,
                    PhaseName = ph.PhaseName,
                    Description = ph.Description,
                    Order = ph.Order,
                    IsCompleted = ph.IsCompleted,
                    CompletedDate = ph.CompletedDate
                }).ToList() ?? new List<ProjectPhaseDto>(),
                Languages = project.Languages?.Select(l => l.LanguageName).ToList() ?? new List<string>()
            };
        }
    }
}
