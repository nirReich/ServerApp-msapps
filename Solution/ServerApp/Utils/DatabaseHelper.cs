using System;
using System.IO;
using System.Data.SqlClient;

namespace ServerApp.Utils
{
    public class DatabaseHelper
    {
        // SQL script to create the database and tables
        private const string SQL_CREATE_DATABASE = @"
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MyDatabase')
BEGIN
    CREATE DATABASE MyDatabase;
    PRINT 'Database MyDatabase created.';
END
ELSE
BEGIN
    PRINT 'Database MyDatabase already exists.';
END";

        private const string SQL_CREATE_USERS_TABLE = @"
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE Users (
        ID INT PRIMARY KEY IDENTITY(1,1),
        Name VARCHAR(100) NOT NULL,
        Email VARCHAR(100) NOT NULL,
        Password VARCHAR(100) NOT NULL,
        LastEmailSent DATETIME NULL
    );
    PRINT 'Table Users created.';
    
    -- Insert sample data
    INSERT INTO Users (Name, Email, Password) VALUES
    ('John Doe', 'john@example.com', 'password123'),
    ('Jane Smith', 'jane@example.com', 'pass456'),
    ('Bob Johnson', 'bob@example.com', 'secure789');
    PRINT 'Sample data inserted.';
END
ELSE
BEGIN
    PRINT 'Table Users already exists.';
END";

        // Initialize the database
        public static void InitializeDatabase(string connectionString)
        {
            try
            {
                // Create database
                string masterConnectionString = connectionString.Replace("Initial Catalog=MyDatabase;", "Initial Catalog=master;");
                ExecuteNonQuery(masterConnectionString, SQL_CREATE_DATABASE);
                
                // Create tables
                ExecuteNonQuery(connectionString, SQL_CREATE_USERS_TABLE);
                
                Console.WriteLine("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }
        
        // Execute a SQL command that doesn't return results
        private static void ExecuteNonQuery(string connectionString, string commandText)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}