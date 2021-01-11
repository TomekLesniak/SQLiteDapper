using System;
using System.Collections.Generic;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace SQLiteDapperHomeworkUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlite = new SQLiteCrud(new SQLiteDataAccess(), GetConnectionString());
            // ReadAllPeople(sqlite);
            // ReadPerson(sqlite, 1);

            SaveFullPerson(sqlite);
            ReadFullPerson(sqlite, 4);

            Console.WriteLine("Sqlite demo");
            Console.ReadLine();
        }

        private static void SaveFullPerson(SQLiteCrud sqlite)
        {
            var addresses = new List<AddressModel>();
            addresses.Add(new AddressModel{Street = "Street1", City = "City1"});
            addresses.Add(new AddressModel{Street = "Street2", City = "City2"});
            var person = new FullPersonModel
            {
                FirstName = "Tim",
                LastName = "Corey",
                Addresses = addresses
            };
            
            sqlite.SaveFullPerson(person);
        }

        private static void ReadFullPerson(SQLiteCrud sqlite, int id)
        {
            FullPersonModel person = sqlite.LoadFullPersonInfo(id);

            Console.WriteLine($"{person.Id}: {person.FirstName} {person.LastName}");
            foreach (var address in person.Addresses)
            {
                Console.WriteLine($"\t{address.Street} {address.City}");
            }
        }

        private static void ReadAllPeople(SQLiteCrud sqlite)
        {
            var people = sqlite.LoadAllPeople();

            foreach (var person in people)
            {
                Console.WriteLine($"{person.Id}: {person.FirstName} {person.LastName}");
            }
        }

        private static void ReadPerson(SQLiteCrud sqlite, int id)
        {
            var person = sqlite.LoadPerson(id);
            Console.WriteLine($"{person.Id}: {person.FirstName} {person.LastName}");
        }
        

        private static string GetConnectionString(string name = "Default")
        {
            var output = "";

            var builder = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json");
            
            var configuration = builder.Build();
            output = configuration.GetConnectionString(name);

            return output;
        }
    }
}
