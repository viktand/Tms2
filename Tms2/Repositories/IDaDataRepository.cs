using Dadata.Model;
using System.Threading.Tasks;

namespace Tms2.Repositories
{
    public interface IDaDataRepository
    {
        Task<SuggestResponse<Party>> GetCompaneByInn(string inn);
    }
}