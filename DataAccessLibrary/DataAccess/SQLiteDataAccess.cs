using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;

namespace DataAccessLibrary
{
    public class SQLiteDataAccess : IDataAccess
    {
        public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
        {
            using IDbConnection connection = new SQLiteConnection(connectionString);
            return connection.Query<T>(sql, parameters).ToList();
        }

        public void SaveData<T>(string sql, T data, string connectionString)
        {
            using IDbConnection connection = new SQLiteConnection(connectionString);
            connection.Execute(sql, data);
        }
    }
}
