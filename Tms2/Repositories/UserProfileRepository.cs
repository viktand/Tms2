using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tms2.Areas.Identity.Pages.Account.Manage;
using Npgsql;
using Tms2.Common;
using static Tms2.Areas.Identity.Pages.Account.Manage.PersonalDataModel;


namespace Tms2.Repositories
{
    public class UserProfileRepository: IUserProfileRepository
    {
        private readonly string _connectionString;
        public UserProfileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<InputPersonalData> ReadProfile(string id)
        {
            return Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(_connectionString);
                var commanText = $"SELECT surname, name, patronymic FROM public.\"UserProfile\" WHERE id = '{id}';";
                using var command = new NpgsqlCommand(commanText, conn);
                conn.Open();
                var result = new InputPersonalData();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Surname = reader.GetString(0);
                    result.Name = reader.GetString(1);
                    result.Patronymic = reader.GetString(2);
                }
                reader.Close();
                return result;
            });
        }

        public Task<bool> SaveProfile(string id, InputPersonalData user)
        {
            return Task.Run(() =>
            {
                if (!IsExist(id))
                {
                    using var conn = new NpgsqlConnection(_connectionString);
                    var commanText = $"INSERT INTO public.\"UserProfile\" (id, surname, name, patronymic) VALUES ('{id}', '{user.Surname.NotNullOrEmpty()}', '{user.Name.NotNullOrEmpty()}', '{user.Patronymic.NotNullOrEmpty()}');";
                    using var command = new NpgsqlCommand(commanText, conn);
                    conn.Open();
                    return command.ExecuteNonQuery() >= 0;
                }
                using var connUp = new NpgsqlConnection(_connectionString);
                var commanTextUp = $"UPDATE public.\"UserProfile\" SET surname = '{user.Surname.NotNullOrEmpty()}', name = '{user.Name.NotNullOrEmpty()}', patronymic = '{user.Patronymic.NotNullOrEmpty()}';";
                using var commandUp = new NpgsqlCommand(commanTextUp, connUp);
                connUp.Open();
                return commandUp.ExecuteNonQuery() >= 0;
            });
          
        }

        /// <summary>
        /// Check user existence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsExist(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            var commanText = $"SELECT COUNT(*) FROM public.\"UserProfile\" WHERE id = '{id}';";
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
