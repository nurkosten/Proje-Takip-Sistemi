using ProjeHavuzu.Data.DTOs.AccountDto;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IAdvisorCandidateService
    {
        Task<List<UserListDto>> GetAdvisorCandidatesAsync();
        Task<bool> IsValidAdvisorAsync(Guid? consultantId);
    }
}
