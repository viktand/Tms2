using System.Threading.Tasks;
using static Tms2.Areas.Identity.Pages.Account.Manage.PersonalDataModel;

namespace Tms2.Repositories
{
    public interface IUserProfileRepository
    {
        Task<bool> SaveProfile(string id, InputPersonalData user);

        Task<InputPersonalData> ReadProfile(string id);
    }
}