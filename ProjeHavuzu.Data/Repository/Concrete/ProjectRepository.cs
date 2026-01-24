using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
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
    join c in _context.Categories
        on p.CategoryId equals c.Id
    join u in _context.Users
        on p.CreatedBy equals u.Id into userGroup
    from u in userGroup.DefaultIfEmpty() // 👈 LEFT JOIN
    select new ProjectListDto
    {
        Id = p.Id,
        ProjectTitle = p.ProjectTitle,
        Description = p.Description,
        CreatedDate = p.CreatedDate,
        DifficultyLevel = p.DifficultyLevel,
        EndTime = p.EndTime,
        ProjectLink = p.ProjectLink,
        CategoryId = c.Id,
        CategoryName = c.CategoryName,
        CreatedByFullName = u != null
            ? u.FirstName + " " + u.LastName
            : "Unknown"
    };
            return await result.ToListAsync();
        }


    }
}
