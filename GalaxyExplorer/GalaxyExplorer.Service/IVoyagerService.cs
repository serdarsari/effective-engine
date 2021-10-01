using GalaxyExplorer.DTO;
using System.Threading.Tasks;

namespace GalaxyExplorer.Service
{
    public interface IVoyagerService
    {
        Task<GetVoyagersResponse> GetVoyagers(GetVoyagersRequest request);
    }
}
