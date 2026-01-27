using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface ISystemHealthService
    {
        Task CheckSystemHealthAsync();
    }
}
