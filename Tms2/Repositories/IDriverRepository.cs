using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tms2.Models;

namespace Tms2.Repositories
{
    public interface IDriverRepository
    {
        Task<string> CreateDriver(DriverModel driver, string user);

        /// <summary>
        /// Спсиок водителей компаний пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Tuple<string, string>>> GetMyDrivers(string id);
    }
}