using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.AccountDto;

namespace ProjeHavuzu.Business.Services.Concrete
{
    /// <summary>
    /// Danışman dropdown: Teacher rolündeki gerçek akademisyenler.
    /// Öğrenciler ve @projehavuzu.com test hesapları hariç.
    /// </summary>
    public class AdvisorCandidateService : IAdvisorCandidateService
    {
        private readonly ApplicationContext _context;

        public AdvisorCandidateService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<UserListDto>> GetAdvisorCandidatesAsync()
        {
            var studentUserIds = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Student"
                select ur.UserId).ToListAsync();

            var teacherUserIds = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Teacher"
                select ur.UserId).ToListAsync();

            var advisorIds = teacherUserIds.Except(studentUserIds).ToHashSet();

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive && advisorIds.Contains(u.Id))
                .Where(u => u.StudentNumber == null || u.StudentNumber == "")
                .Where(u => u.Email == null || !EF.Functions.Like(u.Email, "%@projehavuzu.com"))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    FullName = (u.FirstName + " " + u.LastName).Trim(),
                    Email = u.Email ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<bool> IsValidAdvisorAsync(Guid? consultantId)
        {
            if (!consultantId.HasValue || consultantId.Value == Guid.Empty)
                return false;

            var advisors = await GetAdvisorCandidatesAsync();
            return advisors.Any(a => a.Id == consultantId.Value);
        }
    }
}
