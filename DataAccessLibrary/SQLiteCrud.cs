using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public class SQLiteCrud
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _connectionString;

        public SQLiteCrud(IDataAccess dataAccess, string connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public List<PersonModel> LoadAllPeople()
        {
            string sql = "select Id, FirstName, LastName from People;";

            return _dataAccess.LoadData<PersonModel, dynamic>(sql, new { }, _connectionString);
        }

        public PersonModel LoadPerson(int id)
        {
            string sql = "select Id, FirstName, LastName from People where Id = @Id;";

            return _dataAccess.LoadData<PersonModel, dynamic>(sql, new {Id = id}, _connectionString).FirstOrDefault();
        }

        public FullPersonModel LoadFullPersonInfo(int id)
        {
            FullPersonModel output = new();
            string sql = "SELECT Id, FirstName, LastName from People where Id = @Id";
            output = _dataAccess.LoadData<FullPersonModel, dynamic>(sql, new {Id = id}, _connectionString).First();

            sql = @"SELECT  a.Id, a.Street, A.City
                    FROM PeopleAddresses pa
                    inner JOIN Addresses a on a.Id = pa.AddressId
                    WHERE pa.PersonId = @Id;";

            output.Addresses = _dataAccess.LoadData<AddressModel, dynamic>(sql, new {Id = id}, _connectionString);
            
            return output;
        }

        public void SaveFullPerson(FullPersonModel person)
        {
            string sql = "INSERT into People(FirstName, LastName) VALUES (@FirstName, @LastName);";
            _dataAccess.SaveData(sql, new {person.FirstName, person.LastName}, _connectionString);

            sql = "SELECT Id FROM People WHERE FirstName = @FirstName AND LastName = @LastName";
            var personId = _dataAccess
                .LoadData<PersonModel, dynamic>(sql, new {person.FirstName, person.LastName}, _connectionString)
                .First().Id;

            foreach (var address in person.Addresses)
            {
                if (address.Id == 0)
                {
                    sql = "INSERT INTO Addresses(Street, City) VALUES (@Street, @City)";
                    _dataAccess.SaveData(sql, new {address.Street, address.City}, _connectionString);

                    sql = "SELECT Id from Addresses where City = @City AND Street = @Street";
                    address.Id = _dataAccess.LoadData<AddressModel, dynamic>(sql, new {address.City, address.Street},
                        _connectionString).First().Id;
                }

                sql = "INSERT INTO PeopleAddresses (PersonId, AddressId) VALUES (@PersonId, @AddressId)";
                _dataAccess.SaveData(sql, new{PersonId = personId, AddressId = address.Id}, _connectionString);
            }
        }

    }
}
