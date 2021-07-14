using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tms2.Models;

namespace Tms2.Repositories
{
    public class DriverRepository : IDriverRepository
    {

        private readonly string _connectionString;
        public DriverRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<string> CreateDriver(DriverModel driver, string user)
        {
            return Task.Run(() =>
            {
                if (CheckPhoneExist(driver.Phone))
                {
                    return "Phone exist";
                }
                using var conn = new NpgsqlConnection(_connectionString);
                var t = "[";
                foreach(var c in driver.CompaniesSelect.Where(x => x.Value == true).Select(x => x.Key))
                {
                    t += c + ", ";
                }
                t += "]";
                t = t.Replace(", ]", "]");
                var companies = "{\"companies\":" + t + "}";
                var commanText = $"INSERT INTO public.\"Drivers\" (surname, name, patronymic, phone, createdby, createdat, companies) VALUES ('{driver.Surname}'," +
                    $" '{driver.Name}', '{driver.Patronymic}', '{driver.Phone}', '{user}', '{DateTime.Now}', '{companies}');";
                using var command = new NpgsqlCommand(commanText, conn);
                conn.Open();
                if (command.ExecuteNonQuery() >= 0)
                {
                    return "";
                }
                return "Error insert";
            });
        }

        public Task<List<Tuple<string, string>>> GetMyDrivers(string id)
        {
            return Task.Run(() =>
            {
                var compRepo = new CompanyRepository(_connectionString);
                var companies = compRepo.GetUserCompanies(id).Result;
                if (companies.Count == 0) return new List<Tuple<string, string>>();
                var q = new StringBuilder();
                foreach (var c in companies)
                {
                    q.Append("companies @> '{\"companies\":[" + c.Id + "]}' OR ");
                }
                using var conn = new NpgsqlConnection(_connectionString);
                var commandText = $"SELECT surname, name, patronymic, phone FROM public.\"Drivers\" WHERE {q};";
                commandText = commandText.Replace("OR ;", ";");
                using var command = new NpgsqlCommand(commandText, conn);
                conn.Open();
                var result = new List<Tuple<string, string>>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Tuple<string, string>
                    (
                        reader.GetString(0) + " " + reader.GetString(1)[0] + "." + reader.GetString(2)[0] + ".",
                        reader.GetString(3)
                    ));
                }
                reader.Close();
                return result;
            });

        }

        private bool CheckPhoneExist(string phone)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            var commanText = $"SELECT COUNT(*) FROM public.\"Drivers\" WHERE phone = '{phone}';";
            using var command = new NpgsqlCommand(commanText, conn);
            conn.Open();
            var result = 0;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            reader.Close();
            return result > 0;
        }
    }
}
