using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Tms2.Models;
using Tms2.Repositories;
using Dadata.Model;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace Tms2.Controllers
{
    public class OfficeController : Controller
    {
        private IDaDataRepository _dadata;
        private ICompanyRepository _comp;
        private IDriverRepository _drivers;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signManager;

        public OfficeController(IDaDataRepository daData, 
            ICompanyRepository cmp,
            IDriverRepository drv,
            UserManager<IdentityUser> userManager)
        {
            _dadata = daData;
            _comp = cmp;
            _drivers = drv;
            _userManager = userManager;
        }

        public async Task<IActionResult> Companies()
        {
            var model = new OfficeModel();
            var user = await _userManager.GetUserAsync(User);
            var usersCompany = await _comp.GetUserCompanies(user.Id);
            model.Companies = new List<Tuple<string, string>>();
            model.Companies.Add(new Tuple<string, string>("Название", "ИНН"));
            foreach (var c in usersCompany)
            {
                model.Companies.Add(new Tuple<string, string>(c.CompanyShortName, c.Inn));
            }
            model.ViewData["ActivePage"] = "Companies";
            model.ViewData["Title"] = "Office-Companies";            

            return View("Office", model);
        }

        // Вкладка "Водители"
        [Route("/office/companies/drivers")]
        public async Task<IActionResult> Drivers()
        {
            var model = new OfficeModel();
            model.ViewData["ActivePage"] = "Drivers";
            model.ViewData["Title"] = "Office-Drivers";
            var user = await _userManager.GetUserAsync(User);
            model.Drivers = await _drivers.GetMyDrivers(user.Id);

            return View("Drivers", model);
        }

        [Route("/office/companies/transport")]
        public IActionResult Transport()
        {
            var model = new OfficeModel();
            model.ViewData["ActivePage"] = "Transport";
            model.ViewData["Title"] = "Office-Transport";

            return View("Transport", model);
        }

        [Route("/office/companies/users")]
        public IActionResult Users()
        {
            var model = new OfficeModel();
            model.ViewData["ActivePage"] = "Users";
            model.ViewData["Title"] = "Office-Drivers";

            return View("Users", model);
        }

        public IActionResult CreateComp()
        {          
            var model = new OfficeModel();
            model.ViewData["ActivePage"] = "Users";
            model.ViewData["Title"] = "Office-Drivers";           

            return View("CreateComp", model);
        }

        public async Task<IActionResult> Newcompany(OfficeModel m)
        {            
            var user = await _userManager.GetUserAsync(User);
            
            var t = await _dadata.GetCompaneByInn(m.Inn);
            if(t.suggestions.Count < 1)
            {
                return View("CompError");
            }
            var comp = t.suggestions[0];
            var model = new CompanyModel
            {
                Inn = m.Inn,
                Kpp = comp.data.kpp,
                CompanyNameFull = comp.data.name.full_with_opf,
                CompanyShortName = comp.data.name.short_with_opf
            };

            var result = await _comp.CreateComany(model, user.Id);
            if (result == "")
            {
                return View("CompanyView", model);
            }
            return View("CompError");
        }

        // Создание нового водителя (форма ввода)
        [Route("/office/companies/createdriver")]
        [HttpGet]
        public async Task<IActionResult> createDriver()
        {
            var model = new DriverModel
            {
                Companies = new Dictionary<int, string>(),
                CompaniesSelect = new Dictionary<int, bool>(),
            };

            var user = await _userManager.GetUserAsync(User);
            var usersCompany = await _comp.GetUserCompanies(user.Id);
            foreach(var comp in usersCompany)
            {
                model.Companies.Add(comp.Id, comp.CompanyShortName);
                model.CompaniesSelect.Add(comp.Id, false);
            }

            return View("CreateDriver", model);
        }

        // Согдание водителя - процесс
        [Route("/office/companies/newdriver")]
        [HttpPost]
        public async Task<IActionResult> NewDriver(DriverModel driver)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _drivers.CreateDriver(driver, user.Id);
           
            if (result == "")
            {
                return await Drivers();
            }
            return View("CompError");
        }

        [HttpGet]
        public async Task<IActionResult> Companyview(string inn)
        {
            CompanyModel model = await _comp.GetComany(inn);
            return View("CompanyView", model);
        }
    }
}
