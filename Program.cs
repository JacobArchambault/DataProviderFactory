using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using static System.Console;
using System.Configuration;

namespace DataProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Fun with Data Provider Factories *****\n");
            // Get Connection string/provider from *.config.
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.AppSettings["connectionString"];

            // Get the factory provider.
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);

            // Now get the connection object.
            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    ShowError("Connection");
                    return;
                }
                WriteLine($"Your connection object is a: {connection.GetType().Name}");
                connection.ConnectionString = connectionString;
                connection.Open();

                // Make command object.
                DbCommand command = factory.CreateCommand();
                if (command == null)
                {
                    ShowError("Command");
                    return;
                }
                WriteLine($"Your command object is a: {command.GetType().Name}");
                command.Connection = connection;
                command.CommandText = "Select * From Inventory";

                // Print out data with data reader.
                using (DbDataReader datareader = command.ExecuteReader())
                {
                    WriteLine($"Your data reader object is a: {datareader.GetType().Name}");

                    WriteLine("\n***** Current Inventory *****");
                    while (datareader.Read())
                        WriteLine($"-> Car #{datareader["CarId"]} is a {datareader["Make"]}.");
                }
            }
            ReadLine();
        }
        private static void ShowError(string objectName)
        {
            WriteLine($"There was an issue creating the {objectName}");
            ReadLine();
        }
    }
}
