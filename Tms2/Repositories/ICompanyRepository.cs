using System.Collections.Generic;
using System.Threading.Tasks;
using Tms2.Models;

namespace Tms2.Repositories
{
    public interface ICompanyRepository
    {
        Task<string> CreateComany(CompanyModel cmp, string user);
        Task<List<CompanyModel>> GetUserCompanies(string id);
        Task<CompanyModel> GetComany(string inn);
    }
}