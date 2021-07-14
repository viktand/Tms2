using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tms2.Models
{
    public class DriverModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Phone { get; set; }

        public Dictionary<int, string> Companies { get; set; }
        public Dictionary<int, bool> CompaniesSelect { get; set; }

        public string ShortName()
        {
            return Surname + " " + Name[0] + "." + Patronymic[0] + ".";
        }
    }
}
