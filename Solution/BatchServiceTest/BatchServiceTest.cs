using System;
using ServerApp.Services;

namespace BatchServiceTest
{
    class Program
    {
        private static readonly string connectionString = "Data Source=localhost;Initial Catalog=MyDatabase;Integrated Security=True";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Batch Service Test Application");
            Console.WriteLine("==============================");
            
            try
            {
                // Create batch service
                UserBatchService batchService = new UserBatchService(connectionString);
                
                // Run batch service manually
                Console.WriteLine("Running batch service manually...");
                batchService.RunBatchProcessingNow();
                
                // Schedule batch service for future execution (for testing purposes, schedule for 30 seconds from now)
                Console.WriteLine("\nScheduling batch service to run in 30 seconds...");
                TimeSpan scheduleTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(30));
                batchService.ScheduleBatchProcessing(scheduleTime);
                
                Console.WriteLine("Waiting for scheduled batch service to run...");
                Console.WriteLine("Press any key to exit before scheduled run...");
                
                // Wait for key or timeout (whichever comes first)
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Console.WriteLine("\nTest completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}