using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tms2.Repositories;

namespace Tms2.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        protected internal IUserProfileRepository UserRepo { get; }

        public class InputPersonalData
        {
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Patronymic")]
            public string Patronymic { get; set; }
        }

        [BindProperty]
        public InputPersonalData Input { get; set; }

        public PersonalDataModel(
            UserManager<IdentityUser> userManager,
            ILogger<PersonalDataModel> logger,
            IUserProfileRepository userRepo)
        {
            _userManager = userManager;
            _logger = logger;
            UserRepo = userRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            Input = await UserRepo.ReadProfile(user.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await UserRepo.SaveProfile(user.Id, Input);
            return RedirectToPage();
        }
    }
}