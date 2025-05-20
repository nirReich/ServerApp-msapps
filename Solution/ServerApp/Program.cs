using System;
using System.Data.SqlClient;

namespace ServerApp
{
    class Program
    {
        private static readonly string connectionString = "Server=localhost\\SQLEXPRESS;Initial Catalog=MyDatabase;Integrated Security=True";
        static void Main(string[] args)
        {
            Console.WriteLine("Server Application Starting...");
            
            try
            {
                // Ensure database and tables exist
                EnsureDatabaseSetup();
                
                // Start the HTTP server
                Server server = new Server("http://localhost:8080/", connectionString);
                server.Start();
                
                // Set up batch service to run at 8:00 PM every day
                Services.UserBatchService batchService = new Services.UserBatchService(connectionString);
                TimeSpan scheduleTime = new TimeSpan(20, 0, 0); // 8:00 PM
                batchService.ScheduleBatchProcessing(scheduleTime);
                
                Console.WriteLine("Press 'B' to run batch service manually");
                Console.WriteLine("Press 'Q' to quit");
                
                bool running = true;
                while (running)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.B:
                            Console.WriteLine("\nRunning batch service manually...");
                            batchService.RunBatchProcessingNow();
                            break;
                        
                        case ConsoleKey.Q:
                            Console.WriteLine("\nStopping server...");
                            server.Stop();
                            running = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        static void EnsureDatabaseSetup()
        {
            // Create database if it doesn't exist
            string masterConnection = "Data Source=localhost;Initial Catalog=master;Integrated Security=True";
            
            using (SqlConnection connection = new SqlConnection(masterConnection))
            {
                connection.Open();
                
                // Check if database exists
                string checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = 'MyDatabase'";
                SqlCommand checkDbCommand = new SqlCommand(checkDbQuery, connection);
                int dbCount = (int)checkDbCommand.ExecuteScalar();
                
                if (dbCount == 0)
                {
                    // Create the database
                    string createDbQuery = "CREATE DATABASE MyDatabase";
                    SqlCommand createDbCommand = new SqlCommand(createDbQuery, connection);
                    createDbCommand.ExecuteNonQuery();
                    
                    Console.WriteLine("Database 'MyDatabase' created successfully.");
                }
            }
            
            // Create Users table if it doesn't exist
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Check if table exists
                string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'";
                SqlCommand checkTableCommand = new SqlCommand(checkTableQuery, connection);
                int tableCount = (int)checkTableCommand.ExecuteScalar();
                
                if (tableCount == 0)
                {
                    // Create the table
                    string createTableQuery = @"
                        CREATE TABLE Users (
                            ID INT PRIMARY KEY IDENTITY(1,1),
                            Name VARCHAR(100) NOT NULL,
                            Email VARCHAR(100) NOT NULL,
                            Password VARCHAR(100) NOT NULL,
                            LastEmailSent DATETIME NULL
                        )";
                    SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection);
                    createTableCommand.ExecuteNonQuery();
                    
                    Console.WriteLine("Table 'Users' created successfully.");
                    
                    // Add some sample data
                    string insertDataQuery = @"
                        INSERT INTO Users (Name, Email, Password) VALUES
                        ('John Doe', 'john@example.com', 'password123'),
                        ('Jane Smith', 'jane@example.com', 'pass456'),
                        ('Bob Johnson', 'bob@example.com', 'secure789')";
                    SqlCommand insertDataCommand = new SqlCommand(insertDataQuery, connection);
                    insertDataCommand.ExecuteNonQuery();
                    
                    Console.WriteLine("Sample data inserted into 'Users' table.");
                }
            }
            
            Console.WriteLine("Database setup completed.");
        }
    }
}