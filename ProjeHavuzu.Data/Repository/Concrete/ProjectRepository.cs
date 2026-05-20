using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
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
                join consultant in _context.Users.AsNoTracking()
                    on p.ConsultantId equals consultant.Id into consultantGroup
                from consultant in consultantGroup.DefaultIfEmpty()
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
                    ConsultantFullName = consultant != null ? consultant.FirstName + " " + consultant.LastName : null,
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
                    p.CreatedByFullName.ToLower().Contains(searchValue) ||
                    (p.ConsultantFullName != null && p.ConsultantFullName.ToLower().Contains(searchValue))
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
                    "consultantfullname" => isDesc ? baseQuery.OrderByDescending(p => p.ConsultantFullName) : baseQuery.OrderBy(p => p.ConsultantFullName),
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
                    ConsultantFullName = p.ConsultantFullName,
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

        public async Task<ProjectListDto?> GetProjectListByIdAsync(Guid id)
        {
            return await BuildProjectListQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProjectListDto>> GetAllProjectsListAsync()
        {
            return await BuildProjectListQuery()
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<ProjectListDto>> GetProjectsByConsultantIdAsync(Guid consultantId)
        {
            return await BuildProjectListQuery()
                .Where(p => p.ConsultantId == consultantId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<ProjectListDto>> GetProjectsForStudentAsync(Guid studentId)
        {
            var assignedProjectIds = await _context.ProjectStudents
                .AsNoTracking()
                .Where(ps => ps.StudentId == studentId && !ps.IsDeleted)
                .Select(ps => ps.ProjectId)
                .ToListAsync();

            var studentProjectIds = await _context.Projects.AsNoTracking()
                .Where(p => !p.IsDeleted && (p.CreatedBy == studentId || p.AppUserId == studentId))
                .Select(p => p.Id)
                .ToListAsync();

            var allProjectIds = assignedProjectIds.Union(studentProjectIds).Distinct().ToList();

            return await BuildProjectListQuery()
                .Where(p => allProjectIds.Contains(p.Id))
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        private IQueryable<ProjectListDto> BuildProjectListQuery()
        {
            return
                from p in _context.Projects.AsNoTracking()
                where !p.IsDeleted
                join c in _context.Categories.AsNoTracking()
                    on p.CategoryId equals c.Id
                where !c.IsDeleted
                join creator in _context.Users.AsNoTracking()
                    on p.CreatedBy equals creator.Id into creatorGroup
                from creator in creatorGroup.DefaultIfEmpty()
                join consultant in _context.Users.AsNoTracking()
                    on p.ConsultantId equals consultant.Id into consultantGroup
                from consultant in consultantGroup.DefaultIfEmpty()
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
                    CreatedByFullName = creator != null
                        ? creator.FirstName + " " + creator.LastName
                        : "Bilinmiyor",
                    ConsultantId = p.ConsultantId,
                    ConsultantFullName = consultant != null
                        ? consultant.FirstName + " " + consultant.LastName
                        : null,
                    ApprovalStatus = p.ApprovalStatus,
                    RejectionReason = p.RejectionReason,
                    ApprovedAt = p.ApprovedAt,
                    RejectedAt = p.RejectedAt,
                    IsDeleted = p.IsDeleted,
                    IsActive = p.IsActive,
                    TotalPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted) : 0,
                    CompletedPhasesCount = p.Phases != null ? p.Phases.Count(ph => !ph.IsDeleted && ph.IsCompleted) : 0
                };
        }

        public async Task<ProjectDetailDto> GetProjectDetailWithPhasesAsync(Guid projectId)
        {
            var detail = await (
                from p in _context.Projects.AsNoTracking()
                where p.Id == projectId && !p.IsDeleted
                join c in _context.Categories.AsNoTracking() on p.CategoryId equals c.Id into categoryGroup
                from c in categoryGroup.DefaultIfEmpty()
                join creator in _context.Users.AsNoTracking() on p.CreatedBy equals creator.Id into creatorGroup
                from creator in creatorGroup.DefaultIfEmpty()
                join consultant in _context.Users.AsNoTracking() on p.ConsultantId equals consultant.Id into consultantGroup
                from consultant in consultantGroup.DefaultIfEmpty()
                select new ProjectDetailDto
                {
                    Id = p.Id,
                    ProjectTitle = p.ProjectTitle,
                    Description = p.Description,
                    DifficultyLevel = p.DifficultyLevel,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink,
                    CategoryId = p.CategoryId,
                    CategoryName = c != null ? c.CategoryName : "Bilinmiyor",
                    CreatedDate = p.CreatedDate,
                    CreatedByFullName = creator != null ? creator.FirstName + " " + creator.LastName : "Bilinmiyor",
                    ConsultantFullName = consultant != null ? consultant.FirstName + " " + consultant.LastName : null,
                    ProjectArea = p.ProjectArea,
                    InitialCode = p.InitialCode,
                    IsActive = p.IsActive,
                    IsDeleted = p.IsDeleted,
                    ApprovalStatus = p.ApprovalStatus
                }).FirstOrDefaultAsync();

            if (detail == null)
                return null!;

            var phases = await _context.ProjectPhases
                .AsNoTracking()
                .Where(ph => ph.ProjectId == projectId && !ph.IsDeleted)
                .OrderBy(ph => ph.Order)
                .Select(ph => new ProjectPhaseDto
                {
                    Id = ph.Id,
                    ProjectId = ph.ProjectId,
                    PhaseName = ph.PhaseName,
                    Description = ph.Description,
                    Order = ph.Order,
                    IsCompleted = ph.IsCompleted,
                    CompletedDate = ph.CompletedDate
                })
                .ToListAsync();

            var languages = await _context.ProjectLanguages
                .AsNoTracking()
                .Where(l => l.ProjectId == projectId && !l.IsDeleted)
                .Select(l => l.LanguageName)
                .ToListAsync();

            detail.Phases = phases;
            detail.Languages = languages;
            return detail;
        }
    }
}
