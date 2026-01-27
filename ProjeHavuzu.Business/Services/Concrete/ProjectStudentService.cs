using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.ProjectStudentDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class ProjectStudentService : IProjectStudentService
    {
        private readonly IProjectStudentRepository _projectStudentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<AppUser> _userManager;

        public ProjectStudentService(
            IProjectStudentRepository projectStudentRepository,
            IProjectRepository projectRepository,
            UserManager<AppUser> userManager)
        {
            _projectStudentRepository = projectStudentRepository;
            _projectRepository = projectRepository;
            _userManager = userManager;
        }

        public async Task<AssignStudentsToProjectDto> GetAssignStudentsToProjectDtoAsync(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == projectId && !p.IsDeleted);
            if (project == null)
                throw new ArgumentException("Proje bulunamadı.");

            // Get all students (users with Student role)
            var allStudents = await _userManager.GetUsersInRoleAsync("Student");

            // Get already assigned students
            var assignedStudents = await _projectStudentRepository.GetProjectStudentsByProjectIdAsync(projectId);
            var assignedStudentIds = assignedStudents.Select(ps => ps.StudentId).ToList();

            var availableStudents = allStudents.Select(s => new StudentSelectionDto
            {
                StudentId = s.Id,
                FullName = s.FullName,
                StudentNumber = s.StudentNumber ?? "Öğrenci No Yok",
                Email = s.Email ?? "",
                DepartmentName = s.Department?.DepartmentName ?? "Bölüm Yok",
                IsAssigned = assignedStudentIds.Contains(s.Id)
            }).ToList();

            return new AssignStudentsToProjectDto
            {
                ProjectId = projectId,
                ProjectTitle = project.ProjectTitle,
                AvailableStudents = availableStudents,
                SelectedStudentIds = assignedStudentIds
            };
        }

        public async Task<bool> AssignStudentsToProjectAsync(Guid projectId, List<Guid> studentIds)
        {
            // Validate project exists
            var project = await _projectRepository.GetAsync(p => p.Id == projectId && !p.IsDeleted);
            if (project == null)
                throw new ArgumentException("Proje bulunamadı.");

            // Get current assignments
            var currentAssignments = await _projectStudentRepository.GetProjectStudentsByProjectIdAsync(projectId);
            var currentStudentIds = currentAssignments.Select(ps => ps.StudentId).ToList();

            // Students to add
            var studentsToAdd = studentIds.Except(currentStudentIds).ToList();
            foreach (var studentId in studentsToAdd)
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null) continue;

                var projectStudent = new ProjectStudent
                {
                    ProjectId = projectId,
                    StudentId = studentId
                };
                await _projectStudentRepository.AddAsync(projectStudent);
            }

            // Students to remove
            var studentsToRemove = currentStudentIds.Except(studentIds).ToList();
            foreach (var studentId in studentsToRemove)
            {
                var assignment = currentAssignments.FirstOrDefault(ps => ps.StudentId == studentId);
                if (assignment != null)
                {
                    assignment.IsDeleted = true;
                    _projectStudentRepository.Update(assignment);
                }
            }

            return true;
        }

        public async Task<bool> RemoveStudentFromProjectAsync(Guid projectId, Guid studentId)
        {
            var assignment = await _projectStudentRepository.GetProjectStudentAsync(projectId, studentId);
            if (assignment == null)
                throw new ArgumentException("Atama bulunamadı.");

            assignment.IsDeleted = true;
            _projectStudentRepository.Update(assignment);
            return true;
        }

        public async Task<AssignProjectsToStudentDto> GetAssignProjectsToStudentDtoAsync(Guid studentId)
        {
            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null)
                throw new ArgumentException("Öğrenci bulunamadı.");

            // Get all active projects
            var allProjects = await _projectRepository.GetAllProjectsByCategoryAsync();

            // Get already assigned projects
            var assignedProjects = await _projectStudentRepository.GetProjectStudentsByStudentIdAsync(studentId);
            var assignedProjectIds = assignedProjects.Select(ps => ps.ProjectId).ToList();

            var availableProjects = allProjects
                .Where(p => !p.IsDeleted)
                .Select(p => new ProjectSelectionDto
                {
                    ProjectId = p.Id,
                    ProjectTitle = p.ProjectTitle,
                    Description = p.Description,
                    CategoryName = p.CategoryName,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsAssigned = assignedProjectIds.Contains(p.Id)
                }).ToList();

            return new AssignProjectsToStudentDto
            {
                StudentId = studentId,
                StudentFullName = student.FullName,
                StudentNumber = student.StudentNumber ?? "Öğrenci No Yok",
                AvailableProjects = availableProjects,
                SelectedProjectIds = assignedProjectIds
            };
        }

        public async Task<bool> AssignProjectsToStudentAsync(Guid studentId, List<Guid> projectIds)
        {
            // Validate student exists
            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null)
                throw new ArgumentException("Öğrenci bulunamadı.");

            // Get current assignments
            var currentAssignments = await _projectStudentRepository.GetProjectStudentsByStudentIdAsync(studentId);
            var currentProjectIds = currentAssignments.Select(ps => ps.ProjectId).ToList();

            // Projects to add
            var projectsToAdd = projectIds.Except(currentProjectIds).ToList();
            foreach (var projectId in projectsToAdd)
            {
                var project = await _projectRepository.GetAsync(p => p.Id == projectId && !p.IsDeleted);
                if (project == null) continue;

                var projectStudent = new ProjectStudent
                {
                    ProjectId = projectId,
                    StudentId = studentId
                };
                await _projectStudentRepository.AddAsync(projectStudent);
            }

            // Projects to remove
            var projectsToRemove = currentProjectIds.Except(projectIds).ToList();
            foreach (var projectId in projectsToRemove)
            {
                var assignment = currentAssignments.FirstOrDefault(ps => ps.ProjectId == projectId);
                if (assignment != null)
                {
                    assignment.IsDeleted = true;
                    _projectStudentRepository.Update(assignment);
                }
            }

            return true;
        }

        public async Task<bool> RemoveProjectFromStudentAsync(Guid studentId, Guid projectId)
        {
            var assignment = await _projectStudentRepository.GetProjectStudentAsync(projectId, studentId);
            if (assignment == null)
                throw new ArgumentException("Atama bulunamadı.");

            assignment.IsDeleted = true;
            _projectStudentRepository.Update(assignment);
            return true;
        }

        public async Task<List<ProjectStudentAssignDto>> GetStudentsByProjectIdAsync(Guid projectId)
        {
            var assignments = await _projectStudentRepository.GetProjectStudentsByProjectIdAsync(projectId);
            var result = new List<ProjectStudentAssignDto>();

            foreach (var assignment in assignments)
            {
                var student = await _userManager.FindByIdAsync(assignment.StudentId.ToString());
                if (student != null)
                {
                    result.Add(new ProjectStudentAssignDto
                    {
                        ProjectId = assignment.ProjectId,
                        StudentId = assignment.StudentId,
                        StudentFullName = student.FullName,
                        StudentNumber = student.StudentNumber ?? "Öğrenci No Yok"
                    });
                }
            }

            return result;
        }

        public async Task<List<ProjectStudentAssignDto>> GetProjectsByStudentIdAsync(Guid studentId)
        {
            var assignments = await _projectStudentRepository.GetProjectStudentsByStudentIdAsync(studentId);
            var result = new List<ProjectStudentAssignDto>();

            foreach (var assignment in assignments)
            {
                var projects = await _projectRepository.GetAllProjectsByCategoryAsync();
                var project = projects.FirstOrDefault(p => p.Id == assignment.ProjectId);

                if (project != null)
                {
                    result.Add(new ProjectStudentAssignDto
                    {
                        ProjectId = assignment.ProjectId,
                        ProjectTitle = project.ProjectTitle,
                        StudentId = assignment.StudentId
                    });
                }
            }

            return result;
        }
    }
}
