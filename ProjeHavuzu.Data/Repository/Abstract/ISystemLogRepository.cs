using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using ProjeHavuzu.Data.Entites;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface ISystemLogRepository : IRepository<SystemLog>
    {
        Task<DataTableResponse<UnifiedLogListDto>> GetLogsServerSideAsync(DataTableRequest request, UnifiedLogFilterDto filter);
    }
}
