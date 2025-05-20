using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerApp.Models;
using BCrypt.Net;

namespace ServerApp.Controllers
{
    public class UserController
    {
        private string connectionString;

        public UserController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Create a new user
        public int AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                string query = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password); SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                connection.Open();
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // Read user by ID
        public User GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Name, Email, Password FROM Users WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    User user = new User
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                    return user;
                }

                return null;
            }
        }

        // Get all users
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Name, Email, Password FROM Users";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                    users.Add(user);
                }
            }

            return users;
        }

        // Update user
        public bool UpdateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Check if password needs to be updated
                User existingUser = GetUserById(user.ID);
                string passwordToSave = user.Password;

                // If the password has changed, hash the new one
                if (existingUser != null && user.Password != existingUser.Password)
                {
                    passwordToSave = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                string query = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ID", user.ID);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        // Delete user
        public bool DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Users WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }
}