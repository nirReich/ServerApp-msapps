using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Text.Json;
using ServerApp.Models;
using ServerApp.Controllers;

namespace ServerApp
{
    public class Server
    {
        private readonly HttpListener listener;
        private readonly string url;
        private readonly UserController userController;
        private bool isRunning = false;

        public Server(string url, string connectionString)
        {
            this.url = url;
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(url);
            this.userController = new UserController(connectionString);
        }

        public void Start()
        {
            if (isRunning)
            {
                Console.WriteLine("Server is already running.");
                return;
            }

            listener.Start();
            isRunning = true;
            Console.WriteLine($"Server started. Listening on {url}");

            // Start listening for requests in a separate task
            Task.Run(() => ListenForRequests());
        }

        public void Stop()
        {
            if (!isRunning)
            {
                Console.WriteLine("Server is not running.");
                return;
            }

            listener.Stop();
            isRunning = false;
            Console.WriteLine("Server stopped.");
        }

        private async Task ListenForRequests()
        {
            while (isRunning)
            {
                try
                {
                    // Wait for incoming request
                    HttpListenerContext context = await listener.GetContextAsync();
                    
                    // Process the request in a separate task to handle multiple requests
                    _ = Task.Run(() => ProcessRequest(context));
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        Console.WriteLine($"Error handling request: {ex.Message}");
                    }
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Get the request URL path
                string path = request.Url.AbsolutePath;
                string method = request.HttpMethod;
                
                Console.WriteLine($"Received {method} request for {path}");

                // Parse the path to determine the action
                string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (segments.Length > 0 && segments[0].ToLower() == "users")
                {
                    HandleUserRequest(segments, method, request, response);
                }
                else
                {
                    // Default route - return simple welcome message
                    string responseString = "Welcome to the Server API. Use /users endpoint to interact with user data.";
                    SendResponse(response, responseString, 200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                SendResponse(context.Response, $"{{\"error\": \"{ex.Message}\"}}", 500, "application/json");
            }
        }

        private void HandleUserRequest(string[] segments, string method, HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                // GET /users - Get all users
                if (segments.Length == 1 && method == "GET")
                {
                    List<User> users = userController.GetAllUsers();
                    string json = JsonSerializer.Serialize(users);
                    SendResponse(response, json, 200, "application/json");
                    return;
                }
                
                // GET /users/{id} - Get user by ID
                if (segments.Length == 2 && method == "GET" && int.TryParse(segments[1], out int userId))
                {
                    User user = userController.GetUserById(userId);
                    
                    if (user != null)
                    {
                        string json = JsonSerializer.Serialize(user);
                        SendResponse(response, json, 200, "application/json");
                    }
                    else
                    {
                        SendResponse(response, "{\"error\": \"User not found\"}", 404, "application/json");
                    }
                    return;
                }
                
                // POST /users - Create new user
                if (segments.Length == 1 && method == "POST")
                {
                    // Read request body
                    string requestBody = ReadRequestBody(request);
                    User newUser = JsonSerializer.Deserialize<User>(requestBody);
                    
                    // Add the new user to the database
                    int newId = userController.AddUser(newUser);
                    
                    // Return the created user with ID
                    newUser.ID = newId;
                    string json = JsonSerializer.Serialize(newUser);
                    SendResponse(response, json, 201, "application/json");
                    return;
                }
                
                // PUT /users/{id} - Update user
                if (segments.Length == 2 && method == "PUT" && int.TryParse(segments[1], out int updateUserId))
                {
                    // Read request body
                    string requestBody = ReadRequestBody(request);
                    User updatedUser = JsonSerializer.Deserialize<User>(requestBody);
                    updatedUser.ID = updateUserId;
                    
                    // Update the user
                    bool success = userController.UpdateUser(updatedUser);
                    
                    if (success)
                    {
                        string json = JsonSerializer.Serialize(updatedUser);
                        SendResponse(response, json, 200, "application/json");
                    }
                    else
                    {
                        SendResponse(response, "{\"error\": \"User not found or update failed\"}", 404, "application/json");
                    }
                    return;
                }
                
                // DELETE /users/{id} - Delete user
                if (segments.Length == 2 && method == "DELETE" && int.TryParse(segments[1], out int deleteUserId))
                {
                    bool success = userController.DeleteUser(deleteUserId);
                    
                    if (success)
                    {
                        SendResponse(response, "{\"message\": \"User deleted successfully\"}", 200, "application/json");
                    }
                    else
                    {
                        SendResponse(response, "{\"error\": \"User not found or delete failed\"}", 404, "application/json");
                    }
                    return;
                }
                
                // If we get here, the route was not found
                SendResponse(response, "{\"error\": \"Invalid endpoint or method\"}", 404, "application/json");
            }
            catch (Exception ex)
            {
                SendResponse(response, $"{{\"error\": \"{ex.Message}\"}}", 500, "application/json");
            }
        }

        private string ReadRequestBody(HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        private void SendResponse(HttpListenerResponse response, string responseString, int statusCode, string contentType = "text/plain")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            
            response.StatusCode = statusCode;
            response.ContentType = contentType;
            response.ContentLength64 = buffer.Length;
            
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}