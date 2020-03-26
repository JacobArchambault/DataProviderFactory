using System.Data.Common;
using System.Data.SqlClient;
using static System.Configuration.ConfigurationManager;
using static System.Console;
using static System.Data.Common.DbProviderFactories;

namespace DataProviderFactory
{
    class Program
    {
        static void Main()
        {
            WriteLine("***** Fun with Data Provider Factories *****\n");

            // Get the factory provider from *.config.
            DbProviderFactory factory = GetFactory(AppSettings["provider"]);

            // Now get the connection object.
            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    ShowError("Connection");
                }
                else
                {
                    WriteLine($"Your connection object is a: {connection.GetType().Name}");
                    // Get the connection string from App.config.
                    connection.ConnectionString = ConnectionStrings["AutoLotSqlProvider"].ConnectionString;
                    connection.Open();

                    if (connection is SqlConnection sqlConnection)
                        WriteLine(sqlConnection.ServerVersion);

                    // Make command object.
                    DbCommand command = factory.CreateCommand();
                    if (command == null)
                    {
                        ShowError("Command");
                    }
                    else
                    {
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
