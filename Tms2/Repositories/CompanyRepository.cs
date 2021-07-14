using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tms2.Models;

namespace Tms2.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connectionString;
        public CompanyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<string> CreateComany(CompanyModel cmp, string user)
        {
            return Task.Run(() =>
            {
                if (CheckCompanyExist(cmp.Inn))
                {
                    return "Company exist";
                }
                using var conn = new NpgsqlConnection(_connectionString);
                var editor = "{\"editors\":[\"" + user + "\"],\"readers\":[]}";
                var commanText = $"INSERT INTO public.\"Companies\" (name, fullname, inn, kpp, createdby, createdat, users) VALUES ('{cmp.CompanyShortName}'," +
                    $" '{cmp.CompanyNameFull}', '{cmp.Inn}', '{cmp.Kpp}', '{user}', '{DateTime.Now}', '{editor}');";
                using var command = new NpgsqlCommand(commanText, conn);
                conn.Open();
                if (command.ExecuteNonQuery() >= 0)
                {
                    return "";
                }
                return "Error insert";
            });
        }

        public Task<CompanyModel> GetComany(string inn)
        {
            return Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(_connectionString);
                var commanText = $"SELECT name, fullname, inn, kpp, id FROM public.\"Companies\" WHERE inn = '{inn}';";
                using var command = new NpgsqlCommand(commanText, conn);
                conn.Open();
                var reader = command.ExecuteReader();
                var result = new CompanyModel();
                while (reader.Read())
                {                    
                    result.CompanyShortName = reader.GetString(0);
                    result.CompanyNameFull = reader.GetString(1);
                    result.Inn = reader.GetString(2);
                    result.Kpp = reader.GetString(3);
                    result.Id = reader.GetInt32(4);
                }
                return result;
            });
        }

        public Task<List<CompanyModel>> GetUserCompanies(string id)
        {
            return Task.Run(() =>
            {
                var result = new List<CompanyModel>();
                using var conn = new NpgsqlConnection(_connectionString);
                var commanText = "SELECT name, inn, id FROM public.\"Companies\" WHERE users @> '{\"editors\":[\"" + id +"\"]}';";
                using var command = new NpgsqlCommand(commanText, conn);
                conn.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var t = new CompanyModel
                    {
                        CompanyShortName = reader.GetString(0),
                        Inn = reader.GetString(1),
                        Id = reader.GetInt32(2)
                    };
                    result.Add(t);
                }

                return result;
            });
        }

        private bool CheckCompanyExist(string inn)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            var commanText = $"SELECT COUNT(*) FROM public.\"Companies\" WHERE inn = '{inn}';";
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
