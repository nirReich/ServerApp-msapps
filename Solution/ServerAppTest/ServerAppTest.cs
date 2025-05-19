using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServerApp.Models;

namespace ServerAppTest
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string serverUrl = "http://localhost:8080";
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Server Test Application");
            Console.WriteLine("======================");
            
            try
            {
                // Wait for server to start
                Console.WriteLine("Waiting for server to start...");
                await Task.Delay(2000);
                
                // Test adding a new user
                Console.WriteLine("\nTesting: Add new user");
                User newUser = new User
                {
                    Name = "Test User",
                    Email = "test@example.com",
                    Password = "testpass123"
                };
                User createdUser = await AddUserAsync(newUser);
                Console.WriteLine($"User created successfully with ID: {createdUser.ID}");
                
                // Test getting all users
                Console.WriteLine("\nTesting: Get all users");
                User[] allUsers = await GetAllUsersAsync();
                Console.WriteLine($"Retrieved {allUsers.Length} users:");
                foreach (var user in allUsers)
                {
                    Console.WriteLine($"- {user.ID}: {user.Name} ({user.Email})");
                }
                
                // Test getting a specific user
                Console.WriteLine($"\nTesting: Get user by ID {createdUser.ID}");
                User retrievedUser = await GetUserByIdAsync(createdUser.ID);
                Console.WriteLine($"Retrieved user: {retrievedUser.Name} ({retrievedUser.Email})");
                
                // Test updating a user
                Console.WriteLine($"\nTesting: Update user {createdUser.ID}");
                retrievedUser.Name = "Updated Test User";
                retrievedUser.Email = "updated@example.com";
                User updatedUser = await UpdateUserAsync(retrievedUser);
                Console.WriteLine($"User updated successfully: {updatedUser.Name} ({updatedUser.Email})");
                
                // Test deleting a user
                Console.WriteLine($"\nTesting: Delete user {createdUser.ID}");
                bool deleteResult = await DeleteUserAsync(createdUser.ID);
                Console.WriteLine($"User deleted successfully: {deleteResult}");
                
                // Verify user was deleted
                Console.WriteLine("\nVerifying user was deleted by getting all users again");
                User[] remainingUsers = await GetAllUsersAsync();
                Console.WriteLine($"Retrieved {remainingUsers.Length} users:");
                foreach (var user in remainingUsers)
                {
                    Console.WriteLine($"- {user.ID}: {user.Name} ({user.Email})");
                }
                
                Console.WriteLine("\nAll tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        // Add new user
        static async Task<User> AddUserAsync(User user)
        {
            string json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PostAsync($"{serverUrl}/users", content);
            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseBody);
        }
        
        // Get all users
        static async Task<User[]> GetAllUsersAsync()
        {
            HttpResponseMessage response = await client.GetAsync($"{serverUrl}/users");
            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User[]>(responseBody);
        }
        
        // Get user by ID
        static async Task<User> GetUserByIdAsync(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{serverUrl}/users/{id}");
            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseBody);
        }
        
        // Update user
        static async Task<User> UpdateUserAsync(User user)
        {
            string json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PutAsync($"{serverUrl}/users/{user.ID}", content);
            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseBody);
        }
        
        // Delete user
        static async Task<bool> DeleteUserAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{serverUrl}/users/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}