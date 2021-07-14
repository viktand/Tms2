using Dadata;
using Dadata.Model;
using System.Threading.Tasks;

namespace Tms2.Repositories
{
    public class DaDataRepository : IDaDataRepository
    {
        private string _token;

        public DaDataRepository(string token)
        {
            _token = token;
        }

        public async Task<SuggestResponse<Party>> GetCompaneByInn(string inn)
        {
            var api = new SuggestClientAsync(_token);
            var result = await api.FindParty(inn);
            return result;
        }
    }
}
