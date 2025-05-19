using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ServerApp.Models;
using ServerApp.Controllers;

namespace ServerApp.Services
{
    public class UserBatchService
    {
        private readonly string connectionString;
        private readonly UserController userController;
        private Timer batchTimer;

        public UserBatchService(string connectionString)
        {
            this.connectionString = connectionString;
            this.userController = new UserController(connectionString);
        }

        // Schedule the batch service to run at a specific time
        public void ScheduleBatchProcessing(TimeSpan timeOfDay)
        {
            // Calculate initial delay until first run
            DateTime now = DateTime.Now;
            DateTime scheduledTime = DateTime.Today.Add(timeOfDay);
            
            if (scheduledTime < now)
            {
                // If scheduled time is already passed for today, schedule for tomorrow
                scheduledTime = scheduledTime.AddDays(1);
            }
            
            TimeSpan initialDelay = scheduledTime - now;
            
            // Schedule timer to run once a day
            TimeSpan dailyInterval = TimeSpan.FromDays(1);
            
            batchTimer = new Timer(ExecuteBatchProcessing, null, initialDelay, dailyInterval);
            
            Console.WriteLine($"Batch processing scheduled to run at {timeOfDay} every day.");
            Console.WriteLine($"First run will be in {initialDelay.Hours} hours and {initialDelay.Minutes} minutes.");
        }

        // Process that executes when timer fires
        private void ExecuteBatchProcessing(object state)
        {
            Console.WriteLine($"Batch processing started at {DateTime.Now}");
            
            try
            {
                ProcessUserBatch();
                Console.WriteLine("Batch processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during batch processing: {ex.Message}");
            }
        }

        // Manual execution of batch process
        public void RunBatchProcessingNow()
        {
            Console.WriteLine($"Manual batch processing started at {DateTime.Now}");
            
            try
            {
                ProcessUserBatch();
                Console.WriteLine("Manual batch processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during manual batch processing: {ex.Message}");
            }
        }

        // Core batch processing logic
        private void ProcessUserBatch()
        {
            // Get all users from the database
            List<User> users = userController.GetAllUsers();
            Console.WriteLine($"Processing batch for {users.Count} users");
            
            foreach (User user in users)
            {
                // Simulate sending an email
                bool emailSent = SimulateSendEmail(user);
                
                if (emailSent)
                {
                    // Update user record to indicate email was sent
                    UpdateUserEmailStatus(user.ID);
                    Console.WriteLine($"Email sent to {user.Name} ({user.Email})");
                }
                else
                {
                    Console.WriteLine($"Failed to send email to {user.Name} ({user.Email})");
                }
                
                // Add small delay to prevent overwhelming resources
                Thread.Sleep(100);
            }
        }

        // Simulate sending an email (this would be replaced with actual email sending code)
        private bool SimulateSendEmail(User user)
        {
            // Simulate some failures randomly (10% failure rate)
            Random random = new Random();
            return random.Next(10) > 0; // 90% success rate
        }

        // Update user record to indicate email was sent
        private void UpdateUserEmailStatus(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET LastEmailSent = @CurrentTime WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                
                command.Parameters.AddWithValue("@ID", userId);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.Now);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Clean up resources
        public void Dispose()
        {
            batchTimer?.Dispose();
        }
    }
}