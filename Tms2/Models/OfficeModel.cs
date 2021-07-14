using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tms2.Models
{
    public class OfficeModel 
    {
        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string Inn { get; set; }

        public Dictionary<string, string> ViewData { get; set; }

        public List<Tuple<string, string>> Companies { get; set; } // Название + ИНН

        public List<Tuple<string, string>> Drivers { get; set; } // Фамилия И.О. + телефон     

        public OfficeModel()
        {
            ViewData = new Dictionary<string, string>
            {
                { "Title", "" },
                { "ActivePage", "" }
            };
        }

        public string Select(string name)
        {
            return name == ViewData["ActivePage"] ? "active" : "";
        }

        public string GetCursor(string s)
        {
            return s != "Название" ? "pointer" : "auto";
        }

        public string OpenCompany(string inn)
        {
            return inn == "ИНН" ? "" : $"onclick = location.href='/office/companyview?inn={inn}'";
        }
    }
}
